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
        private readonly string jsonFile;
        private readonly string folder;
        private readonly Dictionary<string, Tuple<List<string>, List<Regex>>> processFilesInclusion = new Dictionary<string, Tuple<List<string>, List<Regex>>>();
        private readonly List<string> processesToIgnore;
        private readonly string[] recentProjects;

        private ApplicationConfig(string jsonFile = "config.json")
        {
            if (File.Exists(jsonFile))
            {
                this.jsonFile = jsonFile;
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
                    processesToIgnore = json.Value<JArray>("exclude").Select(result => ((string)result).ToLower()).ToList();
                    recentProjects = json.Value<JArray>("recent").Select(result => ((string)result).ToLower()).Take(5).ToArray();
                    if (recentProjects.Length != 5)
                    {
                        var padded = new string[5];
                        Array.Copy(recentProjects, padded, recentProjects.Length);
                        for (var i = recentProjects.Length; i < 5; i++)
                            padded[i] = "";
                        recentProjects = padded;
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

        internal string[] RecentProjects { get { return recentProjects; } }

        private void Commit()
        {
            using (var file = File.Open(jsonFile ?? "config.json", FileMode.Create))
            using (var cfg = new StreamWriter(file))
            {
                cfg.Write(JObject.FromObject(new
                {
                    folderInsideDropbox = folder,
                    associations = from process in processFilesInclusion
                                   select new
                                   {
                                       process = process.Key,
                                       extensions = from extension in process.Value.Item1
                                                    select extension,
                                       exclude = from exclude in process.Value.Item2
                                                 select exclude.ToString()
                                   },
                    exclude = from ignore in processesToIgnore
                              select ignore,
                    recent = from project in recentProjects
                             select project
                }));
            }
        }

        internal void PushRecentProject(string cfgPath)
        {
            for (var i = 3; i >= 0; --i)
                recentProjects[i + 1] = recentProjects[i];
            recentProjects[0] = cfgPath;

            Commit();
        }

        internal List<string> ExcludedProcesses { get { return processesToIgnore; } }

        public override string ToString()
        {
            return string.Join(", ", processFilesInclusion.Select(entry => ('(' + entry.Key + " => " + string.Join(", ", entry.Value) + ')')));
        }

        private static readonly ApplicationConfig singleton = new ApplicationConfig();

        internal static ApplicationConfig Instance { get { return singleton; } }
    }
}
