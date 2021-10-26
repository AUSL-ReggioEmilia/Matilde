using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Framework.Threading;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore
{
    public interface ITileUserCtl :
            ICustomWorker
    {
        AppDataMarshaler AppDataMarshaler { get; set; }

        T_ScreenTileRow ScreenTileRow { get; set; }

        SelCdssRuoloRow SelCdssRuoloRow { get; set; }

        bool Highlighted { get; set; }

        bool DrawBorder { get; set; }

        int ParentTableRow { get; }

        int ParentTableColumn { get; }

        Control Control { get; }

        void LoadData(object state);

        void DisplayUI(object state);

        void RefreshData();

        void DisplayUiLoading();
        void DisplayUiWait();
        void DisplayUiNormal();



        void SetCoreStatics();
        void ResetCoreStatics();
    }
}
