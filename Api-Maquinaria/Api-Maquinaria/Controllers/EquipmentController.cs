using Api_Maquinaria.Data;
using Api_Maquinaria.DTOs;
using Api_Maquinaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Maquinaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {

        private readonly AppDbContext _context;

        public EquipmentController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var equipments = await _context.Equipments.ToListAsync();
            return Ok(equipments);

        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Equipment>> GetEquipmentById(string id)
        {

            var equipment = await _context.Equipments.FindAsync(id);

            if (equipment == null)
            {
                return NotFound(
                    $"No se encontró el equipo con ID {id}."
                );
            }

            return Ok(equipment);
        }


        [HttpPost]
        public async Task<IActionResult> Post(EquipmentInput input)
        {
            var code = input.Code.Trim().ToUpperInvariant();

            if (await _context.Equipments.AnyAsync(e => e.Code == code))
            {
                return Conflict("El código ya existe");
            }

            var ids = await _context.Equipments
                .Select(e => e.Id)
                .ToListAsync();

            var numeros = ids.Select(id => int.Parse(id.Substring(3)));

            var siguiente = numeros.DefaultIfEmpty(0).Max() + 1;

            var equipo = new Equipment
            {
                Id = "eq-" + siguiente,
                Name = input.Name.Trim(),
                Code = code,
                Location = input.Location.Trim(),
                Status = input.Status
            };

            _context.Equipments.Add(equipo);

            await _context.SaveChangesAsync();

            return Created($"/api/equipamientos/{equipo.Id}", equipo);
        }
    }
}
