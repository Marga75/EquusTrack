using EquusTrackBackend.Utils;
using System.Net;
using System.Text.Json;
using EquusTrackBackend.Models.Requests;
using EquusTrackBackend.Repositories;

namespace EquusTrackBackend.Controllers
{
    public static class ControladorRegistro
    {
        public static async Task ProcesarRegistro(HttpListenerContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();

                var datos = JsonSerializer.Deserialize<UsuarioRegistro>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                bool ok = false;
                if(datos != null)
                {
                    ok = UsuarioRepository.RegistrarUsuario(
                        datos.Nombre,
                        datos.Apellido,
                        datos.Email,
                        datos.Password,
                        datos.Rol,
                        datos.FechaNacimiento,
                        datos.Genero
                    );
                }

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
