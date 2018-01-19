using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Bot.ML.Common;

namespace Bot.LanguageUnderstanding.MixedLU
{
    class EdiModule : IPureLuModule
    {
        private HttpEdiClient _client;
        private Dictionary<string, List<Tuple<string, string>>> _targetColumns = new Dictionary<string, List<Tuple<string, string>>>();
        private Dictionary<string, List<Tuple<string, string>>> _normalizeConstraint = new Dictionary<string, List<Tuple<string, string>>>();

        private string TargetColumnsPath;
        private string NormalizeConstraintPath;

        public EdiModule(string dir) : base(dir)
        {
        }

        public override void Update(string query, Dictionary<string, PureLuResult> current)
        {
            //Fetch Intent Classifier Results
            var intentLu = current[Constants.INTENT_CLASSIFIER_MODULE_NAME];
   
            if (intentLu.Intents.Any(
                x => x.IntentName == "Payment" 
                || x.IntentName == "Delivery" 
                || x.IntentName == "Promotion"
                || x.IntentName == "MISC"))
            {
                var result = new PureLuResult
                {
                    Source = Constants.EDI_MODULE_Name,
                    RawQuery = query
                };
                current[Constants.EDI_MODULE_Name] = result;
            }
            //Read Edi Model Result
            else
            {
                var luResult = _client.Extract(query);
                luResult.RawQuery = query;
                luResult.Source = Constants.EDI_MODULE_Name;
                PostProcess(ref luResult);

                current[Constants.EDI_MODULE_Name] = luResult;
            }

        }

        internal void PostProcess(ref PureLuResult lu)
        {
            //Target Column Mapping
            DetectTargetColumn(ref lu);

            //Normalization
            Normalize(ref lu);

        }

        internal void Normalize(ref PureLuResult lu)
        {
            //Normalize Intent
            foreach (var intent in lu.Intents)
            {
                if(intent.IntentName == "IsAvailable")
                {
                    intent.IntentName = "AskParameter";
                }
            }

            //Nomalize Constraint
            lu.Segments = lu.Segments.Where(x => x.RawTagName != "Constraint:ProductId").ToList();
           
            var allConstraint = lu.Segments.Where(x => x.RawTagName.StartsWith("Constraint:")).ToList();
            foreach (var segment in allConstraint)
            {
                segment.TagType = "Constraint"; // Change to enum
                segment.TagName = segment.RawTagName.Substring("Constraint:".Length);

                var normalizeConstraintVal = segment.ExtractedValue; //string.Empty;
                if (_normalizeConstraint.ContainsKey(segment.TagName))
                {
                    foreach (var rule in _normalizeConstraint[segment.TagName])
                    {
                        if (!Regex.Match(segment.ExtractedValue, rule.Item1, RegexOptions.IgnoreCase).Success) continue;
                        normalizeConstraintVal = rule.Item2;
                        break;
                    }
                    if (normalizeConstraintVal.Equals(string.Empty))
                    {
                        lu.Segments.Remove(segment);
                        continue;
                    }
                }

                segment.Operator = "like"; // change to enum

                segment.NormalizedValue = normalizeConstraintVal;
            }
        }

        internal void DetectTargetColumn(ref PureLuResult lu)
        {
            var segments = lu.Segments.Where(x => x.RawTagName.StartsWith("Intent:")).ToList();
            int detectedCount = 0;
            if (segments.Any())
            {
                foreach (var segment in segments)
                {
                    string intentName = segment.RawTagName.Substring("Intent:".Length);
                    if (_targetColumns.ContainsKey(intentName))
                    {
                        foreach (var rule in _targetColumns[intentName])
                        {
                            if (rule.Item2 == "*" || Regex.Match(segment.ExtractedValue, rule.Item2, RegexOptions.IgnoreCase).Success)
                            {
                                //Get a target column
                                segment.TagName = rule.Item1;
                                segment.TagType = "Target"; // change to enum
                                lu.Intents.Add(new Intent() { Confidence = 1, IntentName = intentName, SubIntentName = rule.Item1 });
                                detectedCount++;
                            }
                        }
                    }
                }

                if(detectedCount == 0)
                {
                    lu.Segments.RemoveAll(x => x.RawTagName.StartsWith("Intent:"));
                }
            }
        }

        public override void Initialze(dynamic jsonConfig)
        {
            _client = new HttpEdiClient(jsonConfig.EdiHttpEndPoint.Value);

            TargetColumnsPath = jsonConfig.SubIntents.Value;
            NormalizeConstraintPath = jsonConfig.SlotNormalization.Value;

            using (var sr = new StreamReader(GetPath(TargetColumnsPath)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0 || line.Trim().StartsWith("###")) continue;
                    var items = line.Split('\t');
                    //Schema:
                    //Intent\tTargetColumn\tPattern
                    if (items.Length != 3)
                    {
                        throw new Exception("Wrong format of configuration file: ./Edi/TargetColumns.txt");
                    }
                    var intent = items[0];
                    if (_targetColumns.ContainsKey(intent))
                    {
                        _targetColumns[intent].Add(new Tuple<string, string>(items[1], items[2]));
                    }
                    else
                    {
                        _targetColumns.Add(intent, new List<Tuple<string, string>>() { new Tuple<string, string>(items[1], items[2]) });
                    }
                }
            }

            // load normalize conf
            using (var sr = new StreamReader(GetPath(NormalizeConstraintPath)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0 || line.Trim().StartsWith("###")) continue;
                    var items = line.Split('\t');
                    //Schema:
                    //ExtractedValueRegex\tTrueValue
                    if (items.Length != 3)
                    {
                        throw new Exception("Wrong format of configuration file: ./Edi/ConstraintNormalize.txt");
                    }
                    var constrainColumn = items[0];
                    if (_normalizeConstraint.ContainsKey(constrainColumn))
                    {
                        _normalizeConstraint[constrainColumn].Add(new Tuple<string, string>(items[1], items[2]));
                    }
                    else
                    {
                        _normalizeConstraint.Add(constrainColumn, new List<Tuple<string, string>>() { new Tuple<string, string>(items[1], items[2]) });
                    }
                }
            }
        }
    }
}
