using System;
using System.IO;
using System.Text.Json;

namespace ConfigFile
{
    public class Main
    {
        private readonly string configPath;
        public MainSettings Options;
        public Main(string jsonPath = "AppSettings.json")
        {
            configPath = jsonPath;

            LoadFile();
            System.Threading.Thread.Sleep(4000);
        }

        public void SaveFile()
        {
            try
            {
                byte[] utf8Json = JsonSerializer.SerializeToUtf8Bytes(Options);

                using (FileStream fs = new FileStream(configPath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(utf8Json);
                }

                IO_ASCII.PrintOutput.EventMessage($"\"{configPath}\" has been saved!");
            }
            catch (IOException e)
            {
                IO_ASCII.PrintOutput.ErrorMessage(e.Message);
            }
        }

        public void LoadFile()
        {
            try
            {
                byte[] utf8Json;

                using (FileStream fs = new FileStream(configPath, FileMode.Open, FileAccess.Read))
                {
                    utf8Json = new byte[fs.Length];
                    fs.Read(utf8Json, 0, utf8Json.Length);
                }

                //var utf8Reader = new Utf8JsonReader(utf8Json);
                ReadOnlySpan<byte> readOnlySpan = new ReadOnlySpan<byte>(utf8Json);
                Options = JsonSerializer.Deserialize<MainSettings>(readOnlySpan);

                IO_ASCII.PrintOutput.EventMessage($"\"{configPath}\" has been loaded!");
            }
            catch (FileNotFoundException e)
            {
                IO_ASCII.PrintOutput.ErrorMessage(e.Message);
                Options = new MainSettings();
            }
            catch (IOException e)
            {
                IO_ASCII.PrintOutput.ErrorMessage(e.Message);
                Options = new MainSettings();
            }
        }
    }








}




