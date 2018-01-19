using Bot.VSM.Executor.Convertor;
using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.Data;
using System;
using System.Collections.Generic;
using System.IO;
using Bot.ML.Common.Data;
using System.Threading;

namespace TLCService.BinaryClassification
{    
    public class TLCBCPredictorInParallel 
    {

       // private Dictionary<string, ModelFilePathInfo> vsmodelfilepathmap;
        //private Dictionary<string, ModelFilePathInfo> tlcmodelfilemap;
        private string[] PositiveLabelArray;

        private string NegtiveLabelString;

        private int modelNumber = 0;        

        private Dictionary<string, PredictionEngine<ICExample, BCScoredData>> predictionEngines;
        private Dictionary<string, BCVSMConvertor> convertors;

        public TLCBCPredictorInParallel(Dictionary<string, ModelFilePathInfo> vsmodelfilepathmap,  Dictionary<string, ModelFilePathInfo> tlcmodelfilemap)        
        {
           // this.vsmodelfilepathmap = vsmodelfilepathmap;
           // this.tlcmodelfilemap = tlcmodelfilemap;
            
            HashSet<string> PositiveLabels = new HashSet<string>(vsmodelfilepathmap.Keys);
            this.modelNumber = PositiveLabels.Count;

            this.PositiveLabelArray = new string[modelNumber];
            PositiveLabels.CopyTo(this.PositiveLabelArray);                          

            this.convertors = new Dictionary<string, BCVSMConvertor>();
            this.predictionEngines = new Dictionary<string, PredictionEngine<ICExample, BCScoredData>>();            

            for (int index = 0; index < modelNumber; index++)
            {
                String PositiveLabel = PositiveLabelArray[index];
                

                ModelFilePathInfo vsmodelfile = vsmodelfilepathmap[PositiveLabel];
                ModelFilePathInfo tlcmodelfile = tlcmodelfilemap[PositiveLabel];                

                BCVSMConvertor convertor = new BCVSMConvertor(vsmodelfile);

                if (this.NegtiveLabelString == null)
                {
                    this.NegtiveLabelString = convertor.GetNegtiveLabel();
                }

                int FeatureNumber = this.GetFeatureNumber(convertor);
                PredictionEngine<ICExample, BCScoredData> predictor = CreatePredictionEngine(FeatureNumber, tlcmodelfile);

                this.convertors.Add(PositiveLabel, convertor);
                this.predictionEngines.Add(PositiveLabel, predictor);
            }

        }

        private PredictionEngine<ICExample, BCScoredData> CreatePredictionEngine(int FeatureNumber, ModelFilePathInfo tlcmodelfilepath)
        {
            SchemaDefinition inputschema = SchemaDefinition.Create(typeof(ICExample));
            inputschema["Features"].ColumnType = new VectorType(NumberType.R4, FeatureNumber);

            SchemaDefinition outputschema = SchemaDefinition.Create(typeof(BCScoredData));

            using (var env = new TlcEnvironment())
            {
                Stream fs = Bot.ML.Common.Utils.FileUtils.ReadModelAsStream(tlcmodelfilepath);

                try
                {
                    PredictionEngine<ICExample, BCScoredData> predictionEngine =
                    env.CreatePredictionEngine<ICExample, BCScoredData>(fs, inputSchemaDefinition: inputschema, outputSchemaDefinition: outputschema);

                    fs.Close();
                    return predictionEngine;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return null;

            }
        }

        private int GetFeatureNumber(BCVSMConvertor convertor)
        {
            string testingSentence = "你好";
            float[] featureArray = convertor.GenerateFeatures(testingSentence, null);
            int FeatureNumber = featureArray.Length;
            return FeatureNumber;
        }

        public ICTextPredictionInfo Predict(string Utterance, string EdiResult)
        {
            ManualResetEvent[] doneEvents = new ManualResetEvent[modelNumber];
            ICTextPredictionInfo[] results = new ICTextPredictionInfo[modelNumber];

            for (int i = 0; i < this.modelNumber; i ++)
            {
                doneEvents[i] =  new ManualResetEvent(false);
                results[i] = new ICTextPredictionInfo();
            }

            int index = 0;
            foreach(string PositiveLabel in this.PositiveLabelArray)
            {
                SyncDataTransformerForPredictor data = new SyncDataTransformerForPredictor();
                data.Utterance = Utterance;
                data.EdiResult = EdiResult;
                data.eventWaitHandle = new ManualResetEvent(false);

                TLCSingleModelPredictor predictor = new TLCSingleModelPredictor(PositiveLabel, this.convertors[PositiveLabel], this.predictionEngines[PositiveLabel]);
                
                ThreadPool.QueueUserWorkItem(predictor.Predict, data);
                data.eventWaitHandle.WaitOne();

                results[index++] = data.result;
            }
            
            Console.WriteLine("All calculations are complete.");

            ICTextPredictionInfo result = GetPredictionResult(results);
            return result;
        }

        private ICTextPredictionInfo GetPredictionResult(ICTextPredictionInfo[] resultArray)
        {
            List<ICTextPredictionInfo> results = new List<ICTextPredictionInfo>(resultArray);
            results.RemoveAll(item => item == null);

            if (results.Count == 0)
            {
                ICTextPredictionInfo result = new ICTextPredictionInfo();
                result.LabelString = this.NegtiveLabelString;
                result.Possibility = (float)1.0;

                return result;
            }
            else if (results.Count == 1)
            {
                return results[0];
            }
            else
            {
                ICTextPredictionInfo result = results[0];
                for (int i = 1; i < results.Count; i++)
                {
                    if (result.Possibility < results[i].Possibility)
                    {
                        result = results[i];
                    }
                }

                return result;
            }
        }
        
    }
}
