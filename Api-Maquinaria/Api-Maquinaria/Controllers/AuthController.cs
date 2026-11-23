
using Api_Maquinaria.DTOs;
using Api_Maquinaria.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api_Maquinaria.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
     LoginRequestDto request)
    {
        try
        {
            var resultado = await _authService.LoginAsync(request);

            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var resultado = await _authService.RegistrarAsync(request);

        if (resultado == null)
        {
            return Conflict(new
            {
                mensaje = "El email ya está registrado."
            });
        }

        return Created("/api/auth/login", resultado);
    }
}
