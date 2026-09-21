using Microsoft.AspNetCore.Mvc;
using GarrasFitnessApp.Models; // Ya no lleva .ViewModels al final

namespace GarrasFitnessApp.Controllers
{
    public class CajaController : Controller
    {
        public IActionResult Index()
        {
            var mockData = new ArqueoCajaViewModel();
            return View(mockData);
        }

        public IActionResult ExportarPDF()
        {
            return Content("El reporte PDF se generará aquí.");
        }
    }
}