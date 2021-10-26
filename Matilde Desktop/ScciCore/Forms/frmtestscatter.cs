using Infragistics.UltraChart.Resources.Appearance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore
{
    public partial class frmtestscatter : Form
    {
        public frmtestscatter()
        {
            InitializeComponent();
        }

        private void frmtestscatter_Load(object sender, EventArgs e)
        {
            try
            {
                InizializzaGrafico();

                DefinizioneGrafico defgraf = CoreStatics.CoreApplication.DefinizioneGraficoSelezionata;
                List<string> codicitipiPV = new List<string>();
                codicitipiPV.Add("PRS");
                                Dictionary<string, DataTable> dictDati = defgraf.getDataTablesDatiPerGrafico(DateTime.MinValue, DateTime.MinValue, codicitipiPV, null);

                if (dictDati.Count > 0)
                {
                    for (int i = 0; i < dictDati.Count; i++)
                    {

                        DataTable dt = dictDati.ElementAt(i).Value;

                        foreach (DataColumn ocol in dt.Columns)
                        {
                            if (ocol.ColumnName != "DataOra")
                            {
                                NumericTimeSeries ntsS = new NumericTimeSeries();
                                ntsS.Key = ocol.ColumnName;
                                ntsS.Label = ocol.ColumnName;
                                ntsS.DataBind(dt, "DataOra", ocol.ColumnName);
                                                                this.ultraChart1.Series.Add(ntsS);
                            }
                        }
                    }
                    
                                                                                DateTime datamin = new DateTime(2013, 5, 14, 17, 35, 0);
                    DateTime datamax = new DateTime(2013, 5, 14, 22, 40, 0);
                    if (datamin > DateTime.MinValue && datamax > DateTime.MinValue)
                    {
                        try
                        {
                            this.ultraChart1.Axis.X.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                                                        long rangeMin = datamin.ToBinary();
                            long rangeMax = datamax.ToBinary();
                            long margin = 0;
                            if (rangeMax > rangeMin)
                                margin = ((rangeMax - rangeMin) / 100) * 3;
                            else
                                margin = (rangeMin / 100) * 3;
                                                        this.ultraChart1.Axis.X.RangeMin = rangeMin - margin;
                            this.ultraChart1.Axis.X.RangeMax = rangeMax + margin;
                        }
                        catch (Exception ex)
                        {
                        }
                    }


                                                                                float minval = float.MinValue;
                    float maxval = float.MinValue;
                    for (int i = 0; i < dictDati.Count; i++)
                    {

                        DataTable dt = dictDati.ElementAt(i).Value;

                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                foreach (DataColumn col in dt.Columns)
                                {
                                    if (col.ColumnName.ToUpper() != "DataOra".ToUpper() && !row.IsNull(col.ColumnName))
                                    {
                                        if (minval == float.MinValue) minval = (float)row[col.ColumnName];
                                        if (maxval == float.MinValue) maxval = (float)row[col.ColumnName];
                                                                                if (minval > (float)row[col.ColumnName]) minval = (float)row[col.ColumnName];
                                        if (maxval < (float)row[col.ColumnName]) maxval = (float)row[col.ColumnName];
                                    }
                                }
                            }
                        }
                    }
                    if (minval > float.MinValue && maxval > float.MinValue)
                    {
                        this.ultraChart1.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                        this.ultraChart1.Axis.Y.RangeMin = minval - 5;
                        this.ultraChart1.Axis.Y.RangeMax = maxval + 5;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InizializzaGrafico()
        {
            try
            {
                                                this.ultraChart1.Axis.X.Extent = 70;
                this.ultraChart1.Axis.X.Labels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                this.ultraChart1.Axis.X.Labels.HorizontalAlign = System.Drawing.StringAlignment.Center;
                this.ultraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL:dd/MM/yy HH:mm>";
                this.ultraChart1.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Custom;
                this.ultraChart1.Axis.X.Labels.OrientationAngle = 75;
                                this.ultraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                this.ultraChart1.Axis.X.Labels.SeriesLabels.FormatString = "";
                this.ultraChart1.Axis.X.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart1.Axis.X.Labels.VerticalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart1.Axis.X.LineThickness = 1;
                this.ultraChart1.Axis.X.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
                this.ultraChart1.Axis.X.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart1.Axis.X.MajorGridLines.Visible = true;
                this.ultraChart1.Axis.X.MinorGridLines.Color = System.Drawing.Color.LightGray;
                this.ultraChart1.Axis.X.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart1.Axis.X.MinorGridLines.Visible = false;
                this.ultraChart1.Axis.X.Visible = true;
                                                this.ultraChart1.Axis.X.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
                this.ultraChart1.Axis.X.TimeAxisStyle.TimeAxisStyle = Infragistics.UltraChart.Shared.Styles.RulerGenre.Discrete;

                this.ultraChart1.Axis.Y.Extent = 25;
                this.ultraChart1.Axis.Y.Labels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                this.ultraChart1.Axis.Y.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:00.00#>";
                this.ultraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                this.ultraChart1.Axis.Y.Labels.SeriesLabels.FormatString = "";
                this.ultraChart1.Axis.Y.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart1.Axis.Y.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
                this.ultraChart1.Axis.Y.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
                this.ultraChart1.Axis.Y.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
                this.ultraChart1.Axis.Y.LineThickness = 1;
                this.ultraChart1.Axis.Y.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
                this.ultraChart1.Axis.Y.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart1.Axis.Y.MajorGridLines.Visible = true;
                this.ultraChart1.Axis.Y.MinorGridLines.Color = System.Drawing.Color.LightGray;
                this.ultraChart1.Axis.Y.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart1.Axis.Y.MinorGridLines.Visible = false;
                this.ultraChart1.Axis.Y.TickmarkInterval = 1D;
                this.ultraChart1.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
                this.ultraChart1.Axis.Y.Visible = true;
                                this.ultraChart1.ColorModel.ColorBegin = System.Drawing.Color.Pink;
                this.ultraChart1.ColorModel.ColorEnd = System.Drawing.Color.DarkRed;
                this.ultraChart1.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
                                                                this.ultraChart1.Legend.BorderColor = System.Drawing.Color.Transparent;
                this.ultraChart1.Legend.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall));
                this.ultraChart1.Legend.Margins.Bottom = 1;
                this.ultraChart1.Legend.Margins.Left = 1;
                this.ultraChart1.Legend.Margins.Right = 1;
                this.ultraChart1.Legend.Margins.Top = 1;
                this.ultraChart1.Legend.SpanPercentage = 15;
                this.ultraChart1.Legend.Visible = true;
                                this.ultraChart1.ScatterChart.ChartText[0].ChartTextFont = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall) - 1);
                this.ultraChart1.ScatterChart.ChartText[0].ClipText = false;
                this.ultraChart1.ScatterChart.ChartText[0].Column = -2;
                this.ultraChart1.ScatterChart.ChartText[0].ItemFormatString = "<DATA_VALUE_Y:00.00#>";
                this.ultraChart1.ScatterChart.ChartText[0].Row = -2;
                this.ultraChart1.ScatterChart.ChartText[0].VerticalAlign = StringAlignment.Far;
                this.ultraChart1.ScatterChart.ChartText[0].HorizontalAlign = StringAlignment.Near;
                this.ultraChart1.ScatterChart.ChartText[0].Visible = true;
                                                                                                                                this.ultraChart1.ScatterChart.NullHandling = Infragistics.UltraChart.Shared.Styles.NullHandling.DontPlot;
                                                                            }
            catch (Exception ex)
            {
            }
        }

    }
}
