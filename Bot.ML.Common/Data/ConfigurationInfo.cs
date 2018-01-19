using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.ML.Common.Data
{
    [Serializable]
    public class ConfigurationInfo
    {        
        public OptionInfo[] Options;
    }

    [Serializable]
    public class OptionInfo
    {        
        public string Name;
        public string Value;
    }
}
