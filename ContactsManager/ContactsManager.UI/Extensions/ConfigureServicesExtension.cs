using ContactsManager.Core.Domain.IdentityEntities;
using CRUDs.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
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

                // applicable for all post action methods and no need to add [ValidateAntiForgeryToken] on each post action method
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IPersonsRepo, PersonRepo>();
            services.AddScoped<ICountriesRepo, CountriesRepo>();
            services.AddTransient<PersonCreateAndEditActionFilter>();
            services.AddTransient<ResponseHeaderActionFilter>();

            //enable Identity in the project
            services.AddIdentity<ApplicationUser , ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 5; // 5 or more characters
                options.Password.RequireNonAlphanumeric = true; // special character
                options.Password.RequireUppercase = true; // uppercase letter
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser , ApplicationRole , AppDbContext , Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole , AppDbContext , Guid>>();
            services.AddAuthorization(options => 
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                // this custom policy is used to restrict access to login and register pages for authenticated users
                options.AddPolicy("NotAuthenticated", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return !context.User.Identity.IsAuthenticated;
                    });
                });
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
            });
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
