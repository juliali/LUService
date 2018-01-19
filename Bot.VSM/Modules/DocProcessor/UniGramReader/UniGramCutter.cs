using Bot.ML.Common.Data;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bot.VSM.Modules.DocProcessor.UniGramReader
{
    public class UniGramCutter: GramCutter
    {
        private const string Key = "Utterance";
        private List<char> CharList = new List<char>();
        private int index = 0;
        private int start = -1;
        private int len = 0;
        private List<UniGram> Grams = new List<UniGram>();        

        public UniGramCutter(string configPath)
            :base(configPath)
        {            
        }

        private bool IsEnglishCharactor(char c)
        {
            UnicodeCategory cat = char.GetUnicodeCategory(c);

            if (cat == UnicodeCategory.OtherLetter)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void GetGram()
        {
            if (CharList.Count > 0)
            {
                UniGram Gram = new UniGram();
                Gram.GramString = new string(CharList.ToArray());
                Gram.StartIndex = start;
                Gram.UniGramLength = len;
                Grams.Add(Gram);
                
                start = -1;
                len = 0;

                this.CharList.Clear();
            }
        }

        private void GetGram(string GramString)
        {
            if (!string.IsNullOrWhiteSpace(GramString))
            { 
                UniGram Gram = new UniGram();
                Gram.GramString = GramString;
                Gram.StartIndex = start;
                Gram.UniGramLength = len;
                Gram.IsChinese = true;
                Grams.Add(Gram);
                
                start = -1;
                len = 0;                
            }
        }

        private void init()
        {
            Grams = new List<UniGram>();
            CharList = new List<char>();
            index = 0;
            start = -1;
            len = 0;
        }

        override
        public List<UniGram> CutToUniGrams(Dictionary<string, string> InputDict)
        {
            init();

            string Utterance = InputDict[Key];

            Char[] Chars = Utterance.ToCharArray();

            string GramStr = null;
                     
            while (index < Chars.Length)
            {
                char c = Chars[index];

                if (c == ' ')
                {                   
                    GetGram();
                }
                else
                {
                    if (start < 0)
                    {
                        start = index;
                    }

                    if (IsEnglishCharactor(c))
                    {
                        CharList.Add(c);
                        len++;
                    }
                    else
                    {
                        GetGram();

                        GramStr = c.ToString();
                        len++;

                        GetGram(GramStr);
                    }
                }                

                index++;
            }

            GetGram();

            return Grams;
        }        
    }
}
