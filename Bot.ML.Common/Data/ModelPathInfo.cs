using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.ML.Common.Data
{
    public class ModelFilePathInfo
    {
        public ModelFilePathInfo(string fileName)
        {
            this.FileName = fileName;
        }

        public string FileName; //It is also FilePath for local file;
    }

    public class ModelBlobPathInfo : ModelFilePathInfo
    {
        public ModelBlobPathInfo(string containerName, string fileName): base(fileName)
        {
            this.ContainerName = containerName;
        }

        public void SetContainerName(string containerName)
        {
            this.ContainerName = containerName;
        }

        public string ContainerName;
    }
}
