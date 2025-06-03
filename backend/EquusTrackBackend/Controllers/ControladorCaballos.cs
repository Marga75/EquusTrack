using System.Net;
using System.Text.Json;
using EquusTrackBackend.Utils;
using EquusTrackBackend.Models.Requests;
using EquusTrackBackend.Repositories;
using EquusTrackBackend.Models;
using HttpMultipartParser;

namespace EquusTrackBackend.Controllers
{
    public static class ControladorCaballos
    {
        private static readonly string BaseUrl = "http://localhost:5000/";

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

                // Agregar URL completa a FotoUrl
                foreach (var caballo in lista)
                {
                    caballo.FotoUrl = string.IsNullOrEmpty(caballo.FotoUrl) ? null : BaseUrl + caballo.FotoUrl.TrimStart('/');
                }

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

                // Agregar URL completa a FotoUrl
                foreach (var caballo in lista)
                {
                    caballo.FotoUrl = string.IsNullOrEmpty(caballo.FotoUrl) ? null : BaseUrl + caballo.FotoUrl.TrimStart('/');
                }

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
                var req = context.Request;

                if (!req.ContentType.StartsWith("multipart/form-data"))
                {
                    context.Response.StatusCode = 400;
                    await Helpers.EnviarErrorRespuesta(context, null, "Content-Type debe ser multipart/form-data");
                    return;
                }

                // Parsear formulario multipart
                var multipart = MultipartFormDataParser.Parse(req.InputStream);

                // Obtener campos del formulario
                string idUsuarioStr = multipart.Parameters.FirstOrDefault(p => p.Name == "idUsuario")?.Data;
                string nombre = multipart.Parameters.FirstOrDefault(p => p.Name == "nombre")?.Data;
                string raza = multipart.Parameters.FirstOrDefault(p => p.Name == "raza")?.Data;
                string color = multipart.Parameters.FirstOrDefault(p => p.Name == "color")?.Data;
                string fechaNacimientoStr = multipart.Parameters.FirstOrDefault(p => p.Name == "fechaNacimiento")?.Data;
                string fechaAdopcionStr = multipart.Parameters.FirstOrDefault(p => p.Name == "fechaAdopcion")?.Data;
                string idEntrenadorStr = multipart.Parameters.FirstOrDefault(p => p.Name == "idEntrenador")?.Data;

                if (!int.TryParse(idUsuarioStr, out int idUsuario))
                    throw new Exception("IdUsuario inválido");

                DateTime? fechaNacimiento = null;
                if (!string.IsNullOrWhiteSpace(fechaNacimientoStr))
                    fechaNacimiento = DateTime.Parse(fechaNacimientoStr);

                DateTime? fechaAdopcion = null;
                if (!string.IsNullOrWhiteSpace(fechaAdopcionStr))
                    fechaAdopcion = DateTime.Parse(fechaAdopcionStr);

                int? idEntrenador = null;
                if (int.TryParse(idEntrenadorStr, out int entrenadorParsed))
                    idEntrenador = entrenadorParsed;

                // Procesar archivo imagen
                var fotoFile = multipart.Files.FirstOrDefault();
                string fotoUrl = "";

                if (fotoFile != null)
                {
                    string carpetaFotos = Path.Combine(AppContext.BaseDirectory, "Uploads", "Caballos");
                    if (!Directory.Exists(carpetaFotos)) Directory.CreateDirectory(carpetaFotos);

                    string extension = Path.GetExtension(fotoFile.FileName);
                    string nombreArchivo = Guid.NewGuid().ToString() + extension;
                    string rutaArchivo = Path.Combine(carpetaFotos, nombreArchivo);

                    using var fileStream = File.Create(rutaArchivo);
                    fotoFile.Data.CopyTo(fileStream);

                    // Guardamos la ruta relativa que usarás para mostrar la imagen
                    fotoUrl = "/uploads/caballos/" + nombreArchivo;
                }

                // Llamar a repo para crear caballo (modifica para que acepte fotoUrl)
                bool creado = CaballoRepository.CrearCaballo(
                    idUsuario,
                    nombre,
                    raza,
                    color,
                    fotoUrl,
                    fechaNacimiento,
                    fechaAdopcion,
                    idEntrenador
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
