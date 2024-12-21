using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using Newtonsoft.Json;

namespace AibolitAPI.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IAppointmentService appointmentService;
    private readonly IHospitalService hospitalService;
    private readonly IPatientService patientService;

    public StatisticsService(IPatientService patientService, IAppointmentService appointmentService,
        IHospitalService hospitalService)
    {
        this.patientService = patientService;
        this.appointmentService = appointmentService;
        this.hospitalService = hospitalService;
    }

    public async Task<StatisticsDTO> GetStatisticsAsync(Guid hospitalId)
    {
        var patientsJson = await patientService.GetAllPatientsWithSSOAsync(1, int.MaxValue);
        var appointments = await appointmentService.GetAllAsync(1, int.MaxValue);
        var hospitalsJson = await hospitalService.GetAllHospitalsWithDetailsAsync(1, int.MaxValue);

        var hospital = JsonConvert.DeserializeObject<List<HospitalDTO>>(hospitalsJson)
            .FirstOrDefault(h => h.Id == hospitalId);

        if (hospital == null) throw new Exception("Hospital not found!");

        var doctors = hospital.Doctors.Where(d => d.IsActive);
        var patients = JsonConvert.DeserializeObject<List<PatientDTO>>(patientsJson)
            .Where(p => p.Doctors.Any(d => d.HospitalId == hospitalId))
            .ToList();

        var genderDistributionDoctors = GetGenderDistribution(doctors);
        var genderDistributionPatients = GetGenderDistribution(patients);
        var genderPercentageDoctors = GetGenderPercentage(doctors);
        var genderPercentagePatients = GetGenderPercentage(patients);
        var doctorVisitRatings = GetDoctorVisitRatings(doctors);
        var doctorLikeRatings = GetDoctorLikeRatings(doctors);
        var totalStaff = GetTotalStaff(hospital);
        var totalDoctors = doctors.Count();
        var totalPatients = patients.Count;
        var totalAdministrators = hospital.Administrators?.Count ?? 0;

        var patientAgeGroups = GetPatientAgeGroups(patients);
        var doctorSpecializationDistribution = GetDoctorSpecializationDistribution(doctors);
        var patientSpecializationPercentage =
            GetPatientSpecializationPercentage(doctorSpecializationDistribution, doctors);
        var cancellationRate = GetCancellationRate(appointments, doctors);

        return new StatisticsDTO
        {
            GenderDistributionDoctors = genderDistributionDoctors,
            GenderDistributionPatients = genderDistributionPatients,
            GenderPercentageDoctors = genderPercentageDoctors,
            GenderPercentagePatients = genderPercentagePatients,
            DoctorVisitRatings = doctorVisitRatings,
            DoctorLikeRatings = doctorLikeRatings,
            TotalStaff = totalStaff,
            TotalDoctors = totalDoctors,
            TotalAdministrators = totalAdministrators,
            TotalPatients = totalPatients,
            PatientAgeGroups = patientAgeGroups,
            DoctorSpecializationDistribution = doctorSpecializationDistribution,
            PatientSpecializationPercentage = patientSpecializationPercentage,
            CancellationRate = cancellationRate
        };
    }

    //NOTE: Распределение по гендеру среди людей (врачей/пациентов)
    private static Dictionary<string, int> GetGenderDistribution(IEnumerable<dynamic> people)
    {
        return people
            .GroupBy(p => (string)p.Gender)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    //NOTE: Распределение по гендеру среди людей в процентном соотношении (врачей/пациентов)
    private static Dictionary<string, double> GetGenderPercentage(IEnumerable<dynamic> people)
    {
        var genderCounts = people.GroupBy(p => (string)p.Gender)
            .ToDictionary(g => g.Key, g => g.Count());

        var totalPeople = people.Count();
        if (totalPeople == 0) return new Dictionary<string, double>();
        return genderCounts.ToDictionary(g => g.Key, g => Math.Round((double)g.Value / totalPeople * 100, 2));
    }


    //NOTE: Рейтинг врачей по количеству посещений
    private static List<DoctorStatisticInfoDTO> GetDoctorVisitRatings(IEnumerable<DoctorDTO> doctors)
    {
        return doctors
            .OrderByDescending(d => d.VisitCount)
            .Select(d => new DoctorStatisticInfoDTO
            {
                FirstName = d.FirstName,
                LastName = d.LastName,
                VisitCount = d.VisitCount,
                PhotoUrl = d.PhotoUrl,
                LikedByPatientsCount = d.LikedByPatients?.Count ?? 0
            })
            .ToList();
    }

    //NOTE: Рейтинг врачей по количеству лайков от пациентов
    private static List<DoctorStatisticInfoDTO> GetDoctorLikeRatings(IEnumerable<DoctorDTO> doctors)
    {
        return doctors
            .OrderByDescending(d => d.LikedByPatients?.Count ?? 0)
            .Select(d => new DoctorStatisticInfoDTO
            {
                FirstName = d.FirstName,
                LastName = d.LastName,
                VisitCount = d.VisitCount,
                PhotoUrl = d.PhotoUrl,
                LikedByPatientsCount = d.LikedByPatients?.Count ?? 0
            })
            .ToList();
    }

    //NOTE: Общее количество сотрудников (врачей(+глав.врач) и администраторов)
    private static int GetTotalStaff(HospitalDTO hospital)
    {
        var totalAdministrators = hospital.Administrators?.Count ?? 0;
        var totalDoctors = hospital.Doctors.Count(d => d.IsActive);
        return totalDoctors + totalAdministrators;
    }

    //NOTE: Распределение пациентов по возрастным группам
    private static Dictionary<string, int> GetPatientAgeGroups(IEnumerable<PatientDTO> patients)
    {
        return patients
            .GroupBy(p => GetAgeGroup(p.BirthDate))
            .ToDictionary(g => g.Key, g => g.Count());
    }

    //NOTE: Распределение врачей по специализациям
    private static Dictionary<string, int> GetDoctorSpecializationDistribution(IEnumerable<DoctorDTO> doctors)
    {
        return doctors
            .GroupBy(d => d.Specialization)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    //NOTE: Процентное распределение пациентов по специализациям врачей
    private static Dictionary<string, double> GetPatientSpecializationPercentage(
        Dictionary<string, int> specializationDistribution, IEnumerable<DoctorDTO> doctors)
    {
        return specializationDistribution
            .ToDictionary(
                g => g.Key,
                g => Math.Round((double)g.Value / doctors.Count() * 100, 2));
    }

    //NOTE: Процент отменённых приёмов
    private static double GetCancellationRate(IEnumerable<AppointmentDTO> appointments, IEnumerable<DoctorDTO> doctors)
    {
        var cancelledAppointments = appointments.Count(a => !a.IsScheduled && doctors.Any(d => d.Id == a.DoctorId));
        var totalAppointments = appointments.Count();

        if (totalAppointments == 0) return 0;

        return Math.Round((double)cancelledAppointments / totalAppointments * 100, 2);
    }


    //NOTE: Определение возрастной группы по дате рождения
    private static string GetAgeGroup(string birthDate)
    {
        if (!DateTime.TryParse(birthDate, out var date))
            return "Unknown";
        var age = DateTime.Now.Year - date.Year;
        return age switch
        {
            < 18 => "0-17",
            < 30 => "18-29",
            < 45 => "30-44",
            < 60 => "45-59",
            _ => "60+"
        };
    }
}