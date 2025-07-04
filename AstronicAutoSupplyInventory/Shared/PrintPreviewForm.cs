using InventoryServices.Controllers;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AstronicAutoSupplyInventory.Shared
{
    public partial class PrintPreviewForm : Form
    {
        private readonly string title, reportFile;
        private readonly List<ReportDataSource> sources, subSources;
        private readonly List<ReportParameter> parameters;

        private UserController userController = new UserController();

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public PrintPreviewForm(string title, string reportFile, List<ReportDataSource> sources, List<ReportDataSource> subSources = null, List<ReportParameter> parameters = null)
        {
            this.title = title;
            this.reportFile = reportFile;
            this.sources = sources;
            this.subSources = subSources;
            this.parameters = parameters;
            InitializeComponent();

            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            Width = screen.Width - 50;

            Height = screen.Height - 50;
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Control | Keys.P:
                    reportViewer.PrintDialog();

                    return true;   
                case Keys.Escape:
                    this.Close();

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private void SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            subSources.ForEach(source => e.DataSources.Add(source));
        }

        private void PrintPreviewForm_Load(object sender, EventArgs e)
        {
            this.reportViewer.RefreshReport();

            lblTitle.Text = title;

            reportViewer.ProcessingMode = ProcessingMode.Local;

            reportViewer.LocalReport.ReportEmbeddedResource = "AstronicAutoSupplyInventory.Reports." + reportFile;

            sources.ForEach(source => reportViewer.LocalReport.DataSources.Add(source));

            if (subSources != null) // means the report contains sub report
                reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessing);

            reportViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);

            if (parameters != null) reportViewer.LocalReport.SetParameters(parameters);

            reportViewer.ReportExport += reportViewer_ReportExport;

            reportViewer.Print += reportViewer_Print;

            reportViewer.Dock = DockStyle.Fill;
        }

        private async void reportViewer_Print(object sender, ReportPrintEventArgs e)
        {
            if (!e.Cancel)
            {
                await userController.SaveActivity(
                    string.Format("Prints file '{0}'", lblTitle.Text),
                    mainForm.UserDtos.UserId);
            }
        }

        private async void reportViewer_ReportExport(object sender, ReportExportEventArgs e)
        {
            if (!e.Cancel)
            {
                await userController.SaveActivity(
                    string.Format("Exports file '{0}' as '{1}'", lblTitle.Text, e.Extension.Name),
                    mainForm.UserDtos.UserId);
            }
        }
    }
}
