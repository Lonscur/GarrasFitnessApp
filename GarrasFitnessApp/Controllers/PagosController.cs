using Microsoft.AspNetCore.Mvc;
using GarrasFitnessApp.Models;
using System.Text.Json;
using System.Text;

namespace GarrasFitnessApp.Controllers
{
    public class PagosController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public PagosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7286/");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Create()
        {
            await CargarViewDataAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string CiSocio, int PlanId, int? PromocionId, string MetodoPago, decimal MontoEfectivo, decimal MontoQr, decimal MontoTarjeta)
        {
            int usuarioId = 1;
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int idParsed))
            {
                usuarioId = idParsed;
            }

            var requestData = new
            {
                CiSocio,
                PlanId,
                PromocionId,
                MetodoPago,
                MontoEfectivo,
                MontoQr,
                MontoTarjeta,
                UsuarioId = usuarioId
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/PagosApi", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Acceso");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                ModelState.AddModelError("", "No se encontró ningún socio con la Cédula de Identidad ingresada.");
            else
                ModelState.AddModelError("", "El plan seleccionado no es válido o hubo un error al procesar.");

            await CargarViewDataAsync();
            return View();
        }

        private async Task CargarViewDataAsync()
        {
            var response = await _httpClient.GetAsync("api/PagosApi/DatosCreacion");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var datos = JsonDocument.Parse(jsonString);

                ViewBag.PlanesList = JsonSerializer.Deserialize<List<Plan>>(datos.RootElement.GetProperty("planes").GetRawText(), _jsonOptions);
                ViewBag.PromocionesList = JsonSerializer.Deserialize<List<Promocion>>(datos.RootElement.GetProperty("promociones").GetRawText(), _jsonOptions);
            }
            else
            {
                ViewBag.PlanesList = new List<Plan>();
                ViewBag.PromocionesList = new List<Promocion>();
            }
        }
    }
}