using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<Doctor>> GetDoctorsByUserIdsAsync(List<Guid> userIds)
    {
        return await _context.Doctors
            .Where(doctor => userIds.Contains(doctor.UserId))
            .ToListAsync();
    }

    public override async Task<IEnumerable<Doctor>> GetAllAsync(int page, int size)
    {
        return await _context.Doctors
            .Include(d => d.Specialization)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    public override async Task<Doctor> GetByIdAsync(Guid id)
    {
        return await _context.Doctors
                   .Include(d => d.Specialization)
                   .FirstOrDefaultAsync(d => d.Id == id)
               ?? throw new KeyNotFoundException($"Doctor with ID {id} not found.");
    }
}