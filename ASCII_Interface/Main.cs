using System;
using System.Text;

namespace CLI.ASCII_Interface
{
    internal class Main
    {
        public Main()
        {
            Console.CursorVisible = false;
            currentSelection = 0;

            PrintHeader();
            PrintCommands();
            configF = new ConfigFile.Main();

        }

        /// <summary>
        /// Contains the top of the interface.
        /// </summary>
        private readonly string[] header = new[]{
                            @"  +----------------------------------------+",
                            @"  |      Iconoclast Translation Tool       |",
                            @"  |       Version 1.00 (27 MAR 2020)       |",
                            @"  |              by Liquid S!              |",
                            @"  +----------------------------------------+",
                            @""
         };

        /// <summary>
        /// Tells the user how to move through the menu.
        /// </summary>
        private readonly string[] commands = new[]{
                            @"         Use UP, DOWN and ENTER to move",
                            @"               through the menu.",
                            @""
         };

        /// <summary>
        /// Contains the options from the Main Menu.
        /// </summary>
        private readonly string[] mainMenu = new[]{
                            @"  +----------------------------------------+",
                            @"       Extract text",
                            @"       Repack text",
                            @"       Change Folder",
                            @"       Save options",
                            @"       Load options",
                            @"       Exit",
                            @"  +----------------------------------------+",
                            @""
         };

        /// <summary>
        /// Its value determine what option should be highlighted.
        /// </summary>
        private int currentSelection;

        /// <summary>
        /// Changes to "true" when the user press "Enter". This boolean is used by "PrintMainMenu" to do the action chosen by the user only after the Interface has been printed.
        /// </summary>
        private bool doAction = false;

        /// <summary>
        /// Contains all the options saved from the user.
        /// </summary>
        private readonly ConfigFile.Main configF;

        public void PrintHeader()
        {
            Console.WriteLine(string.Join("\n", header));
        }

        public void PrintCommands()
        {
            Console.WriteLine(string.Join("\n", commands));
        }

        /// <summary>
        /// Print to console the FULL ASCII interface.
        /// </summary>
        public void PrintFullInterface(ConsoleKey keyPressedByUser)
        {
            Console.Clear();
            PrintHeader();
            PrintCommands();
            PrintMainMenu(keyPressedByUser);
        }

        /// <summary>
        /// Print to console the Main Menu.
        /// </summary>
        /// <param name="keyPressedByUser">The key pressed by the user.</param>
        public void PrintMainMenu(ConsoleKey keyPressedByUser)
        {
            doAction = false;

            UpdatePositionOrExecuteOption(keyPressedByUser, mainMenu.Length);

            PrintMenuAndHighlightFocusedOption(mainMenu);

            if (doAction)
            {
                ExecuteSelectedOption();
            }
        }

        /// <summary>
        /// Print to console the menu and highlight the current focused option.
        /// </summary>
        /// <param name="menu">Menu where the user is.</param>
        private void PrintMenuAndHighlightFocusedOption(string[] menu)
        {
            for (int i = 0; i < menu.Length; i++)
            {
                if (i == currentSelection)
                {
                    // Highlight the focused option.
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;

                    StringBuilder sb = new StringBuilder(menu[i]);
                    sb[3] = '-';
                    sb[4] = '-';
                    sb[5] = '>';

                    Console.Write(sb.ToString() + "\n");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(mainMenu[i] + "\n");
                }
            }
        }

        /// <summary>
        /// Update "currentSelection" or execute the option chosen by the user. 
        /// </summary>
        /// <param name="keyPressedByUser"></param>
        /// <param name="menuSize"></param>
        private void UpdatePositionOrExecuteOption(ConsoleKey keyPressedByUser, int menuSize)
        {
            switch (keyPressedByUser)
            {
                case ConsoleKey.UpArrow:
                    {
                        currentSelection = (currentSelection == 1) ? menuSize - 3 : currentSelection - 1;
                        break;
                    }

                case ConsoleKey.DownArrow:
                    {
                        currentSelection = (currentSelection == menuSize - 3) ? 1 : currentSelection + 1;
                        break;
                    }

                case ConsoleKey.Enter:
                    {
                        doAction = true;
                        break;
                    }
            }
        }

        private void ExecuteSelectedOption()
        {
            switch (currentSelection)
            {
                case 1:
                    IO_ASCII.PrintOutput.EventMessage("Wait...");
                    Iconoclast.Dia originalDiaFile = new Iconoclast.Dia("dia");
                    Iconoclast.PoFormat filePo = new Iconoclast.PoFormat(originalDiaFile.Speakers, originalDiaFile.Sentences, originalDiaFile.GameCode);
                    filePo.MakePo();
                    IO_ASCII.PrintOutput.EventMessage("Done!");
                    break;
                case 2:
                    IO_ASCII.PrintOutput.EventMessage("Wait...");
                    
                    Iconoclast.PoFormat translatedPo = new Iconoclast.PoFormat(System.IO.Path.Combine("Extracted text", "Iconoclast.po"));
                    Iconoclast.Dia newlDiaFile = new Iconoclast.Dia(translatedPo.Speakers, translatedPo.Sentences, translatedPo.GameCode);
                    newlDiaFile.BuildDia();

                    IO_ASCII.PrintOutput.EventMessage("Done!");
                    break;
                case 3:
                    configF.Options.SetPoFolderPath();
                    break;
                case 4:
                    configF.SaveFile();
                    break;
                case 5:
                    configF.LoadFile();
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
