using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinChart;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using UnicodeSrl.Scci.Enums;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Shared.Styles;
using UnicodeSrl.ScciCore.UserControls;
using System.Collections;

namespace UnicodeSrl.ScciCore
{
    internal partial class ucSCCIScatterChart : UserControl, Interfacce.IViewUserControlBase
    {
        #region DECLARE

        private string _campodataorapergrafico = string.Empty;
        private bool _lineeGiornaliereVisibili = true;
        private bool _lineeGiornaliereAbilitate = true;

        #endregion

        #region PROPRIETA'

        public ucEasyLabel Titolo
        {
            get { return this.lblTitolo; }
            set { this.lblTitolo = value; }
        }

        public UltraChart Grafico
        {
            get { return this.ultraChart; }
            set { this.ultraChart = value; }
        }

        public bool TitoloNascosto
        {
            get { return !this.lblTitolo.Visible; }
            set { this.lblTitolo.Visible = !value; }
        }

        public bool LineeGiornaliereVisibili
        {
            get
            {
                return _lineeGiornaliereVisibili;
            }
            set
            {
                _lineeGiornaliereVisibili = value;
                setTickmark(this.LineeGiornaliereAbilitate && value);

            }
        }

        public bool LineeGiornaliereAbilitate
        {
            get
            {
                return _lineeGiornaliereAbilitate;
            }
        }

        public EnumEntita Entita { get; set; }

        public List<ChartMarker> ChartMarkers { get; private set; }

        #endregion

        public ucSCCIScatterChart()
        {
            InitializeComponent();

            this.ChartMarkers = new List<ChartMarker>();
        }

        #region INTERFACCIA

        public void ViewInit()
        {

            InizializzaControlli();

        }

        #endregion

        #region METODI PUBBLICI

        public void SetMarkers(List<ChartMarker> chartMarkers)
        {
            this.ChartMarkers = chartMarkers;
            this.ultraChart.InvalidateLayers();
        }

