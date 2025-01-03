using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Interfaces.Services;
using AibolitAPI.Models;
using AutoMapper;
using Newtonsoft.Json;

namespace AibolitAPI.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IDoctorService _doctorService;
    private readonly IFilterService _filterService;
    private readonly ILogger<PrescriptionService> _logger;
    private readonly IMapper _mapper;
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly INotificationSender _notificationSender;
    private readonly IPatientService _patientService;
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IEmailTemplateFactory _templateFactory;

    public PrescriptionService(IPrescriptionRepository prescriptionRepository, IMapper mapper,
        ILogger<PrescriptionService> logger, IPatientService patientService,
        IMedicalRecordRepository medicalRecordRepository, IDoctorService doctorService,
        INotificationSender notificationSender, IEmailTemplateFactory templateFactory, IFilterService filterService)
    {
        _prescriptionRepository = prescriptionRepository;
        _mapper = mapper;
        _logger = logger;
        _patientService = patientService;
        _medicalRecordRepository = medicalRecordRepository;
        _doctorService = doctorService;
        _notificationSender = notificationSender;
        _templateFactory = templateFactory;
        _filterService = filterService;
    }

    public async Task<IEnumerable<PrescriptionDTO>> GetAllAsync(int page, int size, PrescriptionFilterDTO? filterDto)
    {
        try
        {
            var prescriptions = await _prescriptionRepository.GetAllAsync(page, size);
            var prescriptionDTOs = _mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);
            var filteredPrescriptions = _filterService.ApplyPrescriptionFilter(prescriptionDTOs, filterDto);

            return filteredPrescriptions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all prescriptions.");
            throw;
        }
    }


    public async Task<object> GetByIdAsync(Guid id)
    {
        try
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);

            if (prescription == null)
                throw new KeyNotFoundException($"Prescription with ID: {id} not found.");

            var prescriptionDTO = _mapper.Map<PrescriptionDTO>(prescription);

            var doctorJson = await _doctorService.GetByIdAsync(prescription.DoctorId);
            var doctorDTO = JsonConvert.DeserializeObject<DoctorDTO>(doctorJson);

            if (doctorDTO == null)
                throw new Exception($"Doctor with ID: {prescription.DoctorId} not found or invalid data.");

            var result = new
            {
                Prescription = prescriptionDTO,
                DoctorName = $"{doctorDTO.FirstName} {doctorDTO.LastName}"
            };

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting prescription with ID: {id}");
            throw;
        }
    }


    public async Task CreateAsync(Guid doctorId, Guid patientId, PrescriptionDTO prescriptionDto)
    {
        ArgumentNullException.ThrowIfNull(prescriptionDto);

        SetDefaultValuesForPrescription(prescriptionDto);

        var patient = await GetPatientByIdAsync(patientId);
        var doctor = await GetDoctorByIdAsync(doctorId);

        await ValidatePatientAndDoctorAssignment(patient, doctorId);

        var medicalRecord = await GetMedicalRecordForPatient(patientId);
        SetPrescriptionData(prescriptionDto, doctorId, patientId, medicalRecord.Id);

        await SendPrescriptionNotification(patient, doctor, prescriptionDto);

        var prescription = _mapper.Map<Prescription>(prescriptionDto);
        await _prescriptionRepository.CreateAsync(prescription);
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

    private void SetDefaultValuesForPrescription(PrescriptionDTO prescriptionDto)
    {
        prescriptionDto.PrescriptionDate = prescriptionDto.PrescriptionDate == default
            ? DateTime.UtcNow
            : prescriptionDto.PrescriptionDate;

        prescriptionDto.IsActive = prescriptionDto.IsActive == false || prescriptionDto.IsActive;
    }

    private async Task<PatientDTO> GetPatientByIdAsync(Guid patientId)
    {
        var patientJson = await _patientService.GetByIdAsync(patientId);
        var patient = JsonConvert.DeserializeObject<PatientDTO>(patientJson);

        if (patient == null)
            throw new KeyNotFoundException($"Patient with ID {patientId} not found.");

        return patient;
    }

    private async Task<DoctorDTO> GetDoctorByIdAsync(Guid doctorId)
    {
        var doctorJson = await _doctorService.GetByIdAsync(doctorId);
        return JsonConvert.DeserializeObject<DoctorDTO>(doctorJson);
    }

    private async Task ValidatePatientAndDoctorAssignment(PatientDTO patient, Guid doctorId)
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

    private void SetPrescriptionData(PrescriptionDTO prescriptionDto, Guid doctorId, Guid patientId,
        Guid medicalRecordId)
    {
        prescriptionDto.MedicalRecordId = medicalRecordId;
        prescriptionDto.DoctorId = doctorId;
        prescriptionDto.PatientId = patientId;
        prescriptionDto.Id = Guid.NewGuid();
    }

    private async Task SendPrescriptionNotification(PatientDTO patient, DoctorDTO doctor,
        PrescriptionDTO prescriptionDto)
    {
        if (!string.IsNullOrEmpty(patient.Email))
        {
            var template = _templateFactory.GetTemplate("new_prescription");
            await _notificationSender.SendAsync(patient.Email, template, new object[]
            {
                $"{patient.FirstName} {patient.LastName}",
                $"{doctor.FirstName} {doctor.LastName}",
                prescriptionDto.MedicationName
            });
        }
    }
}