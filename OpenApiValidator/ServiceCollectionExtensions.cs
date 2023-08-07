using Microsoft.Extensions.DependencyInjection;
using OpenApiValidator.Model;
using OpenApiValidator.Services;

namespace OpenApiValidator
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenApiVerification(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient(OpenApiLoader.OpenApiLoaderClient);
            services.AddSingleton<IOpenApiLoader, OpenApiLoader>();

            services.AddSingleton<IRequestValidator, RequestValidator>();
            services.AddSingleton<IResponseBuilder, ResponseBuilder>();
            services.AddSingleton<IOpenApiQueryService, OpenApiQueryService>();
            services.AddSingleton<IValueGenerator, ValueGenerator>();

            return services;
        }
    }
}
