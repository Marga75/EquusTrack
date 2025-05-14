using System;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace EquusTrackBackend
{
    public class Servidor
    {
        public static async Task Iniciar()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/"); // Endpoint
            listener.Start();
            Console.WriteLine("Servidor escuchando en http://localhost:5000/");

            while (true)
            {
                var context = await listener.GetContextAsync();
                _ = Task.Run(() => ProcesarPeticion(context));
            }
        }

        private static async Task ProcesarPeticion(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath;
            string metodo = context.Request.HttpMethod;
            Console.WriteLine($"Petición a: {path} - Método: {metodo}");

            if (metodo == "OPTIONS")
            {
                EnviarOpcionesCORS(context.Response);
                return;
            }

            switch (path)
            {
                case "/registrar" when metodo == "POST":
                    await ProcesarRegistro(context);
                    break;

                case "/login" when metodo == "POST":
                    await ProcesarLogin(context);
                    break;

                case "/caballos" when metodo == "POST":
                    await ProcesarCaballos(context);
                    break;

                default:
                    // Ruta no encontrada
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    break;
            }
        }

        private static async Task ProcesarRegistro(HttpListenerContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();
                Console.WriteLine($"Registro - Datos recibidos: {body}");

                var datos = JsonSerializer.Deserialize<UsuarioRegistro>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                bool ok = false;
                if (datos != null)
                {
                    ok = Database.RegistrarUsuario(
                        datos.Nombre, datos.Apellido, datos.Email, datos.Password, datos.Rol
                    );
                }

                context.Response.StatusCode = ok ? 200 : 400;
                context.Response.ContentType = "application/json";
                AgregarCabecerasCORS(context.Response);

                var respuesta = JsonSerializer.Serialize(new
                {
                    mensaje = ok ? "Usuario registrado correctamente" : "Error en el registro"
                });

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(respuesta);
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                EnviarErrorRespuesta(context, ex, "Error en registro");
            }
        }

        private static async Task ProcesarLogin(HttpListenerContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();
                Console.WriteLine($"Login - Datos recibidos: {body}");

                var datos = JsonSerializer.Deserialize<LoginRequest>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (datos == null)
                {
                    throw new Exception("Datos de login inválidos");
                }

                var usuario = Database.ValidarLogin(datos.Email, datos.Password);

                context.Response.StatusCode = usuario != null ? 200 : 401;
                context.Response.ContentType = "application/json";
                AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                if (usuario != null)
                {
                    await writer.WriteAsync(JsonSerializer.Serialize(new
                    {
                        exito = true,
                        id = usuario.Id,
                        nombre = usuario.Nombre,
                        rol = usuario.Rol
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
                EnviarErrorRespuesta(context, ex, "Error en login");
            }
        }

        private static async Task ProcesarCaballos(HttpListenerContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();
                Console.WriteLine($"Caballos - Datos recibidos: {body}");

                var datos = JsonSerializer.Deserialize<CaballosRequest>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (datos == null)
                {
                    throw new Exception("Datos de solicitud de caballos inválidos");
                }

                var lista = Database.ObtenerCaballosPorUsuario(datos.IdUsuario, datos.Rol);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                AgregarCabecerasCORS(context.Response);

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(new { caballos = lista }));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                EnviarErrorRespuesta(context, ex, "Error al obtener caballos");
            }
        }

        private static void EnviarErrorRespuesta(HttpListenerContext context, Exception ex, string mensaje)
        {
            Console.WriteLine($"{mensaje}: {ex.Message}");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            AgregarCabecerasCORS(context.Response);

            try
            {
                using var writer = new StreamWriter(context.Response.OutputStream);
                writer.Write(JsonSerializer.Serialize(new
                {
                    exito = false,
                    mensaje = $"Error en el servidor: {ex.Message}"
                }));
                writer.Flush();
                context.Response.Close();
            }
            catch
            {
                // Si falla el manejo de errores, al menos intentamos cerrar la respuesta
                try { context.Response.Close(); } catch { }
            }
        }

        private static void AgregarCabecerasCORS(HttpListenerResponse response)
        {
            response.AddHeader("Access-Control-Allow-Origin", "http://localhost:5173");
            response.AddHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
        }

        private static void EnviarOpcionesCORS(HttpListenerResponse response)
        {
            response.StatusCode = 204;
            AgregarCabecerasCORS(response);
            response.Close();
        }

        private class UsuarioRegistro
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Rol { get; set; }
        }

        private class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        private class CaballosRequest
        {
            public int IdUsuario { get; set; }
            public string Rol { get; set; }
        }
    }
}
