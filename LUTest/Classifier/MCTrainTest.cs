using Bot.VSM.Executor.Convertor;
using Bot.VSM.Executor.Trainer;
using Bot.ML.Common.Data;

namespace LUTest.Classifier
{
    public class MCTrainTest
    {
        private const int bottomdf = 2;
        private const double entropypercentage = 0.3;
        /*
                public static void TrainVSM(string[] configfiles, string traintextfile, string vsmodelfile)
                {
                    string modellabelfile = vsmodelfile.Replace(".model", "_label.txt");
                    MCVSMTrainer vmsTrainer = new MCVSMTrainer(configfiles);
                    vmsTrainer.Training(traintextfile, vsmodelfile, modellabelfile);
                }

                public static void GeneratorFeatureFileForTextFile(string vsmodelfile, string input_textfile, string output_featurefile, bool ignoreInvalid)
                {
                    MCVSMConvertor featureGen = new MCVSMConvertor(new ModelFilePathInfo(vsmodelfile));
                    featureGen.GenerateBatchFeatures(input_textfile, output_featurefile, ignoreInvalid);
                }

                public static void TrainVSMAndGenerateTraninigFeatureFileAndTestFeatureFile(string[] configfiles, string trainingtextfile, string testtextfile, string temporaryfiledir)
                {
                    Bot.ML.Common.Utils.FileUtils.CreateOrCleanDir(temporaryfiledir);                        

                    string filename = Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(trainingtextfile);
                    filename = filename.Replace("training", "");

                    string vsmodelfile = temporaryfiledir + "VSM_MC_" + filename + ".model";
                    TrainVSM(configfiles, trainingtextfile, vsmodelfile);

                    string trainingfeaturefile = temporaryfiledir + Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(trainingtextfile) + "_features_MC.tsv";
                    string testfeaturefile = temporaryfiledir + Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(testtextfile) + "_features_MC.tsv";

                    GeneratorFeatureFileForTextFile(vsmodelfile, trainingtextfile, trainingfeaturefile, true);
                    GeneratorFeatureFileForTextFile(vsmodelfile, testtextfile, testfeaturefile, false);            
                }

                public static void TrainVSMAndTLCModelAndTest(string[] configfiles, string trainingtextfile, string testtextfile, string temporaryfiledir, string testoutputfile)
                {
                    Bot.ML.Common.Utils.FileUtils.CreateOrCleanDir(temporaryfiledir);            

                    string filename = Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(trainingtextfile);
                    filename = filename.Replace("training", "");

                    string vsmodelfile = temporaryfiledir + "VSM_MC_" + filename  + ".model";

                    string tlcmodelfile = temporaryfiledir + "TLC_MC_" + filename + ".zip";
                    string trainingfeaturefile = temporaryfiledir + filename + "_traininig_feature_MC.tsv";

                    MCTrainer executor = new MCTrainer(configfiles, new ModelFilePathInfo(vsmodelfile), new ModelFilePathInfo(tlcmodelfile));
                    executor.GenerateVectorSpace(trainingtextfile);
                    executor.GenerateTrainingFeaturesAndTrain(trainingtextfile, trainingfeaturefile);

                    MCPredictor predictor = new MCPredictor(configfiles, new ModelFilePathInfo(vsmodelfile), new ModelFilePathInfo(tlcmodelfile));
                    predictor.PredictTextFile(testtextfile, testoutputfile);
                }
                */
    }
}
