using System.ComponentModel.DataAnnotations;

namespace GarrasFitnessAPI.Models
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