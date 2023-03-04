using System;
using System.Threading.Tasks;
//using Quobject.SocketIoClientDotNet.Client;
//using System.Net.Sockets.Socket
using ConsoleReader = System.Console;

using ChatApp;
using ChatApp.Constant;
using ChatApp.Interface;
using SocketIOSharp.Server.Client;

public class Client : AbstractClient
{
    private bool isJoined = false;
    public Client()
    {

    }

    public override async Task Start()
    {
        await GetUsername();
        await HandleUsernameError();

        ListenIncomingMessages();
        OnMessageSent();
    }

    private async Task GetUsername()
    {
        await ConsoleReader.Out.WriteAsync("Please enter your username: ");
        var username = ConsoleReader.In.ReadLine().Trim();
        idleTimer.ResetTimeout();
        socket.Emit(SOCKET_COMMAND.join, username);
        isJoined = true;
        ConsoleReader.Out.WriteAsync("> ");
    }

    private void OnMessage(Message message)
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.WriteLine($"{message.username}: {message.text}");
        ConsoleReader.Out.WriteAsync("> ");
    }

    private async Task HandleUsernameError()
    {
        socket.On(SOCKET_COMMAND.usernameError, async (message) =>
        {
            OnMessage((Message)message);
            await GetUsername();
        });
    }

    private void ListenIncomingMessages()
    {
        socket.On(SOCKET_COMMAND.message, (message) =>
        {
            if (isJoined)
            {
                OnMessage((Message)message);
            }
        });

        socket.On(SOCKET_COMMAND.serviceMessage, (message) =>
        {
            OnMessage((Message)message);
        });
    }

    private async Task OnMessageSent()
    {
        while (true)
        {
            var text = await ConsoleReader.In.ReadLineAsync();
            idleTimer.ResetTimeout();
            socket.Emit(SOCKET_COMMAND.sendMessage, text.Trim());
            ConsoleReader.Out.WriteAsync("> ");
        }
    }
}

class Program
{
    static async Task RunClientMain(string[] args)
    {
        var client = new Client();
        await client.Start();
    }
}