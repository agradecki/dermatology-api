using DermatologyApi.DTOs;
using DermatologyApi.Mappers;
using DermatologyApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            try 
            {
                var patient = await _patientService.GetPatientByIdAsync(id);

                if (patient == null)
                    return NotFound();

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
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
            try
            {
                var etagBase64 = Request.Headers["If-Match"].ToString();
                if (string.IsNullOrEmpty(etagBase64))
                    return BadRequest("ETag header is required.");

                byte[] etag = Convert.FromBase64String(etagBase64);

                var patient = await _patientService.UpdatePatientAsync(id, patientDto, etag);
                var updatedPatient = await _patientService.GetPatientEntityByIdAsync(id);
                var newEtag = Convert.ToBase64String(updatedPatient.RowVersion);

                Response.Headers.Add("ETag", newEtag);

                return Ok(patient);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch(DbUpdateConcurrencyException)
            {
                return StatusCode(412, "Precondition failed: The resource has been modified by another user");
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<PatientDto>> PatchPatient(int id, PatientPatchDto patientDto)
        {
            try
            {
                var patient = await _patientService.PatchPatientAsync(id, patientDto);
                return Ok(patient);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            try
            {
                await _patientService.DeletePatientAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{pid}/diagnoses")]
        public async Task<ActionResult<IEnumerable<DiagnosisDto>>> GetPatientDiagnoses(int pid)
        {
            try
            {
                var diagnoses = await _patientService.GetPatientDiagnosesAsync(pid);
                if (diagnoses == null || !diagnoses.Any())
                    return NotFound($"No diagnoses found for patient with ID {pid}.");

                return Ok(diagnoses);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Patient with ID {pid} not found.");
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"An error occurred {ex.Message}");
            }
        }

        [HttpGet("{pid}/consultations")]
        public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetPatientConsultations(int pid)
        {
            try
            {
                var consultations = await _patientService.GetPatientConsultationsAsync(pid);
                if (consultations == null || !consultations.Any())
                    return NotFound($"No consultation found for patient with ID {pid}.");

                return Ok(consultations);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Patient with ID {pid} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred {ex.Message}");
            }
        }
    }
}
