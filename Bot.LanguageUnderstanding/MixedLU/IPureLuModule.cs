using System.Collections.Generic;
using Bot.ML.Common;
using System.IO;

namespace Bot.LanguageUnderstanding.MixedLU
{
    public abstract class IPureLuModule
    {
        public string EnvironmentDirectory;
        public string GetPath(string path)
        {
            return Path.Combine(EnvironmentDirectory, path);
        }

        public IPureLuModule(string dir)
        {
            this.EnvironmentDirectory = dir;
        }

        abstract public void Initialze(dynamic jsonConfig);

        abstract public void Update(string query, Dictionary<string, PureLuResult> current);
    }
}