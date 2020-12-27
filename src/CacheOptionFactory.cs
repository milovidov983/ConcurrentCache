using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;

namespace ConcurrentCache {
	internal class CacheOptionFactory {
		private readonly ConcurrentDictionary<TimeSpan, MemoryCacheEntryOptions> cacheOptions
			= new ConcurrentDictionary<TimeSpan, MemoryCacheEntryOptions>();
		private readonly MemoryCacheEntryOptions defaultOptions;

		public MemoryCacheEntryOptions GetCacheEntryOptions(TimeSpan? absoluteExpiration) {
			if (absoluteExpiration is null) {
				return defaultOptions;
			}
			var isCached = cacheOptions.TryGetValue(absoluteExpiration.Value, out var options);
			if (isCached) {
				return options;
			}

			var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration.Value);

			cacheOptions.AddOrUpdate(
				absoluteExpiration.Value, 
				cacheEntryOptions, 
				(_, oldValue) => oldValue);

			return cacheEntryOptions;
		}
	}
}
