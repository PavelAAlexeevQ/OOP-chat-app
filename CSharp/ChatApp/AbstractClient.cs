using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
    
namespace ChatApp
{
    public abstract class AbstractClient
    {
        private readonly string SOCKET_ADDRESS = "http://localhost:3001";
        private readonly int TIMEOUT = 10 * 1000 * 60;

        protected HubConnection socket;
        protected ConsoleReader consoleReader = new ConsoleReader();
        protected IdleTimer idleTimer;

        public AbstractClient()
        {
            socket = new HubConnectionBuilder().WithUrl(SOCKET_ADDRESS).Build();
            idleTimer = new IdleTimer(() => Environment.Exit(0), TIMEOUT);
        }

        public abstract Task Start();
    }
}