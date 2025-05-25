using EquusTrackBackend.Models.Requests;
using EquusTrackBackend.Utils;
using System.Net;
using System.Text.Json;
using EquusTrackBackend.Repositories;

namespace EquusTrackBackend.Controllers
{
    public class ControladorFisioterapia
    {
        public static async Task ProcesarCrearFisioterapia(HttpListenerContext context, int idCaballo)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();

                var datos = JsonSerializer.Deserialize<FisioterapiaRequest>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new Exception("Datos inválidos");

                if (string.IsNullOrWhiteSpace(datos.Profesional) || string.IsNullOrWhiteSpace(datos.Descripcion) || datos.Costo < 0)
                    throw new Exception("Campos inválidos");

                DateTime fechaActual = DateTime.Today;

                bool exito = FisioRepository.AgregarFisioterapia(idCaballo, fechaActual, datos.Descripcion, datos.Profesional, datos.Costo);

                context.Response.StatusCode = exito ? 200 : 500;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                if (exito)
                {
                    await writer.WriteAsync(JsonSerializer.Serialize(new { exito = true }));
                }
                else
                {
                    await writer.WriteAsync(JsonSerializer.Serialize(new { exito = false, mensaje = "Error al guardar fisioterapia" }));
                }
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al procesar fisioterapia");
            }
        }

        public static async Task ProcesarUltimasSesionesFisio(HttpListenerContext context, int idCaballo)
        {
            try
            {
                var query = context.Request.QueryString;
                bool all = query["all"] == "true";

                int limite = all ? int.MaxValue : 5;
                var sesiones = FisioRepository.ObtenerUltimasSesiones(idCaballo, limite);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new { exito = true, sesiones }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al obtener sesiones de fisioterapia");
            }
        }

    }
}
