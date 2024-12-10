namespace AibolitAPI.Interfaces;

public interface ICloudStorageService
{
    Task<string> UploadPhotoAsync(Stream stream, string fileName);
    Task DeletePhotoAsync(string fileUrl);
}