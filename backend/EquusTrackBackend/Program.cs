using EquusTrackBackend.Repositories;

namespace EquusTrackBackend
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando backend...");
            //UsuarioRepository.InsertarAdminSiNoExiste();
            await Servidor.Iniciar();
        }

    }
}
