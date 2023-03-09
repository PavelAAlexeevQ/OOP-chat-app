using System;
using System.Threading.Tasks;

namespace ChatApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new ChatHub();
            server.Start();
            Console.WriteLine("Server has started.");
            Console.ReadLine();
        }
    }
}