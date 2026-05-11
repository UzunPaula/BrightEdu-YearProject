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
    string AccessToken,
    DateTime ExpiresAt,
    IReadOnlyList<string> Roles);
