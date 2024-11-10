using AutoMapper;
using AibolitAPI.DTOs;
using AibolitAPI.Models;

namespace AibolitAPI.Mappers;

   public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Mapping for User <-> UserDTO
            CreateMap<User, UserDTO>().ReverseMap()
                .MaxDepth(3);

            // Mapping for Hospital <-> HospitalDTO
            CreateMap<Hospital, HospitalDTO>().ReverseMap()
                .MaxDepth(3);

            // Mapping for Administrator <-> AdministratorDTO
            CreateMap<Administrator, AdministratorDTO>()
                .ForMember(dest => dest.ManagedHospital, opt => opt.MapFrom(src => src.ManagedHospital))
                .ForMember(dest => dest.Doctors, opt => opt.MapFrom(src => src.Doctors))
                .ForMember(dest => dest.Patients, opt => opt.MapFrom(src => src.Patients))
                .ReverseMap()
                .MaxDepth(3);

            // Mapping for Doctor <-> DoctorDTO
            CreateMap<Doctor, DoctorDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.WorkSchedule, opt => opt.MapFrom(src => src.WorkSchedule))
                .ForMember(dest => dest.Hospital, opt => opt.MapFrom(src => src.Hospital))
                .ForMember(dest => dest.Patients, opt => opt.MapFrom(src => src.Patients))
                .ForMember(dest => dest.LikedByPatients, opt => opt.MapFrom(src => src.LikedByPatients))
                .ReverseMap()
                .MaxDepth(3);

            // Mapping for Patient <-> PatientDTO
            CreateMap<Patient, PatientDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.MedicalRecord, opt => opt.MapFrom(src => src.MedicalRecord))
                .ForMember(dest => dest.Doctors, opt => opt.MapFrom(src => src.Doctors))
                .ForMember(dest => dest.LikedDoctors, opt => opt.MapFrom(src => src.LikedDoctors))
                .ReverseMap()
                .MaxDepth(3);

            // Mapping for MedicalRecord <-> MedicalRecordDTO
            CreateMap<MedicalRecord, MedicalRecordDTO>()
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Appointments, opt => opt.MapFrom(src => src.Appointments))
                .ForMember(dest => dest.Prescriptions, opt => opt.MapFrom(src => src.Prescriptions))
                .ForMember(dest => dest.Recommendations, opt => opt.MapFrom(src => src.Recommendations))
                .ReverseMap()
                .MaxDepth(3);

            // Mapping for Appointment <-> AppointmentDTO
            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient))
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor))
                .ForMember(dest => dest.MedicalRecord, opt => opt.MapFrom(src => src.MedicalRecord))
                .ReverseMap()
                .MaxDepth(3);

            // Mapping for Prescription <-> PrescriptionDTO
            CreateMap<Prescription, PrescriptionDTO>()
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient))
                .ForMember(dest => dest.PrescribedBy, opt => opt.MapFrom(src => src.PrescribedBy))
                .ForMember(dest => dest.MedicalRecord, opt => opt.MapFrom(src => src.MedicalRecord))
                .ReverseMap()
                .MaxDepth(3);

            // Mapping for Recommendation <-> RecommendationDTO
            CreateMap<Recommendation, RecommendationDTO>()
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient))
                .ForMember(dest => dest.GivenBy, opt => opt.MapFrom(src => src.GivenBy))
                .ForMember(dest => dest.MedicalRecord, opt => opt.MapFrom(src => src.MedicalRecord))
                .ReverseMap()
                .MaxDepth(3);

            // Mapping for WorkSchedule <-> WorkScheduleDTO
            CreateMap<WorkSchedule, WorkScheduleDTO>().ReverseMap()
                .MaxDepth(3);

            // Mapping for Notification <-> NotificationDTO
            CreateMap<Notification, NotificationDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ReverseMap()
                .MaxDepth(3);
        }
    }