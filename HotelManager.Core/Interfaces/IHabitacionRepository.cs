using HotelManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Interfaces
{
    public interface IHabitacionRepository : IBaseRepository<Habitacion>
    {
        Task<IEnumerable<Habitacion>> GetAllConTipoAsync();
    }
}
