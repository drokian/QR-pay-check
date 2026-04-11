using Microsoft.AspNetCore.Hosting;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Infrastructure.Services;

/// <summary>
/// Görselleri wwwroot/uploads klasörüne kaydeder.
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        _env = env;
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

        var webRootPath = !string.IsNullOrWhiteSpace(_env.WebRootPath)
            ? _env.WebRootPath
            : Path.Combine(_env.ContentRootPath, "wwwroot");

        var absolutePath = Path.Combine(webRootPath, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        await using var fileStream = File.Create(absolutePath);
        await content.CopyToAsync(fileStream, ct);

        return $"/{relativePath.Replace(Path.DirectorySeparatorChar, '/')}";
    }
}
