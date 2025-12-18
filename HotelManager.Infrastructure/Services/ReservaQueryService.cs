using Dapper;
using HotelManager.Core.DTOs;
using HotelManager.Core.Interfaces;
using HotelManager.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HotelManager.Infrastructure.Services
{
    public class ReservaQueryService : IReservaQueryService
    {
        private readonly DapperContext _context;

        public ReservaQueryService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HabitacionDisponibleDto>> GetHabitacionesDisponibles(
            DateTime fechaEntrada,
            DateTime fechaSalida,
            int? idTipoHabitacion)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("fecha_entrada", fechaEntrada, DbType.Date);
            parameters.Add("fecha_salida", fechaSalida, DbType.Date);
            parameters.Add("tipo_habitacion", idTipoHabitacion, DbType.Int32);

            return await connection.QueryAsync<HabitacionDisponibleDto>(
                "sp_habitaciones_disponibles",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
