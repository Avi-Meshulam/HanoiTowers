using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanoiTowers
{
    static class Utils
    {
        internal static int GetInteger()
        {
            ConsoleKeyInfo consoleKeyInfo;
            StringBuilder input = new StringBuilder("");

            do
            {
                consoleKeyInfo = Console.ReadKey(true);
                char keyChar = consoleKeyInfo.KeyChar;
                if (char.IsNumber(keyChar))
                {
                    if (keyChar == '0' && input.ToString() == "0")
                    {
                        continue;
                    }
                    Console.Write(keyChar);
                    input.Append(keyChar);
                }
                else
                {
                    //Allow use of Backspace
                    if (consoleKeyInfo.Key == ConsoleKey.Backspace
                        && input.Length > 0)
                    {
                        Console.Write("\b \b");
                        input.Remove(input.Length - 1, 1);
                    }
                }
            } while (consoleKeyInfo.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return int.Parse(input.ToString());
        }

        /// <summary>
        /// Writes the specified string value to the standard 
        /// output stream, using the requested color.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        internal static void ColorConsoleWrite(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current 
        /// line terminator, to the standard output stream, using the requested color.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        internal static void ColorConsoleWriteLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            ColorConsoleWrite($"{message}\n", color);
        }

        /// <summary>
        /// Writes the specified yes/no question to the standard 
        /// output stream, and returns the user's answer.
        /// </summary>
        /// <param name="yesNoQuestion"></param>
        /// <returns></returns>
        internal static bool AskYesNo(string yesNoQuestion)
        {
            ColorConsoleWrite("\n" + yesNoQuestion + " (Y//N)", ConsoleColor.Red);
            char ch;
            while (true)
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                ch = char.ToLower(consoleKeyInfo.KeyChar);
                if (ch == 'y' || ch == 'n')
                {
                    Console.WriteLine();
                }
                if (ch == 'y')
                {
                    return true;
                }
                else if (ch == 'n')
                {
                    return false;
                }
            }
        }
    }
}
