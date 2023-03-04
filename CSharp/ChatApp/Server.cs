using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SocketIOSharp.Server;
using SocketIOSharp.Server.Client;
using Quobject.SocketIoClientDotNet.Client;

using ChatApp.Constant;

namespace ChatApp
{
    public class Server : AbstractServer
    {
        private const int Port = 3001;
        private readonly Users users = new Users();
        private readonly SocketIOServer io = new SocketIOServer();

        public override void Start()
        {
            io.OnConnection(socket =>
            {
                OnJoin(socket);
                ListenMessages(socket);
                OnDisconnect(socket);
            });

            var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls($"http://localhost:{Port}")
                    .Configure(app => app.UseWebSockets().Run(async context => await io.ExecuteAsync(context)))
                .Build();
            host.Run();
            Console.WriteLine($"Server has started.");
        }

        private void OnJoin(Socket socket)
        {
            socket.On(SOCKET_COMMAND.join, (name, callback) =>
            {
                try
                {
                    var user = users.AddUser(socket.id, name);

                    socket.Emit(SOCKET_COMMAND.serviceMessage,
                        new { username = SERVICE_USER_NAME.admin, text = $"You joined the chat" });
                    socket.Broadcast.Emit(SOCKET_COMMAND.message,
                        new { username = SERVICE_USER_NAME.admin, text = $"User {user.Name} has joined the chat" });

                    callback?.Invoke();
                }
                catch (Exception error)
                {
                    socket.Emit(SOCKET_COMMAND.usernameError,
                        new { username = SERVICE_USER_NAME.admin, text = error.Message });
                }
            });
        }

        private void ListenMessages(SocketIOSocket socket)
        {
            socket.On(SOCKET_COMMAND.sendMessage, messageText =>
            {
                var user = users.GetUserById(socket.Id);

                socket.Broadcast.Emit(SOCKET_COMMAND.message, new { username = user.Name, text = messageText });
            });
        }

        private void OnDisconnect(SocketIOSocket socket)
        {
            socket.On(SOCKET_COMMAND.disconnect, () =>
            {
                var user = users.RemoveUser(socket.Id);
                if (user != null)
                {
                    io.Emit(SOCKET_COMMAND.message,
                        new { username = SERVICE_USER_NAME.admin, text = $"User {user.Name} has left the chat" });
                }
            });
        }
    }

    public class Users
    {
        private readonly List<User> userList = new List<User>();

        public User AddUser(string id, string name)
        {
            if (userList.Any(user => user.Name == name))
            {
                throw new Exception($"Username {name} is already taken.");
            }

            var user = new User(id, name);
            userList.Add(user);

            return user;
        }

        public User GetUserById(string id)
        {
            return userList.FirstOrDefault(user => user.Id == id);
        }

        public User RemoveUser(string id)
        {
            var user = GetUserById(id);
            if (user != null)
            {
                userList.Remove(user);
            }

            return user;
        }
    }

    public class User
    {
        public User(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }
}