using Api_Maquinaria.DTOs;
using Api_Maquinaria.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api_Maquinaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _service;
        private readonly UploadthingService _uploadService;
        public EquipmentController(IEquipmentService service, UploadthingService uploadService)
        {
            _service = service;
            _uploadService = uploadService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EquipmentReadDto>>> GetAll()
        {
            try
            {
                var equipos = await _service.GetAllAsync();
                return Ok(equipos);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentReadDto>> GetById(string id)
        {
            try
            {
                var equipo = await _service.GetByIdAsync(id);
                return Ok(equipo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<EquipmentReadDto>> Post(
            [FromBody] EquipmentCreateDto dto)
        {
            try
            {
                var equipo = await _service.CreateAsync(dto);

                return Created(
                    $"/api/equipment/{equipo.Id}",
                    equipo
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(
            string id,
            [FromBody] EquipmentUpdateDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpPost("{id}/imagen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(
            string id,
           [FromForm] IFormFile file)
        {
            try
            {
                var equipo = await _service.GetByIdAsync(id);

                if (file == null || file.Length == 0)
                {
                    return BadRequest("Debe seleccionar una imagen.");
                }

                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(
                        "La imagen no puede superar los 5 MB.");
                }

                string[] tiposPermitidos =
                {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

                if (!tiposPermitidos.Contains(file.ContentType))
                {
                    return BadRequest(
                        "Solo se permiten imágenes JPG, PNG y WEBP.");
                }


                using var stream = file.OpenReadStream();

                var resultado = await _uploadService.UploadAsync(
                    stream,
                    file.FileName,
                    file.ContentType,
                    file.Length
                );

                await _service.UpdateImageAsync(
                    id,
                    resultado.Url,
                    resultado.Key
                );

                return Ok(new
                {
                    Message = "Imagen subida correctamente",
                    ImageUrl = resultado.Url
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "Error al subir la imagen.");
            }
        }

    }
}