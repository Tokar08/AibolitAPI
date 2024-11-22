using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class PrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PrescriptionService> _logger;

    public PrescriptionService(IPrescriptionRepository prescriptionRepository, IMapper mapper, ILogger<PrescriptionService> logger)
    {
        _prescriptionRepository = prescriptionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PrescriptionDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var prescriptions = await _prescriptionRepository.GetAllAsync(page, size);
            return _mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all prescriptions.");
            throw;
        }
    }

    public async Task<PrescriptionDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);
            return _mapper.Map<PrescriptionDTO>(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting prescription with ID: {id}");
            throw;
        }
    }

    public async Task CreateAsync(PrescriptionDTO prescriptionDto)
    {
        try
        {
            var prescription = _mapper.Map<Prescription>(prescriptionDto);
            await _prescriptionRepository.CreateAsync(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating prescription.");
            throw;
        }
    }

    public async Task UpdateAsync(PrescriptionDTO prescriptionDto)
    {
        try
        {
            var prescription = _mapper.Map<Prescription>(prescriptionDto);
            await _prescriptionRepository.UpdateAsync(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating prescription.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _prescriptionRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting prescription with ID: {id}");
            throw;
        }
    }
}