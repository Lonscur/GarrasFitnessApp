using System.ComponentModel.DataAnnotations;

namespace GarrasFitnessApp.Models
{
    public class Plan
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del plan es obligatorio.")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty; 

        [Required]
        public decimal PrecioBase { get; set; } 
        [Required]
        public int DuracionDias { get; set; } 
    }
}