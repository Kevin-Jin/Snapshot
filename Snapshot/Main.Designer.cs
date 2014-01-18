namespace Snapshot
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOpenProject = new System.Windows.Forms.Button();
            this.btnSaveProject = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lstRecentProjects = new System.Windows.Forms.ListBox();
            this.lblRecentProjects = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.BackColor = System.Drawing.Color.White;
            this.btnOpenProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenProject.Location = new System.Drawing.Point(21, 192);
            this.btnOpenProject.Name = "btnOpenProject";
            this.btnOpenProject.Size = new System.Drawing.Size(157, 51);
            this.btnOpenProject.TabIndex = 0;
            this.btnOpenProject.Text = "Open New Project";
            this.btnOpenProject.UseVisualStyleBackColor = false;
            // 
            // btnSaveProject
            // 
            this.btnSaveProject.BackColor = System.Drawing.Color.White;
            this.btnSaveProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveProject.Location = new System.Drawing.Point(21, 128);
            this.btnSaveProject.Name = "btnSaveProject";
            this.btnSaveProject.Size = new System.Drawing.Size(157, 51);
            this.btnSaveProject.TabIndex = 1;
            this.btnSaveProject.Text = "Create New Project";
            this.btnSaveProject.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.White;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(21, 438);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(157, 51);
            this.button3.TabIndex = 3;
            this.button3.Text = "Settings";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Snapshot.Properties.Resources.snapshot3;
            this.pictureBox1.Location = new System.Drawing.Point(21, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(224, 92);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // lstRecentProjects
            // 
            this.lstRecentProjects.FormattingEnabled = true;
            this.lstRecentProjects.Location = new System.Drawing.Point(21, 274);
            this.lstRecentProjects.Name = "lstRecentProjects";
            this.lstRecentProjects.Size = new System.Drawing.Size(157, 134);
            this.lstRecentProjects.TabIndex = 0;
            this.lstRecentProjects.SelectedIndexChanged += lstRecentProjects_SelectedIndexChanged;
            // 
            // lblRecentProjects
            // 
            this.lblRecentProjects.AutoSize = true;
            this.lblRecentProjects.Location = new System.Drawing.Point(21, 255);
            this.lblRecentProjects.Name = "lblRecentProjects";
            this.lblRecentProjects.Size = new System.Drawing.Size(85, 13);
            this.lblRecentProjects.TabIndex = 5;
            this.lblRecentProjects.Text = "Recent projects:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::Snapshot.Properties.Resources.bg;
            this.ClientSize = new System.Drawing.Size(296, 538);
            this.Controls.Add(this.lblRecentProjects);
            this.Controls.Add(this.lstRecentProjects);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnSaveProject);
            this.Controls.Add(this.btnOpenProject);
            this.Name = "Main";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenProject;
        private System.Windows.Forms.Button btnSaveProject;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListBox lstRecentProjects;
        private System.Windows.Forms.Label lblRecentProjects;
    }
}

