import { io } from "socket.io-client";
import { ConsoleReader } from "./consoleReader";
import { IdleTimer } from "./idleTimer";

const SOCKET_ADDRESS = "http://localhost:3001";
const TIMEOUT = 10 * 1000 * 60;

export abstract class AbstractClient {
  protected socket = io(SOCKET_ADDRESS);
  protected consoleReader = new ConsoleReader();
  protected idleTimer = new IdleTimer(() => process.exit(), TIMEOUT);

  abstract start: () => Promise<void>;
}
