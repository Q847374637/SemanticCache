using Microsoft.Extensions.Options;

namespace Semantic.Core
{
    /// <summary>
    /// A thread-safe, in-memory vector storage engine implementing a Least Recently Used (LRU) eviction policy.
    /// </summary>
    public class InMemoryVectorStorage : IVectorStorage
    {
        private readonly int _maxItems;
        private LinkedList<CacheEntry> _cache = new();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Gets the current number of elements stored in the cache.
        /// </summary>
        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _cache.Count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        private class CacheEntry
        {
            public string? Request { get; set; }
            public float[]? Vector { get; set; }
            public string? Response { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryVectorStorage"/> class.
        /// </summary>
        /// <param name="options">The configuration options containing the maximum item limit.</param>
        /// <exception cref="ArgumentNullException">Thrown when options are null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when MaxItems is less than or equal to zero.</exception>
        public InMemoryVectorStorage(IOptions<SemanticCacheOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);

            if (options.Value.MaxItems <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Value.MaxItems), "The size of the cache must be greater than 0.");
            }

            _maxItems = options.Value.MaxItems;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when request, vector, or response is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the vector array is empty.</exception>
        public Task SaveAsync(string request, float[] vector, string response, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(vector);
            ArgumentNullException.ThrowIfNull(response);

            if (vector.Length == 0)
            {
                throw new ArgumentException("Vector can not be null.", nameof(vector));
            }
            
            CacheEntry cacheEntry = new()
            {
                Request = request,
                Vector = vector,
                Response = response
            };

            _lock.EnterWriteLock();
            try
            {
                if (_cache.Count >= _maxItems) _cache.RemoveLast();
                _cache.AddFirst(cacheEntry);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        /// <exception cref="OperationCanceledException">Thrown if the cancellation token is triggered during the search loop.</exception>
        public Task<string?> FindSimilarAsync(float[] vector, float _threshold, bool isNormalized, CancellationToken cancellationToken = default)
        {
            float highestSimilarity = -1.0f;
            string? bestMatchString = null;
            LinkedListNode<CacheEntry>? currentNode = null;
            LinkedListNode<CacheEntry>? bestSimilarityNode = null;

            _lock.EnterReadLock();
            try
            {
                currentNode = _cache.First;

                while (currentNode != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    float cacheRecordSimilarity = VectorMath.SimilarityCheck(vector, currentNode.Value.Vector!, isNormalized);

                    if (cacheRecordSimilarity > highestSimilarity)
                    {
                        highestSimilarity = cacheRecordSimilarity;
                        bestMatchString = currentNode.Value.Response;
                        bestSimilarityNode = currentNode;
                    }
                    currentNode = currentNode.Next;
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }

            if (bestSimilarityNode != null && highestSimilarity >= _threshold)
            {
                _lock.EnterWriteLock();
                try
                {
                    if (bestSimilarityNode.List != null)
                    {
                        _cache.Remove(bestSimilarityNode);
                        _cache.AddFirst(bestSimilarityNode);
                    }
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }

            if (highestSimilarity >= _threshold) return Task.FromResult(bestMatchString);

            return Task.FromResult((string?)null);
        }
    }
}