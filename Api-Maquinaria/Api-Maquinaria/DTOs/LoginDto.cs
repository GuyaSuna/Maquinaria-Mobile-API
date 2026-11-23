namespace Api_Maquinaria.DTOs
{
    public record LoginRequestDto(
       string Email,
       string Password
   );

    public record LoginResponseDto(
        string AccessToken,
        DateTime ExpiresAtUtc,
        string Nombre,
        string Rol
    );
}
