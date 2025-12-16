using HotelManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        Task<T?> GetById(int id);
        Task Add(T entity);
        void Update(T entity);
        Task Delete(int id);
    }
}
