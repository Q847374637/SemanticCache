using Microsoft.Extensions.DependencyInjection;

namespace Semantic.Core
{
    /// <summary>
    /// Provides extension methods for registering SemanticCache services in the dependency injection container.
    /// </summary>
    public static class SemanticCacheExtensions
    {
        /// <summary>
        /// Registers the semantic cache core components, storage, and embedder implementations into the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TEmbedder">The implementation type for <see cref="IEmbedder"/>.</typeparam>
        /// <typeparam name="TStorage">The implementation type for <see cref="IVectorStorage"/>.</typeparam>
        /// <param name="services">The service collection to add the services to.</param>
        /// <param name="configure">An action to configure the <see cref="SemanticCacheOptions"/>.</param>
        /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
        public static IServiceCollection AddSemanticCache<TEmbedder, TStorage>(this IServiceCollection services, Action<SemanticCacheOptions> configure)
        where TEmbedder : class, IEmbedder
        where TStorage : class, IVectorStorage
        {
            services.Configure(configure);
            services.AddSingleton<IEmbedder, TEmbedder>();
            services.AddSingleton<IVectorStorage, TStorage>();
            services.AddSingleton<ISemanticCache, SemanticCache>();

            return services;
        }
    }
}