using System;
using System.IO;

namespace IO_ASCII
{
    public static class ReadInput
    {
        /// <summary>
        /// Ask to the user to insert the absolute path for the "fileType"'s folder.
        /// </summary>
        /// <param name="fileType">The files' extension you are asking to the user to search.</param>
        /// <returns>Returns the folder's path.</returns>
        public static string WaitForFolderPath(string fileType)
        {
            string folderPath;
            Console.CursorVisible = true;

            do
            {
                PrintOutput.EventMessage($"Drop or write the absolute path to the {fileType}'s folder.");
                folderPath = Console.ReadLine().Replace("\"", null);

                if (!Directory.Exists(folderPath))
                {
                    PrintOutput.ErrorMessage("Folder not found!");
                }
                else
                {
                    PrintOutput.EventMessage("Folder has been found!");

                    if (Directory.GetFiles(folderPath, "*." + fileType, SearchOption.AllDirectories).Length == 0)
                    {
                        PrintOutput.ErrorMessage($"{fileType}'s files not found!");
                    }
                }
            }
            while (!Directory.Exists(folderPath) || Directory.GetFiles(folderPath, "*." + fileType, SearchOption.AllDirectories).Length == 0);

            Console.CursorVisible = false;

            return folderPath;
        }

        /// <summary>
        /// Wait for the user to input UpArrow, DownArrow or Enter.
        /// </summary>
        /// <returns>Returns the pressed key.</returns>
        public static ConsoleKey WaitForArrowKeys()
        {
            ConsoleKey k = Console.ReadKey(true).Key;

            while (k != ConsoleKey.UpArrow && k != ConsoleKey.DownArrow && k != ConsoleKey.Enter)
            {
                k = Console.ReadKey(true).Key;
            }

            return k;
        }
    }
}
