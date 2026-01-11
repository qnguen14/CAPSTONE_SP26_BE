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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("AgroTempV1");
        base.OnModelCreating(modelBuilder);
    }
}