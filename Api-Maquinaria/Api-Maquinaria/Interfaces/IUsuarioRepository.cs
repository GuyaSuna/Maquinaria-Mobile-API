using Api_Maquinaria.Models;

namespace Api_Maquinaria.Interfaces
{
    public interface IUsuarioRepository
    {
        
        Task<Usuario?> GetByEmailAsync(string email);
        Task AddAsync(Usuario usuario);

    }

}

