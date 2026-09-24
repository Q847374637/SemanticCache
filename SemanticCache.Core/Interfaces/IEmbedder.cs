namespace Semantic.Core
{
    /// <summary>
    /// Defines a contract for generating dense vector representations (embeddings) from text.
    /// </summary>
    public interface IEmbedder
    {
        /// <summary>
        /// Generates a vector embedding for the specified text.
        /// </summary>
        /// <param name="text">The input text to embed.</param>
        /// <param name="cancellationToken">A token to cancel the HTTP request to the embedding provider.</param>
        /// <returns>A float array representing the multidimensional vector.</returns>
        Task<float[]> GenerateVectorAsync(string text, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a value indicating whether the generated vectors are already L2-normalized by the provider.
        /// </summary>
        /// <remarks>
        /// If true, the cache can skip vector normalization during cosine similarity calculations, significantly improving performance.
        /// </remarks>
        bool ReturnsNormalizedVectors { get; }
    }
}