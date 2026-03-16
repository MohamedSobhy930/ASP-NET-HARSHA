using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitiesManager.Infrastructure.Data;
using Asp.Versioning;

namespace CitiesManager.WebApi.Controllers.v2
{
    [ApiVersion("2.0")]
    public class CitiesController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetCities()
        {
            var cities = await _context.Cities.OrderBy(c => c.Name).Select(c => c.Name).ToListAsync();
            return cities;
        }

    }
}
