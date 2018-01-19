using Bot.ML.Common.Data;
using Bot.VSM.Common;
using System.Collections.Generic;

namespace Bot.VSM.Modules.DocProcessor
{
    public abstract class GramCutter
    {
        protected VSMConfigStore configStore;

        public GramCutter(string ConfigFilePath)
        {
            this.configStore = VSMConfigStore.GetInstance(ConfigFilePath);
        }

        public abstract List<UniGram> CutToUniGrams(Dictionary<string, string> InputDict);
    }
}
