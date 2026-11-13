namespace FixNear.Application.DTOs.Media
{
    /// <summary>
    /// Rich response contract for all Cloudinary image uploads.
    /// Provides standard Cloudinary metadata alongside convenience aliases.
    /// </summary>
    public class ImageUploadResponse
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string SecureUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string ResourceType { get; set; } = "image";
        public int Width { get; set; }
        public int Height { get; set; }
        public long Bytes { get; set; }
        public string Folder { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Image uploaded successfully to Cloudinary";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
