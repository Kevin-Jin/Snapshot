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
            timer.Interval = (10);
            timer.Enabled = true;     
            
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
            //Screenshot animation
            f.AllowTransparency = true;
            f.Opacity = 1;
            f.Left = (Screen.PrimaryScreen.Bounds.Width);
            f.Top = (Screen.PrimaryScreen.Bounds.Height);    
            f.FormBorderStyle = FormBorderStyle.None;
            f.Show();
                                        
            timer.Start(); 
   
         }

        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            //Screenshot animation
            f.AllowTransparency = true;
            f.Opacity = 1;
            f.Left = (Screen.PrimaryScreen.Bounds.Width);
            f.Top = (Screen.PrimaryScreen.Bounds.Height);

            f.FormBorderStyle = FormBorderStyle.None;
            f.Left = (Screen.PrimaryScreen.Bounds.Width);
            f.Top = (Screen.PrimaryScreen.Bounds.Height);
            f.Show();

            timer.Start();

        }

        void timer_Tick(object sender, EventArgs e)
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
