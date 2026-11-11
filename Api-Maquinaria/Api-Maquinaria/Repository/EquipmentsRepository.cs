using Api_Maquinaria.Data;
using Api_Maquinaria.DTOs;
using Api_Maquinaria.Interfaces;
using Api_Maquinaria.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Maquinaria.Repository
{
    public class EquipmentsRepository : IEquipmentRepository
    {
        private readonly AppDbContext _context;

        public EquipmentsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Equipment>> GetAllAsync()
        {
            return await _context.Equipments
                .ToListAsync();
        }

        public async Task<Equipment?> GetByIdAsync(string id)
        {
            return await _context.Equipments.FindAsync(id);
        }

        public async Task<bool> CodeExistsAsync(
            string code, string? excludeId = null)
        {
            return await _context.Equipments.AnyAsync(
                e => e.Code == code && e.Id != excludeId
            );
        }

        public async Task<List<string>> GetIdsAsync()
        {
            return await _context.Equipments
                .Select(e => e.Id)
                .ToListAsync();
        }

        public async Task AddAsync(Equipment equipment)
        {
            await _context.Equipments.AddAsync(equipment);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Equipment equipment)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Equipment equipment)
        {
            _context.Equipments.Remove(equipment);

            await _context.SaveChangesAsync();
        }


        public async Task<PaginaResultadoDto> BuscarAsync(
            string? nombre,
            string? estado,
            string? ubicacion,
            int pagina,
            int tamano)
        {

            IQueryable<Equipment> query = _context.Equipments.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(e => e.Name.Contains(nombre));
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                query = query.Where(e => e.Status == estado);
            }

            if (!string.IsNullOrWhiteSpace(ubicacion))
            {
                query = query.Where(e => e.Location.Contains(ubicacion));
            }

            int total = await query.CountAsync();

            query = query.OrderBy(e => e.Name).ThenBy(e => e.Id);
                
            int salto = (pagina - 1) * tamano;

            var equipos = await query
                .Skip(salto)
                .Take(tamano)
                .Select(e => new EquipmentReadDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Code = e.Code,
                    Location = e.Location,
                    Status = e.Status,
                    ImageUrl = e.ImageUrl ?? ""
                })
                .ToListAsync();

            return new PaginaResultadoDto
            {
                Items = equipos,
                Total = total,
                Pagina = pagina,
                Tamano = tamano,
                TotalPaginas = (int)Math.Ceiling(
                    (double)total / tamano)
            };
        }

    }
}