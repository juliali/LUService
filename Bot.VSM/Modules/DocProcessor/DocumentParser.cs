using Bot.ML.Common.Data;
using System;
using System.Collections.Generic;

namespace Bot.VSM.Modules.DocProcessor
{


    public class DocumentParser
    {
        private UtteranceSpliter Spliter;       
        private NGramTermReader TermReader;

        public DocumentParser(string configFilePath)
        {
            this.Spliter = new UtteranceSpliter(configFilePath);
            this.TermReader = new NGramTermReader(configFilePath);            
        }        

        public DocumentInfo Parse(int DocumentId, string Label, string DocumentContent, string EdiResult)
        {
            DocumentInfo DocInfo = new DocumentInfo();
            DocInfo.DocumentId = DocumentId;
            DocInfo.Label = Label;
            DocInfo.DocumentContent = DocumentContent;

            DocInfo.Utterances = this.Spliter.GetUtterances(DocumentContent);
            List<RawTermInfo> RawTerms = new List<RawTermInfo>();
            foreach (KeyValuePair<int, string> kv in DocInfo.Utterances)
            {
                int UtteranceId = kv.Key;
                string Utterance = kv.Value;

                if (string.IsNullOrWhiteSpace(Utterance))
                {
                    Console.WriteLine("Utterance cannot be empty.");
                    continue;
                }

                Dictionary<string, string> InputDict = Utils.Utils.GenInputDict(Utterance, EdiResult);

                List<RawTermInfo> TermList = this.TermReader.GetTerms(DocumentId, UtteranceId, InputDict);

                if (TermList != null)
                    {

                        RawTerms.AddRange(TermList);

                    }
                
            }

            DocInfo.RawTerms = RawTerms;

            Dictionary<string, int> TermCounts = new Dictionary<string, int>();
            foreach (RawTermInfo Term in RawTerms)
            {
                string TermString = Term.TermString;

                if (TermCounts.ContainsKey(TermString))
                {
                    int OrigCount = TermCounts[TermString];
                    TermCounts[TermString] = OrigCount + 1;
                }
                else
                {
                    TermCounts.Add(TermString, 1);
                }
            }

            DocInfo.TermCounts = TermCounts;

            return DocInfo;
        }        
    }
}
