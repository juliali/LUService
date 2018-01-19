using Bot.VSM.Common;
using Newtonsoft.Json;

using System.Collections.Generic;

using System.Linq;

using System.Text.RegularExpressions;


namespace Bot.VSM.Modules.DocProcessor
{
    /*class CutterConfigInfo
    {
        public string UtteranceSeparator = null;
        public string PunctuationIncluded = null;
    }
    */

    public class UtteranceSpliter
    {
        private string Separator = " ";
        private string PunctuationIncluded = "";

        private VSMConfigStore configStore;
        public UtteranceSpliter(string ConfigFilePath)
        {
            this.configStore = VSMConfigStore.GetInstance(ConfigFilePath);
            string Separator = this.configStore.GetOption("UtteranceSeparator");
            string PunctuationIncluded = this.configStore.GetOption("PunctuationIncluded");
            if (!string.IsNullOrWhiteSpace(Separator))
            {
                this.Separator = Separator; // configInfo.UtteranceSeparator;
            }

            if (!string.IsNullOrWhiteSpace(PunctuationIncluded))
            {
                this.PunctuationIncluded = PunctuationIncluded;
            }
        }

        public Dictionary<int, string> GetUtterances(string DocumentContent)
        {            
            Dictionary<int, string> dict = new Dictionary<int, string>();

            if (!string.IsNullOrWhiteSpace(DocumentContent))
            { 

                string[] utterances = this.CutToUtterances(DocumentContent);//this.DocumentContent.Split('\n');

                for (int i = 0; i < utterances.Length; i++)
                {
                    dict.Add(i + 1, utterances[i]);
                }
            }
            return dict;
        }

        private string[] CutToUtterances(string Document)
        {
            if (string.IsNullOrWhiteSpace(Document))
            {
                return null;
            }
            
            string[] results = Regex.Split(Document, Separator);
            Regex r = new Regex(Separator, RegexOptions.IgnoreCase);

            List<string> Utterances = new List<string>();
            foreach (string result in results)
            {
                if (!string.IsNullOrWhiteSpace(result))
                {
                    Match m = r.Match(result);

                    if (!m.Success)
                    {
                        Utterances.Add(result);
                    }
                }
            }
            
            string[] UtterancesArray = Utterances.ToArray<string>();

            if (!string.IsNullOrWhiteSpace(PunctuationIncluded))
            {
                Regex pr = new Regex(PunctuationIncluded);

                int start = 0;
                int DocLen = Document.Length;

                for (int i = 0; i < UtterancesArray.Length; i ++)
                {
                    string currentUterance = UtterancesArray[i];
                    int len = currentUterance.Length;
                
                    int nextStart = DocLen;
                    if (i < UtterancesArray.Length - 1)
                    { 
                        nextStart = Document.IndexOf(UtterancesArray[i + 1], start + len);
                    }
                
                    if(nextStart > start + len)
                    {
                        string SubString = Document.Substring(start, nextStart - start);
                        Match m = pr.Match(SubString);

                        if (m.Success)
                        {
                        UtterancesArray[i] += SubString;
                        }
                    }

                    start = nextStart;
                }
            }

            return UtterancesArray;
        }
    }
}
