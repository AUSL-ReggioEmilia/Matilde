using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

using UnicodeSrl.ScciCore;
using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUSelezioni : Form, Interfacce.IViewFormPUView
    {
        public frmPUSelezioni()
        {
            InitializeComponent();
        }

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;
        private DialogResult _DialogResult = DialogResult.Cancel;

        private bool _binding = false;

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

            this.SetBindings();
            this.SetBindingsAss();

            switch (_Modality)
            {

                case Enums.EnumModalityPopUp.mpNuovo:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.ubApplica.Enabled = true;
                    this.UltraTabControl.Tabs["Ruoli"].Enabled = false;
                    break;

                case Enums.EnumModalityPopUp.mpModifica:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.uteCodice.Enabled = false;
                    this.ubApplica.Enabled = false;
                    this.UltraTabControl.Tabs["Ruoli"].Enabled = true;
                    break;

                case Enums.EnumModalityPopUp.mpCancella:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    this.UltraTabControl.Tabs["Ruoli"].Enabled = true;
                    break;

                case Enums.EnumModalityPopUp.mpVisualizza:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    this.ubConferma.Enabled = false;
                    this.UltraTabControl.Tabs["Ruoli"].Enabled = true;
                    break;

                default:
                    break;

            }

            this.ResumeLayout();

        }

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

        private void SetBindings()
        {
            _binding = true;
            DataColumn _dcol = null;

            try
            {
                _DataBinds.DataBindings.Clear();

                _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice);
                _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione);
                _DataBinds.DataBindings.Add("Text", "CodTipoSelezione", this.uteCodTipoSelezione);
                _DataBinds.DataBindings.Add("Text", "CodUtenteInserimento", this.uteCodUtenteInserimento);
                _DataBinds.DataBindings.Add("Text", "CodRuoloInserimento", this.uteCodRuoloInserimento);
                _DataBinds.DataBindings.Add("Value", "DataInserimento", this.udteDataInserimento);
                _DataBinds.DataBindings.Add("Text", "CodUtenteUltimaModifica", this.uteCodUtenteUltimaModifica);
                _DataBinds.DataBindings.Add("Text", "CodRuoloUltimaModifica", this.uteCodRuoloUltimaModifica);
                _DataBinds.DataBindings.Add("Value", "DataUltimaModifica", this.udteDataUltimaModifica);
                _DataBinds.DataBindings.Add("Text", "Selezioni", this.xmlSelezioni);
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                if (_dcol.MaxLength > 0) this.uteCodice.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                if (_dcol.MaxLength > 0) this.uteDescrizione.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodTipoSelezione"];
                if (_dcol.MaxLength > 0) this.uteCodTipoSelezione.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodRuoloInserimento"];
                if (_dcol.MaxLength > 0) this.uteCodRuoloInserimento.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodUtenteInserimento"];
                if (_dcol.MaxLength > 0) this.uteCodUtenteInserimento.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodRuoloUltimaModifica"];
                if (_dcol.MaxLength > 0) this.uteCodRuoloUltimaModifica.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodUtenteUltimaModifica"];
                if (_dcol.MaxLength > 0) this.uteCodUtenteUltimaModifica.MaxLength = _dcol.MaxLength;

                _DataBinds.DataBindings.Load();



            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "SetBindings", this.Text);
            }

            _binding = false;
        }
        private void SetBindingsAss()
        {

            string sSql = @"";

            try
            {

                this.ucMultiSelectRuoli.ViewShowAll = true;
                this.ucMultiSelectRuoli.ViewShowFind = true;
                sSql = string.Format("Select Codice, Descrizione As Ruolo From T_Ruoli" + Environment.NewLine +
                                        "Where Codice {0} (Select CodRuolo From T_AssRuoliSelezioni Where CodSelezione = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice.Text);
                this.ucMultiSelectRuoli.ViewDataSetSX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select Codice, Descrizione As Ruolo From T_Ruoli" + Environment.NewLine +
                                        "Where Codice {0} (Select CodRuolo From T_AssRuoliSelezioni Where CodSelezione = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice.Text);
                this.ucMultiSelectRuoli.ViewDataSetDX = DataBase.GetDataSet(sSql);
                this.ucMultiSelectRuoli.ViewInit();
                this.ucMultiSelectRuoli.RefreshData();

                if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("FlagSistema") && (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["FlagSistema"])
                {
                    this.chkFlagSistema.Checked = true;
                }
                else
                {
                    this.chkFlagSistema.Checked = (this.ucMultiSelectRuoli.ViewDataSetDX.Tables[0].Rows.Count > 0);
                }
            }
            catch (Exception)
            {

            }

        }

        private void loadXMLDefault()
        {
            try
            {
                bool bLoad = (!_binding && this.lblCodTipoSelezioneDes.Text != "");
                if (bLoad)
                {

                    switch (this.ViewModality)
                    {
                        case Enums.EnumModalityPopUp.mpNuovo:
                            bLoad = true;
                            break;
                        case Enums.EnumModalityPopUp.mpModifica:
                            if (this.xmlSelezioni.Text.Trim() == "")
                                bLoad = true;
                            else
                                bLoad = (MessageBox.Show(@"Vuoi caricare un XML di esempio?", "Selezioni", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes);

                            break;

                        default:
                            bLoad = false;
                            break;
                    }
                }

                if (bLoad)
                {
                    string sXml = "";
                    if (this.uteCodTipoSelezione.Text.ToUpper() == EnumTipoSelezione.GRAF.ToString().ToUpper())
                    {
                        SelezioniGrafici tmp = new SelezioniGrafici();
                        tmp.RangeDate = "60G";

                        tmp.CodiciTipoPVT.Add("CODPVT1");
                        tmp.CodiciTipoPVT.Add("CODPVT2");

                        tmp.CodiciLAB.Add("CODLAB1");
                        tmp.CodiciLAB.Add("CODLAB2");

                        sXml = tmp.ToXmlString(true);

                        sXml = sXml.Replace(@"xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""", "");
                    }

                    if (sXml.Trim() != "")
                    {
                        sXml = sXml.Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>" + Environment.NewLine, "");
                        this.xmlSelezioni.Text = sXml;
                    }
                }
            }
            catch
            {
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
            if (bRet && this.uteCodTipoSelezione.Text.Trim() == "")
            {
                MessageBox.Show(@"Inserire " + this.lblCodTipoSelezione.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodTipoSelezione.Focus();
                bRet = false;
            }
            if (bRet && this.lblCodTipoSelezioneDes.Text == "")
            {
                MessageBox.Show(@"Inserire " + this.lblCodTipoSelezione.Text + @" esistente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodTipoSelezione.Focus();
                bRet = false;
            }

            if (bRet && !this.xmlSelezioni.XmlValidated)
            {
                MessageBox.Show(@"Parametri XML sintatticamente non corretti !" + Environment.NewLine + this.xmlSelezioni.LastValidateError, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.xmlSelezioni.Focus();
                bRet = false;
            }

            if (bRet && this.ViewModality == Enums.EnumModalityPopUp.mpNuovo && DataBase.FindValue("Codice", "T_Selezioni", "Codice = '" + this.uteCodice.Text + "'", string.Empty) != string.Empty)
            {

                MessageBox.Show(@"Codice inserito già presente in archivio!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodice.Focus();
                bRet = false;
            }

            if (bRet)
            {
                if (!this.chkFlagSistema.Checked && this.ucMultiSelectRuoli.ViewDataSetDX.Tables[0].Rows.Count > 0)
                {
                    MessageBox.Show(@"Il profilo è associato a dei ruoli: verrà impostato come ""Sistema"".", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    bRet = true;
                }
            }

            return bRet;

        }

        private void UpdateBindings()
        {

            try
            {
                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Selezioni"] = this.xmlSelezioni.Text;
                if (this.udteDataUltimaModifica.Value != null)
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["DataUltimaModificaUTC"] = ((DateTime)this.udteDataUltimaModifica.Value).ToUniversalTime();
                else
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["DataUltimaModificaUTC"] = DBNull.Value;

                if (this.udteDataInserimento.Value != null)
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["DataInserimentoUTC"] = ((DateTime)this.udteDataInserimento.Value).ToUniversalTime();
                else
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["DataInserimentoUTC"] = DBNull.Value;


                if (this.chkFlagSistema.Checked)
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["FlagSistema"] = true;
                else
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["FlagSistema"] = (this.ucMultiSelectRuoli.ViewDataSetDX.Tables[0].Rows.Count > 0);

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "UpdateBindings", this.Text);
            }

        }
        private void UpdateBindingsAss()
        {

            string sSql = @"";

            try
            {
                if (this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                {
                    sSql = "Insert Into T_AssRuoliSelezioni (CodSelezione, CodRuolo)" + Environment.NewLine +
                            "Values ('" + this.uteCodice.Text + "', '{0}')";
                    UpdateBindingsAssDataSet(this.ucMultiSelectRuoli.ViewDataSetDX, "Codice", sSql);
                }
                else
                {
                    if (this.ucMultiSelectRuoli.ViewDataSetSX.HasChanges() == true)
                    {
                        sSql = "Delete from T_AssRuoliSelezioni" + Environment.NewLine +
                                "Where CodSelezione = '" + this.uteCodice.Text + "'" + Environment.NewLine +
                                "And CodRuolo = '{0}'";
                        UpdateBindingsAssDataSet(this.ucMultiSelectRuoli.ViewDataSetSX.GetChanges(), "Codice", sSql);
                    }
                    if (this.ucMultiSelectRuoli.ViewDataSetDX.HasChanges() == true)
                    {
                        sSql = "Insert Into T_AssRuoliSelezioni (CodSelezione, CodRuolo)" + Environment.NewLine +
                                "Values ('" + this.uteCodice.Text + "', '{0}')";
                        UpdateBindingsAssDataSet(this.ucMultiSelectRuoli.ViewDataSetDX.GetChanges(), "Codice", sSql);
                    }
                }

            }
            catch (Exception)
            {

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
        private void UpdateBindingsAssDataSet(DataSet oDs, string field1, string field2, string sql)
        {

            try
            {

                foreach (DataRow oRow in oDs.Tables[0].Rows)
                {
                    if (oRow.RowState == DataRowState.Added)
                    {
                        DataBase.ExecuteSql(string.Format(sql, oRow[field1], oRow[field2]));
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

                sSQL = "Delete from T_AssRuoliSelezioni Where CodSelezione = '" + this.uteCodice.Text + "'";
                DataBase.ExecuteSql(sSQL);

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "DeleteBindingsAss", this.Text);
            }

        }

        private void ute_ValueChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void xml_XMLTextChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void chkFlagSistema_CheckedChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void uteCodTipoSelezione_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodTipoSelezione.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_TipoSelezione";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodTipoSelezione.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodTipoSelezione.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }
                f.Dispose();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodTipoSelezione_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodTipoSelezioneDes.Text = DataBase.FindValue("Descrizione", "T_TipoSelezione", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodTipoSelezione.Text)), "");
            this.ubApplica.Enabled = true;
            loadXMLDefault();
            this.ubInfo.Enabled = (this.lblCodTipoSelezioneDes.Text != "");
        }

        private void uteCodRuoloInserimento_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodRuoloInserimento.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Ruoli";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodRuoloInserimento.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodRuoloInserimento.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodRuoloInserimento_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodRuoloInserimentoDes.Text = DataBase.FindValue("Descrizione", "T_Ruoli", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodRuoloInserimento.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodUtenteInserimento_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodUtenteInserimento.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Login";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodUtenteInserimento.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodUtenteInserimento.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodUtenteInserimento_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodUtenteInserimentoDes.Text = DataBase.FindValue("Descrizione", "T_Login", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodUtenteInserimento.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodUtenteUltimaModifica_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodUtenteUltimaModifica.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Login";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodUtenteUltimaModifica.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodUtenteUltimaModifica.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodUtenteUltimaModifica_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodUtenteUltimaModificaDes.Text = DataBase.FindValue("Descrizione", "T_Login", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodUtenteUltimaModifica.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodRuoloUltimaModifica_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodRuoloUltimaModifica.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Ruoli";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodRuoloUltimaModifica.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodRuoloUltimaModifica.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodRuoloUltimaModifica_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodRuoloUltimaModificaDes.Text = DataBase.FindValue("Descrizione", "T_Ruoli", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodRuoloUltimaModifica.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void ucMultiSelect_Change(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void ucMultiSelect_GridSXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("CodAzione") == true) { e.Layout.Bands[0].Columns["CodAzione"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("CodRuolo") == true) { e.Layout.Bands[0].Columns["CodRuolo"].Hidden = true; }

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
                if (e.Layout.Bands[0].Columns.Exists("CodAzione") == true) { e.Layout.Bands[0].Columns["CodAzione"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("CodRuolo") == true) { e.Layout.Bands[0].Columns["CodRuolo"].Hidden = true; }

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
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
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
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
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
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpModifica:
                        if (CheckInput())
                        {
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
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

        private void ubInfo_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.uteCodTipoSelezione.Text.Trim().ToUpper() == EnumTipoSelezione.GRAF.ToString())
                {

                    frmZoom f = new frmZoom();
                    f.ViewText = @"Range Date  <RangeDate>...</RangeDate>";
                    f.ViewIcon = this.ViewIcon;

                    string sSql = "";
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_24H + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_24H)) + @"' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_48H + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_48H)) + @"' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_7G + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_7G)) + @"' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_30G + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_30G)) + @"' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_60G + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_60G)) + @"' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_90G + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_90G)) + @"' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_6M + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_6M)) + @"' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_DOMANI + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_DOMANI)) + @" (dalle ore 00:00 di domani)' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_N24H + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_N24H)) + @" (dalle adesso)' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_N48H + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_N48H)) + @"' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_N7G + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_N7G)) + @"' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_N30G + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_N30G)) + @"' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_N60G + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_N60G)) + @"' As Descrizione " + Environment.NewLine;
                    sSql += @" UNION " + Environment.NewLine;
                    sSql += @" Select '" + ucEasyDateRange.C_RNG_EPI + @"' As Range, '" + DataBase.Ax2(ucEasyDateRange.getRangeDescription(ucEasyDateRange.C_RNG_EPI)) + @"' As Descrizione " + Environment.NewLine;

                    f.ViewSqlStruct.Sql = sSql;
                    f.ViewInit();
                    f.ShowDialog();
                    if (!this.xmlSelezioni.ReadOnly && f.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        if (this.xmlSelezioni.Text.IndexOf(@"<RangeDate>") >= 0)
                        {
                            int iStart = this.xmlSelezioni.Text.IndexOf(@"<RangeDate>");
                            int iEnd = this.xmlSelezioni.Text.IndexOf(@"</RangeDate>");
                            this.xmlSelezioni.Text = this.xmlSelezioni.Text.Substring(0, iStart + @"<RangeDate>".Length) + f.ViewActiveRow.Cells["Range"].Text + this.xmlSelezioni.Text.Substring(iEnd);
                        }
                        else if (this.xmlSelezioni.Text.IndexOf(@"<RangeDate />") >= 0)
                        {
                            this.xmlSelezioni.Text = this.xmlSelezioni.Text.Replace(@"<RangeDate />", @"<RangeDate>" + f.ViewActiveRow.Cells["Range"].Text + @"</RangeDate>");
                        }
                        else
                        {
                            this.xmlSelezioni.Text = this.xmlSelezioni.Text.Insert(0, @"<RangeDate>" + f.ViewActiveRow.Cells["Range"].Text + @"</RangeDate>");
                        }

                    }
                    f.Dispose();

                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "ubInfo_Click", this.Text);
            }
        }

    }
}
