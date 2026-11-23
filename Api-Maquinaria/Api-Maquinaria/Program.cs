using Api_Maquinaria.Data;
using Api_Maquinaria.Interfaces;
using Api_Maquinaria.Models;
using Api_Maquinaria.Repository;
using Api_Maquinaria.Service;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<MercadoPagoService>();
builder.Services.AddHttpClient<UploadthingService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("DefaultConnection");

    options.UseMySQL(connectionString!);
});

builder.Services.AddScoped<IEquipmentService, EquipmentsService>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentsRepository>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IPasswordHasher<Usuario>,PasswordHasher<Usuario>>();
    
var jwtKey = builder.Configuration["Jwt_Key"]
    ?? throw new InvalidOperationException(
        "No se configuró Jwt_Key");

var jwtIssuer = builder.Configuration["Jwt_Issuer"]
    ?? throw new InvalidOperationException(
        "No se configuró Jwt_Issuer");

var jwtAudience = builder.Configuration["Jwt_Audience"]
    ?? throw new InvalidOperationException(
        "No se configuró Jwt_Audience");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "La clave JWT debe tener al menos 32 bytes.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ValidateLifetime = true,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
