using System.Text.Json.Serialization;

namespace EquusTrackBackend.Models.Requests
{
    public class HerradaRequest
    {
        [JsonPropertyName("idCaballo")]
        public int IdCaballo { get; set; }

        [JsonPropertyName("fecha")]
        public DateTime FechaHerrada { get; set; }

        [JsonPropertyName("herradorNombre")]
        public string NombreHerrador { get; set; } = string.Empty;

        [JsonPropertyName("costo")]
        public decimal Costo { get; set; }
    }
}
