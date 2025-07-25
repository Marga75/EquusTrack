﻿using System.Net;
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
                /*if (path.StartsWith("/uploads/") && metodo == "GET")
                {
                    await StaticFiles.ServirArchivo(context);
                    return;
                }*/

                // Servir archivos estáticos en /uploads/
                if (path.StartsWith("/uploads/") && metodo == "GET")
                {
                    string rutaArchivo = Path.Combine(AppContext.BaseDirectory, path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    if (File.Exists(rutaArchivo))
                    {
                        context.Response.StatusCode = 200;
                        string extension = Path.GetExtension(rutaArchivo).ToLowerInvariant();

                        string mimeType = extension switch
                        {
                            ".jpg" or ".jpeg" => "image/jpeg",
                            ".png" => "image/png",
                            ".gif" => "image/gif",
                            _ => "application/octet-stream"
                        };

                        context.Response.ContentType = mimeType;

                        using (var fileStream = File.OpenRead(rutaArchivo))
                        {
                            await fileStream.CopyToAsync(context.Response.OutputStream);
                        }
                        context.Response.Close();
                        return;
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "Archivo no encontrado" });
                        return;
                    }
                }


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

                // 7. Entrenamientos
                if (path.StartsWith("/api/entrenamientos"))
                {
                    var segmentos = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

                    if (metodo == "GET")
                    {
                        if (segmentos.Length == 2) // /api/entrenamientos
                        {
                            await ControladorEntrenamiento.ProcesarListarEntrenamientos(context);
                            return;
                        }
                        else if (segmentos.Length == 3) // /api/entrenamientos/{id}
                        {
                            if (int.TryParse(segmentos[2], out int idEntrenamiento) && idEntrenamiento > 0)
                            {
                                await ControladorEntrenamiento.ProcesarDetalleEntrenamiento(context, idEntrenamiento);
                                return;
                            }
                            else
                            {
                                context.Response.StatusCode = 400;
                                await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "ID de entrenamiento inválido" });
                                return;
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "Ruta no encontrada" });
                            return;
                        }
                    }
                    else
                    {
                        // Método no soportado
                        context.Response.StatusCode = 405;
                        await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "Método no permitido" });
                        return;
                    }
                }


                // 8. Historial entrenamientos
                if (path.StartsWith("/api/historial"))
                {
                    var segmentos = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

                    // GET /api/historial/jinete/{id}
                    if (metodo == "GET" && segmentos.Length == 4 && segmentos[1] == "historial" && segmentos[2] == "jinete")
                    {
                        if (int.TryParse(segmentos[3], out int idJinete))
                        {
                            await ControladorHistorialEntrenamiento.ProcesarHistorialPorJinete(context, idJinete);
                            return;
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                            await Helpers.EnviarJson(context.Response, new { exito = false, mensaje = "ID de jinete inválido" });
                            return;
                        }
                    }

                    // GET /api/historial/{id}
                    if (metodo == "GET" && segmentos.Length == 3 && int.TryParse(segmentos[2], out int idHistorial))
                    {
                        await ControladorHistorialEntrenamiento.ProcesarDetalleHistorial(context, idHistorial);
                        return;
                    }

                    // POST /api/historial
                    if (metodo == "POST" && segmentos.Length == 2)
                    {
                        await ControladorHistorialEntrenamiento.ProcesarCrearHistorial(context);
                        return;
                    }
                }


                // 9. Ruta no encontrada
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
