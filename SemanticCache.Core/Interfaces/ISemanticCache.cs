namespace Semantic.Core
{
    /// <summary>
    /// Represents the main orchestrator for the semantic cache, coordinating between the embedder and the vector storage.
    /// </summary>
    public interface ISemanticCache
    {
        /// <summary>
        /// Retrieves a cached response if a semantically similar request exists; otherwise, generates a new response, caches it, and returns it.
        /// </summary>
        /// <param name="request">The incoming text request from the user.</param>
        /// <param name="generateResponse">A delegate that calls the underlying LLM to generate a response if a cache miss occurs.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>The cached or newly generated response string.</returns>
        Task<string> GetOrGenerateAsync(string request, Func<CancellationToken, Task<string>> generateResponse, CancellationToken cancellationToken = default);
    }
}