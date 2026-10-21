using Api_Maquinaria.Data;
using Api_Maquinaria.DTOs;
using Api_Maquinaria.Interfaces;
using Api_Maquinaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Maquinaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {

        private readonly IEquipmentService _service;

        public EquipmentController(IEquipmentService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<ActionResult<List<EquipmentReadDto>>> GetAll()
        {
            var equipos = await _service.GetAllAsync();
            return Ok(equipos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentReadDto>> GetById(string id)
        {
            try
            {
            var equipo = await _service.GetByIdAsync(id);

            return Ok(equipo);
            }catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }catch (Exception ex)
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
            return Created($"/api/equipment/{equipo?.Id}", equipo);
            }catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");

            }
         }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(
            string id, [FromBody] EquipmentUpdateDto dto)
        {
            var resultado = await _service.UpdateAsync(id, dto);
            if (resultado == EquipmentUpdateResult.NotFound)
                return NotFound("Equipamiento inexistente");
            if (resultado == EquipmentUpdateResult.DuplicateCode)
                return Conflict("El código ya existe");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var eliminado = await _service.DeleteAsync(id);
            if (!eliminado) return NotFound("Equipamiento inexistente");
            return NoContent();
        }
    }
}
