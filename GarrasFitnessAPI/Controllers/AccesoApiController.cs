using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessAPI.Data;

namespace GarrasFitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccesoApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccesoApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Verificar/{ci}")]
        public async Task<IActionResult> Verificar(string ci)
        {
            var socio = await _context.Socios.FirstOrDefaultAsync(s => s.CI == ci);
            if (socio == null) return NotFound();

            var hoy = DateTime.Today;
            bool esValido = socio.FechaVencimiento.Date >= hoy;
            int diasVencido = esValido ? 0 : (hoy - socio.FechaVencimiento.Date).Days;

            return Ok(new
            {
                nombreCompleto = socio.NombreCompleto,
                esValido = esValido,
                fechaVencimiento = socio.FechaVencimiento.ToString("dd/MM/yyyy"),
                diasVencido = diasVencido
            });
        }
    }
}