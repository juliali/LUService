using Bot.ML.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.VSM.VSGenerator
{
    enum TFCountType
    {
        TFCountByDocument, TFCountByCategory
    }

    public class VectorSpaceGenerator
    {
        private TFCountType TFCountingType = TFCountType.TFCountByDocument;

        private Dictionary<string, int> LabelValues = null; // Not always availabel

        private HashSet<int> DocumentIds;

        private Dictionary<int, int> DocumentIdsToLabelIds;

        private Dictionary<string, TermSpaceInfo> EvaluatedTerms;

        private Dictionary<int, HashSet<string>> DocumentIdsToTerms;

        private Dictionary<int, HashSet<string>> LabelIdsToTerms;

        private SortedList<string, double> EntropyList;

        private int TotalDocumentNumber = 0;
        
        public VectorSpaceGenerator()
        {
            this.DocumentIds = new HashSet<int>();
            this.DocumentIdsToLabelIds = new Dictionary<int, int>();
            this.DocumentIdsToTerms = new Dictionary<int, HashSet<string>>();
            this.LabelIdsToTerms = new Dictionary<int, HashSet<string>>();
            this.EvaluatedTerms = new Dictionary<string, TermSpaceInfo>();
            this.EntropyList = new SortedList<string, double>();
        }

        public VectorSpaceGenerator(Dictionary<string, int> LabelValues):this()
        {
            this.LabelValues = LabelValues;            
        }

        private int GetLabelValue(string Label)
        {
            if (this.LabelValues != null && this.LabelValues.ContainsKey(Label))
            {
                return this.LabelValues[Label];
            }

            return -1;
        }

        public VectorSpace Generate(List<DocumentInfo> Documents)
        {
            return Generate(Documents, null, null);
        }

        public VectorSpace Generate(List<DocumentInfo> Documents, string PositiveLabel, string NegtiveLabelString)
        {           
            VectorSpace VS = new VectorSpace();            

            foreach (DocumentInfo Document in Documents)
            {
                TotalDocumentNumber++;

                this.DocumentIds.Add(Document.DocumentId);

                string Label = Document.Label;

                if (!string.IsNullOrWhiteSpace(PositiveLabel))
                {
                    if (Label != PositiveLabel)
                    {
                        Label = NegtiveLabelString;
                    }
                }

                int LabelValue = GetLabelValue(Label);//this.LabelValues[Label];                
                               
                Document.LabelValue = LabelValue;

                this.DocumentIdsToLabelIds.Add(Document.DocumentId, LabelValue);

                HashSet<string> TermsInDocument = new HashSet<string>();

                foreach (RawTermInfo RawTerm in Document.RawTerms)
                {
                    string TermString = RawTerm.TermString;

                    TermsInDocument.Add(TermString);

                    TermSpaceInfo TermInSpace = new TermSpaceInfo();                    
                    TermInSpace.TermString = TermString;
                    TermInSpace.TermLength = RawTerm.TermLength;

                    TermInSpace.IsResidentTerm = RawTerm.IsResidentTerm; // If it is Resident Term

                    TermInSpace.DocumentCountMap = new Dictionary<int, int>();
                    TermInSpace.CategoryCountMap = new Dictionary<int, int>();

                    if (this.EvaluatedTerms.ContainsKey(TermString))
                    {
                        TermInSpace = this.EvaluatedTerms[TermString];
                    }
                    else
                    {
                        this.EvaluatedTerms.Add(TermString, TermInSpace);
                    }

                    int DocumentId = RawTerm.Position.DocumentId;

                    if (TermInSpace.DocumentCountMap.ContainsKey(DocumentId))
                    {
                        int Count = TermInSpace.DocumentCountMap[DocumentId] + 1;
                        TermInSpace.DocumentCountMap[DocumentId] = Count;
                    }
                    else
                    {
                        TermInSpace.DocumentCountMap.Add(DocumentId, 1);
                    }

                    ///
                    if (TermInSpace.CategoryCountMap.ContainsKey(LabelValue))
                    {
                        int CategoryCount = TermInSpace.CategoryCountMap[LabelValue] + 1;
                        TermInSpace.CategoryCountMap[LabelValue] = CategoryCount;
                    }
                    else
                    {
                        TermInSpace.CategoryCountMap.Add(LabelValue, 1);
                    }
                    ///

                }

                this.DocumentIdsToTerms.Add(Document.DocumentId, TermsInDocument);

                ///
                HashSet<string> TermsInLabel = new HashSet<string>(TermsInDocument);

                if (this.LabelIdsToTerms.ContainsKey(LabelValue))
                {
                    HashSet<string> NewTerms = this.LabelIdsToTerms[LabelValue];
                    NewTerms.UnionWith(TermsInLabel);

                    this.LabelIdsToTerms[LabelValue] = NewTerms;

                }
                else
                {
                    this.LabelIdsToTerms.Add(LabelValue, TermsInLabel);
                }
                ///
            }

            int VSIndex = 0;

            double TotalFeatureValues = 0.0;
            foreach (DocumentInfo Document in Documents)
            {
                int DocumentId = Document.DocumentId;
                int LabelValue = this.DocumentIdsToLabelIds[DocumentId];

                foreach (string TermString in this.DocumentIdsToTerms[DocumentId])
                {
                    if (this.EvaluatedTerms[TermString].FeatureValue == -1.0)
                    {
                        double FeatureValue = this.CalculateTFIDF(DocumentId, LabelValue, TermString);
                        this.EvaluatedTerms[TermString].FeatureValue = FeatureValue;
                        this.EvaluatedTerms[TermString].VSIndex = VSIndex++;
                        this.EvaluatedTerms[TermString].DFValue = this.CalculateDF(TermString);

                        double Entropy = 0.0;
                        if (!this.EntropyList.ContainsKey(TermString))
                        {
                            Entropy = this.CalculateEntropy(TermString);
                            this.EntropyList.Add(TermString, Entropy);
                        }
                        else
                        {
                            Entropy = this.EntropyList[TermString];
                        }

                        this.EvaluatedTerms[TermString].Entropy = Entropy;

                        TotalFeatureValues += FeatureValue;
                    }
                }
            }



            VS.DocumentIds = this.DocumentIds;
            
            VS.DocumentIdsToLabelIds = this.DocumentIdsToLabelIds;
            VS.DocumentIdsToTerms = this.DocumentIdsToTerms;
          
            VS.EvaluatedTerms = this.EvaluatedTerms;
            VS.TotalDocumentNumber = this.TotalDocumentNumber;

            VS.EntropyValueList = new List<double>();
            List<KeyValuePair<string, double>> tmpList = this.EntropyList.OrderBy(kvp => kvp.Value).ToList();
            foreach(KeyValuePair<string, double> kv in tmpList)
            {
                VS.EntropyValueList.Add(kv.Value);
            }

            VS.TotalTermNumber = VSIndex + 1;
            VS.AverageFeatureValue = TotalFeatureValues / ((double)VS.TotalTermNumber);

            VS.PositiveLabelOfBinaryClassification = PositiveLabel;            

            return VS;
        }

        private double CalculateTFIDF(int DocumentId, int LabelValue, string TermString)
        {
            double TFValue = 0.0;

            if (TFCountingType == TFCountType.TFCountByDocument)
            {
                TFValue = CalculateTFInDocument(DocumentId, TermString); /////
            }
            else
            {
                TFValue = CalculateTFInLabel(LabelValue, TermString);
            }

            double IDFValue = CalculateIDF(TermString);

            double result = TFValue * IDFValue;
            return result;
        }

        private double CalculateTFInDocument(int DocumentId, string TermString)
        {
            int TermCount = this.EvaluatedTerms[TermString].DocumentCountMap[DocumentId];
            int AllTermCount = 0;

            HashSet<string> Terms = this.DocumentIdsToTerms[DocumentId];

            foreach(string ATerm in Terms)
            {
                AllTermCount += this.EvaluatedTerms[ATerm].DocumentCountMap[DocumentId];
            }

            double TFValue = (double)TermCount / (double)AllTermCount;
            return TFValue;
        }

        private double CalculateTFInLabel(int LabelValue, string TermString)
        {
            int TermCount = this.EvaluatedTerms[TermString].CategoryCountMap[LabelValue];
            int AllTermCount = 0;

            HashSet<string> Terms = this.LabelIdsToTerms[LabelValue];

            foreach (string ATerm in Terms)
            {
                AllTermCount += this.EvaluatedTerms[ATerm].CategoryCountMap[LabelValue];                    
            }

            double TFValue = (double)TermCount / (double)AllTermCount;
            return TFValue;
        }

        private double CalculateIDF(string TermString)
        {
            TermSpaceInfo TermInSpace = this.EvaluatedTerms[TermString];
            int NumberOfDocContainsTerm = TermInSpace.DocumentCountMap.Count;
            double IDFValue = Math.Log(((double)this.TotalDocumentNumber / (double)NumberOfDocContainsTerm));

            return IDFValue;
        }

        private int CalculateDF(string TermString)
        {
            TermSpaceInfo TermInSpace = this.EvaluatedTerms[TermString];
            int NumberOfDocContainsTerm = TermInSpace.DocumentCountMap.Count;
            return NumberOfDocContainsTerm;
        }

        private double CalculateEntropy(string TermString)
        {
            double Entropy = 0.0;

            if (this.LabelValues == null)
            {
                return Entropy;
            }
           
            foreach(int LabelValue in this.LabelValues.Values.ToArray())
            {
                double Possibility = CalculateTermPossibilityPerLabel(LabelValue, TermString);
                Entropy += -1.0 * Possibility * Math.Log(Possibility);
            }

            return Entropy;
        }
  
        private double CalculateTermPossibilityPerLabel(int LabelValue, string TermString)
        {            
            TermSpaceInfo TermInSpace = this.EvaluatedTerms[TermString];
            Dictionary<int, int> CategoryCountMap = TermInSpace.CategoryCountMap;

            int ThisCategoryCount = 0;
            if (CategoryCountMap.ContainsKey(LabelValue))
            {
                ThisCategoryCount = CategoryCountMap[LabelValue];
            }

            double Numerator = Math.Exp((double) ThisCategoryCount);

            double Denominator = 0.0;

            foreach (int ALabelValue in this.LabelValues.Values)
            {
                int count = 0;

                if (CategoryCountMap.ContainsKey(ALabelValue))
                {
                    count = CategoryCountMap[ALabelValue];
                }

                Denominator += Math.Exp((double) count);
            }

            double Possibility = Numerator / Denominator;
            return Possibility;
        }
       
    }
}
