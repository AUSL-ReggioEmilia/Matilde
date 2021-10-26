using Infragistics.UltraChart.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciCore
{
    internal class ChartTooltip
: IRenderLabel
    {


        public string ToString(System.Collections.Hashtable context)
        {


            object data = context["DATA_VALUE"];

            if (data == null)
                return "";

            string res = "";

            if (data is ChartMarker)
            {
                ChartMarker cm = (ChartMarker)data;
                res = cm.ToolTip + Environment.NewLine + cm.DateOnChart.ToString("dd/MM/yyyy HH:mm:ss");
            }
            else
            {
                string dataValueY = context["DATA_VALUE_Y"].ToString();

                if ((dataValueY != null) && (dataValueY != ""))
                {
                    double dv = 0;
                    double.TryParse(dataValueY, out dv);

                    dataValueY = dv.ToString("0.00#");
                }

                string dataValueX = context["DATA_VALUE_X"].ToString();

                if ((dataValueX != null) && (dataValueX != ""))
                {
                    DateTime dt;
                    DateTime.TryParse(dataValueX, out dt);

                    dataValueX = dt.ToString("dd/MM/yyyy HH:mm:ss");
                }

                res = context["SERIES_LABEL"].ToString() + " : " + dataValueY +
                        Environment.NewLine + dataValueX;

            }

            return res;

        }

    }
}
