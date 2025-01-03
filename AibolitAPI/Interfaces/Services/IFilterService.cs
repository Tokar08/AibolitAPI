using AibolitAPI.DTOs;
using AibolitAPI.Models;

namespace AibolitAPI.Interfaces.Services;

public interface IFilterService
{
    IEnumerable<DoctorDTO> ApplyDoctorFilter(IEnumerable<DoctorDTO> doctors, DoctorFilterDTO filterDto,
        IEnumerable<Doctor> dbDoctors);

    IEnumerable<PatientDTO> ApplyPatientFilter(IEnumerable<PatientDTO> patients, PatientFilterDTO filterDto);

    IEnumerable<AppointmentDTO> ApplyAppointmentFilter(
        IEnumerable<AppointmentDTO> appointments,
        AppointmentFilterDTO? filterDto);

    IEnumerable<RecommendationDTO> ApplyRecommendationFilter(
        IEnumerable<RecommendationDTO> recommendations,
        RecommendationFilterDTO? filterDto);

    IEnumerable<PrescriptionDTO> ApplyPrescriptionFilter(IEnumerable<PrescriptionDTO> prescriptions,
        PrescriptionFilterDTO? filterDto);
}