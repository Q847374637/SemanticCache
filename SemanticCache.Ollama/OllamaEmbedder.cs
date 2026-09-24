using OllamaSharp;
using Semantic.Core;

namespace Semantic.Ollama;

/// <summary>
/// Provides an implementation of <see cref="IEmbedder"/> that uses a local Ollama instance to generate text embeddings.
/// </summary>
public class OllamaEmbedder : IEmbedder
{
    private readonly IOllamaApiClient _client;

    /// <inheritdoc/>
    public bool ReturnsNormalizedVectors => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaEmbedder"/> class.
    /// </summary>
    /// <param name="endpoint">The URL of the local Ollama API (e.g., "http://localhost:11434").</param>
    /// <param name="modelId">The name of the embedding model to use. Defaults to "nomic-embed-text".</param>
    /// <exception cref="ArgumentException">Thrown when the endpoint or model ID is null or whitespace.</exception>
    public OllamaEmbedder(string endpoint, string modelId = "nomic-embed-text")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(modelId);

        _client = new OllamaApiClient(endpoint)
        {
            SelectedModel = modelId
        };
    }

    /// <inheritdoc/>
    public async Task<float[]> GenerateVectorAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        
        var requestedVector = await _client.EmbedAsync(text, cancellationToken: cancellationToken);
        
        return requestedVector.Embeddings[0]; 
    }
}