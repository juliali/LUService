using Bot.ML.Common.Data;
using Bot.VSM.Common;
using System;
using System.Collections.Generic;

namespace Bot.VSM.Modules.DocProcessor
{

    public class NGramTermReader
    {
        private int[] DEFAULT_TermLengthArray = { 1};
        private string DEFAULT_CUTTERCLASS = "Bot.VSM.Modules.DocProcessor.UniGramReader.UniGramCutter";        
        
        private VSMConfigStore configStore;
        private string ConfigFilePath;

        public NGramTermReader(string ConfigFilePath)
        {
            this.ConfigFilePath = ConfigFilePath;
            this.configStore = VSMConfigStore.GetInstance(ConfigFilePath);
        }

        private GramCutter GetCutter()
        {
            string className = this.configStore.GetOption("UniGramCutterClass");

            if (string.IsNullOrWhiteSpace(className))
            {
                className = DEFAULT_CUTTERCLASS;
            }

            Type elementType = Type.GetType(className);
            object instance = Activator.CreateInstance(elementType, new object[] { ConfigFilePath }); ;

            return (GramCutter) instance;
        }

        public List<RawTermInfo> GetTerms(int DocumentId, int UtteranceId, Dictionary<string, string> UtteranceInputDict)
        {
            List<RawTermInfo> Terms = new List<RawTermInfo>();

            
                List<RawTermInfo> tmpTerms = GetTermsForSingleCutter(DocumentId, UtteranceId, UtteranceInputDict);
                if (tmpTerms != null)
                {
                    Terms.AddRange(tmpTerms);
                }
            

            return Terms;
        }

        private List<RawTermInfo> GetTermsForSingleCutter(int DocumentId, int UtteranceId, Dictionary<string, string> UtteranceInputDict)
        {
            string arrayStr = this.configStore.GetOption("TermLengthArray");
            int[] GramLens = this.DEFAULT_TermLengthArray;

            if (!string.IsNullOrWhiteSpace(arrayStr))
            {
                string[] tmps = arrayStr.Split(',');
                List<int> list = new List<int>();

                foreach(string tmp in tmps)
                {
                    try
                    {
                        int len = int.Parse(tmp.Trim());
                        list.Add(len);
                    }
                    catch(Exception)
                    {

                    }
                }

                if (list.Count > 0)
                {
                    GramLens = list.ToArray();
                }
            }
            
            List<UniGram> UniGramList = GetUniGrams(UtteranceInputDict);
            List<RawTermInfo> result = GetTerms(DocumentId, UtteranceId, UniGramList, GramLens);
            return result;
        }

        private List<UniGram> GetUniGrams(Dictionary<string,string> UtteranceInputDict)
        {
            GramCutter Cutter = GetCutter();
            return Cutter.CutToUniGrams(UtteranceInputDict);
        }        

        private List<RawTermInfo> GetTerms(int DocumentId, int UtteranceId, List<UniGram> Grams, int[] GramLens)
        {
            List<RawTermInfo> List = new List<RawTermInfo>();

            foreach(int TermLength in GramLens)
            {
                List<RawTermInfo> TermList = GetRawTermsForSepcifiedLenght(DocumentId, UtteranceId, Grams, TermLength);
                if (TermList != null)
                {
                    List.AddRange(TermList);
                }
            }
            return List;
        }

        private List<RawTermInfo> GetRawTermsForSepcifiedLenght(int DocumentId, int UtteranceId, List<UniGram> Grams, int TermLength)
        {
            List<RawTermInfo> TermList = new List<RawTermInfo>();

            int index = 0;

            while (index + TermLength <= Grams.Count)
            {
                RawTermInfo RawTerm = new RawTermInfo();
                TermPositionInfo Position = new TermPositionInfo();

                List<char> CurrentTermChars = new List<char>();
                int CurrentTermLen = 0;

                for (int i = index; i < index + TermLength; i++)
                {
                    CurrentTermChars.AddRange(Grams[i].GramString.ToCharArray());

                    if (i < index + TermLength - 1)
                    {
                        int LackCharNum = Grams[i + 1].StartIndex - (Grams[i].StartIndex + Grams[i].UniGramLength);

                        for (int j = 0; j < LackCharNum; j++)
                        {
                            CurrentTermChars.Add(' ');
                        }

                        if (i > index)
                        {
                            CurrentTermLen += Grams[i].StartIndex - Grams[i - 1].StartIndex;
                        }

                    }
                    else
                    {
                        CurrentTermLen += Grams[i].UniGramLength;
                    }
                }

                RawTerm.TermString = new string(CurrentTermChars.ToArray());
                RawTerm.TermLength = TermLength;

                Position.DocumentId = DocumentId;
                Position.UtteranceId = UtteranceId;
                Position.StartIndex = Grams[index].StartIndex;

                RawTerm.Position = Position;

                TermList.Add(RawTerm);

                index++;               
            }
            return TermList;
        }
    }
}
