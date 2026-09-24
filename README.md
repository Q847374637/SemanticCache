# SemanticCache.Core <img src="assets/logo.png" width="64" height="64" alt="Logo" />

A high-performance semantic caching library for .NET designed for RAG systems and LLM integrations. It provides extremely fast local memory searches for similar queries to reduce API costs and latency.

## Features

* Hardware-accelerated cosine similarity math using SIMD via `System.Numerics.Tensors`
* Thread-safe concurrent access designed for high-load web APIs using `ReaderWriterLockSlim`
* Strict memory footprint control via an internal Least Recently Used (LRU) eviction algorithm
* Lightweight core architecture without heavy transitive dependencies

## Use Cases vs Vector Databases

SemanticCache.Core is not a replacement for distributed vector databases like Milvus or Qdrant. It is designed as an ultra-fast, L1 in-memory caching layer that sits in front of your LLM calls. 

It solves specific architectural challenges
* Eliminates LLM API latency for repetitive queries
* Reduces token costs by intercepting frequent identical or semantically similar requests
* Operates locally without network overhead

## Performance & Benchmarks (Coming Soon)

The core is built on Hardcore Software Engineering principles. We utilize `System.Numerics.Tensors` for SIMD-accelerated cosine similarity calculations and `ReaderWriterLockSlim` for dense multi-threaded access. 

Formal `BenchmarkDotNet` metrics detailing exact search latency in nanoseconds and memory allocation (targeting zero-allocation reads) will be published in the upcoming release alongside the high-level Provider API.

## Installation

```bash
dotnet add package SemanticCache.Core

using Semantic.Core;
using Semantic.Core.Storage;

var options = new SemanticCacheOptions 
{ 
    MaxItems = 1000 
};

var cache = new InMemoryVectorStorage(options);

float[] vector = { 0.1f, 0.5f, 0.4f };
await cache.SaveAsync("What is the capital of France?", vector, "Paris");

string? result = await cache.FindSimilarAsync(
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