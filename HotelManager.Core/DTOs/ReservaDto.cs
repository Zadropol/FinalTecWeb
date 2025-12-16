using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.DTOs
{
    public class ReservaDto
    {
        public int IdReserva { get; set; }
        public int IdHuesped { get; set; }
        public int IdHabitacion { get; set; }
        public int IdTipoHabitacion { get; set; }
        public string CodigoReserva { get; set; } = null!;
        public DateTime FechaReserva { get; set; }
        public DateTime FechaCheckIn { get; set; }
        public DateTime FechaCheckOut { get; set; }
        public int NumeroNoches { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; } = null!;
        public string? Observaciones { get; set; }

        // Información relacionada
        public string? NombreHuesped { get; set; }
        public string? NumeroHabitacion { get; set; }
        public string? TipoHabitacion { get; set; }
    }
}
