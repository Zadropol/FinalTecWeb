using HotelManager.Core.Entities;
using HotelManager.Core.Interfaces;
using HotelManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Infrastructure.Repositories
{
    public class HabitacionRepository : BaseRepository<Habitacion>, IHabitacionRepository
    {
        public HabitacionRepository(HotelContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Habitacion>> GetAllConTipoAsync()
        {
            return await _entities
                .Include(h => h.TipoHabitacion)
                .Where(h => h.Activo)
                .ToListAsync();
        }
    }
}
