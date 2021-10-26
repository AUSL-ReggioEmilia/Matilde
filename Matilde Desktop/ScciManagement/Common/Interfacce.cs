using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace UnicodeSrl.ScciManagement
{
    public static class Interfacce
    {

        public interface IViewFormBase
        {

            Icon ViewIcon { get; set; }
            string ViewText { get; set; }
            void ViewInit();

        }

        public interface IViewFormView : IViewFormBase
        {

            Enums.EnumDataNames ViewDataName { get; set; }
            Enums.EnumDataNames ViewDataNamePU { get; set; }
            Image ViewImage { get; set; }

        }

        public interface IViewFormPUView : IViewFormBase
        {

            PUDataBindings ViewDataBindings { get; set; }
            Enums.EnumDataNames ViewDataNamePU { get; set; }
            Image ViewImage { get; set; }
            Enums.EnumModalityPopUp ViewModality { get; set; }

        }

        public interface IViewFormZoom : IViewFormBase
        {

            UnicodeSrl.Sys.Data2008.SqlStruct ViewSqlStruct { get; set; }
            Infragistics.Win.UltraWinGrid.UltraGridRow ViewActiveRow { get; }

        }

        public interface IViewFormZoomBase : IViewFormBase
        {

            string ViewParametro { get; set; }

        }




    }
}
