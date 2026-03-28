using AgroTemp.Domain.Context;
using AgroTemp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.API.Configuration;

public static class AdminSeedConfiguration
{
    public static async Task EnsureAdminSeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdminSeed");
        var db = scope.ServiceProvider.GetRequiredService<AgroTempDbContext>();

        var enabled = configuration.GetValue<bool>("AdminSeed:Enabled");
        if (!enabled)
        {
            logger.LogInformation("Admin seed is disabled.");
            return;
        }

        var email = configuration["AdminSeed:Email"]?.Trim();
        var phone = configuration["AdminSeed:PhoneNumber"]?.Trim();
        var password = configuration["AdminSeed:Password"];
        var address = configuration["AdminSeed:Address"]?.Trim();

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(phone) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(address))
        {
            logger.LogWarning("Admin seed is enabled but missing required values.");
            return;
        }

        if (phone.Length != 10)
        {
            logger.LogWarning("Admin seed phone number must be exactly 10 characters.");
            return;
        }

        var existingByEmail = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingByEmail != null)
        {
            existingByEmail.RoleId = (int)UserRole.Admin;
            existingByEmail.IsActive = true;
            existingByEmail.IsVerified = true;
            // existingByEmail.Address = address;
            existingByEmail.PhoneNumber = phone;

            if (!BCrypt.Net.BCrypt.Verify(password, existingByEmail.PasswordHash))
            {
                existingByEmail.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            }

            var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == existingByEmail.Id);
            if (wallet == null)
            {
                db.Wallets.Add(new Wallet
                {
                    Id = Guid.NewGuid(),
                    UserId = existingByEmail.Id,
                    Balance = 0,
                    LockedBalance = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync();
            logger.LogInformation("Admin account ensured from existing email.");
            return;
        }

        var existingByPhone = await db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        if (existingByPhone != null)
        {
            existingByPhone.RoleId = (int)UserRole.Admin;
            existingByPhone.IsActive = true;
            existingByPhone.IsVerified = true;
            // existingByPhone.Address = address;
            existingByPhone.Email = email;

            if (!BCrypt.Net.BCrypt.Verify(password, existingByPhone.PasswordHash))
            {
                existingByPhone.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            }

            var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == existingByPhone.Id);
            if (wallet == null)
            {
                db.Wallets.Add(new Wallet
                {
                    Id = Guid.NewGuid(),
                    UserId = existingByPhone.Id,
                    Balance = 0,
                    LockedBalance = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync();
            logger.LogInformation("Admin account ensured from existing phone number.");
            return;
        }

        var userId = Guid.NewGuid();
        db.Users.Add(new User
        {
            Id = userId,
            Email = email,
            PhoneNumber = phone,
            // Address = address,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            RoleId = (int)UserRole.Admin,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsVerified = true
        });

        db.Wallets.Add(new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Balance = 0,
            LockedBalance = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        logger.LogInformation("Admin account seeded successfully.");
    }
}

