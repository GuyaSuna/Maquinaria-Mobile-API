using Api_Maquinaria.Interfaces;
using Api_Maquinaria.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api_Maquinaria.Controllers;

[ApiController]
[Route("api/pagos")]
public class PagosController : ControllerBase
{
    private readonly MercadoPagoService _pagoService;
    private readonly IEquipmentService _equipmentService;

    public PagosController(
        MercadoPagoService pagoService,
        IEquipmentService equipmentService)
    {
        _pagoService = pagoService;
        _equipmentService = equipmentService;
    }

    [HttpPost("crear/{equipmentId}")]
    public async Task<IActionResult> CrearPago(string equipmentId)
    {
        var equipo = await _equipmentService.GetByIdAsync(equipmentId);

        if (equipo is null)
            return NotFound("No existe el equipamiento.");

        var url = await _pagoService.CrearPagoAsync(
            equipo.Id,
            equipo.Name
        );

        return Ok(new { url });
    }
}
