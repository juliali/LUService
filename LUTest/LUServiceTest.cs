using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LUTest.Data;
using Bot.ML.Common.Data;

namespace LUTest
{
    public class LUServiceTest
    {


        public static void TestFile(string inputfile, string outputfile)
        {
            List<SourceData> cases = Bot.ML.Common.Utils.TextUtils.ReadSourceDataFromTextFile(inputfile);

            using (StreamWriter w = new StreamWriter(File.Open(outputfile, FileMode.Create), Encoding.UTF8))
            {
                w.WriteLine("Id\tUtterance\tExpectedLabel\tActualLabel");

                foreach (SourceData aCase in cases)
                {
                    string predictedIntent = TestSingleUtterance(aCase.Utterance);
                    w.WriteLine(aCase.Id.ToString() + '\t' + aCase.Utterance + '\t' + aCase.Label + '\t' + predictedIntent);
                }
            }
        }

        public static string TestSingleUtterance(string Utterance)
        {
            LUSrvRespInfo resp = QueryLu(Utterance);
            string intentName = resp.LuResults.LUAggregator.Intents[0].IntentName;
            Console.WriteLine(intentName);
            return intentName;
        }

        private static LUSrvRespInfo QueryLu(string Utterance)
        {
            var url = "http://localhost:57277/api/LU?query=" + Utterance;
            Console.WriteLine(url);

            //创建post请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            //接受返回来的数据
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string value = reader.ReadToEnd();
            reader.Close();
            stream.Close();
            response.Close();

            Console.WriteLine(value);
            Console.WriteLine("");

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            else
            {
                LUSrvRespInfo resp = JsonConvert.DeserializeObject<LUSrvRespInfo>(value);                
                return resp;                
            }

        }
    }
}
