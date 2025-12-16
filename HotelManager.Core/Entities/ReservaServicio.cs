using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManager.Core.Entities
{
    [Table("ReservaServicio")]
    public class ReservaServicio  : BaseEntity
    {
        [Key]
        public int IdReservaServicio {
            get => Id;
            set => Id = value;
        }

        public int IdReserva { get; set; }

        public int IdServicio { get; set; }

        public int Cantidad { get; set; } = 1;

        public DateTime FechaConsumo { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        // Navegación
        [ForeignKey("IdReserva")]
        public virtual Reserva Reserva { get; set; } = null!;

        [ForeignKey("IdServicio")]
        public virtual Servicio Servicio { get; set; } = null!;
    }
}
