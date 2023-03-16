using Microsoft.AspNetCore.Builder;
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

            var builder = WebApplication.CreateBuilder(args);
            var startup = new Startup();
            startup.ConfigureServices(builder.Services);

            var app = builder.Build();
            startup.Configure(app, builder.Environment);

            Console.WriteLine("Server has started.");
            //Console.ReadLine();

            await app.RunAsync();
            //var builder = WebApplication.CreateBuilder(args);
            //var startup = new Startup();
            //startup.ConfigureServices(builder.Services);

            //var app = builder.Build();
            //startup.Configure(app, builder.Environment);

            //await app.RunAsync();
        }
    }
}