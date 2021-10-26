using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore
{
    public partial class ucEasyPopUpData : UserControl
    {

        #region DECLARE

        public event EventHandler ConfermaClick;
        public event EventHandler AnnullaClick;

        #endregion

        public ucEasyPopUpData()
        {
            InitializeComponent();
        }

        #region PROPERTIES

        public DateTime DataOra
        {
            get
            {
                DateTime dtret = DateTime.MinValue;
                DateTime.TryParse(this.ucEasyCalendarCombo.Value.ToString(), out dtret);
                return dtret;
            }
            set
            {
                this.ucEasyCalendarCombo.Value = value;
            }
        }

        public easyStatics.easyRelativeDimensions TestoEtichettaDimensioneFont
        {
            get
            {
                return this.ucEasyLabel.TextFontRelativeDimension;
            }
            set
            {
                this.ucEasyLabel.TextFontRelativeDimension = value;
            }
        }

        public easyStatics.easyRelativeDimensions DataOraDimensioneFont
        {
            get
            {
                return this.ucEasyCalendarCombo.TextFontRelativeDimension;
            }
            set
            {
                this.ucEasyCalendarCombo.TextFontRelativeDimension = value;
            }
        }

        #endregion

        #region EVENTI

        private void ucEasyButtonConferma_Click(object sender, EventArgs e)
        {
            if (ConfermaClick != null) { ConfermaClick(sender, e); }

        }

        private void ucEasyButtonCancel_Click(object sender, EventArgs e)
        {
            if (AnnullaClick != null) { AnnullaClick(sender, e); }
        }

        #endregion
    }
}
