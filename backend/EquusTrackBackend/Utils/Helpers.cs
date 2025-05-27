using System.Net;
using System.Text.Json;

namespace EquusTrackBackend.Utils
{
    public static class Helpers
    {
        public static void AgregarCabecerasCORS(HttpListenerResponse response)
        {
            response.AddHeader("Access-Control-Allow-Origin", "http://localhost:5173");
            response.AddHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT");
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
        }

        public static void EnviarOpcionesCORS(HttpListenerResponse response)
        {
            response.StatusCode = 200;
            AgregarCabecerasCORS(response);
            response.Close();
        }

        public static async Task EnviarErrorRespuesta(HttpListenerContext context, Exception ex, string mensaje)
        {
            Console.WriteLine($"[ERROR] {mensaje}: {ex.Message}");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            AgregarCabecerasCORS(context.Response);

            using var writer = new StreamWriter(context.Response.OutputStream);
            await writer.WriteAsync(JsonSerializer.Serialize(new
            {
                exito = false,
                mensaje,
                detalle = ex.Message
            }));
            await writer.FlushAsync();
            context.Response.Close();
        }

        public static async Task EnviarJson(HttpListenerResponse response, object contenido, int codigoEstado = 200)
        {
            response.StatusCode = codigoEstado;
            response.ContentType = "application/json";

            AgregarCabecerasCORS(response);

            using var writer = new StreamWriter(response.OutputStream);
            await writer.WriteAsync(JsonSerializer.Serialize(contenido));
            await writer.FlushAsync();
            response.Close();
        }
    }
}
