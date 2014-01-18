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
        private static SQLiteCommand CreateCommand(this SQLiteConnection conn, string commandText)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = commandText;
            return cmd;
        }

        private static bool ValidateTables(SQLiteConnection conn, string table)
        {
            using (var cmd = conn.CreateCommand("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@table"))
            {
                cmd.Parameters.AddWithValue("table", table);
                using (var reader = cmd.ExecuteReader())
                    if (reader.Read())
                        return reader.GetBoolean(0);
            }
            return false;
        }

        private static SQLiteConnection InitializeDataConnection()
        {
            var cfgFile = new ConfigFile();
            var fileInfo = new FileInfo(cfgFile.Folder + cfgFile.Database);
            if (!fileInfo.Exists)
            {
                fileInfo.Directory.Create();
                SQLiteConnection.CreateFile(cfgFile.Folder + cfgFile.Database);
            }
            var conn = new SQLiteConnection(@"Data Source=" + cfgFile.Folder + cfgFile.Database);
            conn.Open();
            return conn;
        }

        internal static List<Tuple<int, string>> GetProjects()
        {
            List<Tuple<int, string>> projects = new List<Tuple<int, string>>();
            using (var conn = InitializeDataConnection())
            {
                if (!ValidateTables(conn, "projects"))
                    using (var cmd = conn.CreateCommand("CREATE TABLE projects (id INTEGER, name TEXT, PRIMARY KEY(id ASC))"))
                        cmd.ExecuteNonQuery();
                using (var cmd = conn.CreateCommand("SELECT id, name FROM projects"))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            projects.Add(new Tuple<int, string>(reader.GetInt32(0), reader.GetString(1)));
            }
            return projects;
        }

        internal static List<string> GetProcesses(string project)
        {
            List<string> processes = new List<string>();
            using (var conn = InitializeDataConnection())
            {
                if (!ValidateTables(conn, "processes"))
                    using (var cmd = conn.CreateCommand("CREATE TABLE processes (id INTEGER, projectid INTEGER, absolutepath TEXT, PRIMARY KEY(id ASC), FOREIGN KEY(projectid) REFERENCES projects(id))"))
                        cmd.ExecuteNonQuery();
                using (var cmd = conn.CreateCommand("SELECT absolutepath FROM processes WHERE project=@project"))
                {
                    cmd.Parameters.AddWithValue("project", project);
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            processes.Add(reader.GetString(0));
                }
            }
            return processes;
        }
    }
}
