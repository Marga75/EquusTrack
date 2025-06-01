using EquusTrackBackend.Models;
using MySql.Data.MySqlClient;

namespace EquusTrackBackend.Repositories
{
    public static class EntrenamientoRepository
    {

        public static List<Entrenamiento> ObtenerTodos()
        {
            var lista = new List<Entrenamiento>();
            using var conn = Database.GetConnection();
            conn.Open();

            string sql = @"
                SELECT e.Id, e.Titulo, e.Tipo, e.Descripcion, e.Imagen,
                       COALESCE(SUM(ee.DuracionSegundos), 0) AS DuracionTotalSegundos
                FROM Entrenamientos e
                LEFT JOIN EjerciciosEntrenamiento ee ON e.Id = ee.EntrenamientoId
                GROUP BY e.Id, e.Titulo, e.Tipo, e.Descripcion, e.Imagen
                ORDER BY e.Titulo;
            ";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var entrenamiento = new Entrenamiento
                {
                    Id = reader.GetInt32("Id"),
                    Titulo = reader.GetString("Titulo"),
                    Tipo = reader.GetString("Tipo"),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString("Descripcion"),
                    Imagen = reader.GetString("Imagen"),
                    DuracionTotalSegundos = reader.GetInt32("DuracionTotalSegundos") / 60
                };
                lista.Add(entrenamiento);
            }

            return lista;
        }

        public static Entrenamiento? ObtenerPorId(int id)
        {
            Entrenamiento? entrenamiento = null;

            using var conn = Database.GetConnection();
            conn.Open();

            // Obtener datos del entrenamiento
            string sqlEntrenamiento = @"
        SELECT e.Id, e.Titulo, e.Tipo, e.Descripcion, e.Imagen,
               COALESCE(SUM(ee.DuracionSegundos), 0) AS DuracionTotalSegundos
        FROM Entrenamientos e
        LEFT JOIN EjerciciosEntrenamiento ee ON e.Id = ee.EntrenamientoId
        WHERE e.Id = @id
        GROUP BY e.Id, e.Titulo, e.Tipo, e.Descripcion, e.Imagen;
    ";

            using var cmdEntr = new MySqlCommand(sqlEntrenamiento, conn);
            cmdEntr.Parameters.AddWithValue("@id", id);

            using var readerEntr = cmdEntr.ExecuteReader();
            if (readerEntr.Read())
            {
                entrenamiento = new Entrenamiento
                {
                    Id = readerEntr.GetInt32("Id"),
                    Titulo = readerEntr.GetString("Titulo"),
                    Tipo = readerEntr.GetString("Tipo"),
                    Descripcion = readerEntr.IsDBNull(readerEntr.GetOrdinal("Descripcion")) ? null : readerEntr.GetString("Descripcion"),
                    Imagen = readerEntr.GetString("Imagen"),
                    DuracionTotalSegundos = readerEntr.GetInt32("DuracionTotalSegundos") / 60,
                    Ejercicios = new List<EjercicioEntrenamiento>()
                };
            }

            readerEntr.Close();

            if (entrenamiento == null) return null;

            // Obtener ejercicios asociados
            string sqlEjercicios = @"
                SELECT Id, Nombre, Descripcion, DuracionSegundos, Repeticiones, TipoBloque, ImagenURL
                FROM EjerciciosEntrenamiento
                WHERE EntrenamientoId = @id
                ORDER BY Orden;
            ";

            using var cmdEjerc = new MySqlCommand(sqlEjercicios, conn);
            cmdEjerc.Parameters.AddWithValue("@id", id);
            using var readerEjerc = cmdEjerc.ExecuteReader();

            while (readerEjerc.Read())
            {
                var ejercicio = new EjercicioEntrenamiento
                {
                    Id = readerEjerc.GetInt32("Id"),
                    Nombre = readerEjerc.GetString("Nombre"),
                    Descripcion = readerEjerc.IsDBNull(readerEjerc.GetOrdinal("Descripcion")) ? null : readerEjerc.GetString("Descripcion"),
                    DuracionSegundos = readerEjerc.GetInt32("DuracionSegundos") / 60,
                    Repeticiones = readerEjerc.IsDBNull(readerEjerc.GetOrdinal("Repeticiones")) ? (int?)null : readerEjerc.GetInt32("Repeticiones"),
                    TipoBloque = readerEjerc.GetString("TipoBloque"),
                    ImagenURL = readerEjerc.IsDBNull(readerEjerc.GetOrdinal("ImagenURL")) ? null : readerEjerc.GetString("ImagenURL")
                };
                entrenamiento.Ejercicios.Add(ejercicio);
            }

            return entrenamiento;
        }


    }
}
