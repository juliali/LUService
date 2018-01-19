using Bot.ML.Common.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LUTest
{
    public class TLCServiceTest
    {
        public static void TestFile(string controller, string inputfile, string outputfile)
        {
            List<SourceData> cases = Bot.ML.Common.Utils.TextUtils.ReadSourceDataFromTextFile(inputfile);

            using (StreamWriter w = new StreamWriter(File.Open(outputfile, FileMode.Create), Encoding.UTF8))
            {
                w.WriteLine("Id\tUtterance\tExpectedLabel\tActualLabel");

                foreach (SourceData aCase in cases)
                {
                    DateTime time1 = DateTime.Now;

                    string predictedIntent = TestSingleUtterance(controller, aCase.Utterance);

                    DateTime time2 = DateTime.Now;

                    long latency = time2.Subtract(time1).Milliseconds;

                    w.WriteLine(aCase.Id.ToString() + '\t' + aCase.Utterance + '\t' + aCase.Label + '\t' + predictedIntent + '\t' + latency.ToString());
                }
            }
        }

        public static string TestSingleUtterance(string controller, string Utterance)
        {
            ICTextPredictionInfo resp = QueryLu(controller, Utterance);
            string intentName = resp.LabelString;
            //Console.WriteLine(intentName);
            return intentName;
        }

        private static ICTextPredictionInfo QueryLu(string controller, string Utterance)
        {
            var url = "http://tlcservice.azurewebsites.net/api/" + controller + "?Utterance=" + Utterance;
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
                ICTextPredictionInfo resp = JsonConvert.DeserializeObject<ICTextPredictionInfo>(value);
                return resp;
            }

        }
    }
}
