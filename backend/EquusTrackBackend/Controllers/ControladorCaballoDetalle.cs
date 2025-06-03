using EquusTrackBackend.Repositories;
using EquusTrackBackend.Utils;
using System.Net;
using System.Text.Json;

namespace EquusTrackBackend.Controllers
{
    public static class ControladorCaballoDetalle
    {
        private static readonly string BaseUrl = "http://localhost:5000/";

        public static async Task ProcesarCaballoPorId(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url.AbsolutePath;
                string[] partes = path.Split('/');

                if (partes.Length < 4 || !int.TryParse(partes[3], out int idCaballo))
                {
                    throw new Exception("ID de caballo inválido");
                }

                var caballo = CaballoRepository.ObtenerCaballoPorId(idCaballo);

                if (caballo == null)
                {
                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "application/json";
                    Helpers.AgregarCabecerasCORS(context.Response);

                    using var writer = new StreamWriter(context.Response.OutputStream);
                    await writer.WriteAsync(JsonSerializer.Serialize(new
                    {
                        exito = false,
                        mensaje = "Caballo no encontrado"
                    }));
                    await writer.FlushAsync();
                    context.Response.Close();
                    return;
                }

                // Agregar URL completa a FotoUrl
                caballo.FotoUrl = string.IsNullOrEmpty(caballo.FotoUrl) ? null : BaseUrl + caballo.FotoUrl.TrimStart('/');

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writerOk = new StreamWriter(context.Response.OutputStream);
                await writerOk.WriteAsync(JsonSerializer.Serialize(new
                {
                    exito = true,
                    caballo
                }));
                await writerOk.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al obtener detalle del caballo");
            }
        }
    }
}
