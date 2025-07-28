using Server.Entities;
using Server.Models;

namespace Server.Services;

public interface IAuthService
{
    Task<UserRegistrationResponseDto?> RegisterAsync(UserRegistrationDto request);
    Task<TokenResponseDto?> LoginAsync(UserDto request);
    Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    Task<InviteTokenResponseDto?> CreateInviteTokenAsync(Guid createdById, Role assignedRole);
}
