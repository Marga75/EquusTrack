using EquusTrackBackend.Utils;
using System.Net;
using System.Text.Json;
using EquusTrackBackend.Models.Requests;
using EquusTrackBackend.Repositories;

namespace EquusTrackBackend.Controllers
{
    public static class ControladorHerradas
    {
        public static async Task ProcesarCrearHerrada(HttpListenerContext context, int idCaballo)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();
                Console.WriteLine($"[DEBUG] JSON recibido: {body}");

                var datos = JsonSerializer.Deserialize<HerradaRequest>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new Exception("Datos inválidos");

                if (string.IsNullOrWhiteSpace(datos.NombreHerrador) || datos.Costo < 0)
                    throw new Exception("Campos inválidos");

                // Calcular fecha actual y próxima herrada (1 mes después)
                DateTime fechaActual = datos.FechaHerrada;
                DateTime proximaHerrada = fechaActual.AddDays(45);

                bool exito = HerradorRepository.AgregarHerrada(idCaballo, fechaActual, proximaHerrada, datos.NombreHerrador, datos.Costo);

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
                    await writer.WriteAsync(JsonSerializer.Serialize(new { exito = false, mensaje = "Error al guardar herrada" }));
                }
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al procesar herrada");
            }
        }

        public static async Task ProcesarUltimasHerradas(HttpListenerContext context, int idCaballo)
        {
            try
            {
                var query = context.Request.QueryString;
                bool all = query["all"] == "true"; // Si viene ?all=true, obtiene todas, sino solo 5

                int limite = all ? int.MaxValue : 5;
                var herradas = HerradorRepository.ObtenerUltimasHerradas(idCaballo, limite);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new { exito = true, herradas }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al obtener herradas");
            }
        }

    }
}
