using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            lstRecentProjects.Items.Add("not implemented yet");
            lstRecentProjects.Items.Add("not implemented yet2");
        }

        private void lstRecentProjects_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (lstRecentProjects.SelectedIndex != -1)
                MessageBox.Show(lstRecentProjects.SelectedIndex.ToString());
        }
    }
}
