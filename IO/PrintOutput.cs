using System;

namespace IO_ASCII
{
    public class PrintOutput
    {
        /// <summary>
        /// Print an error message.
        /// </summary>
        /// <param name="message">The text's message you want to show.</param>
        public static void ErrorMessage(string message)
        {
            Console.WriteLine($"Error: {message}\n");
        }

        /// <summary>
        /// Print an "ok" message.
        /// </summary>
        /// <param name="message">The text's message you want to show.</param>
        public static void EventMessage(string message)
        {
            Console.WriteLine($"{message}\n");
        }
    }
}
