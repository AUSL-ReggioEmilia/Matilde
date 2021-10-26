using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUScreentile : Form, Interfacce.IViewFormPUView
    {
        public frmPUScreentile()
        {
            InitializeComponent();
        }

        #region Declare

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;
        private DialogResult _DialogResult = DialogResult.Cancel;

        #endregion

        #region Interface

        public PUDataBindings ViewDataBindings
        {
            get
            {
                return _DataBinds;
            }
            set
            {
                _DataBinds = value;
            }
        }

        public Enums.EnumDataNames ViewDataNamePU
        {
            get
            {
                return _ViewDataNamePU;
            }
            set
            {
                _ViewDataNamePU = value;
            }
        }

        public Image ViewImage
        {
            get
            {
                return this.PicImage.Image;
            }
            set
            {
                this.PicImage.Image = value;
            }
        }

        public Enums.EnumModalityPopUp ViewModality
        {
            get
            {
                return _Modality;
            }
            set
            {
                _Modality = value;
            }
        }

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
                return (string)this.Tag;
            }
            set
            {
                this.Tag = value;
                this.Text = string.Format("{0} - {1}", MyStatics.GetDataNameModalityFormPU(_Modality), value);
            }
        }

        internal Scci.Model.en_TipoScreen TipoScreen { get; set; }

        public void ViewInit()
        {

            this.SuspendLayout();

            this.SetBindings();

            switch (_Modality)
            {

                case Enums.EnumModalityPopUp.mpNuovo:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.uteCodScreen.Enabled = false;
                    this.ubApplica.Enabled = true;
                    this.SetCollassata();
                    break;

                case Enums.EnumModalityPopUp.mpModifica:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.uteCodScreen.Enabled = false;
                    this.ubApplica.Enabled = false;
                    this.SetCollassata();
                    break;

                case Enums.EnumModalityPopUp.mpCancella:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    break;

                case Enums.EnumModalityPopUp.mpVisualizza:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    this.ubConferma.Enabled = false;
                    break;

                default:
                    break;

            }

            switch (TipoScreen)
            {
                case Scci.Model.en_TipoScreen.EPIGRID:
                    this.chkFissa.Visible = true;
                    this.lblFissa.Visible = true;
                    this.chkNonCollassabile.Visible = true;
                    this.lblNonCollassabile.Visible = true;
                    this.chkCollassata.Visible = true;
                    this.lblCollassata.Visible = true;
                    break;
                default:
                    this.chkFissa.Visible = false;
                    this.lblFissa.Visible = false;
                    this.chkNonCollassabile.Visible = false;
                    this.lblNonCollassabile.Visible = false;
                    this.chkCollassata.Visible = false;
                    this.lblCollassata.Visible = false;
                    break;
            }

            this.ResumeLayout();

        }

        #endregion

        #region Subroutine

        private void SetBindings()
        {

            DataColumn _dcol = null;

            try
            {
                _DataBinds.DataBindings.Clear();

                _DataBinds.DataBindings.Add("Text", "CodScreen", this.uteCodScreen);
                _DataBinds.DataBindings.Add("Value", "Riga", this.umeRiga);
                _DataBinds.DataBindings.Add("Value", "Colonna", this.umeColonna);
                _DataBinds.DataBindings.Add("Value", "Altezza", this.umeAltezza);
                _DataBinds.DataBindings.Add("Value", "Larghezza", this.umeLarghezza);
                _DataBinds.DataBindings.Add("Checked", "InEvidenza", this.chkInEvidenza);
                _DataBinds.DataBindings.Add("Text", "CodPlugin", this.uteCodicePlugin);
                _DataBinds.DataBindings.Add("Text", "NomeTile", this.uteNomeTile);
                _DataBinds.DataBindings.Add("Text", "Attributi", this.xmlAttributi);
                _DataBinds.DataBindings.Add("Checked", "Fissa", this.chkFissa);
                _DataBinds.DataBindings.Add("Checked", "NonCollassabile", this.chkNonCollassabile);
                _DataBinds.DataBindings.Add("Checked", "Collassata", this.chkCollassata);

                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodScreen"];
                if (_dcol.MaxLength > 0) this.uteCodScreen.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodPlugin"];
                if (_dcol.MaxLength > 0) this.uteCodicePlugin.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["NomeTile"];
                if (_dcol.MaxLength > 0) this.uteNomeTile.MaxLength = _dcol.MaxLength;

                _DataBinds.DataBindings.Load();
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "SetBindings", this.Text);
            }

        }

        private bool CheckInput()
        {

            bool bRet = true;
            int testint = 0;

            if (bRet && (this.uteNomeTile.Text.Trim() == ""))
            {
                MessageBox.Show(@"Inserire " + this.lblNomeTile.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteNomeTile.Focus();
                bRet = false;
            }

            if (bRet)
            {
                if (this.umeRiga.Text.Trim() == "" || !int.TryParse(this.umeRiga.Text, out testint))
                {
                    MessageBox.Show(@"Inserire un valore numerico in " + this.lblRiga.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.umeRiga.Focus();
                    bRet = false;
                }
            }

            if (bRet)
            {
                if (this.umeColonna.Text.Trim() == "" || !int.TryParse(this.umeColonna.Text, out testint))
                {
                    MessageBox.Show(@"Inserire un valore numerico in " + this.lblColonna.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.umeColonna.Focus();
                    bRet = false;
                }
            }

            if (bRet)
            {
                if (this.umeAltezza.Text.Trim() == "" || !int.TryParse(this.umeAltezza.Text, out testint))
                {
                    MessageBox.Show(@"Inserire un valore numerico in " + this.lblAltezza.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.umeAltezza.Focus();
                    bRet = false;
                }

            }

            if (bRet)
            {
                if (this.umeLarghezza.Text.Trim() == "" || !int.TryParse(this.umeLarghezza.Text, out testint))
                {
                    MessageBox.Show(@"Inserire un valore numerico in " + this.lblLarghezza.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.umeLarghezza.Focus();
                    bRet = false;
                }
                else if (int.Parse(this.umeLarghezza.Text) <= 0)
                {
                    MessageBox.Show(@"Inserire un valore maggiore di zero in " + this.lblLarghezza.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.umeLarghezza.Focus();
                    bRet = false;
                }
            }

            if (bRet && (this.uteCodicePlugin.Text.Trim() == "" || this.lblCodicePluginDes.Text.Trim() == ""))
            {
                MessageBox.Show(@"Inserire " + this.lblCodicePlugin.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodicePlugin.Focus();
                bRet = false;
            }

            return bRet;

        }

        private string GetLastID()
        {
            return DataBase.FindValue("MAX(ID)", "T_ScreenTile", "", "");
        }

        private void SetCollassata()
        {

            if (this.chkNonCollassabile.Checked == true)
            {
                this.chkCollassata.Checked = false;
                this.chkCollassata.Enabled = false;
                this.lblCollassata.Enabled = false;
            }
            else
            {
                this.chkCollassata.Enabled = true;
                this.lblCollassata.Enabled = true;
            }

        }

        #endregion

        #region Events

        private void ute_ValueChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void uteCodScreen_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodScreenDes.Text = DataBase.FindValue("Descrizione", "T_Screen", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodScreen.Text)), "");
        }

        private void uteCodicePlugin_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodicePluginDes.Text = DataBase.FindValue("Descrizione", "T_CDSSPlugins", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodicePlugin.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodicePlugin_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodicePlugin.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_CDSSPlugins";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodicePlugin.Text) + "' AND CodTipoCDSS = 'SCR'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    this.uteCodicePlugin.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodicePlugin_EditorButtonClick", this.Text);
            }
        }

        private void chkNonCollassabile_CheckedChanged(object sender, EventArgs e)
        {
            this.SetCollassata();
        }

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = _DialogResult;
            this.Close();
        }

        private void ubApplica_Click(object sender, EventArgs e)
        {

            try
            {

                switch (this.ViewModality)
                {

                    case Enums.EnumModalityPopUp.mpNuovo:
                        if (CheckInput())
                        {
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.ViewDataBindings.SqlSelect.Where = "ID = '" + GetLastID() + "'";
                            this.ViewDataBindings.DataBindings.DataSet = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            this.ViewModality = Enums.EnumModalityPopUp.mpModifica;
                            this.ViewText = this.ViewText;
                            _DialogResult = DialogResult.OK;
                            this.ViewInit();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpModifica:
                        if (CheckInput())
                        {
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            _DialogResult = DialogResult.OK;
                            this.ViewInit();
                        }
                        break;

                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "ubApplica_Click", this.Text);
            }

        }

        private void ubConferma_Click(object sender, EventArgs e)
        {

            try
            {
                switch (this.ViewModality)
                {

                    case Enums.EnumModalityPopUp.mpNuovo:
                    case Enums.EnumModalityPopUp.mpModifica:
                        if (CheckInput())
                        {
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpCancella:
                        if (MessageBox.Show("Confermi la cancellazione ?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                        {
                            DataBase.ExecuteSql(this.ViewDataBindings.SqlDelete.Sql);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpVisualizza:
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                        break;

                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "ubConferma_Click", this.Text);
            }
        }

        #endregion

    }
}
