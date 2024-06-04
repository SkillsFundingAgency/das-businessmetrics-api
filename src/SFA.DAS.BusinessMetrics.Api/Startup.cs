using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.BusinessMetrics.Api.AppStart;
using SFA.DAS.BusinessMetrics.Application.Extensions;
using SFA.DAS.Configuration.AzureTableStorage;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SFA.DAS.BusinessMetrics.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly string _environmentName;
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environmentName = configuration["EnvironmentName"];
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = _environmentName;
                    options.PreFixConfigurationKeys = false;
                });
#if DEBUG
            config.AddJsonFile("appsettings.Development.json", true);
#endif

            Configuration = config.Build();
        }

        private bool IsEnvironmentLocalOrDev =>
            _environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
            || _environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase);

        public void ConfigureServices(IServiceCollection services)
        {
            if (!IsEnvironmentLocalOrDev)
            {
                var azureAdConfiguration = Configuration
                    .GetSection("AzureAd")
                    .Get<AzureActiveDirectoryConfiguration>();

                var policies = new Dictionary<string, string>
                {
                    { PolicyNames.Default, "Default" }
                };

                services.AddAuthentication(azureAdConfiguration, policies);
            }

            services.AddOpenTelemetryRegistration(Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
                opt.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services
                .AddControllers(options =>
                {
                    if (!IsEnvironmentLocalOrDev)
                        options.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>
                        {
                            Capacity = 0
                        }));
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1",
                        new OpenApiInfo
                        {
                            Title = "SFA.DAS.BusinessMetrics.Api"
                        });

                    options.OperationFilter<SwaggerVersionHeaderFilter>();
                });

            services.AddServiceHealthChecks();
            services.AddConfigurationOptions(Configuration);
            services.AddApplicationRegistrations();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "SFA.DAS.BusinessMetrics.Api v1");
                options.RoutePrefix = string.Empty;
            });

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
            });

            app.UseHealthChecks("/ping", new HealthCheckOptions
            {
                Predicate = (_) => false,
                ResponseWriter = (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(!IsEnvironmentLocalOrDev ? "pong" : "");
                }
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
