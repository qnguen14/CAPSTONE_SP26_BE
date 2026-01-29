using AgroTemp.Domain.Context;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.API.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                                   ?? configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Database connection string is missing.");
            }

            services.AddDbContext<AgroTempDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }
    }
}
