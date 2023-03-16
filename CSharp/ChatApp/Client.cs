using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

using ChatApp.Interface;

namespace ChatApp
{
    public class Client
    {
        private bool isJoined = false;
        private readonly HubConnection connection;

        public Client(string url)
        {
            this.connection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();
        }

        public async Task Start()
        {
            await this.connection.StartAsync();

            await this.GetUsername();
            await this.HandleUsernameError();

            this.ListenIncomingMessages();
            await this.OnMessageSent();
        }

        private async Task GetUsername()
        {
            Console.Write("Please enter your username: ");
            string username = Console.ReadLine().Trim();

            await this.connection.InvokeAsync("join", username);
            this.isJoined = true;
        }

        private void OnMessage(Message message)
        {
            Console.Write($"\r\x1b[K{message.username}: {message.text}\n> ");
        }

        private async Task HandleUsernameError()
        {
            this.connection.On<Message>("usernameError", async (message) =>
            {
                this.OnMessage(message);
                await this.GetUsername();
            });
        }

        private void ListenIncomingMessages()
        {
            this.connection.On<Message>("message", (message) =>
            {
                if (this.isJoined)
                {
                    this.OnMessage(message);
                }
            });

            this.connection.On<Message>("serviceMessage", (message) =>
            {
                this.OnMessage(message);
            });
        }

        private async Task OnMessageSent()
        {
            Console.Write("> ");
            while (true)
            {
                string text = Console.ReadLine().Trim();
                await this.connection.InvokeAsync("sendMessage", text);
                Console.Write("> ");
            }
        }
    }

    /*class Program
    {
        static async Task Main(string[] args)
        {
            string url = "http://localhost:3001/chatHub";
            var client = new Client(url);
            await client.Start();
        }
    }*/
}