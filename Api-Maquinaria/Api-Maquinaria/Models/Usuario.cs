namespace Api_Maquinaria.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Rol { get; set; } = "Consulta";
        public bool Activo { get; set; } = true;
    }

}
