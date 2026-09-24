using Microsoft.AspNetCore.Mvc;
using GarrasFitnessApp.Models;
using System.Text.Json;

namespace GarrasFitnessApp.Controllers
{
    public class CajaController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public CajaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7286/");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index()
        {
            var pagosHoy = await ObtenerPagosHoyAsync();
            CalcularTotales(pagosHoy);
            return View(pagosHoy);
        }

        public async Task<IActionResult> ReportePdf()
        {
            var pagosHoy = await ObtenerPagosHoyAsync();
            CalcularTotales(pagosHoy);
            return View(pagosHoy);
        }

        private async Task<List<Pago>> ObtenerPagosHoyAsync()
        {
            var response = await _httpClient.GetAsync("api/CajaApi/PagosHoy");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Pago>>(jsonString, _jsonOptions) ?? new List<Pago>();
            }
            return new List<Pago>();
        }

        private void CalcularTotales(List<Pago> pagos)
        {
            decimal totalEfectivo = pagos.Where(p => p.MetodoPago == "Efectivo").Sum(p => p.MontoTotal);
            decimal totalQR = pagos.Where(p => p.MetodoPago == "QR").Sum(p => p.MontoTotal);

            ViewBag.TotalEfectivo = totalEfectivo;
            ViewBag.TotalQR = totalQR;
            ViewBag.TotalGeneral = totalEfectivo + totalQR;
            ViewBag.FechaArqueo = DateTime.Today.ToString("dd/MM/yyyy");
        }
    }
}