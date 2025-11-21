using HRProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Competence> Competences { get; set; }
        public DbSet<UserCompetence> UserCompetences { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserCompetence>()
                .HasKey(uc => new { uc.UserId, uc.CompetenceId });

            builder.Entity<UserCompetence>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCompetences)
                .HasForeignKey(uc => uc.UserId);

            builder.Entity<UserCompetence>()
                .HasOne(uc => uc.Competence)
                .WithMany(c => c.UserCompetences)
                .HasForeignKey(uc => uc.CompetenceId);
        }
    }
}
