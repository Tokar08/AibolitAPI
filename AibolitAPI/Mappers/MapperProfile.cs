using AutoMapper;
using AibolitAPI.DTOs;
using AibolitAPI.Models;

namespace AibolitAPI.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Doctor, DoctorDTO>()
                .ForMember(dto => dto.FullName, opt => opt.MapFrom(model => $"{model.FirstName} {model.LastName}"))
                .ForMember(dto => dto.Specialization, opt => opt.MapFrom(model => model.Specialization))
                .ForMember(dto => dto.HospitalTitle, opt => opt.MapFrom(model => model.Hospital.Title))
                .ForMember(dto => dto.VisitCount, opt => opt.MapFrom(model => model.VisitCount))
                .ForMember(dto => dto.IsActive, opt => opt.MapFrom(model => model.IsActive));

            CreateMap<Patient, PatientDTO>()
                .ForMember(dto => dto.FullName, opt => opt.MapFrom(model => $"{model.FirstName} {model.LastName}"))
                .ForMember(dto => dto.BirthDate, opt => opt.MapFrom(model => model.BirthDate))
                .ForMember(dto => dto.Gender, opt => opt.MapFrom(model => model.Gender))
                .ForMember(dto => dto.PhoneNumber, opt => opt.MapFrom(model => model.PhoneNumber))
                .ForMember(dto => dto.Email, opt => opt.MapFrom(model => model.Email))
                .ForMember(dto => dto.IsActive, opt => opt.MapFrom(model => model.IsActive));

            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dto => dto.PatientFullName, opt => opt.MapFrom(model => $"{model.Patient.FirstName} {model.Patient.LastName}"))
                .ForMember(dto => dto.DoctorFullName, opt => opt.MapFrom(model => $"{model.Doctor.FirstName} {model.Doctor.LastName}"))
                .ForMember(dto => dto.AppointmentDate, opt => opt.MapFrom(model => model.AppointmentDate))
                .ForMember(dto => dto.IsScheduled, opt => opt.MapFrom(model => model.IsScheduled))
                .ForMember(dto => dto.IsActive, opt => opt.MapFrom(model => model.IsActive));

            CreateMap<Hospital, HospitalDTO>()
                .ForMember(dto => dto.ChiefDoctorFullName, opt => opt.MapFrom(model => $"{model.ChiefDoctor.FirstName} {model.ChiefDoctor.LastName}"))
                .ForMember(dto => dto.Title, opt => opt.MapFrom(model => model.Title))
                .ForMember(dto => dto.Address, opt => opt.MapFrom(model => model.Address))
                .ForMember(dto => dto.IsActive, opt => opt.MapFrom(model => model.IsActive));

            CreateMap<Prescription, PrescriptionDTO>()
                .ForMember(dto => dto.PatientFullName, opt => opt.MapFrom(model => $"{model.Patient.FirstName} {model.Patient.LastName}"))
                .ForMember(dto => dto.DoctorFullName, opt => opt.MapFrom(model => $"{model.PrescribedBy.FirstName} {model.PrescribedBy.LastName}"))
                .ForMember(dto => dto.PrescriptionDate, opt => opt.MapFrom(model => model.PrescriptionDate))
                .ForMember(dto => dto.MedicationName, opt => opt.MapFrom(model => model.MedicationName))
                .ForMember(dto => dto.Dosage, opt => opt.MapFrom(model => model.Dosage))
                .ForMember(dto => dto.Instructions, opt => opt.MapFrom(model => model.Instructions))
                .ForMember(dto => dto.IsActive, opt => opt.MapFrom(model => model.IsActive));

            CreateMap<Recommendation, RecommendationDTO>()
                .ForMember(dto => dto.PatientFullName, opt => opt.MapFrom(model => $"{model.Patient.FirstName} {model.Patient.LastName}"))
                .ForMember(dto => dto.DoctorFullName, opt => opt.MapFrom(model => $"{model.GivenBy.FirstName} {model.GivenBy.LastName}"))
                .ForMember(dto => dto.RecommendationDate, opt => opt.MapFrom(model => model.RecommendationDate))
                .ForMember(dto => dto.Content, opt => opt.MapFrom(model => model.Content))
                .ForMember(dto => dto.IsActive, opt => opt.MapFrom(model => model.IsActive));
            
            CreateMap<User, UserDTO>()
                .ForMember(dto => dto.FullName, opt => opt.MapFrom(model => $"{model.FirstName} {model.LastName}"))
                .ForMember(dto => dto.Username, opt => opt.MapFrom(model => model.Username))
                .ForMember(dto => dto.BirthDate, opt => opt.MapFrom(model => model.BirthDate))
                .ForMember(dto => dto.Gender, opt => opt.MapFrom(model => model.Gender))
                .ForMember(dto => dto.PhoneNumber, opt => opt.MapFrom(model => model.PhoneNumber))
                .ForMember(dto => dto.Email, opt => opt.MapFrom(model => model.Email))
                .ForMember(dto => dto.IsActive, opt => opt.MapFrom(model => model.IsActive));

            CreateMap<RegisterDTO, Patient>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username));
            
            CreateMap<LoginDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}
