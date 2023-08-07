using RequestForwarding.Controller;
using RequestForwarding.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RequestForwarding
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRequestForwarding(this IServiceCollection services)
        {
            services.AddHttpClient(HttpForwarderController.HttpForwarderClient);

            services.AddSingleton<IHeaderOptions, HeaderOptions>();
            services.AddSingleton<IRequestCreator, RequestCreator>();
            services.AddSingleton<IResponseForwarder, ResponseForwarder>();

            services.AddControllers();
            return services;
        }
    }
}
