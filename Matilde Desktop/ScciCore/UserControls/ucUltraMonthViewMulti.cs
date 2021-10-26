using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinSchedule;

namespace UnicodeSrl.ScciCore
{
    public partial class ucUltraMonthViewMulti : UserControl
    {
        public ucUltraMonthViewMulti()
        {
            InitializeComponent();
        }

        #region Declare

        public event EventHandler ButtonSeleziona;
        public event BeforeMonthScrollEventHandler BeforeMonthScroll;
        public event EventHandler VisibleMonthsChanged;

        #endregion

        #region Events

        private void UltraMonthViewMulti_BeforeMonthScroll(object sender, BeforeMonthScrollEventArgs e)
        {
            if (BeforeMonthScroll != null) { BeforeMonthScroll(sender, e); }
        }

        private void UltraMonthViewMulti_VisibleMonthsChanged(object sender, EventArgs e)
        {
            if (VisibleMonthsChanged != null) { VisibleMonthsChanged(sender, e); }
        }

        private void ubSeleziona_Click(object sender, EventArgs e)
        {
            if (ButtonSeleziona != null) { ButtonSeleziona(sender, e); }
        }

        #endregion

        #region Public Events

        public void RefreshData()
        {

            try
            {

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

    }
}
