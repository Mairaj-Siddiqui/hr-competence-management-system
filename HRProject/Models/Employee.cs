namespace HRProject.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public string Interests { get; set; }

        public ICollection<UserCompetence> UserCompetences { get; set; } = new List<UserCompetence>();
    }
}
