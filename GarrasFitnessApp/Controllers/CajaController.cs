using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessApp.Data;
using GarrasFitnessApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Rotativa.AspNetCore; 

namespace GarrasFitnessApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CajaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CajaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;

            var pagosHoy = await _context.Pagos
                .Include(p => p.Socio)
                .Include(p => p.Plan)
                .Include(p => p.Usuario)
                .Where(p => p.FechaPago.Date == hoy)
                .ToListAsync();

            decimal totalEfectivo = pagosHoy.Where(p => p.MetodoPago == "Efectivo").Sum(p => p.MontoTotal);
            decimal totalQR = pagosHoy.Where(p => p.MetodoPago == "QR").Sum(p => p.MontoTotal);

            ViewBag.TotalEfectivo = totalEfectivo;
            ViewBag.TotalQR = totalQR;
            ViewBag.TotalGeneral = totalEfectivo + totalQR;
            ViewBag.FechaArqueo = hoy.ToString("dd/MM/yyyy");

            return View(pagosHoy);
        }

        public async Task<IActionResult> ReportePdf()
        {
            var hoy = DateTime.Today;

            var pagosHoy = await _context.Pagos
                .Include(p => p.Socio)
                .Include(p => p.Plan)
                .Include(p => p.Usuario)
                .Where(p => p.FechaPago.Date == hoy)
                .ToListAsync();

            ViewBag.TotalEfectivo = pagosHoy.Where(p => p.MetodoPago == "Efectivo").Sum(p => p.MontoTotal);
            ViewBag.TotalQR = pagosHoy.Where(p => p.MetodoPago == "QR").Sum(p => p.MontoTotal);
            ViewBag.TotalGeneral = ViewBag.TotalEfectivo + ViewBag.TotalQR;
            ViewBag.FechaArqueo = hoy.ToString("dd/MM/yyyy");

           
            return new ViewAsPdf("ReportePdf", pagosHoy)
            {
                FileName = $"Arqueo_Caja_{hoy:dd_MM_yyyy}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }
    }
}