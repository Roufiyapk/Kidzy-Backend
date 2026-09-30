using Microsoft.AspNetCore.Http;

namespace Kidzy.Application.Interfaces.Services;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile image);

    Task<bool> DeleteImageAsync(string publicId);
}