using System.Collections.Generic;


namespace Bot.ML.Common.Data
{
    
    public class BCModelInfo
    {
        public string PositiveLabel;
        public ModelBlobPathInfo VSModelPath;
        public ModelBlobPathInfo TLCModelPath;
    }

    public class BCConfigInfo
    {
        public string NegtiveLabel;
        public string[] VSMConfigPaths;
        public BCModelInfo[] Models;
       
        public HashSet<string> GetPositiveLabels()
        {
            if (Models.Length == 0)
            {
                return null;
            }           

            HashSet<string> PositiveLabels = new HashSet<string>();
            foreach (BCModelInfo Model in Models)
            {
                PositiveLabels.Add(Model.PositiveLabel);                
            }

            return PositiveLabels;
        }         
    }
}
