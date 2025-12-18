using HotelManagement.Core.DTOs;
using HotelManager.Core.DTOs;
using HotelManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReporteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReporteOcupacionDto> GenerarReporteOcupacion(
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            // Si no se especifican fechas, usar el mes actual
            fechaInicio ??= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            fechaFin ??= fechaInicio.Value.AddMonths(1).AddDays(-1);

            var todasHabitaciones = _unitOfWork.HabitacionRepository.GetAll()
                .Where(h => h.Activo)
                .ToList();

            var totalHabitaciones = todasHabitaciones.Count();

            // Obtener reservas en el período usando el método específico
            var reservasEnPeriodo = await _unitOfWork.ReservaRepository
                .GetReservasByFechasAsync(fechaInicio.Value, fechaFin.Value);

            reservasEnPeriodo = reservasEnPeriodo
                .Where(r => r.Estado == "En curso" || r.Estado == "Confirmada")
                .ToList();

            var habitacionesOcupadasIds = reservasEnPeriodo
                .Select(r => r.IdHabitacion)
                .Distinct()
                .Count();

            var habitacionesDisponibles = totalHabitaciones - habitacionesOcupadasIds;
            var porcentajeOcupacion = totalHabitaciones > 0
                ? (decimal)habitacionesOcupadasIds / totalHabitaciones * 100
                : 0;

            // Ocupación por tipo de habitación
            var tiposHabitacion = _unitOfWork.TipoHabitacionRepository.GetAll()
                .Where(t => t.Activo)
                .ToList();

            var ocupacionPorTipo = new List<OcupacionPorTipoDto>();
            foreach (var tipo in tiposHabitacion)
            {
                var habitacionesTipo = todasHabitaciones
                    .Where(h => h.IdTipoHabitacion == tipo.IdTipoHabitacion)
                    .ToList();

                var totalTipo = habitacionesTipo.Count;
                var ocupadasTipo = habitacionesTipo
                    .Count(h => reservasEnPeriodo.Any(r => r.IdHabitacion == h.IdHabitacion));

                var porcentajeTipo = totalTipo > 0
                    ? (decimal)ocupadasTipo / totalTipo * 100
                    : 0;

                ocupacionPorTipo.Add(new OcupacionPorTipoDto
                {
                    TipoHabitacion = tipo.Nombre,
                    TotalHabitaciones = totalTipo,
                    Ocupadas = ocupadasTipo,
                    PorcentajeOcupacion = Math.Round(porcentajeTipo, 2)
                });
            }

            return new ReporteOcupacionDto
            {
                FechaInicio = fechaInicio.Value,
                FechaFin = fechaFin.Value,
                TotalHabitaciones = totalHabitaciones,
                HabitacionesOcupadas = habitacionesOcupadasIds,
                HabitacionesDisponibles = habitacionesDisponibles,
                PorcentajeOcupacion = Math.Round(porcentajeOcupacion, 2),
                OcupacionPorTipo = ocupacionPorTipo
            };
        }

        public async Task<ReporteFinancieroDto> GenerarReporteFinanciero(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            // Ajustar las fechas para incluir todo el día
            var fechaInicioAjustada = fechaInicio.Date;
            var fechaFinAjustada = fechaFin.Date.AddDays(1).AddSeconds(-1);

            var reservas = await _unitOfWork.ReservaRepository.GetAllReservasAsync();
            var reservasEnPeriodo = reservas.Where(r =>
                r.FechaReserva.Date >= fechaInicioAjustada &&
                r.FechaReserva.Date <= fechaFinAjustada &&
                r.Estado != "Cancelada"
            ).ToList();

            var totalReservas = reservasEnPeriodo.Count;
            var ingresosReservas = reservasEnPeriodo.Sum(r => r.MontoTotal);

            // Calcular ingresos por servicios
            var todosServicios = _unitOfWork.ServicioRepository.GetAll();
            var reservaServicios = todosServicios
                .SelectMany(s => s.ReservaServicios)
                .Where(rs => reservasEnPeriodo.Any(r => r.IdReserva == rs.IdReserva));

            var ingresosServicios = reservaServicios.Sum(rs => rs.SubTotal);
            var ingresosTotales = ingresosReservas + ingresosServicios;
            var promedioIngresos = totalReservas > 0
                ? ingresosTotales / totalReservas
                : 0;

            // Ingresos por tipo de habitación
            var ingresosPorTipo = reservasEnPeriodo
                .GroupBy(r => r.TipoHabitacion.Nombre)
                .Select(g => new IngresosPorTipoHabitacionDto
                {
                    TipoHabitacion = g.Key,
                    CantidadReservas = g.Count(),
                    Ingresos = g.Sum(r => r.MontoTotal)
                })
                .OrderByDescending(i => i.Ingresos)
                .ToList();

            // Ingresos por día
            var ingresosPorDia = reservasEnPeriodo
                .GroupBy(r => r.FechaReserva.Date)
                .Select(g => new IngresosPorDiaDto
                {
                    Fecha = g.Key,
                    CantidadReservas = g.Count(),
                    Ingresos = g.Sum(r => r.MontoTotal)
                })
                .OrderBy(i => i.Fecha)
                .ToList();

            return new ReporteFinancieroDto
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TotalReservas = totalReservas,
                IngresosTotales = Math.Round(ingresosTotales, 2),
                IngresosReservas = Math.Round(ingresosReservas, 2),
                IngresosServicios = Math.Round(ingresosServicios, 2),
                PromedioIngresosPorReserva = Math.Round(promedioIngresos, 2),
                IngresosPorTipo = ingresosPorTipo,
                IngresosPorDia = ingresosPorDia
            };
        }

        public async Task<ReporteHuespedesDto> GenerarReporteHuespedes()
        {
            var todosHuespedes = _unitOfWork.HuespedRepository.GetAll();
            var totalHuespedes = todosHuespedes.Count();

            var reservas = await _unitOfWork.ReservaRepository.GetAllReservasAsync();

            // Huéspedes con al menos una reserva
            var huespedesConReservas = reservas
                .Select(r => r.IdHuesped)
                .Distinct()
                .Count();

            // Huéspedes frecuentes (top 10)
            var huespedesFrecuentes = reservas
                .GroupBy(r => r.Huesped)
                .Select(g => new HuespedFrecuenteDto
                {
                    IdHuesped = g.Key.IdHuesped,
                    NombreCompleto = $"{g.Key.Nombre} {g.Key.Apellido}",
                    Email = g.Key.Email,
                    TotalReservas = g.Count(),
                    TotalGastado = g.Sum(r => r.MontoTotal),
                    UltimaReserva = g.Max(r => r.FechaReserva)
                })
                .OrderByDescending(h => h.TotalReservas)
                .Take(10)
                .ToList();

            // Estadísticas de reservas por estado
            var totalReservas = reservas.Count();
            var estadisticasReservas = reservas
                .GroupBy(r => r.Estado)
                .Select(g => new EstadisticaReservasDto
                {
                    Estado = g.Key,
                    Cantidad = g.Count(),
                    Porcentaje = totalReservas > 0
                        ? Math.Round((decimal)g.Count() / totalReservas * 100, 2)
                        : 0
                })
                .OrderByDescending(e => e.Cantidad)
                .ToList();

            return new ReporteHuespedesDto
            {
                TotalHuespedes = totalHuespedes,
                HuespedesConReservas = huespedesConReservas,
                HuespedesFrecuentes = huespedesFrecuentes,
                EstadisticasReservas = estadisticasReservas
            };
        }
    }
}
