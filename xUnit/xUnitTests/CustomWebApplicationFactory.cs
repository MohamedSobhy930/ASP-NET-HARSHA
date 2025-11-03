using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xUnit;

namespace xUnitTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<xUnit.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Test");
            builder.ConfigureTestServices(services =>
            {
                
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestingDatabase");
                });
            });
        }
    }
}
