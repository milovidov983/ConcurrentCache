using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ConcurrentCache {

	public class Cache<TKey, TValue> : ICache<TKey, TValue> {
		private readonly ConcurrentDictionary<TKey, CacheItem<TValue>> store
			= new ConcurrentDictionary<TKey, CacheItem<TValue>>();
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public void Delete(TKey key) {
			store.TryRemove(key, out var _);
		}

		/// <summary>
		/// Get and/or put data in the cache
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="onMissing">Executed when key not found</param>
		/// <param name="expired">Storage duration, if null then infinity</param>
		/// <returns>Result value</returns>
		public async Task<TValue> GetOrAdd(TKey key, Func<Task<TValue>> onMissing, TimeSpan? expired = null){
			var isCached = store.TryGetValue(key, out var cacheItem);
			if (isCached && !cacheItem.IsExpired()) {
				return store[key].Payload;
			}


			var resultPayload = await onMissing();
			var newCacheItem = new CacheItem<TValue>(resultPayload, expired);
			store.AddOrUpdate(key, newCacheItem, (_, oldValue) => newCacheItem);

			return resultPayload;
		}		
		
		
	}
}
