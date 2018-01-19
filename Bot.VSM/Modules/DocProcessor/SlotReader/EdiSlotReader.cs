using System.Collections.Generic;
using Newtonsoft.Json;
using Bot.ML.Common.Data;

namespace Bot.VSM.Modules.DocProcessor.SlotReader
{
    public class EdiSlotReader : GramCutter
    {
        private const string Key = "EdiResult";
        public EdiSlotReader(string ConfigFilePath) : base(ConfigFilePath)
        {
        }

        public override List<UniGram> CutToUniGrams(Dictionary<string, string> InputDict)
        {            
            List<UniGram> list = new List<UniGram>();

            if ((InputDict == null) || (!InputDict.ContainsKey(Key)) || (string.IsNullOrWhiteSpace(InputDict[Key])))
            {
                return list;
            }

            string EdiResultStr = InputDict[Key];


            EdiInfo ediInfo = JsonConvert.DeserializeObject<EdiInfo>(EdiResultStr);

            foreach(SlotInfo slot in ediInfo.Segments)
            {
                if (slot.Tag.StartsWith("Intent:"))
                {
                    continue;
                }

                UniGram gram = new UniGram();
                gram.GramString = slot.Tag;
                gram.IsChinese = false;
                gram.StartIndex = slot.Start;
                gram.UniGramLength = slot.Length;

                list.Add(gram);
            }

            return list;
        }
    }
}
