using AgroTemp.Domain.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AgroTemp.API.Configuration;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AgroTempDbContext>
{
    public AgroTempDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AgroTempDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        return new AgroTempDbContext(optionsBuilder.Options);
    }
}