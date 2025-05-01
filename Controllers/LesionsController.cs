using DermatologyApi.DTOs;
using DermatologyApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LesionsController : ControllerBase
    {
        private readonly ILesionService _lesionService;

        public LesionsController(ILesionService lesionService)
        {
            _lesionService = lesionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LesionDto>>> GetLesions()
        {
            var lesions = await _lesionService.GetAllLesionsAsync();
            return Ok(lesions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LesionDto>> GetLesion(int id)
        {
            try
            {
                var lesion = await _lesionService.GetLesionByIdAsync(id);
                return Ok(lesion);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<LesionDto>> CreateLesion(LesionCreateDto lesionDto)
        {
            var lesion = await _lesionService.CreateLesionAsync(lesionDto);
            return CreatedAtAction(nameof(GetLesion), new { id = lesion.Id }, lesion);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LesionDto>> UpdateLesion(int id, LesionUpdateDto lesionDto)
        {
            try
            {
                var etagBase64 = Request.Headers["If-Match"].ToString();
                if (string.IsNullOrEmpty(etagBase64))
                    return BadRequest("ETag header is required");

                byte[] etag = Convert.FromBase64String(etagBase64);

                var lesion = await _lesionService.UpdateLesionAsync(id, lesionDto, etag);

                var updatedLesion = await _lesionService.GetLesionEntityByIdAsync(id);
                var newEtag = Convert.ToBase64String(updatedLesion.RowVersion);

                Response.Headers.Add("ETag", newEtag);
                return Ok(lesion);
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

        [HttpPatch("{id}")]
        public async Task<ActionResult<LesionDto>> PatchLesion(int id, LesionPatchDto lesionDto)
        {
            try
            {
                var lesion = await _lesionService.PatchLesionAsync(id, lesionDto);
                return Ok(lesion);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLesion(int id)
        {
            try
            {
                await _lesionService.DeleteLesionAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
