using MySql.Data.MySqlClient;
using EquusTrackBackend.Models;

namespace EquusTrackBackend.Repositories
{
    public static class FisioRepository
    {
        public static bool AgregarFisioterapia(int idCaballo, DateTime fecha, string descripcion, string profesional, decimal costo)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"INSERT INTO Fisioterapia 
                             (IdCaballo, Fecha, Descripcion, Profesional, Costo)
                             VALUES (@IdCaballo, @Fecha, @Descripcion, @Profesional, @Costo)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@IdCaballo", idCaballo);
            cmd.Parameters.AddWithValue("@Fecha", fecha);
            cmd.Parameters.AddWithValue("@Descripcion", descripcion);
            cmd.Parameters.AddWithValue("@Profesional", profesional);
            cmd.Parameters.AddWithValue("@Costo", costo);

            int filasAfectadas = cmd.ExecuteNonQuery();

            return filasAfectadas > 0;
        }

        public static List<SesionFisio> ObtenerUltimasSesiones(int idCaballo, int limite)
        {
            var lista = new List<SesionFisio>();
            using var conn = Database.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Fisioterapia WHERE IdCaballo = @idCaballo ORDER BY Fecha DESC LIMIT @limite";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@idCaballo", idCaballo);
            cmd.Parameters.AddWithValue("@limite", limite);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var sesion = new SesionFisio
                {
                    Id = reader.GetInt32("Id"),
                    IdCaballo = reader.GetInt32("IdCaballo"),
                    FechaSesion = reader.GetDateTime("Fecha"),
                    Descripcion = reader.GetString("Descripcion"),
                    Profesional = reader.GetString("Profesional"),
                    Costo = reader.GetDecimal("Costo")
                };
                lista.Add(sesion);
            }

            return lista;
        }

    }
}
