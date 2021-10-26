using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using UDL;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmConfigurazione : Form, Interfacce.IViewFormBase
    {
        public frmConfigurazione()
        {
            InitializeComponent();
        }

        #region Interface

        public Icon ViewIcon
        {
            get
            {
                return this.Icon;
            }
            set
            {
                this.Icon = value;
            }
        }

        public string ViewText
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
                this.UltraTabControl.Tabs["tab1"].Text = value;
            }
        }

        public void ViewInit()
        {

            this.SuspendLayout();

            MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);
            this.PicView.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_CONFIGURAZIONE, Enums.EnumImageSize.isz256));

            this.LoadConfig();

            this.ResumeLayout();
        }

        #endregion

        #region Subroutine

        private void LoadConfig()
        {

            this.uteConnectionString.Text = MyStatics.Configurazione.ConnectionString;

        }

        private void SaveConfig()
        {

            MyStatics.Configurazione.ConnectionString = this.uteConnectionString.Text;

        }

        #endregion

        #region Events

        private void uteConnectionString_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                ConnectionStringDialog fd = new ConnectionStringDialog();
                fd.Provider = "System.Data.SqlClient";
                fd.ConnectionString = this.uteConnectionString.Text;

                if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteConnectionString.Text = fd.ConnectionString;
                }

            }
            catch (Exception)
            {

            }

        }

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {
            this.SaveConfig();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void uteConnectionString_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.Shift && e.KeyCode == Keys.F6)
                {
                    frmCrypt frm = new frmCrypt();
                    frm.ShowDialog();
                    frm.Dispose();
                    frm = null;
                }
            }
            catch
            {
            }
        }

        #endregion

    }
}
