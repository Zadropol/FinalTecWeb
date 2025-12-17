using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManager.Core.Entities
{
    public abstract class BaseEntity
    {
        [NotMapped]
        public int Id { get; set; }
    }
}
