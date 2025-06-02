using System.Net;
using System.Text.Json;
using EquusTrackBackend.Models;
using EquusTrackBackend.Models.Requests;
using EquusTrackBackend.Repositories;
using EquusTrackBackend.Utils;

namespace EquusTrackBackend.Controllers
{
    public static class ControladorHistorialEntrenamiento
    {
        // Obtener historial de entrenamientos de un jinete
        public static async Task ProcesarHistorialPorJinete(HttpListenerContext context, int idJinete)
        {
            try
            {
                List<HistorialEntrenamiento> historial = HistorialEntrenamientoRepository.ObtenerHistorialPorJinete(idJinete);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new { exito = true, historial }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al obtener historial del jinete");
            }
        }

        // Obtener detalle de un historial por su ID
        public static async Task ProcesarDetalleHistorial(HttpListenerContext context, int idHistorial)
        {
            try
            {
                var detalle = HistorialEntrenamientoRepository.ObtenerDetallePorId(idHistorial);

                if (detalle == null)
                {
                    context.Response.StatusCode = 404;
                    await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "Historial no encontrado" });
                    return;
                }

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new { exito = true, detalle }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al obtener detalle del historial");
            }
        }

        // Crear nuevo registro de historial de entrenamiento
        public static async Task ProcesarCrearHistorial(HttpListenerContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();

                var datos = JsonSerializer.Deserialize<HistorialCrearRequest>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new Exception("Datos inválidos");

                bool creado = HistorialEntrenamientoRepository.CrearHistorial(datos);

                context.Response.StatusCode = creado ? 200 : 400;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new
                {
                    exito = creado,
                    mensaje = creado ? "Historial creado correctamente" : "No se pudo crear el historial"
                }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al crear historial");
            }
        }
    }
}
