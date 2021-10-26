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
    public partial class frmPUProtocolliAttivita : Form, Interfacce.IViewFormPUView
    {
        public frmPUProtocolliAttivita()
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
            this.InitializeUltraGrid();

            this.SetBindings();
            this.SetBindingsAss();

            switch (_Modality)
            {

                case Enums.EnumModalityPopUp.mpNuovo:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.ubApplica.Enabled = true;
                    this.UltraTabControl.Tabs["tab2"].Enabled = false;
                    this.UltraTabControl.Tabs["UA"].Enabled = false;
                    this.UltraTabControl.Tabs["Ruoli"].Enabled = false;
                    break;

                case Enums.EnumModalityPopUp.mpModifica:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.uteCodice.Enabled = false;
                    this.ubApplica.Enabled = false;
                    this.UltraTabControl.Tabs["tab2"].Enabled = true;
                    this.UltraTabControl.Tabs["UA"].Enabled = true;
                    this.UltraTabControl.Tabs["Ruoli"].Enabled = true;
                    break;

                case Enums.EnumModalityPopUp.mpCancella:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    this.UltraTabControl.Tabs["tab2"].Enabled = true;
                    this.UltraTabControl.Tabs["UA"].Enabled = true;
                    this.UltraTabControl.Tabs["Ruoli"].Enabled = true;
                    break;

                case Enums.EnumModalityPopUp.mpVisualizza:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    this.ubConferma.Enabled = false;
                    this.UltraTabControl.Tabs["tab2"].Enabled = true;
                    this.UltraTabControl.Tabs["UA"].Enabled = true;
                    this.UltraTabControl.Tabs["Ruoli"].Enabled = true;
                    break;

                default:
                    break;

            }

            this.LoadUltraGridTempi();
            this.LoadUltraGridTempiTipoTask();

            this.SetUltraToolBarManager();

            this.ResumeLayout();

        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);
                MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManagerTempi);

                foreach (ToolBase oTool in this.UltraToolbarsManagerTempi.Tools)
                {
                    oTool.SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz32));
                    oTool.SharedProps.AppearancesSmall.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz16));
                }

            }
            catch (Exception)
            {

            }

        }

        private void SetUltraToolBarManager()
        {

            bool bNuovo = true;
            bool bModifica = true;
            bool bElimina = true;
            bool bVisualizza = true;
            bool bStampa = true;
            bool bAggiorna = true;
            bool bEsporta = true;

            if (this.ViewModality == Enums.EnumModalityPopUp.mpCancella || this.ViewModality == Enums.EnumModalityPopUp.mpVisualizza)
            {
                bNuovo = false;
                bModifica = false;
                bElimina = false;
                bVisualizza = false;
                bStampa = false;
                bAggiorna = false;
                bEsporta = false;
            }
            else
            {
                if ((this.UltraGridTempi.Rows.Count > 0 && this.UltraGridTempi.ActiveRow != null) && this.UltraGridTempi.ActiveRow.IsDataRow)
                {
                    bModifica = true;
                    bElimina = true;
                    bVisualizza = true;
                    bStampa = true;
                    bEsporta = true;
                }
                else
                {
                    bModifica = false;
                    bElimina = false;
                    bVisualizza = false;
                    bStampa = false;
                    bEsporta = false;
                }
            }

            this.UltraToolbarsManagerTempi.Tools[MyStatics.GC_NUOVO].SharedProps.Enabled = bNuovo;
            this.UltraToolbarsManagerTempi.Tools[MyStatics.GC_MODIFICA].SharedProps.Enabled = bModifica;
            this.UltraToolbarsManagerTempi.Tools[MyStatics.GC_ELIMINA].SharedProps.Enabled = bElimina;
            this.UltraToolbarsManagerTempi.Tools[MyStatics.GC_VISUALIZZA].SharedProps.Enabled = bVisualizza;
            this.UltraToolbarsManagerTempi.Tools[MyStatics.GC_STAMPA].SharedProps.Enabled = bStampa;
            this.UltraToolbarsManagerTempi.Tools[MyStatics.GC_AGGIORNA].SharedProps.Enabled = bAggiorna;
            this.UltraToolbarsManagerTempi.Tools[MyStatics.GC_ESPORTA].SharedProps.Enabled = bEsporta;

        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {

            MyStatics.SetUltraGridLayout(ref this.UltraGridTempi, false, false);
            UltraGridTempi.DisplayLayout.GroupByBox.Hidden = true;

            MyStatics.SetUltraGridLayout(ref this.UltraGridTempiTipoTask, false, false);
            UltraGridTempiTipoTask.DisplayLayout.GroupByBox.Hidden = true;
        }

        private void LoadUltraGridTempi()
        {

            int nIndex = -1;

            UnicodeSrl.Sys.Data2008.SqlStruct SqlSelect = new UnicodeSrl.Sys.Data2008.SqlStruct();
            SqlSelect.SelectString = "";
            SqlSelect.Where += "";
            SqlSelect.GroupBy = "";
            SqlSelect.OrderBy = "";
            SqlSelect.Having = "";

            try
            {

                if (UltraGridTempi.ActiveRow != null) nIndex = UltraGridTempi.ActiveRow.Index;

                SqlSelect.SelectString = DataBase.GetSqlView(Enums.EnumDataNames.T_ProtocolliAttivitaTempi);
                SqlSelect.Where += "CodProtocolloAttivita = '" + this.uteCodice.Text + "'";
                SqlSelect.OrderBy = "Descrizione";
                UltraGridTempi.DataSource = DataBase.GetDataSet(SqlSelect.Sql);

                UltraGridTempi.Refresh();
                UltraGridTempi.Text = string.Format("{0} ({1:#,##0})", "Tempi", UltraGridTempi.Rows.Count);

                foreach (UltraGridColumn oCol in UltraGridTempi.DisplayLayout.Bands[0].Columns)
                {
                    try
                    {
                        switch (oCol.DataType.Name.Trim().ToUpper())
                        {
                            case "DATETIME":
                            case "DATE":
                                oCol.Format = @"HH:mm";
                                oCol.Hidden = false;
                                UltraGridTempi.DisplayLayout.Bands[0].Columns["Delta"].Hidden = !oCol.Hidden;
                                UltraGridTempi.DisplayLayout.Bands[0].Columns["CodTipoProtocollo"].Hidden = true;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                break;

                            case "INT":
                            case "INT16":
                            case "INT32":
                            case "INT64":
                            case "LONG":
                            case "INTEGER":
                                if (oCol.Key.Trim().ToUpper().IndexOf("COD") < 0 && oCol.Key.Trim().ToUpper() != "ID") oCol.Format = @"#,##0";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.Hidden = false;
                                UltraGridTempi.DisplayLayout.Bands[0].Columns["Ora"].Hidden = !oCol.Hidden;
                                break;

                            case "DECIMAL":
                            case "DOUBLE":
                            case "SINGLE":
                                if (oCol.Key.Trim().ToUpper().IndexOf("COD") < 0 && oCol.Key.Trim().ToUpper() != "ID") oCol.Format = @"#,##0.00";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                break;

                            case "BOOL":
                            case "BOOLEAN":
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                break;

                            case "BYTE[]":
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                break;


                            default:
                                break;
                        }
                        if (oCol.Key.Trim().ToUpper().IndexOf("FLAG") == 0)
                            oCol.Header.Caption = oCol.Key.Substring(4);

                        if (oCol.Key.Trim().ToUpper().IndexOf("FLG") == 0)
                            oCol.Header.Caption = oCol.Key.Substring(3);
                    }
                    catch (Exception)
                    {
                    }
                }

                UltraGridTempi.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

                if (nIndex != -1)
                {
                    try
                    {
                        UltraGridTempi.ActiveRow = UltraGridTempi.Rows[nIndex];
                    }
                    catch (Exception)
                    {
                        if (UltraGridTempi.Rows.Count > 0)
                            UltraGridTempi.ActiveRow = UltraGridTempi.Rows[UltraGridTempi.Rows.Count - 1];
                    }
                }
                else
                {
                    if (UltraGridTempi.Rows.Count > 0)
                        UltraGridTempi.ActiveRow = UltraGridTempi.Rows[0];
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void LoadUltraGridTempiTipoTask()
        {

            UnicodeSrl.Sys.Data2008.SqlStruct SqlSelect = new UnicodeSrl.Sys.Data2008.SqlStruct();

            try
            {

                if (this.UltraGridTempi.ActiveRow != null)
                {
                    SqlSelect.SelectString = DataBase.GetSqlView(Enums.EnumDataNames.T_ProtocolliAttivitaTempiTipoTask);
                    SqlSelect.Where = "CodProtocolloAttivitaTempi = '" + UltraGridTempi.ActiveRow.Cells["Codice"].Value.ToString() + "'";
                    SqlSelect.OrderBy = "Descrizione";
                    SqlSelect.GroupBy = string.Empty;
                    SqlSelect.Having = string.Empty;
                }
                else
                {
                    SqlSelect.SelectString = DataBase.GetSqlView(Enums.EnumDataNames.T_ProtocolliAttivitaTempiTipoTask);
                    SqlSelect.Where = "0=1";
                    SqlSelect.GroupBy = string.Empty;
                    SqlSelect.Having = string.Empty;
                }

                this.UltraGridTempiTipoTask.DataSource = DataBase.GetDataSet(SqlSelect.Sql);
                this.UltraGridTempiTipoTask.Refresh();

                this.UltraGridTempiTipoTask.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

                this.UltraGridTempiTipoTask.Text = string.Format("{0} ({1:#,##0})", "Task Infermieristici Associati", this.UltraGridTempiTipoTask.Rows.Count);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        #endregion

        #region Subroutine

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

            if (bRet && this.ViewModality == Enums.EnumModalityPopUp.mpNuovo && DataBase.FindValue("Codice", "T_ProtocolliAttivita", "Codice = '" + this.uteCodice.Text + "'", string.Empty) != string.Empty)
            {

                MessageBox.Show(@"Codice inserito già presente in archivio!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodice.Focus();
                bRet = false;
            }

            return bRet;

        }

        private void SetBindingsAss()
        {

            string sSql = @"";

            try
            {

                this.ucMultiSelectUA.ViewShowAll = true;
                this.ucMultiSelectUA.ViewShowFind = true;
                sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                        "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC",
                                        "Not In", this.uteCodice.Text, EnumEntita.PRA.ToString());
                this.ucMultiSelectUA.ViewDataSetSX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                        "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC",
                                        "In", this.uteCodice.Text, EnumEntita.PRA.ToString());
                this.ucMultiSelectUA.ViewDataSetDX = DataBase.GetDataSet(sSql);
                this.ucMultiSelectUA.ViewInit();
                this.ucMultiSelectUA.RefreshData();

                this.ucMultiSelectPlusRuoli.ViewShowAll = true;

                this.ucMultiSelectPlusRuoli.ViewShowFind = true; this.ucMultiSelectPlusRuoli.GridDXFilterColumnIndex = 2;
                this.ucMultiSelectPlusRuoli.GridSXFilterColumnIndex = 2;

                sSql = string.Format("Select QAE.CodAzione, R.Codice AS CodRuolo, R.Descrizione As Ruolo" + Environment.NewLine +
                                     "   From (Select AE.CodEntita, AE.CodAzione" + Environment.NewLine +
                                     "           From T_AzioniEntita AE" + Environment.NewLine +
                                     "                   Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                     "           Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                     "                   And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                     "                   And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                     "           ) As QAE" + Environment.NewLine +
                                     "       Cross Join T_Ruoli R" + Environment.NewLine +
                                     "       Left Join (Select ASS.CodEntita, ASS.CodAzione, ASS.CodRuolo" + Environment.NewLine +
                                     "                   From T_AssRuoliAzioni ASS" + Environment.NewLine +
                                     "                           Inner Join T_Azioni A On ASS.CodAzione = A.Codice" + Environment.NewLine +
                                     "                           Inner Join T_Entita E On ASS.CodEntita = E.Codice" + Environment.NewLine +
                                     "                           Inner Join T_AzioniEntita AE On (ASS.CodEntita = AE.CodEntita" + Environment.NewLine +
                                     "                                                               And ASS.CodAzione = AE.CodAzione)" + Environment.NewLine +
                                     "                   Where ASS.CodEntita = '{0}'" + Environment.NewLine +
                                     "                           And ASS.CodVoce = '{1}'" + Environment.NewLine +
                                     "                           And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                     "                           And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                     "                   ) As QASS On QAE.CodEntita = QASS.CodEntita" + Environment.NewLine +
                                     "                                   And QAE.CodAzione = QASS.CodAzione" + Environment.NewLine +
                                     "                                   And R.Codice = QASS.CodRuolo" + Environment.NewLine +
                                     "   Where QASS.CodEntita Is Null" + Environment.NewLine +
                                     "   Order By QAE.CodAzione, R.Descrizione ASC", EnumEntita.PRA.ToString(), this.uteCodice.Text);
                this.ucMultiSelectPlusRuoli.ViewDataSetSX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select ASS.CodAzione, ASS.CodRuolo, R.Descrizione As Ruolo" + Environment.NewLine +
                                     "   From T_AssRuoliAzioni ASS" + Environment.NewLine +
                                     "           Inner Join T_Ruoli R On ASS.CodRuolo = R.Codice" + Environment.NewLine +
                                     "           Inner Join T_Azioni A On ASS.CodAzione = A.Codice" + Environment.NewLine +
                                     "           Inner Join T_Entita E On ASS.CodEntita = E.Codice" + Environment.NewLine +
                                     "           Inner Join T_AzioniEntita AE On ASS.CodEntita = AE.CodEntita And ASS.CodAzione = AE.CodAzione" + Environment.NewLine +
                                     "   Where ASS.CodEntita = '{0}'" + Environment.NewLine +
                                     "           And ASS.CodVoce = '{1}'" + Environment.NewLine +
                                     "           And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                     "           And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                     "   Order By ASS.CodAzione, R.Descrizione ASC", EnumEntita.PRA.ToString(), this.uteCodice.Text);
                this.ucMultiSelectPlusRuoli.ViewDataSetDX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                     "   From T_AzioniEntita AE" + Environment.NewLine +
                                     "           Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                     "           Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                     "   Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                     "           And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                     "           And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", EnumEntita.PRA.ToString());
                this.ucMultiSelectPlusRuoli.ViewDataSetMaster = DataBase.GetDataSet(sSql);
                this.ucMultiSelectPlusRuoli.ViewInit();
                this.ucMultiSelectPlusRuoli.RefreshData();


            }
            catch (Exception)
            {

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

        private void UpdateBindingsAss()
        {

            string sSql = @"";

            try
            {

                if (this.ucMultiSelectUA.ViewDataSetSX.HasChanges() == true)
                {

                    sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                        "Where CodVoce = '" + this.uteCodice.Text + "'" + Environment.NewLine +
                        "And CodEntita = '" + EnumEntita.PRA.ToString() + "'" + Environment.NewLine +
                        "And CodUA = '{0}'";
                    UpdateBindingsAssDataSet(this.ucMultiSelectUA.ViewDataSetSX.GetChanges(), "Codice", sSql);
                }
                if (this.ucMultiSelectUA.ViewDataSetDX.HasChanges() == true)
                {
                    sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                            "Values ('" + this.uteCodice.Text + "', '" + EnumEntita.PRA.ToString() + "', '{0}')";
                    UpdateBindingsAssDataSet(this.ucMultiSelectUA.ViewDataSetDX.GetChanges(), "Codice", sSql);
                }

                if (this.ucMultiSelectPlusRuoli.ViewDataSetSX.HasChanges() == true)
                {
                    sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine +
                                    "And CodEntita = '" + EnumEntita.PRA.ToString() + "'" + Environment.NewLine +
                                    "And CodRuolo = '{0}'" + Environment.NewLine +
                                    "And CodAzione = '{1}'";
                    UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                }
                if (this.ucMultiSelectPlusRuoli.ViewDataSetDX.HasChanges() == true)
                {
                    sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                           "Values ('{0}', '" + EnumEntita.PRA.ToString() + "', '" + DataBase.Ax2(this.uteCodice.Text) + @"', '{1}')";
                    UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli.ViewDataSetDX.GetChanges(), "CodRuolo", "CodAzione", sSql);
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
                sSQL = @"DELETE FROM T_ProtocolliAttivitaTempiTipoTask WHERE " +
       "CodProtocolloAttivitaTempi IN (SELECT Codice FROM T_ProtocolliAttivitaTempi WHERE CodProtocolloAttivita = '" + this.uteCodice.Text + "')";
                DataBase.ExecuteSql(sSQL);

                sSQL = "DELETE FROM T_ProtocolliAttivitaTempi WHERE CodProtocolloAttivita = '" + this.uteCodice.Text + "'";
                DataBase.ExecuteSql(sSQL);

                sSQL = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice.Text + "' And CodEntita = '" + EnumEntita.PRA.ToString() + "'";
                DataBase.ExecuteSql(sSQL);

                sSQL = "Delete from T_AssRuoliAzioni Where CodVoce = '" + this.uteCodice.Text + "' And CodEntita = '" + EnumEntita.PRA.ToString() + "'";
                DataBase.ExecuteSql(sSQL);

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "DeleteBindingsAss", this.Text);
            }

        }

        #endregion

        #region Events

        private void ute_ValueChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void ucMultiSelect_Change(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void ucpColore_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColore.Value = this.colorDialog.Color;
            }
        }

        private void UltraToolBarManagerTempi_ToolClick(object sender, ToolClickEventArgs e)
        {

            try
            {
                UltraGridRow activeRow = null;
                if (this.UltraGridTempi.ActiveRow != null) activeRow = this.UltraGridTempi.ActiveRow;

                switch (e.Tool.Key)
                {
                    case MyStatics.GC_NUOVO:
                        if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_ProtocolliAttivitaTempi,
                                                           Enums.EnumModalityPopUp.mpNuovo, this.ViewIcon,
                                                           this.ViewImage, this.ViewText, ref activeRow, this.uteCodice.Text) == DialogResult.OK)
                        {
                            this.LoadUltraGridTempi();
                            this.LoadUltraGridTempiTipoTask();
                            this.SetUltraToolBarManager();
                        }
                        break;

                    case MyStatics.GC_MODIFICA:
                        if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_ProtocolliAttivitaTempi,
                                                           Enums.EnumModalityPopUp.mpModifica, this.ViewIcon,
                                                           this.ViewImage, this.ViewText, ref activeRow, this.uteCodice.Text) == DialogResult.OK)
                        {
                            this.LoadUltraGridTempi();
                            this.LoadUltraGridTempiTipoTask();
                            this.SetUltraToolBarManager();
                        }
                        break;

                    case MyStatics.GC_ELIMINA:
                        if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_ProtocolliAttivitaTempi,
                                       Enums.EnumModalityPopUp.mpCancella, this.ViewIcon,
                                       this.ViewImage, this.ViewText, ref activeRow, this.uteCodice.Text) == DialogResult.OK)
                        {
                            this.LoadUltraGridTempi();
                            this.LoadUltraGridTempiTipoTask();
                            this.SetUltraToolBarManager();
                        }
                        break;

                    case MyStatics.GC_VISUALIZZA:
                        MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_ProtocolliAttivitaTempi,
                                                       Enums.EnumModalityPopUp.mpVisualizza, this.ViewIcon,
                                                       this.ViewImage, this.ViewText, ref activeRow, this.uteCodice.Text);
                        break;

                    case MyStatics.GC_STAMPA:
                        try
                        {
                            this.UltraGridTempi.PrintPreview(Infragistics.Win.UltraWinGrid.RowPropertyCategories.All);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(@"Si sono verificati errori durante il processo di stampa." + Environment.NewLine + ex.Message, @"Errore di stampa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    case MyStatics.GC_AGGIORNA:
                        this.UltraGridTempi.Rows.ColumnFilters.ClearAllFilters();
                        this.LoadUltraGridTempi();
                        this.LoadUltraGridTempiTipoTask();
                        this.SetUltraToolBarManager();

                        break;

                    case MyStatics.GC_ESPORTA:
                        if (this.SaveFileDialog.ShowDialog() == DialogResult.OK)
                            this.UltraGridExcelExporter.Export(this.UltraGridTempi, this.SaveFileDialog.FileName);
                        break;

                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void UltraGridTempiTipoTask_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Exists("Icona") == true)
            {
                e.Layout.Bands[0].Columns["Icona"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                e.Layout.Bands[0].Columns["Icona"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            }
            if (e.Layout.Bands[0].Columns.Exists("CodColore") == true)
            {
                e.Layout.Bands[0].Columns["CodColore"].Hidden = true;
            }
            if (e.Layout.Bands[0].Columns.Exists("Colore") == true)
            {
                e.Layout.Bands[0].Columns["Colore"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            }

        }

        private void UltraGridTempiTipoTask_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells.Exists("Colore"))
            {
                e.Row.Cells["Colore"].Appearance.Image = MyStatics.CreateSolidBitmap(MyStatics.GetColorFromString(e.Row.Cells["CodColore"].Value.ToString()), 32, 32);
            }
        }

        private void UltraGridTempi_AfterRowActivate(object sender, EventArgs e)
        {
            this.LoadUltraGridTempiTipoTask();
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

        #endregion

    }
}
