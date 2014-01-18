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

        internal ConfigFile()
        {
            if (File.Exists("config.cfg"))
            {
                using (var file = File.OpenRead("config.cfg"))
                using (var cfg = new StreamReader(file))
                {
                    folder = Environment.ExpandEnvironmentVariables(cfg.ReadLine());
                }
            }
        }

        internal string Folder { get { return folder; } }

        internal string Database { get { return "snapshot.db3"; } }
    }
}
