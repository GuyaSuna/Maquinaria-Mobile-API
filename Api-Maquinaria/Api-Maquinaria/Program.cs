using Api_Maquinaria.Data;
using Api_Maquinaria.Interfaces;
using Api_Maquinaria.Repository;
using Api_Maquinaria.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseMySQL(connectionString!);
});


builder.Services.AddScoped<IEquipmentService, EquipmentsService>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentsRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
