using Bot.ML.Common.Data;
using System;
using System.Collections.Generic;

namespace Bot.VSM.Executor.Trainer
{
    public class BCVSMTrainer : VSMTrainer
    {        
        private string NegtiveLabelString;
        
        public BCVSMTrainer(string[] configFiles, string NegtiveLabelString)
            :base(configFiles)
        {
            this.NegtiveLabelString = NegtiveLabelString;
            
        }

        public ClassificationVSMInfo Training(string PositiveLabel, List<SourceData> cases, string modelfilepath, string modellabelfilepath)
        {
            if (string.IsNullOrWhiteSpace(PositiveLabel))
            {
                throw new Exception("The positive label cannot be empty.");
            }
            
            ClassificationVSMInfo result = TrainingModel(PositiveLabel, NegtiveLabelString, cases);

            SaveVSToModelFile(result, modelfilepath, modellabelfilepath);

            return result;
        }
    }
}
