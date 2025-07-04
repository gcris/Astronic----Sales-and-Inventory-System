using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CommonLibrary.Dtos;
using Infragistics.Win.DataVisualization;

namespace AstronicAutoSupplyInventory.Shared
{
    public partial class FrequencyGraphUI : UserControl
    {
        private readonly string title;
        private readonly IEnumerable<TransactionFrequencyDtos> source;

        public FrequencyGraphUI(IEnumerable<TransactionFrequencyDtos> sources, string title)
        {
            this.source = sources;
            
            this.title = title;

            InitializeComponent();
        }

        private void FrequencyGraphUI_Load(object sender, EventArgs e)
        {
            var numericAxis = new NumericYAxis();
            //numericAxis.Label = "TotalProcessed";
            numericAxis.Title = "Amount";
            numericAxis.FormatLabel += yAxis_FormatLabel;
            numericAxis.TitleFontSize = 8;
            numericAxis.LabelFontSize = 8;

            var categoryAxis = new CategoryXAxis();
            categoryAxis.Label = "Date";
            //categoryAxis.Title = "Date";
            categoryAxis.FormatLabel += xAxis_FormatLabel;
            categoryAxis.DataSource = source;
            categoryAxis.TitleFontSize = 8;
            categoryAxis.LabelFontSize = 8;
                
            var series = new SplineSeries();
            series.ValueMemberPath = "Amount";
            series.Title = title;
            series.DataSource = source;
            series.Legend = ultraLegend;
            series.XAxis = categoryAxis;
            series.YAxis = numericAxis;
            ultraDataChart.Series.Add(series);

            ultraDataChart.Axes.Add(numericAxis);
            ultraDataChart.Axes.Add(categoryAxis);
        }

        private string xAxis_FormatLabel(AxisLabelInfo info)
        {
            var data = (TransactionFrequencyDtos)info.Item;
            if (data == null) return null;

            var date = data.Date;
            return string.Format("{0:MM/dd}", date);
        }

        private string yAxis_FormatLabel(AxisLabelInfo info)
        {
            return info.Value.ToString("#,0;");
        }
    }
}
