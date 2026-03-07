using CRUDs.Extensions;
using CRUDs.Middlewares;
using Rotativa.AspNetCore;

namespace xUnit
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // serilog
            builder.Host.ConfigureLogging();

            builder.Services.ConfigureServices(builder.Configuration , builder.Environment);


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseExceptionHandlingMiddleware();
            }
            if (!app.Environment.IsEnvironment("Test"))
                RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"); 
            });

            app.Run();
        }
    }
}
