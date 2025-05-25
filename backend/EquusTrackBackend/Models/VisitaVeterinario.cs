namespace EquusTrackBackend.Models
{
    public class VisitaVeterinario
    {
        public int Id { get; set; }
        public int IdCaballo { get; set; }
        public DateTime FechaVisita { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string VeterinarioNombre { get; set; } = string.Empty;
        public decimal Costo { get; set; }
    }
}
