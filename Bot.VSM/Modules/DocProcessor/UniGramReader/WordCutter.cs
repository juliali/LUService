using Bot.ML.Common.Data;
using System.Collections.Generic;
using System.Linq;


namespace Bot.VSM.Modules.DocProcessor.UniGramReader
{
    public class WordCutter : GramCutter
    {
        private string market = null;
        private const string Key = "Utterance";
        public WordCutter(string ConfigPath):
            base(ConfigPath)
        {
            this.market = "zh-cn";
        }

        public WordCutter(string ConfigPath, string market):
            base(ConfigPath)
        {
            this.market = market;
        }

        override
        public List<UniGram> CutToUniGrams(Dictionary<string, string> InputDict)
        {
            string Utterance = InputDict[Key];
            List<UniGram> UniGramList = new List<UniGram>();

            string wbQuery = WordBreaker.BreakWords(Utterance, this.market);

            string[] words = wbQuery.Split(' ');
            int index = 0;

            foreach(string word in words)
            {
                UniGram AGram = new UniGram();
                AGram.GramString = word;
                AGram.UniGramLength = word.ToCharArray().Length;
                AGram.StartIndex = Utterance.IndexOf(word, index);
                AGram.IsChinese = IsChinese(word);

                index += AGram.UniGramLength;

                UniGramList.Add(AGram);
            }

            //Console.WriteLine(wbQuery);
            return UniGramList;//.ToArray<UniGram>();
        }

        private bool IsChinese(string text)
        {
            return text.Any(c => c >= 0x20000 && c <= 0xFA2D);
        }
    }
}
