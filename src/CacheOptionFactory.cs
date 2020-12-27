using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;

namespace ConcurrentCache {
	internal class CacheOptionFactory {
		private readonly ConcurrentDictionary<TimeSpan, MemoryCacheEntryOptions> cacheOptions
			= new ConcurrentDictionary<TimeSpan, MemoryCacheEntryOptions>();
		private readonly MemoryCacheEntryOptions defaultOptions = new MemoryCacheEntryOptions();

		public MemoryCacheEntryOptions GetCacheEntryOptions(TimeSpan absoluteExpiration) {
			if (absoluteExpiration == TimeSpan.MaxValue) {
				return defaultOptions;
			}
			var isCached = cacheOptions.TryGetValue(absoluteExpiration, out var options);
			if (isCached) {
				return options;
			}

			var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration);

			cacheOptions.AddOrUpdate(
				absoluteExpiration, 
				cacheEntryOptions, 
				(_, oldValue) => oldValue);

			return cacheEntryOptions;
		}
	}
}
