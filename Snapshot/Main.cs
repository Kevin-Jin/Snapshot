using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        Timer timer = new Timer();
        Form2 f = new Form2();
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            lstRecentProjects.Items.Add("Biology Research");
            lstRecentProjects.Items.Add("Aerospace Homework");
            lstRecentProjects.Items.Add("Snapshot");
            lstRecentProjects.Items.Add("Reddit Time");
            lstRecentProjects.Items.Add("mHacks Detroit");

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

        private void lstRecentProjects_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (lstRecentProjects.SelectedIndex != -1)
                MessageBox.Show("not implemented yet");
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            new Form2().Show();
        }

        private void lstRecentProjects_DrawItem(object sender, DrawItemEventArgs e)
        {
            string output = lstRecentProjects.Items[e.Index].ToString();
            float olength = e.Graphics.MeasureString(output, e.Font).Width;
            float pos = (lstRecentProjects.Width - olength) / 2;
            SolidBrush brush = new SolidBrush(e.ForeColor);
            e.Graphics.DrawString(output, e.Font, brush, pos, e.Bounds.Top);
        }

        private void btnSaveProject_Click(object sender, EventArgs e)
        {
            bool saveToDropbox = false;
            new DirectoryInfo(Environment.ExpandEnvironmentVariables(ApplicationConfig.Instance.Folder)).Create();
            DialogResult diagResult;
            FileInfo configFile = null;
            DirectoryInfo dataDirectory = null;
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.InitialDirectory = Environment.ExpandEnvironmentVariables(ApplicationConfig.Instance.Folder);
                saveDialog.Title = "Where do you want to store the JSON project config and its data folder?";
                saveDialog.FileName = "Snapshot project " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                saveDialog.Filter = "JSON project config (*.json)|*.json";
                saveDialog.OverwritePrompt = false;
                saveDialog.FileOk += (s, ea) =>
                {
                    configFile = new FileInfo(saveDialog.FileName);
                    var configFileExists = configFile.Exists;
                    dataDirectory = new DirectoryInfo(saveDialog.FileName.Substring(0, saveDialog.FileName.LastIndexOf(".")) + "_data");
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
                };
                switch (saveDialog.ShowDialog())
                {
                    case DialogResult.Yes:
                    case DialogResult.OK:
                        TaskEx.Run(async () =>
                        {
                            if (saveToDropbox)
                            {
                                if (dataDirectory.Exists)
                                    dataDirectory.Delete(true);
                                dataDirectory.Create();
                            }

                            var sameNameFiles = new Dictionary<string, int>();
                            var tasks = new List<Task>();
                            var operations = Operations.GetOpenedProcesses().Select(process => new Tuple<string, Task<List<Tuple<string, string>>>>(process, Operations.GetFilesOpenedByProcess(process)));
                            var map = new Dictionary<string, List<String>>();

                            foreach (var op in operations)
                            {
                                tasks.Add(op.Item2);
                                var key = op.Item1;
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
                                if (map.ContainsKey(key))
                                    map[key].AddRange(files);
                                else
                                    map[key] = files;
                            }
                            var cfg = new ProjectConfig(map);
                            await TaskEx.WhenAll(tasks);
                            using (var file = File.Open(configFile.FullName, FileMode.Create))
                            using (var writer = new StreamWriter(file))
                                writer.Write(cfg.ToJson().ToString());
                        }).Wait();
                        //TODO: buttonless modal over save file dialog instead of blocking UI thread
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
            DialogResult diagResult;
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.InitialDirectory = Environment.ExpandEnvironmentVariables(ApplicationConfig.Instance.Folder);
                openDialog.Title = "Where is your JSON project config and its data folder?";
                openDialog.Filter = "JSON project config (*.json)|*.json";
                openDialog.FileOk += (s, ea) =>
                {
                    if (loadFromDropbox)
                    {
                        var dataDirectory = new DirectoryInfo(openDialog.FileName.Substring(0, openDialog.FileName.LastIndexOf(".")) + "_data");
                        bool dataDirectoryExists = dataDirectory.Exists;
                        if (!dataDirectoryExists)
                        {
                            var msg = dataDirectory.Name + "\nDirectory not found.\n" + dataDirectory.Name.Substring(0, dataDirectory.Name.LastIndexOf("_data")) + " is not a valid Snapshot project.";
                            MessageBox.Show(msg, openDialog.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ea.Cancel = true;
                        }
                    }
                };
                diagResult = openDialog.ShowDialog();
                switch (diagResult)
                {
                    case DialogResult.Cancel:
                        break;
                    case DialogResult.OK:
                        {
                            foreach (var entry in new ProjectConfig(openDialog.FileName).Processes)
                            {
                                switch (entry.Key.Substring(entry.Key.LastIndexOf(Path.DirectorySeparatorChar) + 1).ToLower())
                                {
                                    case "winword.exe":
                                        Operations.GetOutput(entry.Key, string.Join(" ", entry.Value));
                                        break;
                                }
                            }
                            StartSplash();
                        }
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
}
