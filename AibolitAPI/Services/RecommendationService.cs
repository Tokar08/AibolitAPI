using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Interfaces.Services;
using AibolitAPI.Models;
using AutoMapper;
using Newtonsoft.Json;

namespace AibolitAPI.Services;

public class RecommendationService : IRecommendationService
{
    private readonly IDoctorService _doctorService;
    private readonly IFilterService _filterService;
    private readonly ILogger<RecommendationService> _logger;
    private readonly IMapper _mapper;
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly INotificationSender _notificationSender;
    private readonly IPatientService _patientService;
    private readonly IRecommendationRepository _recommendationRepository;
    private readonly IEmailTemplateFactory _templateFactory;

    public RecommendationService(IRecommendationRepository recommendationRepository, IMapper mapper,
        ILogger<RecommendationService> logger,
        IMedicalRecordRepository medicalRecordRepository, IEmailTemplateFactory templateFactory,
        IDoctorService doctorService, INotificationSender notificationSender, IPatientService patientService,
        IFilterService filterService)
    {
        _recommendationRepository = recommendationRepository;
        _mapper = mapper;
        _logger = logger;
        _medicalRecordRepository = medicalRecordRepository;
        _templateFactory = templateFactory;
        _doctorService = doctorService;
        _notificationSender = notificationSender;
        _patientService = patientService;
        _filterService = filterService;
    }

    public async Task<IEnumerable<RecommendationDTO>> GetAllAsync(int page, int size,
        RecommendationFilterDTO? filterDto)
    {
        try
        {
            var recommendations = await _recommendationRepository.GetAllAsync(page, size);
            var recommendationDTOs = _mapper.Map<IEnumerable<RecommendationDTO>>(recommendations);
            var filteredRecommendations = _filterService.ApplyRecommendationFilter(recommendationDTOs, filterDto);

            return filteredRecommendations.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all recommendations.");
            throw;
        }
    }


    public async Task<object> GetByIdAsync(Guid id)
    {
        try
        {
            var recommendation = await _recommendationRepository.GetByIdAsync(id);

            if (recommendation == null)
                throw new KeyNotFoundException($"Recommendation with ID: {id} not found.");

            var recommendationDTO = _mapper.Map<RecommendationDTO>(recommendation);

            var doctorJson = await _doctorService.GetByIdAsync(recommendation.DoctorId);

            var doctorDTO = JsonConvert.DeserializeObject<DoctorDTO>(doctorJson);
            if (doctorDTO == null)
                throw new Exception($"Doctor with ID: {recommendation.DoctorId} not found or invalid data.");

            var result = new
            {
                Recommendation = recommendationDTO,
                DoctorName = $"{doctorDTO.FirstName} {doctorDTO.LastName}"
            };

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting recommendation with ID: {id}");
            throw;
        }
    }


    public async Task UpdateAsync(RecommendationDTO recommendationDto)
    {
        try
        {
            var recommendation = _mapper.Map<Recommendation>(recommendationDto);
            await _recommendationRepository.UpdateAsync(recommendation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating recommendation.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _recommendationRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting recommendation with ID: {id}");
            throw;
        }
    }

    public async Task CreateAsync(Guid doctorId, Guid patientId, RecommendationDTO recommendationDto)
    {
        ArgumentNullException.ThrowIfNull(recommendationDto);

        SetDefaultValuesForRecommendation(recommendationDto);

        var patient = await GetPatientByIdAsync(patientId);
        await ValidatePatientAndDoctorAssignment(patient, doctorId);

        var medicalRecord = await GetMedicalRecordForPatient(patientId);
        SetRecommendationData(recommendationDto, doctorId, patientId, medicalRecord.Id);

        var recommendation = _mapper.Map<Recommendation>(recommendationDto);
        await _recommendationRepository.CreateAsync(recommendation);

        await SendRecommendationNotificationAsync(patient, doctorId);
    }

    private static void SetDefaultValuesForRecommendation(RecommendationDTO recommendationDto)
    {
        recommendationDto.RecommendationDate = recommendationDto.RecommendationDate == default
            ? DateTime.UtcNow
            : recommendationDto.RecommendationDate;

        recommendationDto.IsActive = recommendationDto.IsActive == false || recommendationDto.IsActive;
    }

    private async Task<PatientDTO> GetPatientByIdAsync(Guid patientId)
    {
        var patientJson = await _patientService.GetByIdAsync(patientId);
        var patient = JsonConvert.DeserializeObject<PatientDTO>(patientJson);

        if (patient == null)
            throw new KeyNotFoundException($"Patient with ID {patientId} not found.");

        return patient;
    }

    private static async Task ValidatePatientAndDoctorAssignment(PatientDTO patient, Guid doctorId)
    {
        if (!patient.Doctors.Any(d => d.Id == doctorId))
            throw new UnauthorizedAccessException("Patient is not assigned to this doctor.");
    }

    private async Task<MedicalRecord> GetMedicalRecordForPatient(Guid patientId)
    {
        var medicalRecord = await _medicalRecordRepository.GetByPatientIdAsync(patientId);

        if (medicalRecord == null)
            throw new KeyNotFoundException($"Medical record not found for patient with ID {patientId}.");

        return medicalRecord;
    }

    private static void SetRecommendationData(RecommendationDTO recommendationDto, Guid doctorId, Guid patientId,
        Guid medicalRecordId)
    {
        recommendationDto.MedicalRecordId = medicalRecordId;
        recommendationDto.DoctorId = doctorId;
        recommendationDto.PatientId = patientId;
    }

    private async Task SendRecommendationNotificationAsync(PatientDTO patient, Guid doctorId)
    {
        var doctorJson = await _doctorService.GetByIdAsync(doctorId);
        var doctor = JsonConvert.DeserializeObject<DoctorDTO>(doctorJson);

        if (!string.IsNullOrEmpty(patient.Email))
        {
            var template = _templateFactory.GetTemplate("new_recommendation");
            await _notificationSender.SendAsync(patient.Email, template, new object[]
            {
                $"{patient.FirstName} {patient.LastName}",
                $"{doctor.FirstName} {doctor.LastName}"
            });
        }
    }
}