import { createServer } from "http";
import { Server } from "socket.io";
import Client, { Socket } from "socket.io-client";
import http from "http";
import { AddressInfo } from "node:net";
import { SOCKET_COMMAND } from "../src/constant/socketCommand";

const SOCKET_CLIENT_ADDRESS = `http://localhost:3001`;

describe("Chat application testing", () => {
  let httpServer: http.Server;
  let io: Server;

  let socket: Socket;
  let socket2: Socket;

  let clientSocket: Socket;
  let clientSocket2: Socket;

  const username = Math.random().toString();
  const username2 = Math.random().toString();

  beforeAll((done) => {
    httpServer = http.createServer();
    io = new Server(httpServer);

    clientSocket = Client(SOCKET_CLIENT_ADDRESS);
    socket = clientSocket.connect();

    clientSocket2 = Client(SOCKET_CLIENT_ADDRESS);
    socket2 = clientSocket2.connect();

    socket.on("connect", () => {
      done();
    });
  });

  afterAll(() => {
    io.close();
    clientSocket.close();
    clientSocket2.close();
  });

  test("Join the chat succesfully", (done) => {
    socket.emit(SOCKET_COMMAND.join, username);

    clientSocket.on(SOCKET_COMMAND.serviceMessage, (arg) => {
      expect(arg.text).toBe("You joined the chat");
      done();
    });
  });

  test("Error joining the chat", (done) => {
    socket2.emit(SOCKET_COMMAND.join, username);

    clientSocket2.on(SOCKET_COMMAND.usernameError, (arg) => {
      expect(arg.text).toBe(`User ${username} is already exists`);
      done();
    });
  });

  test("Get message from another user", (done) => {
    socket2.emit(SOCKET_COMMAND.join, username2);

    clientSocket2.on(SOCKET_COMMAND.serviceMessage, (arg) => {
      expect(arg.text).toBe("You joined the chat");
    });

    const message = "Test message";

    socket.emit(SOCKET_COMMAND.sendMessage, message);

    clientSocket2.on(SOCKET_COMMAND.message, (arg) => {
      expect(arg.username).toBe(username);
      expect(arg.text).toBe(message);
      done();
    });
  });
});
