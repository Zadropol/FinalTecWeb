using HotelManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IReservaRepository ReservaRepository { get; }
        IBaseRepository<Huesped> HuespedRepository { get; }
        IHabitacionRepository HabitacionRepository { get; }
        IBaseRepository<TipoHabitacion> TipoHabitacionRepository { get; }
        IBaseRepository<Pago> PagoRepository { get; }
        IBaseRepository<Servicio> ServicioRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
