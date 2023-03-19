using System;

namespace CLI
{
    internal class Program
    {
        private static void Main()
        {
            Console.CursorVisible = true;
            Console.ResetColor();

            ASCII_Interface.Main consoleInterface = new ASCII_Interface.Main();

            consoleInterface.PrintFullInterface(ConsoleKey.DownArrow);

            while (true)
            {
                ConsoleKey keyPressedByUser = IO_ASCII.ReadInput.WaitForArrowKeys();
                consoleInterface.PrintFullInterface(keyPressedByUser);
            }
        }
    }
}