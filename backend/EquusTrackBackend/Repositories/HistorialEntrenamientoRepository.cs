using MySql.Data.MySqlClient;
using EquusTrackBackend.Models;
using EquusTrackBackend.Models.Requests;

namespace EquusTrackBackend.Repositories
{
    public class HistorialEntrenamientoRepository
    {
        // Obtener historial por ID de caballo
        public static List<HistorialEntrenamiento> ObtenerHistorialPorCaballo(int idCaballo)
        {
            var lista = new List<HistorialEntrenamiento>();
            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"
                SELECT re.Id, re.IdCaballo, re.IdEntrenamiento, re.Fecha, re.Notas, re.Progreso, re.Estado, re.RegistradoPorId,
                       c.Nombre AS NombreCaballo, e.Nombre AS NombreEntrenamiento, u.Nombre AS NombreUsuario
                FROM RegistroEntrenamientos re
                JOIN Caballos c ON re.IdCaballo = c.Id
                JOIN Entrenamientos e ON re.IdEntrenamiento = e.Id
                LEFT JOIN Usuarios u ON re.RegistradoPorId = u.Id
                WHERE re.IdCaballo = @IdCaballo
                ORDER BY re.Fecha DESC";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@IdCaballo", idCaballo);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new HistorialEntrenamiento
                {
                    Id = reader.GetInt32("Id"),
                    IdCaballo = reader.GetInt32("IdCaballo"),
                    IdEntrenamiento = reader.GetInt32("IdEntrenamiento"),
                    Fecha = reader.GetDateTime("Fecha"),
                    Notas = reader.IsDBNull(reader.GetOrdinal("Notas")) ? null : reader.GetString("Notas"),
                    Progreso = reader.IsDBNull(reader.GetOrdinal("Progreso")) ? null : reader.GetInt32("Progreso"),
                    Estado = reader.GetString("Estado"),
                    RegistradoPorId = reader.IsDBNull(reader.GetOrdinal("RegistradoPorId")) ? null : reader.GetInt32("RegistradoPorId"),
                    NombreCaballo = reader.GetString("NombreCaballo"),
                    NombreEntrenamiento = reader.GetString("NombreEntrenamiento"),
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
                       c.Nombre AS NombreCaballo, e.Nombre AS NombreEntrenamiento, u.Nombre AS NombreUsuario
                FROM RegistroEntrenamientos re
                JOIN Caballos c ON re.IdCaballo = c.Id
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
                    IdCaballo = reader.GetInt32("IdCaballo"),
                    IdEntrenamiento = reader.GetInt32("IdEntrenamiento"),
                    Fecha = reader.GetDateTime("Fecha"),
                    Notas = reader.IsDBNull(reader.GetOrdinal("Notas")) ? null : reader.GetString("Notas"),
                    Progreso = reader.IsDBNull(reader.GetOrdinal("Progreso")) ? null : reader.GetInt32("Progreso"),
                    Estado = reader.GetString("Estado"),
                    RegistradoPorId = reader.IsDBNull(reader.GetOrdinal("RegistradoPorId")) ? null : reader.GetInt32("RegistradoPorId"),
                    NombreCaballo = reader.GetString("NombreCaballo"),
                    NombreEntrenamiento = reader.GetString("NombreEntrenamiento"),
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
            cmd.Parameters.AddWithValue("@RegistradoPorId", datos.RegistradoPorId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", datos.Estado);

            int filas = cmd.ExecuteNonQuery();
            return filas > 0;
        }
    }
}
