using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCache {

	public class Locker<TKey> {
		private const int LOCK_OBJECT_LIMIT = 1024;
		private readonly ConcurrentDictionary<TKey, SemaphoreSlim> locks = new ConcurrentDictionary<TKey, SemaphoreSlim>();
		private readonly SemaphoreSlim globalSemafore = new SemaphoreSlim(1);
		private bool garbageCollectionOn = false;


		public async Task WaitAsync(TKey key) {
			if (garbageCollectionOn) {
				await globalSemafore.WaitAsync();
				globalSemafore.Release();
			}
			var s = locks.GetOrAdd(key, k => new SemaphoreSlim(1));
			await s.WaitAsync();
		}
		public void Release(TKey key) {
			var s = locks.GetOrAdd(key, k => new SemaphoreSlim(1));
			s.Release();

			if (locks.Count < LOCK_OBJECT_LIMIT) {
				return;
			}
			Task.Run(async ()=> await StartGC());
		}

		private async Task StartGC() {
			try {
				await globalSemafore.WaitAsync();
				garbageCollectionOn = true;
				foreach (var key in locks.Keys) {
					if(locks.TryGetValue(key, out var semaphoreSlim)) {
						if(semaphoreSlim.CurrentCount == 0) {
							locks.TryRemove(key, out _);
						}
					}
				}

			} finally {
				garbageCollectionOn = false;
				globalSemafore.Release();
			}

		}
	}
}
