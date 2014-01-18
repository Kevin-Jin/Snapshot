using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapshot
{
    internal static class ConfigDatabase
    {
        internal static void OpenDatabase()
        {
            var cfgFile = new ConfigFile();
            if (!File.Exists(cfgFile.Folder + cfgFile.Database))
                SQLiteConnection.CreateFile(cfgFile.Folder + cfgFile.Database);
            using (var conn = new SQLiteConnection(@"Data Source=" + cfgFile.Folder + cfgFile.Database))
            {
                conn.Open();
                /*bool tableExists = false;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='foo'";
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            tableExists = reader.GetBoolean(0);
                }
                if (!tableExists)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "CREATE TABLE foo (id Integer)";
                        cmd.ExecuteNonQuery();
                    }
                }
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT id FROM foo";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("id"));
                            Console.WriteLine(id);
                        }
                    }
                }*/
            }
        }
    }
}
