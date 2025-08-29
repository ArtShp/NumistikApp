using Server.Models.Auth;
using Server.Models.Common;

namespace Server.Services;

public interface IAuthService
{
    Task<UserRegistrationDto.Response?> RegisterAsync(UserRegistrationDto.Request request);
    Task<RefreshTokenDto.Response?> LoginAsync(UserLoginDto.Request request);
    Task<RefreshTokenDto.Response?> RefreshTokensAsync(RefreshTokenDto.Request request);
    Task<InviteTokenDto.Response?> CreateInviteTokenAsync(Guid createdById, UserAppRole assignedRole);
}
