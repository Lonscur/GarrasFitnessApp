using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarrasFitnessAPI.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SocioId { get; set; }
        [ForeignKey("SocioId")]
        public virtual Socio Socio { get; set; } = null!;

        [Required]
        public int PlanId { get; set; }
        [ForeignKey("PlanId")]
        public virtual Plan Plan { get; set; } = null!;

        public int? PromocionId { get; set; }
        [ForeignKey("PromocionId")]
        public virtual Promocion? Promocion { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;

        [Required]
        public decimal MontoTotal { get; set; }

        [Required]
        [MaxLength(20)]
        public string MetodoPago { get; set; } = string.Empty;

        [Required]
        public DateTime FechaPago { get; set; }
    }
}