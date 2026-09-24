using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessAPI.Data;
using GarrasFitnessAPI.Models;

namespace GarrasFitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CajaApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CajaApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("PagosHoy")]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagosHoy()
        {
            var hoy = DateTime.Today;
            return await _context.Pagos
                .Include(p => p.Socio)
                .Include(p => p.Plan)
                .Include(p => p.Usuario)
                .Where(p => p.FechaPago.Date == hoy)
                .ToListAsync();
        }
    }
}