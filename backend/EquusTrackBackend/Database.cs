using MySql.Data.MySqlClient;
using System;

namespace EquusTrackBackend
{
    internal class Database
    {
        private const string connectionString = "server=localhost;database=EquusTrackDB;user=root;password=123456789;";

        public static MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        public static bool Login(string email, string password)
        {
            using (var conn = GetConnection())
            {
                string query = "SELECT PasswordHash FROM Usuarios WHERE Email = @Email";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string storedHash = result.ToString();
                    return BCrypt.Net.BCrypt.Verify(password, storedHash);
                }
                return false;
            }
        }

        public static void InsertarAdminSiNoExiste()
        {
            using (var conn = GetConnection())
            {
                string checkQuery = "SELECT COUNT(*) FROM Usuarios WHERE Email = 'admin@gestorcaballos.com'";
                using var checkCmd = new MySqlCommand(checkQuery, conn);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    string hash = BCrypt.Net.BCrypt.HashPassword("admin1234");

                    string insert = @"INSERT INTO Usuarios (Nombre, Apellido, Email, PasswordHash, Rol)
                              VALUES ('Admin', 'Principal', 'admin@gestorcaballos.com', @Hash, 'Administrador')";
                    using var cmd = new MySqlCommand(insert, conn);
                    cmd.Parameters.AddWithValue("@Hash", hash);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Administrador creado con éxito.");
                }
                else
                {
                    Console.WriteLine("Administrador ya existe.");
                }
            }
        }

        public static bool RegistrarUsuario(string nombre, string apellido, string email, string password, string rol)
        {
            if (rol != "Jinete" && rol != "Entrenador")
            {
                Console.WriteLine("Rol no válido.");
                return false;
            }

            using (var conn = GetConnection())
            {
                // Verificar si el email ya existe
                string checkQuery = "SELECT COUNT(*) FROM Usuarios WHERE Email = @Email";
                using var checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@Email", email);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count > 0)
                {
                    Console.WriteLine("Ya existe un usuario con ese correo.");
                    return false;
                }

                // Insertar nuevo usuario con hash
                string hash = BCrypt.Net.BCrypt.HashPassword(password);
                string insertQuery = @"INSERT INTO Usuarios (Nombre, Apellido, Email, PasswordHash, Rol)
                               VALUES (@Nombre, @Apellido, @Email, @Hash, @Rol)";
                using var insertCmd = new MySqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@Nombre", nombre);
                insertCmd.Parameters.AddWithValue("@Apellido", apellido);
                insertCmd.Parameters.AddWithValue("@Email", email);
                insertCmd.Parameters.AddWithValue("@Hash", hash);
                insertCmd.Parameters.AddWithValue("@Rol", rol);
                insertCmd.ExecuteNonQuery();

                Console.WriteLine("Usuario registrado correctamente.");
                return true;
            }
        }

    }
}
