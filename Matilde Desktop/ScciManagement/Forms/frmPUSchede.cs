using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Statics;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUSchede : Form, Interfacce.IViewFormPUView
    {
        public frmPUSchede()
        {
            InitializeComponent();
        }

        #region Declare

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;
        private DialogResult _DialogResult = DialogResult.Cancel;

        private bool _runtime = false;



        private const bool C_EnableEPI_PAZ_Figli = false;

        private const bool C_EnableEPI_PAZ_Padri = false;

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
            this.InitializeUltraGrid();
            this.InitializeUltraCombo();

            SetBindings();

            switch (_Modality)
            {

                case Enums.EnumModalityPopUp.mpNuovo:
                    this.UltraTabControl.Tabs["tab2"].Visible = false;
                    this.UltraTabControl.Tabs["tab4"].Visible = false;
                    this.UltraTabControl.Tabs["tab5"].Visible = false;
                    this.UltraTabControl.Tabs["tab6"].Visible = false;
                    this.UltraTabControl.Tabs["tab7"].Visible = false;
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.ubApplica.Enabled = true;
                    break;

                case Enums.EnumModalityPopUp.mpModifica:
                    SetBindingsAss();
                    this.UltraTabControl.Tabs["tab5"].Visible = true;
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.uteCodice.Enabled = false;
                    this.ubApplica.Enabled = false;
                    break;

                case Enums.EnumModalityPopUp.mpCancella:
                    SetBindingsAss();
                    this.UltraTabControl.Tabs["tab5"].Visible = true;
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    break;

                case Enums.EnumModalityPopUp.mpVisualizza:
                    SetBindingsAss();
                    this.UltraTabControl.Tabs["tab5"].Visible = true;
                    MyStatics.SetControls(this.UltraGroupBox.Controls, false);
                    this.ubApplica.Enabled = false;
                    this.ubConferma.Enabled = false;
                    break;

                default:
                    break;

            }

            this.SetRevisione();
            this.SetCartellaAmbulatorialeCodificata();
            switchNumerositaMinima();

            ControlloAbilitazioneDWH();

            this.ResumeLayout();

        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);

                foreach (ToolBase oTool in this.UltraToolbarsManagerGrid.Tools)
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
            bool bAggiorna = true;
            bool bCopia = true;
            bool bConversione = true;

            if ((this.UltraGridVersioni.Rows.Count > 0 && this.UltraGridVersioni.ActiveRow != null) && this.UltraGridVersioni.ActiveRow.IsDataRow)
            {
                bNuovo = (this.ViewModality == Enums.EnumModalityPopUp.mpModifica ? true : false);
                bModifica = (this.ViewModality == Enums.EnumModalityPopUp.mpModifica ? true : false);
                bElimina = (this.ViewModality == Enums.EnumModalityPopUp.mpModifica ? true : false);
                bCopia = (this.ViewModality == Enums.EnumModalityPopUp.mpModifica ? true : false);
                bConversione = (this.ViewModality == Enums.EnumModalityPopUp.mpModifica ? true : false);
                bVisualizza = true;
            }
            else
            {
                bNuovo = (this.ViewModality == Enums.EnumModalityPopUp.mpModifica ? true : false);
                bModifica = false;
                bElimina = false;
                bVisualizza = false;
                bCopia = false;
                bConversione = false;
            }

            this.UltraToolbarsManagerGrid.Tools[MyStatics.GC_NUOVO].SharedProps.Enabled = bNuovo;
            this.UltraToolbarsManagerGrid.Tools[MyStatics.GC_MODIFICA].SharedProps.Enabled = bModifica;
            this.UltraToolbarsManagerGrid.Tools[MyStatics.GC_ELIMINA].SharedProps.Enabled = bElimina;
            this.UltraToolbarsManagerGrid.Tools[MyStatics.GC_VISUALIZZA].SharedProps.Enabled = bVisualizza;
            this.UltraToolbarsManagerGrid.Tools[MyStatics.GC_AGGIORNA].SharedProps.Enabled = bAggiorna;
            this.UltraToolbarsManagerGrid.Tools[MyStatics.GC_COPIA].SharedProps.Enabled = bCopia;
            this.UltraToolbarsManagerGrid.Tools[MyStatics.GC_CONVERSIONE].SharedProps.Enabled = bConversione;

        }

        private void ActionGridToolClick(ToolBase oTool)
        {

            UltraGridRow activeRow = null;
            if (this.UltraGridVersioni.ActiveRow != null) activeRow = this.UltraGridVersioni.ActiveRow;
            switch (oTool.Key)
            {
                case MyStatics.GC_NUOVO:
                    if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_SchedeVersioni, Enums.EnumModalityPopUp.mpNuovo, this.ViewIcon, this.ViewImage, this.ViewText, "", "", this.uteCodice.Text) == DialogResult.OK)
                    {
                        this.LoadUltraGrid();
                        this.SetUltraToolBarManager();
                    }
                    break;

                case MyStatics.GC_MODIFICA:
                    if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_SchedeVersioni, Enums.EnumModalityPopUp.mpModifica, this.ViewIcon, this.ViewImage, this.ViewText, ref activeRow) == DialogResult.OK)
                    {
                        this.LoadUltraGrid();
                        this.SetUltraToolBarManager();
                    }
                    break;

                case MyStatics.GC_ELIMINA:
                    if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_SchedeVersioni, Enums.EnumModalityPopUp.mpCancella, this.ViewIcon, this.ViewImage, this.ViewText, ref activeRow) == DialogResult.OK)
                    {
                        this.LoadUltraGrid();
                        this.SetUltraToolBarManager();
                    }
                    break;

                case MyStatics.GC_VISUALIZZA:
                    MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_SchedeVersioni, Enums.EnumModalityPopUp.mpVisualizza, this.ViewIcon, this.ViewImage, this.ViewText, ref activeRow);
                    break;

                case MyStatics.GC_AGGIORNA:
                    this.UltraGridVersioni.Rows.ColumnFilters.ClearAllFilters();
                    this.LoadUltraGrid();
                    this.SetUltraToolBarManager();
                    break;

                case MyStatics.GC_COPIA:
                    this.CopyVersion();
                    this.UltraGridVersioni.Rows.ColumnFilters.ClearAllFilters();
                    this.LoadUltraGrid();
                    this.SetUltraToolBarManager();
                    break;

                case MyStatics.GC_CONVERSIONE:
                    this.Conversione();
                    break;

                default:
                    break;

            }

        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {
            MyStatics.SetUltraGridLayout(ref this.UltraGridVersioni, true, false);
        }

        private void LoadUltraGrid()
        {

            int nIndex = -1;
            string sSql = @"";

            try
            {

                if (this.UltraGridVersioni.ActiveRow != null) nIndex = this.UltraGridVersioni.ActiveRow.Index;

                sSql = DataBase.GetSqlView(Enums.EnumDataNames.T_SchedeVersioni) + Environment.NewLine +
                        "Where CodScheda = '" + this.uteCodice.Text + "'" + Environment.NewLine +
                        "Order By Versione, Descrizione";
                this.UltraGridVersioni.DataSource = DataBase.GetDataSet(sSql);
                this.UltraGridVersioni.Refresh();
                this.UltraGridVersioni.Text = string.Format("{0} ({1:#,##0})", this.Text, this.UltraGridVersioni.Rows.Count);


                if (this.UltraGridVersioni.DisplayLayout.Bands[0].Columns.Exists("Struttura_OLD"))
                    this.UltraGridVersioni.DisplayLayout.Bands[0].Columns["Struttura_OLD"].Hidden = true;
                if (this.UltraGridVersioni.DisplayLayout.Bands[0].Columns.Exists("LayOut_OLD"))
                    this.UltraGridVersioni.DisplayLayout.Bands[0].Columns["LayOut_OLD"].Hidden = true;

                if (this.UltraGridVersioni.DisplayLayout.Bands[0].Columns.Exists("LayoutV3"))
                    this.UltraGridVersioni.DisplayLayout.Bands[0].Columns["LayoutV3"].Hidden = true;

                if (this.UltraGridVersioni.DisplayLayout.Bands[0].Columns.Exists("StrutturaV3"))
                    this.UltraGridVersioni.DisplayLayout.Bands[0].Columns["StrutturaV3"].Hidden = true;

                this.UltraGridVersioni.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

                if (nIndex != -1)
                {
                    try
                    {
                        this.UltraGridVersioni.ActiveRow = this.UltraGridVersioni.Rows[nIndex];
                    }
                    catch (Exception)
                    {
                        if (this.UltraGridVersioni.Rows.Count > 0)
                            this.UltraGridVersioni.ActiveRow = this.UltraGridVersioni.Rows[this.UltraGridVersioni.Rows.Count - 1];
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Subroutine

        private void InitializeUltraCombo()
        {

            string sSql = string.Empty;
            DataSet oDs = null;

            try
            {

                MyStatics.SetUltraComboEditorLayout(ref this.uceCodModalitaCopiaPrecedente);
                this.uceCodModalitaCopiaPrecedente.ValueMember = "Codice";
                this.uceCodModalitaCopiaPrecedente.DisplayMember = "Descrizione";
                sSql = DataBase.GetSqlPUView(Enums.EnumDataNames.T_ModalitaCopiaPrecedente) + " ORDER BY Descrizione";
                oDs = DataBase.GetDataSet(sSql);
                this.uceCodModalitaCopiaPrecedente.DataMember = oDs.Tables[0].TableName;
                this.uceCodModalitaCopiaPrecedente.DataSource = oDs;
                this.uceCodModalitaCopiaPrecedente.DataBind();

                MyStatics.SetUltraComboEditorLayout(ref this.uceCodContatore);
                this.uceCodContatore.ValueMember = "Codice";
                this.uceCodContatore.DisplayMember = "Descrizione";
                sSql = "Select '' AS Codice, '' AS Descrizione UNION ALL " +
        "Select Codice, Descrizione From T_Contatori " +
        "ORDER BY Descrizione";
                oDs = DataBase.GetDataSet(sSql);
                this.uceCodContatore.DataMember = oDs.Tables[0].TableName;
                this.uceCodContatore.DataSource = oDs;
                this.uceCodContatore.DataBind();

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "InitializeUltraCombo", this.Name);
            }

        }

        private void SetBindings()
        {
            _runtime = true;
            DataColumn _dcol = null;

            try
            {

                _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice);
                _DataBinds.DataBindings.Add("Checked", "SchedaSemplice", this.chkSchedaSemplice);
                _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione);
                _DataBinds.DataBindings.Add("Text", "DescrizioneAlternativa", this.uteDescrizioneAlternativa);
                _DataBinds.DataBindings.Add("Text", "CodTipoScheda", this.uteCodTipoScheda);
                _DataBinds.DataBindings.Add("Text", "Note", this.uteNote);
                if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("SchedaSemplice") || !(bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["SchedaSemplice"])
                {

                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Path"))
                        this.utePath.Text = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Path"].ToString();

                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Ordine"))
                        this.uteOrdine.Text = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Ordine"].ToString();

                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("NumerositaMinima"))
                        this.umeNumerositaMinima.Value = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["NumerositaMinima"];

                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("NumerositaMassima"))
                        this.umeNumerositaMassima.Value = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["NumerositaMassima"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("CreaDefault"))
                        this.chkCreaDefault.Checked = false;
                    else
                        this.chkCreaDefault.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["CreaDefault"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("EsportaDWH"))
                        this.chkEsportaDWH.Checked = false;
                    else
                        this.chkEsportaDWH.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["EsportaDWH"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("EsportaDWHSingola"))
                        this.chkEsportaDWHSingola.Checked = false;
                    else
                        this.chkEsportaDWHSingola.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["EsportaDWHSingola"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("IgnoraStampaCartella"))
                        this.chkIgnoraStampaCartella.Checked = false;
                    else
                        this.chkIgnoraStampaCartella.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["IgnoraStampaCartella"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Validabile"))
                        this.chkValidabile.Checked = false;
                    else
                        this.chkValidabile.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Validabile"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Revisione"))
                        this.chkRevisione.Checked = false;
                    else
                        this.chkRevisione.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Revisione"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("FirmaDigitale"))
                        this.chkFirmaDigitale.Checked = false;
                    else
                        this.chkFirmaDigitale.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["FirmaDigitale"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Riservata"))
                        this.chkRiservata.Checked = false;
                    else
                        this.chkRiservata.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Riservata"];


                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("CodModalitaCopiaPrecedente"))
                        this.uceCodModalitaCopiaPrecedente.Value = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["CodModalitaCopiaPrecedente"];

                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("SistemaDWH"))
                        this.uteSistemaDWH.Text = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["SistemaDWH"].ToString();

                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("CodReportDWH"))
                        this.uteCodReportDWH.Text = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["CodReportDWH"].ToString();

                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("CodPrestazioneDWH"))
                        this.uteCodPrestazioneDWH.Text = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["CodPrestazioneDWH"].ToString();

                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("DescrizionePrestazioneDWH"))
                        this.uteDescrizionePrestazioneDWH.Text = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["DescrizionePrestazioneDWH"].ToString();

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Contenitore"))
                        this.chkContenitore.Checked = false;
                    else
                        this.chkContenitore.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Contenitore"];

                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("CodContatore"))
                        this.uceCodContatore.Value = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["CodContatore"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("AlertSchedaVuota"))
                        this.chkAlertSchedaVuota.Checked = false;
                    else
                        this.chkAlertSchedaVuota.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["AlertSchedaVuota"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("CopiaPrecedenteSelezione"))
                        this.chkCopiaPrecedenteSelezione.Checked = false;
                    else
                        this.chkCopiaPrecedenteSelezione.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["CopiaPrecedenteSelezione"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("CartellaAmbulatorialeCodificata"))
                        this.chkCartellaAmbulatorialeCodificata.Checked = false;
                    else
                        this.chkCartellaAmbulatorialeCodificata.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["CartellaAmbulatorialeCodificata"];

                    if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("EsportaLayerDWH"))
                        this.chkEsportaLayerDWH.Checked = false;
                    else
                        this.chkEsportaLayerDWH.Checked = (bool)_DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["EsportaLayerDWH"];

                }



                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                if (_dcol.MaxLength > 0) this.uteCodice.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                if (_dcol.MaxLength > 0) this.uteDescrizione.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["DescrizioneAlternativa"];
                if (_dcol.MaxLength > 0) this.uteDescrizioneAlternativa.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodTipoScheda"];
                if (_dcol.MaxLength > 0) this.uteCodTipoScheda.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodEntita"];
                if (_dcol.MaxLength > 0) this.uteCodEntita.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Note"];
                if (_dcol.MaxLength > 0) this.uteNote.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Ordine"];
                if (_dcol.MaxLength > 0) this.uteOrdine.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Path"];
                if (_dcol.MaxLength > 0) this.utePath.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["SistemaDWH"];
                if (_dcol.MaxLength > 0) this.uteSistemaDWH.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodReportDWH"];
                if (_dcol.MaxLength > 0) this.uteCodReportDWH.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodPrestazioneDWH"];
                if (_dcol.MaxLength > 0) this.uteCodPrestazioneDWH.MaxLength = _dcol.MaxLength;
                _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["DescrizionePrestazioneDWH"];
                if (_dcol.MaxLength > 0) this.uteDescrizionePrestazioneDWH.MaxLength = _dcol.MaxLength;

                _DataBinds.DataBindings.Load();

            }
            catch (Exception)
            {

            }

            try
            {
                switchAccessoriaStandard();
                bindEntita();
            }
            catch (Exception)
            {
            }

            _runtime = false;

        }
        private void SetBindingsAss()
        {

            string sSql = @"";
            string sTmp = "";
            try
            {

                this.ucMultiSelectPadri.ViewShowAll = true;
                this.ucMultiSelectPadri.ViewShowFind = true;

                sSql = @" Select Codice, Descrizione + ' (' + Codice + ')' As Schede " + Environment.NewLine;
                if (C_EnableEPI_PAZ_Padri) sSql += @"  , CONVERT(bit,0) AS AbilitaEPI, CONVERT(bit,0) AS AbilitaPAZ " + Environment.NewLine;
                sSql += @" From T_Schede" + Environment.NewLine;
                sSql += @" Where ISNULL(SchedaSemplice,0)=0 AND Codice {0} (Select CodSchedaPadre From T_SchedePadri Where CodScheda = '{1}') ORDER BY  Descrizione ASC ";
                sSql = string.Format(sSql, "Not In", this.uteCodice.Text);
                this.ucMultiSelectPadri.ViewDataSetSX = DataBase.GetDataSet(sSql);


                sSql = " Select SP.CodSchedaPadre As Codice, S.Descrizione + ' (' + Codice + ')' As Schede " + Environment.NewLine;
                if (C_EnableEPI_PAZ_Padri) sSql += "   , IsNull(SP.AbilitaEPI,0) As AbilitaEPI, IsNull(SP.AbilitaPAZ,0) As AbilitaPAZ " + Environment.NewLine;
                sSql += " From T_SchedePadri SP" + Environment.NewLine;
                sSql += @" Inner Join T_Schede S On SP.CodSchedaPadre = S.Codice" + Environment.NewLine;
                sSql += @" Where SP.CodScheda {0} '{1}'";
                sSql = string.Format(sSql, "=", this.uteCodice.Text);
                DataSet oDs = DataBase.GetDataSet(sSql);

                DataSet oDsNew = new DataSet();
                oDsNew.Tables.Add(new DataTable());
                oDsNew.Tables[0].Columns.Add("Codice", typeof(string));
                oDsNew.Tables[0].Columns.Add("Schede", typeof(string));
                if (C_EnableEPI_PAZ_Padri)
                {
                    oDsNew.Tables[0].Columns.Add("AbilitaEPI", typeof(bool));
                    oDsNew.Tables[0].Columns.Add("AbilitaPAZ", typeof(bool));
                }
                foreach (DataRow orow in oDs.Tables[0].Rows)
                {
                    oDsNew.Tables[0].ImportRow(orow);
                }
                this.ucMultiSelectPadri.ViewDataSetDX = oDsNew;
                this.ucMultiSelectPadri.ViewInit();
                this.ucMultiSelectPadri.RefreshData();
                this.ucMultiSelectPadri.GridDX.EventManager.SetEnabled(GridEventIds.DoubleClickRow, false);



                this.ucMultiSelectFigli.ViewShowAll = true;
                this.ucMultiSelectFigli.ViewShowFind = true;
                sTmp = "";
                if (C_EnableEPI_PAZ_Figli) sTmp = " , CONVERT(bit,0) AS AbilitaEPI, CONVERT(bit,0) AS AbilitaPAZ ";
                sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As Schede " + sTmp + " From T_Schede" + Environment.NewLine +
                                        "Where ISNULL(SchedaSemplice,0)=0 AND Codice {0} (Select CodScheda From T_SchedePadri Where CodSchedaPadre = '{1}') ORDER BY  Descrizione ASC ", "Not In", this.uteCodice.Text);
                this.ucMultiSelectFigli.ViewDataSetSX = DataBase.GetDataSet(sSql);
                sTmp = "";
                if (C_EnableEPI_PAZ_Figli) sTmp = " , IsNull(SP.AbilitaEPI,0) As AbilitaEPI, IsNull(SP.AbilitaPAZ,0) As AbilitaPAZ ";
                sSql = string.Format("Select SP.CodScheda As Codice, S.Descrizione + ' (' + Codice + ')'  As Schede " + sTmp + " From T_SchedePadri SP" + Environment.NewLine +
                                        "Inner Join T_Schede S On SP.CodScheda = S.Codice" + Environment.NewLine +
                                        "Where  ISNULL(SchedaSemplice,0)=0 AND SP.CodSchedaPadre {0} '{1}' ORDER BY  S.Descrizione ASC", "=", this.uteCodice.Text);
                oDs = DataBase.GetDataSet(sSql);
                oDsNew = new DataSet();
                oDsNew.Tables.Add(new DataTable());
                oDsNew.Tables[0].Columns.Add("Codice", typeof(string));
                oDsNew.Tables[0].Columns.Add("Schede", typeof(string));
                if (C_EnableEPI_PAZ_Figli)
                {
                    oDsNew.Tables[0].Columns.Add("AbilitaEPI", typeof(bool));
                    oDsNew.Tables[0].Columns.Add("AbilitaPAZ", typeof(bool));
                }
                foreach (DataRow orow in oDs.Tables[0].Rows)
                {
                    oDsNew.Tables[0].ImportRow(orow);
                }
                this.ucMultiSelectFigli.ViewDataSetDX = oDsNew;
                this.ucMultiSelectFigli.ViewInit();
                this.ucMultiSelectFigli.RefreshData();
                this.ucMultiSelectFigli.GridDX.EventManager.SetEnabled(GridEventIds.DoubleClickRow, false);



                this.ucMultiSelectCopiaDa.ViewShowAll = true;
                this.ucMultiSelectCopiaDa.ViewShowFind = true;
                sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' AS Schede From T_Schede" + Environment.NewLine +
                                        "Where ISNULL(SchedaSemplice,0)=0 AND Codice <> '{1}' AND Codice {0} (Select CodSchedaCopia From T_SchedeCopia Where CodScheda = '{1}') ORDER BY  Descrizione ASC ", "Not In", Database.testoSQL(this.uteCodice.Text));
                this.ucMultiSelectCopiaDa.ViewDataSetSX = DataBase.GetDataSet(sSql);

                sSql = string.Format("Select SC.CodSchedaCopia As Codice, S.Descrizione + ' (' + Codice + ')' AS Schede " + sTmp + " From T_SchedeCopia SC" + Environment.NewLine +
                                        "Inner Join T_Schede S On SC.CodSchedaCopia = S.Codice" + Environment.NewLine +
                                        "Where  ISNULL(S.SchedaSemplice,0)=0 AND SC.CodScheda {0} '{1}' ORDER BY  S.Descrizione ASC", "=", Database.testoSQL(this.uteCodice.Text));
                oDs = DataBase.GetDataSet(sSql);
                oDsNew = new DataSet();
                oDsNew.Tables.Add(new DataTable());
                oDsNew.Tables[0].Columns.Add("Codice", typeof(string));
                oDsNew.Tables[0].Columns.Add("Schede", typeof(string));
                foreach (DataRow orow in oDs.Tables[0].Rows)
                {
                    oDsNew.Tables[0].ImportRow(orow);
                }
                this.ucMultiSelectCopiaDa.ViewDataSetDX = oDsNew;
                this.ucMultiSelectCopiaDa.ViewInit();
                this.ucMultiSelectCopiaDa.RefreshData();
                this.ucMultiSelectCopiaDa.GridDX.EventManager.SetEnabled(GridEventIds.DoubleClickRow, false);



                this.UltraToolbarsManagerGrid_ToolClick(this.UltraToolbarsManagerGrid, new ToolClickEventArgs(this.UltraToolbarsManagerGrid.Tools[MyStatics.GC_AGGIORNA], null));


                this.ucMultiSelectUA.ViewShowAll = true;
                this.ucMultiSelectUA.ViewShowFind = true;
                sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' AS [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                        "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice.Text, Scci.Enums.EnumEntita.SCH.ToString());
                this.ucMultiSelectUA.ViewDataSetSX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' AS [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                        "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice.Text, Scci.Enums.EnumEntita.SCH.ToString());
                this.ucMultiSelectUA.ViewDataSetDX = DataBase.GetDataSet(sSql);
                this.ucMultiSelectUA.ViewInit();
                this.ucMultiSelectUA.RefreshData();

                this.ucMultiSelectPlusRuoli.ViewShowAll = true;

                this.ucMultiSelectPlusRuoli.ViewShowFind = true; this.ucMultiSelectPlusRuoli.GridDXFilterColumnIndex = 2;
                this.ucMultiSelectPlusRuoli.GridSXFilterColumnIndex = 2;

                sSql = string.Format("Select QAE.CodAzione, R.Codice AS CodRuolo, R.Descrizione + ' (' + R.Codice + ')' As Ruolo" + Environment.NewLine +
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
                                        "Order By QAE.CodAzione, R.Descrizione ASC", "SCH", this.uteCodice.Text);
                this.ucMultiSelectPlusRuoli.ViewDataSetSX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select ASS.CodAzione, ASS.CodRuolo, R.Descrizione + ' (' + R.Codice + ')' AS Ruolo" + Environment.NewLine +
                                        "From T_AssRuoliAzioni ASS" + Environment.NewLine +
                                                "Inner Join T_Ruoli R On ASS.CodRuolo = R.Codice" + Environment.NewLine +
                                                "Inner Join T_Azioni A On ASS.CodAzione = A.Codice" + Environment.NewLine +
                                                "Inner Join T_Entita E On ASS.CodEntita = E.Codice" + Environment.NewLine +
                                                "Inner Join T_AzioniEntita AE On ASS.CodEntita = AE.CodEntita And ASS.CodAzione = AE.CodAzione" + Environment.NewLine +
                                        "Where ASS.CodEntita = '{0}'" + Environment.NewLine +
                                                "And ASS.CodVoce = '{1}'" + Environment.NewLine +
                                                "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                        "Order By ASS.CodAzione, R.Descrizione ASC", "SCH", this.uteCodice.Text);
                this.ucMultiSelectPlusRuoli.ViewDataSetDX = DataBase.GetDataSet(sSql);
                sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                        "From T_AzioniEntita AE" + Environment.NewLine +
                                                "Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                                "Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                        "Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                                "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", "SCH");
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

                if (this.chkSchedaSemplice.Checked)
                {
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita"] = this.uteCodEntita.Text;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita2"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita3"] = System.DBNull.Value;

                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Ordine"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Path"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["NumerositaMinima"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["NumerositaMassima"] = System.DBNull.Value;

                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["EsportaDWH"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["EsportaDWHSingola"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["IgnoraStampaCartella"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Validabile"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Revisione"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Riservata"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CreaDefault"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["SistemaDWH"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Contenitore"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["AlertSchedaVuota"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CopiaPrecedenteSelezione"] = System.DBNull.Value;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CartellaAmbulatorialeCodificata"] = System.DBNull.Value;

                }
                else
                {
                    if (this.uchkEntita1.Checked)
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita"] = Scci.Enums.EnumEntita.PAZ.ToString();
                    else
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita"] = DBNull.Value;

                    if (this.uchkEntita2.Checked)
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita2"] = Scci.Enums.EnumEntita.EPI.ToString();
                    else
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita2"] = DBNull.Value;

                    if (this.uchkEntita3.Checked)
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita3"] = Scci.Enums.EnumEntita.SCH.ToString();
                    else
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita3"] = DBNull.Value;



                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Path"] = this.utePath.Text;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Ordine"] = this.uteOrdine.Text;

                    if (this.chkCreaDefault.Checked)
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["NumerositaMinima"] = this.umeNumerositaMinima.Value;
                    else
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["NumerositaMinima"] = DBNull.Value;

                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["NumerositaMassima"] = this.umeNumerositaMassima.Value;

                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["EsportaDWH"] = this.chkEsportaDWH.Checked;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["EsportaDWHSingola"] = this.chkEsportaDWHSingola.Checked;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["IgnoraStampaCartella"] = this.chkIgnoraStampaCartella.Checked;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Validabile"] = this.chkValidabile.Checked;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Revisione"] = this.chkRevisione.Checked;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["FirmaDigitale"] = this.chkFirmaDigitale.Checked;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Riservata"] = this.chkRiservata.Checked;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CreaDefault"] = this.chkCreaDefault.Checked;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Contenitore"] = this.chkContenitore.Checked;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["AlertSchedaVuota"] = this.chkAlertSchedaVuota.Checked;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CopiaPrecedenteSelezione"] = this.chkCopiaPrecedenteSelezione.Checked;
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CartellaAmbulatorialeCodificata"] = this.chkCartellaAmbulatorialeCodificata.Checked;

                    if (this.uceCodContatore.Value != null && this.uceCodContatore.Value != string.Empty)
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodContatore"] = this.uceCodContatore.Value;
                    else
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodContatore"] = System.DBNull.Value;

                    if (this.uceCodModalitaCopiaPrecedente.Value != null)
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodModalitaCopiaPrecedente"] = this.uceCodModalitaCopiaPrecedente.Value;
                    else
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodModalitaCopiaPrecedente"] = System.DBNull.Value;

                    if (this.chkEsportaDWH.Checked || this.chkEsportaDWHSingola.Checked)
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["SistemaDWH"] = this.uteSistemaDWH.Text;
                    else
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["SistemaDWH"] = System.DBNull.Value;

                    if (this.chkEsportaDWH.Checked || this.chkEsportaDWHSingola.Checked)
                    {
                        if (this.uteCodReportDWH.Text != string.Empty)
                        {
                            this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodReportDWH"] = this.uteCodReportDWH.Text;
                        }
                        else
                        {
                            this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodReportDWH"] = System.DBNull.Value;
                        }
                    }
                    else
                    {
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodReportDWH"] = System.DBNull.Value;
                    }

                    if (this.chkEsportaDWH.Checked || this.chkEsportaDWHSingola.Checked)
                    {
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodPrestazioneDWH"] = this.uteCodPrestazioneDWH.Text;
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["DescrizionePrestazioneDWH"] = this.uteDescrizionePrestazioneDWH.Text;
                    }
                    else
                    {
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodPrestazioneDWH"] = System.DBNull.Value;
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["DescrizionePrestazioneDWH"] = System.DBNull.Value;
                    }
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["EsportaLayerDWH"] = this.chkEsportaLayerDWH.Checked;

                }


            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "UpdateBindings", this.Name);
            }

        }
        private void UpdateBindingsAss()
        {

            string sSql = @"";

            try
            {

                if (_Modality == Enums.EnumModalityPopUp.mpModifica)
                {
                    if (this.chkSchedaSemplice.Checked)
                    {

                        sSql = "Delete from T_SchedePadri" + Environment.NewLine +
                                "Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine;
                        sSql += "   Or CodSchedaPadre = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine + Environment.NewLine;

                        sSql += @" Delete Fron T_SchedeCopia " + Environment.NewLine;
                        sSql += @" Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine + Environment.NewLine;
                        sSql += @"      Or CodSchedaCopia = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine + Environment.NewLine;
                    }


                    if (this.chkSchedaSemplice.Checked)
                    {
                        sSql += "Delete from T_AssUAEntita" + Environment.NewLine +
        "Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine +
        "And CodEntita = '" + Scci.Enums.EnumEntita.SCH.ToString() + "'" + Environment.NewLine;

                        sSql += "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                                "Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine +
                                        "And CodEntita = '" + Scci.Enums.EnumEntita.SCH.ToString() + "'" + Environment.NewLine;
                    }


                    if (sSql != "") DataBase.ExecuteSql(sSql);
                }

                sSql = "";
                if (_Modality != Enums.EnumModalityPopUp.mpNuovo)
                {

                    if (this.ucMultiSelectPadri.ViewDataSetSX.HasChanges() == true)
                    {
                        this.ViewDataBindings.DsLogPrima = DataBase.GetDataSet("Select * From T_SchedePadri Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "'");
                        sSql = "Delete from T_SchedePadri" + Environment.NewLine +
                                "Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine +
                                        "And CodSchedaPadre = '{0}'";
                        UpdateBindingsAssDataSet(this.ucMultiSelectPadri.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        this.ViewDataBindings.DsLogDopo = null;
                        MyStatics.LogManager(Enums.EnumModalityPopUp.mpCancella, Enums.EnumEntitaLog.T_SchedePadri, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                    }
                    if (!this.chkSchedaSemplice.Checked && this.ucMultiSelectPadri.ViewDataSetDX.HasChanges() == true)
                    {
                        string sSqlInsert = "Insert Into T_SchedePadri (CodScheda, CodSchedaPadre, AbilitaEPI, AbilitaPAZ)" + Environment.NewLine +
                                            "Values ('" + DataBase.Ax2(this.uteCodice.Text) + "', '{0}', {1}, {2})";
                        string sSqlUpdate = "Update T_SchedePadri Set AbilitaEPI = {1}, AbilitaPAZ = {2}" + Environment.NewLine +
                                            "Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "' And CodSchedaPadre = '{0}'";

                        if (!C_EnableEPI_PAZ_Padri)
                        {
                            sSqlInsert = "Insert Into T_SchedePadri (CodScheda, CodSchedaPadre)" + Environment.NewLine +
                                            "Values ('" + DataBase.Ax2(this.uteCodice.Text) + "', '{0}')";
                            sSqlUpdate = "";
                        }

                        this.ViewDataBindings.DsLogPrima = null;
                        DataSet oDs = this.ucMultiSelectPadri.ViewDataSetDX.GetChanges();
                        foreach (DataRow oRow in oDs.Tables[0].Rows)
                        {
                            if (oRow.RowState == DataRowState.Added)
                            {
                                if (C_EnableEPI_PAZ_Padri)
                                    DataBase.ExecuteSql(string.Format(sSqlInsert, oRow["Codice"], System.Convert.ToInt32(oRow["AbilitaEPI"]), System.Convert.ToInt32(oRow["AbilitaPAZ"])));
                                else
                                    DataBase.ExecuteSql(string.Format(sSqlInsert, oRow["Codice"]));
                            }
                            else if (oRow.RowState == DataRowState.Modified)
                            {
                                if (C_EnableEPI_PAZ_Padri) DataBase.ExecuteSql(string.Format(sSqlUpdate, oRow["Codice"], System.Convert.ToInt32(oRow["AbilitaEPI"]), System.Convert.ToInt32(oRow["AbilitaPAZ"])));
                            }
                        }
                        this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet("Select * From T_SchedePadri Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "'");
                        MyStatics.LogManager(Enums.EnumModalityPopUp.mpNuovo, Enums.EnumEntitaLog.T_SchedePadri, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                    }


                    if (this.ucMultiSelectFigli.ViewDataSetSX.HasChanges() == true)
                    {
                        this.ViewDataBindings.DsLogPrima = DataBase.GetDataSet("Select * From T_SchedePadri Where CodSchedaPadre = '" + DataBase.Ax2(this.uteCodice.Text) + "'");
                        sSql = "Delete from T_SchedePadri" + Environment.NewLine +
                                "Where CodSchedaPadre = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine +
                                        "And CodScheda = '{0}'";
                        UpdateBindingsAssDataSet(this.ucMultiSelectFigli.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        this.ViewDataBindings.DsLogDopo = null;
                        MyStatics.LogManager(Enums.EnumModalityPopUp.mpCancella, Enums.EnumEntitaLog.T_SchedePadri, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                    }
                    if (!this.chkSchedaSemplice.Checked && this.ucMultiSelectFigli.ViewDataSetDX.HasChanges() == true)
                    {
                        string sSqlInsert = "Insert Into T_SchedePadri (CodScheda, CodSchedaPadre, AbilitaEPI, AbilitaPAZ)" + Environment.NewLine +
                                            "Values ('{0}', '" + DataBase.Ax2(this.uteCodice.Text) + "', {1}, {2})";
                        string sSqlUpdate = "Update T_SchedePadri Set AbilitaEPI = {1}, AbilitaPAZ = {2}" + Environment.NewLine +
                                            "Where CodScheda = '{0}' And CodSchedaPadre ='" + DataBase.Ax2(this.uteCodice.Text) + "'";

                        if (!C_EnableEPI_PAZ_Figli)
                        {
                            sSqlInsert = "Insert Into T_SchedePadri (CodScheda, CodSchedaPadre)" + Environment.NewLine +
                                         "Values ('{0}', '" + DataBase.Ax2(this.uteCodice.Text) + "')";
                            sSqlUpdate = "";
                        }

                        this.ViewDataBindings.DsLogPrima = null;
                        DataSet oDs = this.ucMultiSelectFigli.ViewDataSetDX.GetChanges();
                        foreach (DataRow oRow in oDs.Tables[0].Rows)
                        {
                            if (oRow.RowState == DataRowState.Added)
                            {
                                if (C_EnableEPI_PAZ_Figli)
                                    DataBase.ExecuteSql(string.Format(sSqlInsert, oRow["Codice"], System.Convert.ToInt32(oRow["AbilitaEPI"]), System.Convert.ToInt32(oRow["AbilitaPAZ"])));
                                else
                                    DataBase.ExecuteSql(string.Format(sSqlInsert, oRow["Codice"]));
                            }
                            else if (oRow.RowState == DataRowState.Modified)
                            {
                                if (C_EnableEPI_PAZ_Figli) DataBase.ExecuteSql(string.Format(sSqlUpdate, oRow["Codice"], System.Convert.ToInt32(oRow["AbilitaEPI"]), System.Convert.ToInt32(oRow["AbilitaPAZ"])));
                            }
                        }
                        this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet("Select * From T_SchedePadri Where CodSchedaPadre = '" + DataBase.Ax2(this.uteCodice.Text) + "'");
                        MyStatics.LogManager(Enums.EnumModalityPopUp.mpNuovo, Enums.EnumEntitaLog.T_SchedePadri, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                    }


                    if (this.ucMultiSelectCopiaDa.ViewDataSetSX.HasChanges() == true)
                    {
                        this.ViewDataBindings.DsLogPrima = DataBase.GetDataSet("Select * From T_SchedeCopia Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "'");
                        sSql = "Delete from T_SchedeCopia" + Environment.NewLine +
                                "Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine +
                                        "And CodSchedaCopia = '{0}'";
                        UpdateBindingsAssDataSet(this.ucMultiSelectCopiaDa.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        this.ViewDataBindings.DsLogDopo = null;
                        MyStatics.LogManager(Enums.EnumModalityPopUp.mpCancella, Enums.EnumEntitaLog.T_SchedeCopia, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                    }
                    if (!this.chkSchedaSemplice.Checked && this.ucMultiSelectCopiaDa.ViewDataSetDX.HasChanges() == true)
                    {
                        this.ViewDataBindings.DsLogPrima = null;
                        string sSqlInsert = "Insert Into T_SchedeCopia (CodSchedaCopia, CodScheda)" + Environment.NewLine +
                                            "Values ('{0}', '" + DataBase.Ax2(this.uteCodice.Text) + "')";

                        DataSet oDs = this.ucMultiSelectCopiaDa.ViewDataSetDX.GetChanges();
                        foreach (DataRow oRow in oDs.Tables[0].Rows)
                        {
                            if (oRow.RowState == DataRowState.Added)
                            {
                                DataBase.ExecuteSql(string.Format(sSqlInsert, oRow["Codice"]));
                            }
                        }
                        this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet("Select * From T_SchedeCopia Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "'");
                        MyStatics.LogManager(Enums.EnumModalityPopUp.mpNuovo, Enums.EnumEntitaLog.T_SchedeCopia, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                    }


                    if (this.ucMultiSelectUA.ViewDataSetSX.HasChanges() == true)
                    {
                        this.ViewDataBindings.DsLogPrima = DataBase.GetDataSet("Select * From T_AssUAEntita Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "' And CodEntita = '" + Scci.Enums.EnumEntita.SCH.ToString() + "'");
                        sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                "Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine +
                                "And CodEntita = '" + Scci.Enums.EnumEntita.SCH.ToString() + "'" + Environment.NewLine +
                                "And CodUA = '{0}'";
                        UpdateBindingsAssDataSet(this.ucMultiSelectUA.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        this.ViewDataBindings.DsLogDopo = null;
                        MyStatics.LogManager(Enums.EnumModalityPopUp.mpCancella, Enums.EnumEntitaLog.T_AssUAEntita, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                    }
                    if (!this.chkSchedaSemplice.Checked && this.ucMultiSelectUA.ViewDataSetDX.HasChanges() == true)
                    {
                        this.ViewDataBindings.DsLogPrima = null;
                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                "Values ('{0}', '" + Scci.Enums.EnumEntita.SCH.ToString() + "', '" + DataBase.Ax2(this.uteCodice.Text) + @"')";
                        UpdateBindingsAssDataSet(this.ucMultiSelectUA.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet("Select * From T_AssUAEntita Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "' And CodEntita = '" + Scci.Enums.EnumEntita.SCH.ToString() + "'");
                        MyStatics.LogManager(Enums.EnumModalityPopUp.mpNuovo, Enums.EnumEntitaLog.T_AssUAEntita, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                    }

                    if (this.ucMultiSelectPlusRuoli.ViewDataSetSX.HasChanges() == true)
                    {
                        this.ViewDataBindings.DsLogPrima = DataBase.GetDataSet("Select * From T_AssRuoliAzioni Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "' And CodEntita = '" + Scci.Enums.EnumEntita.SCH.ToString() + "'");
                        sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                                "Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine +
                                        "And CodEntita = '" + Scci.Enums.EnumEntita.SCH.ToString() + "'" + Environment.NewLine +
                                        "And CodRuolo = '{0}'" + Environment.NewLine +
                                        "And CodAzione = '{1}'";
                        UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        this.ViewDataBindings.DsLogDopo = null;
                        MyStatics.LogManager(Enums.EnumModalityPopUp.mpCancella, Enums.EnumEntitaLog.T_AssRuoliAzioni, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                    }
                    if (!this.chkSchedaSemplice.Checked && this.ucMultiSelectPlusRuoli.ViewDataSetDX.HasChanges() == true)
                    {
                        this.ViewDataBindings.DsLogPrima = null;
                        sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                               "Values ('{0}', '" + Scci.Enums.EnumEntita.SCH.ToString() + "', '" + DataBase.Ax2(this.uteCodice.Text) + @"', '{1}')";
                        UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli.ViewDataSetDX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet("Select * From T_AssRuoliAzioni Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "' And CodEntita = '" + Scci.Enums.EnumEntita.SCH.ToString() + "'");
                        MyStatics.LogManager(Enums.EnumModalityPopUp.mpNuovo, Enums.EnumEntitaLog.T_AssRuoliAzioni, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                    }

                }

            }
            catch (Exception)
            {

            }

        }

        private void DeleteBindingsAss()
        {

            string sSql = @"";

            try
            {
                sSql = "Delete from T_SchedePadri Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "'";
                sSql += "   Or CodSchedaPadre = '" + DataBase.Ax2(this.uteCodice.Text) + "'";
                DataBase.ExecuteSql(sSql);

                sSql = "Delete from T_SchedeCopia Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "'";
                sSql += "   Or CodSchedaCopia = '" + DataBase.Ax2(this.uteCodice.Text) + "'";
                DataBase.ExecuteSql(sSql);

                sSql = "Delete from T_SchedeVersioni Where CodScheda = '" + DataBase.Ax2(this.uteCodice.Text) + "'";
                DataBase.ExecuteSql(sSql);
                sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
"Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine +
"And CodEntita = '" + Scci.Enums.EnumEntita.SCH.ToString() + "'";
                DataBase.ExecuteSql(sSql);
                sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice.Text) + "'" + Environment.NewLine +
                    "And CodEntita = '" + Scci.Enums.EnumEntita.SCH.ToString() + "'";
                DataBase.ExecuteSql(sSql);

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
            if (bRet && (this.uteCodTipoScheda.Text.Trim() != "" && this.lblCodTipoSchedaDes.Text.Trim() == ""))
            {
                MessageBox.Show(@"Inserire " + this.lblCodTipoScheda.Text + @" corretto!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodTipoScheda.Focus();
                bRet = false;
            }
            if (bRet && (this.chkSchedaSemplice.Checked && this.uteCodEntita.Text.Trim() != "" && this.lblCodEntitaDes.Text.Trim() == ""))
            {
                MessageBox.Show(@"Inserire " + this.ugbEntita.Text + @" corretta!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.uteCodEntita.Focus();
                bRet = false;
            }



            return bRet;

        }

        private void CopyVersion()
        {
            try
            {

                Parametri op = new Parametri();
                op.Parametro.Add("CodScheda", this.UltraGridVersioni.ActiveRow.Cells["CodScheda"].Value.ToString());
                op.Parametro.Add("Versione", this.UltraGridVersioni.ActiveRow.Cells["Versione"].Value.ToString());

                UnicodeSrl.Framework.Data.SqlParameterExt[] spcoll = new UnicodeSrl.Framework.Data.SqlParameterExt[1];

                string xmlParam = UnicodeSrl.Scci.Statics.XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new UnicodeSrl.Framework.Data.SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = UnicodeSrl.Scci.Statics.Database.GetDataTableStoredProc("MSP_CopiaVersioneScheda", spcoll);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void switchAccessoriaStandard()
        {
            try
            {
                bool bSchedaAccessoria = this.chkSchedaSemplice.Checked;

                this.ugbSchedaNormale.Visible = !bSchedaAccessoria;

                if (bSchedaAccessoria)
                {
                    this.uchkEntita1.Visible = false;
                    this.uchkEntita2.Visible = false;
                    this.uchkEntita3.Visible = false;

                    this.uteCodEntita.Visible = true;
                    this.lblCodEntitaDes.Visible = true;

                }
                else
                {

                    this.uteCodEntita.Visible = false;
                    this.lblCodEntitaDes.Visible = false;

                    this.uchkEntita1.Visible = true;
                    this.uchkEntita2.Visible = true;
                    this.uchkEntita3.Visible = true;

                }

                if (!_runtime) bindEntita();

                this.UltraTabControl.Tabs["tab2"].Visible = (_Modality != Enums.EnumModalityPopUp.mpNuovo && !bSchedaAccessoria);
                this.UltraTabControl.Tabs["tab4"].Visible = (_Modality != Enums.EnumModalityPopUp.mpNuovo && !bSchedaAccessoria);
                this.UltraTabControl.Tabs["tab3"].Visible = (_Modality != Enums.EnumModalityPopUp.mpNuovo && !bSchedaAccessoria);
                this.UltraTabControl.Tabs["tab6"].Visible = (_Modality != Enums.EnumModalityPopUp.mpNuovo && !bSchedaAccessoria);
                this.UltraTabControl.Tabs["tab7"].Visible = (_Modality != Enums.EnumModalityPopUp.mpNuovo && !bSchedaAccessoria);

                if (!_runtime) this.ubApplica.Enabled = true;

            }
            catch
            {
            }
        }

        private void switchNumerositaMinima()
        {
            switch (_Modality)
            {
                case Enums.EnumModalityPopUp.mpNuovo:
                case Enums.EnumModalityPopUp.mpModifica:
                case Enums.EnumModalityPopUp.mpCopia:
                    this.umeNumerositaMinima.Enabled = this.chkCreaDefault.Checked;
                    break;
                case Enums.EnumModalityPopUp.mpCancella:
                case Enums.EnumModalityPopUp.mpVisualizza:
                case Enums.EnumModalityPopUp.mpEsporta:
                case Enums.EnumModalityPopUp.mpImporta:
                default:
                    break;
            }
        }

        private void bindEntita()
        {
            try
            {
                this.uchkEntita1.Checked = false;
                this.uchkEntita2.Checked = false;
                this.uchkEntita3.Checked = false;
                this.uteCodEntita.Text = "";

                if (this.chkSchedaSemplice.Checked)
                {
                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("CodEntita"))
                        this.uteCodEntita.Text = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita"].ToString();

                }
                else
                {
                    string codEntita = "";

                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("CodEntita"))
                        codEntita = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita"].ToString();
                    if (codEntita.Trim().ToUpper() == Scci.Enums.EnumEntita.PAZ.ToString())
                        this.uchkEntita1.Checked = true;
                    else if (codEntita.Trim().ToUpper() == Scci.Enums.EnumEntita.EPI.ToString())
                        this.uchkEntita2.Checked = true;
                    else if (codEntita.Trim().ToUpper() == Scci.Enums.EnumEntita.SCH.ToString())
                        this.uchkEntita3.Checked = true;

                    codEntita = "";
                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("CodEntita2"))
                        codEntita = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita2"].ToString();
                    if (codEntita.Trim().ToUpper() == Scci.Enums.EnumEntita.PAZ.ToString())
                        this.uchkEntita1.Checked = true;
                    else if (codEntita.Trim().ToUpper() == Scci.Enums.EnumEntita.EPI.ToString())
                        this.uchkEntita2.Checked = true;
                    else if (codEntita.Trim().ToUpper() == Scci.Enums.EnumEntita.SCH.ToString())
                        this.uchkEntita3.Checked = true;

                    codEntita = "";
                    if (!_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("CodEntita3"))
                        codEntita = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["CodEntita3"].ToString();
                    if (codEntita.Trim().ToUpper() == Scci.Enums.EnumEntita.PAZ.ToString())
                        this.uchkEntita1.Checked = true;
                    else if (codEntita.Trim().ToUpper() == Scci.Enums.EnumEntita.EPI.ToString())
                        this.uchkEntita2.Checked = true;
                    else if (codEntita.Trim().ToUpper() == Scci.Enums.EnumEntita.SCH.ToString())
                        this.uchkEntita3.Checked = true;

                    switch (_Modality)
                    {
                        case Enums.EnumModalityPopUp.mpModifica:
                        case Enums.EnumModalityPopUp.mpCopia:
                            this.UltraTabControl.Tabs["tab2"].Enabled = this.uchkEntita3.Checked;
                            this.UltraTabControl.Tabs["tab4"].Enabled = true; break;
                        default:
                            this.UltraTabControl.Tabs["tab2"].Enabled = false;
                            this.UltraTabControl.Tabs["tab4"].Enabled = false;
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void Conversione()
        {

            try
            {

                frmPUSchedeConversioni oPUSchede = new frmPUSchedeConversioni();
                oPUSchede.ViewText = string.Format("Conversione Scheda {0} - {1}", this.uteCodice.Text, this.UltraGridVersioni.ActiveRow.Cells["Descrizione"].Text);
                oPUSchede.ViewIcon = this.ViewIcon;
                oPUSchede.CodScheda = this.uteCodice.Text;
                oPUSchede.Versione = (int)this.UltraGridVersioni.ActiveRow.Cells["Versione"].Value;
                oPUSchede.ViewInit();
                if (oPUSchede.ShowDialog() == DialogResult.OK)
                {

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void ControlloAbilitazioneDWH()
        {

            if (!this.chkSchedaSemplice.Checked)
            {
                switch (this.ViewModality)
                {
                    case Enums.EnumModalityPopUp.mpNuovo:
                    case Enums.EnumModalityPopUp.mpModifica:
                    case Enums.EnumModalityPopUp.mpCopia:
                        this.uteSistemaDWH.ReadOnly = !(this.chkEsportaDWH.Checked || this.chkEsportaDWHSingola.Checked);
                        this.uteCodReportDWH.Enabled = (this.chkEsportaDWH.Checked || this.chkEsportaDWHSingola.Checked);
                        this.uteCodPrestazioneDWH.ReadOnly = !(this.chkEsportaDWH.Checked || this.chkEsportaDWHSingola.Checked);
                        this.uteDescrizionePrestazioneDWH.ReadOnly = !(this.chkEsportaDWH.Checked || this.chkEsportaDWHSingola.Checked);
                        this.chkEsportaLayerDWH.Enabled = (this.chkEsportaDWH.Checked || this.chkEsportaDWHSingola.Checked);
                        break;
                    default:
                        this.uteSistemaDWH.ReadOnly = true;
                        this.uteCodReportDWH.Enabled = false;
                        this.uteCodPrestazioneDWH.ReadOnly = true;
                        this.uteDescrizionePrestazioneDWH.ReadOnly = true;
                        this.chkEsportaLayerDWH.Enabled = false;
                        break;
                }
            }
            else
            {
                this.uteSistemaDWH.ReadOnly = true;
                this.uteCodReportDWH.Enabled = false;
                this.uteCodPrestazioneDWH.ReadOnly = true;
                this.uteDescrizionePrestazioneDWH.ReadOnly = true;
                this.chkEsportaLayerDWH.Enabled = false;
            }
        }

        private void SetRevisione()
        {
            if (this.ViewModality == Enums.EnumModalityPopUp.mpNuovo || this.ViewModality == Enums.EnumModalityPopUp.mpModifica)
            {
                this.chkRevisione.Enabled = this.chkValidabile.Checked;
                this.chkFirmaDigitale.Enabled = this.chkValidabile.Checked;
            }
        }

        private void SetCartellaAmbulatorialeCodificata()
        {
            if (this.ViewModality == Enums.EnumModalityPopUp.mpNuovo || this.ViewModality == Enums.EnumModalityPopUp.mpModifica)
            {
                this.chkValidabile.Enabled = !this.chkCartellaAmbulatorialeCodificata.Checked;
            }
        }

        private void SetEntita2()
        {
            if (this.ViewModality == Enums.EnumModalityPopUp.mpNuovo || this.ViewModality == Enums.EnumModalityPopUp.mpModifica)
            {
                this.chkCartellaAmbulatorialeCodificata.Enabled = !this.uchkEntita2.Checked;
            }
        }

        #endregion

        #region Events

        private void frmPUSchede_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.UltraGridVersioni != null) this.UltraGridVersioni.Dispose();
        }

        private void uteCodice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsNumber(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void ute_ValueChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;

            if (sender == this.chkCreaDefault)
            {
                switchNumerositaMinima();
            }
            else if (sender == this.uchkEntita2)
            {
                SetEntita2();
            }
        }

        private void chkValidabile_CheckedChanged(object sender, EventArgs e)
        {
            this.SetRevisione();
        }

        private void chkCartellaAmbulatorialeCodificata_CheckedChanged(object sender, EventArgs e)
        {
            this.SetCartellaAmbulatorialeCodificata();
        }

        private void chkEsportaDWH_CheckedChanged(object sender, EventArgs e)
        {
            ControlloAbilitazioneDWH();
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

        private void UltraGrid_AfterRowActivate(object sender, EventArgs e)
        {
            this.SetUltraToolBarManager();
        }

        private void UltraGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {

            if (e.Row.IsDataRow == true)
            {
                if (this.UltraToolbarsManagerGrid.Tools[MyStatics.GC_MODIFICA].SharedProps.Enabled == true)
                {
                    ActionGridToolClick(this.UltraToolbarsManagerGrid.Tools[MyStatics.GC_MODIFICA]);
                }
                else
                {
                    ActionGridToolClick(this.UltraToolbarsManagerGrid.Tools[MyStatics.GC_VISUALIZZA]);
                }
            }

        }

        private void UltraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("CodScheda") == true)
                {
                    e.Layout.Bands[0].Columns["CodScheda"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("NumerositaMinima") == true)
                {
                    e.Layout.Bands[0].Columns["NumerositaMinima"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("NumerositaMassima") == true)
                {
                    e.Layout.Bands[0].Columns["NumerositaMassima"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("CreaDefault") == true)
                {
                    e.Layout.Bands[0].Columns["CreaDefault"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("EsportaDWH") == true)
                {
                    e.Layout.Bands[0].Columns["EsportaDWH"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("EsportaDWHSingola") == true)
                {
                    e.Layout.Bands[0].Columns["EsportaDWHSingola"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("IgnoraStampaCartella") == true)
                {
                    e.Layout.Bands[0].Columns["IgnoraStampaCartella"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("Validabile") == true)
                {
                    e.Layout.Bands[0].Columns["Validabile"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("Revisione") == true)
                {
                    e.Layout.Bands[0].Columns["Revisione"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("Riservata") == true)
                {
                    e.Layout.Bands[0].Columns["Riservata"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("Contenitore") == true)
                {
                    e.Layout.Bands[0].Columns["Contenitore"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("AlertSchedaVuota") == true)
                {
                    e.Layout.Bands[0].Columns["AlertSchedaVuota"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("CopiaPrecedenteSelezione") == true)
                {
                    e.Layout.Bands[0].Columns["CopiaPrecedenteSelezione"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("CampiRilevanti") == true)
                {
                    e.Layout.Bands[0].Columns["CampiRilevanti"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("CampiObbligatori") == true)
                {
                    e.Layout.Bands[0].Columns["CampiObbligatori"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("Struttura") == true)
                {
                    e.Layout.Bands[0].Columns["Struttura"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("Layout") == true)
                {
                    e.Layout.Bands[0].Columns["Layout"].Hidden = true;
                }
                if (e.Layout.Bands[0].Columns.Exists("DtValI") == true)
                {
                    e.Layout.Bands[0].Columns["DtValI"].Format = @"dd/MM/yyyy HH:mm";
                }
                if (e.Layout.Bands[0].Columns.Exists("DtValF") == true)
                {
                    e.Layout.Bands[0].Columns["DtValF"].Format = @"dd/MM/yyyy HH:mm";
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {

        }

        private void UltraToolbarsManagerGrid_ToolClick(object sender, ToolClickEventArgs e)
        {
            try
            {
                ActionGridToolClick(e.Tool);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void uteCodTipoScheda_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodTipoScheda.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_TipoScheda";
                f.ViewSqlStruct.Where = "Codice <> '" + this.uteCodTipoScheda.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodTipoScheda.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodTipoScheda_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodTipoSchedaDes.Text = DataBase.FindValue("Descrizione", "T_TipoScheda", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodTipoScheda.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodEntita_ValueChanged(object sender, EventArgs e)
        {
            string search = @"Codice = '{0}' And IsNull(UsaSchede, 0) <> 0";
            if (this.chkSchedaSemplice.Checked)
                search += @" And IsNull(UsaSchedaSemplificata, 0) <> 0";
            else
                search += @" And IsNull(UsaSchedaSemplificata, 0) = 0";
            this.lblCodEntitaDes.Text = DataBase.FindValue("Descrizione", "T_Entita", string.Format(search, DataBase.Ax2(this.uteCodEntita.Text)), "");
            this.ubApplica.Enabled = true;
        }
        private void uteCodEntita_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.ugbEntita.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Entita";
                f.ViewSqlStruct.Where = "Codice <> '" + this.uteCodEntita.Text + @"' And IsNull(UsaSchede, 0) <> 0";
                if (this.chkSchedaSemplice.Checked)
                    f.ViewSqlStruct.Where += @" And IsNull(UsaSchedaSemplificata, 0) <> 0";
                else
                    f.ViewSqlStruct.Where += @" And IsNull(UsaSchedaSemplificata, 0) = 0";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodEntita.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }





        private void chkSchedaSemplice_CheckedChanged(object sender, EventArgs e)
        {
            bool bCancel = false;
            string sRel = "";
            try
            {
                if (_Modality == Enums.EnumModalityPopUp.mpModifica && !_runtime && this.chkSchedaSemplice.Checked)
                {
                    if (this.ucMultiSelectPadri.GridDX.Rows.Count > 0)
                    {
                        if (sRel != "") sRel += Environment.NewLine;
                        sRel += " - Schede Padri";
                        bCancel = true;
                    }
                    if (this.ucMultiSelectFigli.GridDX.Rows.Count > 0)
                    {
                        if (sRel != "") sRel += Environment.NewLine;
                        sRel += " - Schede Figli";
                        bCancel = true;
                    }
                    if (this.ucMultiSelectCopiaDa.GridDX.Rows.Count > 0)
                    {
                        if (sRel != "") sRel += Environment.NewLine;
                        sRel += " - Schede Copia da...";
                        bCancel = true;
                    }

                }

                if (bCancel)
                {
                    _runtime = true;
                    MessageBox.Show(@"Prima di impostare la scheda corrente come ""ACCESSORIA""" + Environment.NewLine + @"occorre eliminare le relazioni con:" + Environment.NewLine + sRel, "ATTENZIONE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.chkSchedaSemplice.Checked = false;
                    _runtime = false;
                }
                else
                {
                    switchAccessoriaStandard();
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void uchkEntita3_CheckedChanged(object sender, EventArgs e)
        {
            bool bCancel = false;
            string sRel = "";
            try
            {

                if (_Modality == Enums.EnumModalityPopUp.mpModifica && !_runtime
                    && !this.chkSchedaSemplice.Checked && !this.uchkEntita3.Checked)
                {
                    if (this.ucMultiSelectPadri.GridDX.Rows.Count > 0)
                    {
                        if (sRel != "") sRel += Environment.NewLine;
                        sRel += " - Schede Padri";
                        bCancel = true;
                    }
                    if (this.ucMultiSelectFigli.GridDX.Rows.Count > 0)
                    {
                        if (sRel != "") sRel += Environment.NewLine;
                        sRel += " - Schede Figli";
                        bCancel = true;
                    }

                }

                if (bCancel)
                {
                    _runtime = true;
                    MessageBox.Show(@"Prima di togliere l'entità ""SCHEDA""" + Environment.NewLine + @"occorre eliminare le relazioni con:" + Environment.NewLine + sRel, "ATTENZIONE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uchkEntita3.Checked = true;
                    _runtime = false;
                }
                else
                {

                    this.ubApplica.Enabled = true;

                    switch (_Modality)
                    {
                        case Enums.EnumModalityPopUp.mpModifica:
                        case Enums.EnumModalityPopUp.mpCopia:
                            this.UltraTabControl.Tabs["tab2"].Enabled = this.uchkEntita3.Checked;
                            this.UltraTabControl.Tabs["tab4"].Enabled = true; break;
                        default:
                            this.UltraTabControl.Tabs["tab2"].Enabled = false;
                            this.UltraTabControl.Tabs["tab4"].Enabled = false;
                            break;
                    }
                }


            }
            catch (Exception ex)
            {
            }
        }

        private void uteCodReportDWH_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodReportDWH.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Report";
                f.ViewSqlStruct.Where = "Codice <> '" + this.uteCodReportDWH.Text + @"' And CodFormatoReport = 'CAB' And Codice in ('SCHDPZN1','SCHDPZN4')";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodReportDWH.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodReportDWH_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodReportDWHDes.Text = DataBase.FindValue("Descrizione", "T_Report", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodReportDWH.Text)), "");
            this.ubApplica.Enabled = true;
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
                            this.ViewDataBindings.DsLogPrima = null;
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
                            this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice.Text + "'";
                            this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpNuovo, Enums.EnumEntitaLog.T_Schede, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
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
                            this.ViewDataBindings.DsLogPrima = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
                            this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpModifica, Enums.EnumEntitaLog.T_Schede, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                            _DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.ViewInit();
                        }
                        break;

                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "ubApplica_Click", this.Name);
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
                            this.ViewDataBindings.DsLogPrima = null;
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
                            this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice.Text + "'";
                            this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpNuovo, Enums.EnumEntitaLog.T_Schede, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpModifica:
                        if (CheckInput())
                        {
                            this.ViewDataBindings.DsLogPrima = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
                            this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpModifica, Enums.EnumEntitaLog.T_Schede, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpCancella:
                        if (MessageBox.Show("Confermi la cancellazione ?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                        {
                            this.ViewDataBindings.DsLogPrima = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            this.DeleteBindingsAss();
                            DataBase.ExecuteSql(this.ViewDataBindings.SqlDelete.Sql);
                            this.ViewDataBindings.DsLogDopo = null;
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpCancella, Enums.EnumEntitaLog.T_Schede, this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
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
                MyStatics.ExGest(ref ex, "ubConferma_Click", this.Name);
            }

        }

        #endregion

        #region Event ucMultiSelect Padri

        private void ucMultiSelectPadri_GridSXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaEPI") == true) { e.Layout.Bands[0].Columns["AbilitaEPI"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaPAZ") == true) { e.Layout.Bands[0].Columns["AbilitaPAZ"].Hidden = true; }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelectPadri_GridSXInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                e.Row.Cells["Schede"].Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SYMBOL_RESTRICTED, Enums.EnumImageSize.isz16));

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelectPadri_GridDXDoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {

            if (e.Cell.Column.Key == @"AbilitaEPI" || e.Cell.Column.Key == @"AbilitaPAZ")
            {

                try
                {

                    if ((bool)e.Cell.Value == false)
                    {

                        bool bNext = false;

                        switch (e.Cell.Column.Key)
                        {

                            case @"AbilitaEPI":
                                if (this.uchkEntita2.Checked)
                                {
                                    string sCodEntita = Database.FindValue("CodEntita", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita2 = Database.FindValue("CodEntita2", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita3 = Database.FindValue("CodEntita3", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    if (sCodEntita == Scci.Enums.EnumEntita.EPI.ToString()
                                        || sCodEntita2 == Scci.Enums.EnumEntita.EPI.ToString()
                                        || sCodEntita3 == Scci.Enums.EnumEntita.EPI.ToString())
                                    {
                                        bNext = true;
                                    }
                                }
                                break;

                            case @"AbilitaPAZ":
                                if (this.uchkEntita1.Checked)
                                {
                                    string sCodEntita = Database.FindValue("CodEntita", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita2 = Database.FindValue("CodEntita2", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita3 = Database.FindValue("CodEntita3", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    if (sCodEntita == Scci.Enums.EnumEntita.PAZ.ToString()
                                        || sCodEntita2 == Scci.Enums.EnumEntita.PAZ.ToString()
                                        || sCodEntita3 == Scci.Enums.EnumEntita.PAZ.ToString())
                                    {
                                        bNext = true;
                                    }
                                }
                                break;

                        }
                        if (bNext)
                        {
                            e.Cell.Value = !(bool)e.Cell.Value;
                            e.Cell.Row.Update();
                        }

                    }
                    else
                    {

                        e.Cell.Value = !(bool)e.Cell.Value;
                        e.Cell.Row.Update();

                    }

                }
                catch (Exception)
                {

                }

            }

        }

        private void ucMultiSelectPadri_GridDXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaEPI") == true) { e.Layout.Bands[0].Columns["AbilitaEPI"].Header.Caption = "EPI"; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaPAZ") == true) { e.Layout.Bands[0].Columns["AbilitaPAZ"].Header.Caption = "PAZ"; }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelectPadri_GridDXInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                e.Row.Cells["Schede"].Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SYMBOL_CHECK, Enums.EnumImageSize.isz16));

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Event ucMultiSelect Figli

        private void ucMultiSelectFigli_GridSXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaEPI") == true) { e.Layout.Bands[0].Columns["AbilitaEPI"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaPAZ") == true) { e.Layout.Bands[0].Columns["AbilitaPAZ"].Hidden = true; }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelectFigli_GridSXInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                e.Row.Cells["Schede"].Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SYMBOL_RESTRICTED, Enums.EnumImageSize.isz16));

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelectFigli_GridDXDoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {

            if (e.Cell.Column.Key == @"AbilitaEPI" || e.Cell.Column.Key == @"AbilitaPAZ")
            {

                try
                {

                    if ((bool)e.Cell.Value == false)
                    {

                        bool bNext = false;

                        switch (e.Cell.Column.Key)
                        {

                            case @"AbilitaEPI":
                                if (this.uchkEntita2.Checked)
                                {
                                    string sCodEntita = Database.FindValue("CodEntita", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita2 = Database.FindValue("CodEntita2", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita3 = Database.FindValue("CodEntita3", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    if (sCodEntita == Scci.Enums.EnumEntita.EPI.ToString()
                                        || sCodEntita2 == Scci.Enums.EnumEntita.EPI.ToString()
                                        || sCodEntita3 == Scci.Enums.EnumEntita.EPI.ToString())
                                    {
                                        bNext = true;
                                    }
                                }
                                break;

                            case @"AbilitaPAZ":
                                if (this.uchkEntita1.Checked)
                                {
                                    string sCodEntita = Database.FindValue("CodEntita", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita2 = Database.FindValue("CodEntita2", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita3 = Database.FindValue("CodEntita3", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    if (sCodEntita == Scci.Enums.EnumEntita.PAZ.ToString()
                                        || sCodEntita2 == Scci.Enums.EnumEntita.PAZ.ToString()
                                        || sCodEntita3 == Scci.Enums.EnumEntita.PAZ.ToString())
                                    {
                                        bNext = true;
                                    }
                                }
                                break;

                        }
                        if (bNext)
                        {
                            e.Cell.Value = !(bool)e.Cell.Value;
                            e.Cell.Row.Update();
                        }

                    }
                    else
                    {

                        e.Cell.Value = !(bool)e.Cell.Value;
                        e.Cell.Row.Update();

                    }

                }
                catch (Exception)
                {

                }

            }

        }

        private void ucMultiSelectFigli_GridDXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaEPI") == true) { e.Layout.Bands[0].Columns["AbilitaEPI"].Header.Caption = "EPI"; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaPAZ") == true) { e.Layout.Bands[0].Columns["AbilitaPAZ"].Header.Caption = "PAZ"; }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelectFigli_GridDXInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                e.Row.Cells["Schede"].Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SYMBOL_CHECK, Enums.EnumImageSize.isz16));

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Event ucMultiSelect Copia da

        private void ucMultiSelectCopiaDa_GridSXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaEPI") == true) { e.Layout.Bands[0].Columns["AbilitaEPI"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaPAZ") == true) { e.Layout.Bands[0].Columns["AbilitaPAZ"].Hidden = true; }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelectCopiaDa_GridSXInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                e.Row.Cells["Schede"].Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SYMBOL_RESTRICTED, Enums.EnumImageSize.isz16));

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelectCopiaDa_GridDXDoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {

            if (e.Cell.Column.Key == @"AbilitaEPI" || e.Cell.Column.Key == @"AbilitaPAZ")
            {

                try
                {

                    if ((bool)e.Cell.Value == false)
                    {

                        bool bNext = false;

                        switch (e.Cell.Column.Key)
                        {

                            case @"AbilitaEPI":
                                if (this.uchkEntita2.Checked)
                                {
                                    string sCodEntita = Database.FindValue("CodEntita", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita2 = Database.FindValue("CodEntita2", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita3 = Database.FindValue("CodEntita3", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    if (sCodEntita == Scci.Enums.EnumEntita.EPI.ToString()
                                        || sCodEntita2 == Scci.Enums.EnumEntita.EPI.ToString()
                                        || sCodEntita3 == Scci.Enums.EnumEntita.EPI.ToString())
                                    {
                                        bNext = true;
                                    }
                                }
                                break;

                            case @"AbilitaPAZ":
                                if (this.uchkEntita1.Checked)
                                {
                                    string sCodEntita = Database.FindValue("CodEntita", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita2 = Database.FindValue("CodEntita2", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    string sCodEntita3 = Database.FindValue("CodEntita3", "T_Schede", "Codice = '" + e.Cell.Row.Cells["Codice"].Value + "'", "");
                                    if (sCodEntita == Scci.Enums.EnumEntita.PAZ.ToString()
                                        || sCodEntita2 == Scci.Enums.EnumEntita.PAZ.ToString()
                                        || sCodEntita3 == Scci.Enums.EnumEntita.PAZ.ToString())
                                    {
                                        bNext = true;
                                    }
                                }
                                break;

                        }
                        if (bNext)
                        {
                            e.Cell.Value = !(bool)e.Cell.Value;
                            e.Cell.Row.Update();
                        }

                    }
                    else
                    {

                        e.Cell.Value = !(bool)e.Cell.Value;
                        e.Cell.Row.Update();

                    }

                }
                catch (Exception)
                {

                }

            }

        }

        private void ucMultiSelectCopiaDa_GridDXInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true) { e.Layout.Bands[0].Columns["Codice"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaEPI") == true) { e.Layout.Bands[0].Columns["AbilitaEPI"].Header.Caption = "EPI"; }
                if (e.Layout.Bands[0].Columns.Exists("AbilitaPAZ") == true) { e.Layout.Bands[0].Columns["AbilitaPAZ"].Header.Caption = "PAZ"; }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelectCopiaDa_GridDXInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                e.Row.Cells["Schede"].Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SYMBOL_CHECK, Enums.EnumImageSize.isz16));

            }
            catch (Exception)
            {

            }

        }

        #endregion

    }
}
