namespace AstronicAutoSupplyInventory.Shared
{
    partial class IndexForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndexForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.pnlContent = new System.Windows.Forms.Panel();
            this.picBatch = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.picLaoag = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.panel1.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1218, 76);
            this.panel1.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Cambria", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.DimGray;
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1125, 73);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Astronic Auto-Supply Sales and Inventory System";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.DarkGray;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 73);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1218, 3);
            this.panel3.TabIndex = 1;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlContent.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pnlContent.Controls.Add(this.picBatch);
            this.pnlContent.Controls.Add(this.picLaoag);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 76);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(1218, 329);
            this.pnlContent.TabIndex = 1;
            this.pnlContent.Resize += new System.EventHandler(this.pnlContent_Resize);
            // 
            // picBatch
            // 
            this.picBatch.BackColor = System.Drawing.Color.Transparent;
            this.picBatch.BorderShadowColor = System.Drawing.Color.Gainsboro;
            this.picBatch.BorderShadowDepth = ((byte)(50));
            this.picBatch.Dock = System.Windows.Forms.DockStyle.Left;
            this.picBatch.Image = ((object)(resources.GetObject("picBatch.Image")));
            this.picBatch.ImageTransparentColor = System.Drawing.Color.White;
            this.picBatch.Location = new System.Drawing.Point(587, 0);
            this.picBatch.Name = "picBatch";
            this.picBatch.Padding = new System.Drawing.Size(10, 10);
            this.picBatch.Size = new System.Drawing.Size(410, 329);
            this.picBatch.TabIndex = 0;
            // 
            // picLaoag
            // 
            this.picLaoag.BackColor = System.Drawing.Color.Transparent;
            this.picLaoag.BorderShadowColor = System.Drawing.Color.Gainsboro;
            this.picLaoag.BorderShadowDepth = ((byte)(50));
            this.picLaoag.Dock = System.Windows.Forms.DockStyle.Left;
            this.picLaoag.Image = ((object)(resources.GetObject("picLaoag.Image")));
            this.picLaoag.ImageTransparentColor = System.Drawing.Color.White;
            this.picLaoag.Location = new System.Drawing.Point(0, 0);
            this.picLaoag.Name = "picLaoag";
            this.picLaoag.Padding = new System.Drawing.Size(10, 10);
            this.picLaoag.Size = new System.Drawing.Size(587, 329);
            this.picLaoag.TabIndex = 1;
            // 
            // IndexForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1218, 405);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IndexForm";
            this.Opacity = 0.6D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Home";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.IndexForm_Load);
            this.panel1.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Timer timer;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picBatch;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picLaoag;
    }
}