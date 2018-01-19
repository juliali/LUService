using Bot.ML.Common.Data;
using Bot.VSM.Modules.TextToVector;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bot.VSM.Executor.Convertor
{
    public abstract class VSMConvertor
    {
        protected ClassificationVSMInfo Model;

        public VSMConvertor(ModelFilePathInfo modelfileLocalPath)
        {
            this.Model = Bot.ML.Common.Utils.FileUtils.ReadVSFromModelFile(modelfileLocalPath);
        }

        public VSMConvertor(ClassificationVSMInfo model)
        {
            this.Model = model;
        }
        
        public float[] GenerateFeatures(string Content, string EdiResult)
        {
            FeatureInfo feature = ConvertText(-1, null, Content, EdiResult);
            double[] FeatureValues = feature.Features;

            float[] result = ConvertDoubleArrayToFloatArray(FeatureValues);
            return result;
        }

        protected ICExample ConvertFeatureToExample(FeatureInfo feature)
        {
            string Label = feature.Label;
            ICExample example = new ICExample();
            example.Label = (float)(this.Model.LabelValues.LabelValueDict[Label]);
            example.Features = ConvertDoubleArrayToFloatArray(feature.Features);

            return example;
        }

        protected FeatureInfo ConvertText(int Id, string Label, string Content, string EdiResult)
        {
            List<FeatureInfo> result = new List<FeatureInfo>();

            List<VSMInfo> VSMList = this.Model.VSMWithSelectionPolicyList;
            foreach (VSMInfo VSM in VSMList)
            {                 
                VectorTransformer transformer = new VectorTransformer(VSM, this.Model.LabelValues);
                FeatureInfo features = transformer.Transform(Id, Label, Content, EdiResult);
                result.Add(features);                
            }

            FeatureInfo finalResult = MergeFeature(result);
            return finalResult;
        }

        protected FeatureInfo MergeFeature(List<FeatureInfo> list)
        {
            FeatureInfo result = null;
            List<double> FeaturesList = new List<double>();
            bool isValid = false;

            foreach (FeatureInfo aInfo in list)
            {
                if (result == null)
                {
                    result = new FeatureInfo();
                    result.Label = aInfo.Label;
                    result.LabelValue = aInfo.LabelValue;
                    result.Utterance = aInfo.Utterance;
                }

                FeaturesList.AddRange(aInfo.Features);

                if (aInfo.IsValid)
                {
                    isValid = true;
                }
            }

            result.IsValid = isValid;
            result.Features = FeaturesList.ToArray();

            return result;
        }

        public Dictionary<float, string> GetLabelValueDict()
        {
            Dictionary<string, int> labelValues = this.Model.LabelValues.LabelValueDict;

            Dictionary<float, string> dict = new Dictionary<float, string>();

            foreach (KeyValuePair<string, int> kv in labelValues)
            {
                dict.Add((float)kv.Value, kv.Key);
            }

            return dict;
        }
        

        protected List<ICExample> GenerateBatchFeatures(string PositiveLabel, string NegtiveLabel, List<SourceData> cases, string outputFeatureFile, bool ignoreInvalid)
        {
            string invalidfile = outputFeatureFile.Replace(".tsv", "_valid.tsv");

           // List<LabledTextFileInfo> cases = Bot.ML.Common.Utils.TextUtils.ReadLabledDataFromTextFile(inputtextfile);

            List<ICExample> examples = new List<ICExample>();
            using (StreamWriter w = new StreamWriter(File.Open(outputFeatureFile, FileMode.Create), Encoding.UTF8))
            {
                StreamWriter w2 = null;

                if (ignoreInvalid)
                {
                    w2 = new StreamWriter(File.Open(invalidfile, FileMode.Create), Encoding.UTF8);
                }
                int index = 0;
                foreach (SourceData aCase in cases)
                {
                    string Label = aCase.Label;
                    if (!string.IsNullOrWhiteSpace(PositiveLabel))
                    {
                        if (aCase.Label != PositiveLabel)
                        {
                            Label = NegtiveLabel;
                        }
                    }

                    FeatureInfo Feature = ConvertText(aCase.Id, Label, aCase.Utterance, null);

                    if (index == 0)
                    {
                        string OutputHeader = "Label";
                        for (int j = 0; j < Feature.Features.Length; j++)
                        {
                            OutputHeader += '\t' + "Feature_" + (j + 1).ToString();
                        }
                        w.WriteLine(OutputHeader);

                        index++;
                    }

                    if (ignoreInvalid && !Feature.IsValid)
                    {
                        w2.WriteLine(aCase.Id + '\t' + aCase.Utterance + '\t' + Label);
                        continue;
                    }

                    ICExample example = ConvertFeatureToExample(Feature);
                    examples.Add(example);
                    w.WriteLine(example.ToString());
                }

                if (ignoreInvalid)
                {
                    w.Close();
                }
            }

            return examples;
        }
        
        /*
        public void GenerateFeatureFile(string PositiveLabel, string NegtiveLabel, string inputtextfile, string outputfile, bool ignoreInValid)
        {
            List<LabledTextFileInfo> cases = Utils.Utils.ReadLabledDataFromTextFile(inputtextfile);
            int index = 0;
            using (StreamWriter w = new StreamWriter(File.Open(outputfile, FileMode.Create), Encoding.UTF8))
            {
                StreamWriter w2 = null;

                if (ignoreInValid)
                {
                    string invalidfile = outputfile.Replace(".tsv", "_invalid.tsv");
                    w2 = new StreamWriter(File.Open(invalidfile, FileMode.Create), Encoding.UTF8);
                }

                foreach (LabledTextFileInfo aCase in cases)
                {
                    string Label = aCase.Label;

                    if (!string.IsNullOrWhiteSpace(PositiveLabel))
                    {
                        if (Label != PositiveLabel)
                        {
                            Label = NegtiveLabel;
                        }
                    }

                    FeatureInfo Feature = ConvertText(aCase.Id, Label, aCase.Utterance);

                    string FeatureLine = "";

                    string Header = "";


                    Header = "Label";
                    FeatureLine = Feature.LabelValue + "";

                    for (int i = 0; i < Feature.Features.Length; i++)
                    {
                        if (index == 0)
                        {
                            Header += '\t' + "Feature_" + (i + 1);
                        }

                        FeatureLine += '\t' + Feature.Features[i].ToString();
                    }

                    if (index == 0)
                    {
                        w.WriteLine(Header);
                    }

                    if (ignoreInValid)
                    {
                        if (Feature.IsValid)
                        {
                            w.WriteLine(FeatureLine);
                        }
                        else
                        {
                            if (w2 != null)
                            {
                                w2.WriteLine(Feature.DocumentId.ToString() + '\t' + Feature.Utterance + '\t' + Feature.Label);
                            }
                        }
                    }
                    else
                    {
                        w.WriteLine(FeatureLine);
                    }

                    index++;
                }

                if (w2 != null)
                {
                    w2.Close();
                }
            }
        }
        */
        protected float[] ConvertDoubleArrayToFloatArray(double[] FeatureValues)
        {

            float[] result = new float[FeatureValues.Length];
            int index = 0;

            foreach (double value in FeatureValues)
            {
                result[index++] = (float)value;
            }

            return result;
        }
        
    }
}

