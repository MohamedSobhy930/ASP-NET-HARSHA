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
using StockAppAssignment.Extensions;
using StockAppAssignment.Middlewares;
namespace StockAppWithConfiguration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpClient();

            builder.Host.ConfigureLogging();
            builder.Services.ConfigureServices(builder.Configuration , builder.Environment);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseExceptionHandlingMiddleware();
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
