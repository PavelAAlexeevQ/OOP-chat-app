using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
//using System.Net.WebSockets;
using Quobject.SocketIoClientDotNet.Client;

using ChatApp.Interface;


namespace ChatApp
{
    public abstract class AbstractServer
    {
        protected IApplicationBuilder app;
        protected IWebHost host;
        protected IServiceProvider serviceProvider;

        public virtual void Start()
        {
            host = new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<Users>();
                })
                .Configure(app =>
                {
                    this.app = app;

                    // Add Socket.IO middleware
                    app.UseWebSockets();
                    app.Use(SocketIoMiddleware);

                    // Add custom middleware
                    Configure(app);
                })
                .Build();

            serviceProvider = host.Services;
            host.Run();
        }

        protected virtual void Configure(IApplicationBuilder app) { }

        private async Task SocketIoMiddleware(HttpContext context, Func<Task> next)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var socket = await context.WebSockets.AcceptWebSocketAsync();

                // Add socket handling logic
                await HandleSocket(socket);
            }
            else
            {
                await next();
            }
        }

        protected virtual async Task HandleSocket(Socket socket)
        {
            // Add socket handling logic
        }
    }
}
