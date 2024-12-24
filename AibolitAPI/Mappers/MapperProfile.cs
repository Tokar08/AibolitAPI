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
            .ReverseMap()
            .MaxDepth(2);

        // Mapping for Doctor <-> DoctorDTO
        CreateMap<Doctor, DoctorDTO>()
            .ForMember(dest => dest.Patients, opt => opt.MapFrom(src => src.Patients))
            .ForMember(dest => dest.LikedByPatients, opt => opt.MapFrom(src => src.LikedByPatients))
            .ForMember(dest => dest.SpecializationTitle, opt => opt.MapFrom(src => src.Specialization.Title))
            .ForMember(dest => dest.WorkSchedules, opt => opt.MapFrom(src => src.WorkSchedules))
            .ReverseMap()
            .MaxDepth(2);

        // Mapping for Patient <-> PatientDTO
        CreateMap<Patient, PatientDTO>()
            .ForMember(dest => dest.LikedDoctors, opt => opt.MapFrom(src => src.LikedDoctors))
            .ForMember(dest => dest.Doctors, opt => opt.MapFrom(src => src.Doctors))
            .ReverseMap()
            .MaxDepth(2);

        // Mapping for MedicalRecord <-> MedicalRecordDTO
        CreateMap<MedicalRecord, MedicalRecordDTO>()
            .PreserveReferences()
            .ForMember(dest => dest.Appointments, opt => opt.MapFrom(src => src.Appointments))
            .ForMember(dest => dest.Prescriptions, opt => opt.MapFrom(src => src.Prescriptions))
            .ForMember(dest => dest.Recommendations, opt => opt.MapFrom(src => src.Recommendations))
            .ReverseMap()
            .MaxDepth(2);

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

        // Mapping for ScheduleAdjustment <-> ScheduleAdjustmentDTO
        CreateMap<ScheduleAdjustment, ScheduleAdjustmentDTO>()
            .ReverseMap()
            .MaxDepth(3);

        // Mapping for Specialization <-> SpecializationDTO
        CreateMap<Specialization, SpecializationDTO>()
            .ReverseMap()
            .MaxDepth(3);
    }
}