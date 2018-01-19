using Bot.ML.Common.Data;
using Bot.ML.Common.Utils;
using Bot.TLC.Traininig.Executor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.TLC.Training.Main
{
    public class Trainer
    {
        private readonly string VSMPrefix = "VSM_BC_";
        private readonly string TLCPrefix = "TLC_BC_";

        private List<SourceData> trainingSet;

        private string containerName;      
        private static BlobLoader blobLoader = BlobLoader.GetInstance();
        
        public Trainer(List<SourceData> cases, string containerName)
        {
            this.trainingSet = cases;//GetClassifierTrainingDataFromDB();          

            this.containerName = containerName;
            if (!string.IsNullOrWhiteSpace(containerName))
            { 
                if (!blobLoader.CreateContainer(containerName))
                {
                    throw new Exception("Cannot create container!");
                }
            }            
        }

        public BCConfigInfo Train(string filename, string[] VSMConfigFilePaths, string NegtiveLabel, string[] PositiveLabels)
        {
            BCConfigInfo bcConfigInfo = new BCConfigInfo();
            bcConfigInfo.VSMConfigPaths = VSMConfigFilePaths;
            bcConfigInfo.NegtiveLabel = NegtiveLabel;
                      
            string temporaryfiledir = GetTemporaryDir(filename);
            FileUtils.CreateOrCleanDir(temporaryfiledir);
            
            HashSet<string> PositiveLabelSet = new HashSet<string>(PositiveLabels);

           BCSingleTrainer trainer = new BCSingleTrainer(VSMConfigFilePaths, NegtiveLabel, this.trainingSet);

            List<BCModelInfo> models = new List<BCModelInfo>();
            foreach (string PositiveLabel in PositiveLabelSet)
            {
                
                string trainingfeaturefile = temporaryfiledir + "_TrainingFeatures_" + filename + "_" + PositiveLabel + ".tsv";

                string vsmfilename = VSMPrefix + filename + "_" + PositiveLabel + ".model";

                string vsmfilepath = temporaryfiledir + vsmfilename;

                string tlcmodelfilename = TLCPrefix + filename + "_" + PositiveLabel + ".zip";

                string tlcmodelfilepath = temporaryfiledir + tlcmodelfilename;

                bool result = trainer.GenerateVSMAndTLCModel(PositiveLabel, vsmfilepath, tlcmodelfilepath, trainingfeaturefile);

                if (result)
                { 
                    BCModelInfo modelInfo = new BCModelInfo();
                    modelInfo.PositiveLabel = PositiveLabel;
                    modelInfo.VSModelPath = new ModelBlobPathInfo(this.containerName, vsmfilename);
                    modelInfo.TLCModelPath = new ModelBlobPathInfo(this.containerName, tlcmodelfilename);

                    models.Add(modelInfo);

                    if (!string.IsNullOrWhiteSpace(this.containerName))
                    { 
                        blobLoader.UploadToContainer(this.containerName, vsmfilename, vsmfilepath);
                        blobLoader.UploadToContainer(this.containerName, tlcmodelfilename, tlcmodelfilepath);
                    }
                }
            }

            bcConfigInfo.Models = models.ToArray<BCModelInfo>();

            return bcConfigInfo;
        }        

        private string GetTemporaryDir(string filename)
        {
            string temporaryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrWhiteSpace(filename))
            {
                temporaryPath += "\\" + filename;
            }

            return temporaryPath;
        }
    }
}
