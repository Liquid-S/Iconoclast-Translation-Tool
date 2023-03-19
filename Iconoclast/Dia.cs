using System.Collections.Generic;
using System.IO;

namespace Iconoclast
{
    public class Dia
    {
        private int nSentences;
        public List<string> Speakers { get; private set; }
        public List<string> Sentences { get; private set; }
        public List<string> GameCode { get; private set; }

        private readonly Rosetta Rose;

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

        public Dia(List<string> Speak, List<string> Senten, List<string> GameC)
        {
            Speakers = Speak;
            Sentences = Senten;
            GameCode = GameC;

            Rose = new Rosetta("Rosetta.txt");
            BuildDia();
        }

        private void ExtractTestFromDia(string filePath)
        {
            using FileStream DiaFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using BinaryReader Br = new BinaryReader(DiaFile);
            int sentenceLenght;

            DiaFile.Seek(0x06, SeekOrigin.Begin);

            nSentences = Br.ReadInt32();

            /* File extructure:
                   MAGIC ID       N Files     UNK - ALWAYS BEFORE SPEAKER LENGTH 
             41 52 52 31 2E 30|87 10 00 00| 03 00 00 00 01 00
                              SPEAKER LENGHT    SPEAKER
             00 00 02 00 00 00| 0C 00 00 00| 34 37 7C 36 36 7C
                                    UNK - ALWAYS BEFORE SENTENCES OR GAME'S INSTRUCTIONS
             37 38 7C 37 30 00| 01 00 00 00 02 00 00 00 E7 01
                            SENTENCE
             00 00| 7B 62 75 62 30 34 7D 7C 7B 6C 6F 63 6B 7D...
                                            UNK - ALWAYS BEFORE SENTENCES OR GAME INSTRUCTIONS
             79 65 30 30 7D 7C 31 35 00| 01 00 00 00 02 00 00
                 G.I. LENGHT  GAME INSTRUCTIONS         UNK - ALWAYS BEFORE SPEAKER LENGTH 
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

        private string ReadSingleLineFromDia(in FileStream DF, int SentencesLenght, bool isSentence = false, bool isGameCode = false)
        {
            byte[] tempBuffer = new byte[SentencesLenght - 1];
            DF.Read(tempBuffer, 0, tempBuffer.Length);
            string Sentence = string.Empty;

            if (!isGameCode)
            {
                Sentence = "|";
            }

            Sentence += System.Text.Encoding.Default.GetString(tempBuffer);

            if (!isGameCode)
            {
                for (int i = Rose.Stone.Length - 1; i >= 0; i--)
                {
                    Sentence = Sentence.Replace("|" + i.ToString(), Rose.Stone[i]);
                }

                Sentence = Sentence.Replace("\\", "一");

                if (isSentence)
                {
                    Sentence = Sentence.Replace("{new}", "\n");
                }

                return Sentence.Replace("|", null);
            }

            Sentence = Sentence.Replace("\\", "一");

            return Sentence;
        }

        public void BuildDia(string DestinationDir = "Repacked File")
        {
            if (!Directory.Exists(DestinationDir))
            {
                Directory.CreateDirectory(DestinationDir);
            }

            using FileStream NewDia = new FileStream(Path.Combine(DestinationDir, "dia"), FileMode.Create, FileAccess.Write);
            using BinaryWriter Bw = new BinaryWriter(NewDia), BwText = new BinaryWriter(NewDia, System.Text.Encoding.Default);
            char[] tempS;

            Bw.Write(0x31525241);
            Bw.Write((short)0x302E);
            Bw.Write((uint)Sentences.Count);

            for (int i = 0; i < Sentences.Count; i++)
            {
                Bw.Write(0x03);
                Bw.Write(0x01);
                Bw.Write(0x02);

                if (Speakers[i] == null || Speakers[i] == string.Empty)
                {
                    tempS = "".ToCharArray();
                }
                else
                {
                    string NameTranslated = TranslatedSpeaker(Speakers[i]);
                    tempS = EncodeString(NameTranslated).ToCharArray();
                }

                Bw.Write((uint)tempS.Length + 1);
                BwText.Write(tempS);
                Bw.Write((byte)0x0);

                Bw.Write(0x01);
                Bw.Write(0x02);

                tempS = EncodeString(Sentences[i]).ToCharArray();
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

        private static string TranslatedSpeaker(string Speaker) {
            switch (Speaker)
            {
                default:
                    break;
                case "MOTHER":
                    Speaker = "MADRE";
                    break;
                case "MR. ANDRESS":
                    Speaker = "SIG. ANDRESS";
                    break;
                case "PROGENARIAN":
                    Speaker = "PROGENITO";
                    break;
                case "BASTION SOLDIER":
                    Speaker = "SOLDATO DEL BASTIONE";
                    break;
                case "CONCERN SOLDIER":
                    Speaker = "SOLDATO DELLA CONCERN";
                    break;
                case "CORNER DARK":
                    Speaker = "TRIGONO OSCURO";
                    break;
                case "CORNER LIGHT":
                    Speaker = "TRIGONO CHIARO";
                    break;
                case "CITY STAR":
                    Speaker = "STAR﻿";
                    break;
                case "CITY DUNNER":
                    Speaker = "DUNNER";
                    break;
                case "CITY TONY":
                    Speaker = "TONY";
                    break;
                case "CITY JIANI":
                    Speaker = "JIANI";
                    break;
                case "CITY TILDA":
                    Speaker = "TILDA";
                    break;
                case "CITY MECHANIC":
                    Speaker = "MECCANICO";
                    break;
                case "CITY BRIDGE":
                    Speaker = "BRIDGE";
                    break;
                case "THOR DAUGHTER":
                    Speaker = "FIGLIA DI THOR";
                    break;
                case "GATEKEEPER PETE":
                    Speaker = "GUARDIANO PIETRO";
                    break;
                case "ISI SCIENTIST":
                    Speaker = "SCIENZIATO ISI";
                    break;
                case "ELITE SOLDIER":
                    Speaker = "SOLDATO ÉLITE";
                    break;
                case "ELITE":
                    Speaker = "ÉLITE";
                    break;
                case "ELITE 1":
                    Speaker = "ÉLITE 1";
                    break;
                case "ELITE 2":
                    Speaker = "ÉLITE 2";
                    break;
                case "MECHANIC":
                    Speaker = "MECCANICO";
                    break;
                case "CHEMICO CHEMIST":
                    Speaker = "SCIENZIATO CHEMICO";
                    break;
            }

            return Speaker;
        }

        private string EncodeString(string Sentence)
        {
            Sentence = Sentence.Replace("\n", "{new}");

            string EncondeSentence = string.Empty;
            bool isCode = false;

            foreach (char x in Sentence)
            {
                if (x == '{')
                {
                    isCode = true;
                }
                else if (x == '}')
                {
                    EncondeSentence += "}|";
                    isCode = false;
                    continue;
                }

                if (isCode)
                {

                    if (EncondeSentence.Length > 1 && x == '{' && EncondeSentence[^1] == '一')
                    {
                        EncondeSentence += '|';
                    }

                    EncondeSentence += x;

                }
                else
                {
                    int pos = System.Array.LastIndexOf(Rose.Stone, x.ToString());
                    EncondeSentence += pos.ToString() + "|".Replace("\\\\", "\\").Replace("一", "\\");
                }
            }

            if (EncondeSentence[^1] == '|')
            {
                EncondeSentence = EncondeSentence.Remove(startIndex: EncondeSentence.Length - 1);
            }

            return EncondeSentence.Replace("\\\\", "\\").Replace("一", "\\");
        }
    }
}
