
namespace Api_Maquinaria.Models
{
    public class PagoRevision
    {
        public int Id { get; set; }

        public string EquipmentId { get; set; } = "";

        public string? MercadoPagoOrderId { get; set; }

        public string Referencia { get; set; } = "";

        public string IdempotencyKey { get; set; } = "";

        public string Moneda { get; set; } = "UYU";

        public decimal Monto { get; set; }

        public string Estado { get; set; } = "PENDIENTE";

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
