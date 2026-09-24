using Microsoft.AspNetCore.Mvc;
using GarrasFitnessApp.Models;
using System.Text.Json;
using System.Text;

namespace GarrasFitnessApp.Controllers
{
    public class PromocionesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public PromocionesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7286/");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/PromocionesApi");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var promociones = JsonSerializer.Deserialize<IEnumerable<Promocion>>(jsonString, _jsonOptions);
                return View(promociones);
            }
            return View(new List<Promocion>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descuento,FechaInicio,FechaFin")] Promocion promocion)
        {
            ModelState.Remove("Pagos");
            ModelState.Remove("Id");

            if (promocion.FechaInicio > promocion.FechaFin)
                ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser menor a la fecha de inicio.");

            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonSerializer.Serialize(promocion), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/PromocionesApi", content);
                if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            }
            return View(promocion);
        }
    }
}