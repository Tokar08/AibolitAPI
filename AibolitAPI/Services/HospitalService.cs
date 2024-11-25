using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AibolitAPI.Services;

public class HospitalService
{
    private readonly IHospitalRepository _hospitalRepository;
    private readonly ILogger<HospitalService> _logger;
    private readonly IMapper _mapper;

    public HospitalService(IHospitalRepository hospitalRepository, IMapper mapper, ILogger<HospitalService> logger)
    {
        _hospitalRepository = hospitalRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<HospitalDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var hospitals = await _hospitalRepository.GetAllAsync(page, size,
                q => q
                    .Include(h => h.Administrators)
                    .Include(h => h.Doctors));

            return _mapper.Map<IEnumerable<HospitalDTO>>(hospitals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all hospitals.");
            throw;
        }
    }

    public async Task<HospitalDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var hospital = await _hospitalRepository.GetByIdAsync(id);
            return _mapper.Map<HospitalDTO>(hospital);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting hospital with ID: {id}");
            throw;
        }
    }

    public async Task CreateAsync(HospitalDTO hospitalDto)
    {
        try
        {
            var hospital = _mapper.Map<Hospital>(hospitalDto);
            await _hospitalRepository.CreateAsync(hospital);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating hospital.");
            throw;
        }
    }

    public async Task UpdateAsync(HospitalDTO hospitalDto)
    {
        try
        {
            var hospital = _mapper.Map<Hospital>(hospitalDto);
            await _hospitalRepository.UpdateAsync(hospital);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating hospital.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _hospitalRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting hospital with ID: {id}");
            throw;
        }
    }
}