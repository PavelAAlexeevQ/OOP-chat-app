using System;

namespace ChatApp
{
    public class ConsoleReader
    {
        private readonly System.IO.TextReader input = Console.In;
        private readonly System.IO.TextWriter output = Console.Out;

        public async void ReadUserInput(string query, Action<string> callback)
        {
            output.Write(query);
            var response = await input.ReadLineAsync();
            callback(response);
        }

        public void Prompt()
        {
            output.Flush();
        }

        public void OnLine(Action<string> fn)
        {
            while (true)
            {
                var text = input.ReadLine();
                if (text == null) break;
                fn(text);
            }
        }
    }
}
