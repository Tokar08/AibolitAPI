using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class AdministratorService
{
    private readonly IAdministratorRepository _administratorRepository;
    private readonly ILogger<AdministratorService> _logger;
    private readonly IMapper _mapper;

    public AdministratorService(IAdministratorRepository administratorRepository, IMapper mapper,
        ILogger<AdministratorService> logger)
    {
        _administratorRepository = administratorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AdministratorDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var administrators = await _administratorRepository.GetAllAsync(page, size);
            return _mapper.Map<IEnumerable<AdministratorDTO>>(administrators);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all administrators.");
            throw;
        }
    }

    public async Task<AdministratorDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var administrator = await _administratorRepository.GetByIdAsync(id);
            return _mapper.Map<AdministratorDTO>(administrator);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting administrator with ID: {id}");
            throw;
        }
    }

    public async Task CreateAsync(AdministratorDTO administratorDto)
    {
        try
        {
            var administrator = _mapper.Map<Administrator>(administratorDto);
            await _administratorRepository.CreateAsync(administrator);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating administrator.");
            throw;
        }
    }

    public async Task UpdateAsync(AdministratorDTO administratorDto)
    {
        try
        {
            var administrator = _mapper.Map<Administrator>(administratorDto);
            await _administratorRepository.UpdateAsync(administrator);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating administrator.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _administratorRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting administrator with ID: {id}");
            throw;
        }
    }
}