using Bot.ML.Common.Data;
using Bot.VSM.Common;
using System.Collections.Generic;
using System.Linq;

namespace Bot.VSM.Modules.TextToVector.Policy
{
    public abstract class SelectionPolicy
    {
        protected VSMConfigStore configStore;
        protected VectorSpace VS;

        public SelectionPolicy(string ConfigFilePath, VectorSpace VS)
        {
            this.VS = VS;
            this.configStore = VSMConfigStore.GetInstance(ConfigFilePath);
        }

        public Dictionary<string, TermShortInfo> Filter()
        {
            List<TermSpaceInfo> OrigEvaluatedTerms = VS.EvaluatedTerms.Values.ToList();
            Dictionary<string, TermShortInfo> Results = new Dictionary<string, TermShortInfo>();

            int Index = 0;

            foreach (TermSpaceInfo Info in OrigEvaluatedTerms)
            {

                if (!IsValidTerm(Info))
                {
                    continue;
                }


                TermShortInfo ShortInfo = ConvertToShortInfo(Info, Index++);
                Results.Add(ShortInfo.TermString, ShortInfo);

            }

            return Results;
        }

        protected abstract bool IsValidTerm(TermSpaceInfo Info);

        protected TermShortInfo ConvertToShortInfo(TermSpaceInfo Info, int index)
        {
            TermShortInfo ShortInfo = new TermShortInfo();

            ShortInfo.TermString = Info.TermString;
            ShortInfo.TermLength = Info.TermLength;
            ShortInfo.FeatureValue = Info.FeatureValue;
            ShortInfo.VSIndex = index;

            return ShortInfo;
        }

    }
}
