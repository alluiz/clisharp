using System.Diagnostics.CodeAnalysis;

namespace CommandLineInterface
{
    public interface IConsoleManager
    {
        void WriteLine(string text);
        void WriteLine(string text, int lineNumber);
        string ReadLine();
        void Write(string text);
        void Write(string text, int lineNumber);
        string ReadSensitive();
    }

    /*
        This code is only for wrapper static System.Console class
        Therefore, it cannot be tested.
    */
    [ExcludeFromCodeCoverage]
    public class ConsoleManager : IConsoleManager
    {
        public string ReadLine()
        {
            return System.Console.ReadLine() ?? "";
        }

        public void Write(string text, int lineNumber)
        {
            System.Console.Write($"{(lineNumber+".").PadRight(5)}{text}");
        }

        public void WriteLine(string text, int lineNumber)
        {
            System.Console.WriteLine($"{(lineNumber+".").PadRight(5)}{text}");
        }

        /// <summary>
        /// Read sensitive data from console without print at screen
        /// </summary>
        /// <remarks>
        /// It manipulate the cursor position if user press backspace.
        /// </remarks>
        /// <returns>
        /// The sensitive content
        /// </returns>
        public string ReadSensitive()
        {
            string data = "";

            ConsoleKeyInfo info = Console.ReadKey(true);

            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    data += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(data))
                    {
                        // remove one character from the list of password characters
                        data = data.Substring(0, data.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }

            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();

            return data;
        }

        public void WriteLine(string text)
        {
            System.Console.WriteLine(text);
        }

        public void Write(string text)
        {
            System.Console.Write(text);
        }
    }
}