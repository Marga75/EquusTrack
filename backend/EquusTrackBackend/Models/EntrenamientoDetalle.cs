using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusTrackBackend.Models
{
    public class EntrenamientoDetalle
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int Duracion { get; set; }
        public string? Imagen { get; set; }
    }
}