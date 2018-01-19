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
    public class EdiSlotTagger
    {
       
        public void GetResultsForTestFile(string inputfile, string outputfile)
        {
            List<SourceData> inputList = Bot.ML.Common.Utils.TextUtils.ReadSourceDataFromTextFile(inputfile);

            using (StreamWriter w = new StreamWriter(File.Open(outputfile, FileMode.Create), Encoding.UTF8))
            {
                foreach (SourceData example in inputList)
                {
                    string ediResult = GetEdiResult(example.Utterance);

                    if (ediResult == null)
                    {
                        ediResult = "";
                    }

                    string str = example.ToString() + '\t' + ediResult;
                    w.WriteLine(str);
                }
            }
        }

        public string GetEdiResult(string Utterance)
        {
            var url = "http://ediluweb.azurewebsites.net/models/suning_model_11_22_18/query?query=" + Utterance;
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
                return value;
            }
        }
    }
}
