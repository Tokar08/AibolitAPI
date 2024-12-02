using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class MedicalRecordService
{
    private readonly ILogger<MedicalRecordService> _logger;
    private readonly IMapper _mapper;
    private readonly IMedicalRecordRepository _medicalRecordRepository;

    public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository, IMapper mapper,
        ILogger<MedicalRecordService> logger)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<MedicalRecordDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var medicalRecords = await _medicalRecordRepository.GetAllAsync(page, size
            );
            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(medicalRecords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all medical records.");
            _logger.LogError(ex,
                $"Error occurred in {nameof(GetAllAsync)} method: {ex.Message}. StackTrace: {ex.StackTrace}");

            throw;
        }
    }

    public async Task<MedicalRecordDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(id);
            return _mapper.Map<MedicalRecordDTO>(medicalRecord);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting medical record with ID: {id}");
            throw;
        }
    }

    public async Task CreateAsync(MedicalRecordDTO medicalRecordDto)
    {
        try
        {
            var medicalRecord = _mapper.Map<MedicalRecord>(medicalRecordDto);
            await _medicalRecordRepository.CreateAsync(medicalRecord);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating medical record.");
            throw;
        }
    }

    public async Task UpdateAsync(MedicalRecordDTO medicalRecordDto)
    {
        try
        {
            var medicalRecord = _mapper.Map<MedicalRecord>(medicalRecordDto);
            await _medicalRecordRepository.UpdateAsync(medicalRecord);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating medical record.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _medicalRecordRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting medical record with ID: {id}");
            throw;
        }
    }
}