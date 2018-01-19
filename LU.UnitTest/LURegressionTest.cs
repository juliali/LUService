using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bot.LanguageUnderstanding.MixedLU;
using System.IO;
using Bot.ML.Common;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace LU.UnitTest
{
    [TestClass]
    public class MixedLUEngineUnitTest
    {
        [TestMethod]
        public void SuningIntentRegressionTest()
        {       
            StreamReader reader = new StreamReader("Data\\Intents.txt");
            StreamWriter writer = new StreamWriter("Results.txt");
            string configPath = "Data/MixedLUEngine.json";
            MixedLuEngine luEngine = new MixedLuEngine(System.Environment.CurrentDirectory, configPath);
            //TODO: To finish the regression Test
            int total = 0;
            int error = 0;
            while(!reader.EndOfStream)
            {
                total++;

                string [] arr = reader.ReadLine().Split('\t');
                string sessionId = arr[0];
                string query = arr[1];
                string expectedIntent = arr[2];
                string expectedSubIntent =(string.IsNullOrEmpty(arr[3]) ? null : arr[3]);

                try
                {

                    MixedLuResult result = luEngine.ParseQuery(query);
                    string intent = result.LuResults["LUAggregator"].Intents.First().IntentName;
                    string subIntent = result.LuResults["LUAggregator"].Intents.First().SubIntentName;

                    if (intent != expectedIntent || subIntent != expectedSubIntent)
                    {
                        error++;

                        writer.WriteLine(query + "\t" + intent + "_" + subIntent + "\t" + expectedIntent + "_" + expectedSubIntent);
                        writer.Flush();
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    Assert.Fail();
                }
            }
            writer.Close();

            Assert.IsTrue(error * 1.0 / total < 0.05);

        }

    }
}
