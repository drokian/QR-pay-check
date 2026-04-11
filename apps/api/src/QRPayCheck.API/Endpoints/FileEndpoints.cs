using Microsoft.AspNetCore.Authorization;
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

        await using var stream = file.OpenReadStream();
        var url = await fileStorage.SaveAsync(stream, file.FileName, file.ContentType, ct);

        return Results.Ok(new { url });
    }
}
