using GarrasFitnessApp.Data;
using GarrasFitnessApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarrasFitnessApp.Controllers
{
    public class SociosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SociosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Socio socio)
        {
       
            bool ciExiste = await _context.Socios.AnyAsync(s => s.CI == socio.CI);

            if (ciExiste)
            {
                ModelState.AddModelError("CI", "El número de CI ya se encuentra registrado.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(socio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }

            return View(socio);
        }
    }
}