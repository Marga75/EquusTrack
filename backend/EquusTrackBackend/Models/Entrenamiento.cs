using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusTrackBackend.Models
{
    public class Entrenamiento
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public int Duracion { get; set; }
        public string Imagen { get; set; }
    }
}
