//using HRProject.Data;
//using HRProject.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;

//namespace HRProject.Services
//{
//    public class MatchService
//    {
//        private readonly ApplicationDbContext _context;

//        public MatchService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // This combines the 3 percentages using the weights from MatchSettings
//        public async Task<double> CalculateOverallMatchAsync(
//            int competenceMatchPercent,
//            int experienceMatchPercent,
//            int availabilityMatchPercent)
//        {
//            var settings = await _context.MatchSettings.FirstOrDefaultAsync();

//            if (settings == null)
//            {
//                // fallback if somehow no settings exist
//                settings = new MatchSettings();
//            }

//            // just in case Admin does not use full 100%
//            var totalWeight = settings.CompetenceWeight
//                              + settings.ExperienceWeight
//                              + settings.AvailabilityWeight;

//            if (totalWeight == 0)
//                return 0;

//            double weighted =
//                  competenceMatchPercent * settings.CompetenceWeight
//                + experienceMatchPercent * settings.ExperienceWeight
//                + availabilityMatchPercent * settings.AvailabilityWeight;

//            double score = weighted / totalWeight; // result 0–100

//            return score;
//        }


         public async Task<(int competence, int experience, int availability)>
          CalculateComponentMatchesAsync(int projectId, string userId)
         {
        // load project with requirements
        var project = await _context.Projects
            .Include(p => p.Requirements)
            .ThenInclude(r => r.Competence)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null || !project.Requirements.Any())
            return (0, 0, 0);

        // load user with competences
        var user = await _context.Users
            .Include(u => u.UserCompetences)
            .ThenInclude(uc => uc.Competence)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return (0, 0, 0);

        var userCompetences = user.UserCompetences;

        // --- 1) Competence coverage: how many required skills the user has ---
        int totalRequirements = project.Requirements.Count;
        int requirementsCovered = 0;

        // --- 2) Experience coverage: how many requirements the user meets in years ---
        int experienceCovered = 0;

        foreach (var req in project.Requirements)
        {
            var userSkill = userCompetences
                .FirstOrDefault(uc => uc.CompetenceId == req.CompetenceId);

            if (userSkill != null)
            {
                requirementsCovered++;

                // check years of experience
                if (userSkill.YearsOfExperience.HasValue &&
                    userSkill.YearsOfExperience.Value >= req.MinYearsOfExperience)
                {
                    experienceCovered++;
                }
            }
        }

        int competenceMatchPercent = (int)(requirementsCovered * 100.0 / totalRequirements);
        int experienceMatchPercent = (int)(experienceCovered * 100.0 / totalRequirements);

        // --- 3) Availability: for now just use the user's AvailabilityPercent ---
        int availabilityMatchPercent = user.AvailabilityPercent;

        return (competenceMatchPercent, experienceMatchPercent, availabilityMatchPercent);
    }
        public async Task<List<ProjectMatchResultViewModel>> GetTopMatchesForProjectAsync(
    int projectId, int topN = 10)
        {
            var users = await _context.Users
                .Include(u => u.UserCompetences)
                .ToListAsync();

            var settings = await _context.MatchSettings.FirstOrDefaultAsync()
                           ?? new MatchSettings();

            var totalWeight = settings.CompetenceWeight
                            + settings.ExperienceWeight
                            + settings.AvailabilityWeight;

            if (totalWeight == 0) totalWeight = 1;

            var results = new List<ProjectMatchResultViewModel>();

            foreach (var user in users)
            {
                var (comp, exp, avail) =
                    await CalculateComponentMatchesAsync(projectId, user.Id);

                double weighted =
                      comp * settings.CompetenceWeight
                    + exp * settings.ExperienceWeight
                    + avail * settings.AvailabilityWeight;

                double overall = weighted / totalWeight;

                results.Add(new ProjectMatchResultViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserName = user.FullName ?? user.Email,
                    CompetenceMatchPercent = comp,
                    ExperienceMatchPercent = exp,
                    AvailabilityMatchPercent = avail,
                    OverallMatchPercent = overall
                });
            }

            return results
                .OrderByDescending(r => r.OverallMatchPercent)
                .Take(topN)
                .ToList();
        }

//    }
//}
