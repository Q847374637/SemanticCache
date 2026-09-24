# SemanticCache.Core

A high-performance semantic caching library for .NET designed for RAG systems and AI agents. It provides extremely fast local memory searches for similar queries to reduce API costs and latency.

## Features

* Hardware-accelerated cosine similarity math using SIMD via `System.Numerics.Tensors`
* Thread-safe concurrent access designed for high-load web APIs using `ReaderWriterLockSlim`
* Strict memory footprint control via an internal Least Recently Used (LRU) eviction algorithm
* Lightweight core architecture without heavy transitive dependencies

## Use Cases vs Vector Databases

SemanticCache.Core is not a replacement for distributed vector databases like Milvus or Qdrant. It is designed as an ultra-fast, L1 in-memory caching layer that sits in front of your LLM calls. 

It solves specific architectural challenges:
* Eliminates LLM API latency for repetitive queries
* Reduces token costs by intercepting frequent identical or semantically similar requests
* Operates locally without network overhead

## Performance & Benchmarks

The core is built on Hardcore Software Engineering principles. We utilize `System.Numerics.Tensors` for SIMD-accelerated cosine similarity calculations and `ReaderWriterLockSlim` for dense multi-threaded access. 

Formal `BenchmarkDotNet` metrics run on .NET 9.0 demonstrate linear time complexity O(n) with fixed, minimal memory allocations (targeting zero-allocation reads), regardless of the cache size.

| Method | Cache Size | Mean Time | Memory Allocated |
| :--- | :--- | :--- | :--- |
| **CacheHit** | 100 | 14.33 μs | 288 B |
| **CacheMiss** | 100 | 15.84 μs | 352 B |
| **CacheHit** | 1,000 | 228.44 μs | 288 B |
| **CacheMiss** | 1,000 | 226.33 μs | 352 B |
| **CacheHit** | 10,000 | 3,489.17 μs | 288 B |
| **CacheMiss** | 10,000 | 3,676.67 μs | 352 B |

*(1 μs = 0.001 milliseconds)*

## Installation

Install the core package alongside the embedding provider of your choice:

```bash
dotnet add package SemanticCache.Core
dotnet add package SemanticCache.OpenAI
# or dotnet add package SemanticCache.Ollama
```

## Dependency Injection (Recommended for Web APIs)

```bash
using Semantic.Core;
using Semantic.OpenAI;
// using Semantic.Ollama; // For local models

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SemanticCacheOptions>(options =>
{
    options.MaxItems = 1000;
    options.SimilarityThreshold = 0.85f;
});

// Register cache and provider
builder.Services.AddSemanticCache()
                .WithOpenAiEmbedder("sk-your-api-key", "text-embedding-3-small");

var app = builder.Build();

app.MapGet("/rag/query", async (string prompt, ISemanticCache cache, CancellationToken token) =>
{
    var response = await cache.GetOrGenerateAsync(prompt, async (cancellationToken) => 
    {
        // Fallback: Execute your LLM API call here on cache miss
        return await MyLlmService.GenerateResponseAsync(prompt, cancellationToken);
    }, token);

    return Results.Ok(response);
});

app.Run();
```

## Manual Instantiation (Low-level storage access)

```bash
using Semantic.Core;
using Semantic.Core.Storage;

var options = new SemanticCacheOptions { MaxItems = 1000 };
var storage = new InMemoryVectorStorage(options);

float[] vector = { 0.1f, 0.5f, 0.4f };
await storage.SaveAsync("What is the capital of France?", vector, "Paris");

string? result = await storage.FindSimilarAsync(
    incomingVector: vector, 
    similarityThreshold: 0.85f, 
    isNormalized: false
);

if (result != null)
{
    Console.WriteLine($"Cache hit - {result}");
}
```

## Extensibility
The core library is designed to remain provider-agnostic. It seamlessly integrates with custom embedding providers, including OpenAI and local models.

## License
MIT