namespace AibolitAPI.DTOs;

public class StatisticsDTO
{
    public Dictionary<string, int> GenderDistributionDoctors { get; set; }
    public Dictionary<string, int> GenderDistributionPatients { get; set; }
    public Dictionary<string, double> GenderPercentageDoctors { get; set; }
    public Dictionary<string, double> GenderPercentagePatients { get; set; }

    public List<DoctorStatisticInfoDTO> DoctorVisitRatings { get; set; }
    public List<DoctorStatisticInfoDTO> DoctorLikeRatings { get; set; }

    public int TotalStaff { get; set; }
    public int TotalDoctors { get; set; }
    public int TotalAdministrators { get; set; }
    public int TotalPatients { get; set; }

    public Dictionary<string, int> PatientAgeGroups { get; set; }
    public Dictionary<string, int> DoctorSpecializationDistribution { get; set; }
    public Dictionary<string, double> PatientSpecializationPercentage { get; set; }

    public double CancellationRate { get; set; }
}