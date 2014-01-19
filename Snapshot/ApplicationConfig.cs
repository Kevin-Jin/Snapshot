using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Snapshot
{
    internal class ApplicationConfig
    {
        private readonly string folder;
        private readonly Dictionary<string, Tuple<List<string>, List<Regex>>> processFilesInclusion = new Dictionary<string, Tuple<List<string>, List<Regex>>>();

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
                    {
                        var key = association.Value<string>("process").ToLower();
                        var extensions = association.Value<JArray>("extensions").Select(result => ((string)result).ToLower()).ToList();
                        var exclude = association.Value<JArray>("exclude").Select(result => new Regex((string)result, RegexOptions.Compiled | RegexOptions.Multiline)).ToList();
                        if (processFilesInclusion.ContainsKey(association.Value<string>("process")))
                        {
                            processFilesInclusion[key].Item1.AddRange(extensions);
                            processFilesInclusion[key].Item2.AddRange(exclude);
                        }
                        else
                        {
                            processFilesInclusion[key] = new Tuple<List<string>, List<Regex>>(extensions, exclude);
                        }
                    }
                }
            }
        }

        internal string Folder { get { return folder; } }

        internal List<string> GetExtensionAssociations(string processName)
        {
            return processFilesInclusion.ContainsKey(processName) ? processFilesInclusion[processName].Item1 : null;
        }

        internal List<Regex> GetExclusions(string processName)
        {
            return processFilesInclusion.ContainsKey(processName) ? processFilesInclusion[processName].Item2 : null;
        }

        internal string GetRecent()
        {

        }

        internal void SetRecent(string recent)
        {
            using (var file = File.OpenRead(jsonFile))
            using (var cfg = new StreamReader(file))
            {
                var json = JObject.Parse(cfg.ReadToEnd());
                folder = Environment.ExpandEnvironmentVariables(json.Value<string>("folderInsideDropbox"));
                foreach (var association in json.Value<JArray>("associations"))
                    if (extensionsForProcess.ContainsKey(association.Value<string>("process")))
                        extensionsForProcess[association.Value<string>("process").ToLower()].AddRange(association.Value<JArray>("extensions").Select(result => ((string)result).ToLower()).ToList());
                    else
                        extensionsForProcess[association.Value<string>("process").ToLower()] = association.Value<JArray>("extensions").Select(result => ((string)result).ToLower()).ToList();
            }
        }

        public override string ToString()
        {
            return string.Join(", ", processFilesInclusion.Select(entry => ('(' + entry.Key + " => " + string.Join(", ", entry.Value) + ')')));
        }

        private static readonly ApplicationConfig singleton = new ApplicationConfig();

        internal static ApplicationConfig Instance { get { return singleton; } }
    }
}
