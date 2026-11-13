using FixNear.Application.Models;

namespace FixNear.Application.Interfaces
{
    public interface IPhotoStorageService
    {
        Task<PhotoUploadResult> UploadFileAsync(Stream file, string fileName, string folder = "fixnear/providers");
    }
}
