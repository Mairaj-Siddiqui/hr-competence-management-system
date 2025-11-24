namespace HRProject.Models
{
    public class UserCompetenceProfileViewModel
    {
        public string UserEmail { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }

        public List<UserCompetence> Competences { get; set; }
    }
}
