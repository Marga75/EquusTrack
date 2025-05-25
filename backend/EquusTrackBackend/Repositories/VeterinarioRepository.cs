using MySql.Data.MySqlClient;
using EquusTrackBackend.Models;

namespace EquusTrackBackend.Repositories
{
    public static class VeterinarioRepository
    {
        public static bool AgregarVeterinario(int idCaballo, DateTime fecha, string descripcion, string veterinarioNombre, decimal costo)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"INSERT INTO Veterinario 
                             (IdCaballo, Fecha, Descripcion, VeterinarioNombre, Costo)
                             VALUES (@IdCaballo, @Fecha, @Descripcion, @VeterinarioNombre, @Costo)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@IdCaballo", idCaballo);
            cmd.Parameters.AddWithValue("@Fecha", fecha);
            cmd.Parameters.AddWithValue("@Descripcion", descripcion);
            cmd.Parameters.AddWithValue("@VeterinarioNombre", veterinarioNombre);
            cmd.Parameters.AddWithValue("@Costo", costo);

            int filasAfectadas = cmd.ExecuteNonQuery();

            return filasAfectadas > 0;
        }


        public static List<VisitaVeterinario> ObtenerUltimasVisitas(int idCaballo, int limite)
        {
            var lista = new List<VisitaVeterinario>();
            using var conn = Database.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Veterinario WHERE IdCaballo = @idCaballo ORDER BY Fecha DESC LIMIT @limite";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@idCaballo", idCaballo);
            cmd.Parameters.AddWithValue("@limite", limite);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var visita = new VisitaVeterinario
                {
                    Id = reader.GetInt32("Id"),
                    IdCaballo = reader.GetInt32("IdCaballo"),
                    FechaVisita = reader.GetDateTime("Fecha"),
                    Descripcion = reader.GetString("Descripcion"),
                    VeterinarioNombre = reader.GetString("VeterinarioNombre"),
                    Costo = reader.IsDBNull(reader.GetOrdinal("Costo")) ? 0 : reader.GetDecimal("Costo")
                };
                lista.Add(visita);
            }

            return lista;
        }

    }
}
