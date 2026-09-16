using System.ComponentModel.DataAnnotations;

namespace GarrasFitnessApp.Models
{
    public class Promocion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la promoción es obligatorio.")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty; 
        [Required]
        public decimal Descuento { get; set; } 

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }
    }
}