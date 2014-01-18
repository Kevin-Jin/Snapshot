﻿namespace Snapshot
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
            this.btnSettings = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lstRecentProjects = new System.Windows.Forms.ListBox();
            this.lblRecentProjects = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.BackColor = System.Drawing.Color.White;
            this.btnOpenProject.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenProject.Location = new System.Drawing.Point(71, 228);
            this.btnOpenProject.Name = "btnOpenProject";
            this.btnOpenProject.Size = new System.Drawing.Size(170, 51);
            this.btnOpenProject.TabIndex = 0;
            this.btnOpenProject.Text = "Open Project";
            this.btnOpenProject.UseVisualStyleBackColor = false;
            // 
            // btnSaveProject
            // 
            this.btnSaveProject.BackColor = System.Drawing.Color.White;
            this.btnSaveProject.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveProject.Location = new System.Drawing.Point(71, 128);
            this.btnSaveProject.Name = "btnSaveProject";
            this.btnSaveProject.Size = new System.Drawing.Size(170, 51);
            this.btnSaveProject.TabIndex = 1;
            this.btnSaveProject.Text = "Save Project";
            this.btnSaveProject.UseVisualStyleBackColor = false;
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.Color.White;
            this.btnSettings.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.Location = new System.Drawing.Point(59, 444);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(170, 47);
            this.btnSettings.TabIndex = 3;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Snapshot.Properties.Resources.snapshot3;
            this.pictureBox1.Location = new System.Drawing.Point(44, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(224, 92);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // lstRecentProjects
            // 
            this.lstRecentProjects.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(206)))), ((int)(((byte)(243)))));
            this.lstRecentProjects.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstRecentProjects.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstRecentProjects.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstRecentProjects.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lstRecentProjects.FormattingEnabled = true;
            this.lstRecentProjects.ItemHeight = 18;
            this.lstRecentProjects.Location = new System.Drawing.Point(2, 298);
            this.lstRecentProjects.Name = "lstRecentProjects";
            this.lstRecentProjects.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lstRecentProjects.SelectedIndexChanged += lstRecentProjects_SelectedIndexChanged;
            this.lstRecentProjects.Size = new System.Drawing.Size(293, 108);
            this.lstRecentProjects.TabIndex = 0;
            this.lstRecentProjects.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstRecentProjects_DrawItem);
            // 
            // lblRecentProjects
            // 
            this.lblRecentProjects.AutoSize = true;
            this.lblRecentProjects.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecentProjects.Location = new System.Drawing.Point(95, 280);
            this.lblRecentProjects.Name = "lblRecentProjects";
            this.lblRecentProjects.Size = new System.Drawing.Size(121, 18);
            this.lblRecentProjects.TabIndex = 5;
            this.lblRecentProjects.Text = "Recent projects:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.Location = new System.Drawing.Point(82, 185);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(149, 22);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Close when Saved";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(206)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(296, 538);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.lblRecentProjects);
            this.Controls.Add(this.lstRecentProjects);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnSaveProject);
            this.Controls.Add(this.btnOpenProject);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Main";
            this.Text = "Snapshot";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenProject;
        private System.Windows.Forms.Button btnSaveProject;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListBox lstRecentProjects;
        private System.Windows.Forms.Label lblRecentProjects;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

