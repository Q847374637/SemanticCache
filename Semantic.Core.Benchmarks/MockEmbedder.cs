using Semantic.Core;

namespace Semantic.Benchmarks;

/// <summary>
/// A mock embedder used for benchmarking without network latency.
/// Pre-generates a fixed vector to avoid allocations during measurement.
/// </summary>
public class MockEmbedder : IEmbedder
{
    private readonly float[] _cachedVector;

    /// <inheritdoc/>
    public bool ReturnsNormalizedVectors => true;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockEmbedder"/> class and pre-generates a random vector.
    /// </summary>
    public MockEmbedder()
    {
        _cachedVector = new float[1536];
        var random = new Random(42); // Seeded for consistent results
        
        for (int i = 0; i < 1536; i++)
        {
            // Generate a random float between -1 and 1
            _cachedVector[i] = (float)(random.NextDouble() * 2.0 - 1.0);
        }
    }

    /// <inheritdoc/>
    public Task<float[]> GenerateVectorAsync(string text, CancellationToken cancellationToken = default)
    {
        // Return the cached vector immediately to bypass network calls and allocations
        return Task.FromResult(_cachedVector);
    }
}