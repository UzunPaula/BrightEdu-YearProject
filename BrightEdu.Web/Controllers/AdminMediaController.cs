using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/media")]
public class AdminMediaController : ControllerBase
{
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml",
        "application/pdf", "video/mp4", "video/webm"
    };
    private const long MaxBytes = 50 * 1024 * 1024; // 50 MB

    private readonly IAdminMediaService _service;

    public AdminMediaController(IAdminMediaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AdminMediaAssetDto>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpPost("upload")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult<AdminMediaAssetDto>> Upload(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Niciun fișier primit." });

        if (file.Length > MaxBytes)
            return BadRequest(new { message = "Fișierul depășește limita de 50 MB." });

        if (!AllowedMimeTypes.Contains(file.ContentType))
            return BadRequest(new { message = $"Tipul '{file.ContentType}' nu este permis." });

        await using var stream = file.OpenReadStream();
        var dto = await _service.UploadAsync(stream, file.FileName, file.ContentType, file.Length, ct);
        return Ok(dto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
