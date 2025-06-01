using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusTrackBackend.Models
{
    public class EjercicioEntrenamiento
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int DuracionSegundos { get; set; }
        public int? Repeticiones { get; set; }
        public string TipoBloque { get; set; } // "Calentamiento", "Principal", "VueltaCalma"
        public string? ImagenURL { get; set; }
    }
}
