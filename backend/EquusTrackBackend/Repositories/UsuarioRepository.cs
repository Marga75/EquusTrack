using MySql.Data.MySqlClient;
using EquusTrackBackend.Models;

namespace EquusTrackBackend.Repositories
{
    public class UsuarioRepository
    {
        // Método auxiliar para mapear un usuario desde un MySqlDataReader
        private static Usuario MapearUsuario(MySqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetInt32("Id"),
                Nombre = reader.GetString("Nombre"),
                Apellido = reader.GetString("Apellido"),
                Email = reader.GetString("Email"),
                Rol = reader.GetString("Rol"),
                FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                Genero = reader.GetString("Genero"),
                FotoUrl = reader.IsDBNull(reader.GetOrdinal("FotoUrl")) ? null : reader.GetString("FotoUrl"),
            };
        }

        // Valida el login verificando el email y la contraseña y devuelve el usuario si es correcto
        public static Usuario? ValidarLogin(string email, string password)
        {
            try
            {
                using var conn = Database.GetConnection();
                conn.Open();

                string query = @"SELECT * FROM Usuarios WHERE Email = @Email";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string hash = reader.GetString("PasswordHash");
                    if (BCrypt.Net.BCrypt.Verify(password, hash))
                    {
                        return MapearUsuario(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ValidarLogin: " + ex.Message);
            }

            return null;
        }

        // Registra un nuevo usuario si el email no está en uso y el rol es válido
        public static bool RegistrarUsuario(string nombre, string apellido, string email, string password, string rol, DateTime fechaNacimiento, string genero, string fotoUrl)
        {
            if (rol != "Jinete" && rol != "Entrenador")
            {
                Console.WriteLine("Rol no válido.");
                return false;
            }

            try
            {
                using var conn = Database.GetConnection();
                conn.Open();

                // Verifica si el email ya existe
                string checkQuery = "SELECT COUNT(*) FROM Usuarios WHERE Email = @Email";
                using var checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@Email", email);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count > 0)
                {
                    Console.WriteLine("Ya existe un usuario con ese correo.");
                    return false;
                }

                // Crea un nuevo email
                string hash = BCrypt.Net.BCrypt.HashPassword(password);
                string insertQuery = @"INSERT INTO Usuarios 
                    (Nombre, Apellido, Email, PasswordHash, Rol, FechaNacimiento, Genero, FotoUrl)
                    VALUES (@Nombre, @Apellido, @Email, @Hash, @Rol, @FechaNacimiento, @Genero, @FotoUrl)";
                using var insertCmd = new MySqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@Nombre", nombre);
                insertCmd.Parameters.AddWithValue("@Apellido", apellido);
                insertCmd.Parameters.AddWithValue("@Email", email);
                insertCmd.Parameters.AddWithValue("@Hash", hash);
                insertCmd.Parameters.AddWithValue("@Rol", rol);
                insertCmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento);
                insertCmd.Parameters.AddWithValue("@Genero", genero);
                insertCmd.Parameters.AddWithValue("@FotoUrl", fotoUrl);
                insertCmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al registrar usuario: " + ex.Message);
                return false;
            }
        }

        // Inserta un administrador por defecto si no existe ya en la base de datos
        public static void InsertarAdminSiNoExiste()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar administrador: " + ex.Message);
            }
        }

        // Devuelve una lista con todos los usuarios cuyo rol es 'Entrenador'
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
                entrenadores.Add(MapearUsuario(reader));
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
            {
                return false; // Ya existe
            }

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
                return MapearUsuario(reader);
            }

            return null;
        }

        // Obtener solicitudes pendientes del entrenador (Estado = 'pendiente')
        public static List<Usuario> ObtenerSolicitudesEntrenador(int idEntrenador)
        {
            List<Usuario> solicitudes = new List<Usuario>();

            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"SELECT u.*
                             FROM Usuarios u
                             JOIN RelEntrenadorAlumno r ON r.IdAlumno = u.Id
                             WHERE r.IdEntrenador = @IdEntrenador AND r.Estado = 'pendiente'";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@IdEntrenador", idEntrenador);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                solicitudes.Add(MapearUsuario(reader));
            }

            return solicitudes;
        }

        // Obtener alumnos aceptados del entrenador (Estado = 'aceptado')
        public static List<Usuario> ObtenerAlumnosEntrenador(int idEntrenador)
        {
            List<Usuario> alumnos = new List<Usuario>();

            using var conn = Database.GetConnection();
            conn.Open();

            string query = @"SELECT u.*
                             FROM Usuarios u
                             JOIN RelEntrenadorAlumno r ON r.IdAlumno = u.Id
                             WHERE r.IdEntrenador = @IdEntrenador AND r.Estado = 'aceptado'";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@IdEntrenador", idEntrenador);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                alumnos.Add(MapearUsuario(reader));
            }

            return alumnos;
        }


    }
}
