using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManager.Core.Entities
{
    [Table("Huesped")]
    public class Huesped : BaseEntity
    {
        [Key]
        public int IdHuesped {
            get => Id;
            set => Id = value;

        }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [Required]
        [StringLength(50)]
        public string Documento { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string TipoDocumento { get; set; } = null!;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Navegación
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
