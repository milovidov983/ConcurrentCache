# ConcurrentCache - easy-to-use, thread-safe local cache library for dotnet


## Overview

An easy-to-use, thread-safe local cache library for dotnet. It is based on the implementation recommended by Microsoft Microsoft Microsoft.Extensions.Caching.Memory.



### Examples

```csharp

// Create instance
ICache<string, int> localCache = new Cache<string, int>();

var key = "42number";

// Get or add value from cache by key  "42number"
var result = await cache.GetOrAdd(key, () => Task.FromResult(42));


```