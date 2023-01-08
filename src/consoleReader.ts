import * as readline from "readline";

export class ConsoleReader {
  private userInput = readline.createInterface({
    input: process.stdin,
    output: process.stdout,
  });

  public async readUserInput(
    query: string,
    callback: (response: string) => void
  ) {
    const enteredText = await new Promise((resolve) =>
      this.userInput.question(query, (response: string) => {
        callback(response);
        resolve(response);
      })
    );

    return enteredText;
  }

  public async prompt() {
    this.userInput.prompt();
  }

  public async onLine(fn: (text: string) => void) {
    this.userInput.on("line", (text: string) => {
      fn(text);
      this.userInput.prompt(true);
    });
  }
}
