using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessApp.Data;
using GarrasFitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarrasFitnessApp.Controllers
{
    public class PagosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create()
        {
            CargarViewData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string CiSocio, int PlanId, int? PromocionId, string MetodoPago, decimal MontoEfectivo, decimal MontoQr, decimal MontoTarjeta)
        {
            // 1. Buscar al socio por su Cédula de Identidad (CI)
            var socio = await _context.Socios.FirstOrDefaultAsync(s => s.CI == CiSocio);
            if (socio == null)
            {
                ModelState.AddModelError("", "No se encontró ningún socio con la Cédula de Identidad ingresada.");
                CargarViewData();
                return View();
            }

            // 2. Buscar el plan seleccionado
            var plan = await _context.Planes.FindAsync(PlanId);
            if (plan == null)
            {
                // Si es un plan de prueba (1, 2 o 3) y no está en la BD todavía
                if (PlanId == 1) plan = new Plan { Id = 1, Nombre = "Mensual", PrecioBase = 150, DuracionDias = 30 };
                else if (PlanId == 2) plan = new Plan { Id = 2, Nombre = "Trimestral", PrecioBase = 400, DuracionDias = 90 };
                else if (PlanId == 3) plan = new Plan { Id = 3, Nombre = "Anual", PrecioBase = 1200, DuracionDias = 365 };
                else
                {
                    ModelState.AddModelError("", "El plan seleccionado no es válido.");
                    CargarViewData();
                    return View();
                }
            }

            // 3. Buscar la promoción (si fue seleccionada)
            Promocion promocion = null;
            if (PromocionId.HasValue && PromocionId.Value > 0)
            {
                promocion = await _context.Promociones.FindAsync(PromocionId.Value);
            }

            // 4. Calcular el monto total
            decimal descuento = promocion != null ? promocion.Descuento : 0;
            decimal totalCalculado = Math.Max(0, plan.PrecioBase - descuento);

            // 5. Determinar método de pago
            string metodoFormateado = "Efectivo";
            if (MontoQr > 0 && MontoQr >= MontoEfectivo && MontoQr >= MontoTarjeta)
            {
                metodoFormateado = "QR";
            }
            else if (MontoTarjeta > 0 && MontoTarjeta >= MontoEfectivo)
            {
                metodoFormateado = "Tarjeta";
            }
            else if (!string.IsNullOrEmpty(MetodoPago))
            {
                metodoFormateado = MetodoPago;
            }

            // 6. Obtener ID del Usuario en sesión
            int usuarioId = 1;
            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int idParsed))
            {
                usuarioId = idParsed;
            }

            // 7. Actualizar la fecha de vencimiento del Socio
            DateTime fechaBaseCalculo = socio.FechaVencimiento >= DateTime.Today ? socio.FechaVencimiento : DateTime.Today;
            socio.FechaVencimiento = fechaBaseCalculo.AddDays(plan.DuracionDias);

            // 8. Crear el registro de Pago
            var pago = new Pago
            {
                SocioId = socio.Id,
                PlanId = plan.Id > 0 && _context.Planes.Any(p => p.Id == plan.Id) ? plan.Id : 1, // Asigna PlanId válido
                PromocionId = promocion?.Id,
                UsuarioId = usuarioId,
                MontoTotal = totalCalculado,
                MetodoPago = metodoFormateado,
                FechaPago = DateTime.Now
            };

            // 9. Guardar cambios en la Base de Datos
            _context.Pagos.Add(pago);
            _context.Socios.Update(socio);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Acceso");
        }

        private void CargarViewData()
        {
            var listaPlanes = _context.Planes.ToList();
            if (!listaPlanes.Any())
            {
                // Si la tabla en MySQL está vacía, genera planes temporales para pruebas
                listaPlanes = new List<Plan>
                {
                    new Plan { Id = 1, Nombre = "Mensual", PrecioBase = 150, DuracionDias = 30 },
                    new Plan { Id = 2, Nombre = "Trimestral", PrecioBase = 400, DuracionDias = 90 },
                    new Plan { Id = 3, Nombre = "Anual", PrecioBase = 1200, DuracionDias = 365 }
                };
            }
            ViewBag.PlanesList = listaPlanes;

            ViewBag.PromocionesList = _context.Promociones
                .Where(p => p.FechaInicio <= DateTime.Today && p.FechaFin >= DateTime.Today)
                .ToList();
        }
    }
}