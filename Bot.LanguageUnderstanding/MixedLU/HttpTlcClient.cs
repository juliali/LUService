using System;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using Bot.ML.Common;
using Bot.ML.Common.Data;

namespace Bot.LanguageUnderstanding.MixedLU
{
    public class HttpTlcClient
    {
        private readonly string _httpEndpoint;

        public HttpTlcClient(string endpoint)
        {
            _httpEndpoint = endpoint;
        }

        public PureLuResult Predict(string query)
        {
            string url = string.Format(_httpEndpoint, HttpUtility.HtmlEncode(query));
            string content;
            PureLuResult result = new PureLuResult();

            try
            {
                //Http Call
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = request.GetResponse();
                var dataStream = response.GetResponseStream();
                // ReSharper disable once AssignNullToNotNullAttribute
                var reader = new StreamReader(dataStream);
                content = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return result;
            }

            //Parse
            ICTextPredictionInfo obj = JsonConvert.DeserializeObject<ICTextPredictionInfo>(content);

            List<Intent> intents = new List<Intent>();
            Intent intent = new Intent();
            intent.IntentName = obj.LabelString;
            intent.Confidence = obj.Possibility;

            intents.Add(intent);

            result.Intents = intents;

            return result;
        }
    }
}
