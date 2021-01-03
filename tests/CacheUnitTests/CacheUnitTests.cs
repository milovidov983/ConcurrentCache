using ConcurrentCache;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CacheUnitTests {
	public class CacheUnitTests {
		private ICache<string, int> cache = new Cache<string, int>();

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

		[Fact]
		public async Task Delete_function_removes_elements() {
			var count = 0;

			count++;

			await cache.GetOrAdd("number", () => Task.FromResult(count));
			await cache.Delete("number");

			count++;
			var result = await cache.GetOrAdd("number", () => Task.FromResult(count));

			result.ShouldBe(2);
		}

		[Fact]
		public async Task Set_10000_objects() {
			var count = 10000;

			System.Diagnostics.Debug.WriteLine("Stage I");
			var sw = System.Diagnostics.Stopwatch.StartNew();
			for(int i = 0; i < count; i++) {
				if(i % 1000 == 0) {
					sw.Stop();
					System.Diagnostics.Debug.WriteLine($"{sw.ElapsedMilliseconds} ms");
					sw.Reset();
					sw.Restart();
				}
				await cache.GetOrAdd($"number-{i}", () => Task.FromResult(i));
			}

			System.Diagnostics.Debug.WriteLine("Stage II");
			for (int i = 0; i < count; i++) {
				if (i % 1000 == 0) {
					sw.Stop();
					System.Diagnostics.Debug.WriteLine($"{sw.ElapsedMilliseconds} ms");
					sw.Reset();
					sw.Restart();
				}
				await cache.GetOrAdd($"number-{i}", () => Task.FromResult(i));
			}
		}
	}
}

