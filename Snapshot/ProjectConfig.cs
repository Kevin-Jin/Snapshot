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

        internal ProjectConfig(string jsonFile)
        {
            processes = new Dictionary<string, List<string>>();
            if (File.Exists(jsonFile))
                using (var file = File.OpenRead(jsonFile))
                using (var cfg = new StreamReader(file))
                    JObject.Parse(cfg.ReadToEnd()).Value<JArray>("processes").Select(token => new Tuple<string, string>(token.Value<string>("processAbsolutePath"), token.Value<string>("openedFilesPaths"))).ToList().ForEach(process =>
                    {
                        if (processes.ContainsKey(process.Item1))
                            processes[process.Item1].Add(process.Item2);
                        else
                            processes[process.Item1] = new List<string>() { process.Item2 };
                    });
        }

        internal ProjectConfig(Dictionary<string, List<string>> processes)
        {
            this.processes = processes;
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
                            }
            });
        }

        public override string ToString()
        {
            return string.Join(", ", processes.Select(entry => ('(' + entry.Key + " => " + string.Join(", ", entry.Value) + ')')));
        }
    }
}
