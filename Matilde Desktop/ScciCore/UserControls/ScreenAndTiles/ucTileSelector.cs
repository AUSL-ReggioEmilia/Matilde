using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore.UserControls.ScreenAndTiles
{
    public partial class ucTileSelector : UserControl
    {
        private bool m_checked = false;

        public ucTileSelector()
        {
            InitializeComponent();
        }


        #region eventi key + mouse


        #endregion eventi key + mouse

        public bool Checked
        {
            get
            {
                return m_checked;
            }
            set
            {
                bool bChanged = (m_checked != value);
                m_checked = value;

                if (bChanged) OnChanged_Checked();
            }

        }


        public string Key { get; set; }

        public SelectTileRowDelegate SelectTileRowCB { get; set; }

        private void OnChanged_Checked()
        {
            if (this.Checked)
                this.pb.Image = global::UnicodeSrl.ScciCore.Properties.Resources.GridChecked;
            else
                this.pb.Image = null;
        }


        private void pb_Click(object sender, EventArgs e)
        {
            if ((this.SelectTileRowCB != null) && (this.Checked == false))
            {
                int row = Convert.ToInt32(this.Tag);
                this.SelectTileRowCB(row);
            }
        }
    }
}
