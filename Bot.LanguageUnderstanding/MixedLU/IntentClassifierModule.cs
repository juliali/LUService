using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Bot.ML.Common;
using System.Linq;
using System;
using Bot.ML.Common.Utils;

namespace Bot.LanguageUnderstanding.MixedLU
{
    class IntentClassifierModule : IPureLuModule
    {
        private string Whitelist;
        private string Blacklist;

        private Dictionary<string, IntentInfo> _intents = new Dictionary<string, IntentInfo>();
        private Dictionary<string, string> _value2ColumnName = new Dictionary<string, string>();
        
        public IntentClassifierModule(string dir) : base (dir)
        {           
        }

        public override void Update(string query, Dictionary<string, PureLuResult> current)
        {
            var pr = new PureLuResult
            {
                Source = Constants.INTENT_CLASSIFIER_MODULE_NAME,
                RawQuery = query,
                Intents = new List<Intent>(),
                Segments = new List<Segment>()
            };
            current[Constants.INTENT_CLASSIFIER_MODULE_NAME] = Classify(query, pr);
        }

        public PureLuResult Classify(string sententce, PureLuResult pr)
        {
            foreach (var intent in _intents.Keys)
            {
                var qtype = _intents[intent].GetQtype(sententce);
                if (!string.IsNullOrEmpty(qtype))
                {
                    var segmentConstraint = new Segment
                    {
                        TagType = "Constraint",
                        TagName = _value2ColumnName.ContainsKey(qtype) ? _value2ColumnName[qtype] : "Qtype",
                        Operator = "=",
                        NormalizedValue = qtype
                    };
                    pr.Segments.Add(segmentConstraint);

                    var i = new Intent
                    {
                        IntentName = intent,
                        SubIntentName = qtype,
                        Confidence = 1
                    };
                    pr.Intents.Add(i);
                    var segmentIntent = new Segment
                    {
                        TagType = "Target",
                        TagName = "Answer"
                    };
                    pr.Segments.Add(segmentIntent);

                    break;
                }
                
            }
            return pr;
        }

        private void LoadData(string file, int type)
        {
            using (var sr = new StreamReader(file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    var parts = line.Split('\t');
                    if (parts.Length >= 3)
                    {
                        if (parts.Length == 4)
                        {
                            _value2ColumnName[parts[1]] = parts[3];
                        }
                        var intentname = parts[0].Trim();
                        var qtype = parts[1].Trim();
                        var regex = parts[2].Trim();

                        if (!_intents.ContainsKey(intentname))
                        {
                            _intents[intentname] = new IntentInfo();
                        }

                        if (type == 1)
                        {
                            _intents[intentname].AddWhiteList(qtype, regex);
                        }
                        else
                        {
                            _intents[intentname].AddBlackList(qtype, regex);
                        }
                    }
                }
            }
        }

        private void LoadData(string sqlConn, string sqlCommand, string keywordSqlCommand)
        {
            DatabaseLoader DBLoader = new DatabaseLoader(sqlConn);
            var rules = DBLoader.Load(sqlCommand);
            var keywordRules = DBLoader.Load(keywordSqlCommand);
            Dictionary<string, string> keywords = new Dictionary<string, string>();
            foreach (var row in keywordRules)
            {
                string word = row["word"];
                string rule = row["RegexRule"];
                foreach (string key in keywords.Keys)
                {
                    rule = rule.Replace(key, keywords[key]);
                }
                keywords.Add(word, rule);
            }

            foreach(var row in rules)
            {
                var intentName = row["IntentName"];
                var subIntentName = row["SubIntentName"];
                var rule = row["RegexRule"];
                var type = row["RuleType"];

                foreach (string key in keywords.Keys)
                {
                    rule = rule.Replace(key, keywords[key]);
                }

                if (!_intents.ContainsKey(intentName))
                {
                    _intents[intentName] = new IntentInfo();
                }

                if (type == "Whitelist")
                {
                    _intents[intentName].AddWhiteList(subIntentName, rule);
                }
                else if(type == "Blacklist")
                {
                    _intents[intentName].AddBlackList(subIntentName, rule);
                }
            }
        }

        public override void Initialze(dynamic jsonConfig)
        {
            string loadMethod = jsonConfig.LoadMethod.Value;
            switch(loadMethod)
            {
                case "DB":
                    {
                        string sqlConn = jsonConfig.SqlConnectionString.Value;
                        string sqlCommand = jsonConfig.SqlCommand.Value;
                        string keywordSqlCommand = jsonConfig.KeywordSqlCommand.Value;
                        LoadData(sqlConn, sqlCommand, keywordSqlCommand);
                        break;                       
                    }
                case "File":
                    {
                        Whitelist = jsonConfig.IntentWhiteList.Value;
                        Blacklist = jsonConfig.IntentBlackList.Value;

                        LoadData(GetPath(Whitelist), 1);
                        LoadData(GetPath(Blacklist), 2);

                        break;
                    }
                default:
                    throw new Exception("Unknow loading method");
            }

            
        }

        private class IntentInfo
        {
            private readonly Dictionary<string, List<string>> _whitelist;
            private readonly Dictionary<string, List<string>> _blacklist;

            public IntentInfo()
            {
                _whitelist = new Dictionary<string, List<string>>();
                _blacklist = new Dictionary<string, List<string>>();
            }

            public void AddWhiteList(string qtype, string regex)
            {
                if (!_whitelist.ContainsKey(qtype))
                {
                    _whitelist[qtype] = new List<string>();
                }
                _whitelist[qtype].Add(regex);
            }

            public void AddBlackList(string qtype, string regex)
            {
                if (!_blacklist.ContainsKey(qtype))
                {
                    _blacklist[qtype] = new List<string>();
                }
                _blacklist[qtype].Add(regex);
            }

            public string GetQtype(string sentence)
            {
                var qtypes = new HashSet<string>();
                foreach (var qtype in _whitelist.Keys)
                {
                    foreach (var regex in _whitelist[qtype])
                    {
                        if (Regex.IsMatch(sentence, regex))
                        {
                            qtypes.Add(qtype);
                            break;
                        }
                    }
                }
                foreach (var qtype in _blacklist.Keys)
                {
                    if (qtypes.Contains(qtype))
                    {
                        foreach (var regex in _blacklist[qtype])
                        {
                            if (Regex.IsMatch(sentence, regex))
                            {
                                qtypes.Remove(qtype);
                                break;
                            }
                        }
                    }
                }
                
                if(qtypes.Count == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return qtypes.First().ToString();
                }
            }

        }

    }
}
