using Elastic.Apm.AspNetCore;
using Elastic.Apm.AspNetCore.DiagnosticListener;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwilightSparkle.PapersPlease.Api.HealthChecks;
using TwilightSparkle.PapersPlease.Api.Middlewares;
using TwilightSparkle.PapersPlease.Api.Services;

namespace TwilightSparkle.PapersPlease.Api
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;


        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPassportService, PassportService>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("*");
                });
            });

            services.AddHealthChecks()
                .AddCheck<DefaultHealthCheck>("DefaultHealthCheck");

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseElasticApm(_configuration, new AspNetCoreDiagnosticSubscriber());

            app.UseCors();

            app.UseMiddleware<ErrorLoggerMiddleware>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/api/health");
                endpoints.MapControllerRoute(
                    "API",
                    "api/{ControllerBase=Home}/{action=Index}");
            });
        }
    }
}