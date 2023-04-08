using System.IO;
using System.Text;

namespace Iconoclast
{
    class Rosetta
    {
        public string[] Stone { get; private set; }

        public Rosetta(string fileName)
        {
            using FileStream Rosetta = new FileStream("Rosetta.txt", FileMode.Open, FileAccess.Read);
            using StreamReader Rose = new StreamReader(Rosetta, Encoding.UTF8);

            string txt = Rose.ReadToEnd();
            txt = txt.Replace("\r\n", "\0");
            Stone = txt.Split('\0');
        }
    }
}
