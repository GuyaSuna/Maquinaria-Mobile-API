namespace Api_Maquinaria.DTOs
{
    public class PaginaResultadoDto
    {
        public List<EquipmentReadDto> Items { get; set; } = new();

        public int Total { get; set; }

        public int Pagina { get; set; }

        public int Tamano { get; set; }

        public int TotalPaginas { get; set; }
    }
}