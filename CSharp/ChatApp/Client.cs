using System;
using System.Threading.Tasks;
using SocketIOClientDotNet;
using ConsoleReader = System.Console;

using ChatApp;
using ChatApp.Constant;
using ChatApp.Interface;
public class Client : AbstractClient
{
    private bool isJoined = false;

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
        IdleTimer.ResetTimeout();
        Socket.EmitAsync("join", username);
        isJoined = true;
        ConsoleReader.Out.WriteAsync("> ");
    }

    private void OnMessage(Message message)
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.WriteLine($"{message.Username}: {message.Text}");
        ConsoleReader.Out.WriteAsync("> ");
    }

    private async Task HandleUsernameError()
    {
        Socket.On(SOCKET_COMMAND.usernameError, async (message) =>
        {
            OnMessage(message.ToObject<Message>());
            await GetUsername();
        });
    }

    private void ListenIncomingMessages()
    {
        Socket.On(SOCKET_COMMAND.message, (message) =>
        {
            if (isJoined)
            {
                OnMessage(message.ToObject<Message>());
            }
        });

        Socket.On(SOCKET_COMMAND.serviceMessage, (message) =>
        {
            OnMessage(message.ToObject<Message>());
        });
    }

    private async Task OnMessageSent()
    {
        while (true)
        {
            var text = await ConsoleReader.In.ReadLineAsync();
            IdleTimer.ResetTimeout();
            Socket.EmitAsync(SOCKET_COMMAND.sendMessage, text.Trim());
            ConsoleReader.Out.WriteAsync("> ");
        }
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var client = new Client();
        await client.Start();
    }
}