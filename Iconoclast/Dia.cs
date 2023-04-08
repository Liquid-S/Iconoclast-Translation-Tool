using System.Collections.Generic;
using System.IO;

namespace Iconoclast
{
    public class Dia
    {
        public List<string> Speakers { get; private set; }
        public List<string> Sentences { get; private set; }
        public List<string> GameCode { get; private set; }

        private readonly Rosetta Rose;

        private System.Func<string, string> translateSpeaker;

        public Dia(string filePath)
        {
            if (File.Exists(filePath))
            {
                Speakers = new List<string>();
                Sentences = new List<string>();
                GameCode = new List<string>();

                Rose = new Rosetta("Rosetta.txt");
                ExtractTestFromDia(filePath);
            }
        }

        public Dia(List<string> speak, List<string> senten, List<string> gameC, System.Func<string, string> translateSpeaker)
        {
            Speakers = speak;
            Sentences = senten;
            GameCode = gameC;

            this.translateSpeaker = translateSpeaker;

            Rose = new Rosetta("Rosetta.txt");
            BuildDia();
        }

        private void ExtractTestFromDia(string filePath)
        {
            using FileStream DiaFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using BinaryReader Br = new BinaryReader(DiaFile);
            int sentenceLenght = 0;

            DiaFile.Seek(0x06, SeekOrigin.Begin);

            Br.ReadInt32(); //n Sentences

            /* File extructure:
                 MAGIC ID     | N# Files  | UNK - ALWAYS BEFORE SPEAKER LENGTH 
             41 52 52 31 2E 30|87 10 00 00| 03 00 00 00 01 00
               SPEAKER LENGHT | SPEAKER
             00 00 02 00 00 00| 0C 00 00 00| 34 37 7C 36 36 7C
                UNK - ALWAYS BEFORE SENTENCES OR GAME'S INSTRUCTIONS
             37 38 7C 37 30 00| 01 00 00 00 02 00 00 00 E7 01
                    SENTENCE
             00 00| 7B 62 75 62 30 34 7D 7C 7B 6C 6F 63 6B 7D...
                                       | UNK - ALWAYS BEFORE SENTENCES OR GAME INSTRUCTIONS
             79 65 30 30 7D 7C 31 35 00| 01 00 00 00 02 00 00
           G.I.| LENGHT GAME INSTRUCTIONS | UNK - ALWAYS BEFORE SPEAKER LENGTH 
             00| 08 00 00 00| 62 75 74 74 6F 6E 73 00| 03 00 00
             */


            while (DiaFile.Position != DiaFile.Length)
            {
                switch (Br.ReadInt32())
                {
                    case 1: //Speaker or sentence
                        Br.ReadInt32();
                        sentenceLenght = Br.ReadInt32();
                        if (Sentences.Count == GameCode.Count)
                        {
                            Sentences.Add(ReadSingleLineFromDia(in DiaFile, sentenceLenght, true));
                        }
                        else
                        {
                            GameCode.Add(ReadSingleLineFromDia(in DiaFile, sentenceLenght, false, true));
                        }
                        Br.ReadByte();
                        break;
                    case 3: //New Sentence
                        Br.ReadInt64();
                        sentenceLenght = Br.ReadInt32();
                        Speakers.Add(ReadSingleLineFromDia(in DiaFile, sentenceLenght));
                        Br.ReadByte();
                        break;
                    default:
                        break;
                }
            }
        }

        private string ReadSingleLineFromDia(in FileStream DF, int sentenceLenght, bool isSentence = false, bool isGameCode = false)
        {
            byte[] tempBuffer = new byte[sentenceLenght - 1];
            DF.Read(tempBuffer, 0, tempBuffer.Length);
            string sentence = string.Empty;

            if (!isGameCode)
            {
                sentence = "|";
            }

            sentence += System.Text.Encoding.Default.GetString(tempBuffer);

            if (!isGameCode)
            {
                for (int i = Rose.Stone.Length - 1; i >= 0; i--)
                {
                    sentence = sentence.Replace("|" + i.ToString(), Rose.Stone[i]);
                }

                sentence = sentence.Replace("\\", "一");

                if (isSentence)
                {
                    sentence = sentence.Replace("{new}", "\n");
                }

                return sentence.Replace("|", null);
            }

            sentence = sentence.Replace("\\", "一");

            return sentence;
        }

        public void BuildDia(string destinationDir = "Repacked File")
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            using FileStream newDia = new FileStream(Path.Combine(destinationDir, "dia"), FileMode.Create, FileAccess.Write);
            using BinaryWriter Bw = new BinaryWriter(newDia), BwText = new BinaryWriter(newDia, System.Text.Encoding.Default);
            char[] tempS;

            Bw.Write(0x31525241);
            Bw.Write((short)0x302E);
            Bw.Write((uint)Sentences.Count);

            for (int i = 0; i < Sentences.Count; i++)
            {
                Bw.Write(0x03);
                Bw.Write(0x01);
                Bw.Write(0x02);

                if (string.IsNullOrEmpty(Speakers[i]))
                {
                    tempS = "".ToCharArray();
                }
                else
                {
                    string nameTranslated = translateSpeaker(Speakers[i]);
                    tempS = StringEncoder(nameTranslated).ToCharArray();
                }

                Bw.Write((uint)tempS.Length + 1);
                BwText.Write(tempS);
                Bw.Write((byte)0x0);

                Bw.Write(0x01);
                Bw.Write(0x02);

                tempS = StringEncoder(Sentences[i]).ToCharArray();
                Bw.Write((uint)tempS.Length + 1);
                BwText.Write(tempS);
                Bw.Write((byte)0x0);

                Bw.Write(0x01);
                Bw.Write(0x02);

                tempS = GameCode[i].Replace("一", "\\").ToCharArray();
                Bw.Write((uint)tempS.Length + 1);
                BwText.Write(tempS);
                Bw.Write((byte)0x0);
            }
        }

        private string StringEncoder(string sentence)
        {
            sentence = sentence.Replace("\n", "{new}");

            string encondedSentence = string.Empty;
            bool isCode = false;

            foreach (char x in sentence)
            {
                if (x == '{')
                {
                    isCode = true;
                }
                else if (x == '}')
                {
                    encondedSentence += "}|";
                    isCode = false;
                    continue;
                }

                if (isCode)
                {
                    if (encondedSentence.Length > 1 && x == '{' && encondedSentence[^1] == '一')
                    {
                        encondedSentence += '|';
                    }

                    encondedSentence += x;
                }
                else
                {
                    int pos = System.Array.LastIndexOf(Rose.Stone, x.ToString());
                    encondedSentence += pos.ToString() + "|".Replace("\\\\", "\\").Replace("一", "\\");
                }
            }

            if (encondedSentence[^1] == '|')
            {
                encondedSentence = encondedSentence.Remove(startIndex: encondedSentence.Length - 1);
            }

            return encondedSentence.Replace("\\\\", "\\").Replace("一", "\\");
        }
    }
}
