using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

using ChatApp.Interface;
using ChatApp.Constant;

namespace ChatApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    } 

    public class ChatHub : AbstractServer
    {
        private readonly Users _users = new Users();

        public ChatHub()
        {
            //var host = builder.Build();
        }
        public override void Start()
        {
        }

        public async Task SendMessage(string messageText)
        {
            var user = _users.GetUserById(Context.ConnectionId);
            await Clients.Others.SendAsync("messageReceived", user.name, messageText);
        }

        public async Task Join(string name)
        {
            try
            {
                var user = _users.AddUser(Context.ConnectionId, name);

                await Clients.Caller.SendAsync("serviceMessageReceived", "admin", "You joined the chat");
                await Clients.Others.SendAsync("messageReceived", "admin", $"User {user.name} has joined the chat");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("usernameError", "admin", ex.Message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = _users.RemoveUser(Context.ConnectionId);

            if (user != null)
            {
                await Clients.Others.SendAsync("messageReceived", "admin", $"User {user.name} has left the chat");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

}
