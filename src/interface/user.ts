import { SERVICE_USER_NAME } from "../constant/serviceUserName";

interface User {
  id: string;
  name: string;
}

export class Users {
  private usersByIdMap: Map<string, User> = new Map();
  private activeUserNamesSet: Set<string> = new Set();
  private reservedUserNamesSet = new Set<string>([SERVICE_USER_NAME.admin]);

  public addUser = (id: string, name: string): User => {
    name = name.trim().toLowerCase();

    const isNameReserved = this.reservedUserNamesSet.has(name);

    if (isNameReserved) {
      throw new Error(`Name ${name} cannot be used as a username`);
    }

    const isUserExists = this.activeUserNamesSet.has(name);

    if (isUserExists) {
      throw new Error(`User ${name} is already exists`);
    }
    const user = { id, name };

    this.usersByIdMap.set(id, { id, name });
    this.activeUserNamesSet.add(name);
    return user;
  };

  public removeUser = (id: string): User | undefined => {
    const user = this.usersByIdMap.get(id);

    if (user != null) {
      this.activeUserNamesSet.delete(user.name);
      return user;
    }
  };

  public getUserById = (id: string): User => {
    const user = this.usersByIdMap.get(id);

    if (user == null) {
      throw new Error(`User ${id} is not exists`);
    }

    return user;
  };

  public getAllUsers = () => Array.from(this.usersByIdMap.values());
}
