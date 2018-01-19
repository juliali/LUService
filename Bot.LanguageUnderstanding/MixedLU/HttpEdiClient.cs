using System;
using System.IO;
using System.Net;
using System.Web;
using Bot.ML.Common;
using Newtonsoft.Json;

namespace Bot.LanguageUnderstanding.MixedLU
{
    class HttpEdiClient
    {
        private readonly string _httpEndpoint;

        public HttpEdiClient(string endPoint)
        {
            _httpEndpoint = endPoint;
        }

        public PureLuResult Extract(string query)
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
            dynamic obj = JsonConvert.DeserializeObject(content);
            foreach (var s in obj.Segments)
            {
                Segment segment = new Segment()
                {
                    RawTagName = s.Tag.Value,
                    ExtractedValue = s.Value.Value,
                    Confidence = string.IsNullOrEmpty(s.Confidence.Value) ? -1 : double.Parse(s.Confidence)

                };
                result.Segments.Add(segment);
            }

            return result;
        }
    }
}
