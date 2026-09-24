using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessAPI.Data;
using GarrasFitnessAPI.Models;

namespace GarrasFitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PagosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("DatosCreacion")]
        public async Task<IActionResult> GetDatosCreacion()
        {
            var planes = await _context.Planes.ToListAsync();
            if (!planes.Any())
            {
                planes = new List<Plan>
                {
                    new Plan { Id = 1, Nombre = "Mensual", PrecioBase = 150, DuracionDias = 30 },
                    new Plan { Id = 2, Nombre = "Trimestral", PrecioBase = 400, DuracionDias = 90 },
                    new Plan { Id = 3, Nombre = "Anual", PrecioBase = 1200, DuracionDias = 365 }
                };
            }

            var promociones = await _context.Promociones
                .Where(p => p.FechaInicio <= DateTime.Today && p.FechaFin >= DateTime.Today)
                .ToListAsync();

            return Ok(new { Planes = planes, Promociones = promociones });
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarPago([FromBody] PagoRequest request)
        {
            var socio = await _context.Socios.FirstOrDefaultAsync(s => s.CI == request.CiSocio);
            if (socio == null) return NotFound("Socio no encontrado");

            var plan = await _context.Planes.FindAsync(request.PlanId);
            if (plan == null)
            {
                if (request.PlanId == 1) plan = new Plan { Id = 1, Nombre = "Mensual", PrecioBase = 150, DuracionDias = 30 };
                else if (request.PlanId == 2) plan = new Plan { Id = 2, Nombre = "Trimestral", PrecioBase = 400, DuracionDias = 90 };
                else if (request.PlanId == 3) plan = new Plan { Id = 3, Nombre = "Anual", PrecioBase = 1200, DuracionDias = 365 };
                else return BadRequest("Plan inválido");
            }

            Promocion? promocion = null;
            if (request.PromocionId.HasValue && request.PromocionId.Value > 0)
                promocion = await _context.Promociones.FindAsync(request.PromocionId.Value);

            decimal descuento = promocion?.Descuento ?? 0;
            decimal totalCalculado = Math.Max(0, plan.PrecioBase - descuento);

            string metodoFormateado = "Efectivo";
            if (request.MontoQr > 0 && request.MontoQr >= request.MontoEfectivo && request.MontoQr >= request.MontoTarjeta)
                metodoFormateado = "QR";
            else if (request.MontoTarjeta > 0 && request.MontoTarjeta >= request.MontoEfectivo)
                metodoFormateado = "Tarjeta";
            else if (!string.IsNullOrEmpty(request.MetodoPago))
                metodoFormateado = request.MetodoPago;

            DateTime fechaBaseCalculo = socio.FechaVencimiento >= DateTime.Today ? socio.FechaVencimiento : DateTime.Today;
            socio.FechaVencimiento = fechaBaseCalculo.AddDays(plan.DuracionDias);

            var pago = new Pago
            {
                SocioId = socio.Id,
                PlanId = plan.Id > 0 && _context.Planes.Any(p => p.Id == plan.Id) ? plan.Id : 1,
                PromocionId = promocion?.Id,
                UsuarioId = request.UsuarioId,
                MontoTotal = totalCalculado,
                MetodoPago = metodoFormateado,
                FechaPago = DateTime.Now
            };

            _context.Pagos.Add(pago);
            _context.Socios.Update(socio);
            await _context.SaveChangesAsync();

            return Ok(pago);
        }
    }

    public class PagoRequest
    {
        public string CiSocio { get; set; } = string.Empty;
        public int PlanId { get; set; }
        public int? PromocionId { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public decimal MontoEfectivo { get; set; }
        public decimal MontoQr { get; set; }
        public decimal MontoTarjeta { get; set; }
        public int UsuarioId { get; set; }
    }
}