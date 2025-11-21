using Microsoft.AspNetCore.Identity;

namespace HRProject.Models
{
    // We will add more properties later (skills, availability, etc.)
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string ? JobTitle { get; set; }
    }
}
