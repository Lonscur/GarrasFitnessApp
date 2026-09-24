using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessAPI.Data;
using GarrasFitnessAPI.Models;

namespace GarrasFitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromocionesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PromocionesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promocion>>> GetPromociones()
        {
            return await _context.Promociones.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Promocion>> GetPromocion(int id)
        {
            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion == null) return NotFound();
            return promocion;
        }

        [HttpPost]
        public async Task<ActionResult<Promocion>> PostPromocion(Promocion promocion)
        {
            _context.Promociones.Add(promocion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPromocion), new { id = promocion.Id }, promocion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromocion(int id, Promocion promocion)
        {
            if (id != promocion.Id) return BadRequest();
            _context.Entry(promocion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromocion(int id)
        {
            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion == null) return NotFound();

            _context.Promociones.Remove(promocion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}