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

        // ====== YOUR TABLES ======
        public DbSet<Competence> Competences { get; set; }
        public DbSet<UserCompetence> UserCompetences { get; set; }

        public DbSet<MatchSettings> MatchSettings { get; set; }
   
        public DbSet<ProjectManager> ProjectManager { get; set; }
        public DbSet<ProjectRole> ProjectRoles { get; set; }
        public DbSet<ProjectRequirement> ProjectRequirementsNew { get; set; } 
        public DbSet<ProjectTeamMember> ProjectTeamMembers { get; set; }

        public DbSet<TeamLeader> TeamLeaders { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TeamSkillNeed> TeamSkillNeeds { get; set; }
        public DbSet<TeamGrowthPlan> TeamGrowthPlans { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ================ USER COMPETENCE RELATIONS ==================
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

            // ================ TEAMLEADER RELATIONS ==================
            builder.Entity<TeamLeader>()
                .HasOne(t => t.LeaderUser)
                .WithMany()
                .HasForeignKey(t => t.LeaderUserId);

            builder.Entity<TeamMember>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId);

            builder.Entity<TeamMember>()
                .HasOne(m => m.TeamLeader)
                .WithMany(t => t.Members)
                .HasForeignKey(m => m.TeamLeaderId)
                .OnDelete(DeleteBehavior.NoAction);

            // ================ PROJECTMANAGER RELATION ==================
            builder.Entity<ProjectManager>()
            .HasMany(p => p.ProjectRoles)
            .WithOne(r => r.Project)
            .HasForeignKey(r => r.ProjectId);

            builder.Entity<ProjectManager>()
                .HasMany(p => p.Requirements)
                .WithOne(sr => sr.Project)
                .HasForeignKey(sr => sr.ProjectId);

            builder.Entity<ProjectTeamMember>()
                .HasOne(pt => pt.Project)
                .WithMany()
                .HasForeignKey(pt => pt.ProjectId);

            builder.Entity<ProjectTeamMember>()
                .HasOne(pt => pt.User)
                .WithMany()
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
