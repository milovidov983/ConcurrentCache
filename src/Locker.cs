using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCache {

	internal class Locker<TKey> {
		private const int DEFAULT_LIMIT = 16384;
		private const int DEFAULT_MIN_LIMIT = 1024;


		private readonly ConcurrentDictionary<TKey, SemaphoreSlim> locks = new ConcurrentDictionary<TKey, SemaphoreSlim>();
		private readonly SemaphoreSlim globalSemafore = new SemaphoreSlim(1);

		private int lockObjectLimit = DEFAULT_LIMIT;
		private bool garbageCollectionOn = false;

		public void SetLimit(int limit) {
			lockObjectLimit = limit < DEFAULT_MIN_LIMIT ? DEFAULT_MIN_LIMIT : lockObjectLimit;
		}

		public async Task WaitAsync(TKey key) {
			if (garbageCollectionOn) {
				if (locks.ContainsKey(key)) {
					await globalSemafore.WaitAsync();
					globalSemafore.Release();
				}
			}
			var s = locks.GetOrAdd(key, k => new SemaphoreSlim(1));
			await s.WaitAsync();
		}
		public void Release(TKey key) {
			var s = locks.GetOrAdd(key, k => new SemaphoreSlim(1));
			s.Release();

			if (locks.Count < lockObjectLimit) {
				return;
			}
			Task.Run(async ()=> await StartGC());
		}

		private async Task StartGC() {
			try {
				await globalSemafore.WaitAsync();
				garbageCollectionOn = true;
				int counter = 0;
				int limit = locks.Count / 2;
				foreach (var key in locks.Keys) {
					if(locks.TryGetValue(key, out var semaphoreSlim)) {
						if(semaphoreSlim.CurrentCount == 0) {
							locks.TryRemove(key, out _);
						}
					}
					if(counter++ > limit) {
						break;
					}
				}

			} finally {
				garbageCollectionOn = false;
				globalSemafore.Release();
			}

		}
	}
}
