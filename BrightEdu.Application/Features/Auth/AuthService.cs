using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default);
}

public sealed class AuthService : IAuthService
{
    private const string StudentRoleName = "Student";

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IAuthTokenService _authTokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        IAuthTokenService authTokenService)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _authTokenService = authTokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (existingUser is not null)
            throw new InvalidOperationException("Există deja un cont cu acest email.");

        var user = new User(
            Guid.NewGuid(),
            request.Email,
            _passwordHashService.Hash(request.Password),
            request.FirstName,
            request.LastName,
            request.PreferredLanguage);

        await _userRepository.AddAsync(user, ct);

        var expiresAt = DateTime.UtcNow.AddMinutes(60);
        var roles = new[] { StudentRoleName };
        var accessToken = _authTokenService.CreateAccessToken(user.Id, user.Email, roles, expiresAt);

        return new AuthResponseDto(user.Id, user.Email, user.FirstName, user.LastName, accessToken, expiresAt, roles, user.PreferredLanguage);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null || !_passwordHashService.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email sau parolă invalidă.");

        var roles = user.UserRoles.Select(x => x.Role.Name).DefaultIfEmpty(StudentRoleName).ToArray();
        var expiresAt = DateTime.UtcNow.AddMinutes(60);
        var accessToken = _authTokenService.CreateAccessToken(user.Id, user.Email, roles, expiresAt);

        return new AuthResponseDto(user.Id, user.Email, user.FirstName, user.LastName, accessToken, expiresAt, roles, user.PreferredLanguage);
    }
}
