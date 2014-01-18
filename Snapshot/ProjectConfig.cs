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
    internal class ProcessConfig
    {
        public string processAbsolutePath;
        public string openedFilesPaths;
    }

    internal class ProjectConfig
    {
        private readonly Dictionary<string, string> processes;

        internal ProjectConfig(string jsonFile)
        {
            if (File.Exists(jsonFile))
            {
                processes = new Dictionary<string, string>();
                using (var file = File.OpenRead(jsonFile))
                using (var cfg = new StreamReader(file))
                {
                    var json = JObject.Parse(cfg.ReadToEnd());
                    json.Value<List<ProcessConfig>>("processes").ForEach(process => processes[process.processAbsolutePath] = process.openedFilesPaths);
                }
            }
        }

        internal ProjectConfig(Dictionary<string, string> processes)
        {
            this.processes = processes;
        }

        public JObject ToJson()
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
    }
}
