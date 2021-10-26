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
    public partial class frmPUSchedeCopia : Form, Interfacce.IViewFormPUView
    {

        #region DECLARE

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;

        public string codiceNewScheda { get; set; }

        #endregion

        public frmPUSchedeCopia()
        {
            InitializeComponent();
        }

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
                return this.picView.Image;
            }
            set
            {
                this.picView.Image = value;
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

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.InitializeUltraToolbarsManager();
            this.InitializeUltraGrid();

            loadData();

            this.ResumeLayout();

        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.ultraToolbarsManagerForm);

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {
            MyStatics.SetUltraGridLayout(ref this.ugVersioni, false, false);


            this.ugVersioni.DisplayLayout.GroupByBox.Hidden = true;

            this.ugVersioni.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ugVersioni.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.ugVersioni.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ugVersioni.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.ugVersioni.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.ugVersioni.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.VisibleRows;
            this.ugVersioni.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.ExternalSortSingle;
            this.ugVersioni.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.ugVersioni.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.Sychronized;
            this.ugVersioni.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ugVersioni.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.ugVersioni.Text = "";
            this.ugVersioni.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;

        }

        private void LoadUltraGrid()
        {


            try
            {

                string sSql = @"Select CodScheda, Versione, Descrizione, FlagAttiva, Pubblicato, DtValI, DtValF
                                From T_SchedeVersioni
                                Where CodScheda = '" + DataBase.Ax2(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString()) + @"'
                                Order By Versione";

                DataSet dsVersioni = DataBase.GetDataSet(sSql).Copy();

                DataColumn colsel = dsVersioni.Tables[0].Columns.Add("Sel", Type.GetType(@"System.Boolean"));
                colsel.AllowDBNull = true;
                colsel.DefaultValue = false;
                colsel.ReadOnly = false;
                colsel.Unique = false;

                DataColumn colnewver = dsVersioni.Tables[0].Columns.Add("NVer", Type.GetType(@"System.Int32"));
                colnewver.AllowDBNull = true;
                colnewver.DefaultValue = 1;
                colnewver.ReadOnly = false;
                colnewver.Unique = false;

                DataColumn colnewdescr = dsVersioni.Tables[0].Columns.Add("NDescr", Type.GetType(@"System.String"));
                colnewdescr.AllowDBNull = true;
                colnewdescr.MaxLength = dsVersioni.Tables[0].Columns["Descrizione"].MaxLength;
                colnewdescr.DefaultValue = "";
                colnewdescr.ReadOnly = false;
                colnewdescr.Unique = false;

                for (int v = 0; v < dsVersioni.Tables[0].Rows.Count; v++)
                {
                    if (v < dsVersioni.Tables[0].Rows.Count - 1)
                    {
                        dsVersioni.Tables[0].Rows[v]["Sel"] = false;
                        dsVersioni.Tables[0].Rows[v]["NVer"] = System.DBNull.Value;
                        dsVersioni.Tables[0].Rows[v]["NDescr"] = "";
                    }
                    else
                    {
                        dsVersioni.Tables[0].Rows[v]["Sel"] = true;
                        dsVersioni.Tables[0].Rows[v]["NVer"] = 1;
                        dsVersioni.Tables[0].Rows[v]["NDescr"] = dsVersioni.Tables[0].Rows[v]["Descrizione"];
                    }
                }

                this.ugVersioni.DataMember = dsVersioni.Tables[0].TableName;
                this.ugVersioni.DataSource = dsVersioni;

                foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn oGCol in this.ugVersioni.DisplayLayout.Bands[0].Columns)
                {
                    switch (oGCol.Key)
                    {
                        case @"Sel":
                            oGCol.Header.Caption = "Copia";
                            oGCol.Header.VisiblePosition = 1;
                            oGCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oGCol.CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                            oGCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
                            oGCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                            break;

                        case @"Versione":
                            oGCol.Header.Caption = "Ver.";
                            oGCol.Header.VisiblePosition = 2;
                            oGCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                            oGCol.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                            oGCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                            break;

                        case @"Descrizione":
                            oGCol.Header.Caption = "Descr.";
                            oGCol.Header.VisiblePosition = 3;
                            oGCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                            oGCol.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
                            oGCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
                            break;

                        case @"FlagAttiva":
                            oGCol.Header.Caption = "Attiva";
                            oGCol.Header.VisiblePosition = 4;
                            oGCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oGCol.CellActivation = Infragistics.Win.UltraWinGrid.Activation.Disabled;
                            oGCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                            oGCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                            break;

                        case @"Pubblicato":
                            oGCol.Header.Caption = "Pubbl.";
                            oGCol.Header.VisiblePosition = 5;
                            oGCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oGCol.CellActivation = Infragistics.Win.UltraWinGrid.Activation.Disabled;
                            oGCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                            oGCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                            break;

                        case @"DtValI":
                            oGCol.Header.Caption = "Dal";
                            oGCol.Header.VisiblePosition = 6;
                            oGCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oGCol.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                            oGCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                            oGCol.Format = @"dd/MM/yyyy";
                            break;

                        case @"DtValF":
                            oGCol.Header.Caption = "Al";
                            oGCol.Header.VisiblePosition = 7;
                            oGCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oGCol.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                            oGCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                            oGCol.Format = @"dd/MM/yyyy";
                            break;

                        case @"NVer":
                            oGCol.Header.Caption = "Nuova Versione";
                            oGCol.Header.VisiblePosition = 8;
                            oGCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                            oGCol.CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                            oGCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
                            oGCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerPositive;
                            break;

                        case @"NDescr":
                            oGCol.Header.Caption = "Nuova Descrizione";
                            oGCol.Header.VisiblePosition = 9;
                            oGCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                            oGCol.CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                            oGCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
                            break;

                        default:
                            oGCol.Hidden = true;
                            oGCol.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                            break;
                    }
                }

                foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn oGCol in this.ugVersioni.DisplayLayout.Bands[0].Columns)
                {
                    switch (oGCol.Key)
                    {
                        case @"Sel":
                            oGCol.Header.VisiblePosition = 1;
                            break;

                        case @"Versione":
                            oGCol.Header.VisiblePosition = 2;
                            break;

                        case @"Descrizione":
                            oGCol.Header.VisiblePosition = 3;
                            break;

                        case @"FlagAttiva":
                            oGCol.Header.VisiblePosition = 4;
                            break;

                        case @"Pubblicato":
                            oGCol.Header.VisiblePosition = 5;
                            break;

                        case @"DtValI":
                            oGCol.Header.VisiblePosition = 6;
                            break;

                        case @"DtValF":
                            oGCol.Header.VisiblePosition = 7;
                            break;

                        case @"NVer":
                            oGCol.Header.VisiblePosition = 8;
                            break;

                        case @"NDescr":
                            oGCol.Header.VisiblePosition = 9;
                            break;

                        default:
                            break;
                    }
                }

                this.ugVersioni.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region void & functions

        private void loadData()
        {

            try
            {

                int iMaxL = 0;
                string sNewCodice = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString();
                if (_DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"].MaxLength > 0) iMaxL = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"].MaxLength;
                if (sNewCodice.Trim().Length >= iMaxL)
                    sNewCodice = "";
                else
                {
                    const string suff = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    bool bOK = false;
                    for (int i = 0; i < suff.Length; i++)
                    {
                        string sTmp = sNewCodice + suff.Substring(i, 1);
                        if (DataBase.GetDataSet(@"Select Codice From T_Schede Where codice = '" + DataBase.Ax2(sTmp) + "'").Tables[0].Rows.Count <= 0)
                        {
                            sNewCodice = sTmp;
                            bOK = true;
                            i = suff.Length + 1;
                        }
                    }
                    if (!bOK) sNewCodice = "";
                }

                this.uteCodice.Text = sNewCodice;
                if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Descrizione"))
                    this.uteDescrizione.Text = @"Copia di " + _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Descrizione"].ToString();

                this.chkCopiaPadri.Checked = false;
                this.chkCopiaFigli.Checked = false;
                this.chkCopiaUA.Checked = false;
                this.chkCopiaVersione.Checked = true;

                LoadUltraGrid();

            }
            catch (Exception)
            {
            }

        }

        private bool checkInput()
        {
            bool bReturn = true;
            string sSql = "";

            if (bReturn)
            {
                if (this.uteCodice.Text.Trim() == "")
                {
                    bReturn = false;
                    MessageBox.Show(@"Inserire Codice!", "Copia Scheda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteCodice.Focus();
                }
            }
            if (bReturn)
            {
                sSql = @"Select Codice From T_Schede Where Codice = '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"'";
                if (DataBase.GetDataSet(sSql).Tables[0].Rows.Count > 0)
                {
                    bReturn = false;
                    MessageBox.Show(@"Il codice """ + this.uteCodice.Text + @""" è già utilizzato!", "Copia Scheda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteCodice.Focus();
                }
            }

            if (bReturn)
            {
                if (this.uteDescrizione.Text.Trim() == "")
                {
                    bReturn = false;
                    MessageBox.Show(@"Inserire Descrizione!", "Copia Scheda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteDescrizione.Focus();
                }
            }

            if (bReturn)
            {
                if (this.chkCopiaVersione.Checked && this.ugVersioni.Rows.Count > 0)
                {
                    int iSel = 0;
                    for (int r = 0; r < this.ugVersioni.Rows.Count; r++)
                    {
                        if ((bool)this.ugVersioni.Rows[r].Cells["Sel"].Value) iSel += 1;
                    }

                    if (iSel != 1)
                    {
                        bReturn = false;
                        MessageBox.Show(@"Selezionare una ed una sola versione da copiare!", "Copia Scheda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione.Focus();
                    }

                    if (bReturn)
                    {
                        string sMsg = "";
                        for (int r = 0; r < this.ugVersioni.Rows.Count; r++)
                        {
                            if ((bool)this.ugVersioni.Rows[r].Cells["Sel"].Value)
                            {
                                if (this.ugVersioni.Rows[r].Cells["NVer"].Text.Trim() == "")
                                {
                                    if (sMsg != "") sMsg += Environment.NewLine;
                                    sMsg += @"Inserire un valore per la versione!";
                                }
                                else
                                {
                                    int iVer = 0;
                                    if (int.TryParse(this.ugVersioni.Rows[r].Cells["NVer"].Text.Trim(), out iVer))
                                    {
                                        if (iVer <= 0)
                                        {
                                            if (sMsg != "") sMsg += Environment.NewLine;
                                            sMsg += @"Inserire un numero intero positivo per la versione!";
                                        }
                                    }
                                    else
                                    {
                                        if (sMsg != "") sMsg += Environment.NewLine;
                                        sMsg += @"Inserire un numero intero positivo per la versione!";
                                    }
                                }

                                if (this.ugVersioni.Rows[r].Cells["NDescr"].Text.Trim() == "")
                                {
                                    if (sMsg != "") sMsg += Environment.NewLine;
                                    sMsg += @"Inserire un valore per la descrizione della nuova versione!";
                                }

                            }
                        }

                        if (sMsg != "")
                        {
                            bReturn = false;
                            MessageBox.Show(sMsg, "Copia Scheda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }

                }
            }

            if (bReturn)
            {
                string sCheck = "";
                if (!this.chkCopiaPadri.Checked)
                {
                    if (sCheck != "") sCheck += Environment.NewLine;
                    sCheck += @"- " + this.chkCopiaPadri.Text;
                }
                if (!this.chkCopiaFigli.Checked)
                {
                    if (sCheck != "") sCheck += Environment.NewLine;
                    sCheck += @"- " + this.chkCopiaFigli.Text;
                }
                if (!this.chkCopiaSchedeCopia.Checked)
                {
                    if (sCheck != "") sCheck += Environment.NewLine;
                    sCheck += @"- " + this.chkCopiaSchedeCopia.Text;
                }
                if (!this.chkCopiaUA.Checked)
                {
                    if (sCheck != "") sCheck += Environment.NewLine;
                    sCheck += @"- " + this.chkCopiaUA.Text;
                }
                if (!this.chkCopiaVersione.Checked && this.ugVersioni.Rows.Count > 0)
                {
                    if (sCheck != "") sCheck += Environment.NewLine;
                    sCheck += @"- " + this.chkCopiaVersione.Text;
                }

                if (sCheck != "")
                {
                    if (MessageBox.Show(@"Non hai selezionato:" + Environment.NewLine + sCheck + Environment.NewLine + @"Vuoi proseguire ugualmente con la copia?", "Copia Scheda", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                    {
                        bReturn = false;
                    }
                }
            }

            return bReturn;
        }

        private bool copyScheda()
        {
            bool bReturn = true;

            try
            {

                string sSql = "";

                sSql += @" INSERT INTO T_Schede
                               (Codice
                               ,Descrizione
                               ,Note
                               ,Path
                               ,CodTipoScheda
                               ,SchedaSemplice
                               ,CodEntita
                               ,CodEntita2
                               ,Ordine
                               ,NumerositaMinima
                               ,NumerositaMassima
                               ,CreaDefault
                               ,EsportaDWH                               
                               ,IgnoraStampaCartella
                               ,Validabile
                               ,Riservata
                               ,EsportaDWHSingola
                               ,CodModalitaCopiaPrecedente
                               ,CodEntita3
                               ,SistemaDWH
                               ,Revisione
                               ,CodPrestazioneDWH
                               ,DescrizionePrestazioneDWH
                               ,Contenitore
                               ,AlertSchedaVuota
                               ,CopiaPrecedenteSelezione
                               ,FirmaDigitale
                               ,CodReportDWH
                               ,CancellaPrecedentiDWH
                               ,CartellaAmbulatorialeCodificata
                               ,DescrizioneAlternativa
                               ,CodContatore
                               ,EsportaLayerDWH
                               )
                         SELECT '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"'
                               ,'" + DataBase.Ax2(this.uteDescrizione.Text.Trim()) + @"'
                               ,Note
                               ,Path
                               ,CodTipoScheda
                               ,SchedaSemplice
                               ,CodEntita
                               ,CodEntita2
                               ,Ordine
                               ,NumerositaMinima
                               ,NumerositaMassima
                               ,CreaDefault
                               ,EsportaDWH                  
                               ,IgnoraStampaCartella
                               ,Validabile
                               ,Riservata
                               ,EsportaDWHSingola
                               ,CodModalitaCopiaPrecedente
                               ,CodEntita3
                               ,SistemaDWH
                               ,Revisione
                               ,CodPrestazioneDWH
                               ,DescrizionePrestazioneDWH
                               ,Contenitore
                               ,AlertSchedaVuota
                               ,CopiaPrecedenteSelezione
                               ,FirmaDigitale
                               ,CodReportDWH
                               ,CancellaPrecedentiDWH
                               ,CartellaAmbulatorialeCodificata
                               ,DescrizioneAlternativa
                               ,CodContatore
                               ,EsportaLayerDWH
                        FROM T_Schede
                        WHERE Codice = '" + DataBase.Ax2(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString()) + @"' " + Environment.NewLine;


                if (this.chkCopiaPadri.Checked)
                {
                    sSql += Environment.NewLine;
                    sSql += @" INSERT INTO [dbo].[T_SchedePadri]
                                    ([CodScheda]
                                    ,[CodSchedaPadre])
                                Select '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"'
                                    ,[CodSchedaPadre]
                                From [dbo].[T_SchedePadri]
                                Where CodScheda = '" + DataBase.Ax2(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString()) + @"' " + Environment.NewLine;

                }

                if (this.chkCopiaFigli.Checked)
                {
                    sSql += Environment.NewLine;
                    sSql += @" INSERT INTO [dbo].[T_SchedePadri]
                                    ([CodScheda]
                                    ,[CodSchedaPadre])
                                Select [CodScheda]
                                    ,'" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"'
                                From [dbo].[T_SchedePadri]
                                Where CodSchedaPadre = '" + DataBase.Ax2(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString()) + @"' " + Environment.NewLine;


                }

                if (this.chkCopiaSchedeCopia.Checked)
                {
                    sSql += Environment.NewLine;
                    sSql += @" INSERT INTO [dbo].[T_SchedeCopia]
                                    ([CodScheda]
                                    ,[CodSchedaCopia])
                                Select '" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"'
                                    , [CodSchedaCopia]
                                From [dbo].[T_SchedeCopia]
                                Where CodScheda = '" + DataBase.Ax2(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString()) + @"' " + Environment.NewLine;


                }

                if (this.chkCopiaUA.Checked)
                {
                    sSql += Environment.NewLine;
                    sSql += @" INSERT INTO [dbo].[T_AssUAEntita]
                                       ([CodUA]
                                       ,[CodEntita]
                                       ,[CodVoce])
                                 Select [CodUA]
                                       ,[CodEntita]
                                       ,'" + DataBase.Ax2(this.uteCodice.Text.Trim()) + @"'
                                 From [dbo].[T_AssUAEntita]
                                 Where [CodEntita] = '" + Scci.Enums.EnumEntita.SCH.ToString() + @"' 
                                    And [CodVoce] = '" + DataBase.Ax2(_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Codice"].ToString()) + @"' " + Environment.NewLine;


                }

                if (this.chkCopiaVersione.Checked)
                {
                    string sCodSchedaOrigine;
                    string sVersioneOrigine;

                    string sCodSchedaDestinazione;
                    string sVersioneDestinazione;
                    string sDescrizioneDestinazione;

                    sCodSchedaDestinazione = DataBase.Ax2(this.uteCodice.Text.Trim());

                    ;

                    for (int v = 0; v < this.ugVersioni.Rows.Count; v++)
                    {

                        if ((bool)this.ugVersioni.Rows[v].Cells["Sel"].Value)
                        {

                            sCodSchedaOrigine = DataBase.Ax2(this.ugVersioni.Rows[v].Cells["CodScheda"].Value.ToString());
                            sVersioneOrigine = this.ugVersioni.Rows[v].Cells["Versione"].Value.ToString();

                            sVersioneDestinazione = this.ugVersioni.Rows[v].Cells["NVer"].Value.ToString();

                            sDescrizioneDestinazione = DataBase.Ax2(this.ugVersioni.Rows[v].Cells["NDescr"].Value.ToString());

                            sSql += Environment.NewLine;
                            sSql += @" INSERT INTO [dbo].[T_SchedeVersioni]
                                               ([CodScheda]
                                               ,[Versione]
                                               ,[Descrizione]
                                               ,[FlagAttiva]
                                               ,[Pubblicato]
                                               ,[DtValI]
                                               ,[DtValF]
                                               ,[CampiRilevanti]
                                               ,[CampiObbligatori]                                               
                                               ,[Struttura]
                                               ,[Layout])
                                        Select '" + sCodSchedaDestinazione + @"'
                                               ," + sVersioneDestinazione + @"
                                               ,'" + sDescrizioneDestinazione + @"'
                                               ,[FlagAttiva]
                                               ,[Pubblicato]
                                               ,[DtValI]
                                               ,[DtValF]
                                               ,[CampiRilevanti]
                                               ,[CampiObbligatori]                                               
                                               ,[Struttura]
                                               ,[Layout] 
                                        From [dbo].[T_SchedeVersioni]
                                        Where CodScheda = '" + sCodSchedaOrigine + @"'
                                            And Versione = " + sVersioneOrigine + @" " + Environment.NewLine;

                            sSql += Environment.NewLine;
                            sSql += @"UPDATE T_SchedeVersioni
                                      SET Struttura.modify('replace value of (/DcScheda/ID/text())[1] with """ + sCodSchedaDestinazione + @"""')
                                     WHERE
                                        CodScheda = '" + sCodSchedaDestinazione + @"' AND
                                        Versione = " + sVersioneDestinazione;
                            v = this.ugVersioni.Rows.Count + 1;
                        }
                    }
                }


                if (sSql != "")
                {
                    bReturn = DataBase.ExecuteSql(sSql);
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                MyStatics.ExGest(ref ex, "copyScheda", this.Name);
            }

            return bReturn;
        }

        #endregion

        #region EVENTI

        private void frmPUSchedeCopia_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.ugVersioni != null) this.ugVersioni.Dispose();
        }

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Hide();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {
            try
            {
                this.ugVersioni.UpdateData();

                if (checkInput())
                {
                    if (copyScheda())
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        codiceNewScheda = this.uteCodice.Text;
                        this.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, @"ubConferma_Click", this.Name);
            }
        }

        #endregion

    }
}
