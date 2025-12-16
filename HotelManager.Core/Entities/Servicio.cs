using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManager.Core.Entities
{
    [Table("Servicio")]
    public class Servicio
    {
        [Key]
        public int IdServicio { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Column(TypeName = "text")]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required]
        public string TipoServicio { get; set; } = null!; // Restaurante, Room Service, Lavanderia, Spa, Transporte, Minibar, Otro

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<ReservaServicio> ReservaServicios { get; set; } = new List<ReservaServicio>();
    }
}
