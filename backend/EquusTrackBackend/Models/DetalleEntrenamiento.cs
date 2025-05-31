using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusTrackBackend.Models
{
    public class DetalleEntrenamiento
    {
        public int Id { get; set; }
        public int EntrenamientoId { get; set; }
        public string NombreEjercicio { get; set; }
        public string DescripcionEjercicio { get; set; }
        public int DuracionEjercicio { get; set; }
        public int Orden { get; set; }
    }
}
