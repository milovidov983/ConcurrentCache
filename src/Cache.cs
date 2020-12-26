using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ConcurrentCache {

	public class Cache<TKey, TValue> : ICache<TKey, TValue> {
		private readonly ConcurrentDictionary<TKey, DateTime> expiredTimeStore = new ConcurrentDictionary<TKey, DateTime>();
		private readonly ConcurrentDictionary<TKey, TValue> mainStore = new ConcurrentDictionary<TKey, TValue>();

		/// <summary>
		/// Get and/or put data in the cache
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="onMissing">Executed when key not found</param>
		/// <param name="expired">Storage duration, if null then infinity</param>
		/// <returns>Result value</returns>
		public async Task<TValue> GetOrAdd(TKey key, Func<Task<TValue>> onMissing, TimeSpan? expired = null){
			var isCached = mainStore.ContainsKey(key);
			if (isCached && expired is null || isCached && expiredTimeStore[key] > (DateTime.UtcNow - expired)) {
				return mainStore[key];
			}

			var now = DateTime.UtcNow;
			expiredTimeStore.AddOrUpdate(key, now, (_, oldValue) => now);


			var result = await onMissing();
			mainStore.AddOrUpdate(key, result, (_, oldValue) =>  result);

			return result;
		}
	}
}
