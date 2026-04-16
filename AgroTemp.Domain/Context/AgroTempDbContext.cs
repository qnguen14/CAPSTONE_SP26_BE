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
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Farmer> Farmers { get; set; }
    public DbSet<Farm> Farms { get; set; }
    public DbSet<JobCategory> JobCategories { get; set; }
    public DbSet<JobPost> JobPosts { get; set; }
    public DbSet<WorkerSkill> WorkerSkills { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<JobSkillRequirement> JobSkillRequirements { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<WorkerSession> WorkerSessions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<DeviceToken> DeviceTokens { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<JobDetail> JobAssignments { get; set; }
    public DbSet<JobAttachment> JobAttachments { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PayOSOrder> PayOSOrders { get; set; }
    public DbSet<PayOSOrderItem> PayOSOrderItems { get; set; }
    public DbSet<PayOSTransaction> PayOSTransactions { get; set; }
    public DbSet<PayOSInvoice> PayOSInvoices { get; set; }
    public DbSet<PayOSWebhookLog> PayOSWebhookLogs { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }
    public DbSet<DisputeReport> DisputeReports { get; set; }
    public DbSet<DisputeReportComment> DisputeReportComments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("AgroTempV2");
        
        // Configure User-Worker one-to-one relationship
        modelBuilder.Entity<User>()
            .HasOne(u => u.Worker)
            .WithOne(wp => wp.User)
            .HasForeignKey<Worker>(wp => wp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure User-Farmer one-to-one relationship
        modelBuilder.Entity<User>()
            .HasOne(u => u.Farmer)
            .WithOne(fp => fp.User)
            .HasForeignKey<Farmer>(fp => fp.UserId)
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

        modelBuilder.Entity<User>()
            .HasMany(u => u.DeviceTokens)
            .WithOne(dt => dt.User)
            .HasForeignKey(dt => dt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Farmer-Farm one-to-many relationship
        modelBuilder.Entity<Farmer>()
            .HasMany(f => f.Farms)
            .WithOne(fp => fp.Farmer)
            .HasForeignKey(fp => fp.FarmerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure JobCategory-JobPost one-to-many relationship
        modelBuilder.Entity<JobCategory>()
            .HasMany(jc => jc.JobPosts)
            .WithOne(jp => jp.JobCategory)
            .HasForeignKey(jp => jp.JobCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Farmer-JobPost one-to-many relationship
        modelBuilder.Entity<Farmer>()
            .HasMany<JobPost>()
            .WithOne(jp => jp.Farmer)
            .HasForeignKey(jp => jp.FarmerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure enum conversions for JobPost
        modelBuilder.Entity<JobPost>()
            .Property(jp => jp.JobTypeId)
            .HasConversion<int>();

        modelBuilder.Entity<JobPost>()
            .Property(jp => jp.StatusId)
            .HasConversion<int>();

        // Configure precision for decimal columns
        modelBuilder.Entity<Worker>()
            .Property(wp => wp.AverageRating)
            .HasPrecision(3, 2); // e.g., 5.00

        modelBuilder.Entity<Farmer>()
            .Property(fp => fp.AverageRating)
            .HasPrecision(3, 2); // e.g., 5.00

        modelBuilder.Entity<Farm>()
            .Property(f => f.Latitude)
            .HasPrecision(10, 7); // e.g., -90.0000000 to 90.0000000

        modelBuilder.Entity<Farm>()
            .Property(f => f.Longitude)
            .HasPrecision(10, 7); // e.g., -180.0000000 to 180.0000000
            
        modelBuilder.Entity<JobPost>()
            .Property(jp => jp.WageAmount)
            .HasPrecision(18, 2); // e.g., currency amounts

        // Configure JobPost-JobApplication one-to-many relationship
        modelBuilder.Entity<JobPost>()
            .HasMany<JobApplication>()
            .WithOne(ja => ja.JobPost)
            .HasForeignKey(ja => ja.JobPostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Worker-JobApplication one-to-many relationship
        modelBuilder.Entity<Worker>()
            .HasMany<JobApplication>()
            .WithOne(ja => ja.Worker)
            .HasForeignKey(ja => ja.WorkerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure enum conversions for JobApplication
        modelBuilder.Entity<JobApplication>()
            .Property(ja => ja.StatusId)
            .HasConversion<int>();

        modelBuilder.Entity<JobDetail>()
            .HasMany(ja => ja.WorkerSessions)
            .WithOne(wa => wa.JobDetail)
            .HasForeignKey(wa => wa.JobDetailId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkerSession>()
            .Property(ws => ws.TotalHoursWorked)
            .HasPrecision(10, 2);

        // Configure Worker-WorkerSkill one-to-many relationship
        modelBuilder.Entity<Worker>()
            .HasMany(wp => wp.WorkerSkills)
            .WithOne(ws => ws.Worker)
            .HasForeignKey(ws => ws.WorkerId)
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
            .HasMany(ja => ja.JobDetails)
            .WithOne(ja => ja.JobApplication)
            .HasForeignKey(ja => ja.JobApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure JobPost-JobAssignment one-to-many relationship
        modelBuilder.Entity<JobPost>()
            .HasMany(jp => jp.JobDetails)
            .WithOne(ja => ja.JobPost)
            .HasForeignKey(ja => ja.JobPostId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Worker-JobDetail one-to-many relationship
        modelBuilder.Entity<Worker>()
            .HasMany(w => w.JobDetails)
            .WithOne(jd => jd.Worker)
            .HasForeignKey(jd => jd.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure enum conversions for JobAssignment
        modelBuilder.Entity<JobDetail>()
            .Property(jd => jd.StatusId)
            .HasConversion<int>();


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

        // Configure User-Wallet one-to-one relationship
        modelBuilder.Entity<User>()
            .HasOne(u => u.Wallet)
            .WithOne(w => w.User)
            .HasForeignKey<Wallet>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Wallet-WalletTransaction one-to-many relationship
        modelBuilder.Entity<Wallet>()
            .HasMany(w => w.WalletTransactions)
            .WithOne(t => t.Wallet)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WalletTransaction>()
            .Property(t => t.Type)
            .HasConversion<int>();

        modelBuilder.Entity<Wallet>()
            .HasMany(w => w.WithdrawalRequests)
            .WithOne(t => t.Wallet)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Wallet>()
            .HasMany(w => w.Payments)
            .WithOne(t => t.Wallet)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PayOSOrder>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PayOSOrder>()
            .HasMany(o => o.Transactions)
            .WithOne(t => t.Order)
            .HasForeignKey(t => t.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PayOSOrder>()
            .HasMany(o => o.Invoices)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PayOSOrder>()
            .HasIndex(o => o.OrderCode)
            .IsUnique();

        modelBuilder.Entity<PayOSOrder>()
            .HasIndex(o => o.PaymentLinkId);

        modelBuilder.Entity<PayOSWebhookLog>()
            .HasIndex(l => l.OrderCode);

        modelBuilder.Entity<PayOSWebhookLog>()
            .HasIndex(l => l.Reference);

        modelBuilder.Entity<PayOSWebhookLog>()
            .HasIndex(l => l.ReceivedAt);

        modelBuilder.Entity<DeviceToken>()
            .HasIndex(dt => new { dt.UserId, dt.ExpoPushToken })
            .IsUnique();

        // Configure User-WithdrawalRequest one-to-many relationship
        modelBuilder.Entity<JobDetail>()
            .HasMany(u => u.WalletTransactions)
            .WithOne(wr => wr.JobDetail)
            .HasForeignKey(wr => wr.JobDetailId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobDetail>()
            .HasMany(u => u.JobAttachments)
            .WithOne(wr => wr.JobDetail)
            .HasForeignKey(wr => wr.JobDetailId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DisputeReport>()
            .HasMany(dr => dr.Comments)
            .WithOne(c => c.DisputeReport)
            .HasForeignKey(c => c.DisputeReportId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DisputeReportComment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}