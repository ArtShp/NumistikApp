using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Entities;
using Server.Models.Auth;
using Server.Models.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Server.Services;

public class AuthService(MyDbContext context, IConfiguration configuration) : IAuthService
{
    public async Task<RefreshTokenDto.Response?> LoginAsync(UserLoginDto.Request request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null) return null;

        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return await CreateTokenResponse(user);
    }

    private async Task<RefreshTokenDto.Response> CreateTokenResponse(User user)
    {
        return new RefreshTokenDto.Response
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
        };
    }

    public async Task<UserRegistrationDto.Response?> RegisterAsync(UserRegistrationDto.Request request)
    {
        async Task<User> CreateUserWithRoleAsync(UserAppRole assignedRole)
        {
            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;
            user.Role = assignedRole;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        async Task CreateOwnerAccessTokenAsync(Guid token, Guid userId)
        {
            var time = DateTime.UtcNow;

            var inviteToken = new InviteToken
            {
                Token = token,
                CreatedById = userId,
                CreatedAt = time,
                ExpiresAt = time,
                UsedAt = time,
                UsedById = userId,
                AssignedRole = UserAppRole.Owner
            };

            context.InviteTokens.Add(inviteToken);

            await context.SaveChangesAsync();
        }

        if (request.InviteToken is null)
        {
            if (!await context.Users.AnyAsync())
            {
                if (string.IsNullOrEmpty(configuration["AppSettings:OwnerInviteToken"]))
                    return null;

                if (!Guid.TryParse(configuration["AppSettings:OwnerInviteToken"]!, out var inviteToken))
                    return null;

                request.InviteToken = inviteToken;

                var user = await CreateUserWithRoleAsync(UserAppRole.Owner);
                await CreateOwnerAccessTokenAsync(inviteToken, user.Id);

                return new UserRegistrationDto.Response
                {
                    Role = user.Role
                };
            }
            else
            {
                return null;
            }
        }
        else
        {
            var inviteTokenEntity = await context.InviteTokens
            .FirstOrDefaultAsync(t => t.Token == request.InviteToken);

            if (inviteTokenEntity is null || inviteTokenEntity.UsedAt is not null || inviteTokenEntity.ExpiresAt < DateTime.UtcNow)
                return null;

            if (await context.Users.AnyAsync(u => u.Username == request.Username))
                return null;

            var user = await CreateUserWithRoleAsync(inviteTokenEntity.AssignedRole);

            inviteTokenEntity.UsedAt = DateTime.UtcNow;
            inviteTokenEntity.UsedById = user.Id;

            context.InviteTokens.Update(inviteTokenEntity);
            await context.SaveChangesAsync();

            return new UserRegistrationDto.Response
            {
                Role = user.Role
            };
        }
    }

    public async Task<RefreshTokenDto.Response?> RefreshTokensAsync(RefreshTokenDto.Request request)
    {
        var user = await ValidateRefreshTokenAsync(request.Username, request.RefreshToken);

        if (user is null) return null;

        return await CreateTokenResponse(user);
    }

    private async Task<User?> ValidateRefreshTokenAsync(string username, string refreshToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user is null
            || new PasswordHasher<User>().VerifyHashedPassword(user, user.RefreshTokenHash!, refreshToken)
                == PasswordVerificationResult.Failed
            || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        return user;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
    {
        var refreshToken = GenerateRefreshToken();

        user.RefreshTokenHash = new PasswordHasher<User>()
            .HashPassword(user, refreshToken);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await context.SaveChangesAsync();

        return refreshToken;
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration["AppSettings:Issuer"],
            audience: configuration["AppSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    public async Task<InviteTokenDto.Response?> CreateInviteTokenAsync(Guid createdById, UserAppRole assignedRole)
    {
        var token = new InviteToken
        {
            CreatedById = createdById,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            AssignedRole = assignedRole
        };

        context.InviteTokens.Add(token);

        await context.SaveChangesAsync();

        return new InviteTokenDto.Response
        {
            Token = token.Token,
            AssignedRole = assignedRole,
            ExpirationDate = token.ExpiresAt
        };
    }
}
