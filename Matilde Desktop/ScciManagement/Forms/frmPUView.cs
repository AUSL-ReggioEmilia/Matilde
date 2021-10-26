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
using UnicodeSrl.Framework.Data;
using Newtonsoft.Json;
using ScciCommon.Model;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUView : Form, Interfacce.IViewFormPUView
    {
        public frmPUView()
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

            this.InitializeRichTextBox();
            this.InitializePictureSelect();
            this.InitializeUltraCombo();

            _RuoloEntitaAzione_Voci = new Dictionary<string, DataSet>();
            _TestiPredefiniti_Campi = new Dictionary<string, DataTable>();

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
                    this.uteCodice1.Enabled = false;
                    this.uteCodice2.Enabled = false;
                    this.uteCodice3.Enabled = false;
                    this.uteCodice4.Enabled = false;
                    this.uteCodice5.Enabled = false;
                    this.uteCodice6.Enabled = false;
                    this.uteCodice7.Enabled = false;
                    this.uteCodice8.Enabled = false;
                    this.uteCodice10.Enabled = false;
                    this.uteCodice11.Enabled = false;
                    this.uteCodice12.Enabled = false;
                    this.uteCodice13.Enabled = false;
                    this.uteCodice14.Enabled = false;
                    this.uteCodice15.Enabled = false;
                    this.uteCodice17.Enabled = false;
                    this.uteCodice18.Enabled = false;
                    this.uteCodAzi18.Enabled = false;
                    this.uteCodice19.Enabled = false;
                    this.uteCodice20.Enabled = false;
                    this.uteCodLetto25.Enabled = false;
                    this.uteCodAzi25.Enabled = false;
                    this.uteCodSettore25.Enabled = false;
                    this.uteCodice26.Enabled = false;
                    this.uteCodAzi26.Enabled = false;
                    this.uteCodScheda29.Enabled = false;
                    this.uteCodice29.Enabled = false;
                    this.uteCodice22.Enabled = false;
                    this.ubApplica.Enabled = false;
                    if (this.ViewDataNamePU == Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione) this.uteDescrizione17.Enabled = false;

                    if (this.ViewDataNamePU == Enums.EnumDataNames.T_Maschere)
                    {
                        this.uteCodice9.Enabled = false;
                        this.uteDescrizione9.Enabled = false;
                        this.chkInCache9.Enabled = false;
                        this.chkInCacheDaPercorso9.Enabled = false;
                        this.chkCambioPercorso9.Enabled = false;
                        this.chkAggiorna9.Enabled = false;
                        this.chkMassimizzata9.Enabled = false;
                    }

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
                    this.uteCodice3.Enabled = true;
                    this.uteCodice4.Enabled = true;
                    this.uteCodice8.Enabled = true;
                    this.EditBindingsCopia();
                    this.ubApplica.Enabled = true;
                    break;

                default:
                    break;

            }

            if (this.ViewDataNamePU == Enums.EnumDataNames.T_Report) setCheckBrowser();

            this.ResumeLayout();

        }

        #endregion

        #region Subroutine

        private void InitializeRichTextBox()
        {
            try
            {
                switch (this.ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_Aziende:
                        this.ucRichTextBoxRTFStampaEstesa1.ViewInit();
                        this.ucRichTextBoxRTFStampaSintetica1.ViewInit();
                        break;
                    case Enums.EnumDataNames.T_UnitaAtomiche:
                        this.ucRichTextBoxFirmaCartella41.ViewInit();
                        this.ucRichTextBoxIntestazioneCartella41.ViewInit();
                        this.ucRichTextBoxIntestazioneSintetica42.ViewInit();
                        this.ucRichTextBoxSpallaSinistra42.ViewInit();
                        this.ucRichTextBoxIntestazioni.ViewInit();
                        break;
                    case Enums.EnumDataNames.T_TestiPredefiniti:
                        this.ucRichTextBoxTestoRTF10.ViewInit();
                        break;
                    case Enums.EnumDataNames.T_MovNews:
                        this.ucRichTextBoxTestoRTF11.ViewInit();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "InitializeRichTextBox", this.Name);
            }
        }

        private void InitializePictureSelect()
        {
            try
            {
                switch (this.ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_Login:
                        this.ucPictureSelectFoto3.ViewShowSaveImage = true;
                        this.ucPictureSelectFoto3.ViewCheckSquareImage = false;
                        this.ucPictureSelectFoto3.ViewInit();
                        break;

                    case Enums.EnumDataNames.T_TipoParametroVitale:
                        this.ucPictureSelectIcona6.ViewShowSaveImage = false;
                        this.ucPictureSelectIcona6.ViewCheckSquareImage = true;
                        this.ucPictureSelectIcona6.ViewInit();
                        break;

                    case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                    case Enums.EnumDataNames.T_TipoOrdine:
                        this.ucPictureSelectIcona7.ViewShowSaveImage = false;
                        this.ucPictureSelectIcona7.ViewCheckSquareImage = true;
                        this.ucPictureSelectIcona7.ViewInit();
                        break;

                    case Enums.EnumDataNames.T_ViaSomministrazione:
                        this.ucPictureSelectIcona13.ViewShowSaveImage = false;
                        this.ucPictureSelectIcona13.ViewCheckSquareImage = true;
                        this.ucPictureSelectIcona13.ViewInit();
                        break;

                    case Enums.EnumDataNames.T_StatoAppuntamento:
                    case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                    case Enums.EnumDataNames.T_StatoAllegato:
                        this.ucPictureSelectIcona15.ViewShowSaveImage = false;
                        this.ucPictureSelectIcona15.ViewCheckSquareImage = true;
                        this.ucPictureSelectIcona15.ViewInit();
                        break;

                    case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                        this.ucPictureSelectIcona14.ViewShowSaveImage = false;
                        this.ucPictureSelectIcona14.ViewCheckSquareImage = true;
                        this.ucPictureSelectIcona14.ViewInit();
                        break;

                    case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                    case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                    case Enums.EnumDataNames.T_StatoAlertGenerico:
                    case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                    case Enums.EnumDataNames.T_StatoParametroVitale:
                    case Enums.EnumDataNames.T_StatoConsegna:
                    case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                    case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                    case Enums.EnumDataNames.T_StatoScheda:
                    case Enums.EnumDataNames.T_StatoPrescrizione:
                    case Enums.EnumDataNames.T_StatoDiario:
                    case Enums.EnumDataNames.T_StatoTrasferimento:
                    case Enums.EnumDataNames.T_TipoAllegato:
                    case Enums.EnumDataNames.T_FormatoAllegati:
                    case Enums.EnumDataNames.T_StatoOrdine:
                    case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                    case Enums.EnumDataNames.T_StatoContinuazione:
                    case Enums.EnumDataNames.T_StatoCartella:
                    case Enums.EnumDataNames.T_StatoCartellaInfo:
                    case Enums.EnumDataNames.T_StatoCartellaInVisione:
                    case Enums.EnumDataNames.T_StatoEpisodio:
                        this.ucPictureSelectIcona17.ViewShowSaveImage = false;
                        this.ucPictureSelectIcona17.ViewCheckSquareImage = true;
                        this.ucPictureSelectIcona17.ViewInit();
                        break;

                    case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                        this.ucPictureSelectIcona29.ViewShowSaveImage = false;
                        this.ucPictureSelectIcona29.ViewCheckSquareImage = true;
                        this.ucPictureSelectIcona29.ViewInit();
                        break;

                    case Enums.EnumDataNames.T_Sistemi:
                        this.ucPictureSelectIcona27.ViewShowSaveImage = false;
                        this.ucPictureSelectIcona27.ViewCheckSquareImage = true;
                        this.ucPictureSelectIcona27.ViewInit();
                        break;

                    case Enums.EnumDataNames.T_FormatoReport:
                    case Enums.EnumDataNames.T_TipoEpisodio:
                    case Enums.EnumDataNames.T_TipoDiario:
                    case Enums.EnumDataNames.T_TipoScheda:
                    case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                        this.ucPictureSelectIcona19.ViewShowSaveImage = false;
                        this.ucPictureSelectIcona19.ViewCheckSquareImage = true;
                        this.ucPictureSelectIcona19.ViewInit();
                        break;

                    case Enums.EnumDataNames.T_TipoPrescrizione:
                        this.ucPictureSelectIcona20.ViewShowSaveImage = false;
                        this.ucPictureSelectIcona20.ViewCheckSquareImage = true;
                        this.ucPictureSelectIcona20.ViewInit();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "InitializePictureSelect", this.Name);
            }
        }

        private void InitializeUltraCombo()
        {

            string sSql = string.Empty;
            DataSet oDs = null;

            try
            {
                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_Ruoli:
                        MyStatics.SetUltraComboEditorLayout(ref this.uceCodTipoDiario2);
                        this.uceCodTipoDiario2.ValueMember = "Codice";
                        this.uceCodTipoDiario2.DisplayMember = "Descrizione";
                        sSql = DataBase.GetSqlPUView(Enums.EnumDataNames.T_TipoDiario) + " ORDER BY Descrizione";
                        oDs = DataBase.GetDataSet(sSql);
                        this.uceCodTipoDiario2.DataMember = oDs.Tables[0].TableName;
                        this.uceCodTipoDiario2.DataSource = oDs;
                        this.uceCodTipoDiario2.DataBind();

                        MyStatics.InitUltraComboEditorTVCheck(ref this.uceAzioniFiltro2);

                        break;

                    case Enums.EnumDataNames.T_Report:
                        MyStatics.SetUltraComboEditorLayout(ref this.uceCodFormatoReport8);
                        this.uceCodFormatoReport8.ValueMember = "Codice";
                        this.uceCodFormatoReport8.DisplayMember = "Descrizione";
                        sSql = DataBase.GetSqlPUView(Enums.EnumDataNames.T_FormatoReport) + " WHERE ISNULL(Attivo, 0) = 1 ORDER BY Descrizione";
                        oDs = DataBase.GetDataSet(sSql);
                        this.uceCodFormatoReport8.DataMember = oDs.Tables[0].TableName;
                        this.uceCodFormatoReport8.DataSource = oDs;
                        this.uceCodFormatoReport8.DataBind();
                        break;

                    case Enums.EnumDataNames.T_MovNews:
                        MyStatics.SetUltraComboEditorLayout(ref this.uceCodTipoNews11);
                        this.uceCodTipoNews11.ValueMember = "Codice";
                        this.uceCodTipoNews11.DisplayMember = "Descrizione";
                        oDs = CoreStatics.GetTipoNewsDs();
                        this.uceCodTipoNews11.DataMember = oDs.Tables[0].TableName;
                        this.uceCodTipoNews11.DataSource = oDs;
                        this.uceCodTipoNews11.DataBind();
                        break;

                    case Enums.EnumDataNames.T_OEAttributi:
                        MyStatics.SetUltraComboEditorLayout(ref this.uceCodEntita21);
                        this.uceCodEntita21.ValueMember = "Codice";
                        this.uceCodEntita21.DisplayMember = "Descrizione";
                        oDs = DataBase.GetDataSet("Select Codice, Descrizione From T_Entita Order By Descrizione");
                        this.uceCodEntita21.DataMember = oDs.Tables[0].TableName;
                        this.uceCodEntita21.DataSource = oDs;
                        this.uceCodEntita21.DataBind();
                        break;

                    case Enums.EnumDataNames.T_Contatori:
                        MyStatics.SetUltraComboEditorLayout(ref this.uceCodUnitaScadenza28);
                        this.uceCodUnitaScadenza28.ValueMember = "Codice";
                        this.uceCodUnitaScadenza28.DisplayMember = "Descrizione";
                        oDs = CoreStatics.GetUnitaScadenzaDs();
                        this.uceCodUnitaScadenza28.DataMember = oDs.Tables[0].TableName;
                        this.uceCodUnitaScadenza28.DataSource = oDs;
                        this.uceCodUnitaScadenza28.DataBind();

                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "InitializeUltraCombo", this.Name);
            }

        }

        private void InitAndBindWebReportConfig()
        {
            try
            {
                this.ultraTabControl8.Tabs["ReportWeb"].Visible = false;

                if (this.uceCodFormatoReport8.Value != null && this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_REM.ToUpper())
                {
                    if (Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ReMConnectionString).Trim() == "")
                    {
                        if (_Modality != Enums.EnumModalityPopUp.mpCancella) MessageBox.Show(@"Nella Configurazione Ambiente non è stata definita" + Environment.NewLine + @"la Stringa di Connessione al db di Report Manager!" + Environment.NewLine + @"Impossibile caricare il componente per la configurazione del Report.", "Report Web", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else
                    {
                        this.ultraTabControl8.Tabs["ReportWeb"].Visible = true;
                        this.ultraTabControl8.Tabs["ReportWeb"].Selected = true;

                        if (Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ReMViewer).Trim() == "")
                            if (_Modality != Enums.EnumModalityPopUp.mpCancella)
                                MessageBox.Show(@"Nella Configurazione Ambiente non è stato definito" + Environment.NewLine + @"l'URL di Report Viewer." + Environment.NewLine + @"Non sarà possibile visualizzare l'anteprima del report.", "Report Web", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        if (!this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Parametri"))
                            this.reportLinkConfig8.Initialize(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ReMConnectionString)
                                                            , Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ReMViewer)
                                                            , this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Parametri"].ToString());
                        else
                            this.reportLinkConfig8.Initialize(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ReMConnectionString)
                                                            , Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ReMViewer));


                        LoadReportExternalFields();

                    }
                }
                else if (this.uceCodFormatoReport8.Value != null && this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_CABLATO.ToUpper())
                {
                    if (!this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Parametri"))
                        this.xmlParametri8.Text = this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Parametri"].ToString();
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "InitAndBindWebReportConfig", this.Text);
            }
        }

        private void LoadReportExternalFields()
        {
            try
            {
                this.reportLinkConfig8.ResetExternalFields();



                string sql = @" Select Codice, Descrizione
                                From T_PlaceHolder
                                Order By 1";
                DataSet ds = DataBase.GetDataSet(sql);
                if (ds != null)
                {
                    if (ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rowCol in ds.Tables[0].Rows)
                        {
                            try
                            {
                                DataRow dr = this.reportLinkConfig8.ExternalFields.NewRow();
                                dr["CODICE"] = rowCol["Codice"].ToString();
                                dr["DESCRIZIONE"] = rowCol["Descrizione"].ToString();
                                this.reportLinkConfig8.ExternalFields.Rows.Add(dr);
                            }
                            catch (Exception)
                            {
                            }
                        }

                    }
                    ds.Dispose();
                }


                string xml = "";
                if (!this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Variabili") && this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Variabili"].ToString() != "")
                {
                    xml = this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Variabili"].ToString();
                    DataTable dtVariables = CoreStatics.getVariables(xml);

                    foreach (DataRow drv in dtVariables.Rows)
                    {
                        try
                        {

                            DataRow dr = this.reportLinkConfig8.ExternalFields.NewRow();
                            dr["CODICE"] = drv["CODICE"];
                            dr["DESCRIZIONE"] = drv["DESCRIZIONE"];
                            this.reportLinkConfig8.ExternalFields.Rows.Add(dr);
                        }
                        catch (Exception ex)
                        {
                            UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                        }
                    }
                }


                this.reportLinkConfig8.ReloadExternalFields();
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadReportExternalFields", this.Text);
            }
        }

        private void SetBindings()
        {

            DataColumn _dcol = null;

            try
            {
                _DataBinds.DataBindings.Clear();

                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_Aziende:
                        this.UltraTabControl.Tabs["tab1"].Visible = true;
                        this.UltraTabControl.Tabs["tab1"].Text = this.ViewText;
                        this.ucEasyTableLayoutPanel1.Visible = true;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice1);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione1);
                        _DataBinds.DataBindings.Add("ViewRtf", "RTFStampaEstesa", this.ucRichTextBoxRTFStampaEstesa1);
                        _DataBinds.DataBindings.Add("ViewRtf", "RTFStampaSintetica", this.ucRichTextBoxRTFStampaSintetica1);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice1.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione1.MaxLength = _dcol.MaxLength;

                        if (_Modality == Enums.EnumModalityPopUp.mpNuovo &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != null &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != "")
                        {
                            this.ucRichTextBoxRTFStampaEstesa1.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));
                            this.ucRichTextBoxRTFStampaSintetica1.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));
                        }
                        break;

                    case Enums.EnumDataNames.T_CDSSAzioni:
                        this.UltraTabControl.Tabs["tab1"].Visible = true;
                        this.UltraTabControl.Tabs["tab1"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice1);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione1);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice1.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione1.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Ruoli:
                        this.UltraTabControl.Tabs["tab2"].Visible = true;
                        this.UltraTabControl.Tabs["tab2"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice2);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione2);
                        _DataBinds.DataBindings.Add("Text", "Note", this.uteNote2);
                        _DataBinds.DataBindings.Add("Value", "CodTipoDiario", this.uceCodTipoDiario2);
                        _DataBinds.DataBindings.Add("Text", "NumMaxCercaEpi", this.uteNumMaxCercaEpi2);
                        _DataBinds.DataBindings.Add("Checked", "RichiediPassword", this.chkRichiediPassword2);
                        _DataBinds.DataBindings.Add("Checked", "LimitaEVCAmbulatoriale", this.chkLimitaEVCAmbulatoriale2);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice2.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione2.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Note"];
                        if (_dcol.MaxLength > 0) this.uteNote2.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Login:
                        this.UltraTabControl.Tabs["tab3"].Visible = true;
                        this.UltraTabControl.Tabs["tab3"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice3);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione3);
                        _DataBinds.DataBindings.Add("Text", "Cognome", this.uteCognome3);
                        _DataBinds.DataBindings.Add("Text", "Nome", this.uteNome3);
                        _DataBinds.DataBindings.Add("Text", "CodiceFiscale", this.uteCodiceFiscale3);
                        _DataBinds.DataBindings.Add("Text", "Note", this.uteNote3);
                        _DataBinds.DataBindings.Add("Checked", "FlagAdmin", this.chkFlagAdmin3);
                        _DataBinds.DataBindings.Add("Checked", "FlagObsoleto", this.chkFlagObsoleto3);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice3.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione3.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Cognome"];
                        if (_dcol.MaxLength > 0) this.uteCognome3.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Nome"];
                        if (_dcol.MaxLength > 0) this.uteNome3.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodiceFiscale"];
                        if (_dcol.MaxLength > 0) this.uteCodiceFiscale3.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Note"];
                        if (_dcol.MaxLength > 0) this.uteNote3.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_UnitaAtomiche:
                        this.UltraTabControl.Tabs["tab4"].Visible = true;
                        this.UltraTabControl.Tabs["tab4"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice4);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione4);
                        _DataBinds.DataBindings.Add("Text", "Note", this.uteNote4);
                        _DataBinds.DataBindings.Add("Text", "CodPadre", this.uteCodPadre4);
                        _DataBinds.DataBindings.Add("Text", "UltimoNumeroCartella", this.uteUltimoNumeroCartella4);
                        _DataBinds.DataBindings.Add("Text", "CodUANumerazioneCartella", this.uteCodUANumerazioneCartella4);
                        _DataBinds.DataBindings.Add("ViewRtf", "FirmaCartella", this.ucRichTextBoxFirmaCartella41);
                        _DataBinds.DataBindings.Add("ViewRtf", "IntestazioneCartella", this.ucRichTextBoxIntestazioneCartella41);
                        _DataBinds.DataBindings.Add("ViewRtf", "IntestazioneSintetica", this.ucRichTextBoxIntestazioneSintetica42);
                        _DataBinds.DataBindings.Add("ViewRtf", "SpallaSinistra", this.ucRichTextBoxSpallaSinistra42);
                        _DataBinds.DataBindings.Add("Checked", "AbilitaCollegaCartelle", this.chkAbilitaCollegaCartelle4);
                        _DataBinds.DataBindings.Add("Checked", "AccessoAmbulatoriale", this.chkAccessoAmbulatoriale4);
                        _DataBinds.DataBindings.Add("Text", "OraApertura", this.umeOraApertura4);
                        _DataBinds.DataBindings.Add("Text", "OraChiusura", this.umeOraChiusura4);
                        _DataBinds.DataBindings.Add("Text", "CodAzienda", this.uteCodAzienda4);
                        _DataBinds.DataBindings.Add("Checked", "AbilitaCollegaCartellePA", this.chkAbilitaCollegaCartellePA4);
                        _DataBinds.DataBindings.Add("Checked", "VisualizzaIconeAppuntamenti", this.chkVisualizzaIconeAppuntamenti4);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice4.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione4.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Note"];
                        if (_dcol.MaxLength > 0) this.uteNote4.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodPadre"];
                        if (_dcol.MaxLength > 0) this.uteCodPadre4.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodUANumerazioneCartella"];
                        if (_dcol.MaxLength > 0) this.uteCodUANumerazioneCartella4.MaxLength = _dcol.MaxLength;

                        if (_Modality == Enums.EnumModalityPopUp.mpNuovo &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != null &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != "")
                        {
                            this.ucRichTextBoxIntestazioneCartella41.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));
                            this.ucRichTextBoxFirmaCartella41.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));
                            this.ucRichTextBoxIntestazioneSintetica42.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));
                            this.ucRichTextBoxSpallaSinistra42.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));
                            this.ucRichTextBoxIntestazioni.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));
                        }

                        break;

                    case Enums.EnumDataNames.T_DiarioInfermieristico:
                    case Enums.EnumDataNames.T_DiarioMedico:
                        this.UltraTabControl.Tabs["tab5"].Visible = true;
                        this.UltraTabControl.Tabs["tab5"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice5);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione5);
                        _DataBinds.DataBindings.Add("Text", "CodScheda", this.uteCodScheda5);
                        _DataBinds.DataBindings.Add("Checked", "CopiaDaPrecedente", this.chkCopiaDaPrecedente5);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice5.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione5.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodScheda"];
                        if (_dcol.MaxLength > 0) this.uteCodScheda5.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_TipoParametroVitale:
                        this.UltraTabControl.Tabs["tab6"].Visible = true;
                        this.UltraTabControl.Tabs["tab6"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice6);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione6);
                        _DataBinds.DataBindings.Add("Text", "CodScheda", this.uteCodScheda6);
                        _DataBinds.DataBindings.Add("Text", "CampiFUT", this.uteCampiFUT6);
                        _DataBinds.DataBindings.Add("Text", "CampiGrafici", this.uteCampiGrafici6);
                        _DataBinds.DataBindings.Add("Text", "Ordine", this.umeOrdine6);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice6.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione6.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodScheda"];
                        if (_dcol.MaxLength > 0) this.uteCodScheda6.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CampiFUT"];
                        if (_dcol.MaxLength > 0) this.uteCampiFUT6.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CampiGrafici"];
                        if (_dcol.MaxLength > 0) this.uteCampiGrafici6.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                        this.UltraTabControl.Tabs["tab7"].Visible = true;
                        this.UltraTabControl.Tabs["tab7"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice7);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione7);
                        _DataBinds.DataBindings.Add("Checked", "RiaperturaCartella", this.chkRiaperturaCartella7);
                        _DataBinds.DataBindings.Add("Checked", "AllegaInCartella", this.chkAllegaInCartella7);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice7.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione7.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_TipoOrdine:
                        this.UltraTabControl.Tabs["tab7"].Visible = true;
                        this.UltraTabControl.Tabs["tab7"].Text = this.ViewText;
                        this.chkRiaperturaCartella7.Visible = false;
                        this.chkAllegaInCartella7.Visible = false;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice7);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione7);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice7.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione7.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Report:
                        this.UltraTabControl.Tabs["tab8"].Visible = true;
                        this.UltraTabControl.Tabs["tab8"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice8);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione8);
                        _DataBinds.DataBindings.Add("Value", "CodFormatoReport", this.uceCodFormatoReport8);
                        _DataBinds.DataBindings.Add("Checked", "DaStoricizzare", this.chkDaStoricizzare8);
                        _DataBinds.DataBindings.Add("Text", "Path", this.utePath8);
                        _DataBinds.DataBindings.Add("Text", "PercorsoFile", this.utePercorsoFile8);
                        _DataBinds.DataBindings.Add("Text", "NomePlugIn", this.uteNomePlugin8);
                        _DataBinds.DataBindings.Add("Text", "Note", this.uteNote8);
                        _DataBinds.DataBindings.Add("Checked", "ApriBrowser", this.chkApriBrowser8);
                        _DataBinds.DataBindings.Add("Checked", "ApriIE", this.chkApriIE8);
                        _DataBinds.DataBindings.Add("Checked", "FlagRichiediStampante", this.chkFlagRichiediStampante8);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice8.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione8.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Path"];
                        if (_dcol.MaxLength > 0) this.utePath8.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Note"];
                        if (_dcol.MaxLength > 0) this.uteNote8.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Maschere:
                        this.UltraTabControl.Tabs["tab9"].Visible = true;
                        this.UltraTabControl.Tabs["tab9"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice9);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione9);
                        _DataBinds.DataBindings.Add("Checked", "InCache", this.chkInCache9);
                        _DataBinds.DataBindings.Add("Checked", "InCacheDaPercorso", this.chkInCacheDaPercorso9);
                        _DataBinds.DataBindings.Add("Checked", "CambioPercorso", this.chkCambioPercorso9);
                        _DataBinds.DataBindings.Add("Checked", "Aggiorna", this.chkAggiorna9);
                        _DataBinds.DataBindings.Add("Checked", "Massimizzata", this.chkMassimizzata9);
                        _DataBinds.DataBindings.Add("Text", "TimerRefresh", this.uteTimerRefresh9);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice9.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione9.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_TestiPredefiniti:
                        this.UltraTabControl.Tabs["tab10"].Visible = true;
                        this.UltraTabControl.Tabs["tab10"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice10);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione10);
                        _DataBinds.DataBindings.Add("Text", "Path", this.utePath10);
                        _DataBinds.DataBindings.Add("Text", "CodEntita", this.uteCodEntita10);
                        _DataBinds.DataBindings.Add("ViewRtf", "TestoRTF", this.ucRichTextBoxTestoRTF10);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice10.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione10.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Path"];
                        if (_dcol.MaxLength > 0) this.utePath10.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodEntita"];
                        if (_dcol.MaxLength > 0) this.uteCodEntita10.MaxLength = _dcol.MaxLength;

                        if (_Modality == Enums.EnumModalityPopUp.mpNuovo &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != null &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != "")
                            this.ucRichTextBoxTestoRTF10.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));

                        break;

                    case Enums.EnumDataNames.T_MovNews:
                        this.UltraTabControl.Tabs["tab11"].Visible = true;
                        this.UltraTabControl.Tabs["tab11"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice11);
                        _DataBinds.DataBindings.Add("Text", "Titolo", this.uteDescrizione11);
                        _DataBinds.DataBindings.Add("Value", "DataOra", this.udteDataOra11);
                        _DataBinds.DataBindings.Add("Value", "DataInizioPubblicazione", this.udteDataInizioPubblicazione11);
                        _DataBinds.DataBindings.Add("Value", "DataFinePubblicazione", this.udteDataFinePubblicazione11);
                        _DataBinds.DataBindings.Add("Checked", "Rilevante", this.chkRilevante11);
                        _DataBinds.DataBindings.Add("ViewRtf", "TestoRTF", this.ucRichTextBoxTestoRTF11);
                        _DataBinds.DataBindings.Add("Value", "CodTipoNews", this.uceCodTipoNews11);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice11.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Titolo"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione11.MaxLength = _dcol.MaxLength;

                        if (_Modality == Enums.EnumModalityPopUp.mpNuovo &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != null &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != "")
                            this.ucRichTextBoxTestoRTF11.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));

                        break;

                    case Enums.EnumDataNames.T_UnitaOperative:
                        this.UltraTabControl.Tabs["tab12"].Visible = true;
                        this.UltraTabControl.Tabs["tab12"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice12);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione12);
                        _DataBinds.DataBindings.Add("Text", "CodAzi", this.uteCodAzi12);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice12.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione12.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodAzi"];
                        if (_dcol.MaxLength > 0) this.uteCodAzi12.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_ViaSomministrazione:
                        this.UltraTabControl.Tabs["tab13"].Visible = true;
                        this.UltraTabControl.Tabs["tab13"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice13);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione13);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice13.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione13.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                        this.UltraTabControl.Tabs["tab14"].Visible = true;
                        this.UltraTabControl.Tabs["tab14"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice14);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione14);
                        _DataBinds.DataBindings.Add("Value", "Colore", this.ucpColore14);
                        _DataBinds.DataBindings.Add("Checked", "Visibile", this.chkVisibile14);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice14.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione14.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_StatoAppuntamento:
                    case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                    case Enums.EnumDataNames.T_StatoAllegato:
                        this.UltraTabControl.Tabs["tab15"].Visible = true;
                        this.UltraTabControl.Tabs["tab15"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice15);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione15);
                        _DataBinds.DataBindings.Add("Text", "Ordine", this.umeOrdine15);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice15.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione15.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_OEFormule:
                        this.UltraTabControl.Tabs["tab16"].Visible = true;
                        this.UltraTabControl.Tabs["tab16"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "CodUA", this.uteCodUA16);
                        _DataBinds.DataBindings.Add("Text", "CodAzienda", this.uteCodAzienda16);
                        _DataBinds.DataBindings.Add("Text", "CodErogante", this.uteCodErogante16);
                        _DataBinds.DataBindings.Add("Text", "CodPrestazione", this.uteCodPrestazione16);
                        _DataBinds.DataBindings.Add("Text", "CodDatoAccessorio", this.uteCodDatoAccessorio16);
                        _DataBinds.DataBindings.Add("Text", "Formula", this.uteFormula16);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodUA"];
                        if (_dcol.MaxLength > 0) this.uteCodUA16.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodAzienda"];
                        if (_dcol.MaxLength > 0) this.uteCodAzienda16.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodErogante"];
                        if (_dcol.MaxLength > 0) this.uteCodErogante16.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodPrestazione"];
                        if (_dcol.MaxLength > 0) this.uteCodPrestazione16.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodDatoAccessorio"];
                        if (_dcol.MaxLength > 0) this.uteCodDatoAccessorio16.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                    case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                    case Enums.EnumDataNames.T_StatoAlertGenerico:
                    case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                    case Enums.EnumDataNames.T_StatoParametroVitale:
                    case Enums.EnumDataNames.T_StatoConsegna:
                    case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                    case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                    case Enums.EnumDataNames.T_StatoScheda:
                    case Enums.EnumDataNames.T_StatoPrescrizione:
                    case Enums.EnumDataNames.T_StatoDiario:
                    case Enums.EnumDataNames.T_StatoTrasferimento:
                    case Enums.EnumDataNames.T_TipoAllegato:
                    case Enums.EnumDataNames.T_FormatoAllegati:
                    case Enums.EnumDataNames.T_StatoOrdine:
                    case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                    case Enums.EnumDataNames.T_StatoContinuazione:
                    case Enums.EnumDataNames.T_StatoCartella:
                    case Enums.EnumDataNames.T_StatoCartellaInfo:
                    case Enums.EnumDataNames.T_StatoCartellaInVisione:
                    case Enums.EnumDataNames.T_StatoEpisodio:
                        this.UltraTabControl.Tabs["tab17"].Visible = true;
                        this.UltraTabControl.Tabs["tab17"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice17);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione17);

                        this.lblOrdine17.Visible = (this.ViewDataNamePU == Enums.EnumDataNames.T_StatoTrasferimento);
                        this.umeOrdine17.Visible = (this.ViewDataNamePU == Enums.EnumDataNames.T_StatoTrasferimento);
                        if (this.ViewDataNamePU == Enums.EnumDataNames.T_StatoTrasferimento) _DataBinds.DataBindings.Add("Text", "Ordine", this.umeOrdine17);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice17.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione17.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                        this.UltraTabControl.Tabs["tab29"].Visible = true;
                        this.UltraTabControl.Tabs["tab29"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "CodScheda", this.uteCodScheda29);
                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice29);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione29);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodScheda"];
                        if (_dcol.MaxLength > 0) this.uteCodScheda29.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice29.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione29.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Sistemi:
                        this.UltraTabControl.Tabs["tab27"].Visible = true;
                        this.UltraTabControl.Tabs["tab27"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice27);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione27);
                        _DataBinds.DataBindings.Add("Text", "CodDestinatario", this.uteCodDestinatario27);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice27.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione27.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodDestinatario"];
                        if (_dcol.MaxLength > 0) this.uteCodDestinatario27.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Stanze:
                        this.UltraTabControl.Tabs["tab18"].Visible = true;
                        this.UltraTabControl.Tabs["tab18"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "CodAzi", this.uteCodAzi18);
                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice18);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione18);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice18.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione18.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_FormatoReport:
                    case Enums.EnumDataNames.T_TipoEpisodio:
                    case Enums.EnumDataNames.T_TipoDiario:
                    case Enums.EnumDataNames.T_TipoScheda:
                    case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                        this.UltraTabControl.Tabs["tab19"].Visible = true;
                        this.UltraTabControl.Tabs["tab19"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice19);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione19);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice19.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione19.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_TipoPrescrizione:
                        this.UltraTabControl.Tabs["tab20"].Visible = true;
                        this.UltraTabControl.Tabs["tab20"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice20);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione20);
                        _DataBinds.DataBindings.Add("Text", "CodScheda", this.uteCodScheda20);
                        _DataBinds.DataBindings.Add("Text", "CodViaSomministrazione", this.uteCodViaSomministrazione20);
                        _DataBinds.DataBindings.Add("Checked", "NonProseguibile", this.chkNonProseguibile20);
                        _DataBinds.DataBindings.Add("Checked", "PrescrizioneASchema", this.chkPrescrizioneASchema20);
                        _DataBinds.DataBindings.Add("Text", "Path", this.utePath20);
                        _DataBinds.DataBindings.Add("Text", "CodSchedaPosologia", this.uteCodSchedaPosologia20);
                        _DataBinds.DataBindings.Add("Value", "ColoreGrafico", this.ucpColoreGrafico20);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice20.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione20.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodScheda"];
                        if (_dcol.MaxLength > 0) this.uteCodScheda20.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodViaSomministrazione"];
                        if (_dcol.MaxLength > 0) this.uteCodViaSomministrazione20.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Path"];
                        if (_dcol.MaxLength > 0) this.utePath20.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodSchedaPosologia"];
                        if (_dcol.MaxLength > 0) this.uteCodSchedaPosologia20.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_OEAttributi:
                        this.UltraTabControl.Tabs["tab21"].Visible = true;
                        this.UltraTabControl.Tabs["tab21"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Value", "CodEntita", this.uceCodEntita21);
                        _DataBinds.DataBindings.Add("Text", "CodSistemaRichiedente", this.uteCodSistemaRichiedente21);
                        _DataBinds.DataBindings.Add("Text", "CodAgendaRichiedente", this.uteCodAgendaRichiedente21);
                        _DataBinds.DataBindings.Add("Text", "MappaturaOE", this.xmlMappaturaOE21);
                        _DataBinds.DataBindings.Add("Text", "Note", this.uteNote21);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodSistemaRichiedente"];
                        if (_dcol.MaxLength > 0) this.uteCodSistemaRichiedente21.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodAgendaRichiedente"];
                        if (_dcol.MaxLength > 0) this.uteCodAgendaRichiedente21.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Letti:
                        this.UltraTabControl.Tabs["tab25"].Visible = true;
                        this.UltraTabControl.Tabs["tab25"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "CodAzi", this.uteCodAzi25);
                        _DataBinds.DataBindings.Add("Text", "CodLetto", this.uteCodLetto25);
                        _DataBinds.DataBindings.Add("Text", "CodSettore", this.uteCodSettore25);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione25);
                        _DataBinds.DataBindings.Add("Text", "CodStanza", this.uteCodStanza25);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodLetto"];
                        if (_dcol.MaxLength > 0) this.uteCodLetto25.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione25.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodAzi"];
                        if (_dcol.MaxLength > 0) this.uteCodAzi25.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodSettore"];
                        if (_dcol.MaxLength > 0) this.uteCodSettore25.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodStanza"];
                        if (_dcol.MaxLength > 0) this.uteCodStanza25.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Settori:
                        this.UltraTabControl.Tabs["tab26"].Visible = true;
                        this.UltraTabControl.Tabs["tab26"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "CodAzi", this.uteCodAzi26);
                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice26);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione26);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice26.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodAzi"];
                        if (_dcol.MaxLength > 0) this.uteCodAzi26.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione26.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Contatori:
                        this.UltraTabControl.Tabs["tab28"].Visible = true;
                        this.UltraTabControl.Tabs["tab28"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice28);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione28);
                        _DataBinds.DataBindings.Add("Text", "Valore", this.umeValore28);
                        _DataBinds.DataBindings.Add("Value", "DataScadenza", this.udteDataScadenza28);
                        _DataBinds.DataBindings.Add("Value", "CodUnitaScadenza", this.uceCodUnitaScadenza28);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice28.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione28.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_TestiNotePredefiniti:
                        this.UltraTabControl.Tabs["tab22"].Visible = true;
                        this.UltraTabControl.Tabs["tab22"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice22);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione22);
                        _DataBinds.DataBindings.Add("Text", "OggettoNota", this.uteOggettoNota22);
                        _DataBinds.DataBindings.Add("Text", "DescrizioneNota", this.uteDescrizioneNota22);
                        _DataBinds.DataBindings.Add("Text", "Path", this.utePath22);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice10.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione10.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Path"];
                        if (_dcol.MaxLength > 0) this.utePath10.MaxLength = _dcol.MaxLength;
                        break;

                    default:
                        break;

                }

                _DataBinds.DataBindings.Load();

                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_Login:
                        this.ucPictureSelectFoto3.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Foto"]);
                        break;

                    case Enums.EnumDataNames.T_TipoParametroVitale:
                        this.uteCampiFUT6.Text = MyStatics.IndentXMLString(this.uteCampiFUT6.Text);
                        this.uteCampiGrafici6.Text = MyStatics.IndentXMLString(this.uteCampiGrafici6.Text);
                        this.ucPictureSelectIcona6.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        this.ucpColore6.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
                        break;

                    case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                    case Enums.EnumDataNames.T_TipoOrdine:
                        this.ucPictureSelectIcona7.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        break;

                    case Enums.EnumDataNames.T_ViaSomministrazione:
                        this.ucPictureSelectIcona13.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        break;

                    case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                        this.ucPictureSelectIcona14.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        this.ucpColore14.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
                        break;

                    case Enums.EnumDataNames.T_StatoAppuntamento:
                    case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                    case Enums.EnumDataNames.T_StatoAllegato:
                        this.ucPictureSelectIcona15.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        this.ucpColore15.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
                        break;

                    case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                    case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                    case Enums.EnumDataNames.T_StatoAlertGenerico:
                    case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                    case Enums.EnumDataNames.T_StatoParametroVitale:
                    case Enums.EnumDataNames.T_StatoConsegna:
                    case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                    case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                    case Enums.EnumDataNames.T_StatoScheda:
                    case Enums.EnumDataNames.T_StatoPrescrizione:
                    case Enums.EnumDataNames.T_StatoDiario:
                    case Enums.EnumDataNames.T_StatoTrasferimento:
                    case Enums.EnumDataNames.T_TipoAllegato:
                    case Enums.EnumDataNames.T_FormatoAllegati:
                    case Enums.EnumDataNames.T_StatoOrdine:
                    case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                    case Enums.EnumDataNames.T_StatoContinuazione:
                    case Enums.EnumDataNames.T_StatoCartella:
                    case Enums.EnumDataNames.T_StatoCartellaInfo:
                    case Enums.EnumDataNames.T_StatoCartellaInVisione:
                    case Enums.EnumDataNames.T_StatoEpisodio:
                        this.ucPictureSelectIcona17.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        this.ucpColore17.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
                        break;

                    case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                        this.ucPictureSelectIcona29.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        this.ucpColore29.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
                        break;

                    case Enums.EnumDataNames.T_Sistemi:
                        this.ucPictureSelectIcona27.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        this.ucpColore27.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
                        break;

                    case Enums.EnumDataNames.T_FormatoReport:
                    case Enums.EnumDataNames.T_TipoEpisodio:
                    case Enums.EnumDataNames.T_TipoDiario:
                    case Enums.EnumDataNames.T_TipoScheda:
                    case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                        this.ucPictureSelectIcona19.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        break;

                    case Enums.EnumDataNames.T_TipoPrescrizione:
                        this.ucPictureSelectIcona20.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        this.ucpColoreGrafico20.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["ColoreGrafico"].ToString());
                        break;

                    case Enums.EnumDataNames.T_TestiNotePredefiniti:
                        this.ucpColore22.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
                        break;

                    default:
                        break;

                }

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

                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_Ruoli:
                        this.ucMultiSelectModuli2.ViewShowAll = true;
                        this.ucMultiSelectModuli2.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As Moduli From T_Moduli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodModulo From T_AssRuoliModuli Where CodRuolo = '{1}') ORDER BY Descrizione ASC", "Not In", this.uteCodice2.Text);
                        this.ucMultiSelectModuli2.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As Moduli From T_Moduli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodModulo From T_AssRuoliModuli Where CodRuolo = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice2.Text);
                        this.ucMultiSelectModuli2.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectModuli2.ViewInit();

                        this.ucMultiSelectModuli2.RefreshData();
                        this.ucMultiSelectUA2.ViewShowAll = true;
                        this.ucMultiSelectUA2.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssRuoliUA Where CodRuolo = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice2.Text);
                        this.ucMultiSelectUA2.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssRuoliUA Where CodRuolo = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice2.Text);
                        this.ucMultiSelectUA2.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA2.ViewInit();
                        this.ucMultiSelectUA2.RefreshData();
                        this.ucMultiSelectLogin2.ViewShowAll = true;
                        this.ucMultiSelectLogin2.ViewShowFind = true;
                        this.ucMultiSelectLogin2.GridSXFilterColumnIndex = 2;
                        this.ucMultiSelectLogin2.GridDXFilterColumnIndex = 2;
                        sSql = string.Format("Select Codice, ISNULL(FlagObsoleto,0) AS Obsoleto, Descrizione + ' (' + Codice + ')' As Login From T_Login" + Environment.NewLine +
                                                "Where Codice {0} (Select CodLogin From T_AssLoginRuoli Where CodRuolo = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice2.Text);
                        this.ucMultiSelectLogin2.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, ISNULL(FlagObsoleto,0) AS Obsoleto, Descrizione + ' (' + Codice + ')' As Login From T_Login" + Environment.NewLine +
                                                "Where Codice {0} (Select CodLogin From T_AssLoginRuoli Where CodRuolo = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice2.Text);
                        this.ucMultiSelectLogin2.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectLogin2.ViewInit();
                        this.ucMultiSelectLogin2.RefreshData();
                        this.ucMultiSelectUAVisione2.ViewShowAll = true;
                        this.ucMultiSelectUAVisione2.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUARuoliCartellaInVisione Where CodRuolo = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice2.Text);
                        this.ucMultiSelectUAVisione2.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUARuoliCartellaInVisione Where CodRuolo = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice2.Text);
                        this.ucMultiSelectUAVisione2.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUAVisione2.ViewInit();
                        this.ucMultiSelectUAVisione2.RefreshData();

                        LoadEntitaAzioni();
                        LoadRuoloAzioni();

                        break;

                    case Enums.EnumDataNames.T_Login:
                        this.ucMultiSelectRuoli3.ViewShowAll = true;
                        this.ucMultiSelectRuoli3.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As Ruolo From T_Ruoli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodRuolo From T_AssLoginRuoli Where CodLogin = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice3.Text);
                        this.ucMultiSelectRuoli3.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As Ruolo From T_Ruoli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodRuolo From T_AssLoginRuoli Where CodLogin = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice3.Text);
                        this.ucMultiSelectRuoli3.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectRuoli3.ViewInit();
                        this.ucMultiSelectRuoli3.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_UnitaAtomiche:
                        this.ucMultiSelectRuoli4.ViewShowAll = true;
                        this.ucMultiSelectRuoli4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As Ruolo From T_Ruoli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodRuolo From T_AssRuoliUA Where CodUA = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text);
                        this.ucMultiSelectRuoli4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As Ruolo From T_Ruoli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodRuolo From T_AssRuoliUA Where CodUA = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice4.Text);
                        this.ucMultiSelectRuoli4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectRuoli4.ViewInit();
                        this.ucMultiSelectRuoli4.RefreshData();
                        this.ucMultiSelectModuli4.ViewShowAll = true;
                        this.ucMultiSelectModuli4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As Ruolo From T_ModuliUA" + Environment.NewLine +
                                                "Where Codice {0} (Select CodModulo From T_AssUAModuli Where CodUA = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text);
                        this.ucMultiSelectModuli4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As Ruolo From T_ModuliUA" + Environment.NewLine +
                                                "Where Codice {0} (Select CodModulo From T_AssUAModuli Where CodUA = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice4.Text);
                        this.ucMultiSelectModuli4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectModuli4.ViewInit();
                        this.ucMultiSelectModuli4.RefreshData();
                        this.ucMultiSelectAGE4.ViewShowAll = true;
                        this.ucMultiSelectAGE4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As Agenda From T_Agende" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectAGE4.Tag.ToString());
                        this.ucMultiSelectAGE4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As Agenda From T_Agende" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectAGE4.Tag.ToString());
                        this.ucMultiSelectAGE4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectAGE4.ViewInit();
                        this.ucMultiSelectAGE4.RefreshData();
                        this.ucMultiSelectAPP4.ViewShowAll = true;
                        this.ucMultiSelectAPP4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As Appuntamento From T_TipoAppuntamento" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectAPP4.Tag.ToString());
                        this.ucMultiSelectAPP4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As Appuntamento From T_TipoAppuntamento" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectAPP4.Tag.ToString());
                        this.ucMultiSelectAPP4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectAPP4.ViewInit();
                        this.ucMultiSelectAPP4.RefreshData();
                        this.ucMultiSelectDCL4.ViewShowAll = true;
                        this.ucMultiSelectDCL4.ViewShowFind = true;
                        sSql = string.Format("Select TVD.Codice, TVD.Descrizione + ' (' + TD.Descrizione + ') (' + TVD.Codice + ')' As [Diario Clinico] From T_TipoVoceDiario TVD" + Environment.NewLine +
                                                "Inner Join T_TipoDiario TD On TVD.CodTipoDiario = TD.Codice" + Environment.NewLine +
                                                "Where TVD.Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY TVD.Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectDCL4.Tag.ToString());
                        this.ucMultiSelectDCL4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select TVD.Codice, TVD.Descrizione + ' (' + TD.Descrizione + ') (' + TVD.Codice + ')' As [Diario Clinico] From T_TipoVoceDiario TVD" + Environment.NewLine +
                                                "Inner Join T_TipoDiario TD On TVD.CodTipoDiario = TD.Codice" + Environment.NewLine +
                                                "Where TVD.Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY TVD.Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectDCL4.Tag.ToString());
                        this.ucMultiSelectDCL4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectDCL4.ViewInit();
                        this.ucMultiSelectDCL4.RefreshData();
                        this.ucMultiSelectPRF4.ViewShowAll = true;
                        this.ucMultiSelectPRF4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As Prescrizione From T_TipoPrescrizione" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectPRF4.Tag.ToString());
                        this.ucMultiSelectPRF4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As Prescrizione From T_TipoPrescrizione" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectPRF4.Tag.ToString());
                        this.ucMultiSelectPRF4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectPRF4.ViewInit();
                        this.ucMultiSelectPRF4.RefreshData();
                        this.ucMultiSelectPVT4.ViewShowAll = true;
                        this.ucMultiSelectPVT4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Parametri Vitali] From T_TipoParametroVitale" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectPVT4.Tag.ToString());
                        this.ucMultiSelectPVT4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Parametri Vitali] From T_TipoParametroVitale" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectPVT4.Tag.ToString());
                        this.ucMultiSelectPVT4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectPVT4.ViewInit();
                        this.ucMultiSelectPVT4.RefreshData();
                        this.ucMultiSelectCSG4.ViewShowAll = true;
                        this.ucMultiSelectCSG4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Consegna] From T_TipoConsegna" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectCSG4.Tag.ToString());
                        this.ucMultiSelectCSG4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Consegna] From T_TipoConsegna" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectCSG4.Tag.ToString());
                        this.ucMultiSelectCSG4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectCSG4.ViewInit();
                        this.ucMultiSelectCSG4.RefreshData();
                        this.ucMultiSelectCSP4.ViewShowAll = true;
                        this.ucMultiSelectCSP4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Consegna] From T_TipoConsegnaPaziente" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectCSP4.Tag.ToString());
                        this.ucMultiSelectCSP4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Consegna] From T_TipoConsegnaPaziente" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectCSP4.Tag.ToString());
                        this.ucMultiSelectCSP4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectCSP4.ViewInit();
                        this.ucMultiSelectCSP4.RefreshData();
                        this.ucMultiSelectRPT4.ViewShowAll = true;
                        this.ucMultiSelectRPT4.ViewShowFind = true;
                        sSql = string.Format("Select R.Codice, R.Descrizione + ' (' + FR.Descrizione + ') (' + R.Codice + ')' As Report From T_Report R" + Environment.NewLine +
                                                "Inner Join T_FormatoReport FR On R.CodFormatoReport = FR.Codice" + Environment.NewLine +
                                                "Where R.Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY R.Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectRPT4.Tag.ToString());
                        this.ucMultiSelectRPT4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select R.Codice, R.Descrizione + ' (' + FR.Descrizione + ') (' + R.Codice + ')' As Report From T_Report R" + Environment.NewLine +
                                                "Inner Join T_FormatoReport FR On R.CodFormatoReport = FR.Codice" + Environment.NewLine +
                                                "Where R.Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY R.Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectRPT4.Tag.ToString());
                        this.ucMultiSelectRPT4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectRPT4.ViewInit();
                        this.ucMultiSelectRPT4.RefreshData();
                        this.ucMultiSelectSCH4.ViewShowAll = true;
                        this.ucMultiSelectSCH4.ViewShowFind = true;
                        sSql = string.Format("Select S.Codice, S.Descrizione + ' (' + TS.Descrizione + ') (' + S.Codice + ')' As Schede From T_Schede S" + Environment.NewLine +
                                                "Inner Join T_TipoScheda TS On S.CodTipoScheda = TS.Codice" + Environment.NewLine +
                                                "Where S.Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')" + Environment.NewLine +
                                                "And IsNull(SchedaSemplice, 0) = 0 ORDER BY S.Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectSCH4.Tag.ToString());
                        this.ucMultiSelectSCH4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select S.Codice, S.Descrizione + ' (' + TS.Descrizione + ') (' + S.Codice + ')' As Schede From T_Schede S" + Environment.NewLine +
                                                "Inner Join T_TipoScheda TS On S.CodTipoScheda = TS.Codice" + Environment.NewLine +
                                                "Where S.Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}')" + Environment.NewLine +
                                                "And IsNull(SchedaSemplice, 0) = 0  ORDER BY S.Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectSCH4.Tag.ToString());
                        this.ucMultiSelectSCH4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectSCH4.ViewInit();
                        this.ucMultiSelectSCH4.RefreshData();
                        this.ucMultiSelectTST4.ViewShowAll = true;
                        this.ucMultiSelectTST4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Testi Predefiniti] From T_TestiPredefiniti" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectTST4.Tag.ToString());
                        this.ucMultiSelectTST4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Testi Predefiniti] From T_TestiPredefiniti" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectTST4.Tag.ToString());
                        this.ucMultiSelectTST4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectTST4.ViewInit();
                        this.ucMultiSelectTST4.RefreshData();
                        this.ucMultiSelectTNT4.ViewShowAll = true;
                        this.ucMultiSelectTNT4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Testi Note Predefiniti] From T_TestiNotePredefiniti" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectTNT4.Tag.ToString());
                        this.ucMultiSelectTNT4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Testi Note Predefiniti] From T_TestiNotePredefiniti" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectTNT4.Tag.ToString());
                        this.ucMultiSelectTNT4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectTNT4.ViewInit();
                        this.ucMultiSelectTNT4.RefreshData();
                        this.ucMultiSelectWKI4.ViewShowAll = true;
                        this.ucMultiSelectWKI4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Task Infermieristici] From T_TipoTaskInfermieristico" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectWKI4.Tag.ToString());
                        this.ucMultiSelectWKI4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Task Infermieristici] From T_TipoTaskInfermieristico" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectWKI4.Tag.ToString());
                        this.ucMultiSelectWKI4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectWKI4.ViewInit();
                        this.ucMultiSelectWKI4.RefreshData();
                        this.ucMultiSelectRuoliVisione4.ViewShowAll = true;
                        this.ucMultiSelectRuoliVisione4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As Ruolo From T_Ruoli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodRuolo From T_AssUARuoliCartellaInVisione Where CodUA = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text);
                        this.ucMultiSelectRuoliVisione4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As Ruolo From T_Ruoli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodRuolo From T_AssUARuoliCartellaInVisione Where CodUA = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice4.Text);
                        this.ucMultiSelectRuoliVisione4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectRuoliVisione4.ViewInit();
                        this.ucMultiSelectRuoliVisione4.RefreshData();
                        this.ucMultiSelectEBM4.ViewShowAll = true;
                        this.ucMultiSelectEBM4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Banche Dati] From T_EBM" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectEBM4.Tag.ToString());
                        this.ucMultiSelectEBM4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Banche Dati] From T_EBM" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectEBM4.Tag.ToString());
                        this.ucMultiSelectEBM4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectEBM4.ViewInit();
                        this.ucMultiSelectEBM4.RefreshData();
                        this.ucMultiSelectALG4.ViewShowAll = true;
                        this.ucMultiSelectALG4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Alert Generici] From T_TipoAlertGenerico" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectALG4.Tag.ToString());
                        this.ucMultiSelectALG4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As [Alert Generici] From T_TipoAlertGenerico" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC", "In", this.uteCodice4.Text, this.ucMultiSelectALG4.Tag.ToString());
                        this.ucMultiSelectALG4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectALG4.ViewInit();
                        this.ucMultiSelectALG4.RefreshData();
                        this.ucMultiSelectNWS4.ViewShowAll = true;
                        this.ucMultiSelectNWS4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Titolo + ' (' + Codice + ')' As News From T_MovNews" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Titolo ASC", "Not In", this.uteCodice4.Text, this.ucMultiSelectNWS4.Tag.ToString());
                        this.ucMultiSelectNWS4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Titolo + ' (' + Codice + ')' As News From T_MovNews" + Environment.NewLine +
                                                "Where Codice {0} (Select CodVoce From T_AssUAEntita Where CodUA = '{1}' And CodEntita = '{2}') ORDER BY Titolo ASC", "In", this.uteCodice4.Text, this.ucMultiSelectNWS4.Tag.ToString());
                        this.ucMultiSelectNWS4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectNWS4.ViewInit();
                        this.ucMultiSelectNWS4.RefreshData();
                        this.ucMultiSelectUA4.ViewShowAll = true;
                        this.ucMultiSelectUA4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As UA From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUACollegata From T_AssUAUACollegate Where CodUA = '{1}') And Codice <> '{1}' ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text);
                        this.ucMultiSelectUA4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As UA From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUACollegata From T_AssUAUACollegate Where CodUA = '{1}') ORDER BY Descrizione ASC", "In", this.uteCodice4.Text);
                        this.ucMultiSelectUA4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA4.ViewInit();
                        this.ucMultiSelectUA4.RefreshData();
                        this.ucMultiSelectUAPA4.ViewShowAll = true;
                        this.ucMultiSelectUAPA4.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As UA From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUACollegata From T_AssUAUACollegatePA Where CodUA = '{1}') ORDER BY Descrizione ASC", "Not In", this.uteCodice4.Text);
                        this.ucMultiSelectUAPA4.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As UA From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUACollegata From T_AssUAUACollegatePA Where CodUA = '{1}') ORDER BY Descrizione ASC", "In", this.uteCodice4.Text);
                        this.ucMultiSelectUAPA4.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUAPA4.ViewInit();
                        this.ucMultiSelectUAPA4.RefreshData();
                        LoadUltraTreeIntestazioni(null);
                        this.utvIntestazioni.ActiveNode = this.utvIntestazioni.GetNodeByKey(CoreStatics.TV_ROOT);
                        break;

                    case Enums.EnumDataNames.T_DiarioInfermieristico:
                    case Enums.EnumDataNames.T_DiarioMedico:
                        this.ucMultiSelectUA5.ViewShowAll = true;
                        this.ucMultiSelectUA5.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice5.Text, this.ucMultiSelectUA5.Tag.ToString());
                        this.ucMultiSelectUA5.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice5.Text, this.ucMultiSelectUA5.Tag.ToString());
                        this.ucMultiSelectUA5.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA5.ViewInit();
                        this.ucMultiSelectUA5.RefreshData();
                        this.ucMultiSelectPlusRuoli5.ViewShowAll = true;

                        this.ucMultiSelectPlusRuoli5.ViewShowFind = true; this.ucMultiSelectPlusRuoli5.GridDXFilterColumnIndex = 2;
                        this.ucMultiSelectPlusRuoli5.GridSXFilterColumnIndex = 2;

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
                                                "Order By QAE.CodAzione, R.Descrizione ASC", "DCL", this.uteCodice5.Text);
                        this.ucMultiSelectPlusRuoli5.ViewDataSetSX = DataBase.GetDataSet(sSql);
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
                                                "Order By ASS.CodAzione, R.Descrizione ASC", "DCL", this.uteCodice5.Text);
                        this.ucMultiSelectPlusRuoli5.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                                "From T_AzioniEntita AE" + Environment.NewLine +
                                                        "Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                                        "Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                                "Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                                        "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                        "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", "DCL");
                        this.ucMultiSelectPlusRuoli5.ViewDataSetMaster = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectPlusRuoli5.ViewInit();
                        this.ucMultiSelectPlusRuoli5.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_TipoParametroVitale:
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
                                                "Order By QAE.CodAzione, R.Descrizione ASC", "PVT", this.uteCodice6.Text);
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
                                                "Order By ASS.CodAzione, R.Descrizione ASC", "PVT", this.uteCodice6.Text);
                        this.ucMultiSelectPlusRuoli6.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                                "From T_AzioniEntita AE" + Environment.NewLine +
                                                        "Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                                        "Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                                "Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                                        "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                        "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", "PVT");
                        this.ucMultiSelectPlusRuoli6.ViewDataSetMaster = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectPlusRuoli6.ViewInit();
                        this.ucMultiSelectPlusRuoli6.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_Report:
                        this.ucMultiSelectUA8.ViewShowAll = true;
                        this.ucMultiSelectUA8.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}') ORDER BY Descrizione ASC", "Not In", this.uteCodice8.Text, this.ucMultiSelectUA8.Tag.ToString());
                        this.ucMultiSelectUA8.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice8.Text, this.ucMultiSelectUA8.Tag.ToString());
                        this.ucMultiSelectUA8.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA8.ViewInit();
                        this.ucMultiSelectUA8.RefreshData();

                        this.ucMultiSelectMaschere8.ViewShowAll = true;
                        this.ucMultiSelectMaschere8.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As Maschera From T_Maschere" + Environment.NewLine +
                                                "Where Codice {0} (Select CodMaschera From T_AssReportMaschere Where CodReport = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice8.Text);
                        this.ucMultiSelectMaschere8.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As Maschera From T_Maschere" + Environment.NewLine +
                                                "Where Codice {0} (Select CodMaschera From T_AssReportMaschere Where CodReport = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice8.Text);
                        this.ucMultiSelectMaschere8.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectMaschere8.ViewInit();
                        this.ucMultiSelectMaschere8.RefreshData();

                        this.ucMultiSelectPlusRuoli8.ViewShowAll = true;

                        this.ucMultiSelectPlusRuoli8.ViewShowFind = true; this.ucMultiSelectPlusRuoli8.GridDXFilterColumnIndex = 2;
                        this.ucMultiSelectPlusRuoli8.GridSXFilterColumnIndex = 2;

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
                                                "Order By QAE.CodAzione, R.Descrizione ASC", "RPT", this.uteCodice8.Text);
                        this.ucMultiSelectPlusRuoli8.ViewDataSetSX = DataBase.GetDataSet(sSql);
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
                                                "Order By ASS.CodAzione, R.Descrizione ASC", "RPT", this.uteCodice8.Text);
                        this.ucMultiSelectPlusRuoli8.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                                "From T_AzioniEntita AE" + Environment.NewLine +
                                                        "Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                                        "Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                                "Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                                        "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                        "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", "RPT");
                        this.ucMultiSelectPlusRuoli8.ViewDataSetMaster = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectPlusRuoli8.ViewInit();
                        this.ucMultiSelectPlusRuoli8.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_Maschere:
                        this.ucMultiSelectReport9.ViewShowAll = true;
                        this.ucMultiSelectReport9.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As Report From T_Report" + Environment.NewLine +
                                                "Where Codice {0} (Select CodReport From T_AssReportMaschere Where CodMaschera = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice9.Text);
                        this.ucMultiSelectReport9.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As Report From T_Report" + Environment.NewLine +
                                                "Where Codice {0} (Select CodReport From T_AssReportMaschere Where CodMaschera = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice9.Text);
                        this.ucMultiSelectReport9.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectReport9.ViewInit();
                        this.ucMultiSelectReport9.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_TestiPredefiniti:
                        this.ucMultiSelectUA10.ViewShowAll = true;
                        this.ucMultiSelectUA10.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice10.Text, this.ucMultiSelectUA10.Tag.ToString());
                        this.ucMultiSelectUA10.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice10.Text, this.ucMultiSelectUA10.Tag.ToString());
                        this.ucMultiSelectUA10.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA10.ViewInit();
                        this.ucMultiSelectUA10.RefreshData();
                        LoadTestiPredefiniti();
                        CaricaTestiPredefiniti(this.chkTP10.Checked);
                        break;

                    case Enums.EnumDataNames.T_MovNews:
                        this.ucMultiSelectUA11.ViewShowAll = true;
                        this.ucMultiSelectUA11.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice11.Text, this.ucMultiSelectUA11.Tag.ToString());
                        this.ucMultiSelectUA11.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice11.Text, this.ucMultiSelectUA11.Tag.ToString());
                        this.ucMultiSelectUA11.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA11.ViewInit();
                        this.ucMultiSelectUA11.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_TipoPrescrizione:
                        this.ucMultiSelectUA20.ViewShowAll = true;
                        this.ucMultiSelectUA20.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice20.Text, this.ucMultiSelectUA20.Tag.ToString());
                        this.ucMultiSelectUA20.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice20.Text, this.ucMultiSelectUA20.Tag.ToString());
                        this.ucMultiSelectUA20.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA20.ViewInit();
                        this.ucMultiSelectUA20.RefreshData();

                        this.ucMultiSelectProtocolli20.ViewShowAll = true;
                        this.ucMultiSelectProtocolli20.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Protocolli] From T_Protocolli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodProtocollo From T_AssTipoPrescrizioneProtocolli Where CodTipoPrescrizione = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice20.Text);
                        this.ucMultiSelectProtocolli20.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Protocolli] From T_Protocolli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodProtocollo From T_AssTipoPrescrizioneProtocolli Where CodTipoPrescrizione = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice20.Text);
                        this.ucMultiSelectProtocolli20.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectProtocolli20.ViewInit();
                        this.ucMultiSelectProtocolli20.RefreshData();

                        this.ucMultiSelectTempi20.ViewShowAll = true;
                        this.ucMultiSelectTempi20.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Tempi] From T_TipoPrescrizioneTempi" + Environment.NewLine +
                                                "Where Codice {0} (Select CodTipoPrescrizioneTempi From T_AssTipoPrescrizioneTempi Where CodTipoPrescrizione = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice20.Text);
                        this.ucMultiSelectTempi20.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Tempi] From T_TipoPrescrizioneTempi" + Environment.NewLine +
                                                "Where Codice {0} (Select CodTipoPrescrizioneTempi From T_AssTipoPrescrizioneTempi Where CodTipoPrescrizione = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice20.Text);
                        this.ucMultiSelectTempi20.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectTempi20.ViewInit();
                        this.ucMultiSelectTempi20.RefreshData();

                        this.ucMultiSelectPlusRuoli20.ViewShowAll = true;

                        this.ucMultiSelectPlusRuoli20.ViewShowFind = true; this.ucMultiSelectPlusRuoli20.GridDXFilterColumnIndex = 2;
                        this.ucMultiSelectPlusRuoli20.GridSXFilterColumnIndex = 2;

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
                                                "Order By QAE.CodAzione, R.Descrizione ASC", "PRF", this.uteCodice20.Text);
                        this.ucMultiSelectPlusRuoli20.ViewDataSetSX = DataBase.GetDataSet(sSql);
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
                                                "Order By ASS.CodAzione, R.Descrizione ASC", "PRF", this.uteCodice20.Text);
                        this.ucMultiSelectPlusRuoli20.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                                "From T_AzioniEntita AE" + Environment.NewLine +
                                                        "Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                                        "Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                                "Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                                        "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                        "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", "PRF");
                        this.ucMultiSelectPlusRuoli20.ViewDataSetMaster = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectPlusRuoli20.ViewInit();
                        this.ucMultiSelectPlusRuoli20.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_TestiNotePredefiniti:
                        this.ucMultiSelectUA22.ViewShowAll = true;
                        this.ucMultiSelectUA22.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice22.Text, this.ucMultiSelectUA22.Tag.ToString());
                        this.ucMultiSelectUA22.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice22.Text, this.ucMultiSelectUA22.Tag.ToString());
                        this.ucMultiSelectUA22.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA22.ViewInit();
                        this.ucMultiSelectUA22.RefreshData();
                        break;

                    default:
                        break;

                }

            }
            catch (Exception)
            {

            }

        }

        private void UpdateBindings()
        {

            try
            {

                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_Login:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Foto"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectFoto3.ViewImage);
                        break;

                    case Enums.EnumDataNames.T_UnitaAtomiche:
                        if (this.uteCodUANumerazioneCartella4.Text == string.Empty)
                        {
                            this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodUANumerazioneCartella"] = null;
                        }
                        break;

                    case Enums.EnumDataNames.T_TipoParametroVitale:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona6.ViewImage);
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore6.Value.ToString();
                        break;

                    case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                    case Enums.EnumDataNames.T_TipoOrdine:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona7.ViewImage);
                        break;

                    case Enums.EnumDataNames.T_ViaSomministrazione:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona13.ViewImage);
                        break;

                    case Enums.EnumDataNames.T_Report:
                        if (this.uceCodFormatoReport8.Value == null || this.uceCodFormatoReport8.Value.ToString().ToUpper() != MyStatics.FR_REM.ToUpper())
                        {
                            if (this.uceCodFormatoReport8.Value.ToString().ToUpper() == MyStatics.FR_CABLATO.ToUpper())
                                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Parametri"] = this.xmlParametri8.Text;
                            else
                                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Parametri"] = "";

                            this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodReportVista"] = null;
                            if (this.utePercorsoFile8.Text != "" && System.IO.File.Exists(this.utePercorsoFile8.Text))
                                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Modello"] = Scci.Statics.FileStatics.GetBytesFromFile(this.utePercorsoFile8.Text);

                        }
                        else
                        {
                            if (Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ReMConnectionString).Trim() != "")
                                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Parametri"] = this.reportLinkConfig8.reportLinkSerialized;

                            if (this.uteCodReportVista8.Text == "") this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodReportVista"] = null;
                        }
                        if (!this.uteNomePlugin8.Visible) this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["NomePlugIn"] = null;
                        break;

                    case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona14.ViewImage);
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore14.Value.ToString();
                        break;

                    case Enums.EnumDataNames.T_StatoAppuntamento:
                    case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                    case Enums.EnumDataNames.T_StatoAllegato:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona15.ViewImage);
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore15.Value.ToString();
                        break;

                    case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                    case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                    case Enums.EnumDataNames.T_StatoAlertGenerico:
                    case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                    case Enums.EnumDataNames.T_StatoParametroVitale:
                    case Enums.EnumDataNames.T_StatoConsegna:
                    case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                    case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                    case Enums.EnumDataNames.T_StatoScheda:
                    case Enums.EnumDataNames.T_StatoPrescrizione:
                    case Enums.EnumDataNames.T_StatoDiario:
                    case Enums.EnumDataNames.T_StatoTrasferimento:
                    case Enums.EnumDataNames.T_TipoAllegato:
                    case Enums.EnumDataNames.T_FormatoAllegati:
                    case Enums.EnumDataNames.T_StatoOrdine:
                    case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                    case Enums.EnumDataNames.T_StatoContinuazione:
                    case Enums.EnumDataNames.T_StatoCartella:
                    case Enums.EnumDataNames.T_StatoCartellaInfo:
                    case Enums.EnumDataNames.T_StatoCartellaInVisione:
                    case Enums.EnumDataNames.T_StatoEpisodio:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona17.ViewImage);
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore17.Value.ToString();
                        break;

                    case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona29.ViewImage);
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore29.Value.ToString();
                        break;

                    case Enums.EnumDataNames.T_Sistemi:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona27.ViewImage);
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore27.Value.ToString();
                        break;

                    case Enums.EnumDataNames.T_FormatoReport:
                    case Enums.EnumDataNames.T_TipoEpisodio:
                    case Enums.EnumDataNames.T_TipoDiario:
                    case Enums.EnumDataNames.T_TipoScheda:
                    case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona19.ViewImage);
                        break;

                    case Enums.EnumDataNames.T_TipoPrescrizione:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona20.ViewImage);
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["ColoreGrafico"] = this.ucpColoreGrafico20.Value.ToString();
                        break;

                    case Enums.EnumDataNames.T_TestiNotePredefiniti:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore22.Value.ToString();
                        break;

                    default:
                        break;

                }

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

                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_Ruoli:
                        if (this.ucMultiSelectModuli2.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssRuoliModuli" + Environment.NewLine +
                                    "Where CodRuolo = '" + this.uteCodice2.Text + "'" + Environment.NewLine +
                                    "And CodModulo = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectModuli2.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectModuli2.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssRuoliModuli (CodRuolo, CodModulo)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice2.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectModuli2.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA2.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssRuoliUA" + Environment.NewLine +
                                    "Where CodRuolo = '" + this.uteCodice2.Text + "'" + Environment.NewLine +
                                    "And CodUA = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA2.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA2.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssRuoliUA (CodRuolo, CodUA)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice2.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA2.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectLogin2.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssLoginRuoli" + Environment.NewLine +
                                    "Where CodRuolo = '" + this.uteCodice2.Text + "'" + Environment.NewLine +
                                    "And CodLogin = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectLogin2.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectLogin2.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssLoginRuoli (CodRuolo, CodLogin)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice2.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectLogin2.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUAVisione2.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUARuoliCartellaInVisione" + Environment.NewLine +
                                    "Where CodRuolo = '" + this.uteCodice2.Text + "'" + Environment.NewLine +
                                    "And CodUA = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUAVisione2.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUAVisione2.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUARuoliCartellaInVisione (CodRuolo, CodUA)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice2.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUAVisione2.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }

                        UpdateRuoliAzioni();

                        break;

                    case Enums.EnumDataNames.T_Login:
                        if (this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                        {
                            sSql = "Insert Into T_AssLoginRuoli (CodLogin, CodRuolo)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice3.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectRuoli3.ViewDataSetDX, "Codice", sSql);
                        }
                        else
                        {
                            if (this.ucMultiSelectRuoli3.ViewDataSetSX.HasChanges() == true)
                            {
                                sSql = "Delete from T_AssLoginRuoli" + Environment.NewLine +
                                        "Where CodLogin = '" + this.uteCodice3.Text + "'" + Environment.NewLine +
                                        "And CodRuolo = '{0}'";
                                UpdateBindingsAssDataSet(this.ucMultiSelectRuoli3.ViewDataSetSX.GetChanges(), "Codice", sSql);
                            }
                            if (this.ucMultiSelectRuoli3.ViewDataSetDX.HasChanges() == true)
                            {
                                sSql = "Insert Into T_AssLoginRuoli (CodLogin, CodRuolo)" + Environment.NewLine +
                                        "Values ('" + this.uteCodice3.Text + "', '{0}')";
                                UpdateBindingsAssDataSet(this.ucMultiSelectRuoli3.ViewDataSetDX.GetChanges(), "Codice", sSql);
                            }
                        }

                        break;

                    case Enums.EnumDataNames.T_UnitaAtomiche:

                        if (this.ucMultiSelectRuoli4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssRuoliUA" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodRuolo = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectRuoli4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectRuoli4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssRuoliUA (CodUA, CodRuolo)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectRuoli4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectModuli4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAModuli" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodModulo = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectModuli4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectModuli4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAModuli (CodUA, CodModulo)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectModuli4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectAGE4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectAGE4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectAGE4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectAGE4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectAGE4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectAGE4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectAPP4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectAPP4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectAPP4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectAPP4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectAPP4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectAPP4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectDCL4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectDCL4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectDCL4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectDCL4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectDCL4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectDCL4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectPRF4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectPRF4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPRF4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectPRF4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectPRF4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPRF4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectPVT4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectPVT4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPVT4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectPVT4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectPVT4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPVT4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectCSG4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectCSG4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectCSG4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectCSG4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectCSG4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectCSG4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectCSP4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectCSP4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectCSP4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectCSP4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectCSP4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectCSP4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectRPT4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectRPT4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectRPT4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectRPT4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectRPT4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectRPT4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectSCH4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectSCH4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectSCH4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectSCH4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectSCH4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectSCH4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectTST4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectTST4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTST4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectTST4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectTST4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTST4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectTNT4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectTNT4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTNT4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectTNT4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectTNT4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTNT4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectWKI4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectWKI4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectWKI4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectWKI4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectWKI4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectWKI4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectRuoliVisione4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUARuoliCartellaInVisione" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodRuolo = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectRuoliVisione4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectRuoliVisione4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUARuoliCartellaInVisione (CodUA, CodRuolo)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectRuoliVisione4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectEBM4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectEBM4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectEBM4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectEBM4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectEBM4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectEBM4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectALG4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectALG4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectALG4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectALG4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectALG4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectALG4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectNWS4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectNWS4.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodVoce = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectNWS4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectNWS4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '" + this.ucMultiSelectNWS4.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectNWS4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAUACollegate" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodUACollegata = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAUACollegate (CodUA, CodUACollegata)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUAPA4.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAUACollegatePA" + Environment.NewLine +
                                    "Where CodUA = '" + this.uteCodice4.Text + "'" + Environment.NewLine +
                                    "And CodUACollegata = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUAPA4.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUAPA4.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAUACollegatePA (CodUA, CodUACollegata)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice4.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUAPA4.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_DiarioInfermieristico:
                    case Enums.EnumDataNames.T_DiarioMedico:
                        if (this.ucMultiSelectUA5.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodVoce = '" + this.uteCodice5.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectUA5.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodUA = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA5.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA5.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice5.Text + "', '" + this.ucMultiSelectUA5.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA5.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectPlusRuoli5.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                                    "Where CodVoce = '" + DataBase.Ax2(this.uteCodice5.Text) + "'" + Environment.NewLine +
                                            "And CodEntita = '" + Scci.Enums.EnumEntita.DCL.ToString() + "'" + Environment.NewLine +
                                            "And CodRuolo = '{0}'" + Environment.NewLine +
                                            "And CodAzione = '{1}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli5.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        }
                        if (this.ucMultiSelectPlusRuoli5.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                                   "Values ('{0}', '" + Scci.Enums.EnumEntita.DCL.ToString() + "', '" + DataBase.Ax2(this.uteCodice5.Text) + @"', '{1}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli5.ViewDataSetDX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_TipoParametroVitale:
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
                                            "And CodEntita = '" + Scci.Enums.EnumEntita.PVT.ToString() + "'" + Environment.NewLine +
                                            "And CodRuolo = '{0}'" + Environment.NewLine +
                                            "And CodAzione = '{1}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli6.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        }
                        if (this.ucMultiSelectPlusRuoli6.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                                   "Values ('{0}', '" + Scci.Enums.EnumEntita.PVT.ToString() + "', '" + DataBase.Ax2(this.uteCodice6.Text) + @"', '{1}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli6.ViewDataSetDX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_Report:

                        if (this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
        "Values ('" + this.uteCodice8.Text + "', '" + this.ucMultiSelectUA8.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA8.ViewDataSetDX, "Codice", sSql);

                            sSql = "Insert Into T_AssReportMaschere (CodReport, CodMaschera)" + Environment.NewLine +
        "Values ('" + this.uteCodice8.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectMaschere8.ViewDataSetDX, "Codice", sSql);

                            sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
       "Values ('{0}', '" + Scci.Enums.EnumEntita.RPT.ToString() + "', '" + DataBase.Ax2(this.uteCodice8.Text) + @"', '{1}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli8.ViewDataSetDX, "CodRuolo", "CodAzione", sSql);
                        }
                        else
                        {
                            if (this.ucMultiSelectUA8.ViewDataSetSX.HasChanges() == true)
                            {
                                sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                        "Where CodVoce = '" + this.uteCodice8.Text + "'" + Environment.NewLine +
                                        "And CodEntita = '" + this.ucMultiSelectUA8.Tag.ToString() + "'" + Environment.NewLine +
                                        "And CodUA = '{0}'";
                                UpdateBindingsAssDataSet(this.ucMultiSelectUA8.ViewDataSetSX.GetChanges(), "Codice", sSql);
                            }
                            if (this.ucMultiSelectUA8.ViewDataSetDX.HasChanges() == true)
                            {
                                sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                                        "Values ('" + this.uteCodice8.Text + "', '" + this.ucMultiSelectUA8.Tag.ToString() + "', '{0}')";
                                UpdateBindingsAssDataSet(this.ucMultiSelectUA8.ViewDataSetDX.GetChanges(), "Codice", sSql);
                            }
                            if (this.ucMultiSelectMaschere8.ViewDataSetSX.HasChanges() == true)
                            {
                                sSql = "Delete from T_AssReportMaschere" + Environment.NewLine +
                                        "Where CodReport = '" + this.uteCodice8.Text + "'" + Environment.NewLine +
                                        "And CodMaschera = '{0}'";
                                UpdateBindingsAssDataSet(this.ucMultiSelectMaschere8.ViewDataSetSX.GetChanges(), "Codice", sSql);
                            }
                            if (this.ucMultiSelectMaschere8.ViewDataSetDX.HasChanges() == true)
                            {
                                sSql = "Insert Into T_AssReportMaschere (CodReport, CodMaschera)" + Environment.NewLine +
                                        "Values ('" + this.uteCodice8.Text + "', '{0}')";
                                UpdateBindingsAssDataSet(this.ucMultiSelectMaschere8.ViewDataSetDX.GetChanges(), "Codice", sSql);
                            }
                            if (this.ucMultiSelectPlusRuoli8.ViewDataSetSX.HasChanges() == true)
                            {
                                sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                                        "Where CodVoce = '" + DataBase.Ax2(this.uteCodice8.Text) + "'" + Environment.NewLine +
                                                "And CodEntita = '" + Scci.Enums.EnumEntita.RPT.ToString() + "'" + Environment.NewLine +
                                                "And CodRuolo = '{0}'" + Environment.NewLine +
                                                "And CodAzione = '{1}'";
                                UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli8.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                            }
                            if (this.ucMultiSelectPlusRuoli8.ViewDataSetDX.HasChanges() == true)
                            {
                                sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                                       "Values ('{0}', '" + Scci.Enums.EnumEntita.RPT.ToString() + "', '" + DataBase.Ax2(this.uteCodice8.Text) + @"', '{1}')";
                                UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli8.ViewDataSetDX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                            }
                        }

                        break;

                    case Enums.EnumDataNames.T_Maschere:
                        if (this.ucMultiSelectReport9.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssReportMaschere" + Environment.NewLine +
                                    "Where CodMaschera = '" + this.uteCodice9.Text + "'" + Environment.NewLine +
                                    "And CodReport = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectReport9.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectReport9.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssReportMaschere (CodMaschera, CodReport)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice9.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectReport9.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_TestiPredefiniti:
                        if (this.ucMultiSelectUA10.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodVoce = '" + this.uteCodice10.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectUA10.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodUA = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA10.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA10.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice10.Text + "', '" + this.ucMultiSelectUA10.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA10.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (_TestiPredefiniti_Campi.Count > 0 && _TestiPredefiniti_Campi[MyStatics.GC_NUOVO].Rows.Count > 0)
                        {
                            sSql = "Insert Into T_AssTestiPredefinitiCampi (CodTestoPredefinito, CodEntita, CodTipoEntita, CodCampo)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice10.Text + "', '" + this.uteCodEntita10.Text + "', '{0}', '{1}')";
                            UpdateBindingsAssDataTable(_TestiPredefiniti_Campi[MyStatics.GC_NUOVO], "Codice", "ID", sSql);
                        }
                        if (_TestiPredefiniti_Campi.Count > 0 && _TestiPredefiniti_Campi[MyStatics.GC_ELIMINA].Rows.Count > 0)
                        {
                            sSql = "Delete from T_AssTestiPredefinitiCampi" + Environment.NewLine +
                                    "Where CodTestoPredefinito = '" + this.uteCodice10.Text + "'" + Environment.NewLine +
                                            "And CodEntita = '" + this.uteCodEntita10.Text + "'" + Environment.NewLine +
                                            "And CodTipoEntita = '{0}'" + Environment.NewLine +
                                            "And CodCampo = '{1}'";
                            UpdateBindingsAssDataTable(_TestiPredefiniti_Campi[MyStatics.GC_ELIMINA], "Codice", "ID", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_MovNews:
                        if (this.ucMultiSelectUA11.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodVoce = '" + this.uteCodice11.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectUA11.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodUA = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA11.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA11.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice11.Text + "', '" + this.ucMultiSelectUA11.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA11.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_TipoPrescrizione:
                        if (this.ucMultiSelectUA20.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodVoce = '" + this.uteCodice20.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectUA20.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodUA = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA20.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA20.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice20.Text + "', '" + this.ucMultiSelectUA20.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA20.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }

                        if (this.ucMultiSelectProtocolli20.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete From T_AssTipoPrescrizioneProtocolli" + Environment.NewLine +
                                    "Where CodTipoPrescrizione = '" + this.uteCodice20.Text + "'" + Environment.NewLine +
                                    "And CodProtocollo = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectProtocolli20.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectProtocolli20.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssTipoPrescrizioneProtocolli (CodTipoPrescrizione, CodProtocollo)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice20.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectProtocolli20.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }

                        if (this.ucMultiSelectTempi20.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete From T_AssTipoPrescrizioneTempi" + Environment.NewLine +
                                    "Where CodTipoPrescrizione = '" + this.uteCodice20.Text + "'" + Environment.NewLine +
                                    "And CodTipoPrescrizioneTempi = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTempi20.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectTempi20.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssTipoPrescrizioneTempi (CodTipoPrescrizione, CodTipoPrescrizioneTempi)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice20.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTempi20.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectPlusRuoli20.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                                    "Where CodVoce = '" + DataBase.Ax2(this.uteCodice20.Text) + "'" + Environment.NewLine +
                                            "And CodEntita = '" + Scci.Enums.EnumEntita.PRF.ToString() + "'" + Environment.NewLine +
                                            "And CodRuolo = '{0}'" + Environment.NewLine +
                                            "And CodAzione = '{1}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli20.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        }
                        if (this.ucMultiSelectPlusRuoli20.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                                   "Values ('{0}', '" + Scci.Enums.EnumEntita.PRF.ToString() + "', '" + DataBase.Ax2(this.uteCodice20.Text) + @"', '{1}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli20.ViewDataSetDX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_TestiNotePredefiniti:
                        if (this.ucMultiSelectUA22.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                    "Where CodVoce = '" + this.uteCodice22.Text + "'" + Environment.NewLine +
                                    "And CodEntita = '" + this.ucMultiSelectUA22.Tag.ToString() + "'" + Environment.NewLine +
                                    "And CodUA = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA22.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA22.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice22.Text + "', '" + this.ucMultiSelectUA22.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA22.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        break;

                    default:
                        break;

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

                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_Ruoli:
                        sSql = "Delete from T_AssRuoliModuli Where CodRuolo = '" + this.uteCodice2.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssRuoliUA Where CodRuolo = '" + this.uteCodice2.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssLoginRuoli Where CodRuolo = '" + this.uteCodice2.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssRuoliAzioni Where CodRuolo = '" + this.uteCodice2.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_Login:
                        sSql = "Delete from T_AssLoginRuoli Where CodLogin = '" + this.uteCodice3.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_UnitaAtomiche:
                        sSql = "Delete from T_AssRuoliUA Where CodUA = '" + DataBase.Ax2(this.uteCodice4.Text) + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAModuli Where CodUA = '" + DataBase.Ax2(this.uteCodice4.Text) + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectAGE4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectAPP4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectDCL4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectPRF4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectPVT4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectCSG4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectCSP4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectRPT4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectSCH4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectTST4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectTNT4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectWKI4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectEBM4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectALG4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAEntita Where CodUA = '" + this.uteCodice4.Text + "' And CodEntita = '" + this.ucMultiSelectNWS4.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUAUACollegate Where CodUA = '" + DataBase.Ax2(this.uteCodice4.Text) + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssUARuoliCartellaInVisione  Where CodUA = '" + DataBase.Ax2(this.uteCodice4.Text) + "'";
                        DataBase.ExecuteSql(sSql);

                        break;

                    case Enums.EnumDataNames.T_DiarioInfermieristico:
                    case Enums.EnumDataNames.T_DiarioMedico:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice5.Text + "' And CodEntita = '" + this.ucMultiSelectUA5.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice5.Text) + "'" + Environment.NewLine +
                    "And CodEntita = '" + Scci.Enums.EnumEntita.DCL.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_TipoParametroVitale:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice6.Text + "' And CodEntita = '" + this.ucMultiSelectUA6.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice6.Text) + "'" + Environment.NewLine +
                    "And CodEntita = '" + Scci.Enums.EnumEntita.PVT.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_Report:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice8.Text + "' And CodEntita = '" + this.ucMultiSelectUA8.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssReportMaschere Where CodReport = '" + this.uteCodice8.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice8.Text) + "'" + Environment.NewLine +
                    "And CodEntita = '" + Scci.Enums.EnumEntita.RPT.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_Maschere:
                        sSql = "Delete from T_AssReportMaschere Where CodMaschera = '" + this.uteCodice9.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_TestiPredefiniti:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice10.Text + "' And CodEntita = '" + this.ucMultiSelectUA10.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_MovNews:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice11.Text + "' And CodEntita = '" + this.ucMultiSelectUA11.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_TipoPrescrizione:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice20.Text + "' And CodEntita = '" + this.ucMultiSelectUA20.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);

                        sSql = "Delete from T_AssTipoPrescrizioneProtocolli Where CodTipoPrescrizione = '" + this.uteCodice20.Text + "'";
                        DataBase.ExecuteSql(sSql);

                        sSql = "Delete from T_AssTipoPrescrizioneTempi Where CodTipoPrescrizione = '" + this.uteCodice20.Text + "'";
                        DataBase.ExecuteSql(sSql);

                        sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice20.Text) + "'" + Environment.NewLine +
                    "And CodEntita = '" + Scci.Enums.EnumEntita.PRF.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_TestiNotePredefiniti:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice22.Text + "' And CodEntita = '" + this.ucMultiSelectUA22.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    default:
                        break;

                }

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

                switch (this.ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_Login:
                        this.uteCodice3.Text = string.Empty;
                        this.uteDescrizione3.Text = string.Empty;
                        this.uteCognome3.Text = string.Empty;
                        this.uteNome3.Text = string.Empty;
                        this.uteCodiceFiscale3.Text = string.Empty;
                        this.ucPictureSelectFoto3.ViewImage = null;
                        break;

                    case Enums.EnumDataNames.T_UnitaAtomiche:
                        _CodiceOriginale = this.uteCodice4.Text;
                        this.uteCodice4.Text = string.Empty;
                        break;

                    case Enums.EnumDataNames.T_Report:
                        this.uteCodice8.Text = string.Empty;
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "EditBindingsCopia", this.Text);
            }
        }

        private void CopiaBindingsAss()
        {

            string sSql = @"";

            try
            {

                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_UnitaAtomiche:
                        sSql = "Insert Into T_AssRuoliUA (CodUA, CodRuolo)" + Environment.NewLine +
       String.Format("Select '{0}', CodRuolo From T_AssRuoliUA Where CodUA = '{1}'", this.uteCodice4.Text, _CodiceOriginale);
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAModuli (CodUA, CodModulo)" + Environment.NewLine +
        String.Format("Select '{0}', CodModulo From T_AssUAModuli Where CodUA = '{1}'", this.uteCodice4.Text, _CodiceOriginale);
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectAGE4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectAPP4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectDCL4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectPRF4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectPVT4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectCSG4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectCSP4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectRPT4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectSCH4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectTST4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectTNT4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectWKI4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUARuoliCartellaInVisione (CodUA, CodRuolo)" + Environment.NewLine +
        String.Format("Select '{0}', CodRuolo From T_AssUARuoliCartellaInVisione Where CodUA = '{1}'", this.uteCodice4.Text, _CodiceOriginale);
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectEBM4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectALG4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAEntita (CodUA, CodEntita, CodVoce)" + Environment.NewLine +
        String.Format("Select '{0}', '{2}', CodVoce From T_AssUAEntita Where CodUA = '{1}' AND CodEntita='{2}'", this.uteCodice4.Text, _CodiceOriginale, this.ucMultiSelectNWS4.Tag.ToString());
                        DataBase.ExecuteSql(sSql);

                        sSql = "Insert Into T_AssUAUACollegate (CodUA, CodUACollegata)" + Environment.NewLine +
        String.Format("Select '{0}', CodUACollegata From T_AssUAUACollegate Where CodUA = '{1}'", this.uteCodice4.Text, _CodiceOriginale);
                        DataBase.ExecuteSql(sSql);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "CopiaBindingsAss", this.Text);
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

            switch (this.ViewDataNamePU)
            {
                case Enums.EnumDataNames.T_Aziende:
                case Enums.EnumDataNames.T_CDSSAzioni:
                    if (bRet && this.uteCodice1.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice1.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice1.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione1.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione1.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione1.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Ruoli:
                    if (bRet && this.uteCodice2.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice2.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice2.Focus();
                        bRet = false;
                    }

                    if (bRet && this.uteDescrizione2.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione2.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione2.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uceCodTipoDiario2.Value == null || this.uceCodTipoDiario2.Value.ToString() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodTipoDiario2.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uceCodTipoDiario2.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteNumMaxCercaEpi2.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblNumMaxCercaEpi2.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteNumMaxCercaEpi2.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Login:
                    if (bRet && this.uteCodice3.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice3.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice3.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione3.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione3.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione3.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCognome3.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCognome3.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCognome3.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteNome3.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblNome3.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteNome3.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_UnitaAtomiche:
                    if (bRet && this.uteCodice4.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice4.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice4.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione4.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione4.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione4.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodice4.Text.Trim().ToUpper() == this.uteCodPadre4.Text.Trim().ToUpper())
                    {
                        MessageBox.Show(this.lblCodPadre4.Text + @" deve essere diverso da " + this.lblCodice4.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodPadre4.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteUltimoNumeroCartella4.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblUltimoNumeroCartella.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteUltimoNumeroCartella4.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodice4.Text.Trim().ToUpper() == this.uteCodUANumerazioneCartella4.Text.Trim().ToUpper())
                    {
                        MessageBox.Show(this.lblCodUANumerazioneCartella4.Text + @" deve essere diverso da " + this.lblCodice4.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodUANumerazioneCartella4.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodUANumerazioneCartella4.Text.Trim() != "" && this.lblCodUANumerazioneCartellaDes4.Text == "")
                    {
                        MessageBox.Show(this.lblCodUANumerazioneCartella4.Text + @" deve esistere!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodUANumerazioneCartella4.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_DiarioInfermieristico:
                case Enums.EnumDataNames.T_DiarioMedico:
                    if (bRet && this.uteCodice5.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice5.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice5.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione5.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione5.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione5.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uteCodScheda5.Text.Trim() == "" || this.lblCodSchedaDes5.Text.Trim() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodScheda5.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodScheda5.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_TipoParametroVitale:
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
                    break;

                case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                case Enums.EnumDataNames.T_TipoOrdine:
                    if (bRet && this.uteCodice7.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice7.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice7.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione7.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione7.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione7.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Report:
                    if (bRet && this.uteCodice8.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice8.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice8.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione8.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione8.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione8.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uceCodFormatoReport8.Value == null || this.uceCodFormatoReport8.Value.ToString() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodFormatoReport8.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uceCodFormatoReport8.Focus();
                        bRet = false;
                    }
                    if (this.uceCodFormatoReport8.Value.ToString().ToUpper() == MyStatics.FR_CABLATO.ToUpper())
                    {
                        if (bRet && !this.xmlParametri8.XmlValidated)
                        {
                            MessageBox.Show(@"Parametri XML sintatticamente non corretti !" + Environment.NewLine + this.xmlParametri8.LastValidateError, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            this.xmlParametri8.Focus();
                            bRet = false;
                        }
                    }

                    if (this.uceCodFormatoReport8.Value.ToString().ToUpper() == MyStatics.FR_WORD.ToUpper())
                    {
                        if (bRet && this.utePercorsoFile8.Text.Trim() != "")
                        {
                            if (!System.IO.File.Exists(this.utePercorsoFile8.Text.Trim()))
                            {
                                string domanda = "";
                                domanda += "Il documento" + Environment.NewLine + this.utePercorsoFile8.Text + Environment.NewLine;
                                domanda += @"non è raggiungibile, e non sarà possibile aggiornare il report." + Environment.NewLine;
                                domanda += @"Vuoi salvare ugualmente le altre impostazioni?";
                                if (MessageBox.Show(domanda, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                                {
                                    this.ubOpenDOCX8.Focus();
                                    bRet = false;
                                }
                            }

                        }
                    }

                    break;

                case Enums.EnumDataNames.T_Maschere:
                    if (bRet && this.uteTimerRefresh9.Text.Trim() != "" && Information.IsNumeric(this.uteTimerRefresh9.Text.Trim()) == false)
                    {
                        MessageBox.Show(@"Inserire valore numerico!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteTimerRefresh9.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_TestiPredefiniti:
                    if (bRet && this.uteCodice10.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice10.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice10.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione10.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione10.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione10.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodEntita10.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodEntita10.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodEntita10.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_MovNews:
                    if (bRet && this.uteCodice11.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice11.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice11.Focus();
                        bRet = false;
                    }
                    if (bRet && this.udteDataOra11.Value == DBNull.Value || this.udteDataOra11.Value == null)
                    {
                        MessageBox.Show(@"Inserire " + this.lblDataOra11.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataOra11.Focus();
                        bRet = false;
                    }
                    if (bRet && this.udteDataInizioPubblicazione11.Value == DBNull.Value || this.udteDataInizioPubblicazione11.Value == null)
                    {
                        MessageBox.Show(@"Inserire " + this.lblDataInizioPubblicazione11.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataInizioPubblicazione11.Focus();
                        bRet = false;
                    }
                    if (bRet && this.udteDataFinePubblicazione11.Value == DBNull.Value || this.udteDataFinePubblicazione11.Value == null)
                    {
                        MessageBox.Show(@"Inserire " + this.lblDataFinePubblicazione11.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataFinePubblicazione11.Focus();
                        bRet = false;
                    }
                    if (bRet && Microsoft.VisualBasic.Information.IsDate(this.udteDataInizioPubblicazione11.Value) &&
                    Microsoft.VisualBasic.Information.IsDate(this.udteDataFinePubblicazione11.Value) &&
                    (DateTime)this.udteDataFinePubblicazione11.Value < (DateTime)this.udteDataInizioPubblicazione11.Value)
                    {
                        MessageBox.Show(@"Data fine pubblicazione minore di data inizio pubblicazione!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataFinePubblicazione11.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uceCodTipoNews11.Value == null || this.uceCodTipoNews11.Value.ToString() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodTipoNews11.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uceCodTipoNews11.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_UnitaOperative:
                    if (bRet && this.uteCodice12.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice12.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice12.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione12.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione12.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione12.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodAzi12.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodAzi12.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodAzi12.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_ViaSomministrazione:
                    if (bRet && this.uteCodice13.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice13.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice13.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione13.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione13.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione13.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                    if (bRet && this.uteCodice14.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice14.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice14.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione14.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione14.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione14.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_StatoAppuntamento:
                case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                case Enums.EnumDataNames.T_StatoAllegato:
                    if (bRet && this.uteCodice15.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice15.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice15.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione15.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione15.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione15.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.umeOrdine15.Text.Trim() == "" || this.umeOrdine15.Text.Trim() == "0"))
                    {
                        MessageBox.Show(@"Inserire " + this.lblOrdine15.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.umeOrdine15.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_OEFormule:
                    if (bRet && (this.uteCodUA16.Text.Trim() == "" || this.lblCodUADes16.Text.Trim() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodUA16.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodUA16.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodAzienda16.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodAzienda16.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodAzienda16.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodErogante16.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodErogante16.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodErogante16.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodPrestazione16.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodPrestazione16.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodPrestazione16.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodDatoAccessorio16.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodDatoAccessorio16.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodDatoAccessorio16.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_OEAttributi:
                    break;

                case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                case Enums.EnumDataNames.T_StatoEvidenzaClinicaVisione:
                case Enums.EnumDataNames.T_StatoAlertGenerico:
                case Enums.EnumDataNames.T_StatoAlertAllergiaAnamnesi:
                case Enums.EnumDataNames.T_StatoParametroVitale:
                case Enums.EnumDataNames.T_StatoConsegna:
                case Enums.EnumDataNames.T_StatoConsegnaPaziente:
                case Enums.EnumDataNames.T_StatoConsegnaPazienteRuoli:
                case Enums.EnumDataNames.T_StatoScheda:
                case Enums.EnumDataNames.T_StatoPrescrizione:
                case Enums.EnumDataNames.T_StatoDiario:
                case Enums.EnumDataNames.T_StatoTrasferimento:
                case Enums.EnumDataNames.T_TipoAllegato:
                case Enums.EnumDataNames.T_FormatoAllegati:
                case Enums.EnumDataNames.T_StatoOrdine:
                case Enums.EnumDataNames.T_StatoPrescrizioneTempi:
                case Enums.EnumDataNames.T_StatoContinuazione:
                case Enums.EnumDataNames.T_StatoCartella:
                case Enums.EnumDataNames.T_StatoCartellaInfo:
                case Enums.EnumDataNames.T_StatoCartellaInVisione:
                case Enums.EnumDataNames.T_StatoEpisodio:
                    if (bRet && this.uteCodice17.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice17.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice17.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione17.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione17.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione17.Focus();
                        bRet = false;
                    }
                    if (bRet && this.ViewDataNamePU == Enums.EnumDataNames.T_StatoTrasferimento && (this.umeOrdine17.Text.Trim() == "" || this.umeOrdine17.Text.Trim() == "0"))
                    {
                        MessageBox.Show(@"Inserire " + this.lblOrdine17.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.umeOrdine17.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                    if (bRet && this.uteCodScheda29.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodScheda29.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodScheda29.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodice29.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice29.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice29.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione29.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione29.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione29.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Sistemi:
                    if (bRet && this.uteCodice27.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice27.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice27.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione27.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione27.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione27.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Stanze:
                    if (bRet && this.uteCodAzi18.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodAzi18.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodAzi18.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodice18.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice18.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice18.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione18.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione18.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione18.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_FormatoReport:
                case Enums.EnumDataNames.T_TipoEpisodio:
                case Enums.EnumDataNames.T_TipoDiario:
                case Enums.EnumDataNames.T_TipoScheda:
                case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                    if (bRet && this.uteCodice19.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice19.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice19.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione19.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione19.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione19.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_TipoPrescrizione:
                    if (bRet && this.uteCodice20.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice20.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice20.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione20.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione20.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione20.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uteCodScheda20.Text.Trim() == "" || this.lblCodSchedaDes20.Text.Trim() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodScheda20.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodScheda20.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uteCodSchedaPosologia20.Text.Trim() != "" && this.lblCodSchedaPosologiaDes20.Text.Trim() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodSchedaPosologia20.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodSchedaPosologia20.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Letti:
                    if (bRet && this.uteCodAzi25.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodAzi25.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodAzi25.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodAzi25.Text.Trim() != "" && this.lblCodAziDes25.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodAzi25.Text + @" esistente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodAzi25.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodLetto25.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodLetto25.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodLetto25.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodSettore25.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodSettore25.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodSettore25.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodSettore25.Text.Trim() != "" && this.lblCodSettoreDes25.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodSettore25.Text + @" esistente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodSettore25.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione25.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione25.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione25.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodStanza25.Text.Trim() != "" && this.lblCodStanzaDes25.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodStanza25.Text + @" esistente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodStanza25.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Settori:
                    if (bRet && this.uteCodAzi26.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodAzi26.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodAzi26.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodAzi26.Text.Trim() != "" && this.lblCodAziDes26.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodAzi26.Text + @" corretta!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodAzi26.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodice26.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice26.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice26.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione26.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione26.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione26.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Contatori:
                    if (bRet && this.uteCodice28.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice28.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice28.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione28.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione28.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione28.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_TestiNotePredefiniti:
                    if (bRet && this.uteCodice22.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice22.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice22.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione22.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione22.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione22.Focus();
                        bRet = false;
                    }
                    break;

                default:
                    break;

            }

            return bRet;

        }

        private void LoadTestiPredefiniti()
        {

            string sSql = string.Empty;
            string sCodEntita = string.Empty;
            string sCodTestoPredefinito = string.Empty;

            try
            {

                sCodEntita = this.uteCodEntita10.Text;
                sCodTestoPredefinito = this.uteCodice10.Text;

                if (sCodEntita != "ALL" && sCodEntita != "NWS" && sCodEntita != string.Empty)
                {

                    using (FwDataConnection fdc = new FwDataConnection(MyStatics.Configurazione.ConnectionString))
                    {

                        FwDataParametersList fplist = new FwDataParametersList()
                        {
                            new FwDataParameter ("@CodTestoPredefinito", sCodTestoPredefinito ,ParameterDirection.Input, DbType.String,20),
                        };

                        DataTable dtall = fdc.Query<DataTable>(@"MSP_BO_SelSchedeTestiPredefiniti", fplist, CommandType.StoredProcedure);
                        _TestiPredefiniti_Campi.Add(MyStatics.TV_ROOT, dtall.Copy());

                    }

                    sSql = $"SELECT TP.Codice, TP.Descrizione, TPC.CodCampo As ID\n" +
$"FROM\n";

                    switch (sCodEntita)
                    {
                        case "ALA":
                            sSql += $"T_TipoAlertAllergiaAnamnesi TP\n";
                            break;
                        case "ALG":
                            sSql += $"T_TipoAlertGenerico TP\n";
                            break;
                        case "DCL":
                            sSql += $"T_TipoVoceDiario TP\n";
                            break;
                        case "PVT":
                            sSql += $"T_TipoParametroVitale TP\n";
                            break;
                        case "SCH":
                            sSql += $"T_Schede TP\n";
                            break;
                        case "WKI":
                            sSql += $"T_TipoTaskInfermieristico TP\n";
                            break;
                        default:
                            break;
                    }

                    sSql += $"INNER JOIN T_AssTestiPredefinitiCampi TPC On TP.Codice = TPC.CodTipoEntita\n" +
                            $"WHERE TPC.CodTestoPredefinito = '{sCodTestoPredefinito}' And TPC.CodEntita = '{sCodEntita}'\n" +
                            $"ORDER BY TP.Descrizione, TPC.CodCampo";
                    DataTable dtsel = DataBase.GetDataTable(sSql);

                    _TestiPredefiniti_Campi.Add(MyStatics.TV_VOCE, dtsel.Copy());
                    _TestiPredefiniti_Campi.Add(MyStatics.GC_NUOVO, dtsel.Clone());
                    _TestiPredefiniti_Campi.Add(MyStatics.GC_ELIMINA, dtsel.Clone());

                }


            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadTestiPredefiniti", this.Text);
            }

        }

        private void CaricaTestiPredefiniti(bool onlysel)
        {

            UltraTreeNode oNoderoot = null;
            UltraTreeNode oNodescheda = null;
            UltraTreeNode oNode = null;

            DataTable _dttv = null;
            string _codice = string.Empty;

            try
            {
                if (_TestiPredefiniti_Campi.Count > 0)
                {
                    if (onlysel == false)
                    {
                        _dttv = _TestiPredefiniti_Campi[MyStatics.TV_ROOT];
                    }
                    else
                    {
                        _dttv = _TestiPredefiniti_Campi[MyStatics.TV_VOCE];
                        _dttv.DefaultView.Sort = "Descrizione, ID";
                        _dttv = _dttv.DefaultView.ToTable();

                    }
                    this.utvTPSX10.Nodes.Clear();
                    MyStatics.SetUltraTree(this.utvTPSX10, new Size(16, 16), false);
                    oNoderoot = new UltraTreeNode(MyStatics.TV_ROOT, MyStatics.TV_ROOT);
                    oNoderoot.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                    oNoderoot.Tag = MyStatics.TV_ROOT;
                    this.utvTPSX10.Nodes.Add(oNoderoot);
                    foreach (DataRow dr in _dttv.Rows)
                    {

                        bool bFiltro = true;
                        if (this.uteCampiFiltro10.Text != string.Empty)
                        {
                            bFiltro = (dr["Descrizione"].ToString().ToUpper().Contains(this.uteCampiFiltro10.Text.ToUpper()) || dr["Codice"].ToString().ToUpper().Contains(this.uteCampiFiltro10.Text.ToUpper()));
                        }
                        if (_codice != dr["Codice"].ToString())
                        {
                            _codice = dr["Codice"].ToString();
                            oNodescheda = new UltraTreeNode(dr["Codice"].ToString(), $"{dr["Descrizione"].ToString()} ({_codice})");
                            oNodescheda.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SCHEDE, Enums.EnumImageSize.isz16)));
                            oNodescheda.Tag = MyStatics.TV_ENTITA;
                            if (bFiltro) { oNoderoot.Nodes.Add(oNodescheda); }
                        }

                        oNode = new UltraTreeNode(oNodescheda.Key + "|" + dr["ID"].ToString(), $"{dr["DescrizioneCampo"].ToString()} ({dr["ID"].ToString()})");
                        oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_VOCE, Enums.EnumImageSize.isz16)));
                        oNode.Tag = dr["ID"].ToString();
                        oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                        if (onlysel == false)
                        {
                            IEnumerable<DataRow> results = from myRow in _TestiPredefiniti_Campi[MyStatics.TV_VOCE].AsEnumerable()
                                                           where myRow.Field<string>("Codice").ToString() == dr["Codice"].ToString() &&
                                                                    myRow.Field<string>("ID").ToString() == dr["ID"].ToString()
                                                           select myRow;
                            if (results != null && results.ToArray().Length == 1)
                            {
                                oNode.CheckedState = CheckState.Checked;
                            }
                        }
                        else
                        {
                            oNode.CheckedState = CheckState.Checked;
                        }
                        if (bFiltro) { oNodescheda.Nodes.Add(oNode); }

                    }
                    this.utvTPSX10.PerformAction(UltraTreeAction.FirstNode, false, false);
                    this.utvTPSX10.PerformAction(UltraTreeAction.ExpandNode, false, false);
                }


            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "CaricaTestiPredefiniti", this.Text);
            }

        }

        private void LoadUltraTreeIntestazioni(UltraTreeNode oNodePadre)
        {

            string sSql = @"";

            UltraTreeNode oNode = null;

            try
            {

                if (oNodePadre == null)
                {

                    this.utvIntestazioni.Nodes.Clear();
                    oNode = new UltraTreeNode(MyStatics.TV_ROOT, MyStatics.TV_ROOT);
                    oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                    oNode.Tag = MyStatics.TV_ROOT;
                    this.utvIntestazioni.Nodes.Add(oNode);
                    this.utvIntestazioni.PerformAction(UltraTreeAction.FirstNode, false, false);
                    this.utvIntestazioni.PerformAction(UltraTreeAction.ExpandNode, false, false);

                }
                else
                {

                    oNodePadre.Nodes.Clear();
                    if (oNodePadre.Tag.ToString() == CoreStatics.TV_ROOT)
                    {

                        foreach (string name in Enum.GetNames(typeof(EnumTipoIntestazione)))
                        {

                            oNode = new UltraTreeNode(this.uteCodice4.Text + "|" + name, Enums.GetEnumDescription((EnumTipoIntestazione)Enum.Parse(typeof(EnumTipoIntestazione), name)));
                            oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                            oNode.Tag = MyStatics.TV_PATH;
                            oNodePadre.Nodes.Add(oNode);
                            oNode.Expanded = true;

                        }

                    }
                    else
                    {

                        string[] arvkey = oNodePadre.Key.Split('|');

                        sSql = @"Select * From T_AssUAIntestazioni 
                                            Where CodUA = '" + arvkey[0] + @"'
                                                    And CodIntestazione = '" + arvkey[1] + @"'
                                            Order By DataInizio, DataFine";
                        DataSet oDs = DataBase.GetDataSet(sSql);
                        foreach (DataRow oDataRow in oDs.Tables[0].Rows)
                        {

                            oNode = new UltraTreeNode(oDataRow["CodUA"].ToString() + "|" + oDataRow["CodIntestazione"].ToString() + "|" + ((DateTime)oDataRow["DataInizio"]).Ticks.ToString(),
                                                        string.Format("Dal {0} al {1}", oDataRow["DataInizio"], (oDataRow.IsNull("DataFine") ? "---" : oDataRow["DataFine"])));
                            oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_REPORTTREEVIEW, Enums.EnumImageSize.isz16)));
                            oNode.Tag = MyStatics.TV_INTESTAZIONE;
                            oNodePadre.Nodes.Add(oNode);

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadUltraTreeIntestazioni", this.Text);
            }

        }

        private void SetButtonutvIntestazioni(UltraTreeNode oNode)
        {

            bool bNuovo = false;
            bool bModifica = false;
            bool bCancella = false;

            try
            {

                switch (_Modality)
                {

                    case Enums.EnumModalityPopUp.mpNuovo:
                        break;

                    case Enums.EnumModalityPopUp.mpModifica:
                        if (oNode.Tag.ToString() == MyStatics.TV_PATH)
                        {
                            bNuovo = true;
                        }
                        else if (oNode.Tag.ToString() == MyStatics.TV_INTESTAZIONE)
                        {
                            bModifica = true;
                            bCancella = true;
                        }
                        break;

                    case Enums.EnumModalityPopUp.mpCancella:
                        break;

                    case Enums.EnumModalityPopUp.mpVisualizza:
                        break;

                    case Enums.EnumModalityPopUp.mpCopia:
                        break;

                    default:
                        break;

                }

            }
            catch (Exception)
            {

            }
            finally
            {
                this.ubNuovoI4.Enabled = bNuovo;
                this.ubModificaI4.Enabled = bModifica;
                this.ubCancellaI4.Enabled = bCancella;
            }

        }

        private void LoadIntestazioneRTF(UltraTreeNode oNode)
        {

            try
            {

                switch (oNode.Tag.ToString())
                {

                    case MyStatics.TV_ROOT:
                    case MyStatics.TV_PATH:
                        this.ucRichTextBoxIntestazioni.ViewRtf = "";
                        break;

                    default:
                        string[] arrKeys = this.utvIntestazioni.ActiveNode.Key.Split('|');
                        DateTime dtI = new DateTime(long.Parse(arrKeys[2]));
                        this.ucRichTextBoxIntestazioni.ViewRtf = DataBase.FindValue("Intestazione",
                                                                                    "T_AssUAIntestazioni",
                                                                                    "CodUA = '" + arrKeys[0] + @"'
                                                                                    And CodIntestazione = '" + arrKeys[1] + @"'
                                                                                    And DataInizio = " + DataBase.SQLDateTime(dtI),
                                                                                    "");
                        break;

                }

            }
            catch (Exception)
            {

            }

        }

        private Enums.EnumEntitaLog getEntitaFromDataNames()
        {

            Enums.EnumEntitaLog enRet = Enums.EnumEntitaLog.Nessuna;

            switch (this.ViewDataNamePU)
            {

                case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                    enRet = Enums.EnumEntitaLog.T_TipoEvidenzaClinica;
                    break;

                default:
                    break;

            }

            return enRet;

        }

        private int CheckVersioneCorrenteScheda(string codiceScheda)
        {
            return int.Parse(DataBase.FindValue("IsNull(Versione,0)", "T_SchedeVersioni", "CodScheda = '" + codiceScheda + "' And FlagAttiva = 1 Order By DtValI desc, DtValF", "0"));
        }

        #region Ruoli Azioni

        private void LoadEntitaAzioni()
        {
            try
            {

                MyStatics.SetUltraTree(this.utvEntitaAzioni2, new Size(16, 16), false);
                this.utvEntitaAzioni2.Override.Sort = Infragistics.Win.UltraWinTree.SortType.Ascending;

                this.utvEntitaAzioni2.Nodes.Clear();

                string sSql = "";
                sSql = @"   Select AE.CodEntita, IsNull(E.Descrizione, '') As Entita
	                            , AE.CodAzione, IsNull(A.Descrizione, '') As Azione
	                            , E.Tabella
                            From T_AzioniEntita AE WITH (NOLOCK)
	                            Inner Join T_Entita E WITH (NOLOCK) On AE.CodEntita = E.Codice
	                            Inner Join T_Azioni A WITH (NOLOCK) On AE.CodAzione = A.Codice
                            Where IsNull(E.AbilitaPermessiDettaglio, 0) <> 0
	                            And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0
                            Order By AE.CodEntita, AE.CodAzione";
                DataSet ds = DataBase.GetDataSet(sSql);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        string sCodEntita = "";
                        string sCodAzione = "";
                        Infragistics.Win.UltraWinTree.UltraTreeNode nodeEntita = null;
                        foreach (DataRow drEA in ds.Tables[0].Rows)
                        {
                            try
                            {
                                if (sCodEntita != drEA["CodEntita"].ToString())
                                {
                                    if (nodeEntita != null) nodeEntita.Expanded = true;
                                    sCodAzione = "";
                                    sCodEntita = drEA["CodEntita"].ToString();
                                    nodeEntita = this.utvEntitaAzioni2.Nodes.Add(MyStatics.TV_ENTITA + @"|" + sCodEntita, drEA["Entita"].ToString());
                                    nodeEntita.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GetKeyNameFromEntitaCode(sCodEntita), Enums.EnumImageSize.isz16)));
                                    nodeEntita.Tag = MyStatics.TV_ENTITA;
                                }

                                if (sCodAzione != drEA["CodAzione"].ToString())
                                {
                                    sCodAzione = drEA["CodAzione"].ToString();
                                    Infragistics.Win.UltraWinTree.UltraTreeNode nodeAzione = nodeEntita.Nodes.Add(nodeEntita.Key + @"|" + MyStatics.TV_AZIONE + @"|" + sCodAzione, drEA["Azione"].ToString());
                                    nodeAzione.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_AZIONE, Enums.EnumImageSize.isz16)));
                                    nodeAzione.Tag = MyStatics.TV_AZIONE;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        if (nodeEntita != null) nodeEntita.Expanded = true;
                    }

                    ds.Dispose();
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadEntitaAzioni", this.Text);
            }
        }

        private void LoadRuoloAzioni()
        {
            _runTime = true;
            try
            {

                MyStatics.SetUltraTree(this.utvRuoloAzioni2, new Size(16, 16), true);

                this.utvRuoloAzioni2.Nodes.Clear();

                if (this.utvEntitaAzioni2.ActiveNode != null && this.utvEntitaAzioni2.ActiveNode.Tag.ToString() == MyStatics.TV_AZIONE)
                {
                    string[] arrKeys = this.utvEntitaAzioni2.ActiveNode.Key.Split('|');
                    string sCodEntita = arrKeys[1];
                    string sCodAzione = arrKeys[3];
                    DataSet dsVoci = null;
                    Image imgNode = null;
                    if (this.utvEntitaAzioni2.ActiveNode != null && this.utvEntitaAzioni2.ActiveNode.Parent != null && this.utvEntitaAzioni2.ActiveNode.LeftImages.Count > 0)
                        imgNode = (Image)this.utvEntitaAzioni2.ActiveNode.Parent.LeftImages[0];

                    if (_Modality == Enums.EnumModalityPopUp.mpModifica || _Modality == Enums.EnumModalityPopUp.mpNuovo)
                    {
                        if (_RuoloEntitaAzione_Voci.ContainsKey(sCodEntita + @"|" + sCodAzione))
                            dsVoci = _RuoloEntitaAzione_Voci[sCodEntita + @"|" + sCodAzione];
                    }

                    if (dsVoci == null)
                    {
                        string sSqlDecodifica = "";
                        switch ((UnicodeSrl.Scci.Enums.EnumEntita)Enum.Parse(typeof(UnicodeSrl.Scci.Enums.EnumEntita), sCodEntita))
                        {
                            case UnicodeSrl.Scci.Enums.EnumEntita.DCL:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione + ' (' + IsNull(TD.Descrizione, '') + ')'
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_TipoVoceDiario TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
	                                                Left Join T_TipoDiario TD WITH (NOLOCK) On TV.CodTipoDiario = TD.Codice
                                                Order By 2 --3 DESC, 2";
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.PVT:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_TipoParametroVitale TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Order By 2 --3 DESC, 2";
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.CSG:
                                switch (sCodAzione.ToUpper())
                                {
                                    case "INS":
                                    default:
                                        sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_TipoConsegna TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Order By 2 --3 DESC, 2";
                                        break;
                                }
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.CSP:
                                switch (sCodAzione.ToUpper())
                                {
                                    case "CND":
                                        sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_TipoConsegnaPaziente TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Order By 2 --3 DESC, 2";
                                        break;
                                    case "INS":
                                    default:
                                        sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_TipoConsegnaPaziente TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Order By 2 --3 DESC, 2";
                                        break;
                                }
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.PRF:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_TipoPrescrizione TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Order By 2 --3 DESC, 2";
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.RPT:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione + ' (' + IsNull(FR.Descrizione, '') + ')'
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_Report TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
	                                                Left Join T_FormatoReport FR WITH (NOLOCK) On TV.CodFormatoReport = FR.Codice
                                                Order By 2 --3 DESC, 2";
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.WKI:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_TipoTaskInfermieristico TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Order By 2 --3 DESC, 2";
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.ALA:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_TipoAlertAllergiaAnamnesi TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Order By 2 --3 DESC, 2";
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.ALG:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_TipoAlertGenerico TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Order By 2 --3 DESC, 2";
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.SCR:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_Screen TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Order By 2 --3 DESC, 2";
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.AGE:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_Agende TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Order By 2 --3 DESC, 2";
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.FLS:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_FiltriSpeciali TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Where TV.CodTipoFiltroSpeciale IN ('" + Scci.Enums.EnumTipoFiltroSpeciale.AMBCAR.ToString() + @"', '"
                                                      + Scci.Enums.EnumTipoFiltroSpeciale.EPICAR.ToString() + @"', '"
                                                      + Scci.Enums.EnumTipoFiltroSpeciale.OETRA.ToString() + @",')
                                                Order By 2 --3 DESC, 2";
                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.SCH:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                From T_Schede TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                WHERE   
													ISNULL(SchedaSemplice, 0) = 0" + Environment.NewLine;

                                if (sCodAzione == UnicodeSrl.Scci.Enums.EnumAzioni.VSR.ToString())
                                {
                                    sSqlDecodifica += "AND ISNULL(Riservata, 0) = 1" + Environment.NewLine;
                                }
                                if (sCodAzione == UnicodeSrl.Scci.Enums.EnumAzioni.REV.ToString())
                                {
                                    sSqlDecodifica += "AND ISNULL(Validabile, 0) = 1 AND ISNULL(Revisione, 0) = 1" + Environment.NewLine;
                                }
                                sSqlDecodifica += "Order By 2 --3 DESC, 2";

                                break;
                            case UnicodeSrl.Scci.Enums.EnumEntita.CIV:
                                sSqlDecodifica = @" Select COALESCE(TV.Codice, TA.CodVoce) As Codice
	                                                , Case
		                                                When TV.Descrizione Is Null Then '{' + IsNull(TA.CodVoce, '') + '} <<decodifica assente>>'
		                                                Else TV.Descrizione
	                                                End As Descrizione
                                                    , Convert(bit, Case
                                                        When TA.CodVoce Is Not Null Then 1
                                                        Else 0
                                                    End) As Selected
                                                    , TA.Parametri
                                                From T_Ruoli TV WITH (NOLOCK)
	                                                Full Outer Join (
		                                                Select CodVoce, Parametri
		                                                From T_AssRuoliAzioni WITH (NOLOCK)
		                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"') TA On TV.Codice = TA.CodVoce
                                                Order By 2 --3 DESC, 2";
                                break;
                            default:
                                sSqlDecodifica = @" Select CodVoce As Codice, '{' + IsNull(CodVoce, '') + '} <<decodifica assente>>' As Descrizione, Convert(bit, 1) As Selected
                                                From T_AssRuoliAzioni WITH (NOLOCK)
                                                Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                        And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                        And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"'
                                                Order By 2 --3 DESC, 2";
                                break;
                        }

                        dsVoci = DataBase.GetDataSet(sSqlDecodifica);

                        if (_Modality == Enums.EnumModalityPopUp.mpModifica || _Modality == Enums.EnumModalityPopUp.mpNuovo)
                        {
                            if (_RuoloEntitaAzione_Voci.ContainsKey(sCodEntita + @"|" + sCodAzione))
                                _RuoloEntitaAzione_Voci[sCodEntita + @"|" + sCodAzione] = dsVoci.Copy();
                            else
                                _RuoloEntitaAzione_Voci.Add(sCodEntita + @"|" + sCodAzione, dsVoci.Copy());

                            _RuoloEntitaAzione_Voci[sCodEntita + @"|" + sCodAzione].Tables[0].Columns["Selected"].ReadOnly = false;
                        }

                    }

                    if (dsVoci != null)
                    {
                        if (dsVoci.Tables.Count > 0 && dsVoci.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow drValore in dsVoci.Tables[0].Rows)
                            {
                                bool bAdd = true;
                                if (_Modality == Enums.EnumModalityPopUp.mpCancella || _Modality == Enums.EnumModalityPopUp.mpVisualizza)
                                {
                                    bAdd = (bool)drValore["Selected"];
                                }
                                if (bAdd)
                                {
                                    bool bFiltro = true;
                                    if (this.uteAzioniFiltro2.Text != string.Empty)
                                    {
                                        bFiltro = (drValore["Descrizione"].ToString().ToUpper().Contains(this.uteAzioniFiltro2.Text.ToUpper()) || drValore["Codice"].ToString().ToUpper().Contains(this.uteAzioniFiltro2.Text.ToUpper()));
                                    }
                                    if (bFiltro)
                                    {

                                        Infragistics.Win.UltraWinTree.UltraTreeNode nodeVoce = null;
                                        if (dsVoci.Tables[0].Columns.Contains("Parametri") && !drValore.IsNull("Parametri"))
                                        {
                                            ParametriCIV oPCIV = JsonConvert.DeserializeObject<ParametriCIV>(drValore["Parametri"].ToString());
                                            nodeVoce = this.utvRuoloAzioni2.Nodes.Add(MyStatics.TV_VOCE + @"|" + drValore["Codice"].ToString(),
                                                                                        drValore["Descrizione"].ToString() + @"  [" + drValore["Codice"].ToString() + @"] (" + oPCIV.GGCartelleInVisione + ")");
                                        }
                                        else
                                        {
                                            nodeVoce = this.utvRuoloAzioni2.Nodes.Add(MyStatics.TV_VOCE + @"|" + drValore["Codice"].ToString(),
                                                                                        drValore["Descrizione"].ToString() + @"  [" + drValore["Codice"].ToString() + @"]");
                                        }
                                        nodeVoce.Tag = drValore["Codice"].ToString();
                                        if (imgNode != null)
                                            nodeVoce.LeftImages.Add(imgNode);
                                        else
                                            nodeVoce.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_VOCE, Enums.EnumImageSize.isz16)));

                                        if (_Modality == Enums.EnumModalityPopUp.mpCancella || _Modality == Enums.EnumModalityPopUp.mpVisualizza)
                                            nodeVoce.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.Standard;
                                        else
                                        {
                                            nodeVoce.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                                            if ((bool)drValore["Selected"]) nodeVoce.CheckedState = CheckState.Checked;
                                        }
                                    }
                                }

                            }
                        }
                        dsVoci.Dispose();
                    }

                    MyStatics.FiltraTVCheck(ref this.uceAzioniFiltro2, ref this.utvRuoloAzioni2);

                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadRuoloAzioni", this.Text);
            }
            _runTime = false;
        }

        private void UpdateRuoliAzioni()
        {
            try
            {
                if (_Modality == Enums.EnumModalityPopUp.mpModifica || _Modality == Enums.EnumModalityPopUp.mpNuovo)
                {
                    foreach (string sKey in _RuoloEntitaAzione_Voci.Keys)
                    {
                        if (_RuoloEntitaAzione_Voci[sKey].GetChanges(DataRowState.Modified) != null)
                        {
                            string[] arrCodes = sKey.Split('|');
                            string sCodEntita = arrCodes[0];
                            string sCodAzione = arrCodes[1];

                            string sSql = @"";
                            sSql = @"   Delete From T_AssRuoliAzioni
                                        Where CodRuolo = '" + DataBase.Ax2(this.uteCodice2.Text) + @"'
			                                And CodEntita = '" + DataBase.Ax2(sCodEntita) + @"'
			                                And CodAzione = '" + DataBase.Ax2(sCodAzione) + @"'" + Environment.NewLine + Environment.NewLine;

                            DataSet dsVoci = _RuoloEntitaAzione_Voci[sKey];
                            foreach (DataRow drVoce in dsVoci.Tables[0].Rows)
                            {
                                if ((bool)drVoce["Selected"])
                                {
                                    if (dsVoci.Tables[0].Columns.Contains("Parametri"))
                                    {
                                        sSql += @"  INSERT INTO [T_AssRuoliAzioni] ([CodRuolo],[CodEntita],[CodVoce],[CodAzione],[Parametri])
                                                    VALUES('" + DataBase.Ax2(this.uteCodice2.Text) + @"','" +
                                                                DataBase.Ax2(sCodEntita) + @"','" +
                                                                DataBase.Ax2(drVoce["Codice"].ToString()) + @"','" +
                                                                DataBase.Ax2(sCodAzione) + @"'," +
                                                                (drVoce.IsNull("Parametri") ? "null" : "'" + drVoce["Parametri"].ToString() + "'") + ")" +
                                                    Environment.NewLine + Environment.NewLine;
                                    }
                                    else
                                    {
                                        sSql += @"  INSERT INTO [T_AssRuoliAzioni] ([CodRuolo],[CodEntita],[CodVoce],[CodAzione])
                                                    VALUES('" + DataBase.Ax2(this.uteCodice2.Text) + @"','" + DataBase.Ax2(sCodEntita) + @"','" + DataBase.Ax2(drVoce["Codice"].ToString()) + @"','" + DataBase.Ax2(sCodAzione) + @"')" + Environment.NewLine + Environment.NewLine;
                                    }
                                }
                            }

                            if (sSql != "") DataBase.ExecuteSql(sSql);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "UpdateRuoliAzioni", this.Text);
            }
        }

        #endregion

        private void DisposeUserControls(Control Container)
        {

            foreach (Control c in Container.Controls)
            {

                if (c.HasChildren) DisposeUserControls(c);

                if (c.GetType().ToString() == "UnicodeSrl.ScciCore.ucMultiSelect") c.Dispose();

            }

        }

        private void setCheckBrowser()
        {
            try
            {
                if (chkApriBrowser8.Checked)
                {
                    this.chkApriIE8.Enabled = (this.ViewModality != Enums.EnumModalityPopUp.mpVisualizza && this.ViewModality != Enums.EnumModalityPopUp.mpCancella);
                }
                else
                {
                    this.chkApriIE8.Enabled = false;
                    this.chkApriIE8.Checked = false;
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Events

        private void frmPUView_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeUserControls(this.UltraGroupBox);
        }

        private void ute_ValueChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void uce_ValueChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;

            try
            {
                if (sender == this.uceCodFormatoReport8)
                {
                    InitAndBindWebReportConfig();
                    DataSet dscombo = (DataSet)this.uceCodFormatoReport8.DataSource;
                    if (this.uceCodFormatoReport8.Value != null && dscombo != null && dscombo.Tables[0].Rows.Count > 0)
                    {
                        DataRow findrow = dscombo.Tables[0].Rows.Find(this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper());
                        if (findrow != null)
                        {
                            this.utePercorsoFile8.Visible = System.Convert.ToBoolean(findrow["DaModello"]);
                            this.lblPercorsoFile8.Visible = System.Convert.ToBoolean(findrow["DaModello"]);
                            if (!this.utePercorsoFile8.Visible) this.utePercorsoFile8.Text = string.Empty;

                            if (System.Convert.ToBoolean(findrow["DaModello"]))
                            {

                                this.ubOpenDOCX8.Left = this.utePercorsoFile8.Left;
                                this.ubOpenDOCX8.Top = this.utePercorsoFile8.Top + this.utePercorsoFile8.Height + 4;

                                this.ubOpenDOCX8.Visible = true;
                            }
                            else
                                this.ubOpenDOCX8.Visible = false;

                            this.chkDaStoricizzare8.Visible = System.Convert.ToBoolean(findrow["Storicizzabile"]);
                            if (!this.chkDaStoricizzare8.Visible) this.chkDaStoricizzare8.Checked = false;
                        }


                        this.uteNomePlugin8.Visible = (this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_CABLATO.ToUpper());
                        this.lblNomePlugin8.Visible = (this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_CABLATO.ToUpper());
                        this.chkFlagRichiediStampante8.Visible = (this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_CABLATO.ToUpper());

                        this.xmlParametri8.Visible = (this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_CABLATO.ToUpper());
                        this.lblParametri8.Visible = (this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_CABLATO.ToUpper());
                        if (this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_CABLATO.ToUpper())
                        {
                            this.xmlParametri8.Top = this.chkFlagRichiediStampante8.Top + this.chkFlagRichiediStampante8.Height + 4;
                            this.lblParametri8.Top = this.xmlParametri8.Top;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uce_ValueChanged", this.Text);
            }

        }

        private void chk_CheckedChanged(object sender, EventArgs e)
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
                if (e.Layout.Bands[0].Columns.Exists("Obsoleto") == true) { e.Layout.Bands[0].Columns["Obsoleto"].Hidden = true; }

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
                if (e.Layout.Bands[0].Columns.Exists("Obsoleto") == true) { e.Layout.Bands[0].Columns["Obsoleto"].Hidden = true; }

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

        private void ucRichTextBox_RtfChange(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void uteCodPadre4_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodPadre4.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_UnitaAtomiche";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodice4.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodPadre4.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodPadre4_EditorButtonClick", this.Text);
            }

        }
        private void uteCodPadre4_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodPadreDes4.Text = DataBase.FindValue("Descrizione", "T_UnitaAtomiche", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodPadre4.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodUANumerazioneCartella4_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodUANumerazioneCartellaDes4.Text = DataBase.FindValue("Descrizione", "T_UnitaAtomiche", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodUANumerazioneCartella4.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodUANumerazioneCartella4_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodPadre4.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_UnitaAtomiche";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodice4.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodUANumerazioneCartella4.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodUANumerazioneCartella4_EditorButtonClick", this.Text);
            }
        }

        private void uteCodAzienda4_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodAzienda4.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Aziende";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodAzienda4.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodAzienda4_EditorButtonClick", this.Text);
            }

        }
        private void uteCodAzienda4_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodAziendaDes4.Text = DataBase.FindValue("Descrizione", "T_Aziende", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodAzienda4.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void utvIntestazioni_AfterActivate(object sender, NodeEventArgs e)
        {
            this.LoadIntestazioneRTF(e.TreeNode);
            this.SetButtonutvIntestazioni(e.TreeNode);
        }

        private void utvIntestazioni_BeforeExpand(object sender, CancelableNodeEventArgs e)
        {
            this.LoadUltraTreeIntestazioni(e.TreeNode);
        }

        private void ubNuovoI4_Click(object sender, EventArgs e)
        {

            try
            {

                string[] arvcodici = this.utvIntestazioni.ActiveNode.Key.Split('|');
                string skey = this.utvIntestazioni.ActiveNode.Key;
                if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_AssUAIntestazioni, Enums.EnumModalityPopUp.mpNuovo, this.ViewIcon, this.ViewImage, "Intestazioni", arvcodici) == DialogResult.OK)
                {
                }
                this.LoadUltraTreeIntestazioni(this.utvIntestazioni.ActiveNode);
                this.utvIntestazioni.ActiveNode = this.utvIntestazioni.GetNodeByKey(skey);
                this.utvIntestazioni.PerformAction(UltraTreeAction.SelectActiveNode, false, false);
                this.utvIntestazioni.Focus();

            }
            catch (Exception)
            {

            }

        }

        private void ubModificaI4_Click(object sender, EventArgs e)
        {

            try
            {

                string[] arvkey = this.utvIntestazioni.ActiveNode.Key.Split('|');
                string[] arvcodici = this.utvIntestazioni.ActiveNode.Key.Split('|');
                arvcodici[0] = arvkey[1];
                arvcodici[1] = arvkey[0];
                arvcodici[2] = arvkey[2];
                string skey = "";
                if (MyStatics.ActionDataNameFormPU(Enums.EnumDataNames.T_AssUAIntestazioni, Enums.EnumModalityPopUp.mpModifica, this.ViewIcon, this.ViewImage, "Intestazioni", arvcodici) == DialogResult.OK)
                {
                    skey = this.utvIntestazioni.ActiveNode.Parent.Key;
                }
                else
                {
                    skey = this.utvIntestazioni.ActiveNode.Key;
                }
                this.LoadUltraTreeIntestazioni(this.utvIntestazioni.ActiveNode.Parent);
                this.utvIntestazioni.ActiveNode = this.utvIntestazioni.GetNodeByKey(skey);
                this.utvIntestazioni.PerformAction(UltraTreeAction.SelectActiveNode, false, false);
                this.utvIntestazioni.Focus();

            }
            catch (Exception)
            {

            }

        }

        private void ubCancellaI4_Click(object sender, EventArgs e)
        {

            try
            {

                if (MessageBox.Show("Confermi la cancellazione intestazione ?", "Intestazioni", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                {

                    string[] arrKeys = this.utvIntestazioni.ActiveNode.Key.Split('|');
                    DateTime dtI = new DateTime(long.Parse(arrKeys[2]));
                    string sSql = @"Delete From T_AssUAIntestazioni 
                                    Where CodUA = '" + arrKeys[0] + @"'
                                            And CodIntestazione = '" + arrKeys[1] + @"'
                                            And DataInizio = " + DataBase.SQLDateTime(dtI);
                    DataBase.ExecuteSql(sSql);
                    this.LoadUltraTreeIntestazioni(this.utvIntestazioni.ActiveNode.Parent);
                    this.utvIntestazioni.ActiveNode = this.utvIntestazioni.GetNodeByKey(CoreStatics.TV_ROOT);
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "ubCancellaI4_Click", this.Text);
            }

        }

        private void uteCodScheda5_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                switch (e.Button.Key)
                {

                    case "Zoom":
                        frmZoom f = new frmZoom();
                        f.ViewText = this.lblCodScheda5.Text;
                        f.ViewIcon = this.ViewIcon;
                        f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Schede";
                        f.ViewSqlStruct.Where = "CodEntita = '" + this.ucMultiSelectUA5.Tag.ToString() + "' And Codice <> '" + DataBase.Ax2(this.uteCodScheda5.Text) + "'";
                        f.ViewInit();
                        f.ShowDialog();
                        if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            this.uteCodScheda5.Text = f.ViewActiveRow.Cells["Codice"].Text;
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
                MyStatics.ExGest(ref ex, "uteCodScheda5_EditorButtonClick", this.Text);
            }

        }
        private void uteCodScheda5_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodSchedaDes5.Text = DataBase.FindValue("Descrizione", "T_Schede", string.Format("Codice = '{0}' AND CodEntita = '{1}'", new string[2] { DataBase.Ax2(this.uteCodScheda5.Text), this.ucMultiSelectUA5.Tag.ToString() }), "");
            this.ubApplica.Enabled = true;
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

        private void uteCodReportVista8_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodReportVista8.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_ReportViste";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodReportVista8.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodReportVista8.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodReportVista8_EditorButtonClick", this.Text);
            }
        }
        private void uteCodReportVista8_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodReportVistaDes8.Text = DataBase.FindValue("Descrizione", "T_ReportViste", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodReportVista8.Text)), "");
            this.ubApplica.Enabled = true;

            LoadReportExternalFields();
        }

        private void chkApriBrowser8_CheckedChanged(object sender, EventArgs e)
        {
            setCheckBrowser();
        }

        private void utePercorsoFile8_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.uceCodFormatoReport8.Value != null)
            {
                OpenFileDialog ofdg = new OpenFileDialog();

                switch (this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper())
                {
                    case MyStatics.FR_REM:
                        ofdg.Filter = "Documeto PDF (*.pdf) | *.pdf";
                        break;
                    case MyStatics.FR_WORD:
                        ofdg.Filter = "Documeto Microsoft Word 2007/2010 (*.docx)| *.docx";
                        break;
                }

                if (ofdg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.utePercorsoFile8.Text = ofdg.FileName;
                }
            }
        }

        private void uteNomePlugin8_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                OpenFileDialog ofdg = new OpenFileDialog();
                ofdg.Filter = "Library DLL (*.dll) | *.dll";

                if (ofdg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteNomePlugin8.Text = System.IO.Path.GetFileNameWithoutExtension(ofdg.FileName);
                }
            }
            catch (Exception)
            {
            }
        }

        private void ubVariabili8_Click(object sender, EventArgs e)
        {
            try
            {
                string xml = "";
                DataTable dtVariables = this.reportLinkConfig8.ExternalFields.Clone();
                dtVariables.TableName = "VARIABLES";
                if (!this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Variabili") && this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Variabili"].ToString() != "")
                {
                    xml = this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Variabili"].ToString();
                    dtVariables = CoreStatics.getVariables(xml);
                }

                frmPUVariabile frm = new frmPUVariabile();
                frm.ViewIcon = this.ViewIcon;
                frm.ViewModality = this.ViewModality;
                frm.ViewText = "Campi / Placeholder";
                frm.DataTableVariables = dtVariables;
                frm.ViewInit();
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK && (_Modality == Enums.EnumModalityPopUp.mpModifica || _Modality == Enums.EnumModalityPopUp.mpNuovo))
                {
                    dtVariables = frm.DataTableVariables;
                    xml = CoreStatics.getVariablesXML(dtVariables);
                    this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Variabili"] = xml;

                    LoadReportExternalFields();
                }
                frm.Dispose();
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "ubVariabili8_Click", this.Text);
            }
        }

        private void ubOpenDOCX8_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] modellodocx = null;
                if (this.utePercorsoFile8.Text != "" && System.IO.File.Exists(this.utePercorsoFile8.Text))
                    modellodocx = Scci.Statics.FileStatics.GetBytesFromFile(this.utePercorsoFile8.Text);
                else
                    if (!this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Modello")) modellodocx = (byte[])this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Modello"];

                if (modellodocx != null && modellodocx.Length > 0)
                {
                    string modellodocxfullpath = "TMP" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    if (this.uceCodFormatoReport8.Value != null && this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_WORD.ToUpper())
                    {
                        modellodocxfullpath += @".docx";
                    }
                    else if (this.uceCodFormatoReport8.Value != null && this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_PDF.ToUpper())
                    {
                        modellodocxfullpath += @".pdf";
                    }
                    modellodocxfullpath = System.IO.Path.Combine(Scci.Statics.FileStatics.GetSCCITempPath() + modellodocxfullpath);
                    UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(modellodocxfullpath, ref modellodocx);

                    if (this.uceCodFormatoReport8.Value != null && this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_WORD.ToUpper())
                    {
                        MyStatics.ApriDOCX(modellodocxfullpath);
                    }
                    else if (this.uceCodFormatoReport8.Value != null && this.uceCodFormatoReport8.Value.ToString().Trim().ToUpper() == MyStatics.FR_PDF.ToUpper())
                    {
                        MyStatics.ApriPDF(modellodocxfullpath);
                    }
                }
                else
                    MessageBox.Show(@"Impossibile reperire il modello!", "Anteprima documento", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, @"ubOpenDOCX8_Click", this.Name);
            }
        }

        private void uteCodEntita10_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodEntita10.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Entita";
                f.ViewSqlStruct.Where = "UsaTestiRTF = 1";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodEntita10.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodEntita10_EditorButtonClick", this.Text);
            }

        }
        private void uteCodEntita10_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodEntitaDes10.Text = DataBase.FindValue("Descrizione", "T_Entita", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodEntita10.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void chkTP10_CheckedChanged(object sender, EventArgs e)
        {
            CaricaTestiPredefiniti(this.chkTP10.Checked);
        }

        private void utvTPSX10_AfterCheck(object sender, NodeEventArgs e)
        {

            try
            {

                if (e.TreeNode.CheckedState == CheckState.Unchecked)
                {
                    DataRow[] results = _TestiPredefiniti_Campi[MyStatics.TV_VOCE].Select(string.Format("Codice = '{0}' And ID = '{1}'", e.TreeNode.Parent.Key, e.TreeNode.Tag.ToString()));
                    if (results != null && results.ToArray().Length == 1)
                    {
                        _TestiPredefiniti_Campi[MyStatics.TV_VOCE].Rows.Remove(results[0]);
                    }
                    DataRow drelimina = _TestiPredefiniti_Campi[MyStatics.GC_ELIMINA].NewRow();
                    drelimina["Codice"] = e.TreeNode.Parent.Key;
                    drelimina["Descrizione"] = e.TreeNode.Parent.Text;
                    drelimina["ID"] = e.TreeNode.Tag.ToString();
                    DataRow[] resultsnuovo = _TestiPredefiniti_Campi[MyStatics.GC_NUOVO].Select(string.Format("Codice = '{0}' And ID = '{1}'", e.TreeNode.Parent.Key, e.TreeNode.Tag.ToString()));
                    if (resultsnuovo != null && resultsnuovo.ToArray().Length == 1)
                    {
                        _TestiPredefiniti_Campi[MyStatics.GC_NUOVO].Rows.Remove(resultsnuovo[0]);
                    }
                    else
                    {
                        _TestiPredefiniti_Campi[MyStatics.GC_ELIMINA].Rows.Add(drelimina);
                    }
                }
                else
                {
                    DataRow dr = _TestiPredefiniti_Campi[MyStatics.TV_VOCE].NewRow();
                    dr["Codice"] = e.TreeNode.Parent.Key;
                    dr["Descrizione"] = e.TreeNode.Parent.Text;
                    dr["ID"] = e.TreeNode.Tag.ToString();
                    _TestiPredefiniti_Campi[MyStatics.TV_VOCE].Rows.Add(dr);
                    DataRow drnuovo = _TestiPredefiniti_Campi[MyStatics.GC_NUOVO].NewRow();
                    drnuovo["Codice"] = e.TreeNode.Parent.Key;
                    drnuovo["Descrizione"] = e.TreeNode.Parent.Text;
                    drnuovo["ID"] = e.TreeNode.Tag.ToString();
                    DataRow[] resultselimina = _TestiPredefiniti_Campi[MyStatics.GC_ELIMINA].Select(string.Format("Codice = '{0}' And ID = '{1}'", e.TreeNode.Parent.Key, e.TreeNode.Tag.ToString()));
                    if (resultselimina != null && resultselimina.ToArray().Length == 1)
                    {
                        _TestiPredefiniti_Campi[MyStatics.GC_ELIMINA].Rows.Remove(resultselimina[0]);
                    }
                    else
                    {
                        _TestiPredefiniti_Campi[MyStatics.GC_NUOVO].Rows.Add(drnuovo);
                    }
                }

                this.ubApplica.Enabled = true;

            }
            catch (Exception)
            {

            }

        }

        private void uteCampiFiltro10_ValueChanged(object sender, EventArgs e)
        {
            CaricaTestiPredefiniti(this.chkTP10.Checked);
        }

        private void uteCodAzi12_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodAzi12.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Aziende";
                f.ViewSqlStruct.Where = "Codice <> '" + this.uteCodAzi12.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodAzi12.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodAzi12_EditorButtonClick", this.Text);
            }
        }
        private void uteCodAzi12_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodAziDes12.Text = DataBase.FindValue("Descrizione", "T_Aziende", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodAzi12.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void utvEntitaAzioni2_AfterSelect(object sender, Infragistics.Win.UltraWinTree.SelectEventArgs e)
        {
            try
            {
                LoadRuoloAzioni();
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "utvEntitaAzioni2_AfterSelect", this.Text);
            }
        }

        private void utvRuoloAzioni2_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {
            try
            {
                if (!_runTime && (_Modality == Enums.EnumModalityPopUp.mpModifica || _Modality == Enums.EnumModalityPopUp.mpNuovo))
                {
                    string[] arrKeys = this.utvEntitaAzioni2.ActiveNode.Key.Split('|');

                    string sCodEntita = arrKeys[1];
                    string sCodAzione = arrKeys[3];

                    if (_RuoloEntitaAzione_Voci.ContainsKey(sCodEntita + @"|" + sCodAzione))
                    {
                        DataSet dsVoci = _RuoloEntitaAzione_Voci[sCodEntita + @"|" + sCodAzione];
                        string sCodVoce = e.TreeNode.Tag.ToString();
                        for (int iRow = 0; iRow < dsVoci.Tables[0].Rows.Count; iRow++)
                        {
                            if (dsVoci.Tables[0].Rows[iRow]["Codice"].ToString() == sCodVoce)
                            {
                                dsVoci.Tables[0].Rows[iRow]["Selected"] = (e.TreeNode.CheckedState == CheckState.Checked);

                                if (e.TreeNode.CheckedState == CheckState.Checked)
                                {
                                    string s_parametro = string.Empty;
                                    ParametriCIV o_parametri = JsonConvert.DeserializeObject<ParametriCIV>(dsVoci.Tables[0].Rows[iRow]["Parametri"].ToString());
                                    if (o_parametri == null) { o_parametri = new ParametriCIV(); }

                                    string s_ggcartelleinvisione = Microsoft.VisualBasic.Interaction.InputBox("Inserire GGCartelleInVisione:", "Cartella in Visione", "");
                                    if (s_ggcartelleinvisione != string.Empty && s_ggcartelleinvisione.All(char.IsNumber))
                                    {
                                        o_parametri.GGCartelleInVisione = int.Parse(s_ggcartelleinvisione);
                                        dsVoci.Tables[0].Rows[iRow]["Parametri"] = JsonConvert.SerializeObject(o_parametri);
                                        e.TreeNode.Text += " (" + s_ggcartelleinvisione + ")";
                                        int n = e.TreeNode.Text.IndexOf('(');
                                        if (n != -1)
                                        {
                                            e.TreeNode.Text = e.TreeNode.Text.Substring(0, n - 1);
                                        }
                                        e.TreeNode.Text += " (" + s_ggcartelleinvisione + ")";
                                    }
                                    else
                                    {
                                        dsVoci.Tables[0].Rows[iRow]["Parametri"] = DBNull.Value;
                                        int n = e.TreeNode.Text.IndexOf('(');
                                        if (n != -1)
                                        {
                                            e.TreeNode.Text = e.TreeNode.Text.Substring(0, n - 1);
                                        }
                                    }

                                }
                                else
                                {
                                    dsVoci.Tables[0].Rows[iRow]["Parametri"] = DBNull.Value;
                                    int n = e.TreeNode.Text.IndexOf('(');
                                    if (n != -1)
                                    {
                                        e.TreeNode.Text = e.TreeNode.Text.Substring(0, n - 1);
                                    }
                                }

                            }
                        }
                        _RuoloEntitaAzione_Voci[sCodEntita + @"|" + sCodAzione] = dsVoci;
                    }

                    this.ubApplica.Enabled = true;
                }
            }
            catch (Exception)
            {
            }
        }

        private void utvRuoloAzioni2_DoubleClick(object sender, EventArgs e)
        {

            if (!_runTime && (_Modality == Enums.EnumModalityPopUp.mpModifica || _Modality == Enums.EnumModalityPopUp.mpNuovo))
            {

                if (this.utvRuoloAzioni2.ActiveNode != null && this.utvRuoloAzioni2.ActiveNode.CheckedState == CheckState.Checked)
                {

                    string[] arrKeys = this.utvEntitaAzioni2.ActiveNode.Key.Split('|');

                    string sCodEntita = arrKeys[1];
                    string sCodAzione = arrKeys[3];

                    if (_RuoloEntitaAzione_Voci.ContainsKey(sCodEntita + @"|" + sCodAzione))
                    {
                        DataSet dsVoci = _RuoloEntitaAzione_Voci[sCodEntita + @"|" + sCodAzione];
                        string sCodVoce = this.utvRuoloAzioni2.ActiveNode.Tag.ToString();
                        for (int iRow = 0; iRow < dsVoci.Tables[0].Rows.Count; iRow++)
                        {
                            if (dsVoci.Tables[0].Rows[iRow]["Codice"].ToString() == sCodVoce)
                            {

                                string s_parametro = string.Empty;
                                ParametriCIV o_parametri = JsonConvert.DeserializeObject<ParametriCIV>(dsVoci.Tables[0].Rows[iRow]["Parametri"].ToString());
                                if (o_parametri == null) { o_parametri = new ParametriCIV(); }

                                string s_ggcartelleinvisione = Microsoft.VisualBasic.Interaction.InputBox("Inserire GGCartelleInVisione:", "Cartella in Visione", o_parametri.GGCartelleInVisione.ToString());
                                if (s_ggcartelleinvisione != string.Empty && s_ggcartelleinvisione.All(char.IsNumber))
                                {
                                    o_parametri.GGCartelleInVisione = int.Parse(s_ggcartelleinvisione);
                                    dsVoci.Tables[0].Rows[iRow]["Parametri"] = JsonConvert.SerializeObject(o_parametri);

                                    int n = this.utvRuoloAzioni2.ActiveNode.Text.IndexOf('(');
                                    if (n != -1)
                                    {
                                        this.utvRuoloAzioni2.ActiveNode.Text = this.utvRuoloAzioni2.ActiveNode.Text.Substring(0, n - 1);
                                    }
                                    this.utvRuoloAzioni2.ActiveNode.Text += " (" + s_ggcartelleinvisione + ")";
                                }

                            }
                        }
                        _RuoloEntitaAzione_Voci[sCodEntita + @"|" + sCodAzione] = dsVoci;
                    }

                    this.ubApplica.Enabled = true;

                }

            }

        }

        private void uteAzioniFiltro2_ValueChanged(object sender, EventArgs e)
        {

            try
            {
                LoadRuoloAzioni();
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteAzioniFiltro2_ValueChanged", this.Text);
            }

        }

        private void uceAzioniFiltro2_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                MyStatics.FiltraTVCheck(ref this.uceAzioniFiltro2, ref this.utvRuoloAzioni2);
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uceAzioniFiltro2_ValueChanged", this.Text);
            }

        }

        private void ucpColore14_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColore14.Value = this.colorDialog.Color;
            }
        }

        private void ucpColore22_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColore22.Value = this.colorDialog.Color;
            }
        }

        private void ucpColore15_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColore15.Value = this.colorDialog.Color;
            }
        }

        private void uteCodUA16_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodUA16.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_UnitaAtomiche";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodUA16.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodUA16.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodUA16_EditorButtonClick", this.Text);
            }

        }

        private void uteCodUA16_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodUADes16.Text = DataBase.FindValue("Descrizione", "T_UnitaAtomiche", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodUA16.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void ucpColore17_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColore17.Value = this.colorDialog.Color;
            }
        }

        private void ucpColoreGrafico20_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColoreGrafico20.Value = this.colorDialog.Color;
            }
        }

        private void uteCodAzi18_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodAzi18.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Aziende";
                f.ViewSqlStruct.Where = "Codice <> '" + this.uteCodAzi18.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodAzi18.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodAzi18_EditorButtonClick", this.Text);
            }
        }
        private void uteCodAzi18_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodAziDes18.Text = DataBase.FindValue("Descrizione", "T_Aziende", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodAzi18.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodScheda20_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                switch (e.Button.Key)
                {

                    case "Zoom":
                        frmZoom f = new frmZoom();
                        f.ViewText = this.lblCodScheda20.Text;
                        f.ViewIcon = this.ViewIcon;
                        f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Schede";
                        f.ViewSqlStruct.Where = "CodEntita = '" + this.ucMultiSelectUA20.Tag.ToString() + "' And Codice <> '" + DataBase.Ax2(this.uteCodScheda20.Text) + "'";
                        f.ViewInit();
                        f.ShowDialog();
                        if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            this.uteCodScheda20.Text = f.ViewActiveRow.Cells["Codice"].Text;
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
                MyStatics.ExGest(ref ex, "uteCodScheda20_EditorButtonClick", this.Text);
            }
        }
        private void uteCodScheda20_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodSchedaDes20.Text = DataBase.FindValue("Descrizione", "T_Schede", string.Format("Codice = '{0}' AND CodEntita = '{1}'", new string[2] { DataBase.Ax2(this.uteCodScheda20.Text), this.ucMultiSelectUA20.Tag.ToString() }), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodViaSomministrazione20_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodViaSomministrazione20.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_ViaSomministrazione";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodViaSomministrazione20.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodViaSomministrazione20.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodViaSomministrazione20_EditorButtonClick", this.Text);
            }
        }
        private void uteCodViaSomministrazione20_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodViaSomministrazioneDes20.Text = DataBase.FindValue("Descrizione", "T_ViaSomministrazione", string.Format("Codice = '{0}'", new string[1] { DataBase.Ax2(this.uteCodViaSomministrazione20.Text) }), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodSchedaPosologia20_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodSchedaPosologia20.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Schede";
                f.ViewSqlStruct.Where = "CodEntita = '" + Scci.Enums.EnumEntita.PRT.ToString() + "' And Codice <> '" + DataBase.Ax2(this.uteCodSchedaPosologia20.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodSchedaPosologia20.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodSchedaPosologia20_EditorButtonClick", this.Text);
            }
        }
        private void uteCodSchedaPosologia20_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodSchedaPosologiaDes20.Text = DataBase.FindValue("Descrizione", "T_Schede", string.Format("Codice = '{0}' AND CodEntita = '{1}'", new string[2] { DataBase.Ax2(this.uteCodSchedaPosologia20.Text), Scci.Enums.EnumEntita.PRT.ToString() }), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodAzi25_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodAzi25.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Aziende";
                f.ViewSqlStruct.Where = "Codice <> '" + this.uteCodAzi25.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodAzi25.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodAzi25_EditorButtonClick", this.Text);
            }
        }
        private void uteCodAzi25_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodAziDes25.Text = DataBase.FindValue("Descrizione", "T_Aziende", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodAzi25.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodSettore25_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodAzi25.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Settori";
                f.ViewSqlStruct.Where = "CodAzi = '" + DataBase.Ax2(this.uteCodAzi25.Text) + @"' And Codice <> '" + DataBase.Ax2(this.uteCodSettore25.Text) + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodSettore25.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodSettore25_EditorButtonClick", this.Text);
            }
        }
        private void uteCodSettore25_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodSettoreDes25.Text = DataBase.FindValue("Descrizione", "T_Settori", string.Format("CodAzi = '{0}' And Codice = '{1}'", DataBase.Ax2(this.uteCodAzi25.Text), DataBase.Ax2(this.uteCodSettore25.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodStanza25_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodAzi25.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Stanze";
                f.ViewSqlStruct.Where = "CodAzi = '" + DataBase.Ax2(this.uteCodAzi25.Text) + @"' And Codice <> '" + DataBase.Ax2(this.uteCodStanza25.Text) + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodStanza25.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodStanza25_EditorButtonClick", this.Text);
            }
        }
        private void uteCodStanza25_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodStanzaDes25.Text = DataBase.FindValue("Descrizione", "T_Stanze", string.Format("CodAzi = '{0}' And Codice = '{1}'", DataBase.Ax2(this.uteCodAzi25.Text), DataBase.Ax2(this.uteCodStanza25.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodAzi26_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodAzi26.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Aziende";
                f.ViewSqlStruct.Where = "Codice <> '" + this.uteCodAzi26.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodAzi26.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodAzi26_EditorButtonClick", this.Text);
            }
        }
        private void uteCodAzi26_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodAziDes26.Text = DataBase.FindValue("Descrizione", "T_Aziende", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodAzi26.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodDestinatario27_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodDestinatario27.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Integra_Destinatari";
                f.ViewSqlStruct.Where = "Codice <> '" + this.uteCodDestinatario27.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodDestinatario27.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodDestinatario27_EditorButtonClick", this.Text);
            }
        }
        private void uteCodDestinatario27_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodDestinatarioDes27.Text = DataBase.FindValue("Descrizione", "T_Integra_Destinatari", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodDestinatario27.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodScheda29_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodScheda29.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Schede";
                f.ViewSqlStruct.Where = "SchedaSemplice = 0";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodScheda29.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "uteCodScheda29_EditorButtonClick", this.Text);
            }
        }
        private void uteCodScheda29_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodSchedaDes29.Text = DataBase.FindValue("Descrizione", "T_Schede", string.Format("Codice = '{0}' And SchedaSemplice = 0", DataBase.Ax2(this.uteCodScheda29.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void ucpColore29_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColore29.Value = this.colorDialog.Color;
            }
        }

        private void ubStruttura_Click(object sender, EventArgs e)
        {

            try
            {

                frmZoomUA f = new frmZoomUA();
                f.ViewIcon = this.ViewIcon;
                f.ViewText = this.uteDescrizione4.Text;
                f.ViewParametro = this.uteCodice4.Text;
                f.ViewInit();
                f.ShowDialog();
                f = null;

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

                            this.ViewDataBindings.DsLogPrima = null;
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            if (this.ViewDataNamePU == Enums.EnumDataNames.T_UnitaAtomiche && this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                            {
                                this.CopiaBindingsAss();
                            }
                            else
                            {
                                this.UpdateBindingsAss();
                            }
                            switch (this.ViewDataNamePU)
                            {
                                case Enums.EnumDataNames.T_Aziende:
                                case Enums.EnumDataNames.T_CDSSAzioni:
                                case Enums.EnumDataNames.T_TipoAllegato:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice1.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Ruoli:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice2.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Login:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice3.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_UnitaAtomiche:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice4.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_DiarioInfermieristico:
                                case Enums.EnumDataNames.T_DiarioMedico:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice5.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_TipoParametroVitale:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice6.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                                case Enums.EnumDataNames.T_TipoOrdine:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice7.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Report:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice8.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Maschere:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice9.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_TestiPredefiniti:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice10.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_MovNews:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice11.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_UnitaOperative:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice12.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_ViaSomministrazione:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice13.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_StatoAppuntamento:
                                case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                                case Enums.EnumDataNames.T_StatoAllegato:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice15.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice14.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Stanze:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice18.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_FormatoReport:
                                case Enums.EnumDataNames.T_TipoEpisodio:
                                case Enums.EnumDataNames.T_TipoDiario:
                                case Enums.EnumDataNames.T_TipoScheda:
                                case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice19.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_TipoPrescrizione:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice20.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Letti:
                                    this.ViewDataBindings.SqlSelect.Where = "CodAzi = '" + DataBase.Ax2(this.uteCodAzi25.Text) + "' AND CodLetto = '" + DataBase.Ax2(this.uteCodLetto25.Text) + "' AND CodSettore = '" + DataBase.Ax2(this.uteCodSettore25.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_Settori:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice26.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Contatori:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice28.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                                    this.ViewDataBindings.SqlSelect.Where = @"CodScheda = '" + DataBase.Ax2(this.uteCodScheda29.Text) + @"' AND Codice = '" + DataBase.Ax2(this.uteCodice29.Text) + @"'";
                                    break;
                                case Enums.EnumDataNames.T_TestiNotePredefiniti:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice22.Text + "'";
                                    break;
                            }
                            this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpNuovo, getEntitaFromDataNames(), this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
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
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpModifica, getEntitaFromDataNames(), this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
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

                            this.ViewDataBindings.DsLogPrima = null;
                            this.UpdateBindings();
                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            if (this.ViewDataNamePU == Enums.EnumDataNames.T_UnitaAtomiche && this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                            {
                                this.CopiaBindingsAss();
                            }
                            else
                            {
                                this.UpdateBindingsAss();
                            }
                            switch (this.ViewDataNamePU)
                            {
                                case Enums.EnumDataNames.T_Aziende:
                                case Enums.EnumDataNames.T_CDSSAzioni:
                                case Enums.EnumDataNames.T_TipoAllegato:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice1.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Ruoli:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice2.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Login:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice3.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_UnitaAtomiche:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice4.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_DiarioInfermieristico:
                                case Enums.EnumDataNames.T_DiarioMedico:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice5.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_TipoParametroVitale:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice6.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_TipoEvidenzaClinica:
                                case Enums.EnumDataNames.T_TipoOrdine:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice7.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Report:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice8.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Maschere:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice9.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_TestiPredefiniti:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice10.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_MovNews:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice11.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_UnitaOperative:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice12.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_ViaSomministrazione:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice13.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_StatoAppuntamento:
                                case Enums.EnumDataNames.T_StatoAppuntamentoAgende:
                                case Enums.EnumDataNames.T_StatoAllegato:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice15.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_StatoTaskInfermieristico:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice14.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Stanze:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice18.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_FormatoReport:
                                case Enums.EnumDataNames.T_TipoEpisodio:
                                case Enums.EnumDataNames.T_TipoDiario:
                                case Enums.EnumDataNames.T_TipoScheda:
                                case Enums.EnumDataNames.T_StatoConsensoCalcolato:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice19.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_TipoPrescrizione:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice20.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Letti:
                                    this.ViewDataBindings.SqlSelect.Where = "CodAzi = '" + DataBase.Ax2(this.uteCodAzi25.Text) + "' AND CodLetto = '" + DataBase.Ax2(this.uteCodLetto25.Text) + "' AND CodSettore = '" + DataBase.Ax2(this.uteCodSettore25.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_Settori:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice26.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_Contatori:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice28.Text + "'";
                                    break;

                                case Enums.EnumDataNames.T_StatoSchedaCalcolato:
                                    this.ViewDataBindings.SqlSelect.Where = @"CodScheda = '" + DataBase.Ax2(this.uteCodScheda29.Text) + @"' AND Codice = '" + DataBase.Ax2(this.uteCodice29.Text) + @"'";
                                    break;
                                case Enums.EnumDataNames.T_TestiNotePredefiniti:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice22.Text + "'";
                                    break;
                            }
                            this.ViewDataBindings.DsLogDopo = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpNuovo, getEntitaFromDataNames(), this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
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
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpModifica, getEntitaFromDataNames(), this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
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
                            MyStatics.LogManager(Enums.EnumModalityPopUp.mpCancella, getEntitaFromDataNames(), this.ViewDataBindings.DsLogPrima, this.ViewDataBindings.DsLogDopo);
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
