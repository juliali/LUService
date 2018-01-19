using Microsoft.MachineLearning;
using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SentimentClassifier.Api;
using Newtonsoft.Json;
using System.Reflection;
using System.Configuration;
using System.Diagnostics;

namespace TLCService
{
    public class Predictor
    {
        private static Predictor instance = new Predictor();
        private static string ResponseFilePath;

        static private PredictionEngine<Example, ScoredExample> predictionEngine;
        static private StreamWriter writer;
        static private Dictionary<string, List<string>> responseItems;
        static private Random rand;
        private const int FeatureCount = 7803;
        const double Threshold = 0.8;
        static private string NeutralLabel = "neutral";
        const int retryMax = 3;

        public static Predictor Instance
        {
            get
            {
                return instance;
            }
        }

        private Predictor()
        {
            //FeatureCount = 6273;
            ResponseFilePath = "~/Resource/response.txt";
            responseItems = new Dictionary<string, List<string>>();
            rand = new Random();
            InitModel();
        }

        public class Result
        {
            public string label;
            public Dictionary<string, double> scoresDetail = new Dictionary<string, double>();
        }

        public class ResponseJson
        {
            public List<Item> responses;
        }

        public class Item
        {
            [JsonProperty("type")]
            public string type;

            [JsonProperty("response")]
            public List<string> response;
        }
        private static int GetNextRand(int count)
        {
            return rand.Next(0, count);
        }
        public static void LoadJson()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (StreamReader r = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(ResponseFilePath)))
            {
                string json = r.ReadToEnd();
                ResponseJson responsejson = JsonConvert.DeserializeObject<ResponseJson>(json);
                foreach (Item kv in responsejson.responses)
                {
                    try
                    {
                        responseItems[kv.type] = kv.response;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }
        

        public string GetAnswer(string query, int needResponse = 0, int useRudeRule = 0)
        {
            if (string.IsNullOrEmpty(query))
            {
                return "";
            }

            Predictor.Result result = Predictor.Predict(query);
            string answer = Predictor.Answer(result, needResponse);
            return answer;
        }
        

        public static string PredictLabel(Result result)
        {
            string label = "";
            double topScore = 0.0;
            foreach (var kv in result.scoresDetail)
            {
                if (kv.Value > topScore)
                {
                    topScore = kv.Value;
                    label = kv.Key;
                }
            }
            if (topScore > Threshold)
            {
                return label;
            }
            return NeutralLabel;
        }
        
        public static string Answer(Result result, int needResponse)
        {
            string label = PredictLabel(result);
            if (needResponse == 0) return label;

            int count = responseItems[label].Count;
            int index = GetNextRand(count);
            return responseItems[label][index];
        }

        public static Result Predict(string doc)
        {
            SentimentClassifier.Data.FeatureInfo feature = SentimentClassifier.Api.Helper.GenerateFeatureForSingleDoc(doc);

            var inputExample = new Example();
            // TODO: populate the example's features.

            int i = 0;
            foreach (var v in feature.Features)
            {
                inputExample.Features[i] = Convert.ToSingle(v);
                i++;
            }

            ScoredExample scores = predictionEngine.Predict(inputExample);

            int index = 0;
            Result result = new Result();
            foreach (var s in scores.Score)
            {
                result.scoresDetail[Helper.FormatLabel(index)] = s;
                ++index;
            }
            return result;
        }

        public static string PredictWithJsonString(string doc)
        {
            Result result = Predict(doc);
            var json = JsonConvert.SerializeObject(result);

            return json;
        }

        /// <summary>
        /// This is the input to the trained model.
        ///
        /// In most pipelines, not all columns that are used in training are also used in scoring. Namely, the label 
        /// and weight columns are almost never required at scoring time. Since TLC doesn't know which columns 
        /// are 'optional' in this sense, all the columns are listed below.
        ///
        /// You are free to remove any fields from the below class. If the fields are not required for scoring, the model 
        /// will continue to work. Otherwise, the exception will be thrown when a prediction engine is created.
        /// </summary>
        private class Example
        {
            public Single Label;

            [VectorType(FeatureCount)]
            public Single[] Features = new Single[FeatureCount];
        }

        /// <summary>
        /// This is the output of the scored model, the prediction.
        /// </summary>
        private class ScoredExample
        {
            // Below columns are produced by the model's predictor.
            [KeyType(Count = 5, Min = 0, Contiguous = true)]
            public UInt32 PredictedLabel;

            [VectorType(5)]
            public Single[] Score;

            // These are all remaining available columns, either supplied as the input, or intermediate
            // columns generated by the transforms. Materializing these columns has a performance cost,
            // so they are commented out. Feel free to uncomment any column that is useful for your scenario.
#if false
            public Single Label;

            [VectorType(FeatureCount)]
            public Single[] Features;
#endif
        }

        /// <summary>
        /// This method demonstrates how to run prediction on one example at a time.
        /// </summary>
        private static void ExamplePredictOne(string modelPath)
        {
            var env = new TlcEnvironment(conc: 1);

            var predictionEngine = CreatePredictionEngine(env, modelPath);


            var inputExample = new Example();
            // TODO: populate the example's features.

            ScoredExample score = predictionEngine.Predict(inputExample);
            // TODO: consume the resulting scores.
        }

        /// <summary>
        /// This method demonstrates how to run prediction for multiple examples as 
        /// a single batch.
        /// </summary>
        //public static void ExamplePredictBatch(string modelPath)
        //{
        //    var env = new TlcEnvironment();

        //    var predictionEngine = CreateBatchPredictionEngine(env, modelPath);

        //    IEnumerable<Example> inputExamples = new[] { new Example(), new Example() };
        //    // TODO: populate the examples' features.

        //    IEnumerable<ScoredExample> scores = predictionEngine.Predict(inputExamples, reuseRowObjects: true);
        //    // TODO: consume the resulting scores.

        //    // Note that 'reuseRowObject' parameter controls whether we create a new example
        //    // object for every row (if false), or we keep returning the same example object 
        //    // over and over (if true). 

        //    // The latter provides massive performance benefits, but if we want to retain all
        //    // the predictions in memory together, 'false' must be used.
        //}

        /// <summary>
        /// This function creates a prediction engine from the model located in the <paramref name="modelPath"/>.
        /// </summary>
        private static PredictionEngine<Example, ScoredExample> CreatePredictionEngine(IHostEnvironment env, string modelPath)
        {
            try
            {
                using (var fs = File.OpenRead(modelPath))
                    return env.CreatePredictionEngine<Example, ScoredExample>(fs);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// This function creates a batch prediction engine from the model located in the <paramref name="modelPath"/>.
        /// </summary>
        private static BatchPredictionEngine<Example, ScoredExample> CreateBatchPredictionEngine(IHostEnvironment env, string modelPath)
        {
            using (var fs = File.OpenRead(modelPath))
                return env.CreateBatchPredictionEngine<Example, ScoredExample>(fs);
        }

        public static void InitModel(string sourceDataFile, string modelPath)
        {
            var env = new TlcEnvironment(conc: 1);
            predictionEngine = CreatePredictionEngine(env, modelPath);
            
            SentimentClassifier.Api.Helper.InitBeforeGenerateFeatureForSingleDoc(sourceDataFile);

            LoadJson();
        }

        public static void InitModel()
        {
            Stopwatch watch = Stopwatch.StartNew();

            string sourceDataFile = System.Web.HttpContext.Current.Server.MapPath("~/Resource/data_training.tsv");
            string modelPath = System.Web.HttpContext.Current.Server.MapPath("~/Resource/model.zip");
            Predictor.InitModel(sourceDataFile, modelPath);
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
    }
}
