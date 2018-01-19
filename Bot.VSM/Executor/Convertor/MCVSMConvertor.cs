using Bot.ML.Common.Data;
using System.Collections.Generic;


namespace Bot.VSM.Executor.Convertor
{
    public class MCVSMConvertor : VSMConvertor
    {

        public MCVSMConvertor(ModelFilePathInfo modelfile)
            : base(modelfile)
        {

        }

        public int GetLabelValue(string label)
        {
            if (this.Model.LabelValues.LabelValueDict.ContainsKey(label))
            {
                return this.Model.LabelValues.LabelValueDict[label];
            }
            else
            {
                return -1;
            }
        }

        public List<ICExample> GenerateBatchFeatures(List<SourceData> cases, string outputFeatureFile, bool ignoreInvalid)
        {            
            return GenerateBatchFeatures(null, null, cases, outputFeatureFile, ignoreInvalid);
        }        
    }
}
