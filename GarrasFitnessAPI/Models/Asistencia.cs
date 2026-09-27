using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarrasFitnessAPI.Models
{
    public class Asistencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SocioId { get; set; }

        public DateTime FechaAsistencia { get; set; } = DateTime.Now;

        [ForeignKey("SocioId")]
        public Socio? Socio { get; set; }
    }
}