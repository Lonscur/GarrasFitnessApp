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
            // Retorna la vista Views/Acceso/Index.cshtml que hizo Brunell
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Verificar(string ci)
        {

            var socio = await _context.Socios.FirstOrDefaultAsync(s => s.CI == ci);

            // Si no existe, mandamos un error 404 para que el JS lance su alerta
            if (socio == null)
            {
                return NotFound();
            }

            // 2. Lógica matemática de fechas
            var hoy = DateTime.Today;
            bool esValido = socio.FechaVencimiento.Date >= hoy;

            // Calculamos cuántos días lleva vencido (si aplica)
            int diasVencido = esValido ? 0 : (hoy - socio.FechaVencimiento.Date).Days;

            // 3. Devolvemos los datos empaquetados en JSON
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