using System;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

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

            AgregarCabecerasCORS(context.Response);

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

                case string p when p.StartsWith("/api/caballos/usuario/") && metodo == "GET":
                    await ProcesarCaballosPorGet(context);
                    break;

                case "/api/caballos" when metodo == "POST":
                    await ProcesarCrearCaballo(context);
                    break;

                case string p when p.StartsWith("/api/caballos/") && metodo == "GET":
                    await ProcesarCaballoPorId(context);
                    break;

                default:
                    // Ruta no encontrada
                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "application/json";
                    AgregarCabecerasCORS(context.Response);

                    using (var writer = new StreamWriter(context.Response.OutputStream))
                    {
                        await writer.WriteAsync(JsonSerializer.Serialize(new
                        {
                            exito = false,
                            mensaje = "Ruta no encontrada"
                        }));
                    }

                    context.Response.Close();
                    break;
            }
        }

        private static async Task ProcesarCaballosPorGet(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url.AbsolutePath;
                string[] partes = path.Split('/');

                if (partes.Length < 5 || !int.TryParse(partes[4], out int idUsuario))
                {
                    throw new Exception("ID de usuario inválido");
                }

                // Intentar obtener el rol desde query, si no viene, usar un valor por defecto
                string rol = context.Request.QueryString["rol"] ?? "Jinete"; // Cambia esto si el rol predeterminado debe ser otro

                if (string.IsNullOrWhiteSpace(rol))
                {
                    throw new Exception("El rol es obligatorio");
                }

                var lista = Database.ObtenerCaballosPorUsuario(idUsuario, rol);

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
                EnviarErrorRespuesta(context, ex, "Error al obtener caballos (GET)");
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
                    ok = Database.RegistrarUsuario
                        (
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
                AgregarCabecerasCORS(context.Response);

                var respuesta = JsonSerializer.Serialize(new
                {
                    exito = ok,
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
                        usuario = new
                        {
                            id = usuario.Id,
                            nombre = usuario.Nombre,
                            apellido = usuario.Apellido,
                            email = usuario.Email,
                            rol = usuario.Rol,
                            fechaNacimiento = usuario.FechaNacimiento.ToString("yyyy-MM-dd"),
                            genero = usuario.Genero
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

        private static async Task ProcesarCrearCaballo(HttpListenerContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();
                Console.WriteLine($"Crear Caballo - Datos recibidos: {body}");

                var datos = JsonSerializer.Deserialize<CaballoNuevoRequest>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (datos == null)
                    throw new Exception("Datos inválidos");

                bool creado = Database.CrearCaballo(
                    datos.IdUsuario,
                    datos.Nombre,
                    datos.Edad,
                    datos.Raza,
                    datos.Color,
                    datos.FotoUrl,
                    datos.IdEntrenador
                );

                context.Response.StatusCode = creado ? 200 : 400;
                context.Response.ContentType = "application/json";
                AgregarCabecerasCORS(context.Response);

                var respuesta = new
                {
                    exito = creado,
                    mensaje = creado ? "Caballo creado correctamente" : "No se pudo crear el caballo"
                };

                using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync(JsonSerializer.Serialize(respuesta));
                await writer.FlushAsync();
                context.Response.Close();
            }
            catch (Exception ex)
            {
                EnviarErrorRespuesta(context, ex, "Error al crear caballo");
            }
        }

        private static async Task ProcesarCaballoPorId(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url.AbsolutePath;
                string[] partes = path.Split('/');

                if (partes.Length < 4 || !int.TryParse(partes[3], out int idCaballo))
                {
                    throw new Exception("ID de caballo inválido");
                }

                var caballo = Database.ObtenerCaballoPorId(idCaballo);

                if (caballo == null)
                {
                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "application/json";
                    AgregarCabecerasCORS(context.Response);

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

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                AgregarCabecerasCORS(context.Response);

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
                EnviarErrorRespuesta(context, ex, "Error al obtener detalle del caballo");
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
            public DateTime FechaNacimiento { get; set; }
            public string Genero { get; set; }
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

        private class CaballoNuevoRequest
        {
            [JsonPropertyName("usuarioId")]
            public int IdUsuario { get; set; }
            public string Nombre { get; set; }
            public string Edad { get; set; }
            public string Raza { get; set; }
            public string Color { get; set; }
            public string FotoUrl { get; set; }
            public int? IdEntrenador { get; set; }
        }

    }
}
