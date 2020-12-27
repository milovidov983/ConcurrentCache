using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCache {

	public class Cache<TKey, TValue> : ICache<TKey, TValue> {
		private readonly MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
		private readonly ConcurrentDictionary<TKey, SemaphoreSlim> locks = new ConcurrentDictionary<TKey, SemaphoreSlim>();
		private readonly CacheOptionFactory optionFactory = new CacheOptionFactory();

		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public void Delete(TKey key) {
			cache.Remove(key);
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
				SemaphoreSlim mylock = locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));

				await mylock.WaitAsync();
				try {
					if (!cache.TryGetValue(key, out cacheEntry)) {

						cacheEntry = await createItem();
						var options = optionFactory.GetCacheEntryOptions(expired);
						cache.Set(key, cacheEntry, options);
					}
				} finally {
					mylock.Release();
				}
			}
			return cacheEntry;

		}

	}
}
