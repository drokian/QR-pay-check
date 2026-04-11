using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Infrastructure.Services;

/// <summary>
/// Görselleri wwwroot/uploads klasörüne kaydeder.
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalFileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken ct = default)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var uniqueName = $"{Guid.CreateVersion7()}{ext}";

        var now = DateTime.UtcNow;
        var relativePath = Path.Combine("uploads", now.Year.ToString(), now.Month.ToString("D2"), uniqueName);
        var absolutePath = Path.Combine(_env.WebRootPath, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        await using var fileStream = File.Create(absolutePath);
        await content.CopyToAsync(fileStream, ct);

        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = request is not null
            ? $"{request.Scheme}://{request.Host}"
            : string.Empty;

        return $"{baseUrl}/{relativePath.Replace(Path.DirectorySeparatorChar, '/')}";
    }
}
