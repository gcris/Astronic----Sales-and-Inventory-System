namespace Infrastructure.UserControls
{
    partial class DateNowPanelUI
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblMonth = new System.Windows.Forms.Label();
            this.lblYear = new System.Windows.Forms.Label();
            this.lblDay = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblAm = new System.Windows.Forms.Label();
            this.lblPm = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblDay);
            this.panel1.Controls.Add(this.lblYear);
            this.panel1.Controls.Add(this.lblMonth);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(233, 150);
            this.panel1.TabIndex = 5;
            // 
            // lblMonth
            // 
            this.lblMonth.BackColor = System.Drawing.Color.Teal;
            this.lblMonth.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMonth.Font = new System.Drawing.Font("Calibri", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMonth.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblMonth.Location = new System.Drawing.Point(0, 0);
            this.lblMonth.Name = "lblMonth";
            this.lblMonth.Size = new System.Drawing.Size(233, 50);
            this.lblMonth.TabIndex = 5;
            this.lblMonth.Text = "September";
            this.lblMonth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMonth.UseCompatibleTextRendering = true;
            // 
            // lblYear
            // 
            this.lblYear.BackColor = System.Drawing.Color.Teal;
            this.lblYear.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblYear.Font = new System.Drawing.Font("Calibri", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYear.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblYear.Location = new System.Drawing.Point(0, 100);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(233, 50);
            this.lblYear.TabIndex = 6;
            this.lblYear.Text = "2018";
            this.lblYear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblYear.UseCompatibleTextRendering = true;
            // 
            // lblDay
            // 
            this.lblDay.BackColor = System.Drawing.SystemColors.Control;
            this.lblDay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDay.Font = new System.Drawing.Font("Calibri", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDay.ForeColor = System.Drawing.Color.Black;
            this.lblDay.Location = new System.Drawing.Point(0, 50);
            this.lblDay.Name = "lblDay";
            this.lblDay.Size = new System.Drawing.Size(233, 50);
            this.lblDay.TabIndex = 7;
            this.lblDay.Text = "30";
            this.lblDay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDay.UseCompatibleTextRendering = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(233, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(468, 150);
            this.panel2.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblPm);
            this.panel3.Controls.Add(this.lblAm);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(411, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(57, 150);
            this.panel3.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblTime);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(411, 150);
            this.panel4.TabIndex = 1;
            // 
            // lblTime
            // 
            this.lblTime.BackColor = System.Drawing.Color.Orange;
            this.lblTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTime.Font = new System.Drawing.Font("Calibri", 70F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTime.Location = new System.Drawing.Point(0, 0);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(411, 150);
            this.lblTime.TabIndex = 8;
            this.lblTime.Text = "12:50:40";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTime.UseCompatibleTextRendering = true;
            // 
            // lblAm
            // 
            this.lblAm.BackColor = System.Drawing.Color.Yellow;
            this.lblAm.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAm.Font = new System.Drawing.Font("Calibri", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAm.ForeColor = System.Drawing.Color.DimGray;
            this.lblAm.Location = new System.Drawing.Point(0, 0);
            this.lblAm.Name = "lblAm";
            this.lblAm.Size = new System.Drawing.Size(57, 75);
            this.lblAm.TabIndex = 6;
            this.lblAm.Text = "AM";
            this.lblAm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAm.UseCompatibleTextRendering = true;
            // 
            // lblPm
            // 
            this.lblPm.BackColor = System.Drawing.Color.Gray;
            this.lblPm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblPm.Font = new System.Drawing.Font("Calibri", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPm.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblPm.Location = new System.Drawing.Point(0, 75);
            this.lblPm.Name = "lblPm";
            this.lblPm.Size = new System.Drawing.Size(57, 75);
            this.lblPm.TabIndex = 7;
            this.lblPm.Text = "PM";
            this.lblPm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPm.UseCompatibleTextRendering = true;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // DateNowPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "DateNowPanel";
            this.Size = new System.Drawing.Size(701, 150);
            this.Load += new System.EventHandler(this.DateNowPanel_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblMonth;
        private System.Windows.Forms.Label lblDay;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblAm;
        private System.Windows.Forms.Label lblPm;
        private System.Windows.Forms.Timer timer;
    }
}
