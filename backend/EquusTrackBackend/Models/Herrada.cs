using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusTrackBackend.Models
{
    public class Herrada
    {
        public int Id { get; set; }
        public int IdCaballo { get; set; }
        public DateTime FechaHerrada { get; set; }
        public DateTime ProximaFechaHerrada { get; set; }
        public required string NombreHerrador { get; set; }
        public decimal Costo { get; set; }
    }
}
