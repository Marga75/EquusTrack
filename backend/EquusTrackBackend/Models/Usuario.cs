
namespace EquusTrackBackend.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Email { get; set; }
        public required string Rol { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public required string Genero { get; set; }
    }
}
