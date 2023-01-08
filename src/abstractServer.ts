import express from "express";
import * as http from "http";
import * as socketio from "socket.io";
import { Users } from "./interface/user";

export abstract class AbstractServer {
  protected app = express();
  protected server = http.createServer(this.app);
  protected io = new socketio.Server(this.server);
  protected users = new Users();

  abstract start: () => void;
}
