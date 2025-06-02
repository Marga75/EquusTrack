namespace EquusTrackBackend.Models
{
    public class HistorialEntrenamiento
    {
        public int Id { get; set; }
        public int? IdCaballo { get; set; }
        public int IdEntrenamiento { get; set; }
        public DateTime Fecha { get; set; }
        public string? Notas { get; set; }
        public int? Progreso { get; set; }
        public string Estado { get; set; } = "Completado";
        public int RegistradoPorId { get; set; }

        // Extras para mostrar nombres relacionados
        public string? NombreCaballo { get; set; }
        public string? NombreEntrenamiento { get; set; }
        public string? NombreUsuario { get; set; }
    }
}
