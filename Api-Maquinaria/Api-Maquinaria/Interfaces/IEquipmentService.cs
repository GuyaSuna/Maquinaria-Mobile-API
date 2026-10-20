using Api_Maquinaria.DTOs;
using Api_Maquinaria.Models;

namespace Api_Maquinaria.Interfaces
{
    public enum EquipmentUpdateResult
    {
        Success, NotFound, DuplicateCode
    }
    public interface IEquipmentService
    {
        Task<List<EquipmentReadDto>> GetAllAsync();
        Task<EquipmentReadDto?> GetByIdAsync(string id);
        Task<EquipmentReadDto?> CreateAsync(EquipmentCreateDto dto);
        Task<EquipmentUpdateResult> UpdateAsync(
            string id, EquipmentUpdateDto dto);
        Task<bool> DeleteAsync(string id);

    }
}
