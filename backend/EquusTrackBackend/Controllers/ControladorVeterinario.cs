using EquusTrackBackend.Models.Requests;
using EquusTrackBackend.Repositories;
using EquusTrackBackend.Utils;
using System.Net;
using System.Text.Json;

namespace EquusTrackBackend.Controllers
{
    public static class ControladorVeterinario
    {
        public static async Task ProcesarCrearVeterinario(HttpListenerContext context, int idCaballo)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();

                var datos = JsonSerializer.Deserialize<VeterinarioRequest>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new Exception("Datos inválidos");

                if (string.IsNullOrWhiteSpace(datos.VeterinarioNombre) || string.IsNullOrWhiteSpace(datos.Descripcion) || datos.Costo < 0)
                    throw new Exception("Campos inválidos");

                DateTime fechaActual = DateTime.Today;

                bool exito = VeterinarioRepository.AgregarVeterinario(idCaballo, fechaActual, datos.Descripcion, datos.VeterinarioNombre, datos.Costo);

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
                    await writer.WriteAsync(JsonSerializer.Serialize(new { exito = false, mensaje = "Error al guardar veterinario" }));
                }
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al procesar veterinario");
            }
        }

        public static async Task ProcesarUltimasVisitasVeterinario(HttpListenerContext context, int idCaballo)
        {
            try
            {
                var query = context.Request.QueryString;
                bool all = query["all"] == "true";

                int limite = all ? int.MaxValue : 5;
                var visitas = VeterinarioRepository.ObtenerUltimasVisitas(idCaballo, limite);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new { exito = true, visitas }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al obtener visitas veterinarias");
            }
        }

    }
}
