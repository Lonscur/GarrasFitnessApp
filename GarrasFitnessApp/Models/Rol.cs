using System.ComponentModel.DataAnnotations;

namespace GarrasFitnessApp.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }
    }
}