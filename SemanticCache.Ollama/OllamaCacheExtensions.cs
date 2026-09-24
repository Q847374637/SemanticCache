using Microsoft.Extensions.DependencyInjection;
using Semantic.Core;

namespace Semantic.Ollama;

/// <summary>
/// Provides extension methods for registering the Ollama embedder in the dependency injection container.
/// </summary>
public static class OllamaCacheExtensions
{
    /// <summary>
    /// Registers the <see cref="OllamaEmbedder"/> as the default <see cref="IEmbedder"/> implementation.
    /// </summary>
    /// <param name="services">The service collection to add the embedder to.</param>
    /// <param name="endpoint">The URL of the local Ollama API. Defaults to "http://localhost:11434".</param>
    /// <param name="modelId">The name of the embedding model to use. Defaults to "nomic-embed-text".</param>
    /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the endpoint or model ID is null or whitespace.</exception>
    public static IServiceCollection WithOllamaEmbedder(this IServiceCollection services, string endpoint = "http://localhost:11434", string modelId = "nomic-embed-text")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(modelId);
        
        services.AddSingleton<IEmbedder>(new OllamaEmbedder(endpoint, modelId));
        
        return services;
    }
}