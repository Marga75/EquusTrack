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

        public static EntrenamientoDetalle? ObtenerPorId(int id)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string sql = "SELECT Id, Titulo, Tipo, Descripcion, Duracion, Imagen FROM Entrenamientos WHERE Id = @Id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new EntrenamientoDetalle
                {
                    Id = reader.GetInt32("Id"),
                    Titulo = reader.GetString("Titulo"),
                    Tipo = reader.GetString("Tipo"),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString("Descripcion"),
                    Duracion = reader.GetInt32("Duracion"),
                    Imagen = reader.IsDBNull(reader.GetOrdinal("Imagen")) ? null : reader.GetString("Imagen"),
                };
            }

            return null;
        }

    }
}
