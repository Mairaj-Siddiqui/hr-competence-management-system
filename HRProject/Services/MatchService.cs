using HRProject.Data;
using HRProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HRProject.Services
{
    public class MatchService
    {
        private readonly ApplicationDbContext _context;

        public MatchService(ApplicationDbContext context)
        {
            _context = context;
        }

        // This combines the 3 percentages using the weights from MatchSettings
        public async Task<double> CalculateOverallMatchAsync(
            int competenceMatchPercent,
            int experienceMatchPercent,
            int availabilityMatchPercent)
        {
            var settings = await _context.MatchSettings.FirstOrDefaultAsync();

            if (settings == null)
            {
                // fallback if somehow no settings exist
                settings = new MatchSettings();
            }

            // just in case Admin does not use full 100%
            var totalWeight = settings.CompetenceWeight
                              + settings.ExperienceWeight
                              + settings.AvailabilityWeight;

            if (totalWeight == 0)
                return 0;

            double weighted =
                  competenceMatchPercent * settings.CompetenceWeight
                + experienceMatchPercent * settings.ExperienceWeight
                + availabilityMatchPercent * settings.AvailabilityWeight;

            double score = weighted / totalWeight; // result 0–100

            return score;
        }
    }
}
