using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class AdministratorService
{
    private readonly IAdministratorRepository _administratorRepository;
    private readonly IHospitalRepository _hospitalRepository;
    private readonly ILogger<AdministratorService> _logger;
    private readonly IMapper _mapper;

    public AdministratorService(IAdministratorRepository administratorRepository,
        IHospitalRepository hospitalRepository,
        IMapper mapper, ILogger<AdministratorService> logger)
    {
        _administratorRepository = administratorRepository;
        _hospitalRepository = hospitalRepository;
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
            _logger.LogInformation("Начало создания администратора.");

            // Находим больницу, к которой будем привязывать администратора
            _logger.LogInformation("Попытка найти больницу с ID: {ManagedHospitalId}",
                administratorDto.ManagedHospitalId);
            var hospital = await _hospitalRepository.GetByIdAsync(administratorDto.ManagedHospitalId);
            if (hospital == null)
            {
                _logger.LogError("Больница с ID {ManagedHospitalId} не найдена.", administratorDto.ManagedHospitalId);
                throw new Exception("Hospital not found.");
            }

            _logger.LogInformation("Больница с ID: {ManagedHospitalId} найдена.", administratorDto.ManagedHospitalId);

            // Создаем администратора
            _logger.LogInformation("Маппинг AdministratorDTO в Administrator.");
            var administrator = _mapper.Map<Administrator>(administratorDto);
            _logger.LogInformation("Маппинг успешно выполнен. Администратор готов к сохранению.");

            // Сохраняем администратора
            _logger.LogInformation("Попытка сохранить администратора с ID: {AdministratorId}.", administrator.Id);
            await _administratorRepository.CreateAsync(administrator);
            _logger.LogInformation("Администратор с ID: {AdministratorId} успешно сохранен.", administrator.Id);

            // Добавляем администратора в коллекцию больницы
            _logger.LogInformation("Попытка добавить администратора в коллекцию больницы.");
            hospital.Administrators.Add(administrator);
            _logger.LogInformation("Администратор добавлен в коллекцию больницы.");

            // Обновляем больницу и сохраняем изменения
            _logger.LogInformation("Попытка обновить больницу с ID: {ManagedHospitalId}.",
                administratorDto.ManagedHospitalId);
            await _hospitalRepository.UpdateAsync(hospital);
            _logger.LogInformation("Больница с ID: {ManagedHospitalId} успешно обновлена.",
                administratorDto.ManagedHospitalId);

            _logger.LogInformation("Администратор успешно создан.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании администратора.");
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