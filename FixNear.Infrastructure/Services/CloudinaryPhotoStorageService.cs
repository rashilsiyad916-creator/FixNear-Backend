using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FixNear.Application.Exceptions;
using FixNear.Application.Interfaces;
using FixNear.Application.Models;
using Microsoft.Extensions.Configuration;

namespace FixNear.Infrastructure.Services
{
    public class CloudinaryPhotoStorageService : IPhotoStorageService
    {
        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

        private readonly IConfiguration _config;

        public CloudinaryPhotoStorageService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<PhotoUploadResult> UploadFileAsync(
            Stream file,
            string fileName,
            string folder = "fixnear/providers")
        {
            var section = _config.GetSection("Cloudinary");
            var cloudName = section["CloudName"];
            var apiKey = section["ApiKey"];
            var secret = section["ApiSecret"] ?? section["Secret"];

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(secret))
            {
                throw new BadRequestException(
                    "Cloudinary is not configured. Set Cloudinary:CloudName, Cloudinary:ApiKey and Cloudinary:ApiSecret.");
            }

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
                throw new BadRequestException("Only JPG, PNG or WEBP images are allowed");

            if (file.CanSeek)
            {
                if (file.Length == 0)
                    throw new BadRequestException("The uploaded image is empty");
                file.Position = 0;
            }

            var cloudinary = new Cloudinary(new Account(cloudName, apiKey, secret))
            {
                Api = { Secure = true }
            };

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, file),
                Folder = folder,
                Overwrite = false,
                UniqueFilename = true
            };

            ImageUploadResult result;
            try
            {
                result = await cloudinary.UploadAsync(uploadParams);
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Cloudinary upload failed: {ex.Message}");
            }

            if (!string.IsNullOrWhiteSpace(result.Error?.Message))
                throw new BadRequestException($"Cloudinary upload failed: {result.Error.Message}");

            if (result.SecureUrl == null || string.IsNullOrWhiteSpace(result.SecureUrl.ToString()))
                throw new BadRequestException("Cloudinary did not return a secure image URL");

            return new PhotoUploadResult(
                PublicId: result.PublicId ?? string.Empty,
                Url: result.SecureUrl.ToString(),
                Format: result.Format ?? extension.TrimStart('.').ToLowerInvariant(),
                Width: result.Width,
                Height: result.Height,
                Bytes: result.Bytes,
                Folder: folder);
        }
    }
}
