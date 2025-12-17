using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core.QueryFilters
{
    public class ReservaQueryFilter
    {
        public string? Estado { get; set; }
        public int? IdHuesped { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
