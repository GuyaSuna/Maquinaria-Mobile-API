using System.ComponentModel.DataAnnotations;

namespace Api_Maquinaria.DTOs
{
    public class EquipmentUpdateDto
    {
        [Required, MinLength(3)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public string Location { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
