using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessAPI.Data;
using GarrasFitnessAPI.Models;

namespace GarrasFitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SociosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SociosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSocio(Socio socio)
        {
            bool ciExiste = await _context.Socios.AnyAsync(s => s.CI == socio.CI);
            if (ciExiste) return Conflict(new { message = "El número de CI ya se encuentra registrado." });

            _context.Socios.Add(socio);
            await _context.SaveChangesAsync();
            return Ok(socio);
        }

        // HU-07: FILTRO DE MOROSOS
        [HttpGet("Morosos")]
        public async Task<IActionResult> GetMorosos()
        {
            var fechaActual = DateTime.Today;

            var morosos = await _context.Socios
                .Where(s => s.FechaVencimiento < fechaActual)
                .Select(s => new
                {
                    s.Id,
                    s.NombreCompleto,
                    s.Telefono,
                    s.FechaVencimiento,
                    DiasRetraso = EF.Functions.DateDiffDay(s.FechaVencimiento, fechaActual)
                })
                .OrderByDescending(s => s.DiasRetraso)
                .ToListAsync();

            if (!morosos.Any()) return Ok(new { mensaje = "No hay socios morosos en este momento." });

            return Ok(morosos);
        }

        // HU-08: HISTORIAL DE PAGOS
        [HttpGet("{socioId}/HistorialPagos")]
        public async Task<IActionResult> GetHistorialPagos(int socioId)
        {
            var historial = await _context.Pagos
                .Where(p => p.SocioId == socioId)
                .Join(_context.Planes,
                      pago => pago.PlanId,
                      plan => plan.Id,
                      (pago, plan) => new
                      {
                          PagoId = pago.Id,
                          PlanNombre = plan.Nombre,
                          pago.MontoTotal,
                          pago.FechaPago,
                          pago.MetodoPago
                      })
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();

            if (!historial.Any()) return Ok(new { mensaje = "Este socio aún no tiene pagos registrados en su historial." });

            return Ok(historial);
        }

        // HU-09: CHECK-IN DE ASISTENCIA (¡El toque final!)
        [HttpPost("{socioId}/CheckIn")]
        public async Task<IActionResult> RegistrarAsistencia(int socioId)
        {
            var socio = await _context.Socios.FindAsync(socioId);
            if (socio == null)
            {
                return NotFound(new { mensaje = "El socio no existe." });
            }

            var asistencia = new Asistencia
            {
                SocioId = socioId,
                FechaAsistencia = DateTime.Now
            };

            _context.Asistencias.Add(asistencia);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Check-in exitoso para {socio.NombreCompleto} a las {asistencia.FechaAsistencia:HH:mm}" });
        }
    }
}