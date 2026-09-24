using OpenAI.Embeddings;
using Semantic.Core;

namespace Semantic.OpenAI
{
    /// <summary>
    /// Provides an implementation of <see cref="IEmbedder"/> that uses the official OpenAI API to generate text embeddings.
    /// </summary>
    public class OpenAIEmbedder : IEmbedder
    {
        private readonly EmbeddingClient _client;

        /// <inheritdoc/>
        public bool ReturnsNormalizedVectors => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIEmbedder"/> class.
        /// </summary>
        /// <param name="apiKey">The API key for authenticating with OpenAI.</param>
        /// <param name="modelId">The ID of the embedding model to use. Defaults to "text-embedding-3-small".</param>
        /// <exception cref="ArgumentException">Thrown when the API key or model ID is null or whitespace.</exception>
        public OpenAIEmbedder(string apiKey, string modelId = "text-embedding-3-small")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modelId);
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

            _client = new EmbeddingClient(modelId, apiKey);
        }

        /// <inheritdoc/>
        public async Task<float[]> GenerateVectorAsync(string text, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);
            var requestedVector = await _client.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);
            return requestedVector.Value.ToFloats().ToArray();
        }
    }
}