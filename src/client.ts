import { AbstractClient } from "./abstractClient";
import { SOCKET_COMMAND } from "./constant/socketCommand";
import { Message } from "./interface/message";

class Client extends AbstractClient {
  private isJoined = false;

  public start = async () => {
    await this.getUsername();
    await this.handleUsernameError();

    this.listenIncomingMessages();
    this.onMessageSent();
  };

  private getUsername = async () => {
    await this.consoleReader.readUserInput(
      "Please enter your username: ",
      (text) => {
        this.idleTimer.resetTimeout();
        this.socket.emit("join", text.trim(), () => (this.isJoined = true));
        this.consoleReader.prompt();
      }
    );
  };

  private onMessage = (message: Message) => {
    process.stdout.write("\r\x1b[K");
    console.log(`${message.username}: ${message.text}`);
    process.stdout.write("> ");
  };

  private handleUsernameError = async () => {
    this.socket.on(SOCKET_COMMAND.usernameError, async (message: Message) => {
      this.onMessage(message);
      await this.getUsername();
    });
  };

  private listenIncomingMessages = () => {
    this.socket.on(SOCKET_COMMAND.message, (message: Message) => {
      if (this.isJoined) {
        this.onMessage(message);
      }
    });

    this.socket.on(SOCKET_COMMAND.serviceMessage, async (message: Message) => {
      this.onMessage(message);
    });
  };

  private onMessageSent = async () => {
    await this.consoleReader.onLine((text) => {
      this.idleTimer.resetTimeout();
      this.socket.emit(SOCKET_COMMAND.sendMessage, text.trim());
      process.stdout.write("> ");
    });
  };
}

(async () => {
  const client = new Client();
  client.start();
})();
