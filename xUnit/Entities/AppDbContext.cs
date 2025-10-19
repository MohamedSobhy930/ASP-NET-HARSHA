using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) 
        { 

        }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            // seed from json 
            // 1- read from json
            string countriesjson = File.ReadAllText("Countries.json");
            string personsjson = File.ReadAllText("Persons.json");

            //2- deserialize from json to list 
            List<Country> countries = JsonSerializer.Deserialize<List<Country>>(countriesjson);
            List<Person> persons = JsonSerializer.Deserialize<List<Person>>(personsjson);

            //3- seed to database
            if(countries != null && countries.Any())
            {
                modelBuilder.Entity<Country>().HasData(countries);
            }
            if (persons != null && persons.Any())
            {
                modelBuilder.Entity<Person>().HasData(persons);
            }
        }
    }
}
