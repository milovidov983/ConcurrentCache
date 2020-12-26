using System;
using System.Threading.Tasks;

namespace ConcurrentCache {
	public interface ICache<TKey, TValue> {
		/// <summary>
		/// Main cache interface
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="onMissing">Executed when key not found</param>
		/// <param name="expired">Storage duration</param>
		/// <returns>Result value</returns>
		Task<TValue> GetOrAdd(TKey key, Func<Task<TValue>> onMissing, TimeSpan expired);
	}
}
