using Api_Maquinaria.DTOs;
using Api_Maquinaria.Interfaces;
using Api_Maquinaria.Models;
using Api_Maquinaria.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api_Maquinaria.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AuthService(
            IUsuarioRepository repository,
            IConfiguration config,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _repository = repository;
            _config = config;
            _passwordHasher = passwordHasher;
        }


        public async Task<LoginResponseDto?> LoginAsync(
            LoginRequestDto request)
        {
            ValidarDatos(request);

            var email = request.Email.Trim().ToLowerInvariant();

            var usuario = await _repository.GetByEmailAsync(email);

            if (usuario == null || !usuario.Activo)
            {
                throw new UnauthorizedAccessException(
                    "Correo o contraseña incorrectos.");
            }

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                request.Password
            );

            if (resultado == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException(
                    "Correo o contraseña incorrectos.");
            }

            return GenerarToken(usuario);
        }


        private LoginResponseDto GenerarToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    usuario.Id.ToString()
                ),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    usuario.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    usuario.Rol
                ),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                )
            };

            var keyText = _config["Jwt_Key"];

            if (string.IsNullOrWhiteSpace(keyText))
            {
                throw new InvalidOperationException(
                    "No se configuró la clave JWT.");
            }


            var issuer = _config["Jwt_Issuer"];
            var audience = _config["Jwt_Audience"];

            if (string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "Falta configurar el emisor o la audiencia JWT.");
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyText)
            );

            var credentials = new SigningCredentials( key,
                SecurityAlgorithms.HmacSha256
            );

            var expires = DateTime.UtcNow.AddMinutes(60);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto(
                accessToken,
                expires,
                usuario.Nombre,
                usuario.Rol
            );
        }

        public async Task<RegisterResponseDto?> RegistrarAsync(RegisterRequestDto request)
        {

            var email = request.Email.Trim().ToLowerInvariant();

            var existente = await _repository.GetByEmailAsync(email);

            if (existente != null)
            {
                return null;
            }

            var usuario = new Usuario
            {
                Nombre = request.Nombre.Trim(),
                Email = email,
                Rol = "USER",
                Activo = true
            };

             //Despues vemos el VerifyHashedPassword
            usuario.PasswordHash = _passwordHasher.HashPassword(
                usuario,
                request.Password
            );

            await _repository.AddAsync(usuario);

            return new RegisterResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };
        }

        private static void ValidarDatos(LoginRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentException(
                    "Los datos de inicio de sesión son obligatorios.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException(
                    "El correo electrónico es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException(
                    "La contraseña es obligatoria.");
            }
        }
    }
}