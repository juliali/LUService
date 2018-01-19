using Bot.ML.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bot.ML.Common.Utils
{
    public class TextUtils
    {
        public static List<ICExample> ReadExamples(string filepath)
        {
            List<ICExample> list = new List<ICExample>();
            string[] lines = File.ReadAllLines(filepath);
            int index = 0;
            foreach (string line in lines)
            {
                if (index == 0)
                {
                    index++;
                    continue;
                }
                else
                {
                    index++;
                }

                string[] tmps = line.Split('\t');

                int FeatureNumber = tmps.Length - 1;
                ICExample example = new ICExample();
                example.Label = float.Parse(tmps[0]);
                example.Features = new float[FeatureNumber];
                for (int i = 0; i < FeatureNumber; i++)
                {
                    example.Features[i] = float.Parse(tmps[i + 1]);
                }
                list.Add(example);
            }

            return list;
        }

        public static List<SourceData> ReadSourceDataFromTextFile(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                return null;
            }
            List<SourceData> cases = new List<SourceData>();

            string[] lines = System.IO.File.ReadAllLines(filepath);

            foreach (string line in lines)
            {
                int Id = -1;
                string[] tmps = line.Split('\t');

                if (tmps.Length < 3)
                {
                    throw new Exception("The text training file is invalid. Its format ought to be 'Id\tUtterance\tLabel'.");
                }

                try
                {
                    Id = int.Parse(tmps[0]);
                }
                catch (Exception)
                {
                    continue;
                }

                SourceData aCase = new SourceData();
                aCase.Id = Id;
                aCase.SessionId = aCase.Id.ToString(); ///
                aCase.Utterance = tmps[1];
                aCase.Label = tmps[2];
                aCase.Tag = 0;                

                cases.Add(aCase);
            }

            return cases;
        }

/*        public static List<LabledTextFileInfo> ReadLabledDataFromTextFile(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                return null;
            }
            List<LabledTextFileInfo> cases = new List<LabledTextFileInfo>();

            string[] lines = System.IO.File.ReadAllLines(filepath);

            foreach (string line in lines)
            {
                int Id = -1;
                string[] tmps = line.Split('\t');

                if (tmps.Length < 3)
                {
                    throw new Exception("The text training file is invalid. Its format ought to be 'Id\tUtterance\tLabel'.");
                }

                try
                {
                    Id = int.Parse(tmps[0]);
                }
                catch (Exception)
                {
                    continue;
                }

                LabledTextFileInfo aCase = new LabledTextFileInfo();
                aCase.Id = Id;
                aCase.Utterance = tmps[1];
                aCase.Label = tmps[2];

                if (tmps.Length > 3)
                {
                    aCase.EdiResult = tmps[3];
                }

                cases.Add(aCase);
            }

            return cases;
        }

*/

        
        public static List<UnlabledTextFileInfo> ReadUnlabledDataFromTextFile(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                return null;
            }
            List<UnlabledTextFileInfo> cases = new List<UnlabledTextFileInfo>();

            string[] lines = System.IO.File.ReadAllLines(filepath);

            foreach (string line in lines)
            {
                int Id = -1;
                string[] tmps = line.Split('\t');

                if (tmps.Length < 2)
                {
                    throw new Exception("The text training file is invalid. Its format ought to be 'Id\tUtterance'.");
                }

                try
                {
                    Id = int.Parse(tmps[0]);
                }
                catch (Exception)
                {
                    continue;
                }

                UnlabledTextFileInfo aCase = new UnlabledTextFileInfo();
                aCase.Id = Id;
                aCase.Utterance = tmps[1];

                if (tmps.Length > 3)
                {
                    aCase.EdiResult = tmps[3];
                }

                cases.Add(aCase);
            }

            return cases;
        }
    }
}
