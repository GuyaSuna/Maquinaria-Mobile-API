using Api_Maquinaria.DTOs;

namespace Api_Maquinaria.Interfaces
{
    public interface IEquipmentService
    {
        Task<List<EquipmentReadDto>> GetAllAsync();

        Task<EquipmentReadDto> GetByIdAsync(string id);

        Task<EquipmentReadDto> CreateAsync(
            EquipmentCreateDto dto);

        Task UpdateAsync(
            string id,
            EquipmentUpdateDto dto);

        Task DeleteAsync(string id);
    }
}