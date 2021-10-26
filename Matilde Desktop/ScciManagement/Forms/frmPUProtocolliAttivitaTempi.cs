using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUProtocolliAttivitaTempi : Form, Interfacce.IViewFormPUView
    {
        public frmPUProtocolliAttivitaTempi()
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

            this.SetBindings();
            this.SetBindingsAss();

            switch (_Modality)
            {

                case Enums.EnumModalityPopUp.mpNuovo:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.ubApplica.Enabled = true;
                    this.UltraTabControl.Tabs["tab2"].Enabled = false;
                    break;

                case Enums.EnumModalityPopUp.mpModifica:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.uteCodice.Enabled = false;
                    this.ubApplica.Enabled = false;
                    this.UltraTabControl.Tabs["tab2"].Enabled = true;
                    break;

                case Enums.EnumModalityPopUp.mpCancella:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    this.UltraTabControl.Tabs["tab2"].Enabled = true;
                    break;

                case Enums.EnumModalityPopUp.mpVisualizza:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    this.ubConferma.Enabled = false;
                    this.UltraTabControl.Tabs["tab2"].Enabled = true;
                    break;

                default:
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

                _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice);
                _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione);
                _DataBinds.DataBindings.Add("Value", "DeltaGiorni", this.umeDeltaGiorni);
                _DataBinds.DataBindings.Add("Value", "DeltaOre", this.umeDeltaOre);
                _DataBinds.DataBindings.Add("Value", "DeltaMinuti", this.umeDeltaMinuti);
                _DataBinds.DataBindings.Add("Checked", "DeltaAlle00", this.chkDeltaAlle00);

                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                if (_dcol.MaxLength > 0) this.uteCodice.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                if (_dcol.MaxLength > 0) this.uteDescrizione.MaxLength = _dcol.MaxLength;

                _DataBinds.DataBindings.Load();

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "SetBindings", this.Text);
            }

        }

        private void SetBindingsAss()
        {

            string sSql = @"";

            try
            {
                this.ucMultiSelectTipoTask.ViewShowAll = true;
                this.ucMultiSelectTipoTask.ViewShowFind = true;
                sSql = string.Format("Select Codice, Descrizione As [Tipo Task] From T_TipoTaskInfermieristico" + Environment.NewLine +
                                        "Where Codice <> '" + Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoSchedaTaskDaPrescrizione) + "'" +
                                        " AND Codice {0} (Select CodTipoTaskInfermieristico From T_ProtocolliAttivitaTempiTipoTask Where CodProtocolloAttivitaTempi = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice.Text);
                this.ucMultiSelectTipoTask.ViewDataSetSX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select Codice, Descrizione As [Tipo Task] From T_TipoTaskInfermieristico" + Environment.NewLine +
                                        "Where Codice <> '" + Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoSchedaTaskDaPrescrizione) + "'" +
                                        " AND Codice {0} (Select CodTipoTaskInfermieristico From T_ProtocolliAttivitaTempiTipoTask Where CodProtocolloAttivitaTempi = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice.Text);
                this.ucMultiSelectTipoTask.ViewDataSetDX = DataBase.GetDataSet(sSql);
                this.ucMultiSelectTipoTask.ViewInit();
                this.ucMultiSelectTipoTask.RefreshData();
            }
            catch
            {
            }
        }

        private void UpdateBindingsAss()
        {

            string sSql = @"";

            try
            {
                if (this.ucMultiSelectTipoTask.ViewDataSetSX.HasChanges() == true)
                {

                    sSql = "Delete from T_ProtocolliAttivitaTempiTipoTask" + Environment.NewLine + "Where CodProtocolloAttivitaTempi = '"
                        + this.uteCodice.Text + "' AND CodTipoTaskInfermieristico = '{0}'";
                    UpdateBindingsAssDataSet(this.ucMultiSelectTipoTask.ViewDataSetSX.GetChanges(), "Codice", sSql);
                }
                if (this.ucMultiSelectTipoTask.ViewDataSetDX.HasChanges() == true)
                {
                    sSql = "Insert Into T_ProtocolliAttivitaTempiTipoTask (CodProtocolloAttivitaTempi, CodTipoTaskInfermieristico, Ordine)" + Environment.NewLine +
                            "Values ('" + this.uteCodice.Text + "', '{0}', 0)";
                    UpdateBindingsAssDataSet(this.ucMultiSelectTipoTask.ViewDataSetDX.GetChanges(), "Codice", sSql);
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "UpdateBindingsAss", this.Text);
            }

        }

        private void UpdateBindingsAssDataSet(DataSet oDs, string field, string sql)
        {

            try
            {

                foreach (DataRow oRow in oDs.Tables[0].Rows)
                {
                    if (oRow.RowState == DataRowState.Added)
                    {
                        DataBase.ExecuteSql(string.Format(sql, oRow[field]));
                    }
                }

            }
            catch (Exception)
            {

            }

        }

        private void DeleteBindingsAss()
        {

            string sSQL = string.Empty;

            try
            {
                sSQL = @"DELETE FROM T_ProtocolliAttivitaTempiTipoTask WHERE CodProtocolloAttivitaTempi = '" + this.uteCodice.Text + "'";

                DataBase.ExecuteSql(sSQL);
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "DeleteBindingsAss", this.Text);
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

            if (bRet && this.ViewModality == Enums.EnumModalityPopUp.mpNuovo && DataBase.FindValue("Codice", "T_ProtocolliAttivitaTempi", "Codice = '" + this.uteCodice.Text + "'", string.Empty) != string.Empty)
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

        private void ucMultiSelect_GridSXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelect_GridSXInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells[e.Row.Cells.Count - 1].Hidden == false)
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SYMBOL_RESTRICTED, Enums.EnumImageSize.isz16));
                }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelect_GridDXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelect_GridDXInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells[e.Row.Cells.Count - 1].Hidden == false)
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SYMBOL_CHECK, Enums.EnumImageSize.isz16));
                }

            }
            catch (Exception)
            {

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
                            this.UpdateBindingsAss();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice.Text + "'";
                            this.ViewDataBindings.DataBindings.DataSet = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            this.ViewModality = Enums.EnumModalityPopUp.mpModifica;
                            this.ViewText = this.ViewText;
                            _DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.ViewInit();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpModifica:
                        if (CheckInput())
                        {
                            this.UpdateBindingsAss();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            _DialogResult = System.Windows.Forms.DialogResult.OK;
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
                            this.UpdateBindingsAss();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpModifica:
                        if (CheckInput())
                        {
                            this.UpdateBindingsAss();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpCancella:
                        if (MessageBox.Show("Confermi la cancellazione ?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                        {
                            this.DeleteBindingsAss();
                            DataBase.ExecuteSql(this.ViewDataBindings.SqlDelete.Sql);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpVisualizza:
                        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
