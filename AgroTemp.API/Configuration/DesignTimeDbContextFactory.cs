using AgroTemp.Domain.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AgroTemp.API.Configuration;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AgroTempDbContext>
{
    public AgroTempDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<Program>() // ✅ Add user secrets support
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AgroTempDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in user secrets or appsettings.json");
        }

        optionsBuilder.UseNpgsql(connectionString);

        return new AgroTempDbContext(optionsBuilder.Options);
    }
}