using Entities;
using IServicesContracts;
using Microsoft.EntityFrameworkCore;
using Repos;
using ReposContracts;
using Serilog;
using Services;
using StockAppAssignment.Filters.ActionFilters;
using StockAppWithConfigurationAssignment.Models;
using StockAppWithConfigurationAssignment.Services;

namespace StockAppAssignment.Extensions
{
    public static class ConfigureServicesExtension
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            // Add your custom services here
            services.AddControllersWithViews();

            services.Configure<TradingOptions>(configuration.GetSection("TradingOptions"));

            services.AddScoped<IFinnhubService, FinnhubService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IFinnhubRepo, FinnhubRepo>();
            services.AddScoped<IStocksRepo, StocksRepo>();
            services.AddScoped<ValidateOrderActionFilter>();
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
