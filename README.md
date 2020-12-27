# ConcurrentCache - An easy-to-use, thread-safe local cache library for dotnet
__Package__ - [ConcurrentLocalCache](https://www.nuget.org/packages/ConcurrentLocalCache/)

## Overview

An easy-to-use, thread-safe local cache library for dotnet. It is based on the implementation recommended by Microsoft `Microsoft.Extensions.Caching.Memory`.

---

## Super simple to use

```csharp

// Create instance
ICache<string, int> localCache = new Cache<string, int>();

var key = "42number";

// Get or add value from cache by key  "42number"
var result = await cache.GetOrAdd(key, () => Task.FromResult(42));


```
