using Bot.ML.Common.Data;
using Bot.VSM.Common;
using Bot.VSM.Modules.TextToVector.Policy;
using System;
using System.Collections.Generic;

namespace Bot.VSM.Modules.TextToVector
{
    public class VectorSpaceFilter
    {

        private VectorSpace VS;
        private string ConfigFilePath;

        private string DEFAULT_POLICYCLASS = "Bot.VSM.Data.NGramFeatureSelectionPolicy";
        private SelectionPolicy featureSelectionPolice;

        public VectorSpaceFilter(VSMInfo VSM)
        {
            this.ConfigFilePath = VSM.VSMConfigFilePath;
            this.VS = VSM.VS;

            this.featureSelectionPolice = GetFeatureSelectionPolicy();                                             
        }

        private SelectionPolicy GetFeatureSelectionPolicy()
        {
            string className = VSMConfigStore.GetInstance(ConfigFilePath).GetOption("FeatureSelectionPolicyClass");

            if (string.IsNullOrWhiteSpace(className))
            {
                className = DEFAULT_POLICYCLASS;
            }

            Type elementType = Type.GetType(className);
            object instance = Activator.CreateInstance(elementType, new object[]{ConfigFilePath,VS });//System.Reflection.Assembly.GetExecutingAssembly().CreateInstance()

            return (SelectionPolicy)instance;
        }

        public Dictionary<string, TermShortInfo> Filter()
        {
            return this.featureSelectionPolice.Filter();
        }
         
    }
}
