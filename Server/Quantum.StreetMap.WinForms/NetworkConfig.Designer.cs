namespace Quantum.StreetMap.WinForms
{
    partial class NetworkConfig
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
            this.buttonXClose = new DevComponents.DotNetBar.ButtonX();
            this.buttonXOk = new DevComponents.DotNetBar.ButtonX();
            this.networkProperties1 = new Quantum.StreetMap.WinForms.NetworkProperties();
            this.SuspendLayout();
            // 
            // buttonXClose
            // 
            this.buttonXClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonXClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonXClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonXClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonXClose.Location = new System.Drawing.Point(345, 286);
            this.buttonXClose.Name = "buttonXClose";
            this.buttonXClose.Size = new System.Drawing.Size(75, 23);
            this.buttonXClose.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonXClose.TabIndex = 1;
            this.buttonXClose.Text = "Close";
            this.buttonXClose.Click += new System.EventHandler(this.buttonXClose_Click);
            // 
            // buttonXOk
            // 
            this.buttonXOk.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonXOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonXOk.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonXOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonXOk.Location = new System.Drawing.Point(264, 286);
            this.buttonXOk.Name = "buttonXOk";
            this.buttonXOk.Size = new System.Drawing.Size(75, 23);
            this.buttonXOk.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonXOk.TabIndex = 2;
            this.buttonXOk.Text = "OK";
            this.buttonXOk.Click += new System.EventHandler(this.buttonXOk_Click);
            // 
            // networkProperties1
            // 
            this.networkProperties1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.networkProperties1.Location = new System.Drawing.Point(12, 12);
            this.networkProperties1.Name = "networkProperties1";
            this.networkProperties1.Route = null;
            this.networkProperties1.Size = new System.Drawing.Size(408, 266);
            this.networkProperties1.TabIndex = 0;
            this.networkProperties1.Load += new System.EventHandler(this.networkProperties1_Load);
            // 
            // NetworkConfig
            // 
            this.AcceptButton = this.buttonXOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonXClose;
            this.ClientSize = new System.Drawing.Size(432, 321);
            this.Controls.Add(this.buttonXOk);
            this.Controls.Add(this.buttonXClose);
            this.Controls.Add(this.networkProperties1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NetworkConfig";
            this.RenderFormIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Network Configuration";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private NetworkProperties networkProperties1;
        private DevComponents.DotNetBar.ButtonX buttonXClose;
        private DevComponents.DotNetBar.ButtonX buttonXOk;
    }
}