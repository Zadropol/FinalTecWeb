using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManager.Core.Entities
{
    [Table("Reserva")]
    public class Reserva : BaseEntity
    {
        [Key]
        [Column("IdReserva")]
        public int IdReserva {
            get => Id;
            set => Id = value;
        
        }

        public int IdHuesped { get; set; }

        public int IdHabitacion { get; set; }

        public int IdTipoHabitacion { get; set; }

        [Required]
        [StringLength(20)]
        public string CodigoReserva { get; set; } = null!;

        public DateTime FechaReserva { get; set; } = DateTime.Now;

        [Column(TypeName = "date")]
        public DateTime FechaCheckIn { get; set; }

        [Column(TypeName = "date")]
        public DateTime FechaCheckOut { get; set; }

        public DateTime? FechaCheckInReal { get; set; }

        public DateTime? FechaCheckOutReal { get; set; }

        public int NumeroNoches { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal MontoTotal { get; set; }

        [Required]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmada, En curso, Completada, Cancelada

        [Column(TypeName = "text")]
        public string? Observaciones { get; set; }

        // Navegación
        [ForeignKey("IdHuesped")]
        public virtual Huesped? Huesped { get; set; } = null!;

        [ForeignKey("IdHabitacion")]
        public virtual Habitacion? Habitacion { get; set; } = null!;

        [ForeignKey("IdTipoHabitacion")]
        public virtual TipoHabitacion? TipoHabitacion { get; set; } = null!;

        public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public virtual ICollection<ReservaServicio> ReservaServicios { get; set; } = new List<ReservaServicio>();
    }
}
