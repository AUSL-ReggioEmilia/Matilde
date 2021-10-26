using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class ucAnteprimaRtf : UserControl, Interfacce.IViewUserControlBase
    {
        public ucAnteprimaRtf()
        {
            InitializeComponent();
        }

        #region declare

        private MovScheda _movscheda = null;

        #endregion

        #region Interface

        public void ViewInit()
        {
            this.RefreshRTF();
        }

        #endregion

        #region properties

        public MovScheda MovScheda
        {
            get { return _movscheda; }
            set { _movscheda = value; }
        }

        #endregion

        #region public methods

        public void RefreshRTF()
        {
            if (this.MovScheda != null)
                this.rtbRichTextBox.Rtf = this.MovScheda.AnteprimaRTF;
        }

        #endregion

    }
}
