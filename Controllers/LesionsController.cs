using DermatologyApi.DTOs;
using DermatologyApi.Mappers;
using DermatologyApi.Services;
using DermatologyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using DermatologyApi.Exceptions;

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
        public async Task<ActionResult<LesionDto>> GetLesionById(int id)
        {
            var lesion = await _lesionService.GetLesionEntityByIdAsync(id);
            if (lesion == null)
                return NotFound();

            var etag = lesion.Xmin.ToString();
            var ifNoneMatch = Request.Headers["If-None-Match"].ToString();
            if (etag == ifNoneMatch)
                return StatusCode(304);

            Response.Headers["ETag"] = etag;
            return Ok(LesionMapper.MapToDto(lesion));
        }

        [HttpPost]
        public async Task<ActionResult<LesionDto>> CreateLesion(LesionCreateDto lesionDto)
        {
            var lesion = await _lesionService.CreateLesionAsync(lesionDto);
            return CreatedAtAction(nameof(GetLesionById), new { id = lesion.Id }, lesion);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LesionDto>> UpdateLesion(int id, LesionUpdateDto lesionDto)
        {
            var etagBase64 = Request.Headers["If-Match"].ToString();
            if (string.IsNullOrEmpty(etagBase64))
            {
                throw new ValidationException("ETag header is required.");
            }

            var lesion = await _lesionService.GetLesionEntityByIdAsync(id);
            var etag = lesion.Xmin.ToString();

            if (etagBase64 != etag)
            {
                throw new PreconditionFailedException("Precondition failed: The resource has been modified by anther user.");
            }

            if (!uint.TryParse(etag, out uint expectedXmin))
            {
                return BadRequest("Invalid ETag format");
            }

            var updatedLesion = await _lesionService.UpdateLesionAsync(id, lesionDto, expectedXmin);

            lesion = await _lesionService.GetLesionEntityByIdAsync(id);
            etag = lesion.Xmin.ToString();

            Response.Headers["ETag"] = etag;
            return Ok(updatedLesion);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<LesionDto>> PatchLesion(int id, LesionPatchDto lesionDto)
        {
            var lesion = await _lesionService.PatchLesionAsync(id, lesionDto);
            return Ok(lesion);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLesion(int id)
        {
            await _lesionService.DeleteLesionAsync(id);
            return NoContent();
        }
    }
}
