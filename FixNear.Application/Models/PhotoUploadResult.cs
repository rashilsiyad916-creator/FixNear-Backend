namespace FixNear.Application.Models
{
    /// <summary>
    /// Details returned by the photo storage provider after a successful upload.
    /// Keeping the Cloudinary metadata (public id, size, dimensions) means callers
    /// can store or display more than just a URL without re-querying the CDN.
    /// </summary>
    public sealed record PhotoUploadResult(
        string PublicId,
        string Url,
        string Format,
        int Width,
        int Height,
        long Bytes,
        string Folder);
}
