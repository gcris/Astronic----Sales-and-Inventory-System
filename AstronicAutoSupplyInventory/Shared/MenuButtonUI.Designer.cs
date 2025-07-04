namespace AstronicAutoSupplyInventory.Shared
{
    partial class MenuButtonUI
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
            this.components = new System.ComponentModel.Container();
            this.lblButtonName = new System.Windows.Forms.Label();
            this.pnlBackground = new System.Windows.Forms.Panel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.pnlLeftStatus = new System.Windows.Forms.Panel();
            this.mnuMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.pnlBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblButtonName
            // 
            this.lblButtonName.BackColor = System.Drawing.Color.Transparent;
            this.lblButtonName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblButtonName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblButtonName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblButtonName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblButtonName.Location = new System.Drawing.Point(37, 5);
            this.lblButtonName.Name = "lblButtonName";
            this.lblButtonName.Size = new System.Drawing.Size(118, 36);
            this.lblButtonName.TabIndex = 13;
            this.lblButtonName.Text = "Button Name";
            this.lblButtonName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblButtonName.Click += new System.EventHandler(this.button_Click);
            this.lblButtonName.MouseLeave += new System.EventHandler(this.MenuButton_MouseLeave);
            this.lblButtonName.MouseHover += new System.EventHandler(this.MenuButton_MouseHover);
            // 
            // pnlBackground
            // 
            this.pnlBackground.Controls.Add(this.lblButtonName);
            this.pnlBackground.Controls.Add(this.picLogo);
            this.pnlBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBackground.Location = new System.Drawing.Point(5, 0);
            this.pnlBackground.Name = "pnlBackground";
            this.pnlBackground.Padding = new System.Windows.Forms.Padding(5, 5, 0, 5);
            this.pnlBackground.Size = new System.Drawing.Size(155, 46);
            this.pnlBackground.TabIndex = 6;
            this.pnlBackground.Click += new System.EventHandler(this.button_Click);
            this.pnlBackground.MouseLeave += new System.EventHandler(this.MenuButton_MouseLeave);
            this.pnlBackground.MouseHover += new System.EventHandler(this.MenuButton_MouseHover);
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.Transparent;
            this.picLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picLogo.Dock = System.Windows.Forms.DockStyle.Left;
            this.picLogo.Location = new System.Drawing.Point(5, 5);
            this.picLogo.Name = "picLogo";
            this.picLogo.Padding = new System.Windows.Forms.Padding(1);
            this.picLogo.Size = new System.Drawing.Size(32, 36);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 12;
            this.picLogo.TabStop = false;
            this.picLogo.Click += new System.EventHandler(this.button_Click);
            this.picLogo.MouseLeave += new System.EventHandler(this.MenuButton_MouseLeave);
            this.picLogo.MouseHover += new System.EventHandler(this.MenuButton_MouseHover);
            // 
            // pnlLeftStatus
            // 
            this.pnlLeftStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnlLeftStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeftStatus.Location = new System.Drawing.Point(0, 0);
            this.pnlLeftStatus.Name = "pnlLeftStatus";
            this.pnlLeftStatus.Size = new System.Drawing.Size(5, 46);
            this.pnlLeftStatus.TabIndex = 5;
            this.pnlLeftStatus.Visible = false;
            // 
            // mnuMenu
            // 
            this.mnuMenu.Font = new System.Drawing.Font("Calibri", 15F);
            this.mnuMenu.Name = "mnuMenu";
            this.mnuMenu.Size = new System.Drawing.Size(61, 4);
            this.mnuMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.mnuMenu_Closed);
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // MenuButtonUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pnlBackground);
            this.Controls.Add(this.pnlLeftStatus);
            this.Name = "MenuButtonUI";
            this.Size = new System.Drawing.Size(160, 46);
            this.Load += new System.EventHandler(this.MenuButton_Load);
            this.Click += new System.EventHandler(this.button_Click);
            this.MouseLeave += new System.EventHandler(this.MenuButton_MouseLeave);
            this.MouseHover += new System.EventHandler(this.MenuButton_MouseHover);
            this.pnlBackground.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblButtonName;
        private System.Windows.Forms.Panel pnlBackground;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Panel pnlLeftStatus;
        private System.Windows.Forms.ContextMenuStrip mnuMenu;
        private System.Windows.Forms.Timer timer;
    }
}
