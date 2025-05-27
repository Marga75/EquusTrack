using EquusTrackBackend.Models;
using EquusTrackBackend.Repositories;
using System.Net;
using System.Text;
using System.Text.Json;

namespace EquusTrackBackend.Controllers
{
    public class ControladorEntrenador
    {
        public static async Task Manejar(HttpListenerContext context)
        {
            string metodo = context.Request.HttpMethod;
            string ruta = context.Request.Url!.AbsolutePath;

            if (metodo == "GET" && ruta == "/entrenadores")
            {
                await ObtenerTodosEntrenadores(context);
            }
            else if (metodo == "POST" && ruta == "/relacion/jinete")
            {
                await SolicitarRelacionJineteEntrenador(context);
            }
            else if (metodo == "PUT" && ruta == "/relacion/estado")
            {
                await ActualizarEstadoRelacion(context);
            }
            else if (metodo == "GET" && ruta.StartsWith("/entrenador/jinete/"))
            {
                await ObtenerEntrenadorDeJinete(context);
            }
            else if (metodo == "GET" && ruta.StartsWith("/relacion/solicitudes/entrenador/"))
            {
                await ObtenerSolicitudesEntrenador(context);
            }
            else if (metodo == "GET" && ruta.StartsWith("/relacion/alumnos/entrenador/"))
            {
                await ObtenerAlumnosEntrenador(context);
            }
            else
            {
                context.Response.StatusCode = 404;
                await context.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Ruta no encontrada"));
                context.Response.Close();
            }
        }

        private static async Task ObtenerTodosEntrenadores(HttpListenerContext context)
        {
            var entrenadores = UsuarioRepository.ObtenerTodosEntrenadores();
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.OutputStream, entrenadores);
            context.Response.Close();
        }

        private static async Task SolicitarRelacionJineteEntrenador(HttpListenerContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                var body = await reader.ReadToEndAsync();
                var datos = JsonSerializer.Deserialize<Dictionary<string, int>>(body);

                if (datos == null || !datos.ContainsKey("idJinete") || !datos.ContainsKey("idEntrenador"))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Datos inválidos"));
                    context.Response.Close();
                    return;
                }

                bool resultado = UsuarioRepository.SolicitarRelacionJineteEntrenador(datos["idJinete"], datos["idEntrenador"]);

                context.Response.ContentType = "application/json";
                await JsonSerializer.SerializeAsync(context.Response.OutputStream, new { exito = resultado });
                context.Response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en SolicitarRelacionJineteEntrenador: " + ex.Message);
                context.Response.StatusCode = 500;
                await context.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Error interno"));
                context.Response.Close();
            }
        }

        private static async Task ActualizarEstadoRelacion(HttpListenerContext context)
        {
            try
            {
                var datos = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(context.Request.InputStream);
                if (datos == null || !datos.ContainsKey("idJinete") || !datos.ContainsKey("idEntrenador") || !datos.ContainsKey("estado"))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Datos inválidos"));
                    context.Response.Close();
                    return;
                }

                int idJinete = datos["idJinete"].GetInt32();
                int idEntrenador = datos["idEntrenador"].GetInt32();
                string estado = datos["estado"].GetString()!;

                bool resultado = UsuarioRepository.ActualizarEstadoRelacion(idJinete, idEntrenador, estado);

                context.Response.ContentType = "application/json";
                await JsonSerializer.SerializeAsync(context.Response.OutputStream, new { exito = resultado });
                context.Response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ActualizarEstadoRelacion: " + ex.Message);
                context.Response.StatusCode = 500;
                await context.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Error interno"));
                context.Response.Close();
            }
        }

        private static async Task ObtenerEntrenadorDeJinete(HttpListenerContext context)
        {
            try
            {
                string ruta = context.Request.Url!.AbsolutePath;
                string[] partes = ruta.Split('/');
                if (partes.Length != 4 || !int.TryParse(partes[3], out int idJinete))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("ID inválido"));
                    context.Response.Close();
                    return;
                }

                Usuario? entrenador = UsuarioRepository.ObtenerEntrenadorDeJinete(idJinete);
                context.Response.ContentType = "application/json";
                await JsonSerializer.SerializeAsync(context.Response.OutputStream, entrenador);
                context.Response.Close();
            }
            catch
            {
                context.Response.StatusCode = 500;
                await context.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Error interno"));
                context.Response.Close();
            }
        }

        private static async Task ObtenerSolicitudesEntrenador(HttpListenerContext context)
        {
            try
            {
                string ruta = context.Request.Url!.AbsolutePath;
                string[] partes = ruta.Split('/');
                if (partes.Length != 5 || !int.TryParse(partes[4], out int idEntrenador))
                {
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    await JsonSerializer.SerializeAsync(context.Response.OutputStream, new { exito = false, mensaje = "ID inválido" });
                    context.Response.Close();
                    return;
                }

                var solicitudes = UsuarioRepository.ObtenerSolicitudesEntrenador(idEntrenador);
                context.Response.ContentType = "application/json";
                await JsonSerializer.SerializeAsync(context.Response.OutputStream, solicitudes);
                context.Response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ObtenerSolicitudesEntrenador: " + ex.Message);
                context.Response.StatusCode = 500;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Error interno"));
                context.Response.Close();
            }
        }

        private static async Task ObtenerAlumnosEntrenador(HttpListenerContext context)
        {
            try
            {
                string ruta = context.Request.Url!.AbsolutePath;
                string[] partes = ruta.Split('/');
                if (partes.Length != 5 || !int.TryParse(partes[4], out int idEntrenador))
                {
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    await JsonSerializer.SerializeAsync(context.Response.OutputStream, new { exito = false, mensaje = "ID inválido" });
                    context.Response.Close();
                    return;
                }

                var alumnos = UsuarioRepository.ObtenerAlumnosEntrenador(idEntrenador);
                context.Response.ContentType = "application/json";
                await JsonSerializer.SerializeAsync(context.Response.OutputStream, alumnos);
                context.Response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ObtenerAlumnosEntrenador: " + ex.Message);
                context.Response.StatusCode = 500;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Error interno"));
                context.Response.Close();
            }
        }

    }
}
