namespace QRPayCheck.Application.Common.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Dosyayı kaydeder ve erişim URL'ini döner.
    /// </summary>
    Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken ct = default);
}
