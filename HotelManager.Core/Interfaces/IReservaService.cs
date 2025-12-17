using HotelManager.Core.CustomEntities;
using HotelManager.Core.Entities;
using HotelManager.Core.QueryFilters;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Interfaces
{
    public interface IReservaService
    {
        Task<PagedList<Reserva>> GetReservasAsync(ReservaQueryFilter filters);
        Task<Reserva> GetReservaByIdAsync(int id);
        Task InsertReserva(Reserva reserva);
        Task UpdateReserva(Reserva reserva);
        Task DeleteReserva(int id);
    }
}
