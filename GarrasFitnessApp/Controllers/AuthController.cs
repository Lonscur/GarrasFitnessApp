using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using GarrasFitnessApp.Data;
using GarrasFitnessApp.Models;
using BCrypt.Net;

namespace GarrasFitnessApp.Controllers
{
    public class AuthController : Controller
    {
        // 1. Variable privada para conectar a la base de datos
        private readonly ApplicationDbContext _context;

        // 2. Constructor con inyección de dependencias
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Login()
        {
            // Verificamos si ya inició sesión previamente
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Si ya está logueado, lo manda directo al panel
                return RedirectToAction("Index", "Home");
            }

            // Si no, muestra el formulario de Mirkho
            return View();
        }

 
        [HttpPost]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            // Validar que no manden campos vacíos
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                ViewBag.Error = "Debe ingresar su correo y contraseña.";
                return View();
            }

            // Buscar al usuario en la base de datos e incluir su Rol relacional (INNER JOIN)
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == correo);

            // Validar si el usuario existe y verificar el hash de la contraseña con BCrypt
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(contrasena, usuario.Contrasena))
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }

            // Crear los "Claims" (Las etiquetas de identificación del usuario)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre) // Saca "Administrador" o "Recepcionista"
            };

            // Empaquetar los claims bajo el esquema de Cookies de ASP.NET
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Generar la Cookie de sesión y guardarla en el navegador
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // Redirigir al inicio del sistema tras autenticarse con éxito
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Elimina la cookie del navegador y destruye la sesión
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Lo manda de regreso a la pantalla de login
            return RedirectToAction("Login", "Auth");
        }
    }
}