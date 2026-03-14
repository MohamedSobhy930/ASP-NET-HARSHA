using CitiesManager.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.WebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public ApplicationDbContext()
        {
        }
        public DbSet<City> Cities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<City>().HasData(
                new City { Id = Guid.Parse("DC0306A6-B04E-4AF6-B276-20831CCDBF77"), Name = "New York" },
                new City { Id = Guid.Parse("7A7A85E0-FF23-49E2-8EB6-734CCDE628F4"), Name = "Los Angeles" },
                new City { Id = Guid.Parse("048FE215-906D-4F9C-B96D-A42EA6A93545"), Name = "Chicago" }
            );
        }
    }
}
