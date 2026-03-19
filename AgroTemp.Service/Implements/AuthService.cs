using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Auth;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;

namespace AgroTemp.Service.Interfaces;

public class AuthService : BaseService<User>, IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork<AgroTempDbContext> unitOfWork, 
        IHttpContextAccessor httpContextAccessor, 
        IMapperlyMapper mapper,
        IConfiguration configuration,
        IEmailService emailService,
        ILogger<AuthService> logger) : base(unitOfWork, httpContextAccessor, mapper)
    {
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.PhoneNumber))
        {
            throw new ArgumentException("Either email or phone number must be provided");
        }

        User? user = null;

        // Find user by email or phone number
        if (!string.IsNullOrEmpty(request.Email))
        {
            var usersByEmail = await _unitOfWork.GetRepository<User>()
                .GetListAsync(predicate: u => u.Email == request.Email && u.IsActive);
            user = usersByEmail.FirstOrDefault();
        }
        
        // Fallback to phone number if email search didn't find a user
        if (user == null && !string.IsNullOrEmpty(request.PhoneNumber))
        {
            var usersByPhone = await _unitOfWork.GetRepository<User>()
                .GetListAsync(predicate: u => u.PhoneNumber == request.PhoneNumber && u.IsActive);
            user = usersByPhone.FirstOrDefault();
        }

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email/phone or password");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email/phone or password");
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<LoginResponse> Register(RegisterRequest request)
    {
        // Check if user already exists
        var existingUsers = await _unitOfWork.GetRepository<User>()
            .GetListAsync(predicate: u => u.Email == request.Email);
        
        var existingUser = existingUsers.FirstOrDefault();

        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Check phone number
        var existingPhones = await _unitOfWork.GetRepository<User>()
            .GetListAsync(predicate: u => u.PhoneNumber == request.PhoneNumber);
        
        var existingPhone = existingPhones.FirstOrDefault();

        if (existingPhone != null)
        {
            throw new InvalidOperationException("User with this phone number already exists");
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = request.RoleId,
            Role = (UserRole)request.RoleId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsVerified = true,
        };
            

        await _unitOfWork.GetRepository<User>().InsertAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Generate JWT token
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<LoginResponse> GoogleLogin(GoogleLoginRequest request)
    {
        try
        {
            // Verify Google token
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _configuration["Authentication:Google:ClientId"]! }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.GoogleToken, settings);

            if (payload == null || string.IsNullOrEmpty(payload.Email))
            {
                throw new UnauthorizedAccessException("Invalid Google token");
            }

            // Check if user exists
            var existingUsers = await _unitOfWork.GetRepository<User>()
                .GetListAsync(predicate: u => u.Email == payload.Email);
            
            var user = existingUsers.FirstOrDefault();

            if (user == null)
            {
                // Create new user from Google account
                var newUserId = Guid.NewGuid();
                // Generate a unique 10-digit placeholder phone number from the user's Guid
                // so multiple Google sign-ups never collide on a unique-phone constraint.
                var uniquePhone = string.Concat(
                    newUserId.ToString("N").Where(char.IsDigit).Take(10)
                ).PadRight(10, '0');

                user = new User
                {
                    Id = newUserId,
                    Email = payload.Email,
                    PhoneNumber = uniquePhone,   // Unique per user — update later via profile
                    Address = "Not specified",   // Placeholder — user can update later
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                    RoleId = request.RoleId ?? 2, // Default to Farmer role if not specified
                    Role = (UserRole)(request.RoleId ?? 2),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsVerified = true  // Google has already verified the email address,
                };

                await _unitOfWork.GetRepository<User>().InsertAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            else if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Account is deactivated");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddHours(24);

            return new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
        catch (InvalidJwtException)
        {
            throw new UnauthorizedAccessException("Invalid Google token");
        }
    }

    public async Task Logout(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if(!handler.CanReadToken(token)){
            throw new ArgumentException("Invalid token format");
        }

        var jwtToken = handler.ReadJwtToken(token);
        var jti = jwtToken.Id;

        if(string.IsNullOrEmpty(jti))
        {
            jti = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(token)
                )
            );
        }

        var expiresAt = jwtToken.ValidTo == DateTime.MinValue ? DateTime.UtcNow.AddHours(24) : jwtToken.ValidTo;

        var blacklistedToken = new BlacklistedToken
        {
            TokenId = jti,
            ExpiresAt = expiresAt,
            BlacklistedAt = DateTime.UtcNow
        };

        await _unitOfWork.GetRepository<BlacklistedToken>().InsertAsync(blacklistedToken);
        await _unitOfWork.SaveChangesAsync();
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("RoleId", user.RoleId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public async Task ForgotPassword(ForgotPasswordRequest request)
    {
        var user = (await _unitOfWork.GetRepository<User>()
            .GetListAsync(predicate: u => u.Email == request.Email && u.IsActive)).FirstOrDefault();

        if (user == null)
        {
            // Don't reveal if user exists or not for security
            _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
            return; // Silently succeed
        }

        // Generate 6-digit OTP
        user.PasswordResetToken = new Random().Next(100000, 999999).ToString();
        user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(15);

        _unitOfWork.GetRepository<User>().UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Send password reset email
        try
        {
            await _emailService.SendEmailAsync(user.Email, "AgroTemp Password Reset",
                $"<div style=\"text-align: center;\"><h2>Password Reset Code</h2><h1>{user.PasswordResetToken}</h1><p>This code will expire in 15 minutes.</p><p>If you didn't request this, please ignore this email.</p></div>");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
            throw;
        }
    }

    public async Task<bool> ResetPassword(ResetPasswordRequest request)
    {
        var user = (await _unitOfWork.GetRepository<User>()
            .GetListAsync(predicate: u => u.Email == request.Email)).FirstOrDefault();

        if (user == null)
        {
            return false;
        }

        if (user.PasswordResetToken != request.Otp)
        {
            return false;
        }

        if (user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        // Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;

        _unitOfWork.GetRepository<User>().UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}