using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using ChatApp.Interface;
using ChatApp.Constant;

namespace ChatApp
{
    public abstract class AbstractServer : Hub
    {
        protected readonly IWebHostBuilder builder;
        protected readonly Users users = new Users();

        public AbstractServer()
        {
            builder = new WebHostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices(services =>
                {
                    services.AddSignalR();
                })
                .Configure(app =>
                {
                    app.UseRouting();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHub<YourHub>("/yourhub");
                    });
                });
        }

        public abstract void Start();
    }

    public class YourHub : Hub
    {
        // Your hub implementation goes here
    }
}
