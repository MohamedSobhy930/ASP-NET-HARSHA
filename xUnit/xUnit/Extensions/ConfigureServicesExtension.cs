using CRUDs.Filters.ActionFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepoContracts;
using Repos;
using Serilog;
using ServiceContacts;
using Services;

namespace CRUDs.Extensions
{
    public static class ConfigureServicesExtension
    {
        public static void ConfigureServices(this IServiceCollection services,IConfiguration configuration , IWebHostEnvironment environment)
        {
            // Add your custom services here
            services.AddControllersWithViews(options =>
            {
                var logger = services.BuildServiceProvider()
                .GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                options.Filters.Add(new ResponseHeaderFilterFactory("custom_Key_Global", "custom_Value_Global", 2));
            });

            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IPersonsRepo, PersonRepo>();
            services.AddScoped<ICountriesRepo, CountriesRepo>();
            services.AddTransient<PersonCreateAndEditActionFilter>();
            services.AddTransient<ResponseHeaderActionFilter>();
            if (!environment.IsEnvironment("Test"))
            {
                services.AddDbContext<AppDbContext>(
                options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                });
            }

        }
        public static void ConfigureLogging(this IHostBuilder host)
        {
            host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
            {
                loggerConfiguration
                // read configuration settings from built-in Iconfiguration
                .ReadFrom.Configuration(context.Configuration)
                // read out current app services and provide them to serilog 
                .ReadFrom.Services(services);
            });
        }
    }
}
