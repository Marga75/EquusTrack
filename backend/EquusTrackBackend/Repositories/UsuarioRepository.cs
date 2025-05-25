using MySql.Data.MySqlClient;
using EquusTrackBackend.Models;
using System;
using BCrypt.Net;

namespace EquusTrackBackend.Repositories
{
    public class UsuarioRepository
    {
        public static Usuario? ValidarLogin(string email, string password)
        {
            using var conn = Database.GetConnection();
            string query = @"SELECT Id, Nombre, Apellido, Email, PasswordHash, Rol, FechaNacimiento, Genero 
                             FROM Usuarios WHERE Email = @Email";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string hash = reader.GetString("PasswordHash");
                if (BCrypt.Net.BCrypt.Verify(password, hash))
                {
                    return new Usuario
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Email = reader.GetString("Email"),
                        Rol = reader.GetString("Rol"),
                        FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                        Genero = reader.GetString("Genero")
                    };
                }
            }

            return null;
        }

        public static bool Login(string email, string password)
        {
            using var conn = Database.GetConnection();
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

        public static bool RegistrarUsuario(string nombre, string apellido, string email, string password, string rol, DateTime fechaNacimiento, string genero)
        {
            if (rol != "Jinete" && rol != "Entrenador")
            {
                Console.WriteLine("Rol no válido.");
                return false;
            }

            using var conn = Database.GetConnection();

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

            // Insertar nuevo usuario
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            string insertQuery = @"INSERT INTO Usuarios 
                (Nombre, Apellido, Email, PasswordHash, Rol, FechaNacimiento, Genero)
                VALUES (@Nombre, @Apellido, @Email, @Hash, @Rol, @FechaNacimiento, @Genero)";
            using var insertCmd = new MySqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@Nombre", nombre);
            insertCmd.Parameters.AddWithValue("@Apellido", apellido);
            insertCmd.Parameters.AddWithValue("@Email", email);
            insertCmd.Parameters.AddWithValue("@Hash", hash);
            insertCmd.Parameters.AddWithValue("@Rol", rol);
            insertCmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento);
            insertCmd.Parameters.AddWithValue("@Genero", genero);
            insertCmd.ExecuteNonQuery();

            Console.WriteLine("Usuario registrado correctamente.");
            return true;
        }

        public static void InsertarAdminSiNoExiste()
        {
            using var conn = Database.GetConnection();
            string checkQuery = "SELECT COUNT(*) FROM Usuarios WHERE Email = 'admin@gestorcaballos.com'";
            using var checkCmd = new MySqlCommand(checkQuery, conn);
            int count = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (count == 0)
            {
                string hash = BCrypt.Net.BCrypt.HashPassword("admin1234");
                string insert = @"INSERT INTO Usuarios (Nombre, Apellido, Email, PasswordHash, Rol, FechaNacimiento, Genero) 
                                  VALUES ('Admin', 'Principal', 'admin@gestorcaballos.com', @Hash, 'Administrador', '1980-01-01', 'Otro')";
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
}
