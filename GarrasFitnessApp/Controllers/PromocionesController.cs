using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessApp.Data;
using GarrasFitnessApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace GarrasFitnessApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PromocionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PromocionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Promociones.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descuento,FechaInicio,FechaFin")] Promocion promocion)
        {
            if (promocion.FechaInicio > promocion.FechaFin)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser menor a la fecha de inicio.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(promocion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(promocion);
        }

        // GET: Promociones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion == null) return NotFound();

            return View(promocion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descuento,FechaInicio,FechaFin")] Promocion promocion)
        {
            if (id != promocion.Id) return NotFound();

            if (promocion.FechaInicio > promocion.FechaFin)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser menor a la de inicio.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promocion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromocionExists(promocion.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(promocion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion != null)
            {
                _context.Promociones.Remove(promocion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PromocionExists(int id)
        {
            return _context.Promociones.Any(e => e.Id == id);
        }
    }
}