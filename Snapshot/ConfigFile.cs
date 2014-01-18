using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace Snapshot
{
    internal class ConfigFile
    {
        private readonly string folder;
        private readonly Dictionary<string, string> processForExtension = new Dictionary<string,string>();

        internal ConfigFile()
        {
            if (File.Exists("config.cfg"))
            {
                using (var file = File.OpenRead("config.cfg"))
                using (var cfg = new StreamReader(file))
                {
                    //TODO: better organization of config.cfg
                    folder = Environment.ExpandEnvironmentVariables(cfg.ReadLine());
                    string line;
                    while ((line = cfg.ReadLine()) != null)
                    {
                        var delimit = line.IndexOf(' ');
                        processForExtension[line.Substring(0, delimit)] = line.Substring(delimit + 1);
                    }
                }
            }
        }

        internal string Folder { get { return folder; } }

        internal Dictionary<string, string> ExtensionAssociations { get { return processForExtension; } }

        internal string Database { get { return "snapshot.db3"; } }
    }
}
