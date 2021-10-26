using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciCore
{
    internal class ChartMarker

    {
        #region Costruttori

        public ChartMarker()
        {
            this.Background = Color.Red;
            this.BorderColor = Color.Black;
            this.MarkerType = en_ChartMarkerType.line;
            this.Width = 2;
        }

        public ChartMarker(DateTime dateOnChart, string toolTip
   , en_ChartMarkerType markerType = en_ChartMarkerType.line
   , int width = 5
  ) : this()
        {
            this.DateOnChart = dateOnChart;
            this.ToolTip = toolTip;
            this.MarkerType = en_ChartMarkerType.line;
            this.Width = width;
        }

        #endregion

        public en_ChartMarkerType MarkerType { get; set; }

        public DateTime DateOnChart { get; set; }

        public int Width { get; set; }

        public String ToolTip { get; set; }

        public Color Background { get; set; }

        public Color BorderColor { get; set; }

        public string Tag { get; set; }

        internal void FillSceneGraph(FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = e.Grid["X"] as IAdvanceAxis;
            IAdvanceAxis yAxis = e.Grid["Y"] as IAdvanceAxis;

            if ((xAxis == null) || (yAxis == null))
                return;

            PrimitiveShape shape = null;

            switch (this.MarkerType)
            {
                case en_ChartMarkerType.circle:
                    shape = renderCircle(e);
                    break;

                case en_ChartMarkerType.line:
                    shape = renderLine(e);
                    break;
                default:
                    break;
            }


            shape.PE.Fill = this.Background;
            shape.PE.StrokeWidth = 1;
            shape.PE.Stroke = this.BorderColor;

            shape.Row = shape.Column = 0;
            shape.Caps = PCaps.HitTest | PCaps.Tooltip;
            shape.Layer = e.ChartCore.GetChartLayer();
            shape.Chart = e.ChartCore.ChartType;

            shape.Value = this;

            e.SceneGraph.Add(shape);
        }



        #region     rendering procs

        private PrimitiveShape renderLine(FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = e.Grid["X"] as IAdvanceAxis;
            IAdvanceAxis yAxis = e.Grid["Y"] as IAdvanceAxis;

            Rectangle chartBounds = e.ChartCore.GridLayerBounds;

            int x = (int)xAxis.Map(this.DateOnChart);

            Box line = new Box(new Rectangle(x, chartBounds.Top, this.Width, chartBounds.Height),
                new Infragistics.UltraChart.Shared.Styles.LineStyle(LineCapStyle.NoAnchor, LineCapStyle.NoAnchor, LineDrawStyle.Solid));

            return line;
        }

        private PrimitiveShape renderCircle(FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = e.Grid["X"] as IAdvanceAxis;
            IAdvanceAxis yAxis = e.Grid["Y"] as IAdvanceAxis;

            Rectangle chartBounds = e.ChartCore.GridLayerBounds;

            int x = (int)xAxis.Map(this.DateOnChart);

            Ellipse circle = new Ellipse(new Point(x, (chartBounds.Bottom - chartBounds.Top) / 2), this.Width);

            return circle;
        }


        #endregion  rendering procs



    }
}
