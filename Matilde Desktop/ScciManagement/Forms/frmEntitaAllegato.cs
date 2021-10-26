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
    public partial class frmEntitaAllegato : Form, Interfacce.IViewFormPUView
    {
        public frmEntitaAllegato()
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

        public void ViewInit()
        {

            this.SuspendLayout();

            this.InitializeUltraToolbarsManager();

            this.InitializePictureSelect();

            this.SetBindings();

            switch (_Modality)
            {

                case Enums.EnumModalityPopUp.mpNuovo:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.ubApplica.Enabled = true;
                    break;

                case Enums.EnumModalityPopUp.mpModifica:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.uteCodice.Enabled = false;
                    this.ubApplica.Enabled = false;
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

            this.ResumeLayout();

        }

        #endregion

        #region Subroutine

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);

            }
            catch (Exception)
            {

            }

        }

        private void InitializePictureSelect()
        {
            try
            {
                this.ucPictureSelectIcona.ViewShowSaveImage = true;
                this.ucPictureSelectIcona.ViewCheckSquareImage = false;
                this.ucPictureSelectIcona.ViewInit();
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "InitializePictureSelect", this.Name);
            }
        }

        private void SetBindings()
        {

            DataColumn _dcol = null;

            try
            {
                _DataBinds.DataBindings.Clear();

                _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice);
                _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione);
                _DataBinds.DataBindings.Add("Value", "Colore", this.ucpColore);

                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                if (_dcol.MaxLength > 0) this.uteCodice.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                if (_dcol.MaxLength > 0) this.uteDescrizione.MaxLength = _dcol.MaxLength;

                _DataBinds.DataBindings.Load();

                this.ucPictureSelectIcona.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                this.ucpColore.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "SetBindings", this.Text);
            }

        }

        private void UpdateBindings()
        {

            try
            {
                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona.ViewImage);
                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore.Value.ToString();
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "UpdateBindings", this.Text);
            }

        }

        private bool CheckInput()
        {

            bool bRet = true;

            if (bRet && this.uteCodice.Text.Trim() == "")
            {
                MessageBox.Show(@"Inserire " + this.lblCodice.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodice.Focus();
                bRet = false;
            }
            if (bRet && this.uteDescrizione.Text.Trim() == "")
            {
                MessageBox.Show(@"Inserire " + this.lblDescrizione.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteDescrizione.Focus();
                bRet = false;
            }

            if (bRet && this.ViewModality == Enums.EnumModalityPopUp.mpNuovo && DataBase.FindValue("Codice", "T_EntitaAllegato", "Codice = '" + this.uteCodice.Text + "'", string.Empty) != string.Empty)
            {

                MessageBox.Show(@"Codice inserito già presente in archivio!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodice.Focus();
                bRet = false;
            }

            return bRet;

        }

        #endregion

        #region Events

        private void ute_ValueChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void ucpColore_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.ucpColore.Value = this.colorDialog.Color;
            }
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
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice.Text + "'";
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
                            this.UpdateBindings();
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
                        if (CheckInput())
                        {
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpModifica:
                        if (CheckInput())
                        {
                            this.UpdateBindings();
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
