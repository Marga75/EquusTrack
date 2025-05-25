using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusTrackBackend.Models.Requests
{
    public class FisioterapiaRequest
    {
        public int IdCaballo { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Profesional { get; set; } = string.Empty;
        public decimal Costo { get; set; }
    }
}
