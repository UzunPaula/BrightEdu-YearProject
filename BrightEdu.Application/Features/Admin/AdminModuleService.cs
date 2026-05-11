using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.Features.Admin;

public interface IAdminModuleService
{
    Task<IReadOnlyList<AdminModuleDto>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default);
    Task<AdminModuleDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AdminModuleDto> CreateAsync(CreateModuleRequest request, CancellationToken ct = default);
    Task<AdminModuleDto?> UpdateAsync(Guid id, UpdateModuleRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class AdminModuleService : IAdminModuleService
{
    private readonly IAdminModuleRepository _repository;

    public AdminModuleService(IAdminModuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<AdminModuleDto>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default)
    {
        var modules = await _repository.GetByCourseIdAsync(courseId, ct);
        return modules.OrderBy(x => x.Order).Select(Map).ToList().AsReadOnly();
    }

    public async Task<AdminModuleDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var module = await _repository.GetByIdAsync(id, ct);
        return module is null ? null : Map(module);
    }

    public async Task<AdminModuleDto> CreateAsync(CreateModuleRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Titlul modulului este obligatoriu.");

        if (request.Order <= 0)
            throw new ArgumentException("Ordinea trebuie să fie mai mare ca 0.");

        var module = new Module(Guid.NewGuid(), request.CourseId, request.Order, request.Title, request.Description);

        await _repository.AddAsync(module, ct);
        await _repository.SaveAsync(ct);

        return Map(module);
    }

    public async Task<AdminModuleDto?> UpdateAsync(Guid id, UpdateModuleRequest request, CancellationToken ct = default)
    {
        var module = await _repository.GetByIdAsync(id, ct);
        if (module is null) return null;

        module.AddTranslation(LanguageCode.Ro, request.Title, request.Description);

        await _repository.SaveAsync(ct);
        return Map(module);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var module = await _repository.GetByIdAsync(id, ct);
        if (module is null) return false;

        await _repository.RemoveAsync(module, ct);
        await _repository.SaveAsync(ct);
        return true;
    }

    private static AdminModuleDto Map(Module module)
    {
        var translation = module.GetTranslation(LanguageCode.Ro) ?? module.Translations.FirstOrDefault();
        return new AdminModuleDto(
            module.Id,
            module.CourseId,
            module.Order,
            translation?.Title ?? string.Empty,
            translation?.Description,
            module.State.ToString(),
            module.Lessons.Count);
    }
}
