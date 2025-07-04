using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infrastructure
{
    public partial class MessageBoxForm : Form
    {
        private static MessageBoxForm instance;
        private DialogResult result = DialogResult.OK;
        private MessageBoxButtons buttons;

        public MessageBoxForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Escape:
                    if (buttons == MessageBoxButtons.YesNo) btn2_Click(this, new EventArgs());
                    else btn1_Click(this, new EventArgs());

                    return true;
                case Keys.Left:
                case Keys.Right:
                    SendKeys.Send("{TAB}");

                    return true;
            }
            return base.ProcessCmdKey(ref message, keys);
        }

        public static MessageBoxForm Instance()
        {
            if (instance == null) instance = new MessageBoxForm();

            return instance;
        }

        private void MessageBoxForm_Load(object sender, EventArgs e)
        {

        }

        public DialogResult ShowMessage(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            this.buttons = buttons;

            picMessageIcon.Image = icon == MessageBoxIcon.Error ? SystemIcons.Error.ToBitmap() :
                icon == MessageBoxIcon.Question ? SystemIcons.Question.ToBitmap() :
                SystemIcons.Information.ToBitmap();
            picMessageIcon.SizeMode = PictureBoxSizeMode.CenterImage;

            lblMessage.Text = message;

            lblTitle.Text = title;

            this.Text = title;

            if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.OKCancel)
            {
                btn1.Text = buttons == MessageBoxButtons.YesNo ? "&Yes" : "&OK";

                btn2.Text = buttons == MessageBoxButtons.YesNo ? "&No" : "&Cancel";

                pnlBtn1.Visible = true;

                pnlBtn2.Visible = true;
            }
            else
            {
                btn1.Text = "&OK";

                pnlBtn2.Visible = false;
            }

            btn1.Focus();

            pnlStatus.BackColor = icon == MessageBoxIcon.Error ? Color.Red : Color.LightSeaGreen;

            this.TopMost = true;

            if (!this.Visible) this.ShowDialog();

            this.BringToFront();

            return result;
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            result = buttons == MessageBoxButtons.YesNo ? DialogResult.No : DialogResult.Cancel;

            Close();
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            result = buttons == MessageBoxButtons.YesNo ? DialogResult.Yes : DialogResult.OK;

            Close();
        }
    }
}
