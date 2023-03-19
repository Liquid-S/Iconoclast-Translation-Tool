using System.IO;

namespace ConfigFile
{
    public class MainSettings
    {
        public string PoFolderPath { get; set; }

        public void SetPoFolderPath()
        {
            PoFolderPath = IO_ASCII.ReadInput.WaitForFolderPath("PO");
        }

        public void GetPoFolderPath()
        {
            if (PoFolderPath == string.Empty || !Directory.Exists(PoFolderPath) || Directory.GetFiles(PoFolderPath, "*.po", SearchOption.AllDirectories).Length == 0)
            {
                IO_ASCII.PrintOutput.ErrorMessage($"{PoFolderPath} folder not found or empty!\n");
                PoFolderPath = IO_ASCII.ReadInput.WaitForFolderPath("PO");
            }
        }

    }
}

