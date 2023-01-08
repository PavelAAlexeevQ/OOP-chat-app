import express from "express";
import * as http from "http";
import bodyParser from "body-parser";
import * as socketio from "socket.io";
import { SERVICE_USER_NAME } from "./constant/serviceUserName";
import { Users } from "./interface/user";
import { SOCKET_COMMAND } from "./constant/socketCommand";

class Server {
  private PORT = 3001;
  private app = express();
  private server = http.createServer(this.app);
  private io = new socketio.Server(this.server);
  private users = new Users();

  public start = () => {
    this.app.use(bodyParser.json());
    this.onConnection();

    this.server.listen(this.PORT, () => console.log(`Server has started.`));
  };

  private onConnection = () => {
    this.io.on(SOCKET_COMMAND.connection, (socket) => {
      socket.on(SOCKET_COMMAND.join, (name: string, callback?: () => void) => {
        try {
          console.log("new user");
          console.log(socket.id, name);
          const user = this.users.addUser(socket.id, name);

          socket.emit(SOCKET_COMMAND.serviceMessage, {
            username: SERVICE_USER_NAME.admin,
            text: `You joined the chat`,
          });

          socket.broadcast.emit(SOCKET_COMMAND.message, {
            username: SERVICE_USER_NAME.admin,
            text: `User ${user.name} has joined the chat`,
          });

          callback?.();
        } catch (error) {
          socket.emit(SOCKET_COMMAND.usernameError, {
            username: SERVICE_USER_NAME.admin,
            text: (error as { message: string }).message,
          });
        }
      });

      socket.on(SOCKET_COMMAND.sendMessage, (messageText: string) => {
        const user = this.users.getUserById(socket.id);
        socket.broadcast.emit("message", {
          username: user.name,
          text: messageText,
        });
      });

      socket.on(SOCKET_COMMAND.disconnect, () => {
        const user = this.users.removeUser(socket.id);
        if (user) {
          this.io.emit("message", {
            username: SERVICE_USER_NAME.admin,
            text: `User ${user.name} has left the chat`,
          });
        }
      });
    });
  };
}

(async () => {
  const server = new Server();
  server.start();
})();
