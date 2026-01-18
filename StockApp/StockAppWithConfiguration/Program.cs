using StockAppWithConfigurationAssignment.Models;
using IServicesContracts;
using StockAppWithConfigurationAssignment.Services;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using Repos;
using ReposContracts;
using Serilog;
using StockAppAssignment.Filters.ActionFilters;
namespace StockAppWithConfiguration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();
            builder.Services.Configure<TradingOptions>(builder.Configuration.GetSection("TradingOptions"));

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
            {
                loggerConfiguration
                // read configuration settings from built-in Iconfiguration
                .ReadFrom.Configuration(context.Configuration)
                // read out current app services and provide them to serilog 
                .ReadFrom.Services(services);
            });

            builder.Services.AddScoped<IFinnhubService,FinnhubService>();
            builder.Services.AddScoped<IStockService,StockService>();
            builder.Services.AddScoped<IFinnhubRepo, FinnhubRepo>();
            builder.Services.AddScoped<IStocksRepo, StocksRepo>();
            builder.Services.AddScoped<ValidateOrderActionFilter>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            if(!app.Environment.IsEnvironment("Test"))
                RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
