using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessAPI.Data;
using GarrasFitnessAPI.Models;

namespace GarrasFitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SociosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SociosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSocio(Socio socio)
        {
            bool ciExiste = await _context.Socios.AnyAsync(s => s.CI == socio.CI);
            if (ciExiste) return Conflict(new { message = "El número de CI ya se encuentra registrado." });

            _context.Socios.Add(socio);
            await _context.SaveChangesAsync();
            return Ok(socio);
        }
    }
}