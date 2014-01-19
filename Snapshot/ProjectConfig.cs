using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapshot
{
    internal class ProjectConfig
    {
        private readonly Dictionary<string, List<string>> processes;
        private readonly List<string> ieUrls;

        internal ProjectConfig(string jsonFile)
        {
            processes = new Dictionary<string, List<string>>();
            if (File.Exists(jsonFile))
            {
                using (var file = File.OpenRead(jsonFile))
                using (var cfg = new StreamReader(file))
                {
                    var json = JObject.Parse(cfg.ReadToEnd());
                    json.Value<JArray>("processes").Select(token => new Tuple<string, List<string>>(token.Value<string>("processAbsolutePath"), token.Value<JArray>("openedFilesPaths").Select(result => (string)result).ToList())).ToList().ForEach(process =>
                    {
                        if (processes.ContainsKey(process.Item1))
                            processes[process.Item1].AddRange(process.Item2);
                        else
                            processes[process.Item1] = process.Item2;
                    });
                    ieUrls = json.Value<JArray>("ieTabUrls").Select(token => (string)token).ToList();
                }
            }
        }

        internal ProjectConfig(Dictionary<string, List<string>> processes, List<string> ieUrls)
        {
            this.processes = processes;
            this.ieUrls = ieUrls;
        }

        internal JObject ToJson()
        {
            return JObject.FromObject(new
            {
                processes = from process in processes
                            select new
                            {
                                processAbsolutePath = process.Key,
                                openedFilesPaths = process.Value
                            },
                ieTabUrls = from ieUrl in ieUrls select ieUrl
            });
        }

        internal Dictionary<string, List<string>> Processes { get { return processes; } }

        internal List<string> IeTabUrls { get { return ieUrls; } }

        public override string ToString()
        {
            return string.Join(", ", processes.Select(entry => ('(' + entry.Key + " => " + string.Join(", ", entry.Value) + ')')));
        }
    }
}
