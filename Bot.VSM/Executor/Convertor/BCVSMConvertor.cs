using Bot.ML.Common.Data;
using System.Collections.Generic;

namespace Bot.VSM.Executor.Convertor
{
    public class BCVSMConvertor : VSMConvertor
    {
        private string NegtiveLabelString;
        public BCVSMConvertor(ModelFilePathInfo modelfilepath) 
            : base(modelfilepath)
        {
            NegtiveLabelString = this.Model.LabelValues.NegtiveLabel;
        }        

        public BCVSMConvertor(ClassificationVSMInfo model): base(model)
        {
            NegtiveLabelString = this.Model.LabelValues.NegtiveLabel;
        }

        public string GetNegtiveLabel()
        {
            return this.NegtiveLabelString;
        }

        public List<ICExample> GenerateBatchFeatures(string PositiveLabel, List<SourceData> cases, string outputFeatureFile, bool ignoreInvalid)
        {
            /*List<LabledTextFileInfo> cases = Utils.Utils.ReadLabledDataFromTextFile(inputtextfile);

            List<ICExample> examples = new List<ICExample>();

            foreach (LabledTextFileInfo aCase in cases)
            {
                if (string.IsNullOrWhiteSpace(aCase.Label))
                {
                    continue;
                }

                string Label = aCase.Label;
                if (aCase.Label != PositiveLabel)
                {
                    Label = this.NegtiveLabelString;
                }

                FeatureInfo Feature = ConvertText(aCase.Id, Label, aCase.Utterance);

                if (ignoreInvalid && !Feature.IsValid)
                {
                    continue;
                }

                ICExample example = ConvertFeatureToExample(Feature);
                examples.Add(example);
            }

            return examples;
            */

            return GenerateBatchFeatures(PositiveLabel, this.NegtiveLabelString, cases, outputFeatureFile, ignoreInvalid);
        }


       // public void GenerateFeatureFile(string PositiveLabel, string inputtextfile, string outputfile, bool ignoreInvalid)
       // {
       //     GenerateFeatureFile(PositiveLabel, this.NegtiveLabelString, inputtextfile, outputfile, ignoreInvalid);
       // }
/*
        private List<FeatureInfo> GenerateBatchFeatures(string PositiveLabel, string inputtextfile)
        {
            List<LabledTextFileInfo> cases = Utils.Utils.ReadLabledDataFromTextFile(inputtextfile);

            List<FeatureInfo> examples = new List<FeatureInfo>();

            foreach (LabledTextFileInfo aCase in cases)
            {
                if (string.IsNullOrWhiteSpace(aCase.Label))
                {
                    continue;
                }

                string Label = aCase.Label;
                if (aCase.Label != PositiveLabel)
                {
                    Label = this.NegtiveLabelString;
                }

                FeatureInfo Feature = convertor.ExtractFeatures(aCase.Id, Label, aCase.Utterance);
                examples.Add(Feature);
            }

            return examples;
        }
*/
    }
}
