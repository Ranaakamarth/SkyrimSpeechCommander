namespace SpeechCommander.UI
{
    partial class ListControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.List_gb = new System.Windows.Forms.GroupBox();
            this.InputField_tb = new System.Windows.Forms.TextBox();
            this.Rename_bttn = new System.Windows.Forms.Button();
            this.Add_bttn = new System.Windows.Forms.Button();
            this.Remove_bttn = new System.Windows.Forms.Button();
            this.ItemList_lb = new System.Windows.Forms.ListBox();
            this.List_gb.SuspendLayout();
            this.SuspendLayout();
            // 
            // List_gb
            // 
            this.List_gb.AutoSize = true;
            this.List_gb.Controls.Add(this.InputField_tb);
            this.List_gb.Controls.Add(this.Rename_bttn);
            this.List_gb.Controls.Add(this.Add_bttn);
            this.List_gb.Controls.Add(this.Remove_bttn);
            this.List_gb.Controls.Add(this.ItemList_lb);
            this.List_gb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.List_gb.Location = new System.Drawing.Point(0, 0);
            this.List_gb.Name = "List_gb";
            this.List_gb.Size = new System.Drawing.Size(235, 300);
            this.List_gb.TabIndex = 13;
            this.List_gb.TabStop = false;
            this.List_gb.Text = "Actions:";
            // 
            // InputField_tb
            // 
            this.InputField_tb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputField_tb.Location = new System.Drawing.Point(6, 245);
            this.InputField_tb.Name = "InputField_tb";
            this.InputField_tb.Size = new System.Drawing.Size(223, 20);
            this.InputField_tb.TabIndex = 0;
            // 
            // Rename_bttn
            // 
            this.Rename_bttn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Rename_bttn.Location = new System.Drawing.Point(159, 271);
            this.Rename_bttn.Name = "Rename_bttn";
            this.Rename_bttn.Size = new System.Drawing.Size(70, 23);
            this.Rename_bttn.TabIndex = 1;
            this.Rename_bttn.Text = "Rename";
            this.Rename_bttn.UseVisualStyleBackColor = true;
            // 
            // Add_bttn
            // 
            this.Add_bttn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Add_bttn.Location = new System.Drawing.Point(6, 271);
            this.Add_bttn.Name = "Add_bttn";
            this.Add_bttn.Size = new System.Drawing.Size(70, 23);
            this.Add_bttn.TabIndex = 1;
            this.Add_bttn.Text = "Add";
            this.Add_bttn.UseVisualStyleBackColor = true;
            this.Add_bttn.Click += new System.EventHandler(this.Add_bttn_Click);
            // 
            // Remove_bttn
            // 
            this.Remove_bttn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Remove_bttn.Location = new System.Drawing.Point(82, 271);
            this.Remove_bttn.Name = "Remove_bttn";
            this.Remove_bttn.Size = new System.Drawing.Size(71, 23);
            this.Remove_bttn.TabIndex = 2;
            this.Remove_bttn.Text = "Remove";
            this.Remove_bttn.UseVisualStyleBackColor = true;
            // 
            // ItemList_lb
            // 
            this.ItemList_lb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemList_lb.FormattingEnabled = true;
            this.ItemList_lb.IntegralHeight = false;
            this.ItemList_lb.Location = new System.Drawing.Point(6, 19);
            this.ItemList_lb.Name = "ItemList_lb";
            this.ItemList_lb.Size = new System.Drawing.Size(223, 220);
            this.ItemList_lb.TabIndex = 0;
            // 
            // ListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.List_gb);
            this.Name = "ListControl";
            this.Size = new System.Drawing.Size(235, 300);
            this.List_gb.ResumeLayout(false);
            this.List_gb.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox List_gb;
        private System.Windows.Forms.TextBox InputField_tb;
        private System.Windows.Forms.ListBox ItemList_lb;
        private System.Windows.Forms.Button Rename_bttn;
        private System.Windows.Forms.Button Add_bttn;
        private System.Windows.Forms.Button Remove_bttn;

    }
}
