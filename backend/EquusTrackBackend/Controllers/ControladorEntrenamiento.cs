using EquusTrackBackend.Repositories;
using EquusTrackBackend.Utils;
using System.Net;
using System.Text.Json;

namespace EquusTrackBackend.Controllers
{
    public class ControladorEntrenamiento
    {
        // Obtener todos los entrenamientos
        public static async Task ProcesarListarEntrenamientos(HttpListenerContext context)
        {
            try
            {
                var lista = EntrenamientoRepository.ObtenerTodos();

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new { exito = true, entrenamientos = lista }));
                await writer.FlushAsync();
                context.Response.Close();
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
                    context.Response.StatusCode = 404;
                    await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "Entrenamiento no encontrado" });
                    return;
                }

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new { exito = true, entrenamiento }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al obtener entrenamiento");
            }
        }

    }
}
