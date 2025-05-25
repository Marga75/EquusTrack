namespace EquusTrackBackend.Models
{
    public class SesionFisio
    {
        public int Id { get; set; }
        public int IdCaballo { get; set; }
        public DateTime FechaSesion { get; set; }
        public string Descripcion { get; set; } = String.Empty;
        public string Profesional { get; set; } = String.Empty;
        public decimal Costo { get; set; }
    }
}
