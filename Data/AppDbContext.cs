using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<SmokingStatus> SmokingStatuses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<DailyTask> DailyTasks { get; set; }
        public DbSet<CoachApplication> CoachApplications { get; set; }
        public DbSet<PlanTemplate> PlanTemplates { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<CoachCommission> CoachCommissions { get; set; }
        public DbSet<PostInteraction> PostInteractions { get; set; }
        
        // Added missing DbSets
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue("User");
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Activity configurations
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.HasKey(e => e.ActivityId);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MoneySaved).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Activities)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Post configurations
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.PostId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Parent)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(p => p.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Comment configurations
            modelBuilder.Entity<Comment>().ToTable("Comments", t => t.ExcludeFromMigrations());

            // Appointment configurations
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentId);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                
                entity.HasOne(e => e.Member)
                    .WithMany(u => u.MemberAppointments)
                    .HasForeignKey(e => e.MemberId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Coach)
                    .WithMany(u => u.CoachAppointments)
                    .HasForeignKey(e => e.CoachId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Package configurations
            modelBuilder.Entity<Package>(entity =>
            {
                entity.HasKey(e => e.PackageId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Type).HasMaxLength(50);
                
                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AssignedCoach)
                    .WithMany()
                    .HasForeignKey(e => e.AssignedCoachId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Plan configurations
            modelBuilder.Entity<Plan>(entity =>
            {
                entity.HasKey(e => e.PlanId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                
                entity.HasOne(e => e.Package)
                    .WithMany(p => p.Plans)
                    .HasForeignKey(e => e.PackageId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Member)
                    .WithMany(u => u.MemberPlans)
                    .HasForeignKey(e => e.MemberId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Coach)
                    .WithMany(u => u.CoachPlans)
                    .HasForeignKey(e => e.CoachId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Order configurations
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CoachCommission).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Package)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(e => e.PackageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Achievement configurations
            modelBuilder.Entity<Achievement>(entity =>
            {
                entity.HasKey(e => e.AchievementId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Icon).HasMaxLength(200);
                entity.Property(e => e.Criteria).HasMaxLength(500);
                entity.Property(e => e.Points).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // UserAchievement configurations
            modelBuilder.Entity<UserAchievement>(entity =>
            {
                entity.HasKey(e => e.UserAchievementId);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserAchievements)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Achievement)
                    .WithMany(a => a.UserAchievements)
                    .HasForeignKey(e => e.AchievementId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // SmokingStatus configurations
            modelBuilder.Entity<SmokingStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.SmokingStatuses)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.CostPerPack).HasColumnType("decimal(10,2)");
                entity.Property(e => e.MoneySaved).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Active");
            });

            // Notification configurations
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotificationId);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).HasMaxLength(1000);
                entity.Property(e => e.Type).HasMaxLength(50);
                entity.Property(e => e.IsRead).HasDefaultValue(false);
            });

            // DailyTask configurations
            modelBuilder.Entity<DailyTask>(entity =>
            {
                entity.HasKey(e => e.TaskId);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.DailyTasks)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Plan)
                    .WithMany(p => p.DailyTasks)
                    .HasForeignKey(e => e.PlanId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.Priority).HasMaxLength(20).HasDefaultValue("Medium");
            });

            // CoachApplication configurations
            modelBuilder.Entity<CoachApplication>(entity =>
            {
                entity.HasKey(e => e.ApplicationId);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.CoachApplications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Qualifications).HasMaxLength(1000);
                entity.Property(e => e.Experience).HasMaxLength(1000);
                entity.Property(e => e.Motivation).HasMaxLength(1000);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.ReviewNotes).HasMaxLength(1000);
            });

            // PlanTemplate configurations
            modelBuilder.Entity<PlanTemplate>(entity =>
            {
                entity.HasKey(e => e.TemplateId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Difficulty).HasMaxLength(20);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // RefreshToken configurations
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
                entity.Property(e => e.IsRevoked).HasDefaultValue(false);
            });

            // CoachCommission configurations
            modelBuilder.Entity<CoachCommission>(entity =>
            {
                entity.HasKey(e => e.CommissionId);
                entity.HasOne(e => e.Coach)
                    .WithMany()
                    .HasForeignKey(e => e.CoachId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasMaxLength(20);
            });

            // PostInteraction configurations
            modelBuilder.Entity<PostInteraction>(entity =>
            {
                entity.HasOne(pi => pi.Post)
                    .WithMany(p => p.Interactions)
                    .HasForeignKey(pi => pi.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pi => pi.User)
                    .WithMany()
                    .HasForeignKey(pi => pi.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(pi => new { pi.PostId, pi.UserId, pi.Type }).IsUnique();
            });
        }
    }
} 