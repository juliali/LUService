using System;
using System.Collections.Generic;
using System.Configuration;
using Bot.ML.Common;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace Bot.LanguageUnderstanding.MixedLU
{
    public class MixedLuEngine : ILanguageUnderstandingEngine
    {
        private List<IPureLuModule> _modules = new List<IPureLuModule>();

        public MixedLuEngine(string envDir, string configPath, string tenant = "Suning")
        {
            Init(envDir,configPath, tenant);           
        }


        public void Init(string envDir, string configRelPath, string tenant)
        {
            string configPath = Path.Combine(envDir, configRelPath);
            string content = File.ReadAllText(configPath);
            dynamic config = JsonConvert.DeserializeObject(content);

            foreach (var tenantConfig in config.Tenants)
            {
                if(string.Compare(tenant,tenantConfig.TenantName.Value, true)==0)
                {
                    foreach(var moduleConfig in tenantConfig.Modules)
                    {
                        Type moduleType = Assembly.GetExecutingAssembly().GetType(moduleConfig.FullName.Value);
                        Type[] tps = new Type[1] { typeof(string) };
                        object[] obj = new object[1] { envDir };

                        ConstructorInfo ct1 = moduleType.GetConstructor(tps);

                        IPureLuModule module = (IPureLuModule)ct1.Invoke(obj);
                        module.Initialze(moduleConfig);

                        _modules.Add(module);
                    }
                }
            }
        }


        public MixedLuResult ParseQuery(string query)
        {
            var luResults = new Dictionary<string, PureLuResult>();

            #region DebugInfo
            #endregion

            foreach (var module in _modules)
            {
                module.Update(query, luResults);
            }

            return new MixedLuResult { LuResults = luResults };
        }
    }
}
