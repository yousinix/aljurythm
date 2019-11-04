using System;

namespace Aljurythm
{
    internal static class Logger
    {
        internal static void Write(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        internal static void WriteLine(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            Write($"{message}\n", color);
        }

        internal static void LineBreak()
        {
            Console.WriteLine();
        }

        internal static void Clear()
        {
            Console.Clear();
        }

        internal static void ClearLast()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine(new string(' ', Console.WindowWidth));
        }
    }
}