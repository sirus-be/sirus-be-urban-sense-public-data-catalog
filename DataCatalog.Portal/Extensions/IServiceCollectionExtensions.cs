using Core.Authentication;
using DataCatalog.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using System;

namespace DataCatalog.Portal.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DataCatalogServiceClientOptions>(configuration.GetSection("DataCatalogService"));
            var dataCatalogServiceClientOptions = configuration.GetSection("DataCatalogService").Get<DataCatalogServiceClientOptions>();

            return services.AddDataCatalogServiceClient(dataCatalogServiceClientOptions);
        }

        public static IServiceCollection AddRadzenServices(this IServiceCollection services)
        {
            services.AddScoped<NotificationService>();
            services.AddScoped<DialogService>();
            return services;
        }


        private static IServiceCollection AddDataCatalogServiceClient(this IServiceCollection services, DataCatalogServiceClientOptions clientOptions)
        {
            services.AddScoped<TokenProvider>();

            services
                .AddHttpClient<IDataCatalogServiceClient, DataCatalogServiceClient>(options =>
                {
                    options.BaseAddress = new Uri(clientOptions.Uri);
                });

            return services;
        }
    }
}
