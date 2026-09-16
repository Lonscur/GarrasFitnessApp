using System;
using System.ComponentModel.DataAnnotations;

namespace GarrasFitnessApp.Models
{
    public class Socio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string NombreCompleto { get; set; }

        [Required]
        [MaxLength(20)]
        public string CI { get; set; }

        [Required]
        [MaxLength(20)]
        public string Telefono { get; set; }

        [Required]
        [MaxLength(100)]
        public string Correo { get; set; }

        [Required]
        public DateTime FechaVencimiento { get; set; }
    }
}