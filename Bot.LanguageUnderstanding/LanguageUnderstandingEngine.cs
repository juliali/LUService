using System;
using Bot.ML.Common;

namespace Bot.LanguageUnderstanding
{
    public interface ILanguageUnderstandingEngine
    {
        MixedLuResult ParseQuery(string query);
    }

    public class LanguageUnderstandingEngine : ILanguageUnderstandingEngine
    {
        public string Oid;

        public LanguageUnderstandingEngine()
        {
            Oid = Guid.NewGuid().ToString();
            Console.WriteLine("Empty LU Engine with ID " + Guid.NewGuid().ToString());
        }  

        public MixedLuResult ParseQuery(string query)
        {
            throw new NotImplementedException();
        }
    }
}
