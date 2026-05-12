using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.Auth;

public interface IUserProfileService
{
    Task<UserProfileDto> GetAsync(Guid userId, CancellationToken ct = default);
    Task<UserProfileDto> UpdateAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default);
}

public sealed class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _userRepository;

    public UserProfileService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct)
            ?? throw new InvalidOperationException("Utilizatorul nu a fost găsit.");

        return new UserProfileDto(user.Id, user.Email, user.FirstName, user.LastName, user.PreferredLanguage);
    }

    public async Task<UserProfileDto> UpdateAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct)
            ?? throw new InvalidOperationException("Utilizatorul nu a fost găsit.");

        user.UpdateProfile(request.FirstName, request.LastName, request.PreferredLanguage);
        await _userRepository.SaveChangesAsync(ct);

        return new UserProfileDto(user.Id, user.Email, user.FirstName, user.LastName, user.PreferredLanguage);
    }
}
