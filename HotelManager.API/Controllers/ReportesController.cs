using HotelManagement.Core.DTOs;
using HotelManager.API.Responses;
using HotelManager.Core.CustomEntities;
using HotelManager.Core.DTOs;
using HotelManager.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HotelManager.API.Controllers
{
    /// <summary>
    /// Controlador para la generación de reportes administrativos y estadísticos
    /// </summary>
    /// <remarks>
    /// Este controlador proporciona endpoints para generar diversos tipos de reportes:
    /// - Reportes de ocupación hotelera
    /// - Reportes financieros e ingresos
    /// - Reportes de huéspedes y estadísticas
    /// - Dashboard ejecutivo con métricas consolidadas
    /// 
    /// Los reportes son esenciales para la toma de decisiones gerenciales
    /// y análisis del desempeño del hotel.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        /// <summary>
        /// Constructor del controlador de reportes
        /// </summary>
        /// <param name="reporteService">Servicio de generación de reportes</param>
        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        /// <summary>
        /// Genera un reporte de ocupación de habitaciones
        /// </summary>
        /// <remarks>
        /// Genera un análisis detallado de la ocupación hotelera que incluye:
        /// 
        /// **Métricas Generales:**
        /// - Porcentaje de ocupación total
        /// - Número de habitaciones ocupadas
        /// - Número de habitaciones disponibles
        /// - Total de habitaciones del hotel
        /// 
        /// **Análisis por Tipo de Habitación:**
        /// - Ocupación por cada categoría (Simple, Doble, Suite, etc.)
        /// - Porcentaje de ocupación por tipo
        /// - Habitaciones ocupadas vs. disponibles por tipo
        /// 
        /// **Período de Análisis:**
        /// - Si no se especifican fechas, usa el mes actual
        /// - Permite análisis de períodos personalizados
        /// 
        /// Útil para:
        /// - Análisis de rendimiento
        /// - Planificación de recursos
        /// - Identificar tipos de habitación más demandados
        /// - Optimización de tarifas
        /// 
        /// Ejemplo de requests:
        /// 
        ///     GET /api/Reportes/ocupacion
        ///     GET /api/Reportes/ocupacion?fechaInicio=2024-12-01&amp;fechaFin=2024-12-31
        /// 
        /// </remarks>
        /// <param name="fechaInicio">Fecha de inicio del reporte (opcional, por defecto inicio del mes actual)</param>
        /// <param name="fechaFin">Fecha de fin del reporte (opcional, por defecto fin del mes actual)</param>
        /// <response code="200">Reporte de ocupación generado exitosamente</response>
        /// <response code="400">Si las fechas son inválidas</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Reporte completo de ocupación con métricas y análisis por tipo</returns>
        [HttpGet("ocupacion")]
        [ProducesResponseType(typeof(ApiResponse<ReporteOcupacionDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GenerarReporteOcupacion(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var reporte = await _reporteService.GenerarReporteOcupacion(fechaInicio, fechaFin);

            return Ok(new ApiResponse<ReporteOcupacionDto>(
                $"Reporte de ocupación generado: {reporte.PorcentajeOcupacion:F2}% de ocupación",
                reporte
            ));
        }

        /// <summary>
        /// Genera un reporte financiero detallado por período
        /// </summary>
        /// <remarks>
        /// Genera un análisis financiero completo que incluye:
        /// 
        /// **Ingresos Totales:**
        /// - Suma de todos los ingresos del período
        /// - Ingresos por reservas de habitaciones
        /// - Ingresos por servicios adicionales
        /// 
        /// **Análisis de Reservas:**
        /// - Número total de reservas en el período
        /// - Promedio de ingresos por reserva
        /// - Distribución de reservas por tipo de habitación
        /// 
        /// **Desglose por Tipo de Habitación:**
        /// - Ingresos generados por cada categoría
        /// - Cantidad de reservas por tipo
        /// - Contribución porcentual al ingreso total
        /// 
        /// **Análisis Temporal:**
        /// - Ingresos diarios en el período
        /// - Tendencias de reservas por día
        /// - Identificación de días pico
        /// 
        /// Este reporte es fundamental para:
        /// - Análisis de rentabilidad
        /// - Proyecciones financieras
        /// - Identificación de oportunidades de crecimiento
        /// - Evaluación de estrategias de precios
        /// 
        /// Las fechas son obligatorias para definir el período de análisis.
        /// 
        /// Ejemplo de request:
        /// 
        ///     GET /api/Reportes/financiero?fechaInicio=2024-12-01&amp;fechaFin=2024-12-31
        /// 
        /// </remarks>
        /// <param name="fechaInicio">Fecha de inicio del período a analizar (requerido)</param>
        /// <param name="fechaFin">Fecha de fin del período a analizar (requerido)</param>
        /// <response code="200">Reporte financiero generado exitosamente</response>
        /// <response code="400">Si las fechas son inválidas o fecha inicio >= fecha fin</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Reporte financiero completo con análisis de ingresos y tendencias</returns>
        [HttpGet("financiero")]
        [ProducesResponseType(typeof(ApiResponse<ReporteFinancieroDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GenerarReporteFinanciero(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            if (fechaInicio >= fechaFin)
            {
                return BadRequest(new ApiResponse<object>(
                    "La fecha de inicio debe ser anterior a la fecha de fin",
                    null
                ));
            }

            var reporte = await _reporteService.GenerarReporteFinanciero(fechaInicio, fechaFin);

            return Ok(new ApiResponse<ReporteFinancieroDto>(
                $"Reporte financiero generado: {reporte.IngresosTotales:C} en ingresos totales",
                reporte
            ));
        }

        /// <summary>
        /// Genera un reporte completo de huéspedes y estadísticas
        /// </summary>
        /// <remarks>
        /// Genera un análisis detallado de los huéspedes del hotel que incluye:
        /// 
        /// **Métricas de Huéspedes:**
        /// - Total de huéspedes registrados en el sistema
        /// - Huéspedes con al menos una reserva
        /// - Tasa de conversión de registros a reservas
        /// 
        /// **Huéspedes Frecuentes (Top 10):**
        /// - Ranking de clientes por número de reservas
        /// - Gasto total por huésped
        /// - Fecha de última visita
        /// - Información de contacto
        /// 
        /// **Estadísticas de Reservas por Estado:**
        /// - Distribución de reservas: Pendiente, Confirmada, En curso, Completada, Cancelada
        /// - Porcentaje de cada estado
        /// - Cantidad absoluta por estado
        /// 
        /// Útil para:
        /// - Programas de fidelización
        /// - Marketing dirigido a clientes frecuentes
        /// - Análisis de comportamiento de clientes
        /// - Identificación de VIPs
        /// - Estrategias de retención
        /// 
        /// Este reporte analiza todos los datos históricos del sistema.
        /// 
        /// Ejemplo de request:
        /// 
        ///     GET /api/Reportes/huespedes
        /// 
        /// </remarks>
        /// <response code="200">Reporte de huéspedes generado exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Reporte completo de huéspedes con ranking y estadísticas</returns>
        [HttpGet("huespedes")]
        [ProducesResponseType(typeof(ApiResponse<ReporteHuespedesDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GenerarReporteHuespedes()
        {
            var reporte = await _reporteService.GenerarReporteHuespedes();

            return Ok(new ApiResponse<ReporteHuespedesDto>(
                $"Reporte de huéspedes generado: {reporte.TotalHuespedes} huéspedes registrados",
                reporte
            ));
        }

        /// <summary>
        /// Genera un dashboard ejecutivo con todas las métricas consolidadas
        /// </summary>
        /// <remarks>
        /// Genera un reporte ejecutivo consolidado que incluye:
        /// 
        /// **1. Información del Período:**
        /// - Fechas de análisis (mes actual por defecto)
        /// - Mes y año en formato legible
        /// 
        /// **2. Métricas de Ocupación:**
        /// - Porcentaje de ocupación del mes
        /// - Habitaciones ocupadas vs. disponibles
        /// - Total de habitaciones del hotel
        /// - Tendencias de ocupación
        /// 
        /// **3. Resumen Financiero:**
        /// - Ingresos totales del período
        /// - Desglose: ingresos por reservas e ingresos por servicios
        /// - Total de reservas generadas
        /// - Promedio de ingresos por reserva
        /// - Análisis de rentabilidad
        /// 
        /// **4. Estadísticas de Huéspedes:**
        /// - Total de huéspedes en el sistema
        /// - Huéspedes con reservas activas
        /// - Top 5 huéspedes frecuentes del mes
        /// - Información de contacto de VIPs
        /// 
        /// **5. Distribución de Reservas:**
        /// - Reservas por estado (Pendiente, Confirmada, etc.)
        /// - Porcentajes de conversión
        /// - Análisis de cancelaciones
        /// 
        /// Este dashboard proporciona una vista de 360° del negocio hotelero,
        /// ideal para reuniones gerenciales y toma de decisiones estratégicas.
        /// 
        /// Ejemplo de request:
        /// 
        ///     GET /api/Reportes/dashboard
        /// 
        /// </remarks>
        /// <response code="200">Dashboard generado exitosamente con todas las métricas</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Dashboard ejecutivo completo con métricas consolidadas del mes actual</returns>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GenerarReporteDashboard()
        {
            // Reporte del mes actual
            var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var finMes = inicioMes.AddMonths(1).AddDays(-1);

            var reporteOcupacion = await _reporteService.GenerarReporteOcupacion(inicioMes, finMes);
            var reporteFinanciero = await _reporteService.GenerarReporteFinanciero(inicioMes, finMes);
            var reporteHuespedes = await _reporteService.GenerarReporteHuespedes();

            var dashboard = new
            {
                Periodo = new
                {
                    Inicio = inicioMes,
                    Fin = finMes,
                    MesAnio = inicioMes.ToString("MMMM yyyy")
                },
                Ocupacion = new
                {
                    reporteOcupacion.PorcentajeOcupacion,
                    reporteOcupacion.HabitacionesOcupadas,
                    reporteOcupacion.HabitacionesDisponibles,
                    reporteOcupacion.TotalHabitaciones
                },
                Financiero = new
                {
                    reporteFinanciero.IngresosTotales,
                    reporteFinanciero.IngresosReservas,
                    reporteFinanciero.IngresosServicios,
                    reporteFinanciero.TotalReservas,
                    reporteFinanciero.PromedioIngresosPorReserva
                },
                Huespedes = new
                {
                    reporteHuespedes.TotalHuespedes,
                    reporteHuespedes.HuespedesConReservas,
                    Top5HuespedesFrecuentes = reporteHuespedes.HuespedesFrecuentes.Take(5)
                },
                EstadisticasReservas = reporteHuespedes.EstadisticasReservas
            };

            return Ok(new ApiResponse<object>(
                $"Dashboard generado para {inicioMes:MMMM yyyy}",
                dashboard
            ));
        }
    }
}
