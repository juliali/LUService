using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Bot.ML.Common;

namespace Bot.LanguageUnderstanding.MixedLU
{
    class RuleBasedExtractorModule : IPureLuModule
    {
        private string extFile;
        private string cityFile;
        private Dictionary<string, ConstraintInfo> _constraints = new Dictionary<string, ConstraintInfo>();
        private List<Rule> _rules = new List<Rule>();
        private Dictionary<string, string> _cityDict = new Dictionary<string, string>();
        private const string Seperator = "###";

        public RuleBasedExtractorModule(string dir) : base (dir)
        {
        }
        
        public override void Update(string query, Dictionary<string, PureLuResult> current)
        {
            var pr = new PureLuResult
            {
                Source = Constants.RULEBASED_EXTEACTOR_MODULE_NAME,
                RawQuery = query,
                Intents = new List<Intent>(),
                Segments = new List<Segment>()
            };
            current[Constants.RULEBASED_EXTEACTOR_MODULE_NAME] = Extract(query, pr);
        }

        public PureLuResult Extract(string sententce, PureLuResult pr)
        {

            var segments = Extract(sententce);

            segments = Normalize(segments);

            pr.Segments.AddRange(segments);

            return pr;
        }

        private IEnumerable<Segment> Normalize(IEnumerable<Segment> segments)
        {
            foreach (var seg in segments)
            {
                Segment result = new Segment
                {
                    TagType = "Constraint",
                    Operator = "like",
                    TagName = seg.TagName,
                    RawTagName = seg.TagName,
                    ExtractedValue = seg.ExtractedValue,
                    NormalizedValue = seg.NormalizedValue,
                    Confidence = seg.Confidence
                };

                if (seg.TagName == "Price")
                {
                    result = NormalizePriceTag(seg);
                }

                else if(seg.TagName == "City")
                {
                    result = NormalizeCityTag(seg);
                }

                if(result != null)
                {
                    yield return result;
                }
            }
        }

        Segment NormalizePriceTag(Segment seg)
        {
            Segment result = new Segment()
            {
                TagType = "Constraint",
                Operator = "filter",
                TagName = "Price"
            };

            if (seg.NormalizedValue.Contains(Seperator))
            {
                string[] arr = seg.NormalizedValue.Split(new[] { Seperator }, StringSplitOptions.RemoveEmptyEntries);
                //Does not get the price range
                if (arr.Length != 2)
                {
                    return null;
                }

                var priceValue1 = NormalizePrice(arr[0]);
                var priceValue2 = NormalizePrice(arr[1]);

                if (priceValue1 == -1 || priceValue2 == -1)
                {
                    return null;
                }

                seg.NormalizedValue = priceValue1 + "-" + priceValue2;

            }
            else
            {
                var priceValue = NormalizePrice(seg.NormalizedValue);
                if (priceValue == -1)
                {
                    return null;
                }


                if (seg.ExtractedValue.Contains("上") || seg.ExtractedValue.Contains("高") || seg.ExtractedValue.Contains("大于"))
                {
                    seg.NormalizedValue = priceValue + "-10000";
                }
                if (seg.ExtractedValue.Contains("下") || seg.ExtractedValue.Contains("低") || seg.ExtractedValue.Contains("小于"))
                {
                    seg.NormalizedValue = "0-" + priceValue;
                }
                if (seg.ExtractedValue.Contains("左右") || seg.ExtractedValue.Contains("大约"))
                {
                    seg.NormalizedValue = (priceValue * 0.80) + "-" + (priceValue * 1.2);
                }
            }

            result.ExtractedValue = seg.ExtractedValue;
            result.NormalizedValue = seg.NormalizedValue;

            return result;
        }

        int NormalizePrice(string value)
        {
            value = value.Replace("千", "000");
            value = value.Replace("一", "1");
            value = value.Replace("二", "2");
            value = value.Replace("三", "3");
            value = value.Replace("四", "4");
            value = value.Replace("五", "5");
            value = value.Replace("六", "6");
            value = value.Replace("七", "7");
            value = value.Replace("八", "8");
            value = value.Replace("九", "9");

            int v;
            if (int.TryParse(value, out v))
                return v;
            else return -1;

        }

        Segment NormalizeCityTag(Segment seg)
        {
            Segment result = new Segment()
            {
                TagType = "Constraint",
                Operator = "like"
            };

            result.RawTagName = "City";
            result.TagName = "CityCode";
            result.ExtractedValue = seg.ExtractedValue;
            result.NormalizedValue = _cityDict[seg.ExtractedValue];

            return result;
        }

        public IEnumerable<Segment> Extract(string query)
        {
            foreach(var rule in _rules)
            {
                Match match = Regex.Match(query, rule.RegRule);
                if (match.Success)
                {
                    string value = match.Groups["Target"].Value;
                    if (string.IsNullOrEmpty(value))
                    {
                        value = match.Groups["Target1"].Value + Seperator + match.Groups["Target2"].Value;
                    }

                    var seg = new Segment
                    {
                        TagName = rule.TagName,
                        ExtractedValue = match.Groups[0].Value,
                        NormalizedValue = value
                    };
                    yield return seg;
                }
            }
        }
        
        private void LoadRule(string file)
        {
            using (var sr = new StreamReader(file,System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("###"))
                    {
                        continue;
                    }
                    var parts = line.Split('\t');
                    if (parts.Length == 2)
                    {
                        var constraintName = parts[0].Trim();
                        var regex = parts[1].Trim();

                        Rule rule = new Rule {TagName = constraintName, RegRule = regex };
                        _rules.Add(rule);
                    }
                }
            }
        }

        private void LoadCityInfo(string file)
        {
            using (var sr = new StreamReader(file, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("###"))
                    {
                        continue;
                    }
                    var parts = line.Split('\t');
                    if (parts.Length == 2)
                    {
                        var cityCode = parts[0].Trim();
                        var city = parts[1].Trim();

                        _cityDict.Add(city, cityCode);
                    }
                }
            }
        }

        public override void Initialze(dynamic jsonConfig)
        {
            extFile = jsonConfig.ExtFile.Value;
            cityFile = jsonConfig.CityFile.Value;

            LoadRule(GetPath(extFile));
            LoadCityInfo(GetPath(cityFile));
        }

        internal class Rule
        {
            public string TagName;
            public string RegRule;
        }
        
        private class ConstraintInfo
        {
            private readonly Dictionary<string, string> _constraintRegex;

            public ConstraintInfo()
            {
                _constraintRegex = new Dictionary<string, string>();
            }

            // ReSharper disable once UnusedMember.Local
            public void AddWhiteList(string regex, string value)
            {
                _constraintRegex[regex] = value;
            }

            //Return extracted value, Normalized Value
            // ReSharper disable once UnusedMember.Local
            public HashSet<Tuple<string,string>> GetConstraint(string sentence)
            {
                var values = new HashSet<Tuple<string, string>>();
                foreach (var regex in _constraintRegex.Keys)
                {
                    foreach (Match match in Regex.Matches(sentence, regex))
                    {
                        var tuple = new Tuple<string, string>(match.Groups[0].Value, Regex.Replace(_constraintRegex[regex], "#replace#", match.Groups[match.Groups.Count - 1].Value));
                        values.Add(tuple);
                    }
                }     
                return values;
            }

        }
    }
}
