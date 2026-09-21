namespace GarrasFitnessApp.Models
{
    public class ArqueoCajaViewModel
    {
        public decimal TotalEfectivo { get; set; }
        public decimal TotalQR { get; set; }
        public decimal TotalGeneral => TotalEfectivo + TotalQR;
        public DateTime FechaConsulta { get; set; } = DateTime.Now;
    }
}