using MySql.Data.MySqlClient;
using EquusTrackBackend.Models;

namespace EquusTrackBackend.Repositories
{
    public class UsuarioRepository
    {
        public static Usuario? ValidarLogin(string email, string password)
        {
            using var conn = Database.GetConnection();
            conn.Open();

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
            conn.Open();

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
            conn.Open();

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
            conn.Open();

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

        // Obtener todos los entrenadores registrados
        public static List<Usuario> ObtenerTodosEntrenadores()
        {
            var entrenadores = new List<Usuario>();
            using var conn = Database.GetConnection();
            conn.Open();

            string query = "SELECT * FROM Usuarios WHERE Rol = 'Entrenador'";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                entrenadores.Add(new Usuario
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Email = reader.GetString("Email"),
                    Rol = reader.GetString("Rol"),
                    FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                    Genero = reader.GetString("Genero")
                });
            }

            return entrenadores;
        }

        // Crear una solicitud de relación entre jinete y entrenador (estado pendiente)
        public static bool SolicitarRelacionJineteEntrenador(int idJinete, int idEntrenador)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            // Verificar si ya existe una relación (pendiente o aceptada)
            string checkQuery = @"SELECT COUNT(*) FROM RelEntrenadorAlumno 
              WHERE IdEntrenador = @IdEntrenador AND IdAlumno = @IdAlumno";
            using var checkCmd = new MySqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@IdEntrenador", idEntrenador);
            checkCmd.Parameters.AddWithValue("@IdAlumno", idJinete);
            int count = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (count > 0)
                return false; // Ya existe

            // Insertar nueva relación como pendiente
            string insertQuery = @"INSERT INTO RelEntrenadorAlumno (IdEntrenador, IdAlumno, Estado)
               VALUES (@IdEntrenador, @IdAlumno, 'pendiente')";
            using var insertCmd = new MySqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@IdEntrenador", idEntrenador);
            insertCmd.Parameters.AddWithValue("@IdAlumno", idJinete);
            int rowsAffected = insertCmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        // Aceptar o rechazar una solicitud de relación (por el entrenador)
        public static bool ActualizarEstadoRelacion(int idJinete, int idEntrenador, string nuevoEstado)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            // Actualizar estado
            string updateQuery = @"UPDATE RelEntrenadorAlumno 
               SET Estado = @Estado 
               WHERE IdEntrenador = @IdEntrenador AND IdAlumno = @IdAlumno";
            using var cmd = new MySqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
            cmd.Parameters.AddWithValue("@IdEntrenador", idEntrenador);
            cmd.Parameters.AddWithValue("@IdAlumno", idJinete);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        // Obtener el entrenador aceptado de un jinete
        public static Usuario? ObtenerEntrenadorDeJinete(int idJinete)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            // Obtener entrenador aceptado
            string query = @"SELECT u.* FROM Usuarios u
         JOIN RelEntrenadorAlumno r ON r.IdEntrenador = u.Id
         WHERE r.IdAlumno = @IdAlumno AND r.Estado = 'aceptado'";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@IdAlumno", idJinete);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
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

            return null;
        }

        // Obtener solicitudes pendientes del entrenador (Estado = 'pendiente')
        public static List<Usuario> ObtenerSolicitudesEntrenador(int idEntrenador)
        {
            List<Usuario> solicitudes = new List<Usuario>();

            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"
        SELECT u.*
        FROM Usuarios u
        JOIN RelEntrenadorAlumno r ON r.IdAlumno = u.Id
        WHERE r.IdEntrenador = @IdEntrenador AND r.Estado = 'pendiente'";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@IdEntrenador", idEntrenador);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                solicitudes.Add(new Usuario
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Email = reader.GetString("Email"),
                    Rol = reader.GetString("Rol"),
                    FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                    Genero = reader.GetString("Genero")
                });
            }

            return solicitudes;
        }

        // Obtener alumnos aceptados del entrenador (Estado = 'aceptado')
        public static List<Usuario> ObtenerAlumnosEntrenador(int idEntrenador)
        {
            List<Usuario> alumnos = new List<Usuario>();

            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"
        SELECT u.*
        FROM Usuarios u
        JOIN RelEntrenadorAlumno r ON r.IdAlumno = u.Id
        WHERE r.IdEntrenador = @IdEntrenador AND r.Estado = 'aceptado'";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@IdEntrenador", idEntrenador);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                alumnos.Add(new Usuario
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Email = reader.GetString("Email"),
                    Rol = reader.GetString("Rol"),
                    FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                    Genero = reader.GetString("Genero")
                });
            }

            return alumnos;
        }


    }
}
