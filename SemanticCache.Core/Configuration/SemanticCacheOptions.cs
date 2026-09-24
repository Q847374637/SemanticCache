namespace Semantic.Core
{
    /// <summary>
    /// Configuration options for tuning the behavior and memory limits of the semantic cache.
    /// </summary>
    public class SemanticCacheOptions
    {
        /// <summary>
        /// Gets or sets the minimum cosine similarity score required to trigger a cache hit.
        /// </summary>
        /// <remarks>
        /// A value of 1.0 requires an exact match. A lower value (e.g., 0.85) allows for semantic variations. Default is 0.85f.
        /// </remarks>
        public float SimilarityThreshold { get; set; } = 0.85f;

        /// <summary>
        /// Gets or sets the maximum number of items the cache will hold before evicting older entries.
        /// </summary>
        /// <remarks>
        /// Used by the in-memory storage to prevent OutOfMemory exceptions. Default is 1000.
        /// </remarks>
        public int MaxItems { get; set; } = 1000;
    }
}