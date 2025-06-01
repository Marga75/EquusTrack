namespace EquusTrackBackend.Models.Requests
{
    public class HistorialCrearRequest
    {
        public int? IdCaballo { get; set; }
        public int IdEntrenamiento { get; set; }
        public DateTime Fecha { get; set; }
        public string? Notas { get; set; }
        public int? Progreso { get; set; }
        public int? RegistradoPorId { get; set; }
        public string Estado { get; set; } = "Completado";
    }
}
