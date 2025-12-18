using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManager.Core.Entities
{
    /// <summary>
    /// Representa una reserva de habitación en el sistema hotelero
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena toda la información relacionada con las reservas
    /// de habitaciones, incluyendo fechas, montos, estados y relaciones con
    /// huéspedes y habitaciones. Es el núcleo del sistema de gestión hotelera.
    /// </remarks>
    [Table("Reserva")]
    public class Reserva : BaseEntity
    {
        /// <summary>
        /// Identificador único de la reserva
        /// </summary>
        /// <example>1</example>
        [Key]
        [Column("IdReserva")]
        public int IdReserva
        {
            get => Id;
            set => Id = value;
        }

        /// <summary>
        /// ID del huésped que realiza la reserva
        /// </summary>
        /// <example>5</example>
        public int IdHuesped { get; set; }

        /// <summary>
        /// ID de la habitación física reservada
        /// </summary>
        /// <example>10</example>
        public int IdHabitacion { get; set; }

        /// <summary>
        /// ID del tipo de habitación (Simple, Doble, Suite, etc.)
        /// </summary>
        /// <example>2</example>
        public int IdTipoHabitacion { get; set; }

        /// <summary>
        /// Código único de la reserva para identificación rápida
        /// </summary>
        /// <remarks>
        /// Formato: RES-YYYY-#### (ej: RES-2024-0001)
        /// Se genera automáticamente al crear la reserva
        /// </remarks>
        /// <example>RES-2024-0001</example>
        [Required]
        [StringLength(20)]
        public string CodigoReserva { get; set; } = null!;

        /// <summary>
        /// Fecha y hora en que se realizó la reserva
        /// </summary>
        /// <example>2024-12-15T14:30:00</example>
        public DateTime FechaReserva { get; set; } = DateTime.Now;

        /// <summary>
        /// Fecha planificada de check-in (entrada)
        /// </summary>
        /// <example>2024-12-20</example>
        [Column(TypeName = "date")]
        public DateTime FechaCheckIn { get; set; }

        /// <summary>
        /// Fecha planificada de check-out (salida)
        /// </summary>
        /// <example>2024-12-23</example>
        [Column(TypeName = "date")]
        public DateTime FechaCheckOut { get; set; }

        /// <summary>
        /// Fecha y hora real en que se realizó el check-in
        /// </summary>
        /// <remarks>
        /// Se registra cuando el huésped se presenta en recepción.
        /// Puede ser null si aún no se ha realizado el check-in.
        /// </remarks>
        /// <example>2024-12-20T15:45:00</example>
        public DateTime? FechaCheckInReal { get; set; }

        /// <summary>
        /// Fecha y hora real en que se realizó el check-out
        /// </summary>
        /// <remarks>
        /// Se registra cuando el huésped entrega la habitación.
        /// Puede ser null si aún no se ha realizado el check-out.
        /// </remarks>
        /// <example>2024-12-23T11:30:00</example>
        public DateTime? FechaCheckOutReal { get; set; }

        /// <summary>
        /// Número total de noches de la reserva
        /// </summary>
        /// <remarks>
        /// Se calcula automáticamente como la diferencia entre
        /// FechaCheckOut y FechaCheckIn en días.
        /// </remarks>
        /// <example>3</example>
        public int NumeroNoches { get; set; }

        /// <summary>
        /// Monto total de la reserva en moneda local
        /// </summary>
        /// <remarks>
        /// Se calcula como: PrecioPorNoche * NumeroNoches
        /// No incluye servicios adicionales (minibar, spa, etc.)
        /// </remarks>
        /// <example>750.00</example>
        [Column(TypeName = "decimal(10,2)")]
        public decimal MontoTotal { get; set; }

        /// <summary>
        /// Estado actual de la reserva
        /// </summary>
        /// <remarks>
        /// Estados posibles:
        /// - Pendiente: Reserva creada, esperando confirmación
        /// - Confirmada: Reserva confirmada, esperando check-in
        /// - En curso: Huésped ya hizo check-in, hospedado actualmente
        /// - Completada: Check-out realizado, reserva finalizada
        /// - Cancelada: Reserva cancelada por el hotel o huésped
        /// </remarks>
        /// <example>Confirmada</example>
        [Required]
        public string Estado { get; set; } = "Pendiente";

        /// <summary>
        /// Observaciones o notas adicionales sobre la reserva
        /// </summary>
        /// <remarks>
        /// Campo opcional para registrar preferencias del huésped,
        /// solicitudes especiales, o notas internas del hotel.
        /// </remarks>
        /// <example>Cliente prefiere piso alto, llegada tardía confirmada</example>
        [Column(TypeName = "text")]
        public string? Observaciones { get; set; }

        // ======================================
        // PROPIEDADES DE NAVEGACIÓN
        // ======================================

        /// <summary>
        /// Información del huésped asociado a esta reserva
        /// </summary>
        [ForeignKey("IdHuesped")]
        public virtual Huesped? Huesped { get; set; } = null!;

        /// <summary>
        /// Información de la habitación física reservada
        /// </summary>
        [ForeignKey("IdHabitacion")]
        public virtual Habitacion? Habitacion { get; set; } = null!;

        /// <summary>
        /// Información del tipo de habitación reservado
        /// </summary>
        [ForeignKey("IdTipoHabitacion")]
        public virtual TipoHabitacion? TipoHabitacion { get; set; } = null!;

        /// <summary>
        /// Colección de pagos asociados a esta reserva
        /// </summary>
        /// <remarks>
        /// Una reserva puede tener múltiples pagos (anticipo, saldo, etc.)
        /// </remarks>
        public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

        /// <summary>
        /// Colección de servicios adicionales consumidos durante la reserva
        /// </summary>
        /// <remarks>
        /// Incluye servicios como spa, minibar, room service, etc.
        /// </remarks>
        public virtual ICollection<ReservaServicio> ReservaServicios { get; set; } = new List<ReservaServicio>();
    }
}
