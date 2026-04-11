using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRPayCheck.Application.Common.Interfaces;
using Wolverine.Http;

namespace QRPayCheck.API.Endpoints;

public static class FileEndpoints
{
    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg", "image/png", "image/webp", "image/gif"
    ];

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    [WolverinePost("/api/files/upload")]
    [Authorize]
    [RequestSizeLimit(5_242_880)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5_242_880)]
    public static async Task<IResult> Upload(
        IFormFile file,
        IFileStorageService fileStorage,
        CancellationToken ct)
    {
        if (file.Length == 0)
            return Results.BadRequest("Dosya boş olamaz.");

        if (file.Length > MaxFileSizeBytes)
            return Results.BadRequest("Dosya boyutu 5 MB'ı geçemez.");

        if (!AllowedContentTypes.Contains(file.ContentType))
            return Results.BadRequest("Yalnızca JPEG, PNG, WebP ve GIF dosyaları kabul edilir.");

        if (!await IsValidImageAsync(file, ct))
            return Results.BadRequest("Dosya içeriği belirtilen türle eşleşmiyor.");

        await using var stream = file.OpenReadStream();
        var url = await fileStorage.SaveAsync(stream, file.FileName, file.ContentType, ct);

        return Results.Ok(new { url });
    }

    private static async Task<bool> IsValidImageAsync(IFormFile file, CancellationToken ct)
    {
        const int headerSize = 12;
        var header = new byte[headerSize];

        await using var stream = file.OpenReadStream();
        var bytesRead = await stream.ReadAsync(header.AsMemory(0, headerSize), ct);

        if (bytesRead < 3) return false;

        return file.ContentType switch
        {
            "image/jpeg" =>
                header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            "image/png" =>
                bytesRead >= 4 &&
                header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47,
            "image/gif" =>
                bytesRead >= 6 &&
                header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38 &&
                (header[4] == 0x37 || header[4] == 0x39) && header[5] == 0x61,
            "image/webp" =>
                bytesRead >= 12 &&
                header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50,
            _ => false
        };
    }
}
