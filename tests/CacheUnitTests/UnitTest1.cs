using ConcurrentCache;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CacheUnitTests {
	public class UnitTest1 {
		private ICache<string, int> cache = new Cache<string, int>();
		private const string Key = "someKey";

		[Fact]
		public async Task Non_default_return() {
			var count = 0;

			count++;

			var result = await cache.GetOrAdd("number", () => Task.FromResult(count));
			result.ShouldBe(1);

			count += 999;
			result = await cache.GetOrAdd("number", () => Task.FromResult(count));
			result.ShouldBe(1);
		}
	}
}
