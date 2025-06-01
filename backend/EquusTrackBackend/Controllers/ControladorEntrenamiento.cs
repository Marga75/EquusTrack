using EquusTrackBackend.Repositories;
using EquusTrackBackend.Utils;
using System.Net;
using System.Text.Json;

namespace EquusTrackBackend.Controllers
{
    public class ControladorEntrenamiento
    {
        // Helper para enviar respuesta JSON con cabeceras CORS
        private static async Task EnviarRespuestaJson(HttpListenerResponse response, object obj, int statusCode = 200)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            Helpers.AgregarCabecerasCORS(response);

            using var writer = new StreamWriter(response.OutputStream);
            await writer.WriteAsync(JsonSerializer.Serialize(obj));
            await writer.FlushAsync();
            response.Close();
        }

        // Obtener todos los entrenamientos
        public static async Task ProcesarListarEntrenamientos(HttpListenerContext context)
        {
            try
            {
                var lista = EntrenamientoRepository.ObtenerTodos();

                await EnviarRespuestaJson(context.Response, new { exito = true, entrenamientos = lista });
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al obtener entrenamientos");
            }
        }

        // Obtener un entrenamiento por ID
        public static async Task ProcesarDetalleEntrenamiento(HttpListenerContext context, int idEntrenamiento)
        {
            try
            {
                var entrenamiento = EntrenamientoRepository.ObtenerPorId(idEntrenamiento);
                if (entrenamiento == null)
                {
                    await EnviarRespuestaJson(context.Response, new { exito = false, mensaje = "Entrenamiento no encontrado" }, 404);
                    return;
                }

                await EnviarRespuestaJson(context.Response, new { exito = true, entrenamiento });
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al obtener entrenamiento");
            }
        }

    }
}
