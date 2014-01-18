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
            float pos = (lstRecentProjects.Width - olength)/2;
            SolidBrush brush = new SolidBrush(e.ForeColor);
            e.Graphics.DrawString(output, e.Font, brush, pos, e.Bounds.Top);
        }

        private void btnSaveProject_Click(object sender, EventArgs e)
        {
            new DirectoryInfo(Environment.ExpandEnvironmentVariables(ApplicationConfig.Instance.Folder)).Create();
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.InitialDirectory = Environment.ExpandEnvironmentVariables(ApplicationConfig.Instance.Folder);
                saveDialog.Title = "Where do you want to store the JSON project config and its data folder?";
                saveDialog.FileName = "Snapshot project " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                saveDialog.Filter = "JSON project config (*.json)|*.json";
                saveDialog.OverwritePrompt = false;
                saveDialog.FileOk += (s, ea) =>
                {
                    var configFile = new FileInfo(saveDialog.FileName);
                    bool configFileExists = configFile.Exists;
                    var dataDirectory = new DirectoryInfo(saveDialog.FileName.Substring(0, saveDialog.FileName.LastIndexOf(".")) + "_data");
                    bool dataDirectoryExists = dataDirectory.Exists;
                    string msg = null;
                    if (configFileExists)
                    {
                        msg = configFile.Name + " already exists.\nDo you want to replace it?";
                        if (dataDirectoryExists)
                            msg += "\nThe existing " + dataDirectory.Name + " folder will be deleted as well.";
                    }
                    else if (dataDirectoryExists)
                    {
                        msg = dataDirectory.Name + " already exists.\nDo you want to delete the existing folder?";
                    }
                    if (msg != null)
                    {
                        switch (MessageBox.Show(msg, "Confirm Save As", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                        {
                            case DialogResult.Yes:
                                TaskEx.Run(async () =>
                                {
                                    var tasks = new List<Task<List<Tuple<string, string>>>>();
                                    var lol = Operations.GetOpenedProcesses().Select(process => new Tuple<string, Task<List<Tuple<string, string>>>>(process, Operations.GetFilesOpenedByProcess(process)));
                                    var map = new Dictionary<string, List<String>>();
                                    foreach (var lel in lol)
                                    {
                                        tasks.Add(lel.Item2);
                                        var key = lel.Item1;
                                        if (map.ContainsKey(key))
                                            map[key].AddRange((await lel.Item2).Select(result => result.Item2).ToList());
                                        else
                                            map[key] = (await lel.Item2).Select(result => result.Item2).ToList();
                                    }
                                    var cfg = new ProjectConfig(map);
                                    await TaskEx.WhenAll(tasks);
                                    using (var file = File.Open(configFile.FullName, FileMode.Create))
                                    using (var writer = new StreamWriter(file))
                                        writer.Write(cfg.ToJson().ToString());
                                }).Wait();
                                //TODO: buttonless modal over save file dialog instead of blocking UI thread
                                break;
                            case DialogResult.No:
                                //don't save file
                                break;
                            case DialogResult.Cancel:
                                ea.Cancel = true;
                                break;
                        }
                    }
                };
                saveDialog.ShowDialog();
            }
        }

        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.InitialDirectory = Environment.ExpandEnvironmentVariables(ApplicationConfig.Instance.Folder);
                openDialog.Title = "Where is your JSON project config and its data folder?";
                openDialog.Filter = "JSON project config (*.json)|*.json";
                openDialog.FileOk += (s, ea) =>
                {
                    var dataDirectory = new DirectoryInfo(openDialog.FileName.Substring(0, openDialog.FileName.LastIndexOf(".")) + "_data");
                    bool dataDirectoryExists = dataDirectory.Exists;
                    if (!dataDirectoryExists)
                    {
                        var msg = dataDirectory.Name + "\nDirectory not found.\n" + dataDirectory.Name.Substring(0, dataDirectory.Name.LastIndexOf("_data")) + " is not a valid Snapshot project.";
                        MessageBox.Show(msg, openDialog.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ea.Cancel = true;
                    }
                };
                openDialog.ShowDialog();
                Console.WriteLine(new ProjectConfig(openDialog.FileName));
            }
        }
    }
}
