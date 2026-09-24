using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Options;
using Semantic.Core;

namespace Semantic.Benchmarks;

/// <summary>
/// Benchmarks for measuring the performance and memory allocations of the semantic cache.
/// </summary>
[MemoryDiagnoser]
public class CacheBenchmarks
{
    private SemanticCache _semanticCache = null!;
    private readonly string _targetRequest = "target_request_text";
    private int _missCounter;

    /// <summary>
    /// Gets or sets the number of items to pre-load into the cache.
    /// </summary>
    [Params(100, 1000, 10000)]
    public int CacheSize { get; set; }

    /// <summary>
    /// Initializes the cache and pre-loads it with random data before each benchmark run.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        var options = Options.Create(new SemanticCacheOptions 
        { 
            MaxItems = CacheSize + 1,
            SimilarityThreshold = 0.90f
        });
        
        var storage = new InMemoryVectorStorage(options);
        var embedder = new MockEmbedder();
        var random = new Random(42);

        // Pre-fill the cache with random vectors
        for (int i = 0; i < CacheSize; i++)
        {
            var dummyVector = new float[1536];
            for (int j = 0; j < 1536; j++)
            {
                dummyVector[j] = (float)(random.NextDouble() * 2.0 - 1.0);
            }
            storage.SaveAsync($"req_{i}", dummyVector, $"res_{i}").GetAwaiter().GetResult();
        }

        // Generate and save the specific target vector to guarantee a cache hit
        var targetVector = new float[1536];
        for (int j = 0; j < 1536; j++)
        {
            targetVector[j] = (float)(random.NextDouble() * 2.0 - 1.0);
        }
        storage.SaveAsync(_targetRequest, targetVector, "target_response").GetAwaiter().GetResult();

        _semanticCache = new SemanticCache(embedder, storage, options);
        _missCounter = 0;
    }

    /// <summary>
    /// Measures the performance of a cache hit, expecting minimal latency and zero allocations.
    /// </summary>
    [Benchmark]
    public async Task<string> CacheHit()
    {
        return await _semanticCache.GetOrGenerateAsync(
            _targetRequest, 
            async (token) => await Task.FromResult("fallback_should_not_be_called")
        );
    }

    /// <summary>
    /// Measures the performance of a cache miss, including vector generation and storage allocation.
    /// </summary>
    [Benchmark]
    public async Task<string> CacheMiss()
    {
        // Increment counter to ensure a completely unique request on every invocation
        string uniqueRequest = $"miss_request_{Interlocked.Increment(ref _missCounter)}";
        
        return await _semanticCache.GetOrGenerateAsync(
            uniqueRequest, 
            async (token) => await Task.FromResult("new_generated_response")
        );
    }
}