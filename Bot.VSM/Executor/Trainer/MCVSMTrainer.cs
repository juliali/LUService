using Bot.ML.Common.Data;
using System.Collections.Generic;

namespace Bot.VSM.Executor.Trainer
{
    public class MCVSMTrainer : VSMTrainer
    {        
        public MCVSMTrainer(string[] configFiles):
            base(configFiles)
        {            
        }

        public void Training(List<SourceData> cases, string modelfilepath, string modellabelfilepath)
        {
            ClassificationVSMInfo result = TrainingModel(null, null, cases);            
            SaveVSToModelFile(result, modelfilepath, modellabelfilepath);
        }                
    }
}
