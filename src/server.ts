import bodyParser from "body-parser";
import * as socketio from "socket.io";
import { SERVICE_USER_NAME } from "./constant/serviceUserName";
import { SOCKET_COMMAND } from "./constant/socketCommand";
import { AbstractServer } from "./abstractServer";

const PORT = 3001;

export class Server extends AbstractServer {
  public start = () => {
    this.app.use(bodyParser.json());
    this.onConnection();

    this.server.listen(PORT, () => console.log(`Server has started.`));
  };

  private onConnection = () => {
    this.io.on(SOCKET_COMMAND.connection, (socket) => {
      this.onJoin(socket);

      this.listenMessages(socket);

      this.onDisconnect(socket);
    });
  };

  private onJoin = (socket: socketio.Socket) => {
    socket.on(SOCKET_COMMAND.join, (name: string, callback?: () => void) => {
      try {
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
  };

  private listenMessages = (socket: socketio.Socket) => {
    socket.on(SOCKET_COMMAND.sendMessage, (messageText: string) => {
      const user = this.users.getUserById(socket.id);

      socket.broadcast.emit(SOCKET_COMMAND.message, {
        username: user.name,
        text: messageText,
      });
    });
  };

  private onDisconnect = (socket: socketio.Socket) => {
    socket.on(SOCKET_COMMAND.disconnect, () => {
      const user = this.users.removeUser(socket.id);
      if (user) {
        this.io.emit(SOCKET_COMMAND.message, {
          username: SERVICE_USER_NAME.admin,
          text: `User ${user.name} has left the chat`,
        });
      }
    });
  };
}
