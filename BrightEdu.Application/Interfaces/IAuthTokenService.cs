namespace BrightEdu.Application.Interfaces;

public interface IAuthTokenService
{
    string CreateAccessToken(Guid userId, string email, IReadOnlyList<string> roles, DateTime expiresAt);
}
