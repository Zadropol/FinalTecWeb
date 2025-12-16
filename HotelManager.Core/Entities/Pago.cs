using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManager.Core.Entities
{
    [Table("Pago")]
    public class Pago
    {
        [Key]
        public int IdPago { get; set; }

        public int IdReserva { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        public DateTime FechaPago { get; set; } = DateTime.Now;

        [Required]
        public string MetodoPago { get; set; } = null!; // Efectivo, Tarjeta Credito, Tarjeta Debito, Transferencia, QR

        [Required]
        public string EstadoPago { get; set; } = "Pendiente"; // Pendiente, Procesando, Completado, Rechazado, Reembolsado

        [StringLength(100)]
        public string? Referencia { get; set; }

        // Navegación
        [ForeignKey("IdReserva")]
        public virtual Reserva Reserva { get; set; } = null!;
    }
}
