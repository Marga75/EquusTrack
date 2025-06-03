using EquusTrackBackend.Utils;
using System.Net;
using System.Text.Json;
using EquusTrackBackend.Models.Requests;
using EquusTrackBackend.Repositories;
using HttpMultipartParser;
using System.Text;

namespace EquusTrackBackend.Controllers
{
    public static class ControladorRegistro
    {

        public static async Task ProcesarRegistro(HttpListenerContext context)
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

                // Parsear formulario multipart desde el stream directamente
                var multipart = MultipartFormDataParser.Parse(req.InputStream);

                // Obtener campos (buscando en la lista)
                string nombre = multipart.Parameters.FirstOrDefault(p => p.Name == "nombre")?.Data;
                string apellido = multipart.Parameters.FirstOrDefault(p => p.Name == "apellido")?.Data;
                string email = multipart.Parameters.FirstOrDefault(p => p.Name == "email")?.Data;
                string password = multipart.Parameters.FirstOrDefault(p => p.Name == "password")?.Data;
                string rol = multipart.Parameters.FirstOrDefault(p => p.Name == "rol")?.Data;
                string fechaStr = multipart.Parameters.FirstOrDefault(p => p.Name == "fechaNacimiento")?.Data;
                string genero = multipart.Parameters.FirstOrDefault(p => p.Name == "genero")?.Data;

                DateTime fechaNacimiento = DateTime.Parse(fechaStr);

                // Archivos
                var fotoFile = multipart.Files.FirstOrDefault();
                string fotoUrl = null;

                if (fotoFile != null)
                {
                    string carpetaFotos = Path.Combine(AppContext.BaseDirectory, "Uploads", "Usuarios");
                    if (!Directory.Exists(carpetaFotos)) Directory.CreateDirectory(carpetaFotos);

                    string extension = Path.GetExtension(fotoFile.FileName);
                    string nombreArchivo = Guid.NewGuid().ToString() + extension;
                    string rutaArchivo = Path.Combine(carpetaFotos, nombreArchivo);

                    using var fileStream = File.Create(rutaArchivo);
                    fotoFile.Data.CopyTo(fileStream);

                    fotoUrl = "/uploads/usuarios/" + nombreArchivo;
                }

                bool ok = UsuarioRepository.RegistrarUsuario(
                    nombre,
                    apellido,
                    email,
                    password,
                    rol,
                    fechaNacimiento,
                    genero,
                    fotoUrl ?? ""
                );

                context.Response.StatusCode = ok ? 200 : 400;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new
                {
                    exito = ok,
                    mensaje = ok ? "Usuario registrado correctamente" : "Error en el registro"
                }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error en registro");
            }
        }
    }
}
