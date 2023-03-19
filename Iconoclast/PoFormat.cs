using System.Collections.Generic;
using System.IO;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Iconoclast
{
    public class PoFormat
    {
        public List<string> Speakers { get; private set; }
        public List<string> Sentences { get; private set; }
        public List<string> GameCode { get; private set; }

        public PoFormat(string poFile)
        {
            Speakers = new List<string>();
            Sentences = new List<string>();
            GameCode = new List<string>();

            ReadPo(poFile);
        }

        public PoFormat(List<string> Speak, List<string> Senten, List<string> GameC)
        {
            Speakers = Speak;
            Sentences = Senten;
            GameCode = GameC;
        }

        private void ReadPo(string PoFilePosition)
        {
            string fileName = Path.GetFileNameWithoutExtension(PoFilePosition);

            Po poFile = null;

            using (DataStream ds = DataStreamFactory.FromFile(PoFilePosition, FileOpenMode.Read))
            using (BinaryFormat bf = new BinaryFormat(ds))
            {
                poFile = (Po)ConvertFormat.With<Po2Binary>(bf);
            }

            foreach (PoEntry entry in poFile.Entries)
            {
                if (entry.Context == null || entry.Context.Trim() == string.Empty)
                {
                    Speakers.Add(string.Empty);
                }
                else
                {
                    Speakers.Add(entry.Context[7..]);
                }

                if (entry.Original == "[EMPTY_LINE]")
                {
                    Sentences.Add(string.Empty);
                }
                else if (entry.Translated.Trim() != string.Empty && entry.Translated.Trim().Length > 1)
                {
                    Sentences.Add(entry.Translated);
                }
                else
                {
                    Sentences.Add(entry.Original);
                }

                if (entry.ExtractedComments == "GAMECODE NOT FOUND")
                {
                    GameCode.Add(string.Empty);
                }
                else
                {
                    GameCode.Add(entry.ExtractedComments);
                }
            }
        }

        public void MakePo(string DestinationDir = "Extracted text")
        {
            if (!Directory.Exists(DestinationDir))
            {
                Directory.CreateDirectory(DestinationDir);
            }

            //Read the language used by the user' OS, this way the editor can spellcheck the translation.
            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;

            Po po = new Po
            {
                Header = new PoHeader("Iconoclast", "your_email", currentCulture.Name)
            };

            for (int i = 0; i < Sentences.Count; i++)
            {
                PoEntry entry = new PoEntry();

                // Print the "Speaker".
                if (Speakers[i] != string.Empty)
                {
                    if (i < Speakers.Count)
                    {
                        entry.Context = $"{i + 1:D4} | {Speakers[i]}";
                    }
                    else
                    {
                        entry.Context = $"{i + 1:D4} | {"ERROR"}";
                    }
                }
                else
                {
                    entry.Context = $"{i + 1:D4}"; // If there isn't a Speaker, then just print the sentence number
                }

                // Print the original sentence.
                if (Sentences[i] == "" || Sentences[i] == string.Empty)
                {
                    entry.Original = "[EMPTY_LINE]";
                    entry.Translated = "[EMPTY_LINE]";
                }
                else if (Sentences[i].Length == 1 || Sentences[i] == " \n" || Sentences[i] == "\n" || Sentences[i] == "..." || Sentences[i] == "…" || Sentences[i] == "...\n" || Sentences[i] == "…\n" || Sentences[i] == "\"...\"" || Sentences[i] == "\"…\"" || Sentences[i] == "\"...\n\"" || Sentences[i] == "\"…\n\"")
                {
                    entry.Original = Sentences[i];
                    entry.Translated = Sentences[i];
                }
                else
                {
                    entry.Original = Sentences[i];
                }

                // Print the Game Code.
                if (GameCode != null)
                {
                    if (i < GameCode.Count && GameCode[i] != null && GameCode[i] != "")
                    {
                        entry.ExtractedComments = GameCode[i].Replace("\r\n", "\n#. ").Replace("\n\r", "\n#. ").Replace("\n", "\n#. ").Replace("\r", string.Empty); // The repalce is needed, otherwise PoEditor is not going to load correctly the jp text and the Repack is gonna crash.
                    }
                    else
                    {
                        entry.ExtractedComments = "GAMECODE NOT FOUND";
                    }
                }

                po.Add(entry);
            }

            string NewPOAddress = Path.Combine(DestinationDir, "Iconoclast.po");

            ((BinaryFormat)ConvertFormat.With<Po2Binary>(po)).Stream.WriteTo(NewPOAddress);
        }
    }
}
