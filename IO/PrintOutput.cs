using System;

namespace IO_ASCII
{
    public class PrintOutput
    {
        /// <summary>
        /// Print an error message.
        /// </summary>
        /// <param name="Message">The text's message you want to show.</param>
        public static void ErrorMessage(string Message)
        {
            Console.WriteLine($"Error: {Message}\n");
        }

        /// <summary>
        /// Print an "ok" message.
        /// </summary>
        /// <param name="Message">The text's message you want to show.</param>
        public static void EventMessage(string Message)
        {
            Console.WriteLine($"{Message}\n");
        }
    }
}
