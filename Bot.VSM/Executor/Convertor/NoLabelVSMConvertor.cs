using Bot.ML.Common.Data;
using Bot.VSM.Modules.TextToVector;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bot.VSM.Executor.Convertor
{
    public class NoLabelVSMConvertor :VSMConvertor
    {
        public NoLabelVSMConvertor(ModelFilePathInfo modelfile) : base(modelfile)
        {
        }

        public new float[] GenerateFeatures(string Content, string EdiResult)
        {
            FeatureInfo feature = ConvertText(-1, Content, EdiResult);
            double[] FeatureValues = feature.Features;

            float[] result = ConvertDoubleArrayToFloatArray(FeatureValues);
            return result;
        }

        protected FeatureInfo ConvertText(int Id, string Content, string EdiResult)
        {
            List<FeatureInfo> result = new List<FeatureInfo>();

            List<VSMInfo> VSMList = this.Model.VSMWithSelectionPolicyList;
            foreach (VSMInfo VSM in VSMList)
            {
                VectorTransformer transformer = new VectorTransformer(VSM);
                FeatureInfo features = transformer.Transform(Id, Content, EdiResult);
                result.Add(features);
            }

            FeatureInfo finalResult = MergeFeature(result);
            return finalResult;
        }

        public List<UnlabelExample> GenerateBatchFeatures(string inputtextfile, string outputFeatureFile)
        {            
            List<UnlabledTextFileInfo> cases = Bot.ML.Common.Utils.TextUtils.ReadUnlabledDataFromTextFile(inputtextfile);

            List<UnlabelExample> examples = new List<UnlabelExample>();
            using (StreamWriter w = new StreamWriter(File.Open(outputFeatureFile, FileMode.Create), Encoding.UTF8))
            {
               
                
                int index = 0;
                foreach (UnlabledTextFileInfo aCase in cases)
                {                   

                    FeatureInfo Feature = ConvertText(aCase.Id, aCase.Utterance, aCase.EdiResult);

                    if (index == 0)
                    {
                        string OutputHeader = "";
                        for (int j = 0; j < Feature.Features.Length; j++)
                        {
                            OutputHeader += "Feature_" + (j + 1).ToString();
                            if(j < Feature.Features.Length - 1)
                            {
                                OutputHeader += '\t';
                            }
                        }
                        w.WriteLine(OutputHeader);

                        index++;
                    }

                    UnlabelExample example = ConvertFeatureToUnlabelExample(Feature);
                    examples.Add(example);
                    w.WriteLine(example.ToString());
                }
                
            }

            return examples;
        }

        private UnlabelExample ConvertFeatureToUnlabelExample(FeatureInfo feature)
        {
            string Label = feature.Label;
            UnlabelExample example = new UnlabelExample();
            example.Features = ConvertDoubleArrayToFloatArray(feature.Features);

            return example;
        }
    }
}
