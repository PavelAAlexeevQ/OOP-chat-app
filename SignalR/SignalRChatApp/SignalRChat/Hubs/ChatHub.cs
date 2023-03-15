using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage2(string user, string message)
    {
        //await Clients.All.SendAsync("ReceiveMessage2", user, message);
        //await Clients.User(user).SendAsync("ReceiveMessage2", message);// .SendAsync("ReceiveMessage2", user, message);
        await Clients.Others.SendAsync("ReceiveMessage2", user, message);
    }
}