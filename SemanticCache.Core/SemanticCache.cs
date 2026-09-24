using Microsoft.Extensions.Options;

namespace Semantic.Core
{
    /// <inheritdoc cref="ISemanticCache"/>
    public class SemanticCache : ISemanticCache
    {
        private readonly IEmbedder _embedder;
        private readonly IVectorStorage _vectorStorage;
        private readonly float _similiarityThreshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticCache"/> class.
        /// </summary>
        /// <param name="embedder">The embedding provider to convert text to vectors.</param>
        /// <param name="vectorStorage">The underlying storage engine.</param>
        /// <param name="options">The configured threshold and memory settings.</param>
        public SemanticCache(IEmbedder embedder, IVectorStorage vectorStorage, IOptions<SemanticCacheOptions> options)
        {
            _embedder = embedder;
            _vectorStorage = vectorStorage;
            _similiarityThreshold = options.Value.SimilarityThreshold;
        }

        /// <inheritdoc/>
        public async Task<string> GetOrGenerateAsync(string request, Func<CancellationToken, Task<string>> generateResponse, CancellationToken cancellationToken = default)
        {
            float[] vector = await _embedder.GenerateVectorAsync(request, cancellationToken);
            string? cachedResponse = await _vectorStorage.FindSimilarAsync(vector, _similiarityThreshold, _embedder.ReturnsNormalizedVectors, cancellationToken);

            if (cachedResponse != null)
            {
                return cachedResponse;
            }

            string response = await generateResponse(cancellationToken);
            await _vectorStorage.SaveAsync(request, vector, response, cancellationToken);
            return response;
        }
    }
}