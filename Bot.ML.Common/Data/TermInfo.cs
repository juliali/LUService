using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Bot.ML.Common.Data
{


    [Serializable]
    public class TermPositionInfo
    {
        public int DocumentId { get; set; }
        public int UtteranceId { get; set; }
        public int StartIndex { get; set; }
    }

    [Serializable]
    public class RawTermInfo
    {
        public string TermString { get; set; } 

        public int TermLength { get; set; } = 0;        

        public TermPositionInfo Position { get; set; }

        public bool IsResidentTerm { get; set; } = false;
    }

    [Serializable]
    public class DocumentInfo
    {
        public int DocumentId { get; set; }
        public string Label { get; set; }

        public int LabelValue { get; set; } = -1;
        public string DocumentContent { get; set; }
        public Dictionary<int, string> Utterances { get; set; }

        public List<RawTermInfo> RawTerms { get; set; }

        public Dictionary<string, int> TermCounts { get; set; }
    }

    [Serializable]
    public class TermSpaceInfo
    {        

        public string TermString { get; set; }

        public int TermLength { get; set; }

        public int VSIndex { get; set; } = -1;

        public double FeatureValue { get; set; } = -1.0;

        public int DFValue { get; set; } = 0;

        public double Entropy { get; set; } = 0.0;  

        public Dictionary<int, int> DocumentCountMap { get; set; }

        public Dictionary<int, int> CategoryCountMap { get; set; }

        public bool IsResidentTerm { get; set; } = false;


    }

    /*[Serializable]
    public class VectorSpaceWithLabel
    {
        public VectorSpace VS { get; set; }
       
        public Dictionary<string, int> LabelValues { get; set; }
    }
    */

    [Serializable]
    public class VectorSpace
    {
        public string PositiveLabelOfBinaryClassification { get; set; } = null;
        public int TotalDocumentNumber { get; set; }

        public int TotalTermNumber { get; set; } = 0;

        public double AverageFeatureValue { get; set; } = 0.0;
        
        public HashSet<int> DocumentIds { get; set; }
        
        public Dictionary<int, int> DocumentIdsToLabelIds { get; set; }

        public Dictionary<int, HashSet<string>> DocumentIdsToTerms;

        public Dictionary<string, TermSpaceInfo> EvaluatedTerms { get; set; }

        public List<double> EntropyValueList { get; set; }
    }

    public class LabelValueDict
    {
        public string Label;
        public int LabelValue;
    }

    public class DocumentLabelIdDict
    {
        public int DocumentId;
        public int LabelValue;
    }

    public class DocumentTermDict
    {
        public int DocumentId;
        public List<string> TermStrings;
    }

    public class TermShortInfo
    {
        public string TermString { get; set; }

        public int TermLength { get; set; }

        public int VSIndex { get; set; }

        public double FeatureValue { get; set; }
    }

    public class PersistentVS
    {
        public int TotalDocumentNumber { get; set; }

        public int TotalTermNumber { get; set; } = 0;

        public double AverageFeatureValue { get; set; } = 0.0;

        [JsonExtensionData]
        public List<int> DocumentIds { get; set; }

        [JsonExtensionData]
        public List<LabelValueDict> LabelValues { get; set; }

        [JsonExtensionData]
        public List<DocumentLabelIdDict> DocumentIdsToLabelIds { get; set; }

        [JsonExtensionData]
        public List<DocumentTermDict> DocumentIdsToTerms;

        [JsonExtensionData]
        public List<TermShortInfo> EvaluatedTerms { get; set; }

    }

    [Serializable]
    public class UniGram
    {
        public int StartIndex { get; set; }
        public int UniGramLength { get; set; }
        public string GramString { get; set; }

        public bool IsChinese { get; set; } = false;
    }

    [Serializable]
    public class FeatureInfo
    {
        public int DocumentId { get; set; }
        public int LabelValue { get; set; } = -1;
        public double[] Features { get; set; }

        public bool IsValid { get; set; } = false;
        public string Label { get; set; } = null;
        public string Utterance { get; set; }
    }

    

}
