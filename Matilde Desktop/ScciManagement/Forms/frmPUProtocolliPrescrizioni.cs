using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciCore;
using UnicodeSrl.ScciCore.Common.ProtocolliPrescrizioni;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUProtocolliPrescrizioni : Form, Interfacce.IViewFormPUView
    {
        public frmPUProtocolliPrescrizioni()
        {
            InitializeComponent();
        }

        #region Declare

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;
        private DialogResult _DialogResult = DialogResult.Cancel;

        List<ModelloPrescrizione> _modelliPrescrizioni = new List<ModelloPrescrizione>();

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

            ucPictureSelectIcona.ViewOpenFileDialogFilter = @"png files (*.png)|*.png|jpg files (*.jpg)|*.jpg|bmp files (*.bmp)|*.bmp";

            _modelliPrescrizioni = SerializerProtocolli.DeSerializeModelli(ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["ModelliPrescrizioni"].ToString());

            this.LoadUltraGridPrescrizioni();
            this.SetUltraToolBarManager();

            this.ResumeLayout();
        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.UltraToolbarManager);
                MyStatics.SetUltraToolbarsManager(this.UltraToolbarManagerPrescrizioni);

                foreach (ToolBase oTool in this.UltraToolbarManagerPrescrizioni.Tools)
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
            bool bElimina = true;
            bool bStampa = true;
            bool bAggiorna = true;
            bool bEsporta = true;

            if (this.ViewModality == Enums.EnumModalityPopUp.mpCancella || this.ViewModality == Enums.EnumModalityPopUp.mpVisualizza)
            {
                bNuovo = false;
                bElimina = false;
                bStampa = false;
                bAggiorna = false;
                bEsporta = false;
            }
            else
            {
                if ((this.UltraGridPrescrizioni.Rows.Count > 0 && this.UltraGridPrescrizioni.ActiveRow != null) && this.UltraGridPrescrizioni.ActiveRow.IsDataRow)
                {
                    bElimina = true;
                    bStampa = true;
                    bEsporta = true;
                }
                else
                {
                    bElimina = false;
                    bStampa = false;
                    bEsporta = false;
                }
            }

            this.UltraToolbarManagerPrescrizioni.Tools[MyStatics.GC_NUOVO].SharedProps.Enabled = bNuovo;
            this.UltraToolbarManagerPrescrizioni.Tools[MyStatics.GC_ELIMINA].SharedProps.Enabled = bElimina;
            this.UltraToolbarManagerPrescrizioni.Tools[MyStatics.GC_STAMPA].SharedProps.Enabled = bStampa;
            this.UltraToolbarManagerPrescrizioni.Tools[MyStatics.GC_AGGIORNA].SharedProps.Enabled = bAggiorna;
            this.UltraToolbarManagerPrescrizioni.Tools[MyStatics.GC_ESPORTA].SharedProps.Enabled = bEsporta;

        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {

            MyStatics.SetUltraGridLayout(ref this.UltraGridPrescrizioni, false, false);
            UltraGridPrescrizioni.DisplayLayout.GroupByBox.Hidden = true;
            UltraGridPrescrizioni.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
        }

        private void LoadUltraGridPrescrizioni()
        {
            try
            {
                if (_modelliPrescrizioni != null)
                {
                    DataTable dt = new DataTable();

                    dt.Columns.Add(new DataColumn("ID", typeof(string)));
                    dt.Columns.Add(new DataColumn("Descrizione", typeof(string)));

                    foreach (var mod in _modelliPrescrizioni)
                    {
                        DataRow row = dt.NewRow();
                        row["ID"] = mod.ID.ToString();
                        row["Descrizione"] = mod.Descrizione;
                        dt.Rows.Add(row);
                    }

                    UltraGridPrescrizioni.DataSource = dt;
                }
                else
                    UltraGridPrescrizioni.DataSource = null;

                UltraGridPrescrizioni.Refresh();
                UltraGridPrescrizioni.Text = string.Format("{0} ({1:#,##0})", "Modelli Terapie Farmacologiche", UltraGridPrescrizioni.Rows.Count);
                UltraGridPrescrizioni.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

                if (UltraGridPrescrizioni.Rows.Count > 0)
                {
                    UltraGridPrescrizioni.ActiveRow = UltraGridPrescrizioni.Rows[0];
                }
            }
            catch (Exception ex)
            {
                this.UltraGridPrescrizioni.DataSource = null;
                Framework.Diagnostics.Statics.AddDebugInfo(ex);
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
                                        "Not In", this.uteCodice.Text, EnumEntita.PRP.ToString());
                this.ucMultiSelectUA.ViewDataSetSX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                        "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC",
                                        "In", this.uteCodice.Text, EnumEntita.PRP.ToString());
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
                                     "   Order By QAE.CodAzione, R.Descrizione ASC", EnumEntita.PRP.ToString(), this.uteCodice.Text);
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
                                     "   Order By ASS.CodAzione, R.Descrizione ASC", EnumEntita.PRP.ToString(), this.uteCodice.Text);
                this.ucMultiSelectPlusRuoli.ViewDataSetDX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                     "   From T_AzioniEntita AE" + Environment.NewLine +
                                     "           Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                     "           Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                     "   Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                     "           And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                     "           And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", EnumEntita.PRP.ToString());
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
                ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona.ViewImage);
                ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore.Value.ToString();
                ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["ModelliPrescrizioni"] = SerializerProtocolli.SerializeModelli(_modelliPrescrizioni);
                ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["DataOraInizioObbligatoria"] = CheckDataOraInizioObbligatoria();
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
                        "And CodEntita = '" + EnumEntita.PRP.ToString() + "'" + Environment.NewLine +
                        "And CodUA = '{0}'";
                    UpdateBindingsAssDataSet(this.ucMultiSelectUA.ViewDataSetSX.GetChanges(), "Codice", sSql);
                }
                if (this.ucMultiSelectUA.ViewDataSetDX.HasChanges() == true)
                {
                    sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                            "Values ('" + this.uteCodice.Text + "', '" + EnumEntita.PRP.ToString() + "', '{0}')";
                    UpdateBindingsAssDataSet(this.ucMultiSelectUA.ViewDataSetDX.GetChanges(), "Codice", sSql);
                }

                if (this.ucMultiSelectPlusRuoli.ViewDataSetSX.HasChanges() == true)
                {
                    sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine +
                                    "And CodEntita = '" + EnumEntita.PRP.ToString() + "'" + Environment.NewLine +
                                    "And CodRuolo = '{0}'" + Environment.NewLine +
                                    "And CodAzione = '{1}'";
                    UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                }
                if (this.ucMultiSelectPlusRuoli.ViewDataSetDX.HasChanges() == true)
                {
                    sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                           "Values ('{0}', '" + EnumEntita.PRP.ToString() + "', '" + DataBase.Ax2(this.uteCodice.Text) + @"', '{1}')";
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
                sSQL = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice.Text + "' And CodEntita = '" + EnumEntita.PRP.ToString() + "'";
                DataBase.ExecuteSql(sSQL);

                sSQL = "Delete from T_AssRuoliAzioni Where CodVoce = '" + this.uteCodice.Text + "' And CodEntita = '" + EnumEntita.PRP.ToString() + "'";
                DataBase.ExecuteSql(sSQL);

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "DeleteBindingsAss", this.Text);
            }

        }

        #endregion

        #region modelli prescrizioni

        private bool CheckIDPrescrizione(string idPrescrizione)
        {
            string sSQL = string.Empty;

            try
            {
                sSQL = "SELECT ID FROM T_MovPrescrizioni WHERE ID = '" + idPrescrizione + "'";
                return DataBase.GetDataTable(sSQL).Rows.Count == 1;
            }
            catch (Exception ex)
            {
                Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return false;
            }
        }

        private bool InsertModelloPrescrizione(string idPrescrizione, string descrizione)
        {
            bool bRet = true;
            string sSQL = string.Empty;

            DataTable dtP = null;
            DataTable dtT = null;
            DataSet dsTM = null;
            ModelloPrescrizione mod = null;
            ModelloPrescrizioneTempo modt = null;
            ModelloTempoManuale modtm = null;

            try
            {
                sSQL = "SELECT" + Environment.NewLine;
                sSQL += "P.ID," + Environment.NewLine;
                sSQL += "    P.IDEpisodio," + Environment.NewLine;
                sSQL += "    P.IDTrasferimento," + Environment.NewLine;
                sSQL += "    P.CodTipoPrescrizione," + Environment.NewLine;
                sSQL += "    P.CodViaSomministrazione," + Environment.NewLine;
                sSQL += "    S.CodScheda," + Environment.NewLine;
                sSQL += "    S.Versione," + Environment.NewLine;
                sSQL += "    S.Dati" + Environment.NewLine;
                sSQL += "FROM" + Environment.NewLine;
                sSQL += "    T_MovPrescrizioni P INNER JOIN T_MovSchede S ON" + Environment.NewLine;
                sSQL += "        P.ID = S.IDEntita" + Environment.NewLine;
                sSQL += " WHERE" + Environment.NewLine;
                sSQL += "    S.CodEntita = 'PRF' AND S.Storicizzata = 0" + Environment.NewLine;
                sSQL += "    AND P.ID = '" + idPrescrizione + "'";

                dtP = DataBase.GetDataTable(sSQL);

                if (dtP != null && dtP.Rows.Count > 0)
                {
                    sSQL = "SELECT" + Environment.NewLine;
                    sSQL += "    T.CodTipoPrescrizioneTempi," + Environment.NewLine;
                    sSQL += "    T.Posologia," + Environment.NewLine;
                    sSQL += "    T.CodProtocollo," + Environment.NewLine;
                    sSQL += "    T.DataOraInizio," + Environment.NewLine;
                    sSQL += "    T.DataOraFine," + Environment.NewLine;
                    sSQL += "    ISNULL(T.AlBisogno, 0) AS AlBisogno," + Environment.NewLine;
                    sSQL += "    ISNULL(T.Durata, 0) AS Durata," + Environment.NewLine;
                    sSQL += "    ISNULL(T.Continuita, 0) AS Continuita," + Environment.NewLine;
                    sSQL += "    ISNULL(T.PeriodicitaGiorni, 0) AS PeriodicitaGiorni," + Environment.NewLine;
                    sSQL += "    ISNULL(T.PeriodicitaOre, 0) AS PeriodicitaOre," + Environment.NewLine;
                    sSQL += "    ISNULL(T.PeriodicitaMinuti, 0) AS PeriodicitaMinuti," + Environment.NewLine;
                    sSQL += "    ISNULL(T.Manuale, 0) AS Manuale," + Environment.NewLine;
                    sSQL += "    S.CodScheda," + Environment.NewLine;
                    sSQL += "    S.Versione," + Environment.NewLine;
                    sSQL += "    S.Dati," + Environment.NewLine;
                    sSQL += "    T.TempiManuali" + Environment.NewLine;
                    sSQL += "FROM" + Environment.NewLine;
                    sSQL += "    T_MovPrescrizioniTempi T LEFT JOIN T_MovSchede S ON" + Environment.NewLine;
                    sSQL += "        T.ID = S.IDEntita AND S.CodEntita = 'PRT' AND S.Storicizzata = 0" + Environment.NewLine;
                    sSQL += "WHERE" + Environment.NewLine;
                    sSQL += "    T.IDPrescrizione = '" + idPrescrizione + "'" + Environment.NewLine;
                    sSQL += "    AND T.CodStatoPrescrizioneTempi <> 'CA'" + Environment.NewLine;

                    dtT = DataBase.GetDataTable(sSQL);

                    mod = new ModelloPrescrizione();

                    #region caricamento testata modello prescrizione

                    mod.ID = Guid.NewGuid();
                    mod.Descrizione = descrizione;
                    mod.CodTipoPrescrizione = dtP.Rows[0]["CodTipoPrescrizione"].ToString();
                    mod.CodViaSomministrazione = dtP.Rows[0]["CodViaSomministrazione"].ToString();
                    mod.CodiceScheda = dtP.Rows[0]["CodScheda"].ToString();
                    mod.VersioneScheda = (int)dtP.Rows[0]["Versione"];
                    mod.DatiXMLScheda = dtP.Rows[0]["Dati"].ToString();

                    mod.ModelliPrescrizionitempi = new List<ModelloPrescrizioneTempo>();

                    #region caricamento righe tempi

                    foreach (DataRow row in dtT.Rows)
                    {
                        modt = new ModelloPrescrizioneTempo();

                        modt.CodTipoPrescrizioneTempi = row["CodTipoPrescrizioneTempi"].ToString();
                        modt.Posologia = row["Posologia"].ToString();
                        modt.CodProtocollo = row["CodProtocollo"].ToString();
                        modt.AlBisogno = (bool)row["AlBisogno"];

                        DateTime datainizio, datafine;
                        if (DateTime.TryParse(row["DataOraInizio"].ToString(), out datainizio) &&
                            DateTime.TryParse(row["DataOraFine"].ToString(), out datafine))
                        {
                            modt.DataOraInizioOriginale = datainizio;
                            modt.DataOraFineOriginale = datafine;
                        }

                        modt.Durata = (int)row["Durata"];
                        modt.Continuita = (bool)row["Continuita"];
                        modt.PeriodicitaGiorni = (int)row["PeriodicitaGiorni"];
                        modt.PeriodicitaOre = (int)row["PeriodicitaOre"];
                        modt.PeriodicitaMinuti = (int)row["PeriodicitaMinuti"];
                        modt.Manuale = (bool)row["Manuale"];
                        modt.CodiceSchedaPosologia = row["CodScheda"].ToString();

                        if (row["Versione"] != null && row["Versione"] != DBNull.Value)
                            modt.VersioneSchedaPosologia = (int)row["Versione"];
                        else
                            modt.VersioneSchedaPosologia = null;

                        modt.DatiXMLSchedaPosologia = row["Dati"].ToString();

                        modt.ModelliTempiManuali = new List<ModelloTempoManuale>();

                        #region caricamento eventuali tempi manuali

                        if (row["TempiManuali"] != null && row["TempiManuali"] != DBNull.Value)
                        {
                            dsTM = new DataSet();
                            dsTM.ReadXml(GenerateStreamFromString(row["TempiManuali"].ToString()));

                            if (dsTM != null && dsTM.Tables.Count > 0)
                            {
                                foreach (DataRow rowtm in dsTM.Tables[0].Rows)
                                {
                                    IntervalloTempi inttempo = new IntervalloTempi(DateTime.Parse(rowtm["DataOraInizio"].ToString()), DateTime.Parse(rowtm["DataOraFine"].ToString()),
                                                                                    rowtm["NomeProtocollo"].ToString(), rowtm["EtichettaTempo"].ToString());

                                    modtm = new ModelloTempoManuale();

                                    modtm.DataOraInizioOriginale = inttempo.DataOraInizio;
                                    modtm.DataOraFineOriginale = inttempo.DataOraFine;
                                    modtm.DeltaDataOraInizio = 0;
                                    modtm.DeltaDataOraFine = 0;
                                    modtm.EtichettaTempo = inttempo.EtichettaTempo;
                                    modtm.NomeProtocollo = inttempo.NomeProtocollo;

                                    modt.ModelliTempiManuali.Add(modtm);
                                }
                            }
                        }
                        #endregion

                        mod.ModelliPrescrizionitempi.Add(modt);
                    }



                    #endregion

                    #endregion
                }

                _modelliPrescrizioni.Add(mod);
            }
            catch (Exception ex)
            {
                Framework.Diagnostics.Statics.AddDebugInfo(ex);
                bRet = false;
            }

            return bRet;
        }

        private void RicalcoloDeltaDate()
        {

            DateTime dataInizioPerDelta = DateTime.MinValue;

            try
            {

                foreach (var mod in _modelliPrescrizioni)
                {
                    foreach (var modp in mod.ModelliPrescrizionitempi)
                    {
                        if (dataInizioPerDelta == DateTime.MinValue)
                            dataInizioPerDelta = modp.DataOraInizioOriginale;
                        else
                            if (dataInizioPerDelta > modp.DataOraInizioOriginale && !modp.AlBisogno) dataInizioPerDelta = modp.DataOraInizioOriginale;
                    }
                }

                dataInizioPerDelta = DateTime.MinValue;

                foreach (var mod in _modelliPrescrizioni)
                {
                    foreach (var modp in mod.ModelliPrescrizionitempi)
                    {
                        if (modp.AlBisogno)
                        {
                            modp.DeltaDataOraInizio = 0;
                            modp.DeltaDataOraFine = 0;
                        }
                        else
                        {
                            if (dataInizioPerDelta == DateTime.MinValue)
                            {
                                dataInizioPerDelta = new DateTime(modp.DataOraInizioOriginale.Year, modp.DataOraInizioOriginale.Month, modp.DataOraInizioOriginale.Day);
                            }

                            modp.DeltaDataOraInizio = modp.DataOraInizioOriginale.Subtract(dataInizioPerDelta).TotalSeconds;
                            modp.DeltaDataOraFine = modp.DataOraFineOriginale.Subtract(dataInizioPerDelta).TotalSeconds;
                        }

                        if (modp.ModelliTempiManuali != null)
                            foreach (var modtm in modp.ModelliTempiManuali)
                            {
                                modtm.DeltaDataOraInizio = modtm.DataOraInizioOriginale.Subtract(dataInizioPerDelta).TotalSeconds;
                                modtm.DeltaDataOraFine = modtm.DataOraFineOriginale.Subtract(dataInizioPerDelta).TotalSeconds;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private Stream GenerateStreamFromString(string s)
        {
            try
            {
                if (s.Contains("NewDataSet"))
                {
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(s.Replace("<TempiManuali>", string.Empty).Replace("</TempiManuali>", string.Empty));
                    writer.Flush();
                    stream.Position = 0;
                    return stream;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return null;
            }
        }

        private bool DeleteModelloPrescrizione(string idPrescrizione)
        {
            bool bRet = true;

            try
            {
                ModelloPrescrizione delItem = _modelliPrescrizioni.Find(m => m.ID.ToString() == idPrescrizione);

                if (delItem != null)
                {
                    bRet = _modelliPrescrizioni.Remove(delItem);
                }
            }
            catch (Exception ex)
            {
                Framework.Diagnostics.Statics.AddDebugInfo(ex);
                bRet = false;
            }

            return bRet;
        }

        private bool CheckDataOraInizioObbligatoria()
        {

            bool bRet = false;

            try
            {
                if (_modelliPrescrizioni != null)
                {
                    foreach (var mod in _modelliPrescrizioni)
                    {
                        bRet = mod.ModelliPrescrizionitempi.Find(m => m.AlBisogno == false) != null;
                        if (bRet) break;
                    }
                }
                else
                    bRet = false;
            }
            catch (Exception ex)
            {
                Framework.Diagnostics.Statics.AddDebugInfo(ex);
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

        private void UltraGridPrescrizioni_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Exists("ID") == true)
            {
                e.Layout.Bands[0].Columns["ID"].Hidden = true;
            }
        }

        private void UltraToolbarManagerPrescrizioni_ToolClick(object sender, ToolClickEventArgs e)
        {

            string sMsg = string.Empty;


            try
            {
                UltraGridRow activeRow = null;
                if (this.UltraGridPrescrizioni.ActiveRow != null) activeRow = this.UltraGridPrescrizioni.ActiveRow;

                switch (e.Tool.Key)
                {
                    case MyStatics.GC_NUOVO:

                        frmPUProtocolliPrescrizioniInsert puins = new frmPUProtocolliPrescrizioniInsert();
                        puins.ViewIcon = this.ViewIcon;
                        puins.ViewImage = this.ViewImage;
                        puins.ViewText = "Inserimento modello prescrizione";
                        puins.ShowDialog();

                        if (puins.DialogResult == DialogResult.OK)
                            if (this.CheckIDPrescrizione(puins.IDPrescrizione))
                            {
                                if (InsertModelloPrescrizione(puins.IDPrescrizione, puins.Descrizione))
                                {
                                    LoadUltraGridPrescrizioni();
                                    SetUltraToolBarManager();
                                }
                                else
                                    MessageBox.Show(@"Si sono verificati errori durante il processo di inserimento.", "Inserimento modello prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                sMsg = "ID inserito non valido o non trovato in archivio";
                                MessageBox.Show(sMsg, "Inserimento modello prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        puins.Dispose();
                        puins = null;

                        break;

                    case MyStatics.GC_ELIMINA:
                        if (MessageBox.Show("Confermi la cancellazione ?", "Cancellazione modello prescrizione", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            if (DeleteModelloPrescrizione(this.UltraGridPrescrizioni.ActiveRow.Cells["ID"].Value.ToString()))
                            {
                                LoadUltraGridPrescrizioni();
                                SetUltraToolBarManager();
                            }
                            else
                                MessageBox.Show(@"Si sono verificati errori durante il processo di cancellazione.", "Cancellazione modello prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    case MyStatics.GC_STAMPA:
                        try
                        {
                            UltraGridPrescrizioni.PrintPreview(RowPropertyCategories.All);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(@"Si sono verificati errori durante il processo di stampa." + Environment.NewLine + ex.Message, @"Errore di stampa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    case MyStatics.GC_AGGIORNA:
                        LoadUltraGridPrescrizioni();
                        SetUltraToolBarManager();
                        break;

                    case MyStatics.GC_ESPORTA:
                        if (SaveFileDialog.ShowDialog() == DialogResult.OK) UltraGridExcelExporter.Export(UltraGridPrescrizioni, SaveFileDialog.FileName);
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
                            this.RicalcoloDeltaDate();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
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
                            this.RicalcoloDeltaDate();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
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
                            this.RicalcoloDeltaDate();
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
                            this.RicalcoloDeltaDate();
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
