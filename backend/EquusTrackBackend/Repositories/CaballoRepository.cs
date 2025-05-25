using MySql.Data.MySqlClient;
using EquusTrackBackend.Models;

namespace EquusTrackBackend.Repositories
{
    public class CaballoRepository
    {
        public static List<Caballo> ObtenerCaballosPorUsuario(int idUsuario, string rol)
        {
            var lista = new List<Caballo>();
            using var conn = Database.GetConnection();
            conn.Open();

            string query = rol.ToLower() == "entrenador"
                ? @"SELECT c.Id, c.Nombre, c.FotoUrl
                    FROM Caballos c
                    INNER JOIN Usuarios u ON c.IdUsuario = u.Id OR c.IdEntrenador = u.Id
                    WHERE u.Id = @Id"
                : @"SELECT Id, Nombre, FotoUrl FROM Caballos WHERE IdUsuario = @Id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", idUsuario);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Caballo
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    FotoUrl = reader.IsDBNull(reader.GetOrdinal("FotoUrl")) ? null : reader.GetString("FotoUrl"),
                    Color = null,
                    Raza = null
                });
            }

            return lista;
        }

        public static bool CrearCaballo(int idUsuario, string nombre, string raza, string color, string fotoUrl, DateTime? fechaNacimiento, DateTime? fechaAdopcion, int? idEntrenador = null)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            // Validar que el usuario existe
            string checkUserQuery = "SELECT COUNT(*) FROM Usuarios WHERE Id = @IdUsuario";
            using var checkCmd = new MySqlCommand(checkUserQuery, conn);
            checkCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            int userCount = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (userCount == 0)
            {
                Console.WriteLine($"Error: El usuario con Id {idUsuario} no existe.");
                return false;
            }

            string query = @"INSERT INTO Caballos (Nombre, Raza, Color, FotoUrl, IdUsuario, IdEntrenador, FechaNacimiento, FechaAdopcion)
                             VALUES (@Nombre, @Raza, @Color, @FotoUrl, @IdUsuario, @IdEntrenador, @FechaNacimiento, @FechaAdopcion)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Raza", raza ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Color", color ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FotoUrl", fotoUrl ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@IdEntrenador", idEntrenador ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaAdopcion", fechaAdopcion ?? (object)DBNull.Value);

            int filasAfectadas = cmd.ExecuteNonQuery();
            return filasAfectadas > 0;
        }

        public static Caballo? ObtenerCaballoPorId(int id)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"SELECT Id, Nombre, Raza, Color, FotoUrl, IdUsuario, IdEntrenador, FechaNacimiento, FechaAdopcion 
                             FROM Caballos WHERE Id = @Id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Caballo
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Raza = reader.GetString("Raza"),
                    Color = reader.GetString("Color"),
                    FotoUrl = reader.GetString("FotoUrl"),
                    IdUsuario = reader.GetInt32("IdUsuario"),
                    IdEntrenador = reader.IsDBNull(reader.GetOrdinal("IdEntrenador")) ? null : reader.GetInt32("IdEntrenador"),
                    FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("FechaNacimiento")) ? null : reader.GetDateTime("FechaNacimiento"),
                    FechaAdopcion = reader.IsDBNull(reader.GetOrdinal("FechaAdopcion")) ? null : reader.GetDateTime("FechaAdopcion")
                };
            }

            return null;
        }
    }
}
