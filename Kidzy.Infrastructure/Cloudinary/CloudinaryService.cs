using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Kidzy.Infrastructure.Cloudinary;

public class CloudinaryService : ICloudinaryService
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;

    public CloudinaryService(
        IConfiguration configuration)
    {
        var cloudName =
            configuration["CloudinarySettings:CloudName"];

        var apiKey =
            configuration["CloudinarySettings:ApiKey"];

        var apiSecret =
            configuration["CloudinarySettings:ApiSecret"];

        var account = new Account(
            cloudName,
            apiKey,
            apiSecret);

        _cloudinary =
            new CloudinaryDotNet.Cloudinary(account);
    }

    // UPLOAD IMAGE

    public async Task<string> UploadImageAsync(
        IFormFile image)
    {
        if (image == null ||
            image.Length == 0)
        {
            throw new ArgumentException(
                "Image is required.");
        }

        await using var stream =
            image.OpenReadStream();

        var uploadParams =
            new ImageUploadParams
            {
                File =
                    new FileDescription(
                        image.FileName,
                        stream),

                Folder = "kidzy/products"
            };

        var result =
            await _cloudinary.UploadAsync(
                uploadParams);

        if (result.Error != null)
        {
            throw new Exception(
                result.Error.Message);
        }

        return result.SecureUrl.ToString();
    }

    // DELETE IMAGE

    public async Task<bool> DeleteImageAsync(
        string publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
        {
            return false;
        }

        var deleteParams =
            new DeletionParams(publicId);

        var result =
            await _cloudinary.DestroyAsync(
                deleteParams);

        return result.Result == "ok";
    }
}