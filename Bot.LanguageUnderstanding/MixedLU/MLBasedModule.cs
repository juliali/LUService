using System.Collections.Generic;
using Bot.ML.Common;
using System;
using Newtonsoft.Json;
using Bot.ML.Common.Data;

namespace Bot.LanguageUnderstanding.MixedLU
{
    public class MLBasedModule : IPureLuModule
    {
        private HttpTlcClient _client;

        public MLBasedModule(string dir) : base (dir)
        {
        }

        public override void Update(string query, Dictionary<string, PureLuResult> current)
        {
            PureLuResult ediLuResult = current[Constants.EDI_MODULE_Name];

            string ediResultStr = null;
            if (ediLuResult != null)
            {
                EdiInfo ediInfo = new EdiInfo();
                ediInfo.Text = query;
                foreach (Segment segment in ediLuResult.Segments)
                {
                    SlotInfo slot = new SlotInfo();
                    slot.Tag = segment.RawTagName;
                    slot.Value = segment.ExtractedValue;
                    slot.Length = slot.Value.Length;
                    slot.Start = query.IndexOf(slot.Value);
                    slot.End = slot.Start + slot.Length - 1;
                    Console.WriteLine(segment.RawTagName);
                }
                ediResultStr = JsonConvert.SerializeObject(ediInfo);
            }

            var result = PredictUtterance(query, ediResultStr);
            result.Source = Constants.MLBASED_MODULE_NAME;
            result.RawQuery = query;
            current[Constants.MLBASED_MODULE_NAME] = result;
        }

        private PureLuResult PredictUtterance(string utterance, string ediResult)
        {
            PureLuResult pureResult = _client.Predict(utterance);

            return pureResult;
        }

        public override void Initialze(dynamic jsonConfig)
        {
            _client = new HttpTlcClient(jsonConfig.TLCHttpEndPoint.Value);
        }
    }
}