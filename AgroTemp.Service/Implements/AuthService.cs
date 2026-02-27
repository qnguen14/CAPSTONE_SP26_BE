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

    public AuthService(
        IUnitOfWork<AgroTempDbContext> unitOfWork, 
        IHttpContextAccessor httpContextAccessor, 
        IMapperlyMapper mapper,
        IConfiguration configuration) : base(unitOfWork, httpContextAccessor, mapper)
    {
        _configuration = configuration;
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
            Email = user.Email
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
            IsActive = true
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
            Email = user.Email
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
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = payload.Email,
                    PhoneNumber = "0000000000", // Default placeholder - user can update later
                    Address = "Not specified", // Default placeholder
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Random password
                    RoleId = request.RoleId ?? 2, // Default to Farmer role if not specified
                    Role = (UserRole)(request.RoleId ?? 2),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
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
                Email = user.Email
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
}