namespace Semantic.Core
{
    /// <summary>
    /// Defines a contract for storing and querying multidimensional vectors.
    /// </summary>
    public interface IVectorStorage
    {
        /// <summary>
        /// Saves a text request, its corresponding vector, and the LLM response into the storage.
        /// </summary>
        /// <param name="request">The original user prompt.</param>
        /// <param name="vector">The generated embedding for the prompt.</param>
        /// <param name="response">The generated LLM response to cache.</param>
        /// <param name="cancellationToken">A token to cancel the save operation.</param>
        Task SaveAsync(string request, float[] vector, string response, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches the storage for a vector that matches the incoming vector above the specified similarity threshold.
        /// </summary>
        /// <param name="vector">The incoming vector to query against the cache.</param>
        /// <param name="similarityThreshold">The minimum cosine similarity score (0.0 to 1.0) required for a cache hit.</param>
        /// <param name="isNormalized">Indicates whether the incoming vector is pre-normalized.</param>
        /// <param name="cancellationToken">A token to abort the search loop.</param>
        /// <returns>The cached string response if a match is found; otherwise, null.</returns>
        Task<string?> FindSimilarAsync(float[] vector, float similarityThreshold, bool isNormalized, CancellationToken cancellationToken = default);
    }
}