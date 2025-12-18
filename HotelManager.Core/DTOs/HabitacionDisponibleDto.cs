using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.DTOs
{
    public class HabitacionDisponibleDto
    {
        public int IdHabitacion { get; set; }
        public string Numero { get; set; } = null!;
        public int Piso { get; set; }
        public string TipoHabitacion { get; set; } = null!;
        public int Capacidad { get; set; }
        public decimal PrecioPorNoche { get; set; }
        public string? Descripcion { get; set; }
    }
}
