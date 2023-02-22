using System.Threading.Tasks;

namespace ChatApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new Server();
            server.Start();
            await Task.CompletedTask;
        }
    }
}