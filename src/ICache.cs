using System;
using System.Threading.Tasks;

namespace ConcurrentCache {
	public interface ICache<TKey, TValue> {
		/// <summary>
		/// Main cache interface
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="onMissing">Executed when key not found</param>
		/// <param name="expired">Storage duration, if null then infinity</param>
		/// <returns>Result value</returns>
		Task<TValue> GetOrAdd(TKey key, Func<Task<TValue>> onMissing, TimeSpan? expired = null);

		/// <summary>
		/// Remove value from the cache
		/// </summary>
		/// <param name="key">cache key</param>
		void Delete(TKey key);
	}
}
