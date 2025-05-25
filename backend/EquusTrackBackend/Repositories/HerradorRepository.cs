using MySql.Data.MySqlClient;
using EquusTrackBackend.Models;

namespace EquusTrackBackend.Repositories
{
    public static class HerradorRepository
    {
        public static bool AgregarHerrada(int idCaballo, DateTime fechaHerrada, DateTime proximaHerrada, string nombreHerrador, decimal costo)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"INSERT INTO Herrador 
                             (IdCaballo, Fecha, ProximaHerrada, HerradorNombre, Costo) 
                             VALUES (@IdCaballo, @Fecha, @ProximaHerrada, @HerradorNombre, @Costo)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@IdCaballo", idCaballo);
            cmd.Parameters.AddWithValue("@Fecha", fechaHerrada);
            cmd.Parameters.AddWithValue("@ProximaHerrada", proximaHerrada);
            cmd.Parameters.AddWithValue("@HerradorNombre", nombreHerrador);
            cmd.Parameters.AddWithValue("@Costo", costo);

            int filasAfectadas = cmd.ExecuteNonQuery();

            return filasAfectadas > 0;
        }

        public static List<Herrada> ObtenerUltimasHerradas(int idCaballo, int limite)
        {
            var lista = new List<Herrada>();

            using var connection = Database.GetConnection();
            connection.Open();

            string sql = @"
                SELECT Id, IdCaballo, Fecha AS FechaHerrada, ProximaHerrada AS ProximaFechaHerrada, HerradorNombre AS NombreHerrador, Costo
                FROM Herrador
                WHERE IdCaballo = @idCaballo
                ORDER BY Fecha DESC
                LIMIT @limite";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@idCaballo", idCaballo);
            cmd.Parameters.AddWithValue("@limite", limite);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var herrada = new Herrada
                {
                    Id = reader.GetInt32("Id"),
                    IdCaballo = reader.GetInt32("IdCaballo"),
                    FechaHerrada = reader.GetDateTime("FechaHerrada"),
                    ProximaFechaHerrada = reader.GetDateTime("ProximaFechaHerrada"),
                    NombreHerrador = reader.GetString("NombreHerrador"),
                    Costo = reader.GetDecimal("Costo")
                };

                lista.Add(herrada);
            }

            return lista;
        }
    }
}
