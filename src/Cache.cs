using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCache {

	public class Cache<TKey, TValue> : ICache<TKey, TValue> {
		private readonly MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
		private readonly CacheOptionFactory optionFactory = new CacheOptionFactory();
		private readonly Locker<TKey> locker = new Locker<TKey>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public async Task<TValue> Delete(TKey key) {
			if (cache.TryGetValue(key, out TValue cacheEntry)) {
				

				await locker.WaitAsync(key);
				try {
					if (cache.TryGetValue(key, out cacheEntry)) {

						cache.Remove(key);
						return cacheEntry;
					}
				} finally {
					locker.Release(key);

				}
			}
			return cacheEntry;

		}

		public async Task<TValue> GetOrAdd(TKey key, Func<Task<TValue>> createItem) {
			return await GetOrAdd(key, createItem, TimeSpan.MaxValue);
		}

		/// <summary>
		/// Get and/or put data in the cache
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="onMissing">Executed when key not found</param>
		/// <param name="expired">Storage duration, if null then infinity</param>
		/// <returns>Result value</returns>
		public async Task<TValue> GetOrAdd(TKey key, Func<Task<TValue>> createItem, TimeSpan expired){
			if (!cache.TryGetValue(key, out TValue cacheEntry))
			{
				await locker.WaitAsync(key);
				try {
					if (!cache.TryGetValue(key, out cacheEntry)) {

						cacheEntry = await createItem();
						var options = optionFactory.GetCacheEntryOptions(expired);
						cache.Set(key, cacheEntry, options);
					}
				} finally {
					locker.Release(key);
				}
			}
			return cacheEntry;

		}

	}
}
