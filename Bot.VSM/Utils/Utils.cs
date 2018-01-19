using System;
using System.Collections.Generic;

namespace Bot.VSM.Utils
{
    public class Utils
    {

        public static Dictionary<string, string> GenInputDict(String Utterance, String EdiResult)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("Utterance", Utterance);

            if (!string.IsNullOrWhiteSpace(EdiResult))
            {
                result.Add("EdiResult", EdiResult);
            }

            return result;
        }
    }
}
