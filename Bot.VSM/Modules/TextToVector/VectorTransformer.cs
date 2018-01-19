using Bot.ML.Common.Data;
using Bot.VSM.Modules.DocProcessor;
using System;
using System.Collections.Generic;

namespace Bot.VSM.Modules.TextToVector
{
    public class VectorTransformer
    {

        private Dictionary<string, int> LabelValueDict = null;
        private string PositiveLabelOfBinaryClassification = null;
        private string NegtiveLabelString = null;

        private Dictionary<string, TermShortInfo> FilteredVS;
        private int TotalTermNumber;        
        private string ConfigFilePath;

        public VectorTransformer(VSMInfo VSM)
        {
            VectorSpaceFilter Filter = new VectorSpaceFilter(VSM);
            this.FilteredVS = Filter.Filter();

            this.TotalTermNumber = this.FilteredVS.Count;

            this.PositiveLabelOfBinaryClassification = VSM.VS.PositiveLabelOfBinaryClassification;
            this.ConfigFilePath = VSM.VSMConfigFilePath;
        }

        public VectorTransformer(VSMInfo VSM, LabelValueInfo LabelValues)
            :this(VSM)
        {                        
            this.LabelValueDict = LabelValues.LabelValueDict;
            this.NegtiveLabelString = LabelValues.NegtiveLabel;           
        }

        public FeatureInfo Transform(int DocumentId, string DocumentStr, string EdiResult)
        {
            return Transform(DocumentId, null, DocumentStr, EdiResult);
        }

        public FeatureInfo Transform(int DocumentId, string LabelStr, string DocumentStr, string EdiResult)
        {
            FeatureInfo FeaturesOfDoc = new FeatureInfo();
            FeaturesOfDoc.DocumentId = DocumentId;
            try
            {
                if (!string.IsNullOrWhiteSpace(LabelStr))
                {
                    if (!string.IsNullOrWhiteSpace(this.PositiveLabelOfBinaryClassification))
                    {
                        if (LabelStr != this.PositiveLabelOfBinaryClassification)
                        {
                            LabelStr = this.NegtiveLabelString;
                        }
                    }

                    FeaturesOfDoc.LabelValue = this.LabelValueDict[LabelStr];
                    FeaturesOfDoc.Label = LabelStr;
                }

                FeaturesOfDoc.Utterance = DocumentStr;
            }
            catch (Exception e)
            {
                throw e;
            }

            double[] Features = new double[this.TotalTermNumber];
            for (int i = 0; i < this.TotalTermNumber; i++)
            {
                Features[i] = 0.0;
            }

            
            DocumentParser Parser = new DocumentParser(ConfigFilePath);

           // Dictionary<string, string> InputDict = Utils.Utils.GenInputDict(DocumentStr, EdiResult);

            DocumentInfo DocInfo = Parser.Parse(DocumentId, LabelStr, DocumentStr, EdiResult);

            foreach (RawTermInfo RawTerm in DocInfo.RawTerms)
            {
                string TermStr = RawTerm.TermString;
                if (this.FilteredVS.ContainsKey(TermStr))
                {
                    double FeatureValue = this.FilteredVS[TermStr].FeatureValue;
                    int VSIndex = this.FilteredVS[TermStr].VSIndex;

                    Features[VSIndex] += FeatureValue; ///// counting TF got higher macro and micro avg accurency
                }
            }

            FeaturesOfDoc.Features = Features;

            foreach (double value in Features)
            {
                if (value != 0)
                {
                    FeaturesOfDoc.IsValid = true;
                    break;
                }
            }

            return FeaturesOfDoc;
        }
    }
}
