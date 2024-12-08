using AibolitAPI.DTOs;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Mappers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // Mapping for User <-> UserDTO
        CreateMap<User, UserDTO>().ReverseMap()
            .MaxDepth(3);

        // Mapping for Hospital <-> HospitalDTO
        CreateMap<Hospital, HospitalDTO>()
            .ForMember(dest => dest.Administrators, opt => opt.MapFrom(src => src.Administrators))
            .ReverseMap()
            .MaxDepth(3);

        // Mapping for Administrator <-> AdministratorDTO
        CreateMap<Administrator, AdministratorDTO>()
            .ForMember(dest => dest.Doctors, opt => opt.MapFrom(src => src.Doctors))
            .ForMember(dest => dest.Patients, opt => opt.MapFrom(src => src.Patients))
            .ReverseMap()
            .MaxDepth(3);

        // Mapping for Doctor <-> DoctorDTO
        CreateMap<Doctor, DoctorDTO>()
            .ForMember(dest => dest.Patients, opt => opt.MapFrom(src => src.Patients))
            .ForMember(dest => dest.LikedByPatients, opt => opt.MapFrom(src => src.LikedByPatients))
            .ReverseMap()
            .MaxDepth(3);

        // Mapping for Patient <-> PatientDTO
        CreateMap<Patient, PatientDTO>()
            .ForMember(dest => dest.LikedDoctors, opt => opt.MapFrom(src => src.LikedDoctors))
            .ForMember(dest => dest.Doctors, opt => opt.MapFrom(src => src.Doctors))
            .ReverseMap()
            .MaxDepth(3);

        // Mapping for MedicalRecord <-> MedicalRecordDTO
        CreateMap<MedicalRecord, MedicalRecordDTO>()
            .PreserveReferences()
            .ForMember(dest => dest.Appointments, opt => opt.MapFrom(src => src.Appointments))
            .ForMember(dest => dest.Prescriptions, opt => opt.MapFrom(src => src.Prescriptions))
            .ForMember(dest => dest.Recommendations, opt => opt.MapFrom(src => src.Recommendations))
            .ReverseMap();

        // Mapping for Appointment <-> AppointmentDTO
        CreateMap<Appointment, AppointmentDTO>()
            .ReverseMap()
            .MaxDepth(3);

        // Mapping for Prescription <-> PrescriptionDTO
        CreateMap<Prescription, PrescriptionDTO>()
            .ReverseMap()
            .MaxDepth(3);

        // Mapping for Recommendation <-> RecommendationDTO
        CreateMap<Recommendation, RecommendationDTO>()
            .ReverseMap()
            .MaxDepth(3);

        // Mapping for WorkSchedule <-> WorkScheduleDTO
        CreateMap<WorkSchedule, WorkScheduleDTO>().ReverseMap()
            .MaxDepth(3);

        // Mapping for Notification <-> NotificationDTO
        CreateMap<Notification, NotificationDTO>()
            .ForMember(dest => dest.NotificationDate, opt => opt.MapFrom(src => src.NotificationDate))
            .ReverseMap()
            .MaxDepth(3);
    }
}