using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HospitalController(IHospitalService hospitalService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HospitalDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var hospitals = await hospitalService.GetAllHospitalsWithDetailsAsync(page, size);
            return new ContentResult
            {
                Content = hospitals,
                ContentType = "application/json"
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<HospitalDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var hospital = await hospitalService.GetByIdAsync(id);
            return new ContentResult
            {
                Content = hospital,
                ContentType = "application/json"
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{hospitalId:guid}/doctors")]
    public async Task<ActionResult<string>> GetDoctors(Guid hospitalId, int page = 1, int size = 10)
    {
        try
        {
            var doctors = await hospitalService.GetDoctorsByHospitalIdAsync(hospitalId, page, size);
            return new ContentResult
            {
                Content = doctors,
                ContentType = "application/json"
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{hospitalId:guid}/doctors/{doctorId:guid}")]
    public async Task<ActionResult<string>> GetDoctorWithPatients(Guid hospitalId, Guid doctorId, int page = 1,
        int size = 10)
    {
        try
        {
            var doctorWithPatients = await hospitalService.GetDoctorWithPatientsAsync(hospitalId, doctorId, page, size);
            return new ContentResult
            {
                Content = doctorWithPatients,
                ContentType = "application/json"
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{hospitalId:guid}/doctors/{doctorId:guid}/patients")]
    public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatientsForDoctor(Guid hospitalId, Guid doctorId,
        int page = 1, int size = 10)
    {
        try
        {
            var patients = await hospitalService.GetPatientsForDoctorAsync(hospitalId, doctorId, page, size);
            return new ContentResult
            {
                Content = patients,
                ContentType = "application/json"
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{hospitalId:guid}/doctors/{doctorId:guid}/patients/{patientId:guid}")]
    public async Task<ActionResult<string>> GetPatientInfo(Guid hospitalId, Guid doctorId, Guid patientId)
    {
        try
        {
            var patientInfo = await hospitalService.GetPatientInfoAsync(hospitalId, doctorId, patientId);
            return new ContentResult
            {
                Content = patientInfo,
                ContentType = "application/json"
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpGet("{hospitalId:guid}/doctors/{doctorId:guid}/patients/{patientId:guid}/recommendations")]
    public async Task<ActionResult<IEnumerable<RecommendationDTO>>> GetPatientRecommendations(Guid hospitalId,
        Guid doctorId, Guid patientId)
    {
        try
        {
            var patientRecommendations =
                await hospitalService.GetPatientRecommendationsAsync(hospitalId, doctorId, patientId);
            return Ok(patientRecommendations);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{hospitalId:guid}/doctors/{doctorId:guid}/patients/{patientId:guid}/prescriptions")]
    public async Task<ActionResult<IEnumerable<PrescriptionDTO>>> GetPatientPrescriptions(Guid hospitalId,
        Guid doctorId, Guid patientId)
    {
        try
        {
            var patientPrescriptions =
                await hospitalService.GetPatientPrescriptionsAsync(hospitalId, doctorId, patientId);
            return Ok(patientPrescriptions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet(
        "{hospitalId:guid}/doctors/{doctorId:guid}/patients/{patientId:guid}/recommendations/{recommendationId:guid}")]
    public async Task<ActionResult<RecommendationDTO>> GetPatientRecommendationById(Guid hospitalId,
        Guid doctorId, Guid patientId, Guid recommendationId)
    {
        try
        {
            var recommendation =
                await hospitalService.GetPatientRecommendationByIdAsync(hospitalId, doctorId, patientId,
                    recommendationId);
            return new JsonResult(recommendation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{hospitalId:guid}/doctors/{doctorId:guid}/patients/{patientId:guid}/prescriptions/{prescriptionId:guid}")]
    public async Task<ActionResult<PrescriptionDTO>> GetPatientPrescriptionById(Guid hospitalId,
        Guid doctorId, Guid patientId, Guid prescriptionId)
    {
        try
        {
            var prescription =
                await hospitalService.GetPatientPrescriptionByIdAsync(hospitalId, doctorId, patientId, prescriptionId);
            return new JsonResult(prescription);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] HospitalDTO hospitalDto)
    {
        try
        {
            await hospitalService.CreateAsync(hospitalDto);
            return Created($"api/Hospital/{hospitalDto.Id}", hospitalDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] HospitalDTO hospitalDto)
    {
        try
        {
            if (id != hospitalDto.Id) return BadRequest(new { message = "ID mismatch" });

            await hospitalService.UpdateAsync(hospitalDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> SoftDeleteAsync(Guid id)
    {
        try
        {
            await hospitalService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}