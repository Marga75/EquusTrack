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
            Console.WriteLine($"Petición a: {path}");

            if (context.Request.HttpMethod == "OPTIONS")
            {
                EnviarOpcionesCORS(context.Response);
                return;
            }

            if (path == "/registrar" && context.Request.HttpMethod == "POST")
            {
                try
                {
                    using var reader = new StreamReader(context.Request.InputStream);
                    string body = await reader.ReadToEndAsync();
                    Console.WriteLine($"Datos recibidos: {body}"); // Log para depuración
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
                    Console.WriteLine($"Error: {ex.Message}");
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    AgregarCabecerasCORS(context.Response);
                    using var writer = new StreamWriter(context.Response.OutputStream);
                    await writer.WriteAsync(JsonSerializer.Serialize(new { mensaje = $"Error en el servidor: {ex.Message}" }));
                    await writer.FlushAsync();
                    context.Response.Close();
                }
            }
            else if (path == "/login" && context.Request.HttpMethod == "POST")
            {
                try
                {
                    using var reader = new StreamReader(context.Request.InputStream);
                    string body = await reader.ReadToEndAsync();
                    Console.WriteLine($"Login - Datos recibidos: {body}"); // Log para depuración

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
                    Console.WriteLine($"Error en login: {ex.Message}");
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    AgregarCabecerasCORS(context.Response);
                    using var writer = new StreamWriter(context.Response.OutputStream);
                    await writer.WriteAsync(JsonSerializer.Serialize(new
                    {
                        exito = false,
                        mensaje = $"Error en el servidor: {ex.Message}"
                    }));
                    await writer.FlushAsync();
                    context.Response.Close();
                }
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
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

    }
}
