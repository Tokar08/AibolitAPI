using AibolitAPI.Models;

namespace AibolitAPI.Interfaces;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<string> UploadPhotoAsync(Stream photoStream, string fileName);
    Task DeletePhotoAsync(string photoUrl);
}