using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;

namespace AibolitAPI.Repositories;

public class DoctorRepository : Repository<Doctor>, IDoctorRepository
{
    private readonly ICloudStorageService _cloudStorageService;

    public DoctorRepository(AibolitDbContext context, ICloudStorageService cloudStorageService)
        : base(context)
    {
        _cloudStorageService = cloudStorageService;
    }

    public async Task<string> UploadPhotoAsync(Stream photoStream, string fileName)
    {
        try
        {
            return await _cloudStorageService.UploadPhotoAsync(photoStream, fileName);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error uploading photo.", ex);
        }
    }

    public async Task DeletePhotoAsync(string photoUrl)
    {
        try
        {
            await _cloudStorageService.DeletePhotoAsync(photoUrl);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error deleting photo from cloud storage.", ex);
        }
    }
}