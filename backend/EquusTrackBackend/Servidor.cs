using System.Net;
using System.Text.Json;
using EquusTrackBackend.Controllers;
using EquusTrackBackend.Utils;

namespace EquusTrackBackend
{
    public class Servidor
    {
        public static async Task Iniciar()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
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

            Helpers.AgregarCabecerasCORS(context.Response);
            if (metodo == "OPTIONS")
            {
                Helpers.EnviarOpcionesCORS(context.Response);
                return;
            }

            try
            {
                switch (path)
                {
                    case "/registrar" when metodo == "POST":
                        await ControladorRegistro.ProcesarRegistro(context);
                        break;

                    case "/login" when metodo == "POST":
                        await ControladorLogin.ProcesarLogin(context);
                        break;

                    case "/caballos" when metodo == "POST":
                        await ControladorCaballos.ProcesarCaballos(context);
                        break;

                    case string p when p.StartsWith("/api/caballos/usuario/") && metodo == "GET":
                        await ControladorCaballos.ProcesarCaballosPorGet(context);
                        break;

                    case "/api/caballos" when metodo == "POST":
                        await ControladorCaballos.ProcesarCrearCaballo(context);
                        break;

                    case string p when p.StartsWith("/api/caballos/") && metodo == "GET":
                        await ControladorCaballoDetalle.ProcesarCaballoPorId(context);
                        break;

                    default:
                        // Ruta no encontrada
                        context.Response.StatusCode = 404;
                        context.Response.ContentType = "application/json";
                        Helpers.AgregarCabecerasCORS(context.Response);

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
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Ruta no encontrada");
            }
        }
    }
}
