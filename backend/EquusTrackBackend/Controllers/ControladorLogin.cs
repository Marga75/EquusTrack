using EquusTrackBackend.Utils;
using System.Net;
using System.Text.Json;
using EquusTrackBackend.Models.Requests;
using EquusTrackBackend.Repositories;

namespace EquusTrackBackend.Controllers
{
    public static class ControladorLogin
    {
        public static async Task ProcesarLogin(HttpListenerContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();

                var datos = JsonSerializer.Deserialize<LoginRequest>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new Exception("Datos de login inválidos");

                var usuario = UsuarioRepository.ValidarLogin(datos.Email, datos.Password);

                context.Response.StatusCode = usuario != null ? 200 : 401;
                context.Response.ContentType = "application/json";
                Helpers.AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                if (usuario != null)
                {
                    await writer.WriteAsync(JsonSerializer.Serialize(new
                    {
                        exito = true,
                        usuario = new
                        {
                            id = usuario.Id,
                            nombre = usuario.Nombre,
                            apellido = usuario.Apellido,
                            email = usuario.Email,
                            rol = usuario.Rol,
                            fechaNacimiento = usuario.FechaNacimiento.ToString("yyyy-MM-dd"),
                            genero = usuario.Genero,
                            fotoUrl = usuario.FotoUrl
                        }
                    }));
                }
                else
                {
                    await writer.WriteAsync(JsonSerializer.Serialize(new
                    {
                        exito = false,
                        mensaje = "Credenciales incorrectas"
                    }));
                }
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Error en login");
            }
        }
    }
}
