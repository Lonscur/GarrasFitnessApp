using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessApp.Data;

namespace GarrasFitnessApp.Controllers
{
    //[Authorize] // Solo el personal logueado (Admin/Recepción) puede usar el semáforo
    public class AccesoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccesoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =======================================================
        // 1. MUESTRA LA PANTALLA VISUAL
        // =======================================================
        [HttpGet]
        public IActionResult Index()
        {
            // Retorna la vista Views/Acceso/Index.cshtml que hizo Brunell
            return View();
        }

        // =======================================================
        // 2. ENDPOINT DE CONSULTA (Lo llama el JS de Brunell)
        // =======================================================
        [HttpGet]
        public async Task<IActionResult> Verificar(string ci)
        {
            // 1. Buscamos al socio en la base de datos por su CI
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