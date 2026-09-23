using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessApp.Data;
using GarrasFitnessApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GarrasFitnessApp.Controllers
{
    [Authorize] // Admin y Recepcionista pueden cobrar
    public class PagosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            ViewData["SocioId"] = new SelectList(_context.Socios, "Id", "NombreCompleto");
            ViewData["PlanId"] = new SelectList(_context.Planes, "Id", "Nombre");
            var promocionesVigentes = _context.Promociones
                .Where(p => p.FechaInicio <= DateTime.Today && p.FechaFin >= DateTime.Today)
                .ToList();
            ViewData["PromocionId"] = new SelectList(promocionesVigentes, "Id", "Nombre");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SocioId,PlanId,PromocionId,MetodoPago")] Pago pago)
        {
            var plan = await _context.Planes.FindAsync(pago.PlanId);
            var promocion = pago.PromocionId.HasValue ? await _context.Promociones.FindAsync(pago.PromocionId) : null;
            var socio = await _context.Socios.FindAsync(pago.SocioId);

            if (plan == null || socio == null)
            {
                ModelState.AddModelError("", "Error al procesar los datos del plan o del socio.");
                CargarViewData(pago);
                return View(pago);
            }

            decimal descuento = promocion != null ? promocion.Descuento : 0;
            pago.MontoTotal = Math.Max(0, plan.PrecioBase - descuento);
            pago.FechaPago = DateTime.Now;

            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int usuarioId))
            {
                pago.UsuarioId = usuarioId;
            }
            else
            {
                return Unauthorized("La sesión no es válida para procesar pagos.");
            }

            DateTime fechaBaseCalculo = socio.FechaVencimiento >= DateTime.Today ? socio.FechaVencimiento : DateTime.Today;
            socio.FechaVencimiento = fechaBaseCalculo.AddDays(plan.DuracionDias);

            if (ModelState.IsValid)
            {
                _context.Pagos.Add(pago);
                _context.Update(socio);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Acceso");
            }

            CargarViewData(pago);
            return View(pago);
        }

        private void CargarViewData(Pago pago)
        {
            ViewData["SocioId"] = new SelectList(_context.Socios, "Id", "NombreCompleto", pago.SocioId);
            ViewData["PlanId"] = new SelectList(_context.Planes, "Id", "Nombre", pago.PlanId);
            var promocionesVigentes = _context.Promociones
                .Where(p => p.FechaInicio <= DateTime.Today && p.FechaFin >= DateTime.Today)
                .ToList();
            ViewData["PromocionId"] = new SelectList(promocionesVigentes, "Id", "Nombre", pago.PromocionId);
        }
    }
}