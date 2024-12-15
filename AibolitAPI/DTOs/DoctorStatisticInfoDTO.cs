namespace AibolitAPI.DTOs;

public class DoctorStatisticInfoDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int VisitCount { get; set; }
    public string PhotoUrl { get; set; }
    public int LikedByPatientsCount { get; set; }
}