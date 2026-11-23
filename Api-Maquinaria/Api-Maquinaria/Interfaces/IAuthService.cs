
using Api_Maquinaria.DTOs;

namespace Api_Maquinaria.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<RegisterResponseDto?> RegistrarAsync(RegisterRequestDto request);
    
}
