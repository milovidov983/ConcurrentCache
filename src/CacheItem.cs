using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentCache {
	internal struct CacheItem<TValue> {
		public TValue Payload { get; }

		private readonly DateTime storedAt;
		private readonly TimeSpan? lifeTime;

		public CacheItem(TValue payload, TimeSpan? cacheItemLifeTime) {
			Payload = payload;
			this.lifeTime = cacheItemLifeTime;
			this.storedAt = DateTime.UtcNow;
		}


		public bool IsExpired() {
			return lifeTime is not null && storedAt < (DateTime.UtcNow - lifeTime);
		}
	}

	internal class ThreadWaitObject {
		public TaskCompletionSource<object> Up { get; set; } = new TaskCompletionSource<object>();
		public TaskCompletionSource<object> Down { get; set; } = new TaskCompletionSource<object>();
	}
}
