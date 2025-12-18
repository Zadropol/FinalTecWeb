using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.DTOs
{
    public class ReporteOcupacionDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TotalHabitaciones { get; set; }
        public int HabitacionesOcupadas { get; set; }
        public int HabitacionesDisponibles { get; set; }
        public decimal PorcentajeOcupacion { get; set; }
        public List<OcupacionPorTipoDto> OcupacionPorTipo { get; set; } = new();
    }

    public class OcupacionPorTipoDto
    {
        public string TipoHabitacion { get; set; }
        public int TotalHabitaciones { get; set; }
        public int Ocupadas { get; set; }
        public decimal PorcentajeOcupacion { get; set; }
    }
}

// ============================================
// HotelManagement.Core/DTOs/ReporteFinancieroDto.cs
// ============================================
namespace HotelManagement.Core.DTOs
{
    public class ReporteFinancieroDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TotalReservas { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal IngresosReservas { get; set; }
        public decimal IngresosServicios { get; set; }
        public decimal PromedioIngresosPorReserva { get; set; }
        public List<IngresosPorTipoHabitacionDto> IngresosPorTipo { get; set; } = new();
        public List<IngresosPorDiaDto> IngresosPorDia { get; set; } = new();
    }

    public class IngresosPorTipoHabitacionDto
    {
        public string TipoHabitacion { get; set; }
        public int CantidadReservas { get; set; }
        public decimal Ingresos { get; set; }
    }

    public class IngresosPorDiaDto
    {
        public DateTime Fecha { get; set; }
        public int CantidadReservas { get; set; }
        public decimal Ingresos { get; set; }
    }
}

// ============================================
// HotelManagement.Core/DTOs/ReporteHuespedesDto.cs
// ============================================
namespace HotelManagement.Core.DTOs
{
    public class ReporteHuespedesDto
    {
        public int TotalHuespedes { get; set; }
        public int HuespedesConReservas { get; set; }
        public List<HuespedFrecuenteDto> HuespedesFrecuentes { get; set; } = new();
        public List<EstadisticaReservasDto> EstadisticasReservas { get; set; } = new();
    }

    public class HuespedFrecuenteDto
    {
        public int IdHuesped { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public int TotalReservas { get; set; }
        public decimal TotalGastado { get; set; }
        public DateTime? UltimaReserva { get; set; }
    }

    public class EstadisticaReservasDto
    {
        public string Estado { get; set; }
        public int Cantidad { get; set; }
        public decimal Porcentaje { get; set; }
    }
}
