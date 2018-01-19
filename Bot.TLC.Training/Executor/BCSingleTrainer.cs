using System.Collections.Generic;
using Bot.VSM.Executor.Trainer;
using Bot.ML.Common.Data;

namespace Bot.TLC.Traininig.Executor
{
    public class BCSingleTrainer 
    {
        private List<SourceData> cases;       
        private BCVSMTrainer vmsTrainer;
        private BCTrainer tlcTrainer;
        
        public BCSingleTrainer(string[] VSMConfigPaths, string NegtiveLabel, List<SourceData> cases)
        {           
            this.cases = cases;

            this.vmsTrainer = new BCVSMTrainer(VSMConfigPaths, NegtiveLabel);
            this.tlcTrainer = new BCTrainer();
        }      

        public bool GenerateVSMAndTLCModel(string PositiveLabel, string vsmodelPath, string tlcmodelpath, string trainingfeaturefile)
        {
            string modellabelfile = vsmodelPath.Replace(".model", "_label.txt");            
            ClassificationVSMInfo model = this.vmsTrainer.Training(PositiveLabel, this.cases, vsmodelPath, modellabelfile);
            
            tlcTrainer.Train(model, PositiveLabel, this.cases, trainingfeaturefile, tlcmodelpath);

            return true;
        } 
                     
    }
}
