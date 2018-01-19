using LUTest.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LUTest
{
    public class AssociateTest
    {
        private static void SplitFeatureFile(string inputfile, double percentage, string trainingdatafile, string testdatafile)
        {
            DatasetSpliter Spliter = new DatasetSpliter(inputfile, percentage);
            Spliter.Split(trainingdatafile, testdatafile);
        }

        public static void Split(string textsourcefile, double percentage)
        {
            string traingingfile = textsourcefile.Replace(".tsv", "_training.tsv");
            string testfile = textsourcefile.Replace(".tsv", "_test.tsv");
            SplitFeatureFile(textsourcefile, percentage, traingingfile, testfile);
        }

        /*public static void SaveVSToModelFile(VectorSpace VS, string modelfile)
        {
            string labelfile = modelfile.Replace(".model", "_labels.txt");

            using (StreamWriter w = new StreamWriter(File.Open(labelfile, FileMode.Create), Encoding.UTF8))
            {
                foreach (KeyValuePair<string, int> kv in VS.LabelValues)
                {
                    w.WriteLine(kv.Key + '\t' + kv.Value.ToString());
                }
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(modelfile,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, VS);
            stream.Close();
        }

        public static VectorSpace ReadVSFromModelFile(string modelfile)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(modelfile,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);
            VectorSpace VS = (VectorSpace)formatter.Deserialize(stream);
            stream.Close();

            return VS;
        }
        
        private static void PrintObject(Object obj)
        {
            Type t = obj.GetType();
            bool isDict = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);

            if (isDict)
            {
                IDictionary dict = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));

                foreach (KeyValuePair<Object, Object> entry in dict)
                {
                    PrintObject(entry.Key);
                    PrintObject(entry.Value);
                }
            }
            else
            {
                if (TypeDescriptor.GetProperties(obj).Count > 0)
                {
                    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
                    {
                        string name = descriptor.Name;
                        object value = descriptor.GetValue(obj);
                        Console.Write(name + "=");
                        PrintObject(value);
                    }
                }
                else
                {
                    Console.WriteLine(obj.ToString());
                }
            }
        }
        */
       /* public static void TestDocumentParser(int Id, string Label, string Document)
        {
            //string Document = "我的信用卡卡丢了";
            DocumentParser DocParser = new DocumentParser(new CutterType[]{CutterType.NGRAM});

            DocumentInfo Doc = DocParser.Parse(Id, Label, Document);

            Console.WriteLine(Doc.ToString());
        }
        */

       /* public static void Wordbreak(string query)
        {
            WordCutter breaker = new WordCutter("zh-cn");
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Utterance", query);
            breaker.CutToUniGrams(dict);
        }
 */
    }
}
