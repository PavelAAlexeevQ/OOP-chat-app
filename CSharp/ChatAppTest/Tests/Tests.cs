using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Tests
{
    public class ChatAppTests
    {
        private const string SOCKET_CLIENT_ADDRESS = "http://localhost:52916";
        private HubConnection connection;

        [Fact]
        public async Task JoinTheChatSuccessfully()
        {
            // arrange
            connection = new HubConnectionBuilder().WithUrl(SOCKET_CLIENT_ADDRESS).Build();
            await connection.StartAsync();
            var username = new System.Random().ToString();

            // act
            await connection.InvokeAsync("Join", username);

            // assert
            connection.On("ServiceMessage", (string message) =>
            {
                Assert.Equal("You joined the chat", message);
            });
        }

        [Fact]
        public async Task ErrorJoiningTheChat()
        {
            // arrange
            connection = new HubConnectionBuilder().WithUrl(SOCKET_CLIENT_ADDRESS).Build();
            await connection.StartAsync();
            var username = new Random().ToString();
            var username2 = new Random().ToString();

            // act
            await connection.InvokeAsync("Join", username);
            await connection.InvokeAsync("Join", username);

            // assert
            connection.On("UsernameError", (string message) =>
            {
                Assert.Equal($"User {username} is already exists", message);
            });
        }

        [Fact]
        public async Task GetMessageFromAnotherUser()
        {
            // arrange
            connection = new HubConnectionBuilder().WithUrl(SOCKET_CLIENT_ADDRESS).Build();
            await connection.StartAsync();
            var username = new Random().ToString();
            var username2 = new Random().ToString();

            // act
            await connection.InvokeAsync("Join", username2);

            connection.On("ServiceMessage", (string message) =>
            {
                Assert.Equal("You joined the chat", message);
            });

            var message = "Test message";
            await connection.InvokeAsync("SendMessage", username, message);

            // assert
            connection.On("Message", (string usernameReceived, string messageReceived) =>
            {
                Assert.Equal(username, usernameReceived);
                Assert.Equal(message, messageReceived);
            });
        }
    }
}