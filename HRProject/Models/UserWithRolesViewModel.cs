using System.Collections.Generic;

namespace HRProject.Models
{
    public class UserWithRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; } = new();

        public bool IsLockedOut { get; set; }   

    }
}
