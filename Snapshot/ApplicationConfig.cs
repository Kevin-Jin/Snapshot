using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Snapshot
{
    internal class ApplicationConfig
    {
        private readonly string folder;
        private readonly Dictionary<string, List<string>> extensionsForProcess = new Dictionary<string, List<string>>();

        private ApplicationConfig(string jsonFile = "config.json")
        {
            if (File.Exists(jsonFile))
            {
                using (var file = File.OpenRead(jsonFile))
                using (var cfg = new StreamReader(file))
                {
                    var json = JObject.Parse(cfg.ReadToEnd());
                    folder = Environment.ExpandEnvironmentVariables(json.Value<string>("folderInsideDropbox"));
                    foreach (var association in json.Value<JArray>("associations"))
                        if (extensionsForProcess.ContainsKey(association.Value<string>("process")))
                            extensionsForProcess[association.Value<string>("process")].Add(association.Value<string>("extension"));
                        else
                            extensionsForProcess[association.Value<string>("process")] = new List<string>() { association.Value<string>("extension") };
                }
            }
        }

        internal string Folder { get { return folder; } }

        internal Dictionary<string, List<string>> ExtensionAssociations { get { return extensionsForProcess; } }

        public override string ToString()
        {
            return string.Join(", ", extensionsForProcess.Select(entry => ('(' + entry.Key + " => " + string.Join(", ", entry.Value) + ')')));
        }

        private static readonly ApplicationConfig singleton = new ApplicationConfig();

        internal static ApplicationConfig Instance { get { return singleton; } }
    }
}