        public void CaricaDati(ref DataTable dtGrafico)
        {
            try
            {
                CaricaDati(ref dtGrafico, float.MinValue, float.MinValue, DateTime.MinValue, DateTime.MinValue, "", new List<string>(), _lineeGiornaliereVisibili);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void CaricaDati(ref DataTable dtGrafico, float inizioscala, float finescala, DateTime datamin, DateTime datamax, string campodataorapergrafico, List<string> campidimensione, bool lineeGiornaliereVisibili)
        {

            _campodataorapergrafico = campodataorapergrafico;
            _lineeGiornaliereVisibili = lineeGiornaliereVisibili;

            if (_campodataorapergrafico == string.Empty || _campodataorapergrafico.Trim() == "")
                _campodataorapergrafico = CoreStatics.CoreApplication.DefinizioneGraficoSelezionata.CampoDataOraPerGrafici;

            try
            {

                DateTime datarif = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                if (dtGrafico != null && dtGrafico.Rows.Count <= 0)
                {
                    if (datamin > DateTime.MinValue && datamax > DateTime.MinValue)
                    {
                        if (datamax > datamin)
                        {
                            datarif = new DateTime(datamin.Ticks + Convert.ToInt64((datamax.Ticks - datamin.Ticks) / 2));
                        }
                        else
                            datarif = datamax;
                    }

                    DataRow drempty = dtGrafico.NewRow();
                    foreach (DataColumn col in dtGrafico.Columns)
                    {
                        if (col.ColumnName == _campodataorapergrafico)
                            drempty[col.ColumnName] = datarif;
                        else
                            drempty[col.ColumnName] = System.DBNull.Value;
                    }
                    dtGrafico.Rows.Add(drempty);
                }


                _lineeGiornaliereAbilitate = UnicodeSrl.Scci.Statics.Database.LineeGiornaliereAbilitate(datamin, datamax);

                if (datamin > DateTime.MinValue && datamax > DateTime.MinValue)
                {
                    try
                    {
                        this.ultraChart.Axis.X.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                        long rangeMin = datamin.Ticks;
                        long rangeMax = datamax.Ticks;
                        double margin = 0;
                        if (rangeMax > rangeMin)
                            margin = ((Convert.ToDouble(rangeMax) - Convert.ToDouble(rangeMin)) / 100) * 6;
                        else
                            margin = (Convert.ToDouble(rangeMin) / 100) * 6;
                        rangeMin = rangeMin - Convert.ToInt64(margin);
                        if (!datamin.Date.Equals(datamin.Date))
                        {


                            if (_lineeGiornaliereAbilitate)
                            {
                                DateTime dtmp = new DateTime(rangeMin);
                                if (dtmp.AddDays(1).Date <= datamin)
                                    rangeMin = dtmp.AddDays(1).Date.Ticks;
                                else
                                    rangeMin = dtmp.Date.Ticks;
                            }
                        }
                        this.ultraChart.Axis.X.RangeMin = rangeMin;
                        this.ultraChart.Axis.X.RangeMax = rangeMax + Convert.ToInt64(margin);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    try
                    {
                        this.ultraChart.Axis.X.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                        long rangeMin = datarif.Ticks;
                        long rangeMax = rangeMin;
                        double margin = 0;
                        if (rangeMax > rangeMin)
                            margin = ((Convert.ToDouble(rangeMax) - Convert.ToDouble(rangeMin)) / 100) * 3;
                        else
                            margin = (Convert.ToDouble(rangeMin) / 100) * 3;
                        rangeMin = rangeMin - Convert.ToInt64(margin);
                        if (_lineeGiornaliereAbilitate)
                        {
                            DateTime dtmp = new DateTime(rangeMin);
                            rangeMin = dtmp.Date.Ticks;
                        }
                        this.ultraChart.Axis.X.RangeMin = rangeMin;
                        this.ultraChart.Axis.X.RangeMax = rangeMax + Convert.ToInt64(margin);
                    }
                    catch (Exception)
                    {
                    }
                }

                setTickmark(_lineeGiornaliereAbilitate && _lineeGiornaliereVisibili);

                float minval = inizioscala;
                float maxval = finescala;
                if (inizioscala == float.MinValue && finescala == float.MinValue && campodataorapergrafico != "" && dtGrafico.Rows.Count > 0)
                {
                    foreach (DataRow row in dtGrafico.Rows)
                    {
                        foreach (DataColumn col in dtGrafico.Columns)
                        {
                            if (col.ColumnName.ToUpper() != campodataorapergrafico.ToUpper() && !row.IsNull(col.ColumnName))
                            {
                                if (minval == float.MinValue) minval = (float)row[col.ColumnName];
                                if (maxval == float.MinValue) maxval = (float)row[col.ColumnName];
                                if (minval > (float)row[col.ColumnName]) minval = (float)row[col.ColumnName];
                                if (maxval < (float)row[col.ColumnName]) maxval = (float)row[col.ColumnName];
                            }
                        }
                    }
                    if (minval > float.MinValue && maxval > float.MinValue)
                    {
                        minval -= 5;
                        maxval += 5;
                    }
                }
                if (minval > float.MinValue && maxval > float.MinValue)
                {
                    this.ultraChart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                    this.ultraChart.Axis.Y.RangeMin = minval;
                    this.ultraChart.Axis.Y.RangeMax = maxval;
                }

                foreach (DataColumn ocol in dtGrafico.Columns)
                {
                    if (ocol.DataType.Name.ToUpper() != "DateTime".ToUpper()
                        && (campidimensione == null || campidimensione.Count <= 0 || campidimensione.Contains(ocol.ColumnName)))
                    {
                        try
                        {
                            NumericTimeSeries ntsS = new NumericTimeSeries();
                            ntsS.Key = ocol.ColumnName;
                            ntsS.Label = ocol.ColumnName;
                            ntsS.DataBind(dtGrafico, campodataorapergrafico, ocol.ColumnName);
                            this.ultraChart.Series.Add(ntsS);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception(@"<" + this.Name + @".CaricaDati(grafico)>" + Environment.NewLine + ex.Message, ex);
            }

        }

        #endregion

        #region PRIVATE

        private void InizializzaControlli()
        {
            InizializzaGrafico();
        }

        private void InizializzaGrafico()
        {
            try
            {
                this.ultraChart.Axis.X.Extent = 76;
                this.ultraChart.Axis.X.Labels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall));
                this.ultraChart.Axis.X.Labels.HorizontalAlign = System.Drawing.StringAlignment.Center;
                this.ultraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL:d/M/yy HH:mm>";
                this.ultraChart.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Custom;
                this.ultraChart.Axis.X.Labels.OrientationAngle = 75;
                this.ultraChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                this.ultraChart.Axis.X.Labels.SeriesLabels.FormatString = "";
                this.ultraChart.Axis.X.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart.Axis.X.Labels.VerticalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart.Axis.X.LineThickness = 1;
                this.ultraChart.Axis.X.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
                this.ultraChart.Axis.X.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart.Axis.X.MajorGridLines.Visible = true;
                this.ultraChart.Axis.X.MinorGridLines.Color = System.Drawing.Color.LightGray;
                this.ultraChart.Axis.X.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart.Axis.X.MinorGridLines.Visible = false;
                this.ultraChart.Axis.X.Visible = true;
                this.ultraChart.Axis.X.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart; this.ultraChart.Axis.X.TimeAxisStyle.TimeAxisStyle = Infragistics.UltraChart.Shared.Styles.RulerGenre.Discrete;
                this.ultraChart.Axis.Y.Extent = 35;
                this.ultraChart.Axis.Y.Labels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                this.ultraChart.Axis.Y.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:0.00#>";
                this.ultraChart.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                this.ultraChart.Axis.Y.Labels.SeriesLabels.FormatString = "";
                this.ultraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
                this.ultraChart.Axis.Y.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
                this.ultraChart.Axis.Y.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
                this.ultraChart.Axis.Y.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
                this.ultraChart.Axis.Y.LineThickness = 1;
                this.ultraChart.Axis.Y.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
                this.ultraChart.Axis.Y.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart.Axis.Y.MajorGridLines.Visible = true;
                this.ultraChart.Axis.Y.MinorGridLines.Color = System.Drawing.Color.LightGray;
                this.ultraChart.Axis.Y.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                this.ultraChart.Axis.Y.MinorGridLines.Visible = false;
                this.ultraChart.Axis.Y.TickmarkInterval = 1D;
                this.ultraChart.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
                this.ultraChart.Axis.Y.Visible = true;
                this.ultraChart.ColorModel.ColorBegin = System.Drawing.Color.Pink;
                this.ultraChart.ColorModel.ColorEnd = System.Drawing.Color.DarkRed;
                this.ultraChart.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
                this.ultraChart.Legend.BorderColor = System.Drawing.Color.Transparent;
                this.ultraChart.Legend.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall));
                this.ultraChart.Legend.Margins.Bottom = 1;
                this.ultraChart.Legend.Margins.Left = 1;
                this.ultraChart.Legend.Margins.Right = 1;
                this.ultraChart.Legend.Margins.Top = 1;
                this.ultraChart.Legend.SpanPercentage = 15;
                this.ultraChart.Legend.Visible = true;
                this.ultraChart.ScatterChart.ChartText[0].ChartTextFont = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small) * 0.90F);
                this.ultraChart.ScatterChart.ChartText[0].ClipText = false;
                this.ultraChart.ScatterChart.ChartText[0].Column = -2;
                this.ultraChart.ScatterChart.ChartText[0].ItemFormatString = "<DATA_VALUE_Y:0.00#>";
                this.ultraChart.ScatterChart.ChartText[0].Row = -2;
                this.ultraChart.ScatterChart.ChartText[0].VerticalAlign = StringAlignment.Far;
                this.ultraChart.ScatterChart.ChartText[0].HorizontalAlign = StringAlignment.Far;
                this.ultraChart.ScatterChart.ChartText[0].Visible = true;
                this.ultraChart.ScatterChart.LineAppearance.Thickness = 4;

                float splineTension = 0.27F;
                if (!float.TryParse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(Scci.Enums.EnumConfigTable.FattoreCurvaturaGrafici), out splineTension))
                    splineTension = 0.27F;
                else
                    splineTension /= 100F;
                this.ultraChart.ScatterChart.LineAppearance.SplineTension = splineTension;

                this.ultraChart.ScatterChart.Icon = Infragistics.UltraChart.Shared.Styles.SymbolIcon.Square;
                this.ultraChart.ScatterChart.IconSize = Infragistics.UltraChart.Shared.Styles.SymbolIconSize.Auto;
                this.ultraChart.ScatterChart.NullHandling = Infragistics.UltraChart.Shared.Styles.NullHandling.DontPlot;


                Hashtable ht = new Hashtable();
                ht.Add("ChartTooltip", new ChartTooltip());

                this.ultraChart.Tooltips.Format = TooltipStyle.Custom;
                this.ultraChart.Tooltips.FormatString = "<ChartTooltip>";
                this.ultraChart.Tooltips.Overflow = TooltipOverflow.ChartArea;
                this.ultraChart.LabelHash = ht;

                this.ultraChart.Tooltips.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall));
                this.ultraChart.Tooltips.HighlightFillColor = System.Drawing.Color.DimGray;
                this.ultraChart.Tooltips.HighlightOutlineColor = System.Drawing.Color.DarkGray;


            }
            catch (Exception)
            {
            }
        }

        private void setTickmark(bool visible)
        {
            try
            {
                if (visible)
                {
                    this.ultraChart.Axis.X2.Extent = 10;
                    this.ultraChart.Axis.X2.Labels.Font = new System.Drawing.Font(this.Font.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall));
                    this.ultraChart.Axis.X2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
                    this.ultraChart.Axis.X2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
                    this.ultraChart.Axis.X2.Labels.VerticalAlign = System.Drawing.StringAlignment.Near;
                    this.ultraChart.Axis.X2.Labels.ItemFormatString = "<ITEM_LABEL:d/M/yy>";
                    this.ultraChart.Axis.X2.Labels.Visible = true;

                    this.ultraChart.Axis.X2.LineThickness = 0;
                    this.ultraChart.Axis.X2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
                    this.ultraChart.Axis.X2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                    this.ultraChart.Axis.X2.MajorGridLines.Visible = false;

                    this.ultraChart.Axis.X2.MinorGridLines.Thickness = 1;
                    this.ultraChart.Axis.X2.MinorGridLines.Color = System.Drawing.Color.DarkGray;
                    this.ultraChart.Axis.X2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Solid;
                    this.ultraChart.Axis.X2.MinorGridLines.Visible = true;

                    this.ultraChart.Axis.X2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.DataInterval;
                    this.ultraChart.Axis.X2.TickmarkIntervalType = Infragistics.UltraChart.Shared.Styles.AxisIntervalType.Days;
                    this.ultraChart.Axis.X2.TickmarkInterval = 1;
                    this.ultraChart.Axis.X2.TimeAxisStyle.TimeAxisStyle = Infragistics.UltraChart.Shared.Styles.RulerGenre.Continuous;

                    this.ultraChart.Axis.X2.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                    this.ultraChart.Axis.X2.RangeMin = this.ultraChart.Axis.X.RangeMin;
                    this.ultraChart.Axis.X2.RangeMax = this.ultraChart.Axis.X.RangeMax;

                    this.ultraChart.Axis.X2.Visible = true;
                }
                else
                {
                    this.ultraChart.Axis.X2.Visible = false;
                }
            }
            catch
            {
            }
        }

        #endregion

        #region EVENTI

        #endregion

        private void ultraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                if (this.ChartMarkers != null)
                {
                    foreach (ChartMarker cm in this.ChartMarkers)
                    {
                        cm.FillSceneGraph(e);
                    }
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
    }
}
