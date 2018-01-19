using Bot.ML.Common.Data;
using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bot.Classifier.Multiclassification.AlgorithmExecutor
{      
    public class SimpleTrainTest
    {
        private int FeatureNumber = 0;
        private Dictionary<int, string> LabelValueDict;

        public SimpleTrainTest()
        {
            LabelValueDict = new Dictionary<int, string>();//ApiSamples.Utils.Utils.GetLabelValues();
        }

        private List<ICExample> ReadExamples(string filepath)
        {
            List<ICExample> list = new List<ICExample>();
            string[] lines = File.ReadAllLines(filepath);
            int index = 0;
            foreach(string line in lines)
            {
                if (index == 0)
                {
                    index++;
                    continue;
                }
                else
                {
                    index++;
                }
                
                string[] tmps = line.Split('\t');
                
                this.FeatureNumber = tmps.Length - 1;
                ICExample example = new ICExample();
                example.Label = float.Parse(tmps[0]);
                example.Features = new float[FeatureNumber];
                for (int i = 0; i < FeatureNumber; i ++)
                {
                    example.Features[i] = float.Parse(tmps[i + 1]);
                }
                list.Add(example);                
            }

            return list;
        }

        public void Train(string trainfile, string modelfile)
        {
            List<ICExample> trainExamples = ReadExamples(trainfile);

            // Initialize the environment.
            var host = new TlcEnvironment();

            // Create a data view of the training data.
            var trainingData = host.CreateDataView(trainExamples);
            var featurized = host.CreateTransform("MinMax{col=Features}", trainingData);

            // Train a predictor.
            var trainingExamples = host.CreateExamples(featurized, "Features", "Label");
            var predictor = host.TrainPredictor(/*new LogisticRegression.Arguments { optTol = 0.01f }*/               
                "MultiClassLogisticRegression", trainingExamples);

            // Save to the model file.
            using (var fs = File.Create(modelfile))
            { 
                host.SaveModel(fs, predictor, trainingExamples);
            }
        }

        public void Test(string modelfile, string testfile, string outputfile)
        {
            List<ICExample> testExamples = ReadExamples(testfile);

            using (var env = new TlcEnvironment())
            {
                using (var fs = File.OpenRead(modelfile))
                {
                    var schema = SchemaDefinition.Create(typeof(ICExample));

                    // Modify schema to specify the vector sizes.
                    schema["Features"].ColumnType = new VectorType(NumberType.R4, 1821);

                    var labelschema = SchemaDefinition.Create(typeof(ICScoredData));
                    labelschema["Score"].ColumnType = new VectorType(NumberType.R4, 6);

                    var predictionEngine = //env.CreateSimplePredictionEngine(fs, this.FeatureNumber);
                        env.CreatePredictionEngine<ICExample, ICScoredData>(fs, inputSchemaDefinition: schema, outputSchemaDefinition: labelschema);

                    int totalTestExampleNum = 0;
                    int correctTestExampleNum = 0;
                    using (StreamWriter w = new StreamWriter(File.Open(outputfile, FileMode.Create), Encoding.UTF8))
                    {
                        foreach (ICExample test in testExamples)
                        {
                            ICScoredData result = predictionEngine.Predict(test);
                            float[] score = result.Score;

                            int actualLabel = 0;
                            float possibility = 0;

                            for (int i = 0; i < score.Length; i++)
                            {
                                if (score[i] > possibility)
                                {
                                    actualLabel = i;
                                    possibility = score[i];
                                }
                            }

                            //Console.WriteLine("Expected Label: " + test.Label + ",　Actual Label: " + actualLabel + " (Actual Score: " + result.Score + ")");
                            totalTestExampleNum++;

                            string LabelStr = this.LabelValueDict[actualLabel];
                            w.WriteLine(totalTestExampleNum.ToString() + '\t' + LabelStr);

                            if (actualLabel == (int)test.Label)
                            {
                                correctTestExampleNum++;
                            }
                            else
                            {
                                Console.WriteLine("Expected Label: " + test.Label + ",　Actual Label: " + actualLabel + " (Probability: " + possibility + ")");
                                Console.WriteLine("     " + string.Join(", ", score));
                                Console.WriteLine("");
                            }

                        }
                  
                    float Accurency = (float)correctTestExampleNum / (float)totalTestExampleNum;
                    
                    w.WriteLine("TotalNumber: " + totalTestExampleNum.ToString() + ", CorrectNumber: " + correctTestExampleNum.ToString()  +", Accurency is : " + Accurency.ToString());
                        
                    }
                }
            }

        }            
    }
}
