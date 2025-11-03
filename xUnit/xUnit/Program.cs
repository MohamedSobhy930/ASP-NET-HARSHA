using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepoContracts;
using Repos;
using Rotativa.AspNetCore;
using ServiceContacts;
using Services;

namespace xUnit
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddScoped<IPersonService, PersonService>();
            builder.Services.AddScoped<IPersonsRepo, PersonRepo>();
            builder.Services.AddScoped<ICountriesRepo,CountriesRepo>();

            if(!builder.Environment.IsEnvironment("Test"))
            {
                builder.Services.AddDbContext<AppDbContext>(
                                options =>
                                {
                                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                                });
            }
            
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
