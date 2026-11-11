using Api_Maquinaria.DTOs;
using Api_Maquinaria.Models;

namespace Api_Maquinaria.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<List<Equipment>> GetAllAsync();
        Task<Equipment?> GetByIdAsync(string id);
        Task<bool> CodeExistsAsync(string code, string? excludeId = null);
        Task<List<string>> GetIdsAsync();
        Task AddAsync(Equipment equipment);
        Task UpdateAsync(Equipment equipment);
        Task DeleteAsync(Equipment equipment);
        Task<PaginaResultadoDto> BuscarAsync(
            string? nombre,
            string? estado,
            string? ubicacion,
            int pagina,
            int tamano
        );
    }
}
