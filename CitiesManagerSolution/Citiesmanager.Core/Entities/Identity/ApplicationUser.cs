using Microsoft.AspNetCore.Identity;

namespace Citiesmanager.Core.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? PersonName { get; set; }
    }
}
