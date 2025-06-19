using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<CoachSession> CoachSessions { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<CommunityPost> CommunityPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<MemberPackage> MemberPackages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<QuitPlan> QuitPlans { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SmokingStatus> SmokingStatuses { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<Progress> ProgressRecords { get; set; }
        
        // New entities for Package System
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<QuitStageProgress> QuitStageProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Account configurations
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Role);
                entity.HasIndex(e => e.CreatedAt);
            });

            // Coach configurations
            modelBuilder.Entity<Coach>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithOne(p => p.Coach)
                    .HasForeignKey<Coach>(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.AccountId).IsUnique();
                entity.HasIndex(e => e.Status);
            });

            // CoachSession configurations
            modelBuilder.Entity<CoachSession>(entity =>
            {
                entity.HasOne(d => d.Coach)
                    .WithMany(p => p.CoachSessions)
                    .HasForeignKey(d => d.CoachId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.CoachSessions)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.CoachId);
                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.SessionDate);
                entity.HasIndex(e => e.Status);
            });

            // Feedback configurations
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.CoachSession)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.CoachSessionId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.CoachSessionId);
            });

            // CommunityPost configurations
            modelBuilder.Entity<CommunityPost>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.CommunityPosts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Status);
            });

            // Comment configurations
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.Replies)
                    .HasForeignKey(d => d.ParentId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.PostId);
                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.ParentId);
            });

            // PostLike configurations
            modelBuilder.Entity<PostLike>(entity =>
            {
                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostLikes)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.PostLikes)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => new { e.PostId, e.AccountId }).IsUnique();
                entity.HasIndex(e => e.PostId);
                entity.HasIndex(e => e.AccountId);
            });

            // MemberPackage configurations
            modelBuilder.Entity<MemberPackage>(entity =>
            {
                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.CreatedPackages)
                    .HasForeignKey(d => d.CreatedById)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.AssignedCoach)
                    .WithMany(p => p.AssignedPackages)
                    .HasForeignKey(d => d.AssignedCoachId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.CreatedById);
                entity.HasIndex(e => e.AssignedCoachId);
                entity.HasIndex(e => e.IsActive);
            });

            // Order configurations
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PackageId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.PackageId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.OrderDate);
            });

            // QuitPlan configurations
            modelBuilder.Entity<QuitPlan>(entity =>
            {
                entity.HasOne(d => d.Package)
                    .WithMany(p => p.QuitPlans)
                    .HasForeignKey(d => d.PackageId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.QuitPlans)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Coach)
                    .WithMany(p => p.QuitPlans)
                    .HasForeignKey(d => d.CoachId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.MemberId);
                entity.HasIndex(e => e.CoachId);
                entity.HasIndex(e => e.PackageId);
                entity.HasIndex(e => e.Status);
            });

            // Notification configurations
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.IsRead);
            });

            // SmokingStatus configurations
            modelBuilder.Entity<SmokingStatus>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.SmokingStatuses)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.Status);
            });

            // UserAchievement configurations
            modelBuilder.Entity<UserAchievement>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.UserAchievements)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Achievement)
                    .WithMany(p => p.UserAchievements)
                    .HasForeignKey(d => d.AchievementId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => new { e.AccountId, e.AchievementId }).IsUnique();
                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.AchievementId);
            });

            // Progress configurations
            modelBuilder.Entity<Progress>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ProgressRecords)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.Date);
            });

            // UserSubscription configurations
            modelBuilder.Entity<UserSubscription>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany()
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.AssignedCoach)
                    .WithMany()
                    .HasForeignKey(d => d.AssignedCoachId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.PackageType);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.StartDate);
                entity.HasIndex(e => e.EndDate);
            });

            // ChatMessage configurations
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasOne(d => d.Sender)
                    .WithMany()
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Receiver)
                    .WithMany()
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.SenderId);
                entity.HasIndex(e => e.ReceiverId);
                entity.HasIndex(e => e.SentAt);
                entity.HasIndex(e => e.IsRead);
            });

            // QuitStageProgress configurations
            modelBuilder.Entity<QuitStageProgress>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany()
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.CurrentStage);
                entity.HasIndex(e => e.StageStartDate);
                entity.HasIndex(e => e.StageEndDate);
            });
        }
    }
} 