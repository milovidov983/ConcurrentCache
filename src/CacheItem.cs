﻿using System;
using System.Collections.Generic;
using System.Text;

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
			return storedAt < (DateTime.UtcNow - lifeTime);
		}
	}
}
