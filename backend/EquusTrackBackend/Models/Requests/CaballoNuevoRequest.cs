using System.Text.Json.Serialization;

namespace EquusTrackBackend.Models.Requests
{
    public class CaballoNuevoRequest
    {
        [JsonPropertyName("usuarioId")]
        public int IdUsuario { get; set; }
        public required string Nombre { get; set; }
        public required string Raza { get; set; }
        public required string Color { get; set; }
        public required string FotoUrl { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaAdopcion { get; set; }
        public int? IdEntrenador { get; set; }
    }
}
