using Bot.ML.Common.Data;
using Bot.VSM.Executor.Convertor;
using Bot.VSM.Executor.Trainer;

namespace Bot.Cluster.Test
{
    public class UnlabelFeatureGenTest
    {

        public static void TrainVSM(string[] configfiles, string traintextfile, string vsmodelfile)
        {
            string modellabelfile = vsmodelfile.Replace(".model", "_label.txt");
            NoLabelVSMTrainer vmsTrainer = new NoLabelVSMTrainer(configfiles);
            vmsTrainer.Training(traintextfile, vsmodelfile);
        }

        public static void GeneratorFeatureFileForTextFile(string vsmodelfile, string input_textfile, string output_featurefile)
        {
            NoLabelVSMConvertor featureGen = new NoLabelVSMConvertor(new ModelFilePathInfo(vsmodelfile));
            featureGen.GenerateBatchFeatures(input_textfile, output_featurefile);
        }

        public static void TrainVSMAndGenerateTraninigFeatureFile(string[] configfiles, string trainingtextfile, string trainingfeaturefile, string vsmodelfile)
        {            
            TrainVSM(configfiles, trainingtextfile, vsmodelfile);
            
            GeneratorFeatureFileForTextFile(vsmodelfile, trainingtextfile, trainingfeaturefile);            
        }
    }
}
