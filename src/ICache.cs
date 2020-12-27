using System;
using System.Threading.Tasks;

namespace ConcurrentCache {
	public interface ICache<TKey, TValue> {
		/// <summary>
		/// Get a value from the cache, or put and get if it is not there
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="createItem">Executed when key not found</param>
		/// <returns>Result value</returns>
		Task<TValue> GetOrAdd(TKey key, Func<Task<TValue>> createItem);

		/// <summary>
		/// Get a value from the cache, or put and get if it is not there
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="createItem">Executed when key not found</param>
		/// <param name="expired">Storage duration</param>
		/// <returns>Result value</returns>
		Task<TValue> GetOrAdd(TKey key, Func<Task<TValue>> createItem, TimeSpan expired);

		/// <summary>
		/// Remove value from the cache
		/// </summary>
		/// <param name="key">cache key</param>
		void Delete(TKey key);
	}
}
