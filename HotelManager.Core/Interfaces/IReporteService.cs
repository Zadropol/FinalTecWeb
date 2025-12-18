using HotelManagement.Core.DTOs;
using HotelManager.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Interfaces
{
    public interface IReporteService
    {
        Task<ReporteOcupacionDto> GenerarReporteOcupacion(DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<ReporteFinancieroDto> GenerarReporteFinanciero(DateTime fechaInicio, DateTime fechaFin);
        Task<ReporteHuespedesDto> GenerarReporteHuespedes();
    }
}
