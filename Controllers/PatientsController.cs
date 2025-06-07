using DermatologyApi.DTOs;
using DermatologyApi.Mappers;
using DermatologyApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using DermatologyApi.Exceptions;

namespace DermatologyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientById(int id)
        {
            var patient = await _patientService.GetPatientEntityByIdAsync(id);

            var etag = patient.Xmin.ToString();
            var ifNoneMatch = Request.Headers["If-None-Match"].ToString();
            if (etag == ifNoneMatch)
                return StatusCode(304);

            Response.Headers["ETag"] = etag;
            return Ok(PatientMapper.MapToDto(patient));
        }

        [HttpPost]
        public async Task<ActionResult<PatientDto>> CreatePatient(PatientCreateDto patientDto)
        {
            var patient = await _patientService.CreatePatientAsync(patientDto);
            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PatientDto>> UpdatePatient(int id, PatientUpdateDto patientDto)
        {
            var etagBase64 = Request.Headers["If-Match"].ToString();
            if (string.IsNullOrEmpty(etagBase64))
            {
                throw new ValidationException("ETag header is required.");
            }

            var patient = await _patientService.GetPatientEntityByIdAsync(id);
            var etag = patient.Xmin.ToString();

            if (etagBase64 != etag)
            {
                throw new PreconditionFailedException("Precondition failed: The resource has been modified by anther user.");
            }

            if (!uint.TryParse(etag, out uint expectedXmin))
            {
                return BadRequest("Invalid ETag format");
            }

            var updatedPatient = await _patientService.UpdatePatientAsync(id, patientDto, expectedXmin);

            patient = await _patientService.GetPatientEntityByIdAsync(id);
            etag = patient.Xmin.ToString();

            Response.Headers["ETag"] = etag;

            return Ok(updatedPatient);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<PatientDto>> PatchPatient(int id, PatientPatchDto patientDto)
        {
            var patient = await _patientService.PatchPatientAsync(id, patientDto);
            return Ok(patient);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            await _patientService.DeletePatientAsync(id);
            return NoContent();
        }

        [HttpGet("{pid}/diagnoses")]
        public async Task<ActionResult<IEnumerable<DiagnosisDto>>> GetPatientDiagnoses(int pid)
        {
            var diagnoses = await _patientService.GetPatientDiagnosesAsync(pid);
            return Ok(diagnoses);
        }

        [HttpGet("{pid}/consultations")]
        public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetPatientConsultations(int pid)
        {
            var consultations = await _patientService.GetPatientConsultationsAsync(pid);
            return Ok(consultations);
        }
    }
}
