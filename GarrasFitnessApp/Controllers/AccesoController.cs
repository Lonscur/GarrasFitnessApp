using Microsoft.AspNetCore.Mvc;

namespace GarrasFitnessApp.Controllers
{
    public class AccesoController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccesoController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7286/");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Verificar(string ci)
        {
            var response = await _httpClient.GetAsync($"api/AccesoApi/Verificar/{ci}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Content(result, "application/json");
            }

            return NotFound();
        }
    }
}