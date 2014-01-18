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
        private readonly Dictionary<string, string> processForExtension = new Dictionary<string,string>();

<<<<<<< HEAD
        internal ApplicationConfig(string jsonFile = "config.json.cfg")
=======
        private ApplicationConfig(string jsonFile = "config.json")
>>>>>>> cb584ed2f180dfcb052f90cd169f74ef4c96f449
        {
            if (File.Exists(jsonFile))
            {
                using (var file = File.OpenRead(jsonFile))
                using (var cfg = new StreamReader(file))
                {
                    var json = JObject.Parse(cfg.ReadToEnd());
                    folder = Environment.ExpandEnvironmentVariables(json.Value<string>("folderInsideDropbox"));
                    foreach (var association in json.Value<JArray>("associations"))
                        processForExtension[association.Value<string>("extension")] = association.Value<string>("process");
                }
            }
        }

        internal string Folder { get { return folder; } }

        internal Dictionary<string, string> ExtensionAssociations { get { return processForExtension; } }

        private static readonly ApplicationConfig singleton = new ApplicationConfig();

        internal static ApplicationConfig Instance { get { return singleton; } }
    }
}
