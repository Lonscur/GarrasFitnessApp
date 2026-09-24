using Microsoft.AspNetCore.Mvc;
using GarrasFitnessApp.Models;
using System.Text.Json;
using System.Text;

namespace GarrasFitnessApp.Controllers
{
    public class SociosController : Controller
    {
        private readonly HttpClient _httpClient;

        public SociosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7286/");
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
            if (!ModelState.IsValid) return View(socio);

            var content = new StringContent(JsonSerializer.Serialize(socio), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/SociosApi", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Create));
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                ModelState.AddModelError("CI", "El número de CI ya se encuentra registrado.");
            }

            return View(socio);
        }
    }
}