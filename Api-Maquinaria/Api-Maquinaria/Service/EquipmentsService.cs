using Api_Maquinaria.DTOs;
using Api_Maquinaria.Interfaces;
using Api_Maquinaria.Models;

namespace Api_Maquinaria.Service
{
    public class EquipmentsService : IEquipmentService
    {

        private readonly IEquipmentRepository _repository;

        public EquipmentsService(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        private static EquipmentReadDto ToReadDto(Equipment equipo)
        {
            return new EquipmentReadDto
            {
                Id = equipo.Id,
                Name = equipo.Name,
                Code = equipo.Code,
                Location = equipo.Location,
                Status = equipo.Status
            };
        }

        private static Equipment ToEntity(EquipmentCreateDto dto, string id)
        {
            return new Equipment
            {
                Id = id,
                Name = dto.Name.Trim(),
                Code = dto.Code.Trim().ToUpper(),
                Location = dto.Location.Trim(),
                Status = dto.Status
            };
        }

        public async Task<List<EquipmentReadDto>> GetAllAsync()
        {
            var equipos = await _repository.GetAllAsync();
            return equipos.Select(e => ToReadDto(e)).ToList();
        }

        public async Task<EquipmentReadDto?> GetByIdAsync(string id)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) throw new KeyNotFoundException($"No se encontró el equipamiento con id {id}");
            return ToReadDto(equipo);
        }

        public async Task<EquipmentReadDto?> CreateAsync(
            EquipmentCreateDto dto)
        {
            Validar(dto);
            var code = dto.Code.Trim().ToUpper();
            var repetido = await _repository.CodeExistsAsync(code);
            if (repetido) throw new ArgumentException($"Ya existe un equipamiento con el código {code}");

            var ids = await _repository.GetIdsAsync();
            var numeros = ids.Select(id => int.Parse(id.Substring(3)));
            var siguiente = numeros.DefaultIfEmpty(0).Max() + 1;
            var nuevoId = "eq-" + siguiente;
            var equipo = ToEntity(dto, nuevoId);

            await _repository.AddAsync(equipo);
            return ToReadDto(equipo);
        }

        public async Task<EquipmentUpdateResult> UpdateAsync(
            string id, EquipmentUpdateDto dto)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) throw new KeyNotFoundException($"No se encontró el equipamiento con id {id}");

            var code = dto.Code.Trim().ToUpper();
            var repetido = await _repository.CodeExistsAsync(code, id);
            if (repetido) return EquipmentUpdateResult.DuplicateCode;

            equipo.Name = dto.Name.Trim();
            equipo.Code = code;
            equipo.Location = dto.Location.Trim();
            equipo.Status = dto.Status;
            await _repository.UpdateAsync(equipo);
            return EquipmentUpdateResult.Success;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return false;
            await _repository.DeleteAsync(equipo);
            return true;
        }

        public void Validar(EquipmentCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("El nombre del equipamiento no puede estar vacío.");
            }
            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                throw new ArgumentException("El código del equipamiento no puede estar vacío.");
            }
            if(string.IsNullOrWhiteSpace(dto.Location))
            {
                throw new ArgumentException("La ubicación del equipamiento no puede estar vacía.");
            }
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                throw new ArgumentException("El estado del equipamiento no puede estar vacío.");
            }
        }
    }
}
