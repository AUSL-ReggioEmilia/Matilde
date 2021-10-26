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
using Infragistics.Win.UltraWinTree;
using UnicodeSrl.Scci.Enums;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUTipoConsegna : Form, Interfacce.IViewFormPUView
    {
        public frmPUTipoConsegna()
        {
            InitializeComponent();
        }

        #region Declare

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;
        private DialogResult _DialogResult = DialogResult.Cancel;

        private Dictionary<string, DataSet> _RuoloEntitaAzione_Voci = new Dictionary<string, DataSet>();
        private Dictionary<string, DataTable> _TestiPredefiniti_Campi = new Dictionary<string, DataTable>();

        private bool _runTime = false;

        string _CodiceOriginale = string.Empty;

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

            MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);

            this.InitializePictureSelect();

            _RuoloEntitaAzione_Voci = new Dictionary<string, DataSet>();
            _TestiPredefiniti_Campi = new Dictionary<string, DataTable>();

            if (this.ViewDataNamePU == Enums.EnumDataNames.T_TipoConsegna)
            {
                this.ucMultiSelectUA6.Tag = "CSG";
            }
            else if (this.ViewDataNamePU == Enums.EnumDataNames.T_TipoConsegnaPaziente)
            {
                this.ucMultiSelectUA6.Tag = "CSP";
            }

            SetBindings();
            SetBindingsAss();

            switch (_Modality)
            {

                case Enums.EnumModalityPopUp.mpNuovo:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.ubApplica.Enabled = true;
                    break;

                case Enums.EnumModalityPopUp.mpModifica:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.uteCodice6.Enabled = false;
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

                case Enums.EnumModalityPopUp.mpCopia:
                    this.EditBindingsCopia();
                    this.ubApplica.Enabled = true;
                    break;

                default:
                    break;

            }

            this.ResumeLayout();

        }

        #endregion

        #region Subroutine

        private void InitializePictureSelect()
        {
            try
            {
                this.ucPictureSelectIcona6.ViewShowSaveImage = false;
                this.ucPictureSelectIcona6.ViewCheckSquareImage = true;
                this.ucPictureSelectIcona6.ViewInit();

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

                this.UltraTabControl.Tabs["tab6"].Visible = true;
                this.UltraTabControl.Tabs["tab6"].Text = this.ViewText;

                _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice6);
                _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione6);
                _DataBinds.DataBindings.Add("Text", "CodScheda", this.uteCodScheda6);

                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                if (_dcol.MaxLength > 0) this.uteCodice6.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                if (_dcol.MaxLength > 0) this.uteDescrizione6.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodScheda"];
                if (_dcol.MaxLength > 0) this.uteCodScheda6.MaxLength = _dcol.MaxLength;


                _DataBinds.DataBindings.Load();

                this.ucPictureSelectIcona6.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                this.ucpColore6.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());


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


                this.ucMultiSelectUA6.ViewShowAll = true;
                this.ucMultiSelectUA6.ViewShowFind = true;
                sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                        "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice6.Text, this.ucMultiSelectUA6.Tag.ToString());
                this.ucMultiSelectUA6.ViewDataSetSX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                        "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice6.Text, this.ucMultiSelectUA6.Tag.ToString());
                this.ucMultiSelectUA6.ViewDataSetDX = DataBase.GetDataSet(sSql);
                this.ucMultiSelectUA6.ViewInit();
                this.ucMultiSelectUA6.RefreshData();
                this.ucMultiSelectPlusRuoli6.ViewShowAll = true;

                this.ucMultiSelectPlusRuoli6.ViewShowFind = true; this.ucMultiSelectPlusRuoli6.GridDXFilterColumnIndex = 2;
                this.ucMultiSelectPlusRuoli6.GridSXFilterColumnIndex = 2;

                sSql = string.Format("Select QAE.CodAzione, R.Codice AS CodRuolo, R.Descrizione As Ruolo" + Environment.NewLine +
                                        "From (Select AE.CodEntita, AE.CodAzione" + Environment.NewLine +
                                                "From T_AzioniEntita AE" + Environment.NewLine +
                                                        "Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                                "Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                                        "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                        "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                ") As QAE" + Environment.NewLine +
                                            "Cross Join T_Ruoli R" + Environment.NewLine +
                                            "Left Join (Select ASS.CodEntita, ASS.CodAzione, ASS.CodRuolo" + Environment.NewLine +
                                                        "From T_AssRuoliAzioni ASS" + Environment.NewLine +
                                                                "Inner Join T_Azioni A On ASS.CodAzione = A.Codice" + Environment.NewLine +
                                                                "Inner Join T_Entita E On ASS.CodEntita = E.Codice" + Environment.NewLine +
                                                                "Inner Join T_AzioniEntita AE On (ASS.CodEntita = AE.CodEntita" + Environment.NewLine +
                                                                                                    "And ASS.CodAzione = AE.CodAzione)" + Environment.NewLine +
                                                        "Where ASS.CodEntita = '{0}'" + Environment.NewLine +
                                                                "And ASS.CodVoce = '{1}'" + Environment.NewLine +
                                                                "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                                "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                        ") As QASS On QAE.CodEntita=QASS.CodEntita" + Environment.NewLine +
                                                                        "And QAE.CodAzione=QASS.CodAzione" + Environment.NewLine +
                                                                        "And R.Codice=QASS.CodRuolo" + Environment.NewLine +
                                        "Where QASS.CodEntita Is Null" + Environment.NewLine +
                                        "Order By QAE.CodAzione, R.Descrizione ASC", this.ucMultiSelectUA6.Tag.ToString(), this.uteCodice6.Text);
                this.ucMultiSelectPlusRuoli6.ViewDataSetSX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select ASS.CodAzione, ASS.CodRuolo, R.Descrizione As Ruolo" + Environment.NewLine +
                                        "From T_AssRuoliAzioni ASS" + Environment.NewLine +
                                                "Inner Join T_Ruoli R On ASS.CodRuolo = R.Codice" + Environment.NewLine +
                                                "Inner Join T_Azioni A On ASS.CodAzione = A.Codice" + Environment.NewLine +
                                                "Inner Join T_Entita E On ASS.CodEntita = E.Codice" + Environment.NewLine +
                                                "Inner Join T_AzioniEntita AE On ASS.CodEntita = AE.CodEntita And ASS.CodAzione = AE.CodAzione" + Environment.NewLine +
                                        "Where ASS.CodEntita = '{0}'" + Environment.NewLine +
                                                "And ASS.CodVoce = '{1}'" + Environment.NewLine +
                                                "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                        "Order By ASS.CodAzione, R.Descrizione ASC", this.ucMultiSelectUA6.Tag.ToString(), this.uteCodice6.Text);
                this.ucMultiSelectPlusRuoli6.ViewDataSetDX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                        "From T_AzioniEntita AE" + Environment.NewLine +
                                                "Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                                "Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                        "Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                                "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", this.ucMultiSelectUA6.Tag.ToString());
                this.ucMultiSelectPlusRuoli6.ViewDataSetMaster = DataBase.GetDataSet(sSql);
                this.ucMultiSelectPlusRuoli6.ViewInit();
                this.ucMultiSelectPlusRuoli6.RefreshData();


            }
            catch (Exception)
            {

            }

        }

        private void UpdateBindings()
        {

            try
            {
                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona6.ViewImage);
                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore6.Value.ToString();

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
                if (this.ucMultiSelectUA6.ViewDataSetSX.HasChanges() == true)
                {
                    sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                            "Where CodVoce = '" + this.uteCodice6.Text + "'" + Environment.NewLine +
                            "And CodEntita = '" + this.ucMultiSelectUA6.Tag.ToString() + "'" + Environment.NewLine +
                            "And CodUA = '{0}'";
                    UpdateBindingsAssDataSet(this.ucMultiSelectUA6.ViewDataSetSX.GetChanges(), "Codice", sSql);
                }
                if (this.ucMultiSelectUA6.ViewDataSetDX.HasChanges() == true)
                {
                    sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                            "Values ('" + this.uteCodice6.Text + "', '" + this.ucMultiSelectUA6.Tag.ToString() + "', '{0}')";
                    UpdateBindingsAssDataSet(this.ucMultiSelectUA6.ViewDataSetDX.GetChanges(), "Codice", sSql);
                }
                if (this.ucMultiSelectPlusRuoli6.ViewDataSetSX.HasChanges() == true)
                {
                    sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice6.Text) + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectUA6.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodRuolo = '{0}'" + Environment.NewLine +
                                    "And CodAzione = '{1}'";
                    UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli6.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                }
                if (this.ucMultiSelectPlusRuoli6.ViewDataSetDX.HasChanges() == true)
                {
                    sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                           "Values ('{0}', '" + this.ucMultiSelectUA6.Tag.ToString() + "', '" + DataBase.Ax2(this.uteCodice6.Text) + @"', '{1}')";
                    UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli6.ViewDataSetDX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "UpdateBindingsAss", this.Text);
            }

        }

        private void DeleteBindingsAss()
        {

            string sSql = @"";

            try
            {
                sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice6.Text + "' And CodEntita = '" + this.ucMultiSelectUA6.Tag.ToString() + "'";
                DataBase.ExecuteSql(sSql);
                sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice6.Text) + "'" + Environment.NewLine +
                    "And CodEntita = '" + this.ucMultiSelectUA6.Tag.ToString() + "'";
                DataBase.ExecuteSql(sSql);

            }
            catch (Exception)
            {

            }

        }

        private void EditBindingsCopia()
        {
            try
            {

                foreach (DataColumn col in this.ViewDataBindings.DataBindings.DataSet.Tables[0].Columns)
                {
                    try
                    {
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0][col.ColumnName] = DBNull.Value;
                    }
                    catch
                    {
                    }
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "EditBindingsCopia", this.Text);
            }
        }

        private void UpdateBindingsAssDataSet(DataSet oDs, string field, string sql)
        {

            try
            {

                foreach (DataRow oRow in oDs.Tables[0].Rows)
                {
                    if (oRow.RowState == DataRowState.Added || this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                    {
                        DataBase.ExecuteSql(string.Format(sql, oRow[field]));
                    }
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "UpdateBindingsAssDataSet", this.Text);
            }

        }
        private void UpdateBindingsAssDataSet(DataSet oDs, string field1, string field2, string sql)
        {

            try
            {

                foreach (DataRow oRow in oDs.Tables[0].Rows)
                {
                    if (oRow.RowState == DataRowState.Added || this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                    {
                        DataBase.ExecuteSql(string.Format(sql, oRow[field1], oRow[field2]));
                    }
                }

            }
            catch (Exception)
            {

            }

        }

        private void UpdateBindingsAssDataTable(DataTable dt, string codice1, string codice2, string sql)
        {

            try
            {

                foreach (DataRow odr in dt.Rows)
                {
                    DataBase.ExecuteSql(string.Format(sql, odr[codice1].ToString(), odr[codice2].ToString()));
                }

            }
            catch (Exception)
            {

            }

        }

        private bool CheckInput()
        {

            bool bRet = true;

            if (bRet && this.uteCodice6.Text.Trim() == "")
            {
                MessageBox.Show(@"Inserire " + this.lblCodice6.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodice6.Focus();
                bRet = false;
            }
            if (bRet && this.uteDescrizione6.Text.Trim() == "")
            {
                MessageBox.Show(@"Inserire " + this.lblDescrizione6.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteDescrizione6.Focus();
                bRet = false;
            }
            if (bRet && (this.uteCodScheda6.Text.Trim() == "" || this.lblCodSchedaDes6.Text.Trim() == ""))
            {
                MessageBox.Show(@"Inserire " + this.lblCodScheda6.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodScheda6.Focus();
                bRet = false;
            }

            return bRet;

        }

        private void DisposeUserControls(Control Container)
        {

            foreach (Control c in Container.Controls)
            {

                if (c.HasChildren) DisposeUserControls(c);

                if (c.GetType().ToString() == "UnicodeSrl.ScciCore.ucMultiSelect") c.Dispose();

            }

        }

        private int CheckVersioneCorrenteScheda(string codiceScheda)
        {
            return int.Parse(DataBase.FindValue("IsNull(Versione,0)", "T_SchedeVersioni", "CodScheda = '" + codiceScheda + "' And FlagAttiva = 1 Order By DtValI desc, DtValF", "0"));
        }

        #endregion

        #region Events

        private void frmPUTipoConsegna_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeUserControls(this.UltraGroupBox);
        }

        private void ute_ValueChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void ucMultiSelect_Change(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void ucMultiSelect_GridMasterInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("CodAzione") == true) { e.Layout.Bands[0].Columns["CodAzione"].Hidden = true; }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelect_GridMasterInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

            }
            catch (Exception)
            {

            }

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

        private void uteCodScheda6_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                switch (e.Button.Key)
                {

                    case "Zoom":
                        frmZoom f = new frmZoom();
                        f.ViewText = this.lblCodScheda6.Text;
                        f.ViewIcon = this.ViewIcon;
                        f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Schede";
                        f.ViewSqlStruct.Where = "CodEntita = '" + this.ucMultiSelectUA6.Tag.ToString() + "' And Codice <> '" + DataBase.Ax2(this.uteCodScheda6.Text) + "'";
                        f.ViewInit();
                        f.ShowDialog();
                        if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            this.uteCodScheda6.Text = f.ViewActiveRow.Cells["Codice"].Text;
                        }
                        break;

                    case "Scheda":
                        string codscheda = ((UltraTextEditor)sender).Text;
                        if (codscheda != "")
                        {
                            int nVersione = this.CheckVersioneCorrenteScheda(codscheda);
                            if (nVersione != 0)
                            {
                                if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_SchedeVersioni, Enums.EnumModalityPopUp.mpModifica, this.ViewIcon, this.ViewImage, this.ViewText, codscheda, nVersione.ToString()) == DialogResult.OK)
                                {
                                }
                            }
                            else
                            {
                                MessageBox.Show(@"Nessuna versione esistente per la scheda '" + codscheda + "'.", @"Errore di caricamento", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        break;

                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodScheda6_EditorButtonClick", this.Text);
            }

        }
        private void uteCodScheda6_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodSchedaDes6.Text = DataBase.FindValue("Descrizione", "T_Schede", string.Format("Codice = '{0}' AND CodEntita = '{1}'", new string[2] { DataBase.Ax2(this.uteCodScheda6.Text), this.ucMultiSelectUA6.Tag.ToString() }), "");
            this.ubApplica.Enabled = true;
        }

        private void ucpColore6_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColore6.Value = this.colorDialog.Color;
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
                    case Enums.EnumModalityPopUp.mpCopia:
                        if (CheckInput())
                        {

                            if (this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                            {
                                this.ViewDataBindings.SqlSelect.Where = @"0=1";
                                this.ViewDataBindings.DataBindings.DataSet = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                                DataRow _dr = this.ViewDataBindings.DataBindings.DataSet.Tables[0].NewRow();
                                DataBase.GetDefultValues(ref _dr, this.ViewDataNamePU);
                                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows.Add(_dr);
                            }

                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
                            this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice6.Text + "'";
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
                    case Enums.EnumModalityPopUp.mpCopia:
                        if (CheckInput())
                        {
                            if (this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                            {
                                this.ViewDataBindings.SqlSelect.Where = @"0=1";
                                this.ViewDataBindings.DataBindings.DataSet = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                                DataRow _dr = this.ViewDataBindings.DataBindings.DataSet.Tables[0].NewRow();
                                DataBase.GetDefultValues(ref _dr, this.ViewDataNamePU);
                                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows.Add(_dr);
                            }

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

        #endregion

    }
}
