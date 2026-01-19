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

        base.OnModelCreating(modelBuilder);
    }
}