using Microsoft.Extensions.DependencyInjection;
using Semantic.Core;

namespace Semantic.OpenAI
{
    /// <summary>
    /// Provides extension methods for registering the OpenAI embedder in the dependency injection container.
    /// </summary>
    public static class OpenAiCacheExtensions
    {
        /// <summary>
        /// Registers the <see cref="OpenAIEmbedder"/> as the default <see cref="IEmbedder"/> implementation.
        /// </summary>
        /// <param name="services">The service collection to add the embedder to.</param>
        /// <param name="apiKey">The OpenAI API key.</param>
        /// <param name="modelId">The ID of the embedding model to use. Defaults to "text-embedding-3-small".</param>
        /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
        /// <exception cref="ArgumentException">Thrown when the API key is null or whitespace.</exception>
        public static IServiceCollection WithOpenAiEmbedder(this IServiceCollection services, string apiKey, string modelId = "text-embedding-3-small")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
            
            services.AddSingleton<IEmbedder>(new OpenAIEmbedder(apiKey, modelId));
            
            return services;
        }
    }
}