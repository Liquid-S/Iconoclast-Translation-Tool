using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Iconoclast
{
    public class PoSpeaker
    {
        private readonly string fileName;
        private readonly string destinationDir;
        private string NewPOAddress => Path.Combine(destinationDir, fileName);

        private List<string> originalSpeakers;

        private List<string> translatedSpeakers;

        public PoSpeaker()
        {
            destinationDir = "Extracted text";
            fileName = "Speakers.po";
        }

        public PoSpeaker(List<string> originalSpeakers)
        {
            destinationDir = "Extracted text";
            fileName = "Speakers.po";
            this.originalSpeakers = originalSpeakers.Distinct().ToList();
            MakePo();
        }

        public void MakePo()
        {
            if (File.Exists(NewPOAddress))
            {
                return;
            }

            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            //Read the language used by the user' OS, this way the editor can spellcheck the translation.
            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;

            Po po = new Po
            {
                Header = new PoHeader("Iconoclast", "your_email", currentCulture.Name)
            };

            for (int i = 0; i < originalSpeakers.Count; i++)
            {
                PoEntry entry = new PoEntry();

                entry.Context = $"{i + 1:D4}";
                entry.Original = originalSpeakers[i];

                po.Add(entry);
            }

            ((BinaryFormat)ConvertFormat.With<Po2Binary>(po)).Stream.WriteTo(NewPOAddress);
        }

        public void ReadPo()
        {
            translatedSpeakers = new List<string>();
            originalSpeakers = new List<string>();

            Po poFile = null;

            using (DataStream ds = DataStreamFactory.FromFile(NewPOAddress, FileOpenMode.Read))
            using (BinaryFormat bf = new BinaryFormat(ds))
            {
                poFile = (Po)ConvertFormat.With<Po2Binary>(bf);
            }

            foreach (PoEntry entry in poFile.Entries)
            {
                originalSpeakers.Add(entry.Original);

                if (entry.Translated.Trim() != string.Empty && entry.Translated.Trim().Length > 1)
                {
                    translatedSpeakers.Add(entry.Translated);
                }
                else
                {
                    translatedSpeakers.Add(entry.Original);
                }
            }
        }

        public string TranslateSpeaker(string speaker)
        {
            var index = originalSpeakers.IndexOf(speaker);

            return index < 0 ? speaker : translatedSpeakers[index];
        }
    }
}
