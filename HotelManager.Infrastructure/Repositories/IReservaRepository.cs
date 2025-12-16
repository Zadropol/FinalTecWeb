using HotelManager.Core.Entities;
using HotelManager.Core.Interfaces;
using HotelManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Infrastructure.Repositories
{
    public class ReservaRepository : BaseRepository<Reserva>, IReservaRepository
    {
        public ReservaRepository(HotelContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Reserva>> GetAllReservasAsync()
        {
            return await _entities
                .Include(r => r.Huesped)
                .Include(r => r.Habitacion)
                .Include(r => r.TipoHabitacion)
                .ToListAsync();
        }

        public async Task<Reserva?> GetReservaByIdAsync(int id)
        {
            return await _entities
                .Include(r => r.Huesped)
                .Include(r => r.Habitacion)
                .Include(r => r.TipoHabitacion)
                .FirstOrDefaultAsync(r => r.IdReserva == id);
        }

        public async Task<Reserva?> GetReservaByCodigoAsync(string codigo)
        {
            return await _entities
                .Include(r => r.Huesped)
                .Include(r => r.Habitacion)
                .Include(r => r.TipoHabitacion)
                .FirstOrDefaultAsync(r => r.CodigoReserva == codigo);
        }

        public async Task InsertReserva(Reserva reserva)
        {
            await Add(reserva);
            // ❌ NO hacer SaveChanges aquí, lo maneja el UnitOfWork
        }

        public async Task UpdateReserva(Reserva reserva)
        {
            Update(reserva);
            // ❌ NO hacer SaveChanges aquí, lo maneja el UnitOfWork
        }

        public async Task DeleteReserva(Reserva reserva)
        {
            await Delete(reserva.IdReserva);
            // ❌ NO hacer SaveChanges aquí, lo maneja el UnitOfWork
        }
    }
}
