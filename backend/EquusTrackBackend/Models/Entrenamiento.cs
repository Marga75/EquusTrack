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
        public string Titulo { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string? Descripcion { get; set; }
        // Duración total en segundos (suma de ejercicios)
        public int DuracionTotalSegundos { get; set; }
        public string? Imagen { get; set; }

        public List<EjercicioEntrenamiento> Ejercicios { get; set; }
    }
}
