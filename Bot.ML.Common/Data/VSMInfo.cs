using System;
using System.Collections.Generic;

namespace Bot.ML.Common.Data
{
    [Serializable]
    public class VSMInfo
    {       
        public VectorSpace VS { get; set; }
        //public FeatureSelectionInfo FeatureSelection { get; set; }
        public string VSMConfigFilePath;
    }

    [Serializable]
    public class LabelValueInfo
    {
        public Dictionary<string, int> LabelValueDict;
        public string NegtiveLabel { get; set; } = null;
    }

    [Serializable]
    public class ClassificationVSMInfo
    {
        public LabelValueInfo LabelValues;
        public List<VSMInfo> VSMWithSelectionPolicyList;
    }
}
