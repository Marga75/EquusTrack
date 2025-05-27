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
                // 1. Obtener caballos por usuario
                if (path.StartsWith("/api/caballos/usuario/") && metodo == "GET")
                {
                    await ControladorCaballos.ProcesarCaballosPorGet(context);
                    return;
                }

                // 2. Crear herrada, veterinario o fisio
                if (path.StartsWith("/api/caballos/") && metodo == "POST")
                {
                    var segmentos = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (segmentos.Length == 4)
                    {
                        bool parsed = int.TryParse(segmentos[2], out int idCaballo);
                        string tipo = segmentos[3].ToLower();

                        if (!parsed || idCaballo <= 0)
                        {
                            context.Response.StatusCode = 400;
                            await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "ID de caballo inválido" });
                            return;
                        }

                        switch (tipo)
                        {
                            case "herrador":
                                await ControladorHerradas.ProcesarCrearHerrada(context, idCaballo);
                                return;
                            case "veterinario":
                                await ControladorVeterinario.ProcesarCrearVeterinario(context, idCaballo);
                                return;
                            case "fisio":
                                await ControladorFisioterapia.ProcesarCrearFisioterapia(context, idCaballo);
                                return;
                            default:
                                context.Response.StatusCode = 404;
                                await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "Tipo no reconocido" });
                                return;
                        }
                    }
                }

                // 3. Obtener últimas herradas, veterinario o fisio
                if (path.StartsWith("/api/caballos/") && metodo == "GET")
                {
                    var segmentos = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (segmentos.Length == 4)
                    {
                        bool parsed = int.TryParse(segmentos[2], out int idCaballo);
                        string tipo = segmentos[3].ToLower();

                        if (!parsed || idCaballo <= 0)
                        {
                            context.Response.StatusCode = 400;
                            await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "ID de caballo inválido" });
                            return;
                        }

                        switch (tipo)
                        {
                            case "herrador":
                                await ControladorHerradas.ProcesarUltimasHerradas(context, idCaballo);
                                return;
                            case "veterinario":
                                await ControladorVeterinario.ProcesarUltimasVisitasVeterinario(context, idCaballo);
                                return;
                            case "fisio":
                                await ControladorFisioterapia.ProcesarUltimasSesionesFisio(context, idCaballo);
                                return;
                            default:
                                context.Response.StatusCode = 404;
                                await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "Tipo no reconocido" });
                                return;
                        }
                    }
                }

                // 4. Rutas simples (registro, login, crear caballo)
                switch (path)
                {
                    case "/registrar" when metodo == "POST":
                        await ControladorRegistro.ProcesarRegistro(context);
                        return;

                    case "/login" when metodo == "POST":
                        await ControladorLogin.ProcesarLogin(context);
                        return;

                    case "/caballos" when metodo == "POST":
                        await ControladorCaballos.ProcesarCaballos(context);
                        return;

                    case "/api/caballos" when metodo == "POST":
                        await ControladorCaballos.ProcesarCrearCaballo(context);
                        return;
                }

                // 5. Obtener detalles de caballo por ID
                if (path.StartsWith("/api/caballos/") && metodo == "GET")
                {
                    await ControladorCaballoDetalle.ProcesarCaballoPorId(context);
                    return;
                }

                // 6. Rutas de entrenador/jinete
                if ((path == "/entrenadores" && metodo == "GET") ||
                    (path == "/relacion/jinete" && metodo == "POST") ||
                    (path == "/relacion/estado" && metodo == "PUT") ||
                    (path.StartsWith("/entrenador/jinete/") && metodo == "GET") ||
                    (path.StartsWith("/relacion/solicitudes/entrenador/") && metodo == "GET") ||
                    (path.StartsWith("/relacion/alumnos/entrenador/") && metodo == "GET"))
                {
                    await ControladorEntrenador.Manejar(context);
                    return;
                }

                // 7. Ruta no encontrada
                context.Response.StatusCode = 404;
                await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "Ruta no encontrada" });

            }
            catch (Exception ex)
            {
                await Helpers.EnviarErrorRespuesta(context, ex, "Ruta no encontrada");
            }
        }
    }
}
