using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManager.Core.Entities
{
    [Table("Habitacion")]
    public class Habitacion : BaseEntity
    {
        [Key]
        public int IdHabitacion {
            get => Id;
            set => Id = value;
        }

        public int IdTipoHabitacion { get; set; }

        [Required]
        [StringLength(10)]
        public string Numero { get; set; } = null!;

        public int Piso { get; set; }

        [Required]
        public string Estado { get; set; } = null!;

        [Column(TypeName = "text")]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        [ForeignKey("IdTipoHabitacion")]
        public virtual TipoHabitacion TipoHabitacion { get; set; } = null!;
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}