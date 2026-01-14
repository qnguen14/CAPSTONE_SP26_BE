using AgroTemp.Domain.Context;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.API.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string 'DefaultConnection' is not configured.");
            }

            services.AddDbContext<AgroTempDbContext>(options =>
                options.UseNpgsql(connectionString));
            return services;
        }
    }
}
