using System.ComponentModel.DataAnnotations;

namespace GarrasFitnessAPI.Models
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

        // ============================================================
        // NUEVOS: campos que faltaban
        // ============================================================

        /// <summary>
        /// Ruta relativa de la imagen dentro de wwwroot.
        /// Ej: "/img/promociones/abc-123.jpg"
        /// </summary>
        [MaxLength(255)]
        public string? ImagenUrl { get; set; }

        /// <summary>
        /// Descripción corta de la promoción (opcional).
        /// </summary>
        [MaxLength(500)]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Indica si la promoción está activa.
        /// </summary>
        public bool Activa { get; set; } = true;
    }
}