using Bot.ML.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Bot.ML.Common.Utils
{
    public class FileUtils
    {
        public static string ReadEmbeddedResourceFile(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        public static ClassificationVSMInfo ReadVSFromModelFile(ModelFilePathInfo pathInfo)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = ReadModelAsStream(pathInfo);
                ClassificationVSMInfo model = (ClassificationVSMInfo)formatter.Deserialize(stream);
                stream.Close();

                return model;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static Stream ReadModelAsStream(ModelFilePathInfo pathInfo)
        {
            if (pathInfo.GetType() == typeof(ModelBlobPathInfo))
            {
                return ReadStreamFromBlob((ModelBlobPathInfo)pathInfo);
            }
            else
            {
                return ReadStreamFromLocalFile(pathInfo);
            }
        }

        private static Stream ReadStreamFromLocalFile(ModelFilePathInfo pathInfo)
        {

            Stream stream = new FileStream(pathInfo.FileName,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);
            return stream;
        }

        private static Stream ReadStreamFromBlob(ModelBlobPathInfo pathInfo)
        {
            BlobLoader loader = BlobLoader.GetInstance();
            Stream stream = loader.ReadFileAsStream(pathInfo.ContainerName, pathInfo.FileName);

            return stream;
        }

        public static List<UnlabelExample> ReadUnlabelExampleFromFeatureFile(string featurefilepath)
        {
            List<UnlabelExample> list = new List<UnlabelExample>();

            string[] lines = System.IO.File.ReadAllLines(featurefilepath);

            foreach (string line in lines)
            {
                string[] tmps = line.Split('\t');
                float[] features = new float[tmps.Length];

                try
                {
                    float.Parse(tmps[0]);
                }
                catch (Exception)
                {
                    continue;
                }

                for (int i = 0; i < tmps.Length; i++)
                {

                    features[i] = float.Parse(tmps[i]);

                }

                UnlabelExample example = new UnlabelExample();
                example.Features = features;
                list.Add(example);
            }

            return list;
        }

        public static void CreateOrCleanDir(string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir);
            }

            DirectoryInfo di = Directory.CreateDirectory(dir);
        }



        public static string GetTimestamp()
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
            return timestamp;
        }

        public static string GetFileDirFromPath(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                return null;
            }

            string dir = "";
            string[] strs = filepath.Split('\\');
            for (int i = 0; i < strs.Length - 1; i++)
            {
                dir += strs[i] + '\\';
            }
            return dir;
        }

        public static string GetFileNameFromPath(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                return null;
            }

            string[] strs = filepath.Split('\\');
            string filename = strs[strs.Length - 1];
            filename = filename.Split('.')[0];

            return filename;
        }
    }
}
