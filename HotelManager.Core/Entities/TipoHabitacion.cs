using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManager.Core.Entities
{
    [Table("TipoHabitacion")]
    public class TipoHabitacion
    {
        [Key]
        public int IdTipoHabitacion { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;

        [Column(TypeName = "text")]
        public string? Descripcion { get; set; }

        public int Capacidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioPorNoche { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
