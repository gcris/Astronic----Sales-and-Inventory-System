namespace Infrastructure
{
    partial class MessageBoxForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.picMessageIcon = new System.Windows.Forms.PictureBox();
            this.pnlSeparator = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnlBtn1 = new System.Windows.Forms.Panel();
            this.btn1 = new System.Windows.Forms.Button();
            this.pnlBtn2 = new System.Windows.Forms.Panel();
            this.btn2 = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMessageIcon)).BeginInit();
            this.panel2.SuspendLayout();
            this.pnlBtn1.SuspendLayout();
            this.pnlBtn2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Controls.Add(this.picMessageIcon);
            this.panel1.Controls.Add(this.pnlSeparator);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(413, 52);
            this.panel1.TabIndex = 2;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Calibri", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.DimGray;
            this.lblTitle.Location = new System.Drawing.Point(54, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(359, 50);
            this.lblTitle.TabIndex = 10;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picMessageIcon
            // 
            this.picMessageIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.picMessageIcon.Location = new System.Drawing.Point(0, 0);
            this.picMessageIcon.Name = "picMessageIcon";
            this.picMessageIcon.Size = new System.Drawing.Size(54, 50);
            this.picMessageIcon.TabIndex = 9;
            this.picMessageIcon.TabStop = false;
            // 
            // pnlSeparator
            // 
            this.pnlSeparator.BackColor = System.Drawing.Color.LightGray;
            this.pnlSeparator.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSeparator.Location = new System.Drawing.Point(0, 50);
            this.pnlSeparator.Name = "pnlSeparator";
            this.pnlSeparator.Size = new System.Drawing.Size(413, 2);
            this.pnlSeparator.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.pnlBtn1);
            this.panel2.Controls.Add(this.pnlBtn2);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 186);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(413, 59);
            this.panel2.TabIndex = 3;
            // 
            // pnlBtn1
            // 
            this.pnlBtn1.Controls.Add(this.btn1);
            this.pnlBtn1.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlBtn1.Location = new System.Drawing.Point(213, 2);
            this.pnlBtn1.Name = "pnlBtn1";
            this.pnlBtn1.Padding = new System.Windows.Forms.Padding(10);
            this.pnlBtn1.Size = new System.Drawing.Size(100, 57);
            this.pnlBtn1.TabIndex = 10;
            // 
            // btn1
            // 
            this.btn1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn1.Font = new System.Drawing.Font("Calibri", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn1.Location = new System.Drawing.Point(10, 10);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(80, 37);
            this.btn1.TabIndex = 0;
            this.btn1.Text = "&Yes";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.btn1_Click);
            // 
            // pnlBtn2
            // 
            this.pnlBtn2.Controls.Add(this.btn2);
            this.pnlBtn2.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlBtn2.Location = new System.Drawing.Point(313, 2);
            this.pnlBtn2.Name = "pnlBtn2";
            this.pnlBtn2.Padding = new System.Windows.Forms.Padding(10);
            this.pnlBtn2.Size = new System.Drawing.Size(100, 57);
            this.pnlBtn2.TabIndex = 11;
            // 
            // btn2
            // 
            this.btn2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn2.Font = new System.Drawing.Font("Calibri", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn2.Location = new System.Drawing.Point(10, 10);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(80, 37);
            this.btn2.TabIndex = 0;
            this.btn2.Text = "&No";
            this.btn2.UseVisualStyleBackColor = true;
            this.btn2.Click += new System.EventHandler(this.btn2_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.LightGray;
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(413, 2);
            this.panel4.TabIndex = 9;
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.SystemColors.Control;
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStatus.Location = new System.Drawing.Point(0, 52);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(413, 10);
            this.pnlStatus.TabIndex = 4;
            this.pnlStatus.Visible = false;
            // 
            // lblMessage
            // 
            this.lblMessage.BackColor = System.Drawing.Color.White;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("Calibri", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.Color.Black;
            this.lblMessage.Location = new System.Drawing.Point(0, 62);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(413, 124);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.Text = "Message";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MessageBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 245);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MessageBoxForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Message";
            this.Load += new System.EventHandler(this.MessageBoxForm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picMessageIcon)).EndInit();
            this.panel2.ResumeLayout(false);
            this.pnlBtn1.ResumeLayout(false);
            this.pnlBtn2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox picMessageIcon;
        private System.Windows.Forms.Panel pnlSeparator;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnlBtn1;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Panel pnlBtn2;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblMessage;
    }
}