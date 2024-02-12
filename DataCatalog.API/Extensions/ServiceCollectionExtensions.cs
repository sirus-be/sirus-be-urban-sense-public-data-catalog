using DataCatalog.API.Configuration;
using DataCatalog.API.Handlers;
using DataCatalog.API.Infrastructure.Database;
using DataCatalog.API.Infrastructure.DataStores;
using DataCatalog.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Authentication;

namespace DataCatalog.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var apiConfiguration = configuration.GetSection(nameof(ApiConfiguration));
            services.Configure<ApiConfiguration>(apiConfiguration);
        }

        public static void AddDataStores(this IServiceCollection services)
        {
            services.AddTransient<IDataSetStore, DataSetStore>();
        }

        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IDataSetService, DataSetService>();
        }


        public static void AddCustomAuthentication(this IServiceCollection services, bool isDevelopment, IConfiguration configuration)
        {
            var identityProvider = configuration.GetSection("IdentityProvider").Value;

            if (identityProvider == "IdentityServer")
            {
                var identityProviderUrl = configuration.GetSection("IdentityProviderUrl").Value;
                var apiName = configuration.GetSection("ApiConfiguration:Name").Value;
                int apiVersion = int.Parse(configuration.GetSection("ApiConfiguration:Version").Value);

                services.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication("Bearer", options =>
                    {
                        options.ApiName = apiName;
                        options.Authority = $"{identityProviderUrl}";
                        if (isDevelopment)
                        {   // to make sure this is only used during development
                        options.RequireHttpsMetadata = false;
                        }
                        if (identityProviderUrl == configuration["OpenIdAuthentication:Authority"])
                    {
                            options.RequireHttpsMetadata = true;
                            options.JwtBackChannelHandler = GetHandler();
                        }
                    });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("dataset_access", policy =>
                        policy.RequireClaim("scope", "DataCatalog.Api.dataset"));
                    options.AddPolicy("datacatalog_access", policy =>
                        policy.RequireClaim("scope", "DataCatalog.Api.catalog"));
                });
            }

            else
            {
                services.AddJwtAuthentication(isDevelopment, options =>
                {
                    options.Audience = configuration["OpenIdAuthentication:Audience"];
                    options.Authority = configuration["OpenIdAuthentication:Authority"];
                    options.TokenValidationParameters.ValidIssuer = options.Authority;
                });
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ValidRoleRolesPolicy", policy =>
                        policy.Requirements.Add(new ValidRoleRolesRequirement()));

                    options.DefaultPolicy = new AuthorizationPolicyBuilder(
                        JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
                });

                services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
            }
        }

        public static void AddDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkNpgsql();

            services.AddDbContext<DataCatalogDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DataCatalogDatabase"));
            });
        }

        public static void RegisterSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{configuration.GetSection("ApiConfiguration:Version").Value}", new OpenApiInfo
                {
                    Title = configuration.GetSection("ApiConfiguration:Name").Value,
                    Version = $"v{configuration.GetSection("ApiConfiguration:Version").Value}",
                    Description = "DataCatalog DCAT compliant API",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Sirus",
                        Url = new System.Uri("https://www.sirus.be")
                    }
                });
                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {Value}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearerAuth"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlfile);
                c.IncludeXmlComments(xmlPath);
            });
        }
        private static HttpClientHandler GetHandler()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.SslProtocols = SslProtocols.Tls12;
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            return handler;
        }

    }
}
