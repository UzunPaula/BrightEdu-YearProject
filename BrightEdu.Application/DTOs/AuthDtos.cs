using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.DTOs;

public sealed record RegisterRequestDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    LanguageCode PreferredLanguage);

public sealed record LoginRequestDto(
    string Email,
    string Password);

public sealed record AuthResponseDto(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    DateTime ExpiresAt,
    IReadOnlyList<string> Roles,
    LanguageCode PreferredLanguage);

public sealed record UserProfileDto(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    LanguageCode PreferredLanguage);

public sealed record UpdateProfileRequest(
    string FirstName,
    string LastName,
    LanguageCode PreferredLanguage);
