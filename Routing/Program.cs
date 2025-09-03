namespace Routing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            

            // Enable Routing Middleware
            app.UseRouting();

            //Data 
            Dictionary<int, string> countries = new()
                    {
                        { 1, "USA" },
                        { 2, "UK" },
                        { 3, "India" },
                        { 4, "Germany" },
                        { 5, "France" }
                    };


            // Creating Endpoints
            app.UseEndpoints(endpoints =>
            {
                // Eg: /Countries 
                endpoints.MapGet("/Countries", async context =>
                {
                    foreach (var country in countries)
                    {
                        await context.Response.WriteAsync($"{country.Key}, {country.Value}\n");
                    }
                });
                // Eg: /Countries/1
                endpoints.MapGet("/Countries/{id:int}", async context =>
                {
                    int id = Convert.ToInt32(context.Request.RouteValues["id"]);
                    if (countries.ContainsKey(id))
                    {
                        await context.Response.WriteAsync($"{countries[id]}");
                    }
                    else if(id > 100)
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("ID should be between 1 and 100");
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("Country not found");
                    }
                });
            });

            // Fallback Middleware
            app.Run(async context =>
            {
                await context.Response.WriteAsync($"No Response");
            });

            app.Run();
        }
    }
}
