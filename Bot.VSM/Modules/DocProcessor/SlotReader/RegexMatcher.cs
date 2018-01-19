using System;
using System.Collections.Generic;
using Bot.ML.Common.Data;

namespace Bot.VSM.Modules.DocProcessor.SlotReader
{
    public class RegexMatcher : GramCutter
    {
        private string RuleFilePath;
        public RegexMatcher(string configPath)
            :base(configPath)
        {
            this.RuleFilePath = this.configStore.GetOption("RuleFile");
        }



        override
        public List<UniGram> CutToUniGrams(Dictionary<string, string> InputDict)
        {
            throw new NotImplementedException();
        }
    }
}
