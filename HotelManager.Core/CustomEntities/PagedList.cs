using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.CustomEntities
{
    public class PagedList<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalRecords { get; set; }

        public PagedList(IEnumerable<T> items, int totalRecords)
        {
            Items = items;
            TotalRecords = totalRecords;
        }
    }
}
