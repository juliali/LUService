using Bot.ML.Common.Data;
using Bot.VSM.Executor.Convertor;
using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace TLCService.BinaryClassification
{
    public class SyncDataTransformerForPredictor
    {
        public EventWaitHandle eventWaitHandle;// = new ManualResetEvent(false);
        public string Utterance;
        public string EdiResult;
        public ICTextPredictionInfo result;
    }

    public class TLCSingleModelPredictor
    {
        private PredictionEngine<ICExample, BCScoredData> predictionEngine;
        private BCVSMConvertor convertor;      

        string PositiveLabel;

        public TLCSingleModelPredictor(string PositiveLabel, BCVSMConvertor convertor, PredictionEngine<ICExample, BCScoredData> predictionEngine)           
        {
            this.PositiveLabel = PositiveLabel;

            this.convertor = convertor;

            this.predictionEngine = predictionEngine;          
        }

        
        public void Predict(Object dataTranfer)
        {
            SyncDataTransformerForPredictor data = (SyncDataTransformerForPredictor)dataTranfer;

            float[] features = convertor.GenerateFeatures(data.Utterance, data.EdiResult);

            ICExample example = new ICExample();
            example.Features = features;

            BCScoredData result = predictionEngine.Predict(example);

            ICPredictionOutputInfo actualLabel = GetActualLabelForBinaryClass(PositiveLabel, result);

            ICTextPredictionInfo textLabel = new ICTextPredictionInfo();
            textLabel.Possibility = actualLabel.Possibility;
            textLabel.LabelString = convertor.GetLabelValueDict()[actualLabel.LabelValue];

            if (textLabel.LabelString == PositiveLabel)
            {                
                data.result = textLabel;
            }
            else
            {               
                data.result = null;
            }
            
            data.eventWaitHandle.Set();
        }


        private ICPredictionOutputInfo GetActualLabelForBinaryClass(string PositiveLabel, BCScoredData scoreData)
        {
            ICPredictionOutputInfo result = new ICPredictionOutputInfo();
            result.Possibility = scoreData.Probability;
            result.LabelValue = scoreData.PredictedLabel.RawValue;

            return result;
        }


    }
}

