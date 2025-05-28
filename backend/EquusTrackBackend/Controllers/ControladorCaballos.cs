using System.Net;
using System.Text.Json;
using EquusTrackBackend.Utils;
using EquusTrackBackend.Models.Requests;
using EquusTrackBackend.Repositories;
using EquusTrackBackend.Models;

namespace EquusTrackBackend.Controllers
{
    public static class ControladorCaballos
    {
        // Procesa una solicitud POST para obtener los caballos de un usuario según su rol
        public static async Task ProcesarCaballos(HttpListenerContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();

                var datos = JsonSerializer.Deserialize<CaballosRequest>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new Exception("Datos inválidos para obtener caballos");

                List<Caballo> lista = CaballoRepository.ObtenerCaballosPorUsuario(datos.IdUsuario, datos.Rol);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new { caballos = lista }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al obtener caballos");
            }
        }

        // Procesa una solicitud GET para obtener los caballos de un usuario desde la URL
        public static async Task ProcesarCaballosPorGet(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url.AbsolutePath;
                string[] partes = path.Split('/');

                if (partes.Length < 5 || !int.TryParse(partes[4], out int idUsuario))
                {
                    throw new Exception("ID de usuario inválido");
                }

                string rol = context.Request.QueryString["rol"] ?? "Jinete";

                if (string.IsNullOrWhiteSpace(rol))
                {
                    throw new Exception("El rol es obligatorio");
                }

                List<Caballo> lista = CaballoRepository.ObtenerCaballosPorUsuario(idUsuario, rol);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new { caballos = lista }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al obtener caballos (GET)");
            }
        }

        // Procesa una solicitud POST para crear un nuevo caballo en la base de datos
        public static async Task ProcesarCrearCaballo(HttpListenerContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();

                var datos = JsonSerializer.Deserialize<CaballoNuevoRequest>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new Exception("Datos inválidos para crear caballo");

                bool creado = CaballoRepository.CrearCaballo(
                    datos.IdUsuario,
                    datos.Nombre,
                    datos.Raza,
                    datos.Color,
                    datos.FotoUrl,
                    datos.FechaNacimiento,
                    datos.FechaAdopcion,
                    datos.IdEntrenador
                );

                context.Response.StatusCode = creado ? 200 : 400;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new
                {
                    exito = creado,
                    mensaje = creado ? "Caballo creado correctamente" : "No se pudo crear el caballo"
                }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error al crear caballo");
            }
        }
    }
}
