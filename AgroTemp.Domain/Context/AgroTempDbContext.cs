using AgroTemp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.Domain.Context;

public class AgroTempDbContext : DbContext
{
    public AgroTempDbContext()
    {
    }

    public AgroTempDbContext(DbContextOptions<AgroTempDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<WorkerProfile> WorkerProfiles { get; set; }
    public DbSet<FarmerProfile> FarmerProfiles { get; set; }
    public DbSet<Farm> Farms { get; set; }
    public DbSet<JobCategory> JobCategories { get; set; }
    public DbSet<JobPost> JobPosts { get; set; }
    public DbSet<WorkerSkill> WorkerSkills { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<JobSkillRequirement> JobSkillRequirements { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<WorkerAttendance> WorkerAttendances { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<JobAssignment> JobAssignments { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("AgroTempV1");
        
        // Configure User-WorkerProfile one-to-one relationship
        modelBuilder.Entity<User>()
            .HasOne(u => u.WorkerProfile)
            .WithOne(wp => wp.User)
            .HasForeignKey<WorkerProfile>(wp => wp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure User-FarmerProfile one-to-one relationship
        modelBuilder.Entity<User>()
            .HasOne(u => u.FarmerProfile)
            .WithOne(fp => fp.User)
            .HasForeignKey<FarmerProfile>(fp => fp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure User-Notification one-to-many relationship
        modelBuilder.Entity<User>()
            .HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure User-ChatMessage (Sender) one-to-many relationship
        modelBuilder.Entity<User>()
            .HasMany(u => u.SentMessages)
            .WithOne(m => m.Sender)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure User-ChatMessage (Recipient) one-to-many relationship
        modelBuilder.Entity<User>()
            .HasMany(u => u.ReceivedMessages)
            .WithOne(m => m.Recipient)
            .HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure FarmerProfile-Farm one-to-many relationship
        modelBuilder.Entity<FarmerProfile>()
            .HasMany(fp => fp.Farms)
            .WithOne(f => f.FarmerProfile)
            .HasForeignKey(f => f.FarmerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure JobCategory-JobPost one-to-many relationship
        modelBuilder.Entity<JobCategory>()
            .HasMany(jc => jc.JobPosts)
            .WithOne(jp => jp.JobCategory)
            .HasForeignKey(jp => jp.JobCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure FarmerProfile-JobPost one-to-many relationship
        modelBuilder.Entity<FarmerProfile>()
            .HasMany<JobPost>()
            .WithOne(jp => jp.FarmerProfile)
            .HasForeignKey(jp => jp.FarmerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure enum conversions for JobPost
        modelBuilder.Entity<JobPost>()
            .Property(jp => jp.WageTypeId)
            .HasConversion<int>();

        modelBuilder.Entity<JobPost>()
            .Property(jp => jp.PaymentMethodId)
            .HasConversion<int>();

        modelBuilder.Entity<JobPost>()
            .Property(jp => jp.StatusId)
            .HasConversion<int>();

        // Configure precision for decimal columns
        modelBuilder.Entity<WorkerProfile>()
            .Property(wp => wp.AverageRating)
            .HasPrecision(3, 2); // e.g., 5.00

        modelBuilder.Entity<FarmerProfile>()
            .Property(fp => fp.AverageRating)
            .HasPrecision(3, 2); // e.g., 5.00

        modelBuilder.Entity<Farm>()
            .Property(f => f.Latitude)
            .HasPrecision(10, 7); // e.g., -90.0000000 to 90.0000000

        modelBuilder.Entity<Farm>()
            .Property(f => f.Longitude)
            .HasPrecision(10, 7); // e.g., -180.0000000 to 180.0000000

        modelBuilder.Entity<JobPost>()
            .Property(jp => jp.Latitude)
            .HasPrecision(10, 7); // e.g., -90.0000000 to 90.0000000

        modelBuilder.Entity<JobPost>()
            .Property(jp => jp.Longitude)
            .HasPrecision(10, 7); // e.g., -180.0000000 to 180.0000000

        modelBuilder.Entity<JobPost>()
            .Property(jp => jp.EstimatedHours)
            .HasPrecision(10, 2); // e.g., 12345678.90

        modelBuilder.Entity<JobPost>()
            .Property(jp => jp.WageAmount)
            .HasPrecision(18, 2); // e.g., currency amounts

        // Configure JobPost-JobApplication one-to-many relationship
        modelBuilder.Entity<JobPost>()
            .HasMany<JobApplication>()
            .WithOne(ja => ja.JobPost)
            .HasForeignKey(ja => ja.JobPostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure WorkerProfile-JobApplication one-to-many relationship
        modelBuilder.Entity<WorkerProfile>()
            .HasMany<JobApplication>()
            .WithOne(ja => ja.WorkerProfile)
            .HasForeignKey(ja => ja.WorkerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure enum conversions for JobApplication
        modelBuilder.Entity<JobApplication>()
            .Property(ja => ja.StatusId)
            .HasConversion<int>();

        // Configure JobApplication-WorkerAttendance one-to-many relationship
        modelBuilder.Entity<JobApplication>()
            .HasMany(ja => ja.WorkerAttendances)
            .WithOne(wa => wa.JobApplication)
            .HasForeignKey(wa => wa.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure precision for WorkerAttendance decimal columns
        modelBuilder.Entity<WorkerAttendance>()
            .Property(wa => wa.TotalHoursWorked)
            .HasPrecision(10, 2);

        // Configure WorkerProfile-WorkerSkill one-to-many relationship
        modelBuilder.Entity<WorkerProfile>()
            .HasMany(wp => wp.WorkerSkills)
            .WithOne(ws => ws.WorkerProfile)
            .HasForeignKey(ws => ws.WorkerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Skill-WorkerSkill one-to-many relationship
        modelBuilder.Entity<Skill>()
            .HasMany(s => s.WorkerSkills)
            .WithOne(ws => ws.Skill)
            .HasForeignKey(ws => ws.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure JobPost-JobSkillRequirement one-to-many relationship
        modelBuilder.Entity<JobPost>()
            .HasMany(jp => jp.JobSkillRequirements)
            .WithOne(jsr => jsr.JobPost)
            .HasForeignKey(jsr => jsr.JobPostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Skill-JobSkillRequirement one-to-many relationship
        modelBuilder.Entity<Skill>()
            .HasMany(s => s.JobSkillRequirements)
            .WithOne(jsr => jsr.Skill)
            .HasForeignKey(jsr => jsr.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure JobApplication-JobAssignment one-to-many relationship
        modelBuilder.Entity<JobApplication>()
            .HasMany(ja => ja.JobAssignments)
            .WithOne(ja => ja.JobApplication)
            .HasForeignKey(ja => ja.JobApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure JobPost-JobAssignment one-to-many relationship
        modelBuilder.Entity<JobPost>()
            .HasMany(jp => jp.JobAssignments)
            .WithOne(ja => ja.JobPost)
            .HasForeignKey(ja => ja.JobPostId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure WorkerProfile-JobAssignment one-to-many relationship
        modelBuilder.Entity<WorkerProfile>()
            .HasMany(wp => wp.JobAssignments)
            .WithOne(ja => ja.WorkerProfile)
            .HasForeignKey(ja => ja.WorkerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure enum conversions for JobAssignment
        modelBuilder.Entity<JobAssignment>()
            .Property(ja => ja.StatusId)
            .HasConversion<int>();

        // Configure precision for JobAssignment decimal columns
        modelBuilder.Entity<JobAssignment>()
            .Property(ja => ja.TotalHoursWorked)
            .HasPrecision(10, 2);

        // Configure JobAssignment TotalAmountDue precision
        modelBuilder.Entity<JobAssignment>()
            .Property(ja => ja.TotalAmountDue)
            .HasPrecision(18, 2);

        // Configure User-Rating (Rater) one-to-many relationship
        modelBuilder.Entity<User>()
            .HasMany(u => u.GivenRatings)
            .WithOne(r => r.Rater)
            .HasForeignKey(r => r.RaterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure User-Rating (Ratee) one-to-many relationship
        modelBuilder.Entity<User>()
            .HasMany(u => u.ReceivedRatings)
            .WithOne(r => r.Ratee)
            .HasForeignKey(r => r.RateeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure JobPost-Rating one-to-many relationship
        modelBuilder.Entity<JobPost>()
            .HasMany(jp => jp.Ratings)
            .WithOne(r => r.JobPost)
            .HasForeignKey(r => r.JobPostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure enum conversions for Rating
        modelBuilder.Entity<Rating>()
            .Property(r => r.TypeId)
            .HasConversion<int>();

        base.OnModelCreating(modelBuilder);
    }
}