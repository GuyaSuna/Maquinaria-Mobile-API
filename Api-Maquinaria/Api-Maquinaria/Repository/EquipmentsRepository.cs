using Api_Maquinaria.Data;
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
            return await _context.Equipments.ToListAsync();
        }

        public async Task<Equipment?> GetByIdAsync(string id)
        {
            return await _context.Equipments.FindAsync(id);
        }

        public async Task<bool> CodeExistsAsync(
            string code, string? excludeId = null)
        {
            return await _context.Equipments.AnyAsync(
                e => e.Code == code && e.Id != excludeId);
        }

        public async Task<List<string>> GetIdsAsync()
        {
            return await _context.Equipments.Select(e => e.Id).ToListAsync();
        }

        public async Task AddAsync(Equipment equipment)
        {
            _context.Equipments.Add(equipment);
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
    }
}
