using AibolitAPI.DTOs;
using AibolitAPI.Interfaces.Services;
using AibolitAPI.Models;

namespace AibolitAPI.Services;

public class FilterService : IFilterService
{
    public IEnumerable<DoctorDTO> ApplyDoctorFilter(IEnumerable<DoctorDTO> doctors, DoctorFilterDTO filterDto,
        IEnumerable<Doctor> dbDoctors)
    {
        if (filterDto == null) return doctors;

        return doctors
            .Where(d => string.IsNullOrWhiteSpace(filterDto.SearchTerm) ||
                        $"{d.FirstName} {d.LastName}".Contains(filterDto.SearchTerm,
                            StringComparison.OrdinalIgnoreCase))
            .Where(d => string.IsNullOrWhiteSpace(filterDto.SpecializationTitle) ||
                        d.SpecializationTitle?.Contains(filterDto.SpecializationTitle,
                            StringComparison.OrdinalIgnoreCase) == true)
            .Where(d => string.IsNullOrWhiteSpace(filterDto.HospitalTitle) ||
                        (d.HospitalId != Guid.Empty &&
                         dbDoctors.Any(doc =>
                             doc.Hospital.Title.Contains(filterDto.HospitalTitle, StringComparison.OrdinalIgnoreCase))))
            .Where(d => string.IsNullOrWhiteSpace(filterDto.Gender) || d.Gender == filterDto.Gender)
            .Where(d => !filterDto.MinYearsOfExperience.HasValue ||
                        d.YearsOfExperience >= filterDto.MinYearsOfExperience.Value)
            .Where(d => !filterDto.MaxYearsOfExperience.HasValue ||
                        d.YearsOfExperience <= filterDto.MaxYearsOfExperience.Value);
    }

    public IEnumerable<PatientDTO> ApplyPatientFilter(IEnumerable<PatientDTO> patients, PatientFilterDTO filterDto)
    {
        if (filterDto == null) return patients;

        return patients.Where(p =>
        {
            var isValidBirthDate = DateTime.TryParse(p.BirthDate, out var birthDate);
            return
                (string.IsNullOrWhiteSpace(filterDto.SearchTerm) ||
                 $"{p.FirstName} {p.LastName}".Contains(filterDto.SearchTerm, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(filterDto.Gender) || p.Gender == filterDto.Gender) &&
                (string.IsNullOrWhiteSpace(filterDto.City) ||
                 p.City?.Contains(filterDto.City, StringComparison.OrdinalIgnoreCase) == true) &&
                (string.IsNullOrWhiteSpace(filterDto.PhoneNumber) ||
                 p.PhoneNumber?.Contains(filterDto.PhoneNumber, StringComparison.OrdinalIgnoreCase) == true) &&
                (!filterDto.MinBirthDate.HasValue || (isValidBirthDate && birthDate >= filterDto.MinBirthDate.Value)) &&
                (!filterDto.MaxBirthDate.HasValue || (isValidBirthDate && birthDate <= filterDto.MaxBirthDate.Value));
        });
    }

    public IEnumerable<AppointmentDTO> ApplyAppointmentFilter(
        IEnumerable<AppointmentDTO> appointments,
        AppointmentFilterDTO? filterDto)
    {
        if (filterDto == null) return appointments;

        var query = appointments.AsQueryable();

        if (filterDto.MinDate.HasValue)
            query = query.Where(a => a.AppointmentDate >= filterDto.MinDate.Value);

        if (filterDto.MaxDate.HasValue)
            query = query.Where(a => a.AppointmentDate <= filterDto.MaxDate.Value);

        if (filterDto.IsScheduled.HasValue)
            query = query.Where(a => a.IsScheduled == filterDto.IsScheduled.Value);

        if (filterDto.SortByDescending.HasValue && filterDto.SortByDescending.Value)
            query = query.OrderByDescending(a => a.AppointmentDate);
        else
            query = query.OrderBy(a => a.AppointmentDate);

        return query.ToList();
    }


    public IEnumerable<RecommendationDTO> ApplyRecommendationFilter(
        IEnumerable<RecommendationDTO> recommendations,
        RecommendationFilterDTO? filterDto)
    {
        if (filterDto == null)
            return recommendations;

        var query = recommendations.AsQueryable();

        if (filterDto.MinDate.HasValue)
            query = query.Where(r => r.RecommendationDate >= filterDto.MinDate.Value);

        if (filterDto.MaxDate.HasValue)
            query = query.Where(r => r.RecommendationDate <= filterDto.MaxDate.Value);

        if (!string.IsNullOrWhiteSpace(filterDto.ContentSearch))
            query = query.Where(r => r.Content.Contains(filterDto.ContentSearch, StringComparison.OrdinalIgnoreCase));

        query = filterDto.SortByDescending.GetValueOrDefault(false)
            ? query.OrderByDescending(r => r.RecommendationDate)
            : query.OrderBy(r => r.RecommendationDate);

        return query.ToList();
    }

    public IEnumerable<PrescriptionDTO> ApplyPrescriptionFilter(IEnumerable<PrescriptionDTO> prescriptions,
        PrescriptionFilterDTO? filterDto)
    {
        if (filterDto == null) return prescriptions;

        var query = prescriptions.AsQueryable();

        if (filterDto.MinDate.HasValue)
            query = query.Where(p => p.PrescriptionDate >= filterDto.MinDate.Value);

        if (filterDto.MaxDate.HasValue)
            query = query.Where(p => p.PrescriptionDate <= filterDto.MaxDate.Value);

        if (!string.IsNullOrWhiteSpace(filterDto.MedicationName))
            query = query.Where(p =>
                p.MedicationName.Contains(filterDto.MedicationName, StringComparison.OrdinalIgnoreCase));

        query = filterDto.SortByDescending == true
            ? query.OrderByDescending(p => p.PrescriptionDate)
            : query.OrderBy(p => p.PrescriptionDate);

        return query.ToList();
    }
}