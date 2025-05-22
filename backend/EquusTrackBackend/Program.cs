using EquusTrackBackend;
using System;

namespace EquusTrackBackend
{
    class Program
    {
        /*static void Main(string[] args)
        {
            Database.InsertarAdminSiNoExiste();

            Console.WriteLine("Registro de nuevo usuario");
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("Apellido: ");
            string apellido = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Contraseña: ");
            string password = Console.ReadLine();

            Console.Write("Rol (Jinete/Entrenador): ");
            string rol = Console.ReadLine();

            Database.RegistrarUsuario(nombre, apellido, email, password, rol);

        }*/

        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando backend...");
            //Database.InsertarAdminSiNoExiste();
            await Servidor.Iniciar();
        }

    }
}
