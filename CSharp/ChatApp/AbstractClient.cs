using System;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;

namespace ChatApp
{
    public abstract class AbstractClient
    {
        private const string SOCKET_ADDRESS = "http://localhost:3001";
        private const int TIMEOUT = 10 * 1000 * 60;

        protected readonly Socket socket = IO.Socket(SOCKET_ADDRESS);
        protected readonly ConsoleReader consoleReader = new ConsoleReader();
        protected readonly IdleTimer idleTimer = new IdleTimer(() => Environment.Exit(0), TIMEOUT);

        public abstract Task Start();
    }
}