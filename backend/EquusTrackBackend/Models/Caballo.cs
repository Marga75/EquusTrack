
namespace EquusTrackBackend.Models
{
    public class Caballo
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Raza { get; set; }
        public required string Color { get; set; }
        public required string FotoUrl { get; set; }
        public int IdUsuario { get; set; }
        public int? IdEntrenador { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaAdopcion { get; set; }
    }
}
