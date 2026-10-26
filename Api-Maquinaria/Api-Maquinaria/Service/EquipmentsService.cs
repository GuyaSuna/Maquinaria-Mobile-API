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

        private static Equipment ToEntity(
            EquipmentCreateDto dto, string id)
        {
            return new Equipment
            {
                Id = id,
                Name = dto.Name.Trim(),
                Code = dto.Code.Trim().ToUpperInvariant(),
                Location = dto.Location.Trim(),
                Status = dto.Status.Trim()
            };
        }

        public async Task<List<EquipmentReadDto>> GetAllAsync()
        {
            var equipos = await _repository.GetAllAsync();

            return equipos.Select(e => ToReadDto(e)).ToList();
        }

        public async Task<EquipmentReadDto> GetByIdAsync(string id)
        {
            ValidarId(id);

            var equipo = await _repository.GetByIdAsync(id);

            if (equipo == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el equipamiento con ID {id}");
            }

            return ToReadDto(equipo);
        }

        public async Task<EquipmentReadDto> CreateAsync(
            EquipmentCreateDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentException(
                    "Los datos del equipamiento son obligatorios.");
            }

            ValidarDatos(
                dto.Name,
                dto.Code,
                dto.Location,
                dto.Status
            );

            var code = dto.Code.Trim().ToUpperInvariant();

            var repetido = await _repository.CodeExistsAsync(code);

            if (repetido)
            {
                throw new InvalidOperationException(
                    $"Ya existe un equipamiento con el código {code}");
            }

            var ids = await _repository.GetIdsAsync();

            var numeros = new List<int>();

            foreach (var id in ids)
            {
                if (id.StartsWith("eq-") &&
                    int.TryParse(id.Substring(3), out int numero))
                {
                    numeros.Add(numero);
                }
            }

            var siguiente = numeros.DefaultIfEmpty(0).Max() + 1;

            var nuevoId = "eq-" + siguiente;

            var equipo = ToEntity(dto, nuevoId);

            await _repository.AddAsync(equipo);

            return ToReadDto(equipo);
        }

        public async Task UpdateAsync(
            string id, EquipmentUpdateDto dto)
        {
            ValidarId(id);

            if (dto == null)
            {
                throw new ArgumentException(
                    "Los datos del equipamiento son obligatorios.");
            }

            ValidarDatos(
                dto.Name,
                dto.Code,
                dto.Location,
                dto.Status
            );

            var equipo = await _repository.GetByIdAsync(id);

            if (equipo == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el equipamiento con ID {id}");
            }

            var code = dto.Code.Trim().ToUpperInvariant();

            var repetido = await _repository.CodeExistsAsync(code, id);

            if (repetido)
            {
                throw new InvalidOperationException(
                    $"Ya existe un equipamiento con el código {code}");
            }

            equipo.Name = dto.Name.Trim();
            equipo.Code = code;
            equipo.Location = dto.Location.Trim();
            equipo.Status = dto.Status.Trim();

            await _repository.UpdateAsync(equipo);
        }

        public async Task DeleteAsync(string id)
        {
            ValidarId(id);

            var equipo = await _repository.GetByIdAsync(id);

            if (equipo == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el equipamiento con ID {id}");
            }

            await _repository.DeleteAsync(equipo);
        }

        private static void ValidarId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    "El ID del equipamiento es obligatorio.");
            }
        }

        private static void ValidarDatos(
            string name,
            string code,
            string location,
            string status)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "El nombre del equipamiento no puede estar vacío.");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(
                    "El código del equipamiento no puede estar vacío.");
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentException(
                    "La ubicación del equipamiento no puede estar vacía.");
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException(
                    "El estado del equipamiento no puede estar vacío.");
            }
        }
    }
}