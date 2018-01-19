using Bot.ML.Common.Data;
using System;
using System.Collections.Generic;


namespace Bot.VSM.Modules.VSGenerator
{
    public class LabelValueGenerator
    {

        public Dictionary<string, int> Generate(List<SourceData> cases, string PositiveLabel, string NegtiveLabelString)
        {
            Dictionary<string, int> LabelValues = new Dictionary<string, int>();            

            VectorSpace VS = new VectorSpace();

            if (!string.IsNullOrWhiteSpace(PositiveLabel))
            {
                LabelValues.Add(PositiveLabel, 1);
                LabelValues.Add(NegtiveLabelString, 0);
            }
            else
            {
                //string[] lines = System.IO.File.ReadAllLines(trainingtextfile);

                foreach (SourceData aCase in cases)
                {
                    string Label = aCase.Label;

                    if (!string.IsNullOrWhiteSpace(PositiveLabel))
                    {
                        if (Label != PositiveLabel)
                        {
                            Label = NegtiveLabelString;
                        }
                    }
                    
                    if (!LabelValues.ContainsKey(Label))
                    {
                        int LabelValue = LabelValues.Count;
                        LabelValues.Add(Label, LabelValue);
                    }
                }
            }

            return LabelValues;
        }
    }
}
