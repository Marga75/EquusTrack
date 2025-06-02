using MySql.Data.MySqlClient;
using EquusTrackBackend.Models;
using EquusTrackBackend.Models.Requests;

namespace EquusTrackBackend.Repositories
{
    public class HistorialEntrenamientoRepository
    {
        // Obtener historial por ID de jinete
        public static List<HistorialEntrenamiento> ObtenerHistorialPorJinete(int idJinete)
        {
            var lista = new List<HistorialEntrenamiento>();
            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"
                SELECT re.Id, re.IdCaballo, re.IdEntrenamiento, re.Fecha, re.Notas, re.Progreso, re.Estado, re.RegistradoPorId,
                    c.Nombre AS NombreCaballo, e.Titulo AS NombreEntrenamiento, u.Nombre AS NombreUsuario
                FROM RegistroEntrenamientos re
                LEFT JOIN Caballos c ON re.IdCaballo = c.Id
                JOIN Entrenamientos e ON re.IdEntrenamiento = e.Id
                LEFT JOIN Usuarios u ON re.RegistradoPorId = u.Id
                WHERE re.RegistradoPorId = @RegistradoPorId
                ORDER BY re.Fecha DESC
            ";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@RegistradoPorId", idJinete);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new HistorialEntrenamiento
                {
                    Id = reader.GetInt32("Id"),
                    IdCaballo = reader.IsDBNull(reader.GetOrdinal("IdCaballo")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdCaballo")),
                    IdEntrenamiento = reader.GetInt32("IdEntrenamiento"),
                    Fecha = reader.GetDateTime("Fecha"),
                    Notas = reader.IsDBNull(reader.GetOrdinal("Notas")) ? null : reader.GetString("Notas"),
                    Progreso = reader.IsDBNull(reader.GetOrdinal("Progreso")) ? null : reader.GetInt32("Progreso"),
                    Estado = reader.GetString("Estado"),
                    RegistradoPorId = reader.GetInt32("RegistradoPorId"),
                    NombreCaballo = reader.IsDBNull(reader.GetOrdinal("NombreCaballo")) ? null : reader.GetString("NombreCaballo"),
                    NombreEntrenamiento = reader.IsDBNull(reader.GetOrdinal("NombreEntrenamiento")) ? null : reader.GetString("NombreEntrenamiento"),
                    NombreUsuario = reader.IsDBNull(reader.GetOrdinal("NombreUsuario")) ? null : reader.GetString("NombreUsuario")
                });
            }

            return lista;
        }

        // Obtener un registro específico por ID
        public static HistorialEntrenamiento? ObtenerDetallePorId(int id)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"
                SELECT re.Id, re.IdCaballo, re.IdEntrenamiento, re.Fecha, re.Notas, re.Progreso, re.Estado, re.RegistradoPorId,
                    c.Nombre AS NombreCaballo,
                    e.Titulo AS NombreEntrenamiento,
                    e.Tipo,
                    (
                        SELECT IFNULL(SUM(ee.DuracionSegundos), 0)
                        FROM EjerciciosEntrenamiento ee
                        WHERE ee.EntrenamientoId = e.Id
                    ) AS DuracionTotal,
                    u.Nombre AS NombreUsuario
                FROM RegistroEntrenamientos re
                LEFT JOIN Caballos c ON re.IdCaballo = c.Id
                JOIN Entrenamientos e ON re.IdEntrenamiento = e.Id
                LEFT JOIN Usuarios u ON re.RegistradoPorId = u.Id
                WHERE re.Id = @Id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new HistorialEntrenamiento
                {
                    Id = reader.GetInt32("Id"),
                    IdCaballo = reader.IsDBNull(reader.GetOrdinal("IdCaballo")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdCaballo")),
                    IdEntrenamiento = reader.GetInt32("IdEntrenamiento"),
                    Fecha = reader.GetDateTime("Fecha"),
                    Notas = reader.IsDBNull(reader.GetOrdinal("Notas")) ? null : reader.GetString("Notas"),
                    Progreso = reader.IsDBNull(reader.GetOrdinal("Progreso")) ? null : reader.GetInt32("Progreso"),
                    Estado = reader.GetString("Estado"),
                    RegistradoPorId = reader.GetInt32("RegistradoPorId"),
                    NombreCaballo = reader.IsDBNull(reader.GetOrdinal("NombreCaballo")) ? null : reader.GetString("NombreCaballo"),
                    NombreEntrenamiento = reader.IsDBNull(reader.GetOrdinal("NombreEntrenamiento")) ? null : reader.GetString("NombreEntrenamiento"),
                    Tipo = reader.GetString("Tipo"),
                    Duracion = reader.GetInt32("DuracionTotal") / 60,
                    NombreUsuario = reader.IsDBNull(reader.GetOrdinal("NombreUsuario")) ? null : reader.GetString("NombreUsuario")
                };
            }

            return null;
        }

        // Crear un nuevo historial de entrenamiento
        public static bool CrearHistorial(HistorialCrearRequest datos)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"
                INSERT INTO RegistroEntrenamientos (IdCaballo, IdEntrenamiento, Fecha, Notas, Progreso, RegistradoPorId, Estado)
                VALUES (@IdCaballo, @IdEntrenamiento, @Fecha, @Notas, @Progreso, @RegistradoPorId, @Estado)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@IdCaballo", datos.IdCaballo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdEntrenamiento", datos.IdEntrenamiento);
            cmd.Parameters.AddWithValue("@Fecha", datos.Fecha);
            cmd.Parameters.AddWithValue("@Notas", datos.Notas ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Progreso", datos.Progreso ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RegistradoPorId", datos.RegistradoPorId);
            cmd.Parameters.AddWithValue("@Estado", datos.Estado);

            int filas = cmd.ExecuteNonQuery();
            return filas > 0;
        }
    }
}
