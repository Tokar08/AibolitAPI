using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Interfaces.Services;
using AutoMapper;

namespace AibolitAPI.Services;

public class SpecializationService : ISpecializationService
{
    private readonly IMapper mapper;
    private readonly ISpecializationRepository specializationRepository;

    public SpecializationService(ISpecializationRepository specializationRepository, IMapper mapper)
    {
        this.specializationRepository = specializationRepository;
        this.mapper = mapper;
    }

    public async Task<SpecializationDTO?> GetSpecializationByTitleAsync(string title)
    {
        var specialization = await specializationRepository.GetByTitleAsync(title);
        return mapper.Map<SpecializationDTO?>(specialization);
    }

    public async Task<IEnumerable<SpecializationDTO>> GetAllSpecializationsAsync()
    {
        var specializations = await specializationRepository.GetAllAsync(1, int.MaxValue);
        return mapper.Map<IEnumerable<SpecializationDTO>>(specializations);
    }

    public async Task<bool> IsFamilyDoctorAsync(Guid specializationId)
    {
        var specialization = await specializationRepository.GetByIdAsync(specializationId);
        return specialization != null &&
               specialization.Title.Equals("Сімейний лікар", StringComparison.OrdinalIgnoreCase);
    }
}