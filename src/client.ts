import { io } from "socket.io-client";
import { ConsoleReader } from "./consoleReader";
import { SOCKET_COMMAND } from "./constant/socketCommand";
import { Message } from "./interface/message";
import { IdleTimer } from "./idleTimer";

const SOCKET_ADDRESS = "http://localhost:3001";
const TIMEOUT = 10 * 1000 * 60;

class Client {
  private socket = io(SOCKET_ADDRESS);
  private consoleReader = new ConsoleReader();
  private idleTimer = new IdleTimer(() => process.exit(), TIMEOUT);
  private isJoined = false;

  private async getUsername() {
    await this.consoleReader.readUserInput(
      "Please enter your username: ",
      (text) => {
        this.idleTimer.resetTimeout();
        this.socket.emit("join", text.trim(), () => (this.isJoined = true));
        this.consoleReader.prompt();
      }
    );
  }

  private onMessage = (message: Message) => {
    process.stdout.write("\r\x1b[K");
    console.log(`${message.username}: ${message.text}`);
    process.stdout.write("> ");
  };

  public async start() {
    this.socket.on(SOCKET_COMMAND.message, (message: Message) => {
      if (this.isJoined) {
        this.onMessage(message);
      }
    });

    await this.getUsername();

    this.socket.on(SOCKET_COMMAND.serviceMessage, async (message: Message) => {
      this.onMessage(message);
    });

    this.socket.on(SOCKET_COMMAND.usernameError, async (message: Message) => {
      this.onMessage(message);
      await this.getUsername();
    });

    await this.consoleReader.onLine((text) => {
      this.idleTimer.resetTimeout();
      this.socket.emit(SOCKET_COMMAND.sendMessage, text.trim());
      process.stdout.write("> ");
    });
  }
}

(async () => {
  const client = new Client();
  client.start();
})();
