using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snapshot
{
    public partial class Main : Form
    {
        private readonly Timer timer = new Timer();
        private readonly Splash f = new Splash();

        public Main()
        {
            InitializeComponent();
        }

        private void ReloadRecentProjects()
        {
            lstRecentProjects.Items.Clear();
            lstRecentProjects.Items.AddRange(ApplicationConfig.Instance.RecentProjects.Select(str => str.Substring(str.LastIndexOf(Path.DirectorySeparatorChar) + 1, Math.Max(0, str.LastIndexOf(".")) - str.LastIndexOf(Path.DirectorySeparatorChar) - 1)).ToArray());
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ReloadRecentProjects();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 10;
            timer.Enabled = true;
        }

        private void StartSplash()
        {
            //Screenshot animation
            f.AllowTransparency = true;
            f.Opacity = 1;
            f.Left = (Screen.PrimaryScreen.Bounds.Width);
            f.Top = (Screen.PrimaryScreen.Bounds.Height);
            f.FormBorderStyle = FormBorderStyle.None;
            f.Show();
            timer.Start();
        }

        private void LoadProject(string cfgFile)
        {
            var project = new ProjectConfig(cfgFile);
            foreach (var entry in project.Processes)
            {
                switch (entry.Key.Substring(entry.Key.LastIndexOf(Path.DirectorySeparatorChar) + 1).ToLower())
                {
                    default:
                        Operations.GetOutput(entry.Key, "\"" + string.Join("\" \"", entry.Value) + "\"");
                        break;
                }
            }
            Operations.OpenInternetExplorerTabs(project.IeTabUrls);
            StartSplash();
        }

        private void lstRecentProjects_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (lstRecentProjects.SelectedIndex != -1)
                LoadProject(ApplicationConfig.Instance.RecentProjects[lstRecentProjects.SelectedIndex]);
        }

        private void lstRecentProjects_DrawItem(object sender, DrawItemEventArgs e)
        {
            string output = lstRecentProjects.Items[e.Index].ToString();
            float olength = e.Graphics.MeasureString(output, e.Font).Width;
            float pos = (lstRecentProjects.Width - olength) / 2;
            SolidBrush brush = new SolidBrush(e.ForeColor);
            e.Graphics.DrawString(output, e.Font, brush, pos, e.Bounds.Top);
        }

        private async Task SaveProject(bool saveToDropbox, bool closeAfter, FileInfo configFile, DirectoryInfo dataDirectory)
        {
            if (saveToDropbox)
            {
                if (dataDirectory.Exists)
                    dataDirectory.Delete(true);
                dataDirectory.Create();
            }

            var sameNameFiles = new Dictionary<string, int>();
            var tasks = new List<Task>();
            var processes = Operations.GetOpenedProcesses();
            var operations = processes.Select(process => new Tuple<string, Task<List<Tuple<string, string>>>, Action>(process.Item1, Operations.GetFilesOpenedByProcess(process.Item1.Substring(process.Item1.LastIndexOf(Path.DirectorySeparatorChar) + 1)), process.Item2));
            var processOpenedFiles = new Dictionary<string, List<String>>();
            var close = new Dictionary<string, List<Action>>();

            foreach (var op in operations)
            {
                tasks.Add(op.Item2);
                var key = op.Item1.ToLower();
                var files = (await op.Item2).Select(result => result.Item2).ToList();
                for (var i = 0; i < files.Count; i++)
                {
                    var fileName = files[i].Substring(files[i].LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    string originalLocation = files[i];
                    if (saveToDropbox)
                    {
                        if (!sameNameFiles.ContainsKey(fileName))
                        {
                            sameNameFiles[fileName] = 1;
                            files[i] = dataDirectory.ToString() + Path.DirectorySeparatorChar + fileName;
                        }
                        else
                        {
                            sameNameFiles[fileName]++;
                            files[i] = dataDirectory.ToString() + Path.DirectorySeparatorChar + fileName + sameNameFiles[fileName];
                        }

                        using (Stream source = File.OpenRead(originalLocation))
                        using (Stream destination = File.Create(files[i]))
                            tasks.Add(source.CopyToAsync(destination));
                    }
                }
                if (processOpenedFiles.ContainsKey(key))
                    processOpenedFiles[key].AddRange(files);
                else
                    processOpenedFiles[key] = files;
                if (close.ContainsKey(key))
                    close[key].Add(op.Item3);
                else
                    close[key] = new List<Action>() { op.Item3 };
            }
            processOpenedFiles = processOpenedFiles.Where(entry => !ApplicationConfig.Instance.ExcludedProcesses.Contains(entry.Key.Substring(entry.Key.LastIndexOf(Path.DirectorySeparatorChar) + 1))).ToDictionary(p => p.Key, p => p.Value);
            close = close.Where(entry => !ApplicationConfig.Instance.ExcludedProcesses.Contains(entry.Key.Substring(entry.Key.LastIndexOf(Path.DirectorySeparatorChar) + 1))).ToDictionary(p => p.Key, p => p.Value);
            var cfg = new ProjectConfig(processOpenedFiles, Operations.GetInternetExplorerUrls());
            await TaskEx.WhenAll(tasks);
            using (var file = File.Open(configFile.FullName, FileMode.Create))
            using (var writer = new StreamWriter(file))
                writer.Write(cfg.ToJson().ToString());

            if (closeAfter)
            {
                foreach (var entry in close)
                    foreach (var closeAction in entry.Value)
                        closeAction();
                foreach (var proc in Process.GetProcesses().Where(result => result.ProcessName.ToLower() == "iexplore"))
                    proc.CloseWindow();
            }
        }

        private void btnSaveProject_Click(object sender, EventArgs e)
        {
            bool saveToDropbox = false;
            new DirectoryInfo(Environment.ExpandEnvironmentVariables(ApplicationConfig.Instance.Folder)).Create();
            DialogResult? diagResult = null;
            FileInfo configFile = null;
            DirectoryInfo dataDirectory = null;
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.InitialDirectory = Environment.ExpandEnvironmentVariables(ApplicationConfig.Instance.Folder);
                saveDialog.Title = "Where do you want to store the JSON project config and its data folder?";
                saveDialog.FileName = "Snapshot project";
                saveDialog.Filter = "JSON project config (*.json)|*.json";
                saveDialog.OverwritePrompt = false;
                saveDialog.FileOk += (s, ea) =>
                {
                    configFile = new FileInfo(saveDialog.FileName);
                    var configFileExists = configFile.Exists;
                    dataDirectory = new DirectoryInfo(saveDialog.FileName.Substring(0, Math.Max(0, saveDialog.FileName.LastIndexOf("."))) + "_data");
                    var dataDirectoryExists = dataDirectory.Exists;
                    string msg = null;
                    if (configFileExists)
                    {
                        msg = configFile.Name + " already exists.\nDo you want to replace it?";
                        if (dataDirectoryExists && saveToDropbox)
                            msg += "\nThe existing " + dataDirectory.Name + " folder will be deleted as well.";
                    }
                    else if (dataDirectoryExists && saveToDropbox)
                    {
                        msg = dataDirectory.Name + " already exists.\nDo you want to delete the existing folder?";
                    }
                    if (msg != null)
                    {
                        diagResult = MessageBox.Show(msg, "Confirm Save As", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                        if (diagResult == DialogResult.Cancel)
                            ea.Cancel = true;
                    }

                    ApplicationConfig.Instance.PushRecentProject(configFile.FullName);
                    ReloadRecentProjects();
                };
                var saveDiagResult = saveDialog.ShowDialog();
                diagResult = diagResult ?? saveDiagResult;
                switch (diagResult)
                {
                    case DialogResult.Yes:
                    case DialogResult.OK:
                        //TODO: buttonless modal over save file dialog instead of blocking UI thread
                        TaskEx.Run(() => SaveProject(saveToDropbox, checkBox1.Checked, configFile, dataDirectory)).Wait();
                        StartSplash();
                        break;
                    case DialogResult.No:
                    case DialogResult.Cancel:
                        break;
                }
            }
        }

        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            bool loadFromDropbox = false;
            DialogResult? diagResult = null;
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.InitialDirectory = Environment.ExpandEnvironmentVariables(ApplicationConfig.Instance.Folder);
                openDialog.Title = "Where is your JSON project config and its data folder?";
                openDialog.Filter = "JSON project config (*.json)|*.json";
                openDialog.FileOk += (s, ea) =>
                {
                    if (loadFromDropbox)
                    {
                        var dataDirectory = new DirectoryInfo(openDialog.FileName.Substring(0, Math.Max(0, openDialog.FileName.LastIndexOf("."))) + "_data");
                        bool dataDirectoryExists = dataDirectory.Exists;
                        if (!dataDirectoryExists)
                        {
                            var msg = dataDirectory.Name + "\nDirectory not found.\n" + dataDirectory.Name.Substring(0, dataDirectory.Name.LastIndexOf("_data")) + " is not a valid Snapshot project.";
                            MessageBox.Show(msg, openDialog.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ea.Cancel = true;
                        }
                    }
                };
                var openDiagResult = openDialog.ShowDialog();
                diagResult = diagResult ?? openDiagResult;
                switch (diagResult)
                {
                    case DialogResult.Cancel:
                        break;
                    case DialogResult.OK:
                        LoadProject(openDialog.FileName);
                        break;
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            f.Opacity -= 0.02;
            if (f.Opacity <= 0)
            {
                f.Hide();
                timer.Stop();
            }
        }
    }

    internal static class SnapshotExtensions
    {
        internal static void RemoveRange<T1, T2>(this Dictionary<T1, T2> dict, IEnumerable<T1> keysToRemove)
        {
            foreach (var key in keysToRemove)
                dict.Remove(key);
        }
    }
}
