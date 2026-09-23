using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessApp.Data;

namespace GarrasFitnessApp.Controllers
{
    //[Authorize] 
    public class AccesoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccesoController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
     
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Verificar(string ci)
        {

            var socio = await _context.Socios.FirstOrDefaultAsync(s => s.CI == ci);


            if (socio == null)
            {
                return NotFound();
            }

            var hoy = DateTime.Today;
            bool esValido = socio.FechaVencimiento.Date >= hoy;


            int diasVencido = esValido ? 0 : (hoy - socio.FechaVencimiento.Date).Days;

 
            return Json(new
            {
                nombreCompleto = socio.NombreCompleto,
                esValido = esValido,
                fechaVencimiento = socio.FechaVencimiento.ToString("dd/MM/yyyy"),
                diasVencido = diasVencido
            });
        }
    }
}