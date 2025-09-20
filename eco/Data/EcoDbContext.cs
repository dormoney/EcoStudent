using Microsoft.EntityFrameworkCore;
using eco.Models;

namespace eco.Data
{
    public class EcoDbContext : DbContext
    {
        public EcoDbContext(DbContextOptions<EcoDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<RecyclingPoint> RecyclingPoints { get; set; }
        public DbSet<RecyclingSubmission> RecyclingSubmissions { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<UserChallenge> UserChallenges { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<UserPromotion> UserPromotions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.RegistrationDate).HasDefaultValueSql("GETDATE()");
                entity.Property(u => u.TotalPoints).HasDefaultValue(0);
            });

            // RecyclingSubmission configurations
            modelBuilder.Entity<RecyclingSubmission>(entity =>
            {
                entity.Property(rs => rs.SubmissionDate).HasDefaultValueSql("GETDATE()");
                entity.Property(rs => rs.IsVerified).HasDefaultValue(false);
                
                entity.HasOne(rs => rs.User)
                      .WithMany(u => u.RecyclingSubmissions)
                      .HasForeignKey(rs => rs.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rs => rs.RecyclingPoint)
                      .WithMany(rp => rp.RecyclingSubmissions)
                      .HasForeignKey(rs => rs.PointId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // User-Group relationship
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.Group)
                      .WithMany(g => g.Users)
                      .HasForeignKey(u => u.GroupId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // UserAchievement configurations
            modelBuilder.Entity<UserAchievement>(entity =>
            {
                entity.Property(ua => ua.DateEarned).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(ua => ua.User)
                      .WithMany(u => u.UserAchievements)
                      .HasForeignKey(ua => ua.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ua => ua.Achievement)
                      .WithMany(a => a.UserAchievements)
                      .HasForeignKey(ua => ua.AchievementId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // UserChallenge configurations
            modelBuilder.Entity<UserChallenge>(entity =>
            {
                entity.Property(uc => uc.CurrentValue).HasDefaultValue(0);
                entity.Property(uc => uc.IsCompleted).HasDefaultValue(false);
                
                entity.HasOne(uc => uc.User)
                      .WithMany(u => u.UserChallenges)
                      .HasForeignKey(uc => uc.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(uc => uc.Challenge)
                      .WithMany(c => c.UserChallenges)
                      .HasForeignKey(uc => uc.ChallengeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Challenge configurations
            modelBuilder.Entity<Challenge>(entity =>
            {
                entity.Property(c => c.IsGroupChallenge).HasDefaultValue(false);
                entity.Property(c => c.IsActive).HasDefaultValue(true);
            });

            // News configurations
            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(n => n.PublishDate).HasDefaultValueSql("GETDATE()");
            });

            // Promotion configurations
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.Property(p => p.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(p => p.IsActive).HasDefaultValue(true);
            });

            // UserPromotion configurations
            modelBuilder.Entity<UserPromotion>(entity =>
            {
                entity.Property(up => up.PurchaseDate).HasDefaultValueSql("GETDATE()");
                entity.Property(up => up.IsUsed).HasDefaultValue(false);
                
                entity.HasOne(up => up.User)
                      .WithMany(u => u.UserPromotions)
                      .HasForeignKey(up => up.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(up => up.Promotion)
                      .WithMany(p => p.UserPromotions)
                      .HasForeignKey(up => up.PromotionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
