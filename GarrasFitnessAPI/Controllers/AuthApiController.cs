using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessAPI.Data;
using GarrasFitnessAPI.Models;

namespace GarrasFitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == request.Correo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena))
            {
                return Unauthorized();
            }

            return Ok(usuario);
        }

        [HttpGet("CrearAdminInicial")]
        public async Task<IActionResult> CrearAdminInicial()
        {
            if (_context.Usuarios.Any())
            {
                return BadRequest("La base de datos ya tiene usuarios registrados.");
            }

            var admin = new Usuario
            {
                NombreCompleto = "Administrador Master",
                Correo = "admin@garras.com",
                Contrasena = BCrypt.Net.BCrypt.HashPassword("123456"),
                RolId = 1
            };

            _context.Usuarios.Add(admin);
            await _context.SaveChangesAsync();

            return Ok("Usuario Administrador inyectado con éxito. Ya puedes iniciar sesión.");
        }
    }

    public class LoginRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}