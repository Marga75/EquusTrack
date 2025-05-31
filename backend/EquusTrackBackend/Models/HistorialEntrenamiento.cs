using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusTrackBackend.Models
{
    public class HistorialEntrenamiento
    {
        public int Id { get; set; }
        public int IdCaballo { get; set; }
        public int IdEntrenamiento { get; set; }
        public DateTime Fecha { get; set; }
        public string Notas { get; set; }
        public int Progreso { get; set; }
    }
}
