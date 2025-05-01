using DermatologyApi.DTOs;
using DermatologyApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosesController : ControllerBase
    {
        private readonly IDiagnosisService _diagnosisService;

        public DiagnosesController(IDiagnosisService diagnosisService)
        {
            _diagnosisService = diagnosisService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiagnosisDto>>> GetDiagnoses()
        {
            var diagnoses = await _diagnosisService.GetAllDiagnosesAsync();
            return Ok(diagnoses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiagnosisDto>> GetDiagnosis(int id)
        {
            try
            {
                var diagnosis = await _diagnosisService.GetDiagnosisByIdAsync(id);
                return Ok(diagnosis);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<DiagnosisDto>> CreateDiagnosis(DiagnosisCreateDto diagnosisDto)
        {
            try
            {
                var diagnosis = await _diagnosisService.CreateDiagnosisAsync(diagnosisDto);
                return CreatedAtAction(nameof(GetDiagnosis), new { id = diagnosis.Id }, diagnosis);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DiagnosisDto>> UpdateDiagnosis(int id, DiagnosisUpdateDto diagnosisDto)
        {
            try
            {
                var etagBase64 = Request.Headers["If-Match"].ToString();
                if (string.IsNullOrEmpty(etagBase64))
                    return BadRequest("ETag header is required");

                byte[] etag = Convert.FromBase64String(etagBase64);

                var diagnosis = await _diagnosisService.UpdateDiagnosisAsync(id, diagnosisDto, etag);
                var updatedDiagnosis = await _diagnosisService.GetDiagnosisEntityByIdAsync(id);
                var newEtag = Convert.ToBase64String(updatedDiagnosis.RowVersion);

                Response.Headers.Add("ETag", newEtag);
                return Ok(diagnosis);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(412, "Precondition failed: The resource has been modified by another user");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDiagnosis(int id)
        {
            try
            {
                await _diagnosisService.DeleteDiagnosisAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
