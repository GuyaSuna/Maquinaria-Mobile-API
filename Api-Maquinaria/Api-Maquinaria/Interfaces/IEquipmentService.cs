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


        Task UpdateImageAsync(
            string id,
            string imageUrl,
            string imageKey
            );

        Task<PaginaResultadoDto> BuscarAsync(string? nombre,string? estado,
            string? ubicacion,
            int pagina,
            int tamano
            );
    }

}