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
       
        }

        private void lstRecentProjects_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            MessageBox.Show("not implemented yet");

        }

        Form2 form2 = new Form2();
        private void btnSettings_Click(object sender, EventArgs e)
        {
            
            form2.Show();
        }

        private void lstRecentProjects_DrawItem(object sender, DrawItemEventArgs e)
        {

            string output = lstRecentProjects.Items[e.Index].ToString();
            float olength = e.Graphics.MeasureString(output, e.Font).Width;
            float pos = (lstRecentProjects.Width - olength)/2;
            SolidBrush brush = new SolidBrush(e.ForeColor);
            e.Graphics.DrawString(output, e.Font, brush, pos, e.Bounds.Top);

        }

        
        
    }
}
