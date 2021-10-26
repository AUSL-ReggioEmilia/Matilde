using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Reflection;
using System.Xml;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinSchedule;
using Infragistics.Win.UltraWinTree;
using UnicodeSrl.Framework.Collections;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using Infragistics.Win.UltraWinEditors;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmPUView2 : Form, Interfacce.IViewFormPUView
    {
        public frmPUView2()
        {
            InitializeComponent();
        }

        #region Declare

        private PUDataBindings _DataBinds = new PUDataBindings();
        private Enums.EnumModalityPopUp _Modality;
        private Enums.EnumDataNames _ViewDataNamePU;
        private DialogResult _DialogResult = DialogResult.Cancel;

        private WorkingHourTime mo_WorkingHourTime = new WorkingHourTime();
        private List<WorkingHourTimeDaysOfWeek> mo_WorkingHourTimeDaysOfWeek = new List<WorkingHourTimeDaysOfWeek>();
        private ParametriListaAgenda mo_ParametriListaAgenda = new ParametriListaAgenda();
        private MassimaliAgenda mo_MassimaliAgenda = new MassimaliAgenda();

        private bool _runtime = false;

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
                return this.PicImageShared.Image;
            }
            set
            {
                this.PicImageShared.Image = value;
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
            this.InitializeUltraToolbarsManager();
            this.InitializeUltraGrid();
            this.InitializeUltraTree();

            SetBindings();
            SetBindingsAss();

            switch (_Modality)
            {

                case Enums.EnumModalityPopUp.mpNuovo:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.umeMinuti21.Enabled = false;
                    this.uteCodUA22.Enabled = false;
                    this.uteCodIntestazione22.Enabled = false;
                    this.ubApplica.Enabled = true;
                    if (this.ViewDataNamePU == Enums.EnumDataNames.T_ProtocolliTempi) this.uteCodProtocollo10.Enabled = false;
                    if (this.ViewDataNamePU == Enums.EnumDataNames.T_Protocolli) this.chkContinuita9_CheckedChanged(this.chkContinuita9, new System.EventArgs());
                    if (this.ViewDataNamePU == Enums.EnumDataNames.T_Agende) this.chkLista6_CheckedChanged(this.chkLista6, new System.EventArgs());
                    if (this.ViewDataNamePU == Enums.EnumDataNames.T_TipoAppuntamento) this.chkSenzaData7_CheckedChanged(this.chkSenzaData7, new System.EventArgs());
                    break;

                case Enums.EnumModalityPopUp.mpModifica:
                    MyStatics.SetControls(this.UltraGroupBox.Controls, true);
                    this.uteCodice1.Enabled = false;
                    this.uteCodice2.Enabled = false;
                    this.uteCodice3.Enabled = false;
                    this.uteCodice4.Enabled = false;
                    this.uteCodUA4.Enabled = false;
                    this.uteCodice7.Enabled = false;
                    this.uteCodice9.Enabled = false;
                    this.uteCodice10.Enabled = false;
                    this.uteCodice11.Enabled = false;
                    this.uteCodice13.Enabled = false;
                    this.uteCodice14.Enabled = false;
                    this.uteCodice15.Enabled = false;
                    this.uteCodice16.Enabled = false;
                    this.uteCodice17.Enabled = false;
                    this.uteCodice18.Enabled = false;
                    this.umeMinuti21.Enabled = false;
                    this.ubApplica.Enabled = false;
                    this.uteCodUA22.Enabled = false;
                    this.uteCodIntestazione22.Enabled = false;
                    if (this.ViewDataNamePU == Enums.EnumDataNames.T_ProtocolliTempi) this.uteCodProtocollo10.Enabled = false;
                    if (this.ViewDataNamePU == Enums.EnumDataNames.T_Protocolli) this.chkContinuita9_CheckedChanged(this.chkContinuita9, new System.EventArgs());
                    if (this.ViewDataNamePU == Enums.EnumDataNames.T_Agende) this.chkLista6_CheckedChanged(this.chkLista6, new System.EventArgs());
                    if (this.ViewDataNamePU == Enums.EnumDataNames.T_TipoAppuntamento) setCheckTipoAppuntamento7();
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
                    this.uteCodice6.Enabled = true;
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

        private void InitializeRichTextBox()
        {

            try
            {

                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_DCDecodificheValori:
                        this.ucRichTextBoxInfoRTF2.ViewInit();
                        break;

                    case Enums.EnumDataNames.T_AssUAIntestazioni:
                        this.ucRichTextBoxInfoRTF22.ViewInit();
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

            switch (_ViewDataNamePU)
            {

                case Enums.EnumDataNames.T_DCDecodificheValori:
                    this.ucPictureSelectIcona2.ViewShowSaveImage = false;
                    this.ucPictureSelectIcona2.ViewCheckSquareImage = true;
                    this.ucPictureSelectIcona2.ViewInit();
                    break;

                case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                    this.ucPictureSelectIcona3.ViewShowSaveImage = false;
                    this.ucPictureSelectIcona3.ViewCheckSquareImage = true;
                    this.ucPictureSelectIcona3.ViewInit();
                    break;

                case Enums.EnumDataNames.T_TipoAgenda:
                    this.ucPictureSelectIcona4.ViewShowSaveImage = false;
                    this.ucPictureSelectIcona4.ViewCheckSquareImage = true;
                    this.ucPictureSelectIcona4.ViewInit();
                    break;

                case Enums.EnumDataNames.T_TipoAppuntamento:
                    this.ucPictureSelectIcona7.ViewShowSaveImage = false;
                    this.ucPictureSelectIcona7.ViewCheckSquareImage = true;
                    this.ucPictureSelectIcona7.ViewInit();
                    break;

                case Enums.EnumDataNames.T_SezioniFUT:
                    this.ucPictureSelectIcona11.ViewShowSaveImage = false;
                    this.ucPictureSelectIcona11.ViewCheckSquareImage = true;
                    this.ucPictureSelectIcona11.ViewInit();
                    break;

                case Enums.EnumDataNames.T_TipoAlertGenerico:
                    this.ucPictureSelectIcona8.ViewShowSaveImage = false;
                    this.ucPictureSelectIcona8.ViewCheckSquareImage = true;
                    this.ucPictureSelectIcona8.ViewInit();
                    break;

                case Enums.EnumDataNames.T_CDSSPlugins:
                    this.ucPictureSelectIcona13.ViewShowSaveImage = false;
                    this.ucPictureSelectIcona13.ViewCheckSquareImage = true;
                    this.ucPictureSelectIcona13.ViewInit();
                    break;

                case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                    this.ucPictureSelectIcona18.ViewShowSaveImage = false;
                    this.ucPictureSelectIcona18.ViewCheckSquareImage = true;
                    this.ucPictureSelectIcona18.ViewInit();
                    break;

                default:
                    break;
            }

        }

        private void InitializeUltraCombo()
        {

            try
            {

                switch (_ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_Agende:
                        this.uceIntervalloSlot6.ValueMember = "Codice";
                        this.uceIntervalloSlot6.DisplayMember = "Descrizione";
                        this.uceIntervalloSlot6.SortStyle = Infragistics.Win.ValueListSortStyle.None;
                        DataSet oDSi = CoreStatics.GetTimeSlotIntervalDs();
                        this.uceIntervalloSlot6.DataMember = oDSi.Tables[0].TableName;
                        this.uceIntervalloSlot6.DataSource = oDSi;
                        this.uceIntervalloSlot6.DataBind();
                        this.uceTipoRaggruppamentoAgenda16.ValueMember = "Codice";
                        this.uceTipoRaggruppamentoAgenda16.DisplayMember = "Descrizione";
                        this.uceTipoRaggruppamentoAgenda16.SortStyle = Infragistics.Win.ValueListSortStyle.None;
                        DataSet oDS1 = CoreStatics.GetTipoRaggruppamentoAgendaDs();
                        this.uceTipoRaggruppamentoAgenda16.DataMember = oDS1.Tables[0].TableName;
                        this.uceTipoRaggruppamentoAgenda16.DataSource = oDS1;
                        this.uceTipoRaggruppamentoAgenda16.DataBind();
                        this.uceTipoRaggruppamentoAgenda26.ValueMember = "Codice";
                        this.uceTipoRaggruppamentoAgenda26.DisplayMember = "Descrizione";
                        this.uceTipoRaggruppamentoAgenda26.SortStyle = Infragistics.Win.ValueListSortStyle.None;
                        DataSet oDS2 = CoreStatics.GetTipoRaggruppamentoAgendaDs();
                        this.uceTipoRaggruppamentoAgenda26.DataMember = oDS2.Tables[0].TableName;
                        this.uceTipoRaggruppamentoAgenda26.DataSource = oDS2;
                        this.uceTipoRaggruppamentoAgenda26.DataBind();
                        this.uceTipoRaggruppamentoAgenda36.ValueMember = "Codice";
                        this.uceTipoRaggruppamentoAgenda36.DisplayMember = "Descrizione";
                        this.uceTipoRaggruppamentoAgenda36.SortStyle = Infragistics.Win.ValueListSortStyle.None;
                        DataSet oDS3 = CoreStatics.GetTipoRaggruppamentoAgendaDs();
                        this.uceTipoRaggruppamentoAgenda36.DataMember = oDS3.Tables[0].TableName;
                        this.uceTipoRaggruppamentoAgenda36.DataSource = oDS3;
                        this.uceTipoRaggruppamentoAgenda36.DataBind();
                        this.uceCodPeriodoDisponibilita6.ValueMember = "Codice";
                        this.uceCodPeriodoDisponibilita6.DisplayMember = "Descrizione";
                        this.uceCodPeriodoDisponibilita6.SortStyle = Infragistics.Win.ValueListSortStyle.None;
                        DataSet oDS4 = CoreStatics.GetCodPeriodoDisponibilitaDs();
                        this.uceCodPeriodoDisponibilita6.DataMember = oDS4.Tables[0].TableName;
                        this.uceCodPeriodoDisponibilita6.DataSource = oDS4;
                        this.uceCodPeriodoDisponibilita6.DataBind();
                        break;

                    case Enums.EnumDataNames.T_Protocolli:
                        MyStatics.SetUltraComboEditorLayout(ref this.uceCodTipoProtocollo9);
                        this.uceCodTipoProtocollo9.ValueMember = "Codice";
                        this.uceCodTipoProtocollo9.DisplayMember = "Descrizione";
                        DataSet oDs = CoreStatics.GetTipoProtocolliDs();
                        this.uceCodTipoProtocollo9.DataMember = oDs.Tables[0].TableName;
                        this.uceCodTipoProtocollo9.DataSource = oDs;
                        this.uceCodTipoProtocollo9.DataBind();
                        break;

                    default:
                        break;

                }

            }
            catch (Exception)
            {

            }

        }

        private void InitializeUltraTree()
        {

            try
            {

                MyStatics.SetUltraTree(this.utvOrariLavoro21);

            }
            catch (Exception)
            {

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

                    case Enums.EnumDataNames.T_DCDecodifiche:
                        this.UltraTabControl.Tabs["tab1"].Visible = true;
                        this.UltraTabControl.Tabs["tab1"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice1);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione1);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice1.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione1.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_DCDecodificheValori:
                        this.UltraTabControl.Tabs["tab2"].Visible = true;
                        this.UltraTabControl.Tabs["tab2"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice2);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione2);
                        _DataBinds.DataBindings.Add("Text", "Ordine", this.umeOrdine2);
                        _DataBinds.DataBindings.Add("Value", "DtValI", this.udteDataInizioValidita2);
                        _DataBinds.DataBindings.Add("Value", "DtValF", this.udteDataFineValidita2);
                        _DataBinds.DataBindings.Add("ViewRtf", "InfoRTF", this.ucRichTextBoxInfoRTF2);
                        _DataBinds.DataBindings.Add("Text", "Path", this.utePath2);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice2.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione2.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Path"];
                        if (_dcol.MaxLength > 0) this.utePath2.MaxLength = _dcol.MaxLength;

                        if (_Modality == Enums.EnumModalityPopUp.mpNuovo &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != null &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != "")
                            this.ucRichTextBoxInfoRTF2.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));

                        break;

                    case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                        this.UltraTabControl.Tabs["tab3"].Visible = true;
                        this.UltraTabControl.Tabs["tab3"].Text = this.ViewText;

                        try
                        {
                            if (_DataBinds.DataBindings.DataSet != null && _DataBinds.DataBindings.DataSet.Tables.Count > 0 && _DataBinds.DataBindings.DataSet.Tables[0].Rows.Count > 0)
                                if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Anticipo"))
                                    _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Anticipo"] = 0;
                        }
                        catch
                        {
                        }
                        try
                        {
                            if (_DataBinds.DataBindings.DataSet != null && _DataBinds.DataBindings.DataSet.Tables.Count > 0 && _DataBinds.DataBindings.DataSet.Tables[0].Rows.Count > 0)
                                if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Ripianificazione"))
                                    _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Ripianificazione"] = 0;
                        }
                        catch
                        {
                        }

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice3);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione3);
                        _DataBinds.DataBindings.Add("Value", "Colore", this.ucpColore3);
                        _DataBinds.DataBindings.Add("Text", "Sigla", this.uteSigla3);
                        _DataBinds.DataBindings.Add("Text", "CodScheda", this.uteCodScheda3);
                        _DataBinds.DataBindings.Add("Value", "Anticipo", this.umeAnticipo3);
                        _DataBinds.DataBindings.Add("Value", "Ripianificazione", this.umeRipianificazione3);
                        _DataBinds.DataBindings.Add("Checked", "ErogazioneDiretta", this.chkErogazioneDiretta3);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice3.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione3.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Sigla"];
                        if (_dcol.MaxLength > 0) this.uteSigla3.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodScheda"];
                        if (_dcol.MaxLength > 0) this.uteCodScheda3.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_TipoAgenda:
                        this.UltraTabControl.Tabs["tab4"].Visible = true;
                        this.UltraTabControl.Tabs["tab4"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice4);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione4);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice4.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione4.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_AssUAUOLetti:
                        this.UltraTabControl.Tabs["tab5"].Visible = true;
                        this.UltraTabControl.Tabs["tab5"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "CodUA", this.uteCodUA4);
                        _DataBinds.DataBindings.Add("Text", "CodAzi", this.uteCodAzi4);
                        _DataBinds.DataBindings.Add("Text", "CodUO", this.uteCodUO4);
                        _DataBinds.DataBindings.Add("Text", "CodSettore", this.uteCodSettore4);
                        _DataBinds.DataBindings.Add("Text", "Codletto", this.uteCodLetto4);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodUA"];
                        if (_dcol.MaxLength > 0) this.uteCodUA4.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodAzi"];
                        if (_dcol.MaxLength > 0) this.uteCodAzi4.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodUO"];
                        if (_dcol.MaxLength > 0) this.uteCodUO4.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodSettore"];
                        if (_dcol.MaxLength > 0) this.uteCodSettore4.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codletto"];
                        if (_dcol.MaxLength > 0) this.uteCodLetto4.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Agende:
                        this.UltraTabControl.Tabs["tab6"].Visible = true;
                        this.UltraTabControl.Tabs["tab6"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice6);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione6);
                        _DataBinds.DataBindings.Add("Text", "DescrizioneAlternativa", this.uteDescrizioneAlternativa6);
                        _DataBinds.DataBindings.Add("Value", "Colore", this.ucpColore6);
                        _DataBinds.DataBindings.Add("Value", "IntervalloSlot", this.uceIntervalloSlot6);
                        _DataBinds.DataBindings.Add("Text", "Ordine", this.umeOrdine6);
                        _DataBinds.DataBindings.Add("Checked", "UsaColoreTipoAppuntamento", this.chkUsaColoreTipoAppuntamento6);
                        _DataBinds.DataBindings.Add("Checked", "EscludiFestivita", this.chkEscludiFestivita6);
                        _DataBinds.DataBindings.Add("Text", "MassimoAnticipoPrenotazione", this.umeMassimoAnticipoPrenotazione6);
                        _DataBinds.DataBindings.Add("Text", "MassimoRitardoPrenotazione", this.umeMassimoRitardoPrenotazione6);
                        _DataBinds.DataBindings.Add("Checked", "Lista", this.chkLista6);
                        _DataBinds.DataBindings.Add("Value", "CodPeriodoDisponibilita", this.uceCodPeriodoDisponibilita6);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice6.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione6.MaxLength = _dcol.MaxLength;


                        this.LoadParametriLista();
                        this.LoadHour();
                        this.LoadUltraGridOrari();
                        this.LoadUltraGridMassimali();

                        break;

                    case Enums.EnumDataNames.T_TipoAppuntamento:
                        this.UltraTabControl.Tabs["tab7"].Visible = true;
                        this.UltraTabControl.Tabs["tab7"].Text = this.ViewText;

                        try
                        {
                            if (_DataBinds.DataBindings.DataSet != null && _DataBinds.DataBindings.DataSet.Tables.Count > 0 && _DataBinds.DataBindings.DataSet.Tables[0].Rows.Count > 0)
                                if (_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Ripianificazione"))
                                    _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["Ripianificazione"] = 0;
                        }
                        catch
                        {
                        }

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice7);
                        _DataBinds.DataBindings.Add("Checked", "Multiplo", this.chkMultiplo7);
                        _DataBinds.DataBindings.Add("Checked", "SenzaData", this.chkSenzaData7);
                        _DataBinds.DataBindings.Add("Checked", "SenzaDataSempre", this.chkSenzaDataSempre7);
                        _DataBinds.DataBindings.Add("Checked", "Settimanale", this.chkSettimanale7);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione7);
                        _DataBinds.DataBindings.Add("Value", "Colore", this.ucpColore7);
                        _DataBinds.DataBindings.Add("Value", "TimeSlotInterval", this.umeTimeSlotInterval7);
                        _DataBinds.DataBindings.Add("Text", "CodScheda", this.uteCodScheda7);
                        _DataBinds.DataBindings.Add("Text", "FormulaTitolo", this.uteFormulaTitolo);
                        _DataBinds.DataBindings.Add("Value", "Ripianificazione", this.umeRipianificazione7);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice7.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione7.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodScheda"];
                        if (_dcol.MaxLength > 0) this.uteCodScheda7.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_TipoAlertGenerico:
                        this.UltraTabControl.Tabs["tab8"].Visible = true;
                        this.UltraTabControl.Tabs["tab8"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice8);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione8);
                        _DataBinds.DataBindings.Add("Text", "CodScheda", this.uteCodScheda8);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice8.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione8.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodScheda"];
                        if (_dcol.MaxLength > 0) this.uteCodScheda8.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Protocolli:
                        this.UltraTabControl.Tabs["tab9"].Visible = true;
                        this.UltraTabControl.Tabs["tab9"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice9);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione9);
                        _DataBinds.DataBindings.Add("Checked", "Continuita", this.chkContinuita9);
                        _DataBinds.DataBindings.Add("Value", "Durata", this.umeDurata9);
                        _DataBinds.DataBindings.Add("Value", "CodTipoProtocollo", this.uceCodTipoProtocollo9);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice9.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione9.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_ProtocolliTempi:
                        this.UltraTabControl.Tabs["tab10"].Visible = true;
                        this.UltraTabControl.Tabs["tab10"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice10);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione10);
                        _DataBinds.DataBindings.Add("Text", "CodProtocollo", this.uteCodProtocollo10);
                        _DataBinds.DataBindings.Add("Value", "Delta", this.umeDelta10);
                        _DataBinds.DataBindings.Add("Value", "Ora", this.udteOra10);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice10.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione10.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_SezioniFUT:
                        this.UltraTabControl.Tabs["tab11"].Visible = true;
                        this.UltraTabControl.Tabs["tab11"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice11);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione11);
                        _DataBinds.DataBindings.Add("Value", "Ordine", this.umeOrdine11);
                        _DataBinds.DataBindings.Add("Text", "CodEntita", this.uteCodEntita11);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice10.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione10.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_CDSSStruttura:
                        this.UltraTabControl.Tabs["tab12"].Visible = true;
                        this.UltraTabControl.Tabs["tab12"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "CodUA", this.uteCodUA12);
                        _DataBinds.DataBindings.Add("Text", "CodAzione", this.uteCodAzione12);
                        _DataBinds.DataBindings.Add("Text", "CodPlugin", this.uteCodPlugin12);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodUA"];
                        if (_dcol.MaxLength > 0) this.uteCodUA12.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodAzione"];
                        if (_dcol.MaxLength > 0) this.uteCodAzione12.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodPlugin"];
                        if (_dcol.MaxLength > 0) this.uteCodPlugin12.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_CDSSPlugins:
                        this.UltraTabControl.Tabs["tab13"].Visible = true;
                        this.UltraTabControl.Tabs["tab13"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice13);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione13);
                        _DataBinds.DataBindings.Add("Text", "NomePlugin", this.uteNomePlugin13);
                        _DataBinds.DataBindings.Add("Text", "Comando", this.uteComando13);
                        _DataBinds.DataBindings.Add("Text", "Modalita", this.uteModalita13);
                        _DataBinds.DataBindings.Add("Text", "CodTipoCDSS", this.uteCodTipoCDSS13);
                        _DataBinds.DataBindings.Add("Value", "Ordine", this.umeOrdine13);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice13.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione13.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["NomePlugin"];
                        if (_dcol.MaxLength > 0) this.uteNomePlugin13.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Comando"];
                        if (_dcol.MaxLength > 0) this.uteComando13.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Modalita"];
                        if (_dcol.MaxLength > 0) this.uteModalita13.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodTipoCDSS"];
                        if (_dcol.MaxLength > 0) this.uteCodTipoCDSS13.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_EBM:
                        this.UltraTabControl.Tabs["tab14"].Visible = true;
                        this.UltraTabControl.Tabs["tab14"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice14);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione14);
                        _DataBinds.DataBindings.Add("Text", "Url", this.uteUrl14);
                        _DataBinds.DataBindings.Add("Value", "Ordine", this.umeOrdine14);
                        _DataBinds.DataBindings.Add("Text", "Note", this.uteNote14);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice14.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione14.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Url"];
                        if (_dcol.MaxLength > 0) this.uteUrl14.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Note"];
                        if (_dcol.MaxLength > 0) this.uteNote14.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Integra_Destinatari:
                        this.UltraTabControl.Tabs["tab15"].Visible = true;
                        this.UltraTabControl.Tabs["tab15"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice15);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione15);
                        _DataBinds.DataBindings.Add("Text", "Indirizzo", this.uteIndirizzo15);
                        _DataBinds.DataBindings.Add("Text", "Dominio", this.uteDominio15);
                        _DataBinds.DataBindings.Add("Text", "Utente", this.uteUtente15);
                        _DataBinds.DataBindings.Add("Text", "Password", this.utePassword15);
                        _DataBinds.DataBindings.Add("Text", "Note", this.uteNote15);
                        _DataBinds.DataBindings.Add("Checked", "Https", this.chkHttps15);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice15.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione15.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Indirizzo"];
                        if (_dcol.MaxLength > 0) this.uteIndirizzo15.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Dominio"];
                        if (_dcol.MaxLength > 0) this.uteDominio15.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Utente"];
                        if (_dcol.MaxLength > 0) this.uteUtente15.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Password"];
                        if (_dcol.MaxLength > 0) this.utePassword15.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Note"];
                        if (_dcol.MaxLength > 0) this.uteNote15.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Integra_Campi:
                        this.UltraTabControl.Tabs["tab16"].Visible = true;
                        this.UltraTabControl.Tabs["tab16"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice16);
                        _DataBinds.DataBindings.Add("Text", "CodEntita", this.uteCodEntita16);
                        _DataBinds.DataBindings.Add("Text", "CodTipoEntita", this.uteCodTipoEntita16);
                        _DataBinds.DataBindings.Add("Text", "Campo", this.uteCampo16);
                        _DataBinds.DataBindings.Add("Text", "Note", this.uteNote16);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice16.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodEntita"];
                        if (_dcol.MaxLength > 0) this.uteCodEntita16.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodTipoEntita"];
                        if (_dcol.MaxLength > 0) this.uteCodTipoEntita16.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Campo"];
                        if (_dcol.MaxLength > 0) this.uteCampo16.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Note"];
                        if (_dcol.MaxLength > 0) this.uteNote16.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_FiltriSpeciali:
                        this.UltraTabControl.Tabs["tab17"].Visible = true;
                        this.UltraTabControl.Tabs["tab17"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice17);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione17);
                        _DataBinds.DataBindings.Add("Text", "CodTipoFiltroSpeciale", this.uteCodTipoFiltroSpeciale17);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice17.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione17.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodTipoFiltroSpeciale"];
                        if (_dcol.MaxLength > 0) this.uteCodTipoFiltroSpeciale17.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                        this.UltraTabControl.Tabs["tab18"].Visible = true;
                        this.UltraTabControl.Tabs["tab18"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "Codice", this.uteCodice18);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione18);
                        _DataBinds.DataBindings.Add("Text", "CodScheda", this.uteCodScheda18);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Codice"];
                        if (_dcol.MaxLength > 0) this.uteCodice18.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione18.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodScheda"];
                        if (_dcol.MaxLength > 0) this.uteCodScheda18.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                        this.UltraTabControl.Tabs["tab19"].Visible = true;
                        this.UltraTabControl.Tabs["tab19"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "CodRuolo", this.uteCodRuolo19);
                        _DataBinds.DataBindings.Add("Text", "CodAzione", this.uteCodAzione19);
                        _DataBinds.DataBindings.Add("Text", "CodPlugin", this.uteCodPlugin19);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodRuolo"];
                        if (_dcol.MaxLength > 0) this.uteCodRuolo19.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodAzione"];
                        if (_dcol.MaxLength > 0) this.uteCodAzione19.MaxLength = _dcol.MaxLength;
                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["CodPlugin"];
                        if (_dcol.MaxLength > 0) this.uteCodPlugin19.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_Festivita:
                        this.UltraTabControl.Tabs["tab20"].Visible = true;
                        this.UltraTabControl.Tabs["tab20"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Value", "Data", this.udteData20);
                        _DataBinds.DataBindings.Add("Text", "Descrizione", this.uteDescrizione20);

                        _dcol = _DataBinds.DataBindings.DataSet.Tables[0].Columns["Descrizione"];
                        if (_dcol.MaxLength > 0) this.uteDescrizione2.MaxLength = _dcol.MaxLength;
                        break;

                    case Enums.EnumDataNames.T_AgendePeriodi:
                        this.UltraTabControl.Tabs["tab21"].Visible = true;
                        this.UltraTabControl.Tabs["tab21"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Value", "DataInizio", this.udteDataInizio21);
                        _DataBinds.DataBindings.Add("Value", "DataFine", this.udteDataFine21);

                        break;

                    case Enums.EnumDataNames.T_AssUAIntestazioni:
                        this.UltraTabControl.Tabs["tab22"].Visible = true;
                        this.UltraTabControl.Tabs["tab22"].Text = this.ViewText;

                        _DataBinds.DataBindings.Add("Text", "CodUA", this.uteCodUA22);
                        _DataBinds.DataBindings.Add("Text", "CodIntestazione", this.uteCodIntestazione22);
                        _DataBinds.DataBindings.Add("Value", "DataInizio", this.udteDataInizio22);
                        _DataBinds.DataBindings.Add("Value", "DataFine", this.udteDataFine22);
                        _DataBinds.DataBindings.Add("ViewRtf", "Intestazione", this.ucRichTextBoxInfoRTF22);

                        if (_Modality == Enums.EnumModalityPopUp.mpNuovo &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != null &&
                            Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != "")
                            this.ucRichTextBoxInfoRTF22.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));

                        break;

                    default:
                        break;

                }

                _DataBinds.DataBindings.Load();

                switch (this.ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_DCDecodifiche:
                        this.LoadUltraGrid(ref this.UltraGrid1);
                        this.SetUltraToolBarManager(UltraToolbarsManager1, ref UltraGrid1, this.ViewModality != Enums.EnumModalityPopUp.mpModifica);
                        break;

                    case Enums.EnumDataNames.T_DCDecodificheValori:
                        this.ucPictureSelectIcona2.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        break;

                    case Enums.EnumDataNames.T_TipoAppuntamento:
                        this.ucPictureSelectIcona7.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        this.ucpColore7.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
                        break;

                    case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                        this.ucPictureSelectIcona3.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        this.ucpColore3.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
                        break;

                    case Enums.EnumDataNames.T_TipoAgenda:
                        this.ucPictureSelectIcona4.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        break;

                    case Enums.EnumDataNames.T_Agende:
                        this.ucpColore6.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
                        this.LoadUltraGrid(ref this.UltraGrid6);
                        this.SetUltraToolBarManager(UltraToolbarsManager6, ref UltraGrid6, this.ViewModality != Enums.EnumModalityPopUp.mpModifica);
                        break;

                    case Enums.EnumDataNames.T_TipoAlertGenerico:
                        this.ucPictureSelectIcona8.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        break;

                    case Enums.EnumDataNames.T_Protocolli:
                        this.LoadUltraGrid(ref this.UltraGrid9);
                        this.SetUltraToolBarManager(UltraToolbarsManager9, ref UltraGrid9, this.ViewModality != Enums.EnumModalityPopUp.mpModifica);
                        break;

                    case Enums.EnumDataNames.T_SezioniFUT:
                        this.ucPictureSelectIcona11.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        this.ucpColore11.Value = MyStatics.GetColorFromString(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"].ToString());
                        break;

                    case Enums.EnumDataNames.T_CDSSStruttura:
                        this.xmlParametri12.Text = this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Parametri"].ToString();
                        _DataBinds.DataBindings.Add("Text", "Parametri", this.xmlParametri12);
                        break;

                    case Enums.EnumDataNames.T_CDSSPlugins:
                        this.ucPictureSelectIcona13.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        break;

                    case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                        this.ucPictureSelectIcona18.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"]);
                        break;

                    case Enums.EnumDataNames.T_FiltriSpeciali:
                        this.qbSql17.Initialize(MyStatics.Configurazione.ConnectionString);
                        this.qbSql17.SQLTextComplete = this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Sql"].ToString();
                        break;

                    case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                        this.xmlParametri19.Text = this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Parametri"].ToString();
                        _DataBinds.DataBindings.Add("Text", "Parametri", this.xmlParametri19);
                        break;

                    case Enums.EnumDataNames.T_AgendePeriodi:
                        this.LoadUltraTreeOrariLavoro(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["OrariLavoro"].ToString());
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private void SetBindingsAss()
        {

            string sSql = @"";

            try
            {

                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                        this.ucMultiSelectUA3.ViewShowAll = true;
                        this.ucMultiSelectUA3.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice3.Text, UnicodeSrl.Scci.Enums.EnumEntita.WKI.ToString());
                        this.ucMultiSelectUA3.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice3.Text, UnicodeSrl.Scci.Enums.EnumEntita.WKI.ToString());
                        this.ucMultiSelectUA3.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA3.ViewInit();
                        this.ucMultiSelectUA3.RefreshData();

                        this.ucMultiSelectProtocolli3.ViewShowAll = true;
                        this.ucMultiSelectProtocolli3.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Protocolli] From T_Protocolli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodProtocollo From T_AssTipoTaskInfermieristicoProtocolli Where CodTipoTaskInfermieristico = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice3.Text);
                        this.ucMultiSelectProtocolli3.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Protocolli] From T_Protocolli" + Environment.NewLine +
                                                "Where Codice {0} (Select CodProtocollo From T_AssTipoTaskInfermieristicoProtocolli Where CodTipoTaskInfermieristico = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice3.Text);
                        this.ucMultiSelectProtocolli3.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectProtocolli3.ViewInit();
                        this.ucMultiSelectProtocolli3.RefreshData();

                        this.ucMultiSelectTempi3.ViewShowAll = true;
                        this.ucMultiSelectTempi3.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Tempi] From T_TipoTaskInfermieristicoTempi" + Environment.NewLine +
                                                "Where Codice {0} (Select CodTipoTaskInfermieristicoTempi From T_AssTipoTaskInfermieristicoTempi Where CodTipoTaskInfermieristico = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice3.Text);
                        this.ucMultiSelectTempi3.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Tempi] From T_TipoTaskInfermieristicoTempi" + Environment.NewLine +
                                                "Where Codice {0} (Select CodTipoTaskInfermieristicoTempi From T_AssTipoTaskInfermieristicoTempi Where CodTipoTaskInfermieristico = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice3.Text);
                        this.ucMultiSelectTempi3.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectTempi3.ViewInit();
                        this.ucMultiSelectTempi3.RefreshData();

                        this.ucMultiSelectPlusRuoli3.ViewShowAll = true;

                        this.ucMultiSelectPlusRuoli3.ViewShowFind = true; this.ucMultiSelectPlusRuoli3.GridDXFilterColumnIndex = 2;
                        this.ucMultiSelectPlusRuoli3.GridSXFilterColumnIndex = 2;

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
                                                "Order By QAE.CodAzione, R.Descrizione ASC", "WKI", this.uteCodice3.Text);
                        this.ucMultiSelectPlusRuoli3.ViewDataSetSX = DataBase.GetDataSet(sSql);
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
                                                "Order By ASS.CodAzione, R.Descrizione ASC", "WKI", this.uteCodice3.Text);
                        this.ucMultiSelectPlusRuoli3.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                                "From T_AzioniEntita AE" + Environment.NewLine +
                                                        "Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                                        "Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                                "Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                                        "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                        "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", "WKI");
                        this.ucMultiSelectPlusRuoli3.ViewDataSetMaster = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectPlusRuoli3.ViewInit();
                        this.ucMultiSelectPlusRuoli3.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_TipoAppuntamento:
                        this.ucMultiSelectUA7.ViewShowAll = true;
                        this.ucMultiSelectUA7.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice7.Text, UnicodeSrl.Scci.Enums.EnumEntita.APP.ToString());
                        this.ucMultiSelectUA7.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice7.Text, UnicodeSrl.Scci.Enums.EnumEntita.APP.ToString());
                        this.ucMultiSelectUA7.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA7.ViewInit();
                        this.ucMultiSelectUA7.RefreshData();

                        this.ucMultiSelectAgente7.ViewShowAll = true;
                        this.ucMultiSelectAgente7.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As Agende, CAST(0 As bit) As EscludiSovrapposizioni From T_Agende" + Environment.NewLine +
                                                "Where Codice {0} (Select CodAgenda From T_AssAgendeTipoAppuntamenti Where CodTipoApp = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice7.Text);
                        this.ucMultiSelectAgente7.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select ATT.CodAgenda As Codice, A.Descrizione As Agende, ATT.EscludiSovrapposizioni From T_AssAgendeTipoAppuntamenti ATT Inner Join T_Agende A On ATT.CodAgenda = A.Codice " + Environment.NewLine +
"Where ATT.CodTipoApp = '{0}'  ORDER BY A.Descrizione ASC", this.uteCodice7.Text);
                        DataSet ds = DataBase.GetDataSet(sSql).Copy();
                        ds.Tables[0].Columns["EscludiSovrapposizioni"].ReadOnly = false;
                        this.ucMultiSelectAgente7.ViewDataSetDX = ds;
                        this.ucMultiSelectAgente7.ViewInit();
                        this.ucMultiSelectAgente7.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_Festivita:

                        this.ucMultiSelectAgente20.ViewShowAll = true;
                        this.ucMultiSelectAgente20.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As Agende From T_Agende" + Environment.NewLine +
                                                "Where Codice {0} (Select CodAgenda From T_FestivitaAgende Where Data = {1})  ORDER BY Descrizione ASC", "Not In", DataBase.SQLDate(this.udteData20.DateTime));
                        this.ucMultiSelectAgente20.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione + ' (' + Codice + ')' As Agende From T_Agende" + Environment.NewLine +
                                                "Where Codice {0} (Select CodAgenda From T_FestivitaAgende Where Data = {1})  ORDER BY Descrizione ASC", "In", DataBase.SQLDate(this.udteData20.DateTime));
                        this.ucMultiSelectAgente20.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectAgente20.ViewInit();
                        this.ucMultiSelectAgente20.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_Agende:
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

                        this.ucMultiSelectTipoApp6.ViewShowAll = true;
                        this.ucMultiSelectTipoApp6.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Tipo Appuntamento], CAST(0 As bit) As EscludiSovrapposizioni From T_TipoAppuntamento" + Environment.NewLine +
                                                "Where Codice {0} (Select CodTipoApp From T_AssAgendeTipoAppuntamenti Where CodAgenda = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice6.Text);
                        this.ucMultiSelectTipoApp6.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select ATT.CodTipoApp As Codice, T.Descrizione AS [Tipo Appuntamento], ATT.EscludiSovrapposizioni From T_AssAgendeTipoAppuntamenti ATT Inner Join T_TipoAppuntamento T On ATT.CodTipoApp = T.Codice " + Environment.NewLine +
"Where ATT.CodAgenda = '{0}'  ORDER BY T.Descrizione ASC", this.uteCodice6.Text);
                        DataSet ds6 = DataBase.GetDataSet(sSql).Copy();
                        ds6.Tables[0].Columns["EscludiSovrapposizioni"].ReadOnly = false;
                        this.ucMultiSelectTipoApp6.ViewDataSetDX = ds6;
                        this.ucMultiSelectTipoApp6.ViewInit();
                        this.ucMultiSelectTipoApp6.RefreshData();

                        LoadMultiSelCampi();


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
                                                "Order By QAE.CodAzione, R.Descrizione ASC", EnumEntita.AGE.ToString(), this.uteCodice6.Text);
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
                                                "Order By ASS.CodAzione, R.Descrizione ASC", EnumEntita.AGE.ToString(), this.uteCodice6.Text);
                        this.ucMultiSelectPlusRuoli6.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                                "From T_AzioniEntita AE" + Environment.NewLine +
                                                        "Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                                        "Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                                "Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                                        "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                        "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", EnumEntita.AGE.ToString());
                        this.ucMultiSelectPlusRuoli6.ViewDataSetMaster = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectPlusRuoli6.ViewInit();
                        this.ucMultiSelectPlusRuoli6.RefreshData();


                        break;

                    case Enums.EnumDataNames.T_TipoAlertGenerico:
                        this.ucMultiSelectUA8.ViewShowAll = true;
                        this.ucMultiSelectUA8.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice8.Text, UnicodeSrl.Scci.Enums.EnumEntita.ALG.ToString());
                        this.ucMultiSelectUA8.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice8.Text, UnicodeSrl.Scci.Enums.EnumEntita.ALG.ToString());
                        this.ucMultiSelectUA8.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA8.ViewInit();
                        this.ucMultiSelectUA8.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_Protocolli:
                        this.ucMultiSelectTP9.ViewShowAll = true;
                        this.ucMultiSelectTP9.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Tipi Prescrizioni] From T_TipoPrescrizione" + Environment.NewLine +
                                                "Where Codice {0} (Select CodTipoPrescrizione From T_AssTipoPrescrizioneProtocolli Where CodProtocollo = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice9.Text);
                        this.ucMultiSelectTP9.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Tipi Prescrizioni] From T_TipoPrescrizione" + Environment.NewLine +
                                                "Where Codice {0} (Select CodTipoPrescrizione From T_AssTipoPrescrizioneProtocolli Where CodProtocollo = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice9.Text);
                        this.ucMultiSelectTP9.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectTP9.ViewInit();
                        this.ucMultiSelectTP9.RefreshData();
                        this.ucMultiSelectTT9.ViewShowAll = true;
                        this.ucMultiSelectTT9.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Tipi Task Infermieristici] From T_TipoTaskInfermieristico" + Environment.NewLine +
                                                "Where Codice {0} (Select CodTipoTaskInfermieristico From T_AssTipoTaskInfermieristicoProtocolli Where CodProtocollo = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice9.Text);
                        this.ucMultiSelectTT9.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Tipi Task Infermieristici] From T_TipoTaskInfermieristico" + Environment.NewLine +
                                                "Where Codice {0} (Select CodTipoTaskInfermieristico From T_AssTipoTaskInfermieristicoProtocolli Where CodProtocollo = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice9.Text);
                        this.ucMultiSelectTT9.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectTT9.ViewInit();
                        this.ucMultiSelectTT9.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_EBM:
                        this.ucMultiSelectUA14.ViewShowAll = true;
                        this.ucMultiSelectUA14.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice14.Text, UnicodeSrl.Scci.Enums.EnumEntita.EBM.ToString());
                        this.ucMultiSelectUA14.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice14.Text, UnicodeSrl.Scci.Enums.EnumEntita.EBM.ToString());
                        this.ucMultiSelectUA14.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA14.ViewInit();
                        this.ucMultiSelectUA14.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_Integra_Destinatari:
                        this.ucMultiSelectCampi15.ViewShowAll = true;
                        this.ucMultiSelectCampi15.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Campo As [Campi] From T_Integra_Campi" + Environment.NewLine +
                                                "Where Codice {0} (Select CodCampo From T_Integra_AssCampiDestinatari Where CodDestinatario = '{1}')  ORDER BY Campo ASC", "Not In", this.uteCodice15.Text);
                        this.ucMultiSelectCampi15.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Campo As [Campo] From T_Integra_Campi" + Environment.NewLine +
                                                "Where Codice {0} (Select CodCampo From T_Integra_AssCampiDestinatari Where CodDestinatario = '{1}')  ORDER BY Campo ASC", "In", this.uteCodice15.Text);
                        this.ucMultiSelectCampi15.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectCampi15.ViewInit();
                        this.ucMultiSelectCampi15.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_Integra_Campi:
                        this.ucMultiSelectDestinatari16.ViewShowAll = true;
                        this.ucMultiSelectDestinatari16.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Destinatari] From T_Integra_Destinatari" + Environment.NewLine +
                                                "Where Codice {0} (Select CodDestinatario From T_Integra_AssCampiDestinatari Where CodCampo = '{1}')  ORDER BY Descrizione ASC", "Not In", this.uteCodice16.Text);
                        this.ucMultiSelectDestinatari16.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Destinatari] From T_Integra_Destinatari" + Environment.NewLine +
                                                "Where Codice {0} (Select CodDestinatario From T_Integra_AssCampiDestinatari Where CodCampo = '{1}')  ORDER BY Descrizione ASC", "In", this.uteCodice16.Text);
                        this.ucMultiSelectDestinatari16.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectDestinatari16.ViewInit();
                        this.ucMultiSelectDestinatari16.RefreshData();
                        break;

                    case Enums.EnumDataNames.T_FiltriSpeciali:
                        this.ucMultiSelectUA17.ViewShowAll = true;
                        this.ucMultiSelectUA17.ViewShowFind = true;
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Codice ASC", "Not In", this.uteCodice17.Text, UnicodeSrl.Scci.Enums.EnumEntita.FLS.ToString());
                        this.ucMultiSelectUA17.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select Codice, Descrizione As [Unità Atomiche] From T_UnitaAtomiche" + Environment.NewLine +
                                                "Where Codice {0} (Select CodUA From T_AssUAEntita Where CodVoce = '{1}' And CodEntita = '{2}')  ORDER BY Descrizione ASC", "In", this.uteCodice17.Text, UnicodeSrl.Scci.Enums.EnumEntita.FLS.ToString());
                        this.ucMultiSelectUA17.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectUA17.ViewInit();
                        this.ucMultiSelectUA17.RefreshData();


                        this.ucMultiSelectPlusRuoli17.ViewShowAll = true;

                        this.ucMultiSelectPlusRuoli17.ViewShowFind = true; this.ucMultiSelectPlusRuoli17.GridDXFilterColumnIndex = 2;
                        this.ucMultiSelectPlusRuoli17.GridSXFilterColumnIndex = 2;

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
                                                "Order By QAE.CodAzione, R.Descrizione ASC", "FLS", this.uteCodice17.Text);
                        this.ucMultiSelectPlusRuoli17.ViewDataSetSX = DataBase.GetDataSet(sSql);
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
                                                "Order By ASS.CodAzione, R.Descrizione ASC", "FLS", this.uteCodice17.Text);
                        this.ucMultiSelectPlusRuoli17.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                                "From T_AzioniEntita AE" + Environment.NewLine +
                                                        "Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                                        "Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                                "Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                                        "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                        "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", "FLS");
                        this.ucMultiSelectPlusRuoli17.ViewDataSetMaster = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectPlusRuoli17.ViewInit();
                        this.ucMultiSelectPlusRuoli17.RefreshData();

                        break;

                    case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                        this.ucMultiSelectPlusRuoli18.ViewShowAll = true;

                        this.ucMultiSelectPlusRuoli18.ViewShowFind = true; this.ucMultiSelectPlusRuoli18.GridDXFilterColumnIndex = 2;
                        this.ucMultiSelectPlusRuoli18.GridSXFilterColumnIndex = 2;

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
                                                "Order By QAE.CodAzione, R.Descrizione ASC", "ALA", this.uteCodice18.Text);
                        this.ucMultiSelectPlusRuoli18.ViewDataSetSX = DataBase.GetDataSet(sSql);
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
                                                "Order By ASS.CodAzione, R.Descrizione ASC", "ALA", this.uteCodice18.Text);
                        this.ucMultiSelectPlusRuoli18.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        sSql = string.Format("Select AE.CodAzione, A.Descrizione As Azione" + Environment.NewLine +
                                                "From T_AzioniEntita AE" + Environment.NewLine +
                                                        "Inner Join T_Entita E On AE.CodEntita = E.Codice" + Environment.NewLine +
                                                        "Inner Join T_Azioni A On AE.CodAzione = A.Codice" + Environment.NewLine +
                                                "Where AE.CodEntita = '{0}'" + Environment.NewLine +
                                                        "And IsNull(E.AbilitaPermessiDettaglio, 0) <> 0" + Environment.NewLine +
                                                        "And IsNull(AE.AbilitaPermessiDettaglio, 0) <> 0", "ALA");
                        this.ucMultiSelectPlusRuoli18.ViewDataSetMaster = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectPlusRuoli18.ViewInit();
                        this.ucMultiSelectPlusRuoli18.RefreshData();
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

                    case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
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

        private void UpdateBindings()
        {

            try
            {

                switch (this.ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_TipoAppuntamento:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona7.ViewImage);
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore7.Value.ToString();
                        break;

                    case Enums.EnumDataNames.T_DCDecodificheValori:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona2.ViewImage);
                        break;

                    case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona3.ViewImage);
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore3.Value.ToString();
                        break;

                    case Enums.EnumDataNames.T_TipoAgenda:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona4.ViewImage);
                        break;

                    case Enums.EnumDataNames.T_Agende:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore6.Value.ToString();
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["ElencoCampi"] = this.getElencoCampiXml();
                        SaveUltraGridOrari();
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["OrariLavoro"] = UnicodeSrl.Scci.Statics.XmlProcs.XmlSerializeToString(mo_WorkingHourTime); SaveParametriLista();
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["ParametriLista"] = UnicodeSrl.Scci.Statics.XmlProcs.XmlSerializeToString(mo_ParametriListaAgenda);
                        this.SaveUltraGridMassimali();
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Risorse"] = UnicodeSrl.Scci.Statics.XmlProcs.XmlSerializeToString(mo_MassimaliAgenda);
                        break;

                    case Enums.EnumDataNames.T_TipoAlertGenerico:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona8.ViewImage);
                        break;

                    case Enums.EnumDataNames.T_SezioniFUT:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona11.ViewImage);
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Colore"] = this.ucpColore11.Value.ToString();
                        break;

                    case Enums.EnumDataNames.T_CDSSPlugins:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona13.ViewImage);
                        break;

                    case Enums.EnumDataNames.T_CDSSStruttura:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Parametri"] = this.xmlParametri12.Text;
                        break;

                    case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Icona"] = Scci.Statics.DrawingProcs.GetByteFromImage(this.ucPictureSelectIcona18.ViewImage);
                        break;

                    case Enums.EnumDataNames.T_FiltriSpeciali:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Sql"] = this.qbSql17.SQLTextComplete;
                        if (this.uteCodTipoFiltroSpeciale17.Text.Trim() == "")
                            this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodTipoFiltroSpeciale"] = System.DBNull.Value;
                        break;

                    case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Parametri"] = this.xmlParametri19.Text;
                        break;

                    case Enums.EnumDataNames.T_AgendePeriodi:
                        this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["OrariLavoro"] = this.SaveUltraTreeOrariLavoro();
                        break;

                    case Enums.EnumDataNames.T_AssUAIntestazioni:
                        if (this.udteDataFine22.Value != null)
                        {
                            this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["DataFine"] = (DateTime)this.udteDataFine22.Value;
                        }
                        else
                        {
                            this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["DataFine"] = DBNull.Value;
                        }
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void UpdateBindingsAss()
        {

            string sSql = @"";
            string sSqlUpd = @"";

            try
            {
                switch (this.ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                        if (this.ucMultiSelectUA3.ViewDataSetSX.HasChanges() == true)
                        {

                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                "Where CodVoce = '" + this.uteCodice3.Text + "'" + Environment.NewLine +
                                "And CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.WKI.ToString() + "'" + Environment.NewLine +
                                "And CodUA = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA3.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA3.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice3.Text + "', '" + UnicodeSrl.Scci.Enums.EnumEntita.WKI.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA3.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }

                        if (this.ucMultiSelectProtocolli3.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete From T_AssTipoTaskInfermieristicoProtocolli" + Environment.NewLine +
                                    "Where CodTipoTaskInfermieristico = '" + this.uteCodice3.Text + "'" + Environment.NewLine +
                                    "And CodProtocollo = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectProtocolli3.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectProtocolli3.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssTipoTaskInfermieristicoProtocolli (CodTipoTaskInfermieristico, CodProtocollo)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice3.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectProtocolli3.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }

                        if (this.ucMultiSelectTempi3.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete From T_AssTipoTaskInfermieristicoTempi" + Environment.NewLine +
                                    "Where CodTipoTaskInfermieristico = '" + this.uteCodice3.Text + "'" + Environment.NewLine +
                                    "And CodTipoTaskInfermieristicoTempi = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTempi3.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectTempi3.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssTipoTaskInfermieristicoTempi (CodTipoTaskInfermieristico, CodTipoTaskInfermieristicoTempi)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice3.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTempi3.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }

                        if (this.ucMultiSelectPlusRuoli3.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                                    "Where CodVoce = '" + DataBase.Ax2(this.uteCodice3.Text) + "'" + Environment.NewLine +
                                            "And CodEntita = '" + Scci.Enums.EnumEntita.WKI.ToString() + "'" + Environment.NewLine +
                                            "And CodRuolo = '{0}'" + Environment.NewLine +
                                            "And CodAzione = '{1}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli3.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        }
                        if (this.ucMultiSelectPlusRuoli3.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                                   "Values ('{0}', '" + Scci.Enums.EnumEntita.WKI.ToString() + "', '" + DataBase.Ax2(this.uteCodice3.Text) + @"', '{1}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli3.ViewDataSetDX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_TipoAppuntamento:
                        if (this.ucMultiSelectUA7.ViewDataSetSX.HasChanges() == true)
                        {

                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                "Where CodVoce = '" + this.uteCodice7.Text + "'" + Environment.NewLine +
                                "And CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.APP.ToString() + "'" + Environment.NewLine +
                                "And CodUA = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA7.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA7.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice7.Text + "', '" + UnicodeSrl.Scci.Enums.EnumEntita.APP.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA7.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }

                        if (this.ucMultiSelectAgente7.ViewDataSetSX.HasChanges() == true)
                        {

                            sSql = "Delete from T_AssAgendeTipoAppuntamenti" + Environment.NewLine +
                                "Where CodTipoApp = '" + this.uteCodice7.Text + "'" + Environment.NewLine +
                                "And CodAgenda = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectAgente7.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectAgente7.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssAgendeTipoAppuntamenti (CodTipoApp, CodAgenda, EscludiSovrapposizioni)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice7.Text + "', '{0}', {1})";
                            sSqlUpd = "Update T_AssAgendeTipoAppuntamenti set EscludiSovrapposizioni = {1} " + Environment.NewLine +
                                      "Where CodTipoApp = '" + this.uteCodice7.Text + "' And CodAgenda = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectAgente7.ViewDataSetDX.GetChanges(), "Codice", "EscludiSovrapposizioni", sSql, sSqlUpd);
                        }
                        break;

                    case Enums.EnumDataNames.T_Festivita:
                        if (this.ucMultiSelectAgente20.ViewDataSetSX.HasChanges() == true)
                        {

                            sSql = "Delete from T_FestivitaAgende" + Environment.NewLine +
                                "Where Data = " + DataBase.SQLDate(this.udteData20.DateTime) + Environment.NewLine +
                                "And CodAgenda = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectAgente20.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectAgente20.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_FestivitaAgende (Data, CodAgenda)" + Environment.NewLine +
                                    "Values (" + DataBase.SQLDate(this.udteData20.DateTime) + ", '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectAgente20.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_Agende:
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


                        if (this.ucMultiSelectTipoApp6.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssAgendeTipoAppuntamenti" + Environment.NewLine +
                                "Where CodAgenda = '" + this.uteCodice6.Text + "'" + Environment.NewLine +
                                "And CodTipoApp = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTipoApp6.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectTipoApp6.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssAgendeTipoAppuntamenti (CodAgenda, CodTipoApp, EscludiSovrapposizioni)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice6.Text + "', '{0}', {1})";
                            sSqlUpd = "Update T_AssAgendeTipoAppuntamenti set EscludiSovrapposizioni = {1} " + Environment.NewLine +
                                    "Where CodAgenda = '" + this.uteCodice6.Text + "' And CodTipoApp = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTipoApp6.ViewDataSetDX.GetChanges(), "Codice", "EscludiSovrapposizioni", sSql, sSqlUpd);
                        }


                        if (this.ucMultiSelectPlusRuoli6.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                                    "Where CodVoce = '" + DataBase.Ax2(this.uteCodice6.Text) + "'" + Environment.NewLine +
                                            "And CodEntita = '" + Scci.Enums.EnumEntita.AGE.ToString() + "'" + Environment.NewLine +
                                            "And CodRuolo = '{0}'" + Environment.NewLine +
                                            "And CodAzione = '{1}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli6.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        }
                        if (this.ucMultiSelectPlusRuoli6.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                                   "Values ('{0}', '" + Scci.Enums.EnumEntita.AGE.ToString() + "', '" + DataBase.Ax2(this.uteCodice6.Text) + @"', '{1}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli6.ViewDataSetDX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                        }

                        break;

                    case Enums.EnumDataNames.T_TipoAlertGenerico:
                        if (this.ucMultiSelectUA8.ViewDataSetSX.HasChanges() == true)
                        {

                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                "Where CodVoce = '" + this.uteCodice8.Text + "'" + Environment.NewLine +
                                "And CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.ALG.ToString() + "'" + Environment.NewLine +
                                "And CodUA = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA8.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA8.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice8.Text + "', '" + UnicodeSrl.Scci.Enums.EnumEntita.ALG.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA8.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_Protocolli:
                        if (this.ucMultiSelectTP9.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssTipoPrescrizioneProtocolli" + Environment.NewLine +
                                    "Where CodProtocollo = '" + this.uteCodice9.Text + "'" + Environment.NewLine +
                                            "And CodTipoPrescrizione = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTP9.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectTP9.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssTipoPrescrizioneProtocolli (CodProtocollo, CodTipoPrescrizione)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice9.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTP9.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectTT9.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssTipoTaskInfermieristicoProtocolli" + Environment.NewLine +
                                    "Where CodProtocollo = '" + this.uteCodice9.Text + "'" + Environment.NewLine +
                                            "And CodTipoTaskInfermieristico = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTT9.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectTT9.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssTipoTaskInfermieristicoProtocolli (CodProtocollo, CodTipoTaskInfermieristico)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice9.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectTT9.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_EBM:
                        if (this.ucMultiSelectUA14.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                "Where CodVoce = '" + this.uteCodice14.Text + "'" + Environment.NewLine +
                                "And CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.EBM.ToString() + "'" + Environment.NewLine +
                                "And CodUA = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA14.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectUA14.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice14.Text + "', '" + UnicodeSrl.Scci.Enums.EnumEntita.EBM.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA14.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_Integra_Destinatari:
                        if (this.ucMultiSelectCampi15.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_Integra_AssCampiDestinatari" + Environment.NewLine +
                                    "Where CodDestinatario = '" + this.uteCodice15.Text + "'" + Environment.NewLine +
                                            "And CodCampo = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectCampi15.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectCampi15.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_Integra_AssCampiDestinatari (CodCampo, CodDestinatario)" + Environment.NewLine +
                                    "Values ('{0}', '" + this.uteCodice15.Text + "')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectCampi15.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_Integra_Campi:
                        if (this.ucMultiSelectDestinatari16.ViewDataSetSX.HasChanges() == true)
                        {
                            sSql = "Delete from T_Integra_AssCampiDestinatari" + Environment.NewLine +
                                    "Where CodCampo  = '" + this.uteCodice16.Text + "'" + Environment.NewLine +
                                            "And CodDestinatario = '{0}'";
                            UpdateBindingsAssDataSet(this.ucMultiSelectDestinatari16.ViewDataSetSX.GetChanges(), "Codice", sSql);
                        }
                        if (this.ucMultiSelectDestinatari16.ViewDataSetDX.HasChanges() == true)
                        {
                            sSql = "Insert Into T_Integra_AssCampiDestinatari (CodCampo, CodDestinatario)" + Environment.NewLine +
                                    "Values ('" + this.uteCodice16.Text + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectDestinatari16.ViewDataSetDX.GetChanges(), "Codice", sSql);
                        }
                        break;

                    case Enums.EnumDataNames.T_FiltriSpeciali:
                        if (this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                        {
                            sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
        "Values ('" + this.uteCodice17.Text + "', '" + this.ucMultiSelectUA17.Tag.ToString() + "', '{0}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectUA17.ViewDataSetDX, "Codice", sSql);

                            sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
       "Values ('{0}', '" + Scci.Enums.EnumEntita.FLS.ToString() + "', '" + DataBase.Ax2(this.uteCodice17.Text) + @"', '{1}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli17.ViewDataSetDX, "CodRuolo", "CodAzione", sSql);
                        }
                        else
                        {
                            if (this.ucMultiSelectUA17.ViewDataSetSX.HasChanges() == true)
                            {
                                sSql = "Delete from T_AssUAEntita" + Environment.NewLine +
                                        "Where CodVoce = '" + this.uteCodice17.Text + "'" + Environment.NewLine +
                                                "And CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.FLS.ToString() + "'" + Environment.NewLine +
                                                "And CodUA = '{0}'";
                                UpdateBindingsAssDataSet(this.ucMultiSelectUA17.ViewDataSetSX.GetChanges(), "Codice", sSql);
                            }
                            if (this.ucMultiSelectUA17.ViewDataSetDX.HasChanges() == true)
                            {
                                sSql = "Insert Into T_AssUAEntita (CodVoce, CodEntita, CodUA)" + Environment.NewLine +
                                        "Values ('" + this.uteCodice17.Text + "', '" + UnicodeSrl.Scci.Enums.EnumEntita.FLS.ToString() + "', '{0}')";
                                UpdateBindingsAssDataSet(this.ucMultiSelectUA17.ViewDataSetDX.GetChanges(), "Codice", sSql);
                            }



                            if (this.ucMultiSelectPlusRuoli17.ViewDataSetSX.HasChanges() == true)
                            {
                                sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                                        "Where CodVoce = '" + DataBase.Ax2(this.uteCodice17.Text) + "'" + Environment.NewLine +
                                                "And CodEntita = '" + Scci.Enums.EnumEntita.FLS.ToString() + "'" + Environment.NewLine +
                                                "And CodRuolo = '{0}'" + Environment.NewLine +
                                                "And CodAzione = '{1}'";
                                UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli17.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                            }
                            if (this.ucMultiSelectPlusRuoli17.ViewDataSetDX.HasChanges() == true)
                            {
                                sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                                       "Values ('{0}', '" + Scci.Enums.EnumEntita.FLS.ToString() + "', '" + DataBase.Ax2(this.uteCodice17.Text) + @"', '{1}')";
                                UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli17.ViewDataSetDX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                            }
                        }

                        break;

                    case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                        if (this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                        {
                            sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
       "Values ('{0}', '" + Scci.Enums.EnumEntita.ALA.ToString() + "', '" + DataBase.Ax2(this.uteCodice18.Text) + @"', '{1}')";
                            UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli18.ViewDataSetDX, "CodRuolo", "CodAzione", sSql);
                        }
                        else
                        {
                            if (this.ucMultiSelectPlusRuoli18.ViewDataSetSX.HasChanges() == true)
                            {
                                sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
                                        "Where CodVoce = '" + DataBase.Ax2(this.uteCodice18.Text) + "'" + Environment.NewLine +
                                                "And CodEntita = '" + Scci.Enums.EnumEntita.ALA.ToString() + "'" + Environment.NewLine +
                                                "And CodRuolo = '{0}'" + Environment.NewLine +
                                                "And CodAzione = '{1}'";
                                UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli18.ViewDataSetSX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                            }
                            if (this.ucMultiSelectPlusRuoli18.ViewDataSetDX.HasChanges() == true)
                            {
                                sSql = "Insert Into T_AssRuoliAzioni (CodRuolo, CodEntita, CodVoce, CodAzione)" + Environment.NewLine +
                                       "Values ('{0}', '" + Scci.Enums.EnumEntita.ALA.ToString() + "', '" + DataBase.Ax2(this.uteCodice18.Text) + @"', '{1}')";
                                UpdateBindingsAssDataSet(this.ucMultiSelectPlusRuoli18.ViewDataSetDX.GetChanges(), "CodRuolo", "CodAzione", sSql);
                            }
                        }

                        break;

                    default:
                        break;

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

                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_TipoAppuntamento:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice7.Text + "' And CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.APP.ToString() + "'";
                        DataBase.ExecuteSql(sSql);

                        sSql = "Delete from T_AssAgendeTipoAppuntamenti Where CodTipoApp = '" + this.uteCodice7.Text + "'";
                        DataBase.ExecuteSql(sSql);

                        sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice3.Text) + "'" + Environment.NewLine +
                    "And CodEntita = '" + Scci.Enums.EnumEntita.WKI.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_DCDecodifiche:
                        sSql = "Delete from T_DCDecodificheValori Where CodDec = '" + this.uteCodice1.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice3.Text + "' And CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.WKI.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_Agende:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice6.Text + "' And CodEntita = '" + this.ucMultiSelectUA6.Tag.ToString() + "'";
                        DataBase.ExecuteSql(sSql);

                        sSql = "Delete from T_AssRuoliAzioni Where CodEntita = 'AGE' And CodVoce = '" + this.uteCodice6.Text + "'";
                        DataBase.ExecuteSql(sSql);

                        sSql = "Delete from T_AssAgendeTipoAppuntamenti Where CodAgenda = '" + this.uteCodice6.Text + "'";
                        DataBase.ExecuteSql(sSql);

                        sSql = "Delete from T_AgendePeriodi Where CodAgenda = '" + this.uteCodice6.Text + "'";
                        DataBase.ExecuteSql(sSql);

                        break;

                    case Enums.EnumDataNames.T_TipoAlertGenerico:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice8.Text + "' And CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.ALG.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_Protocolli:
                        sSql = "Delete from T_AssTipoPrescrizioneProtocolli Where CodProtocollo = '" + this.uteCodice9.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        sSql = "Delete from T_ProtocolliTempi Where CodProtocollo = '" + this.uteCodice9.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_EBM:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice14.Text + "' And CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.EBM.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_Integra_Destinatari:
                        sSql = "Delete from T_Integra_AssCampiDestinatari Where CodDestinatario = '" + this.uteCodice15.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_Integra_Campi:
                        sSql = "Delete from T_Integra_AssCampiDestinatari Where CodCampo = '" + this.uteCodice16.Text + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_FiltriSpeciali:
                        sSql = "Delete from T_AssUAEntita Where CodVoce = '" + this.uteCodice17.Text + "' And CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.FLS.ToString() + "'";
                        DataBase.ExecuteSql(sSql);

                        sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice17.Text) + "'" + Environment.NewLine +
                    "And CodEntita = '" + Scci.Enums.EnumEntita.FLS.ToString() + "'";
                        DataBase.ExecuteSql(sSql);
                        break;

                    case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                        sSql = "Delete from T_AssRuoliAzioni" + Environment.NewLine +
            "Where CodVoce = '" + DataBase.Ax2(this.uteCodice18.Text) + "'" + Environment.NewLine +
                    "And CodEntita = '" + Scci.Enums.EnumEntita.ALA.ToString() + "'";
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
        private void UpdateBindingsAssDataSet(DataSet oDs, string field1, string field2, string sqlInsert, string sqlUpdate)
        {

            try
            {

                foreach (DataRow oRow in oDs.Tables[0].Rows)
                {
                    var val1 = oRow[field1];
                    var val2 = oRow[field2];

                    if (oRow.Table.Columns[field1].DataType.Name.ToUpper().Contains("BOOL"))
                        val1 = ((bool)oRow[field1] ? "1" : "0");

                    if (oRow.Table.Columns[field2].DataType.Name.ToUpper().Contains("BOOL"))
                        val2 = ((bool)oRow[field2] ? "1" : "0");

                    if (oRow.RowState == DataRowState.Added)
                    {
                        DataBase.ExecuteSql(string.Format(sqlInsert, val1, val2));
                    }
                    if (oRow.RowState == DataRowState.Modified)
                    {
                        DataBase.ExecuteSql(string.Format(sqlUpdate, val1, val2));
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

            switch (this.ViewDataNamePU)
            {

                case Enums.EnumDataNames.T_DCDecodifiche:
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

                case Enums.EnumDataNames.T_DCDecodificheValori:
                    if (bRet && this.uteCodice2.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice2.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice2.Focus();
                        bRet = false;
                    }
                    if (bRet && !Char.IsLetter(this.uteCodice2.Text[0]) && this.ucPictureSelectIcona2.ViewImage != null)
                    {
                        MessageBox.Show(@"Per valori di dizionari con immagini è necessario inserire una lettera nel codice !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice2.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione2.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione2.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione2.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_TipoAppuntamento:
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
                    if (bRet && this.umeTimeSlotInterval7.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblTimeSlotInterval7.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.umeTimeSlotInterval7.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uteCodScheda7.Text.Trim() == "" || this.lblCodSchedaDes7.Text.Trim() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodScheda7.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodScheda7.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_TipoTaskInfermieristico:
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
                    if (bRet && (this.uteCodScheda3.Text.Trim() == "" || this.lblCodSchedaDes3.Text.Trim() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodScheda3.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodScheda3.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_TipoAgenda:
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
                    break;

                case Enums.EnumDataNames.T_TipoAlertGenerico:
                    if (bRet && this.uteCodice8.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice8.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice8.Focus();
                        bRet = false;
                    }
                    if (bRet && this.lblDescrizione8.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione8.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione8.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uteCodScheda8.Text.Trim() == "" || this.lblCodSchedaDes8.Text.Trim() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodScheda8.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodScheda8.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Agende:
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
                    if (bRet && this.uceIntervalloSlot6.SelectedIndex < 0)
                    {
                        MessageBox.Show(@"Inserire " + this.lblIntervalloSlot6.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uceIntervalloSlot6.Focus();
                        bRet = false;
                    }


                    if (bRet)
                    {
                        if ((this.umeMassimoRitardoPrenotazione6.Value != System.DBNull.Value && this.umeMassimoAnticipoPrenotazione6.Value != System.DBNull.Value)
                            && ((int)this.umeMassimoRitardoPrenotazione6.Value < (int)this.umeMassimoAnticipoPrenotazione6.Value))
                        {
                            MessageBox.Show(@"Inserire " + this.ugbVincoliPrenotazione6.Text + @" coerenti!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            this.umeMassimoRitardoPrenotazione6.Focus();
                            bRet = false;
                        }
                    }
                    if (bRet)
                    {
                        if (((this.umeMassimoRitardoPrenotazione6.Value != System.DBNull.Value && (int)this.umeMassimoRitardoPrenotazione6.Value > 0)
                                && (this.umeMassimoAnticipoPrenotazione6.Value == System.DBNull.Value || (int)this.umeMassimoAnticipoPrenotazione6.Value <= 0))
                            || ((this.umeMassimoRitardoPrenotazione6.Value == System.DBNull.Value || (int)this.umeMassimoRitardoPrenotazione6.Value <= 0)
                                && (this.umeMassimoAnticipoPrenotazione6.Value != System.DBNull.Value && (int)this.umeMassimoAnticipoPrenotazione6.Value > 0)))
                        {
                            MessageBox.Show(@"I vincoli di prenotazione non sono stati valorizzati entrambi:" + Environment.NewLine + @"NON verranno presi in considerazione.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.umeMassimoAnticipoPrenotazione6.Focus();
                        }
                    }
                    break;

                case Enums.EnumDataNames.T_AssUAUOLetti:
                    if (bRet && this.uteCodUA4.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodUA4.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodUA4.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodAzi4.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodAzi4.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodAzi4.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodUO4.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodUO4.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodUO4.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodSettore4.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodSettore4.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodSettore4.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodLetto4.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodLetto4.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodLetto4.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Protocolli:
                    if (bRet && this.uteCodice9.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice9.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice9.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione9.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione9.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione9.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uceCodTipoProtocollo9.Value == null || this.uceCodTipoProtocollo9.Value.ToString() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodTipoProtocollo9.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uceCodTipoProtocollo9.Focus();
                        bRet = false;
                    }

                    break;

                case Enums.EnumDataNames.T_ProtocolliTempi:
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
                    if (bRet && (this.uteCodProtocollo10.Text.Trim() == "" || this.lblCodProtocollo10Des.Text.Trim() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodProtocollo10.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodProtocollo10.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_SezioniFUT:
                    if (bRet && this.uteCodice11.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice11.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice11.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione11.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione11.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione11.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodEntita11.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodEntita11.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodEntita11.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uteCodEntita11.Text.Trim() != "" && this.lblCodEntitaDes11.Text.Trim() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodEntita11.Text + @" corretta!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodEntita11.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_CDSSStruttura:
                    if (bRet && this.uteCodUA12.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodUA12.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodUA12.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodAzione12.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodAzione12.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodAzione12.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodPlugin12.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodPlugin12.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodPlugin12.Focus();
                        bRet = false;
                    }
                    if (bRet && !this.xmlParametri12.XmlValidated)
                    {
                        MessageBox.Show(@"Parametri XML sintatticamente non corretti !" + Environment.NewLine + this.xmlParametri12.LastValidateError, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.xmlParametri12.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_CDSSPlugins:
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

                case Enums.EnumDataNames.T_EBM:
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
                    if (bRet && this.uteUrl14.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblUrl14.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteUrl14.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Integra_Destinatari:
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
                    break;

                case Enums.EnumDataNames.T_Integra_Campi:
                    if (bRet && this.uteCodice16.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice16.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice16.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodEntita16.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodEntita16.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodEntita16.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodTipoEntita16.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodTipoEntita16.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodTipoEntita16.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCampo16.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCampo16.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCampo16.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_FiltriSpeciali:
                    if (bRet && this.uteCodice17.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice17.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice17.Focus();
                        bRet = false;
                    }
                    if (bRet && this.lblDescrizione17.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione17.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione17.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uteCodTipoFiltroSpeciale17.Text.Trim() != "" && this.lblCodTipoFiltroSpecialeDes17.Text.Trim() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodTipoFiltroSpeciale17.Text + @" valido!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodTipoFiltroSpeciale17.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                    if (bRet && this.uteCodice18.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodice18.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodice18.Focus();
                        bRet = false;
                    }
                    if (bRet && this.lblDescrizione18.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione18.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione18.Focus();
                        bRet = false;
                    }
                    if (bRet && (this.uteCodScheda18.Text.Trim() == "" || this.lblCodSchedaDes18.Text.Trim() == ""))
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodScheda18.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodScheda18.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                    if (bRet && this.uteCodRuolo19.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodRuolo19.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodRuolo19.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodAzione19.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodAzione19.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodAzione19.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodPlugin19.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodPlugin19.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodPlugin19.Focus();
                        bRet = false;
                    }
                    if (bRet && !this.xmlParametri19.XmlValidated)
                    {
                        MessageBox.Show(@"Parametri XML sintatticamente non corretti !" + Environment.NewLine + this.xmlParametri19.LastValidateError, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.xmlParametri19.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_Festivita:
                    if (bRet && this.udteData20.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblData20.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteData20.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteDescrizione20.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDescrizione20.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteDescrizione20.Focus();
                        bRet = false;
                    }
                    break;

                case Enums.EnumDataNames.T_AssUAIntestazioni:
                    if (bRet && this.uteCodUA22.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodUA22.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodUA22.Focus();
                        bRet = false;
                    }
                    if (bRet && this.uteCodIntestazione22.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblCodIntestazione22.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteCodIntestazione22.Focus();
                        bRet = false;
                    }
                    if (bRet && this.udteDataInizio22.Text.Trim() == "")
                    {
                        MessageBox.Show(@"Inserire " + this.lblDataInizio22.Text + @"!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.udteDataInizio22.Focus();
                        bRet = false;
                    }
                    break;

                default:
                    break;

            }

            return bRet;

        }

        private string IndentXMLString(string xml)
        {

            String outXml = String.Empty;

            MemoryStream ms = new MemoryStream();

            XmlTextWriter xtw = new XmlTextWriter(ms, System.Text.Encoding.Unicode);
            XmlDocument doc = new XmlDocument();

            try
            {

                doc.LoadXml(xml);

                xtw.Formatting = Formatting.Indented;

                doc.WriteContentTo(xtw);
                xtw.Flush();

                ms.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(ms);
                return sr.ReadToEnd();

            }
            catch (Exception)
            {
                return String.Empty;
            }

        }

        private void DisposeUserControls(Control Container)
        {

            foreach (Control c in Container.Controls)
            {

                if (c.HasChildren) DisposeUserControls(c);

                if (c.GetType().ToString() == "UnicodeSrl.ScciCore.ucMultiSelect") c.Dispose();

            }

        }

        private int CalcolaMinuti()
        {

            int nret = 0;

            try
            {

                int nDay = (int)Enum.Parse(typeof(System.DayOfWeek), this.utvOrariLavoro21.ActiveNode.Key);
                string HourI = string.Format("{0:00}:{1:00}", ((DateTime)this.udteHourI21.Value).Hour, ((DateTime)this.udteHourI21.Value).Minute);
                string HourF = string.Format("{0:00}:{1:00}", ((DateTime)this.udteHourF21.Value).Hour, ((DateTime)this.udteHourF21.Value).Minute);

                WorkingHourTimeDaysOfWeek owht = new WorkingHourTimeDaysOfWeek(nDay, HourI, HourF);
                owht.CalcolaMinuti();
                nret = owht.WorkingMinutes;
                owht = null;

            }
            catch (Exception)
            {

            }

            return nret;

        }

        private int CheckVersioneCorrenteScheda(string codiceScheda)
        {
            return int.Parse(DataBase.FindValue("IsNull(Versione,0)", "T_SchedeVersioni", "CodScheda = '" + codiceScheda + "' And FlagAttiva = 1 Order By DtValI desc, DtValF", "0"));
        }

        #endregion

        #region Events

        private void ute_ValueChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void xml_XMLTextChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
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
                if (e.Layout.Bands[0].Columns.Exists("EscludiSovrapposizioni") == true) { e.Layout.Bands[0].Columns["EscludiSovrapposizioni"].Hidden = true; }

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

                if (e.Layout.Bands[0].Columns.Exists("EscludiSovrapposizioni") == true)
                {
                    e.Layout.Bands[0].Columns["EscludiSovrapposizioni"].Header.Caption = "Escl.Sovrap.";
                    e.Layout.Bands[0].Columns["EscludiSovrapposizioni"].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    e.Layout.Bands[0].Columns["EscludiSovrapposizioni"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    e.Layout.Bands[0].Columns["EscludiSovrapposizioni"].MinWidth = 60;
                    e.Layout.Bands[0].Columns["EscludiSovrapposizioni"].Width = 60;
                    e.Layout.Bands[0].Columns["EscludiSovrapposizioni"].MaxWidth = 60;

                    if (this._Modality == Enums.EnumModalityPopUp.mpModifica || this._Modality == Enums.EnumModalityPopUp.mpNuovo)
                    {
                        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
                        e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
                        if (e.Layout.Bands[0].Columns.Exists("Agende"))
                        {
                            e.Layout.Bands[0].Columns["Agende"].CellActivation = Activation.NoEdit;
                            e.Layout.Bands[0].Columns["Agende"].CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                        }
                        if (e.Layout.Bands[0].Columns.Exists("Tipo Appuntamento"))
                        {
                            e.Layout.Bands[0].Columns["Tipo Appuntamento"].CellActivation = Activation.NoEdit;
                            e.Layout.Bands[0].Columns["Tipo Appuntamento"].CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                        }

                        e.Layout.Bands[0].Columns["EscludiSovrapposizioni"].CellActivation = Activation.AllowEdit;
                        e.Layout.Bands[0].Columns["EscludiSovrapposizioni"].CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
                    }

                }

            }
            catch (Exception)
            {

            }

        }

        private void ucMultiSelect_GridDXInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {
                int imagecolindex = e.Row.Cells.Count - 1;
                if (e.Row.Cells.Exists("EscludiSovrapposizioni")) imagecolindex = 1;
                if (e.Row.Cells[imagecolindex].Hidden == false)
                {
                    e.Row.Cells[imagecolindex].Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SYMBOL_CHECK, Enums.EnumImageSize.isz16));
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraToolbarsManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.ActionGridToolClick(e.Tool, ref this.UltraGrid1);
        }

        private void ucpColore3_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColore3.Value = this.colorDialog.Color;
            }
        }

        private void uteCodScheda3_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                switch (e.Button.Key)
                {

                    case "Zoom":
                        frmZoom f = new frmZoom();
                        f.ViewText = this.lblCodScheda3.Text;
                        f.ViewIcon = this.ViewIcon;
                        f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Schede";
                        f.ViewSqlStruct.Where = "CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.WKI.ToString() + "' And Codice <> '" + DataBase.Ax2(this.uteCodScheda3.Text) + "'";
                        f.ViewInit();
                        f.ShowDialog();
                        if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            this.uteCodScheda3.Text = f.ViewActiveRow.Cells["Codice"].Text;
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
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodScheda3_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodSchedaDes3.Text = DataBase.FindValue("Descrizione", "T_Schede", string.Format("Codice = '{0}' AND CodEntita = '{1}'", new string[2] { DataBase.Ax2(this.uteCodScheda3.Text), "WKI" }), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodUA4_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodUA4.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_UnitaAtomiche";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodUA4.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodUA4.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodUA4_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodUADes4.Text = DataBase.FindValue("Descrizione", "T_UnitaAtomiche", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodUA4.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodAzi4_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodAzi4.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Aziende";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodAzi4.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodAzi4.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodAzi4_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodAziDes4.Text = DataBase.FindValue("Descrizione", "T_Aziende", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodAzi4.Text)), "");
            this.uteCodUO4_ValueChanged(this.uteCodUO4, new System.EventArgs());
            this.uteCodSettore4_ValueChanged(this.uteCodSettore4, new System.EventArgs());
            this.uteCodLetto4_ValueChanged(this.uteCodLetto4, new System.EventArgs());
            this.ubApplica.Enabled = true;
        }

        private void uteCodUO4_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {
                if (this.uteCodAzi4.Text != "")
                {
                    frmZoom f = new frmZoom();
                    f.ViewText = this.lblCodUO4.Text;
                    f.ViewIcon = this.ViewIcon;
                    f.ViewSqlStruct.Sql = "Select '*' AS Codice, 'Tutte' AS Descrizione UNION ALL Select Codice, Descrizione From T_UnitaOperative";
                    f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodUO4.Text) + "' AND CodAzi = '" + DataBase.Ax2(this.uteCodAzi4.Text) + "'";
                    f.ViewInit();
                    f.ShowDialog();
                    if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        this.uteCodUO4.Text = f.ViewActiveRow.Cells["Codice"].Text;
                    }
                }
                else
                    MessageBox.Show(this.lblCodAzi4.Text + @" non inserito !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodUO4_ValueChanged(object sender, EventArgs e)
        {
            if (this.uteCodUO4.Text == "*")
                this.lblCodUODes4.Text = @"Tutte";
            else
                this.lblCodUODes4.Text = DataBase.FindValue("Descrizione", "T_UnitaOperative", string.Format("Codice = '{0}' AND CodAzi = '{1}'", new string[2] { DataBase.Ax2(this.uteCodUO4.Text), DataBase.Ax2(this.uteCodAzi4.Text) }), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodSettore4_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {
                if (this.uteCodAzi4.Text != "")
                {
                    frmZoom f = new frmZoom();
                    f.ViewText = this.lblCodSettore4.Text;
                    f.ViewIcon = this.ViewIcon;
                    f.ViewSqlStruct.Sql = "Select '*' AS Codice, 'Tutti' AS Descrizione UNION ALL Select Codice, Descrizione From T_Settori";
                    f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodSettore4.Text) + "' AND CodAzi = '" + DataBase.Ax2(this.uteCodAzi4.Text) + "'";
                    f.ViewInit();
                    f.ShowDialog();
                    if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        this.uteCodSettore4.Text = f.ViewActiveRow.Cells["Codice"].Text;
                    }
                }
                else
                    MessageBox.Show(this.lblCodAzi4.Text + @" non inserito !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodSettore4_ValueChanged(object sender, EventArgs e)
        {
            if (this.uteCodSettore4.Text == "*")
                this.lblCodSettoreDes4.Text = @"Tutti";
            else
                this.lblCodSettoreDes4.Text = DataBase.FindValue("Descrizione", "T_Settori", string.Format("Codice = '{0}' AND CodAzi = '{1}'", new string[2] { DataBase.Ax2(this.uteCodSettore4.Text), DataBase.Ax2(this.uteCodAzi4.Text) }), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodLetto4_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {
                if (this.uteCodAzi4.Text != "" && this.uteCodSettore4.Text != "")
                {
                    frmZoom f = new frmZoom();
                    f.ViewText = this.lblCodLetto4.Text;
                    f.ViewIcon = this.ViewIcon;
                    f.ViewSqlStruct.Sql = "Select '*' AS Codice, 'Tutti' AS Descrizione UNION ALL Select CodLetto, Descrizione From T_Letti";
                    f.ViewSqlStruct.Where = "CodLetto <> '" + this.uteCodLetto4.Text + "' AND CodSettore = '" + DataBase.Ax2(this.uteCodSettore4.Text) + "' AND CodAzi = '" + DataBase.Ax2(this.uteCodAzi4.Text) + "'";
                    f.ViewInit();
                    f.ShowDialog();
                    if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        this.uteCodLetto4.Text = f.ViewActiveRow.Cells["Codice"].Text;
                    }
                }
                else
                    MessageBox.Show(this.lblCodAzi4.Text + @" o " + this.lblCodSettore4.Text + @" non inseriti !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodLetto4_ValueChanged(object sender, EventArgs e)
        {
            if (this.uteCodLetto4.Text == "*")
                this.lblCodLettoDes4.Text = @"Tutti";
            else
                this.lblCodLettoDes4.Text = DataBase.FindValue("Descrizione", "T_Letti", string.Format("CodLetto = '{0}' AND CodAzi = '{1}' AND CodSettore = '{2}'", new string[3] { DataBase.Ax2(this.uteCodSettore4.Text), DataBase.Ax2(this.uteCodAzi4.Text), DataBase.Ax2(this.uteCodSettore4.Text) }), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodTipoAgenda6_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodTipoAgenda6.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_TipoAgenda";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodTipoAgenda6.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodTipoAgenda6.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodTipoAgenda6_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodTipoAgendaDes6.Text = DataBase.FindValue("Descrizione", "T_TipoAgenda", string.Format("Codice = '{0}'", new string[1] { DataBase.Ax2(this.uteCodTipoAgenda6.Text) }), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodEntita6_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodEntita6.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Entita";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodEntita6.Text) + "' And IsNull(UsaAgende, 0) <> 0";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodEntita6.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodEntita6_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodEntitaDes6.Text = DataBase.FindValue("Descrizione", "T_Entita", string.Format("Codice = '{0}' And IsNull(UsaAgende, 0) <> 0", new string[1] { DataBase.Ax2(this.uteCodEntita6.Text) }), "");
            LoadMultiSelCampi();
            this.ubApplica.Enabled = true;
        }

        private void ucpColore6_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColore6.Value = this.colorDialog.Color;
            }
        }

        private void UltraToolbarsManager6_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.ActionGridToolClick(e.Tool, ref this.UltraGrid6);
        }

        private void UltraGrid6_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("CodAgenda") == true) { e.Layout.Bands[0].Columns["CodAgenda"].Hidden = true; }
                if (e.Layout.Bands[0].Columns.Exists("OrariLavoro") == true) { e.Layout.Bands[0].Columns["OrariLavoro"].Hidden = true; }

            }
            catch (Exception)
            {

            }

        }

        private void ucpColore7_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColore7.Value = this.colorDialog.Color;
            }
        }

        private void uteCodScheda7_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                switch (e.Button.Key)
                {

                    case "Zoom":
                        frmZoom f = new frmZoom();
                        f.ViewText = this.lblCodScheda7.Text;
                        f.ViewIcon = this.ViewIcon;
                        f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Schede";
                        f.ViewSqlStruct.Where = "CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.APP.ToString() + "' And Codice <> '" + DataBase.Ax2(this.uteCodScheda7.Text) + "'";
                        f.ViewInit();
                        f.ShowDialog();
                        if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            this.uteCodScheda7.Text = f.ViewActiveRow.Cells["Codice"].Text;
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
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodScheda7_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodSchedaDes7.Text = DataBase.FindValue("Descrizione", "T_Schede", string.Format("Codice = '{0}' AND CodEntita = '{1}'", new string[2] { DataBase.Ax2(this.uteCodScheda7.Text), UnicodeSrl.Scci.Enums.EnumEntita.APP.ToString() }), "");
            this.ubApplica.Enabled = true;
        }

        private void chkSenzaData7_CheckedChanged(object sender, EventArgs e)
        {
            setCheckTipoAppuntamento7();
        }
        private void chkSenzaDataSempre7_CheckedChanged(object sender, EventArgs e)
        {
            setCheckTipoAppuntamento7();
        }
        private void chkMultiplo7_CheckedChanged(object sender, EventArgs e)
        {
            setCheckTipoAppuntamento7();

        }
        private void chkSettimanale7_CheckedChanged(object sender, EventArgs e)
        {
            setCheckTipoAppuntamento7();

        }

        private void setCheckTipoAppuntamento7()
        {

            if (!_runtime)
            {
                _runtime = true;

                if (this.chkSenzaData7.Checked)
                {
                    this.chkSenzaDataSempre7.Enabled = true;
                    this.chkSettimanale7.Enabled = true;
                    this.chkMultiplo7.Enabled = true;

                    if (this.chkSenzaDataSempre7.Checked)
                    {
                        this.chkSettimanale7.Enabled = false;
                        this.chkMultiplo7.Enabled = false;
                        this.chkSettimanale7.Checked = false;
                        this.chkMultiplo7.Checked = false;
                    }

                }


                else
                {
                    this.chkSenzaDataSempre7.Enabled = false;
                    this.chkSettimanale7.Enabled = true;
                    this.chkMultiplo7.Enabled = true;
                }

                _runtime = false;
            }
        }

        private void uteCodScheda8_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                switch (e.Button.Key)
                {

                    case "Zoom":
                        frmZoom f = new frmZoom();
                        f.ViewText = this.lblCodScheda8.Text;
                        f.ViewIcon = this.ViewIcon;
                        f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Schede";
                        f.ViewSqlStruct.Where = "CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.ALG.ToString() + "' And Codice <> '" + DataBase.Ax2(this.uteCodScheda8.Text) + "'";
                        f.ViewInit();
                        f.ShowDialog();
                        if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            this.uteCodScheda8.Text = f.ViewActiveRow.Cells["Codice"].Text;
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
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodScheda8_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodSchedaDes8.Text = DataBase.FindValue("Descrizione", "T_Schede", string.Format("Codice = '{0}' AND CodEntita = '{1}'", new string[2] { DataBase.Ax2(this.uteCodScheda8.Text), UnicodeSrl.Scci.Enums.EnumEntita.ALG.ToString() }), "");
            this.ubApplica.Enabled = true;
        }

        private void chkContinuita9_CheckedChanged(object sender, EventArgs e)
        {
            this.umeDurata9.Enabled = chkContinuita9.Checked;
        }

        private void UltraToolbarsManager9_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.ActionGridToolClick(e.Tool, ref this.UltraGrid9);
        }

        private void uteCodProtocollo10_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodProtocollo10Des.Text = DataBase.FindValue("Descrizione", "T_Protocolli", string.Format("Codice = '{0}'", new string[1] { DataBase.Ax2(this.uteCodProtocollo10.Text) }), "");
            this.ubApplica.Enabled = true;
        }

        private void ucpColore11_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ucpColore11.Value = this.colorDialog.Color;
            }
        }

        private void uteCodEntita11_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodEntita11.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Entita";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodEntita11.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodEntita11.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodEntita11_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodEntitaDes11.Text = DataBase.FindValue("Descrizione", "T_Entita", string.Format("Codice = '{0}'", new string[1] { DataBase.Ax2(this.uteCodEntita11.Text) }), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodUA12_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodUA12.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_UnitaAtomiche";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodUA12.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodUA12.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodUA12_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodUADes12.Text = DataBase.FindValue("Descrizione", "T_UnitaAtomiche", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodUA12.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodAzione12_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodAzione12.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_CDSSAzioni";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodAzione12.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodAzione12.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodAzione12_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodAzioneDes12.Text = DataBase.FindValue("Descrizione", "T_CDSSAzioni", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodAzione12.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodPlugin12_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodPlugin12.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_CDSSPlugins";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodPlugin12.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodPlugin12.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodPlugin12_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodPluginDes12.Text = DataBase.FindValue("Descrizione", "T_CDSSPlugins", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodPlugin12.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteParametri12_ValueChanged(object sender, EventArgs e)
        {
            this.ubApplica.Enabled = true;
        }

        private void uteNomePlugin13_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                OpenFileDialog ofdg = new OpenFileDialog();

                ofdg.Filter = "dll (*.dll)|*.dll|exe (*.exe)|*.exe|Tutti i files (*.*)|*.*";
                ofdg.FilterIndex = 1;
                ofdg.RestoreDirectory = true;
                ofdg.AddExtension = true;
                ofdg.CheckFileExists = true;
                ofdg.DefaultExt = ".dll";
                ofdg.Multiselect = false;
                ofdg.Title = "Selezione Plug-in";

                if (ofdg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    Assembly oAssembly = Assembly.LoadFrom(ofdg.FileName);
                    this.uteNomePlugin13.Text = oAssembly.ManifestModule.Name;

                    if (oAssembly.GetTypes().Length > 0)
                    {

                        if (oAssembly.GetTypes().Length == 1)
                        {
                            this.uteComando13.Text = oAssembly.GetTypes()[0].FullName;
                        }
                        else
                        {
                            string sSql = string.Empty;

                            foreach (Type type in oAssembly.GetTypes())
                            {
                                if (type.IsPublic)
                                {
                                    if (sSql != string.Empty) { sSql += "Union" + Environment.NewLine; }
                                    sSql += string.Format("Select '{0}' As Comando", type.FullName) + Environment.NewLine;
                                }
                            }

                            frmZoom f = new frmZoom();
                            f.ViewText = "Elenco Comandi";
                            f.ViewIcon = this.ViewIcon;
                            f.ViewSqlStruct.Sql = sSql;
                            f.ViewSqlStruct.Where = "";
                            f.ViewInit();
                            f.ShowDialog();
                            if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                            {
                                this.uteComando13.Text = f.ViewActiveRow.Cells["Comando"].Text;
                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void uteCodTipoCDSS13_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodTipoCDSS13.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_CDSSTipo";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodTipoCDSS13.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodTipoCDSS13.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void uteCodTipoCDSS13_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodTipoCDSSDes13.Text = DataBase.FindValue("Descrizione", "T_CDSSTipo", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodTipoCDSS13.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodEntita16_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodEntita16.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Entita";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodEntita16.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodEntita16.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodEntita16_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodEntitaDes16.Text = DataBase.FindValue("Descrizione", "T_Entita", string.Format("Codice = '{0}'", new string[1] { DataBase.Ax2(this.uteCodEntita16.Text) }), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodTipoEntita16_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                if (this.uteCodEntita16.Text == "PVT")
                {
                    f.ViewText = this.uteCodTipoEntita16.Text;
                    f.ViewIcon = this.ViewIcon;
                    f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_TipoParametroVitale";
                    f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodTipoEntita16.Text) + "'";
                }
                else if (this.uteCodEntita16.Text == "SCH")
                {
                    f.ViewText = this.uteCodTipoEntita16.Text;
                    f.ViewIcon = this.ViewIcon;
                    f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Schede";
                    f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodTipoEntita16.Text) + "'";
                }
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodTipoEntita16.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodTipoEntita16_ValueChanged(object sender, EventArgs e)
        {
            if (this.uteCodEntita16.Text == "PVT")
            {
                this.lblCodTipoEntitaDes16.Text = DataBase.FindValue("Descrizione", "T_TipoParametroVitale", string.Format("Codice = '{0}'", new string[1] { DataBase.Ax2(this.uteCodTipoEntita16.Text) }), "");
            }
            else if (this.uteCodEntita16.Text == "SCH")
            {
                this.lblCodTipoEntitaDes16.Text = DataBase.FindValue("Descrizione", "T_Schede", string.Format("Codice = '{0}'", new string[1] { DataBase.Ax2(this.uteCodTipoEntita16.Text) }), "");
            }
            this.ubApplica.Enabled = true;
        }

        private void uteCodTipoFiltroSpeciale17_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodTipoFiltroSpeciale17.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_TipoFiltroSpeciale";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodTipoFiltroSpeciale17.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodTipoFiltroSpeciale17.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodTipoFiltroSpeciale17_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodTipoFiltroSpecialeDes17.Text = DataBase.FindValue("Descrizione", "T_TipoFiltroSpeciale", @"Codice = '" + DataBase.Ax2(this.uteCodTipoFiltroSpeciale17.Text) + @"'", "");
            this.ubApplica.Enabled = true;

            if (this.uteCodTipoFiltroSpeciale17.Text.Trim().ToUpper() == EnumTipoFiltroSpeciale.AMBCAR.ToString()
    || this.uteCodTipoFiltroSpeciale17.Text.Trim().ToUpper() == EnumTipoFiltroSpeciale.EPICAR.ToString()
    || this.uteCodTipoFiltroSpeciale17.Text.Trim().ToUpper() == EnumTipoFiltroSpeciale.OETRA.ToString()
    || this.uteCodTipoFiltroSpeciale17.Text.Trim().ToUpper() == EnumTipoFiltroSpeciale.CSPTRA.ToString()
    )
            {
                this.ultraTabControl17.Tabs["RUOLI"].Enabled = true;
            }
            else
            {
                this.ultraTabControl17.Tabs["RUOLI"].Enabled = false;
            }
        }

        private void uteCodScheda18_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                switch (e.Button.Key)
                {

                    case "Zoom":
                        frmZoom f = new frmZoom();
                        f.ViewText = this.lblCodScheda18.Text;
                        f.ViewIcon = this.ViewIcon;
                        f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Schede";
                        f.ViewSqlStruct.Where = "CodEntita = '" + UnicodeSrl.Scci.Enums.EnumEntita.ALA.ToString() + "' And Codice <> '" + DataBase.Ax2(this.uteCodScheda18.Text) + "'";
                        f.ViewInit();
                        f.ShowDialog();
                        if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            this.uteCodScheda18.Text = f.ViewActiveRow.Cells["Codice"].Text;
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
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        private void uteCodScheda18_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodSchedaDes18.Text = DataBase.FindValue("Descrizione", "T_Schede", string.Format("Codice = '{0}' AND CodEntita = '{1}'", new string[2] { DataBase.Ax2(this.uteCodScheda18.Text), UnicodeSrl.Scci.Enums.EnumEntita.ALA.ToString() }), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodRuolo19_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodRuolo19.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Ruoli";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodRuolo19.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodRuolo19.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodRuolo19_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodRuoloDes19.Text = DataBase.FindValue("Descrizione", "T_Ruoli", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodRuolo19.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodAzione19_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodAzione19.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_CDSSAzioni";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodAzione19.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodAzione19.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodAzione19_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodAzioneDes19.Text = DataBase.FindValue("Descrizione", "T_CDSSAzioni", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodAzione19.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void uteCodPlugin19_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodPlugin19.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_CDSSPlugins";
                f.ViewSqlStruct.Where = "Codice <> '" + DataBase.Ax2(this.uteCodPlugin19.Text) + "'";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteCodPlugin19.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void uteCodPlugin19_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodPluginDes19.Text = DataBase.FindValue("Descrizione", "T_CDSSPlugins", string.Format("Codice = '{0}'", DataBase.Ax2(this.uteCodPlugin19.Text)), "");
            this.ubApplica.Enabled = true;
        }

        private void utvOrariLavoro21_AfterActivate(object sender, NodeEventArgs e)
        {

            if (_Modality == Enums.EnumModalityPopUp.mpNuovo || _Modality == Enums.EnumModalityPopUp.mpModifica)
            {

                switch (e.TreeNode.Tag.ToString())
                {

                    case MyStatics.TV_ROOT:
                        this.ubAggiungi21.Enabled = false;
                        this.ubRimuovi21.Enabled = false;
                        this.udteHourI21.Enabled = false;
                        this.udteHourI21.Value = null;
                        this.udteHourF21.Enabled = false;
                        this.udteHourF21.Value = null;
                        this.umeMinuti21.Enabled = false;
                        this.umeMinuti21.Value = null;
                        break;

                    case MyStatics.TV_VOCE:
                        this.ubAggiungi21.Enabled = true;
                        this.ubRimuovi21.Enabled = false;
                        this.udteHourI21.Enabled = true;
                        this.udteHourI21.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
                        this.udteHourF21.Enabled = true;
                        this.udteHourF21.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0);
                        this.umeMinuti21.Enabled = true;
                        this.umeMinuti21.Value = this.CalcolaMinuti();
                        break;

                    default:
                        this.ubAggiungi21.Enabled = false;
                        this.ubRimuovi21.Enabled = true;
                        this.udteHourI21.Enabled = false;
                        this.udteHourI21.Value = null;
                        this.udteHourF21.Enabled = false;
                        this.udteHourF21.Value = null;
                        this.umeMinuti21.Enabled = false;
                        this.umeMinuti21.Value = null;
                        break;

                }

            }

        }

        private void ubAggiungi21_Click(object sender, EventArgs e)
        {

            int nDay = (int)Enum.Parse(typeof(System.DayOfWeek), this.utvOrariLavoro21.ActiveNode.Key);
            string HourI = string.Format("{0:00}:{1:00}", ((DateTime)this.udteHourI21.Value).Hour, ((DateTime)this.udteHourI21.Value).Minute);
            string HourF = string.Format("{0:00}:{1:00}", ((DateTime)this.udteHourF21.Value).Hour, ((DateTime)this.udteHourF21.Value).Minute);

            WorkingHourTimeDaysOfWeek owht = new WorkingHourTimeDaysOfWeek(nDay, HourI, HourF);
            owht.WorkingMinutes = (int)this.umeMinuti21.Value;

            try
            {

                UltraTreeNode oNodeTime = new UltraTreeNode(this.utvOrariLavoro21.ActiveNode.Key + owht.HourI + owht.HourF, string.Format("Dalle {0} Alle {1} (Min.:{2})", owht.HourI, owht.HourF, owht.WorkingMinutes));
                oNodeTime.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_AGENDE, Enums.EnumImageSize.isz16)));
                oNodeTime.Tag = owht;
                this.utvOrariLavoro21.ActiveNode.Nodes.Add(oNodeTime);
                this.utvOrariLavoro21.SelectedNodes.Clear();
                this.utvOrariLavoro21.ActiveNode = oNodeTime;
                this.utvOrariLavoro21.PerformAction(UltraTreeAction.SelectActiveNode, false, false);

                mo_WorkingHourTimeDaysOfWeek.Add(owht);

                this.ubApplica.Enabled = true;

            }
            catch (Exception)
            {

            }

        }

        private void ubRimuovi21_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Vuoi rimuovere il periodo selezionato?", "Periodi", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {

                mo_WorkingHourTimeDaysOfWeek.Remove((WorkingHourTimeDaysOfWeek)this.utvOrariLavoro21.ActiveNode.Tag);
                this.utvOrariLavoro21.ActiveNode.Remove();
                this.utvOrariLavoro21.PerformAction(UltraTreeAction.FirstNode, false, false);

                this.ubApplica.Enabled = true;

            }

        }

        private void udteHour21_ValueChanged(object sender, EventArgs e)
        {
            this.umeMinuti21.Value = this.CalcolaMinuti();
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

                            if (this.ViewModality == Enums.EnumModalityPopUp.mpCopia)
                            {
                                this.ViewDataBindings.SqlSelect.Where = @"0=1";
                                this.ViewDataBindings.DataBindings.DataSet = DataBase.GetDataSet(this.ViewDataBindings.SqlSelect.Sql);
                                DataRow _dr = this.ViewDataBindings.DataBindings.DataSet.Tables[0].NewRow();
                                DataBase.GetDefultValues(ref _dr, this.ViewDataNamePU);
                                this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows.Add(_dr);
                            }

                            this.ViewDataBindings.DataBindings.Update(this.ViewDataBindings.SqlSelect.Sql, MyStatics.Configurazione.ConnectionString, false);
                            this.UpdateBindingsAss();
                            long id = 0;
                            switch (this.ViewDataNamePU)
                            {
                                case Enums.EnumDataNames.T_DCDecodifiche:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + this.uteCodice1.Text + "'";
                                    this.SetUltraToolBarManager(this.UltraToolbarsManager1, ref this.UltraGrid1);
                                    break;

                                case Enums.EnumDataNames.T_DCDecodificheValori:
                                    this.ViewDataBindings.SqlSelect.Where = "CodDec = '" + this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["CodDec"] + "' AND Codice = '" + this.uteCodice2.Text + "'";
                                    this.SetUltraToolBarManager(this.UltraToolbarsManager1, ref this.UltraGrid1);
                                    break;

                                case Enums.EnumDataNames.T_TipoAppuntamento:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + DataBase.Ax2(this.uteCodice7.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_TipoTaskInfermieristico:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + DataBase.Ax2(this.uteCodice3.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_TipoAgenda:
                                case Enums.EnumDataNames.T_TipoAlertGenerico:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + DataBase.Ax2(this.uteCodice4.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_Festivita:
                                    this.ViewDataBindings.SqlSelect.Where = "Data = " + DataBase.SQLDate((DateTime)this.udteData20.Value);
                                    break;

                                case Enums.EnumDataNames.T_Agende:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + DataBase.Ax2(this.uteCodice6.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_Protocolli:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + DataBase.Ax2(this.uteCodice9.Text) + "'";
                                    this.SetUltraToolBarManager(this.UltraToolbarsManager9, ref this.UltraGrid9);
                                    break;

                                case Enums.EnumDataNames.T_ProtocolliTempi:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + DataBase.Ax2(this.uteCodice10.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_EBM:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + DataBase.Ax2(this.uteCodice14.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_Integra_Destinatari:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + DataBase.Ax2(this.uteCodice15.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_Integra_Campi:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + DataBase.Ax2(this.uteCodice16.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_FiltriSpeciali:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + DataBase.Ax2(this.uteCodice17.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_TipoAlertAllergiaAnamnesi:
                                    this.ViewDataBindings.SqlSelect.Where = "Codice = '" + DataBase.Ax2(this.uteCodice18.Text) + "'";
                                    break;

                                case Enums.EnumDataNames.T_CDSSStruttura:
                                    id = DataBase.GetLastIdentityForTable(@"T_CDSSStruttura");
                                    if (id > 0) this.ViewDataBindings.SqlSelect.Where = "ID = " + id.ToString();
                                    break;

                                case Enums.EnumDataNames.T_CDSSStrutturaRuoli:
                                    id = DataBase.GetLastIdentityForTable(@"T_CDSSStrutturaRuoli");
                                    if (id > 0) this.ViewDataBindings.SqlSelect.Where = "ID = " + id.ToString();
                                    break;

                            }
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
            catch (Exception)
            {

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
                MessageBox.Show(@"Si sono verificati errori." + Environment.NewLine + ex.Message, @"Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void frmPUView2_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeUserControls(this.UltraGroupBox);

            if (this.UltraGrid1 != null) this.UltraGrid1.Dispose();
            if (this.UltraGrid9 != null) this.UltraGrid9.Dispose();
            if (this.ugOrariLavoro6 != null) this.ugOrariLavoro6.Dispose();
            if (this.ugMassimale6 != null) this.ugMassimale6.Dispose();
            if (this.ugTipoRaggruppamentoAgenda16 != null) this.ugTipoRaggruppamentoAgenda16.Dispose();
            if (this.ugTipoRaggruppamentoAgenda26 != null) this.ugTipoRaggruppamentoAgenda26.Dispose();
            if (this.ugTipoRaggruppamentoAgenda36 != null) this.ugTipoRaggruppamentoAgenda36.Dispose();
        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                switch (this.ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_DCDecodifiche:
                        MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager1);

                        foreach (ToolBase oTool in this.UltraToolbarsManager1.Tools)
                        {
                            oTool.SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz32));
                            oTool.SharedProps.AppearancesSmall.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz16));
                        }
                        break;

                    case Enums.EnumDataNames.T_Agende:
                        MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager6);

                        foreach (ToolBase oTool in this.UltraToolbarsManager6.Tools)
                        {
                            oTool.SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz32));
                            oTool.SharedProps.AppearancesSmall.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz16));
                        }
                        break;

                    case Enums.EnumDataNames.T_Protocolli:
                        MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager9);

                        foreach (ToolBase oTool in this.UltraToolbarsManager9.Tools)
                        {
                            oTool.SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz32));
                            oTool.SharedProps.AppearancesSmall.Appearance.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(oTool.Key, Enums.EnumImageSize.isz16));
                        }
                        break;
                }


            }
            catch (Exception)
            {
            }

        }

        private void SetUltraToolBarManager(UltraToolbarsManager oToolBarManager,
                                            ref UltraGrid roUltraGrid)
        {
            this.SetUltraToolBarManager(oToolBarManager, ref roUltraGrid, false);
        }
        private void SetUltraToolBarManager(UltraToolbarsManager oToolBarManager,
                                            ref UltraGrid roUltraGrid, bool bLocked)
        {
            bool bNuovo = !bLocked;
            bool bModifica = !bLocked;
            bool bElimina = !bLocked;
            bool bVisualizza = !bLocked;
            bool bStampa = !bLocked;
            bool bAggiorna = !bLocked;
            bool bEsporta = !bLocked;

            if (!bLocked)
            {
                if ((roUltraGrid.Rows.Count > 0 && roUltraGrid.ActiveRow != null) && roUltraGrid.ActiveRow.IsDataRow)
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

                switch (this.ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_StatoEvidenzaClinica:
                        bNuovo = false;
                        bModifica = true;
                        bElimina = false;
                        bVisualizza = true;
                        bStampa = true;
                        bEsporta = true;
                        break;

                    default:
                        break;

                }
            }

            oToolBarManager.Tools[MyStatics.GC_NUOVO].SharedProps.Enabled = bNuovo;
            oToolBarManager.Tools[MyStatics.GC_MODIFICA].SharedProps.Enabled = bModifica;
            oToolBarManager.Tools[MyStatics.GC_ELIMINA].SharedProps.Enabled = bElimina;
            oToolBarManager.Tools[MyStatics.GC_VISUALIZZA].SharedProps.Enabled = bVisualizza;
            oToolBarManager.Tools[MyStatics.GC_STAMPA].SharedProps.Enabled = bStampa;
            oToolBarManager.Tools[MyStatics.GC_AGGIORNA].SharedProps.Enabled = bAggiorna;
            oToolBarManager.Tools[MyStatics.GC_ESPORTA].SharedProps.Enabled = bEsporta;
        }

        private void ActionGridToolClick(ToolBase oTool, ref UltraGrid roUltraGrid)
        {

            UltraGridRow activeRow = null;
            Enums.EnumDataNames oDataNamePU = Enums.EnumDataNames.Nessuno;
            string sCodPadre = "";

            if (roUltraGrid.ActiveRow != null) activeRow = roUltraGrid.ActiveRow;

            switch (this.ViewDataNamePU)
            {
                case Enums.EnumDataNames.T_DCDecodifiche:
                    oDataNamePU = Enums.EnumDataNames.T_DCDecodificheValori;
                    sCodPadre = this.uteCodice1.Text;
                    break;

                case Enums.EnumDataNames.T_Agende:
                    oDataNamePU = Enums.EnumDataNames.T_AgendePeriodi;
                    sCodPadre = this.uteCodice6.Text;

                    break;
                case Enums.EnumDataNames.T_Protocolli:
                    oDataNamePU = Enums.EnumDataNames.T_ProtocolliTempi;
                    sCodPadre = this.uteCodice9.Text;
                    break;

            }

            if (oDataNamePU != Enums.EnumDataNames.Nessuno)
                switch (oTool.Key)
                {
                    case MyStatics.GC_NUOVO:
                        if (MyStatics.ActionDataNameFormPU(oDataNamePU, Enums.EnumModalityPopUp.mpNuovo, this.ViewIcon, this.ViewImage, this.ViewText, ref activeRow, sCodPadre) == DialogResult.OK)
                        {
                            this.LoadUltraGrid(ref roUltraGrid);
                            this.SetUltraToolBarManager(oTool.ToolbarsManager, ref roUltraGrid);
                        }
                        break;

                    case MyStatics.GC_MODIFICA:
                        if (MyStatics.ActionDataNameFormPU(oDataNamePU, Enums.EnumModalityPopUp.mpModifica, this.ViewIcon, this.ViewImage, this.ViewText, ref activeRow, sCodPadre) == DialogResult.OK)
                        {
                            this.LoadUltraGrid(ref roUltraGrid);
                            this.SetUltraToolBarManager(oTool.ToolbarsManager, ref roUltraGrid);
                        }
                        break;

                    case MyStatics.GC_ELIMINA:
                        if (MyStatics.ActionDataNameFormPU(oDataNamePU, Enums.EnumModalityPopUp.mpCancella, this.ViewIcon, this.ViewImage, this.ViewText, ref activeRow, sCodPadre) == DialogResult.OK)
                        {
                            this.LoadUltraGrid(ref roUltraGrid);
                            this.SetUltraToolBarManager(oTool.ToolbarsManager, ref roUltraGrid);
                        }
                        break;

                    case MyStatics.GC_VISUALIZZA:
                        MyStatics.ActionDataNameFormPU(oDataNamePU, Enums.EnumModalityPopUp.mpVisualizza, this.ViewIcon, this.ViewImage, this.ViewText, ref activeRow, sCodPadre);
                        break;

                    case MyStatics.GC_STAMPA:
                        try
                        {
                            roUltraGrid.PrintPreview(Infragistics.Win.UltraWinGrid.RowPropertyCategories.All);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(@"Si sono verificati errori durante il processo di stampa." + Environment.NewLine + ex.Message, @"Errore di stampa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    case MyStatics.GC_AGGIORNA:
                        roUltraGrid.Rows.ColumnFilters.ClearAllFilters();
                        this.LoadUltraGrid(ref roUltraGrid);
                        this.SetUltraToolBarManager(oTool.ToolbarsManager, ref roUltraGrid);
                        break;

                    case MyStatics.GC_ESPORTA:
                        if (this.SaveFileDialog.ShowDialog() == DialogResult.OK)
                            this.UltraGridExcelExporter.Export(roUltraGrid, this.SaveFileDialog.FileName);
                        break;

                    default:
                        break;

                }

        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {

            switch (this.ViewDataNamePU)
            {

                case Enums.EnumDataNames.T_Agende:
                    MyStatics.SetUltraGridLayout(ref this.ugOrariLavoro6, false, false);
                    MyStatics.SetUltraGridLayout(ref this.ugMassimale6, false, false);
                    MyStatics.SetUltraGridLayout(ref this.UltraGrid6, true, false);
                    MyStatics.SetUltraGridLayout(ref this.ugTipoRaggruppamentoAgenda16, true, false);
                    MyStatics.SetUltraGridLayout(ref this.ugTipoRaggruppamentoAgenda26, true, false);
                    MyStatics.SetUltraGridLayout(ref this.ugTipoRaggruppamentoAgenda36, true, false);
                    this.ugOrariLavoro6.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
                    this.ugOrariLavoro6.DisplayLayout.GroupByBox.Hidden = true;
                    this.ugOrariLavoro6.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;
                    this.ugMassimale6.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
                    this.ugMassimale6.DisplayLayout.GroupByBox.Hidden = true;
                    this.ugMassimale6.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;
                    this.ugTipoRaggruppamentoAgenda16.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
                    this.ugTipoRaggruppamentoAgenda26.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
                    this.ugTipoRaggruppamentoAgenda36.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
                    this.ugTipoRaggruppamentoAgenda16.DisplayLayout.GroupByBox.Hidden = true;
                    this.ugTipoRaggruppamentoAgenda26.DisplayLayout.GroupByBox.Hidden = true;
                    this.ugTipoRaggruppamentoAgenda36.DisplayLayout.GroupByBox.Hidden = true;


                    this.ugTipoRaggruppamentoAgenda16.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
                    this.ugTipoRaggruppamentoAgenda26.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
                    this.ugTipoRaggruppamentoAgenda36.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

                    switch (_Modality)
                    {
                        case Enums.EnumModalityPopUp.mpNuovo:
                        case Enums.EnumModalityPopUp.mpModifica:
                            this.ugOrariLavoro6.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
                            this.ugMassimale6.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
                            this.ugTipoRaggruppamentoAgenda16.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
                            this.ugTipoRaggruppamentoAgenda26.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
                            this.ugTipoRaggruppamentoAgenda36.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
                            break;
                        default:
                            this.ugOrariLavoro6.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
                            this.ugMassimale6.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
                            this.ugTipoRaggruppamentoAgenda16.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
                            this.ugTipoRaggruppamentoAgenda26.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
                            this.ugTipoRaggruppamentoAgenda36.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
                            break;
                    }


                    break;

                case Enums.EnumDataNames.T_DCDecodifiche:
                    MyStatics.SetUltraGridLayout(ref this.UltraGrid1, true, false);
                    break;

                case Enums.EnumDataNames.T_Protocolli:
                    MyStatics.SetUltraGridLayout(ref this.UltraGrid9, false, false);
                    UltraGrid9.DisplayLayout.GroupByBox.Hidden = true;
                    break;

                default:
                    break;

            }

        }

        private void LoadUltraGrid(ref UltraGrid roUltraGrid)
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

                if (roUltraGrid.ActiveRow != null) nIndex = roUltraGrid.ActiveRow.Index;

                switch (this.ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_DCDecodifiche:
                        SqlSelect.SelectString = DataBase.GetSqlView(Enums.EnumDataNames.T_DCDecodificheValori);
                        SqlSelect.Where += "CodDec = '" + this.uteCodice1.Text + "'";
                        SqlSelect.OrderBy = "Descrizione";
                        roUltraGrid.DataSource = DataBase.GetDataSet(SqlSelect.Sql);
                        break;

                    case Enums.EnumDataNames.T_Agende:
                        SqlSelect.SelectString = DataBase.GetSqlView(Enums.EnumDataNames.T_AgendePeriodi);
                        SqlSelect.Where += "CodAgenda = '" + this.uteCodice6.Text + "'";
                        SqlSelect.OrderBy = "DataInizio, DataFine";
                        roUltraGrid.DataSource = DataBase.GetDataSet(SqlSelect.Sql);
                        break;

                    case Enums.EnumDataNames.T_Protocolli:
                        SqlSelect.SelectString = DataBase.GetSqlView(Enums.EnumDataNames.T_ProtocolliTempi);
                        SqlSelect.Where += "CodProtocollo = '" + this.uteCodice9.Text + "'";
                        SqlSelect.OrderBy = "Delta,Ora";
                        roUltraGrid.DataSource = DataBase.GetDataSet(SqlSelect.Sql);
                        break;

                    default:
                        roUltraGrid.DataSource = null;
                        break;

                }

                roUltraGrid.Refresh();
                roUltraGrid.Text = string.Format("{0} ({1:#,##0})", this.Text, roUltraGrid.Rows.Count);

                foreach (UltraGridColumn oCol in roUltraGrid.DisplayLayout.Bands[0].Columns)
                {
                    try
                    {
                        switch (oCol.DataType.Name.Trim().ToUpper())
                        {
                            case "DATETIME":
                            case "DATE":
                                switch (this.ViewDataNamePU)
                                {
                                    case Enums.EnumDataNames.T_DCDecodifiche:
                                        oCol.Format = @"dd/MM/yyyy HH:mm";
                                        break;

                                    case Enums.EnumDataNames.T_Protocolli:
                                        oCol.Format = @"HH:mm";
                                        oCol.Hidden = (this.uceCodTipoProtocollo9.Text == EnumTipoProtocollo.DELTA.ToString() ? true : false);
                                        roUltraGrid.DisplayLayout.Bands[0].Columns["Delta"].Hidden = !oCol.Hidden;
                                        roUltraGrid.DisplayLayout.Bands[0].Columns["CodTipoProtocollo"].Hidden = true;
                                        break;

                                    default:
                                        oCol.Format = @"dd/MM/yyyy";
                                        break;
                                }
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
                                switch (this.ViewDataNamePU)
                                {

                                    case Enums.EnumDataNames.T_Protocolli:
                                        oCol.Hidden = (this.uceCodTipoProtocollo9.Text == EnumTipoProtocollo.DELTA.ToString() ? false : true);
                                        roUltraGrid.DisplayLayout.Bands[0].Columns["Ora"].Hidden = !oCol.Hidden;
                                        break;

                                }
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
                                switch (this.ViewDataNamePU)
                                {
                                    case Enums.EnumDataNames.T_DCDecodifiche:
                                        oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                        oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                        break;
                                }
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

                roUltraGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

                if (nIndex != -1)
                {
                    try
                    {
                        roUltraGrid.ActiveRow = roUltraGrid.Rows[nIndex];
                    }
                    catch (Exception)
                    {
                        if (roUltraGrid.Rows.Count > 0)
                            roUltraGrid.ActiveRow = roUltraGrid.Rows[roUltraGrid.Rows.Count - 1];
                    }
                }
                else
                {
                    if (roUltraGrid.Rows.Count > 0)
                        roUltraGrid.ActiveRow = roUltraGrid.Rows[0];
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region UltraTree

        private void LoadUltraTreeOrariLavoro(string parametri)
        {

            UltraTreeNode oNodeParent = null;

            try
            {

                if (parametri != string.Empty)
                {
                    mo_WorkingHourTimeDaysOfWeek = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<List<WorkingHourTimeDaysOfWeek>>(parametri);
                }

                this.utvOrariLavoro21.Nodes.Clear();
                UltraTreeNode oNode = new UltraTreeNode(MyStatics.TV_ROOT, MyStatics.TV_ROOT);
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_ROOT;
                this.utvOrariLavoro21.Nodes.Add(oNode);

                oNodeParent = new UltraTreeNode(System.DayOfWeek.Sunday.ToString(), "Domenica");
                oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNodeParent.Tag = MyStatics.TV_VOCE;
                this.CaricaOrariLavoro(oNodeParent);
                oNode.Nodes.Add(oNodeParent);
                oNodeParent.Expanded = true;

                oNodeParent = new UltraTreeNode(System.DayOfWeek.Monday.ToString(), "Lunedì");
                oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNodeParent.Tag = MyStatics.TV_VOCE;
                this.CaricaOrariLavoro(oNodeParent);
                oNode.Nodes.Add(oNodeParent);
                oNodeParent.Expanded = true;

                oNodeParent = new UltraTreeNode(System.DayOfWeek.Tuesday.ToString(), "Martedì");
                oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNodeParent.Tag = MyStatics.TV_VOCE;
                this.CaricaOrariLavoro(oNodeParent);
                oNode.Nodes.Add(oNodeParent);
                oNodeParent.Expanded = true;

                oNodeParent = new UltraTreeNode(System.DayOfWeek.Wednesday.ToString(), "Mercoledì");
                oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNodeParent.Tag = MyStatics.TV_VOCE;
                this.CaricaOrariLavoro(oNodeParent);
                oNode.Nodes.Add(oNodeParent);
                oNodeParent.Expanded = true;

                oNodeParent = new UltraTreeNode(System.DayOfWeek.Thursday.ToString(), "Giovedì");
                oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNodeParent.Tag = MyStatics.TV_VOCE;
                this.CaricaOrariLavoro(oNodeParent);
                oNode.Nodes.Add(oNodeParent);
                oNodeParent.Expanded = true;

                oNodeParent = new UltraTreeNode(System.DayOfWeek.Friday.ToString(), "Venerdì");
                oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNodeParent.Tag = MyStatics.TV_VOCE;
                this.CaricaOrariLavoro(oNodeParent);
                oNode.Nodes.Add(oNodeParent);
                oNodeParent.Expanded = true;

                oNodeParent = new UltraTreeNode(System.DayOfWeek.Saturday.ToString(), "Sabato");
                oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_FOLDER, Enums.EnumImageSize.isz16)));
                oNodeParent.Tag = MyStatics.TV_VOCE;
                this.CaricaOrariLavoro(oNodeParent);
                oNode.Nodes.Add(oNodeParent);
                oNodeParent.Expanded = true;

                this.utvOrariLavoro21.PerformAction(UltraTreeAction.FirstNode, false, false);
                this.utvOrariLavoro21.PerformAction(UltraTreeAction.ExpandNode, false, false);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void CaricaOrariLavoro(UltraTreeNode oNode)
        {

            UltraTreeNode oNodeTime = null;

            List<WorkingHourTimeDaysOfWeek> ListDaysOfWeek = mo_WorkingHourTimeDaysOfWeek.Where(m => m.DaysOfWeek == (int)Enum.Parse(typeof(System.DayOfWeek), oNode.Key)).OrderBy(r => r.HourI).ToList();
            foreach (WorkingHourTimeDaysOfWeek owht in ListDaysOfWeek)
            {
                oNodeTime = new UltraTreeNode(oNode.Key + owht.HourI + owht.HourF, string.Format("Dalle {0} Alle {1} (Min.:{2})", owht.HourI, owht.HourF, owht.WorkingMinutes));
                oNodeTime.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_AGENDE, Enums.EnumImageSize.isz16)));
                oNodeTime.Tag = owht;
                oNode.Nodes.Add(oNodeTime);
            }

        }

        private string SaveUltraTreeOrariLavoro()
        {

            string sret = string.Empty;

            try
            {

                sret = UnicodeSrl.Scci.Statics.XmlProcs.XmlSerializeToString(mo_WorkingHourTimeDaysOfWeek);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return sret;

        }

        #endregion

        #region CAMPI+ORARI

        private void LoadMultiSelCampi()
        {
            try
            {
                string sSql = "";

                switch (_ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_Agende:

                        this.ucMultiSelectCampi6.ViewShowAll = false;
                        this.ucMultiSelectCampi6.ViewShowFind = true;

                        List<string> o_Split = new List<string>();
                        string elencoCampiXml = "";
                        if (this.ucMultiSelectCampi6.ViewDataSetDX != null) elencoCampiXml = getElencoCampiXml();

                        if (elencoCampiXml == "" && !_DataBinds.DataBindings.DataSet.Tables[0].Rows[0].IsNull("ElencoCampi"))
                            elencoCampiXml = _DataBinds.DataBindings.DataSet.Tables[0].Rows[0]["ElencoCampi"].ToString();

                        o_Split = CoreStatics.DeserializeXmlToElencoCampi(elencoCampiXml);
                        sSql = @"Select Codice, Descrizione As Campi From T_CampiAgende" + Environment.NewLine;
                        sSql += @" Where ";
                        if (this.uteCodEntita6.Text.Trim() != "" && this.lblCodEntitaDes6.Text.Trim() != "")
                            sSql += " CodEntita = '" + DataBase.Ax2(this.uteCodEntita6.Text) + "' " + Environment.NewLine;
                        else
                            sSql += " CodEntita = '' " + Environment.NewLine;
                        if (o_Split.Count > 0)
                        {
                            sSql += @" AND Codice Not In (";
                            for (int x = 0; x < o_Split.Count; x++)
                            {
                                if (o_Split[x].Trim() != "") sSql += @"'" + o_Split[x] + @"',";
                            }
                            sSql = sSql.Substring(0, sSql.Length - 1);
                            sSql += @")" + Environment.NewLine;
                        }
                        sSql += @" Order By Descrizione ";
                        this.ucMultiSelectCampi6.ViewDataSetSX = DataBase.GetDataSet(sSql);
                        sSql = @"Select Codice, Descrizione As Campi From T_CampiAgende" + Environment.NewLine;
                        string sSqlUnion = @"";
                        sSql += @"Where ";
                        if (this.uteCodEntita6.Text.Trim() != "" && this.lblCodEntitaDes6.Text.Trim() != "")
                            sSql += " CodEntita = '" + DataBase.Ax2(this.uteCodEntita6.Text) + "' " + Environment.NewLine;
                        else
                            sSql += " CodEntita = '' " + Environment.NewLine;
                        if (o_Split.Count > 0)
                        {
                            for (int x = 0; x < o_Split.Count; x++)
                            {
                                if (o_Split[x].Trim() != "")
                                {
                                    if (sSqlUnion != "")
                                    {
                                        sSqlUnion += "UNION ALL" + Environment.NewLine;
                                    }
                                    sSqlUnion += sSql + @" And Codice In ('" + o_Split[x] + @"')" + Environment.NewLine;
                                }
                            }
                            sSql = sSqlUnion;
                            sSql += @" Order By Descrizione ";
                        }
                        else
                        {
                            sSql += @" AND 0=1" + Environment.NewLine;
                            sSql += @" Order By Descrizione ";
                        }
                        this.ucMultiSelectCampi6.ViewDataSetDX = DataBase.GetDataSet(sSql);
                        this.ucMultiSelectCampi6.ViewInit();
                        this.ucMultiSelectCampi6.RefreshData();

                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
            }
        }

        private void LoadHour()
        {
            try
            {
                UltraGrid grid = null;
                switch (_ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_Agende:
                        grid = this.ugOrariLavoro6;
                        break;
                    default:
                        break;
                }


                grid.DisplayLayout.ValueLists.Clear();
                grid.DisplayLayout.ValueLists.Add("Hour");
                DateTime oDt = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
                string s = oDt.Hour.ToString("00") + mo_WorkingHourTime.TimeSeparator + oDt.Minute.ToString("00");

                grid.DisplayLayout.ValueLists["Hour"].ValueListItems.Clear();

                while (oDt.Date == DateTime.Today.Date)
                {
                    s = oDt.Hour.ToString("00") + mo_WorkingHourTime.TimeSeparator + oDt.Minute.ToString("00");
                    grid.DisplayLayout.ValueLists["Hour"].ValueListItems.Add(s, s);

                    if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.OneMinute))
                        oDt = oDt.AddMinutes(1);
                    else if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.TwoMinutes))
                        oDt = oDt.AddMinutes(2);
                    else if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.ThreeMinutes))
                        oDt = oDt.AddMinutes(3);
                    else if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.FourMinutes))
                        oDt = oDt.AddMinutes(4);
                    else if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.FiveMinutes))
                        oDt = oDt.AddMinutes(5);
                    else if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.SixMinutes))
                        oDt = oDt.AddMinutes(6);
                    else if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.TenMinutes))
                        oDt = oDt.AddMinutes(10);
                    else if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.TwelveMinutes))
                        oDt = oDt.AddMinutes(12);
                    else if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.FifteenMinutes))
                        oDt = oDt.AddMinutes(15);
                    else if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.TwentyMinutes))
                        oDt = oDt.AddMinutes(20);
                    else if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.ThirtyMinutes))
                        oDt = oDt.AddMinutes(30);
                    else if (this.uceIntervalloSlot6.Text == CoreStatics.GetTimeSlotIntervalIta(TimeSlotInterval.SixtyMinutes))
                        oDt = oDt.AddMinutes(60);
                    else
                        oDt = oDt.AddDays(1);

                }

            }
            catch (Exception)
            {
            }
        }

        private void LoadUltraGridOrari()
        {
            try
            {
                switch (_ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_Agende:
                        try
                        {
                            if (this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0].IsNull("OrariLavoro"))
                                mo_WorkingHourTime = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<WorkingHourTime>("");
                            else
                                mo_WorkingHourTime = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<WorkingHourTime>(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["OrariLavoro"].ToString());
                        }
                        catch (Exception)
                        {
                        }

                        DataTable dataTable = new DataTable("WorkingHourTime");
                        DataColumn colWork = new DataColumn("Giorno", typeof(string));
                        dataTable.Columns.Add(colWork);
                        colWork = new DataColumn("Ora Inizio", typeof(string));
                        dataTable.Columns.Add(colWork);
                        colWork = new DataColumn("Ora Fine", typeof(string));
                        dataTable.Columns.Add(colWork);
                        colWork = new DataColumn("Minuti", typeof(int));
                        dataTable.Columns.Add(colWork);

                        for (int x = 0; x < mo_WorkingHourTime.HourI.Length; x++)
                        {
                            DataRow row = dataTable.NewRow();
                            row["Giorno"] = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((System.DayOfWeek)x);
                            row["Ora Inizio"] = mo_WorkingHourTime.HourI[x];
                            row["Ora Fine"] = mo_WorkingHourTime.HourF[x];
                            row["Minuti"] = mo_WorkingHourTime.WorkingMinutes[x];
                            dataTable.Rows.Add(row);
                        }
                        this.ugOrariLavoro6.DataSource = dataTable;
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
            }

        }

        private void SaveUltraGridOrari()
        {
            UltraGrid grid = null;
            switch (_ViewDataNamePU)
            {
                case Enums.EnumDataNames.T_Agende:
                    grid = this.ugOrariLavoro6;
                    break;
                default:
                    break;
            }


            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow oRow in grid.Rows)
            {
                foreach (Infragistics.Win.UltraWinGrid.UltraGridCell oCel in oRow.Cells)
                {
                    switch (oCel.Column.Index)
                    {
                        case 1:
                            mo_WorkingHourTime.HourI[oRow.Index] = oCel.Value.ToString();
                            break;
                        case 2:
                            mo_WorkingHourTime.HourF[oRow.Index] = oCel.Value.ToString();
                            break;
                        case 3:
                            if (oCel.Value != null && oCel.Text.Trim() != "")
                                mo_WorkingHourTime.WorkingMinutes[oRow.Index] = Convert.ToInt32(oCel.Value);
                            else
                                mo_WorkingHourTime.WorkingMinutes[oRow.Index] = 0;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void uceTimeSlotInterval_ValueChanged(object sender, EventArgs e)
        {
            LoadHour();
            LoadUltraGridOrari();
        }

        private void ugWorkingHourTime_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid grid = null;
            switch (_ViewDataNamePU)
            {
                case Enums.EnumDataNames.T_Agende:
                    grid = this.ugOrariLavoro6;
                    break;
                default:
                    break;
            }

            grid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;

            grid.DisplayLayout.Bands[0].Columns["Ora Inizio"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDown;
            grid.DisplayLayout.Bands[0].Columns["Ora Inizio"].ValueList = grid.DisplayLayout.ValueLists["Hour"];

            grid.DisplayLayout.Bands[0].Columns["Ora Fine"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDown;
            grid.DisplayLayout.Bands[0].Columns["Ora Fine"].ValueList = grid.DisplayLayout.ValueLists["Hour"];

            grid.DisplayLayout.Bands[0].Columns["Minuti"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerNonNegative;
            grid.DisplayLayout.Bands[0].Columns["Minuti"].CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            grid.DisplayLayout.Bands[0].Columns["Minuti"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            grid.DisplayLayout.Bands[0].Columns["Minuti"].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
        }

        private void ugOrariLavoro6_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                int iMinuti = -1;

                switch (e.Cell.Column.Key)
                {
                    case "Ora Inizio":
                    case "Ora Fine":
                        iMinuti = WorkingHourTime.CalcolaMinuti(e.Cell.Row.Cells["Ora Inizio"].Text, e.Cell.Row.Cells["Ora Fine"].Text);
                        break;

                    default:
                        iMinuti = -1;
                        break;
                }

                if (iMinuti >= 0)
                {
                    e.Cell.Row.Cells["Minuti"].Value = iMinuti;
                    e.Cell.Row.Update();
                }

            }
            catch
            {
            }
        }

        private List<string> getElencoCampi()
        {
            List<string> lFields = new List<string>();

            UltraGrid grid = null;
            switch (_ViewDataNamePU)
            {
                case Enums.EnumDataNames.T_Agende:
                    grid = this.ucMultiSelectCampi6.GridDX;
                    break;
                default:
                    break;
            }

            try
            {
                foreach (UltraGridRow oDr in grid.Rows)
                {
                    lFields.Add(oDr.Cells["Codice"].Value.ToString());
                }
            }
            catch (Exception)
            {
            }
            return lFields;
        }
        private string getElencoCampiXml()
        {
            List<string> lFields = getElencoCampi();

            return CoreStatics.SerializeElencoCampi(lFields);
        }

        private void LoadUltraGridMassimali()
        {
            try
            {
                switch (_ViewDataNamePU)
                {
                    case Enums.EnumDataNames.T_Agende:
                        try
                        {
                            if (this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0].IsNull("Risorse"))
                                mo_MassimaliAgenda = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<MassimaliAgenda>("");
                            else
                                mo_MassimaliAgenda = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<MassimaliAgenda>(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["Risorse"].ToString());
                        }
                        catch (Exception)
                        {
                        }

                        DataTable dataTable = new DataTable("Massimali");
                        DataColumn colWork = new DataColumn("Massimale", typeof(int));
                        dataTable.Columns.Add(colWork);

                        for (int x = 0; x < mo_MassimaliAgenda.Massimale.Length; x++)
                        {
                            DataRow row = dataTable.NewRow();
                            row["Massimale"] = mo_MassimaliAgenda.Massimale[x];
                            dataTable.Rows.Add(row);
                        }
                        this.ugMassimale6.DataSource = dataTable;
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
            }

        }

        private void SaveUltraGridMassimali()
        {

            UltraGrid grid = null;

            switch (_ViewDataNamePU)
            {
                case Enums.EnumDataNames.T_Agende:
                    grid = this.ugMassimale6;
                    foreach (Infragistics.Win.UltraWinGrid.UltraGridRow oRow in grid.Rows)
                    {
                        foreach (Infragistics.Win.UltraWinGrid.UltraGridCell oCel in oRow.Cells)
                        {
                            mo_MassimaliAgenda.Massimale[oRow.Index] = (int)oCel.Value;
                        }
                    }
                    break;

                default:
                    break;

            }

        }

        private void ugMassimale6_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid grid = null;
            switch (_ViewDataNamePU)
            {
                case Enums.EnumDataNames.T_Agende:
                    grid = this.ugMassimale6;
                    break;
                default:
                    break;
            }

            grid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;

            grid.DisplayLayout.Bands[0].Columns["Massimale"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerNonNegative;
            grid.DisplayLayout.Bands[0].Columns["Massimale"].CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            grid.DisplayLayout.Bands[0].Columns["Massimale"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            grid.DisplayLayout.Bands[0].Columns["Massimale"].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
        }

        private void ugMassimale6_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {

            try
            {

                e.Cell.Row.Update();

            }
            catch
            {

            }

        }

        #endregion

        #region Parametri Lista

        private void LoadParametriLista()
        {

            try
            {

                switch (_ViewDataNamePU)
                {

                    case Enums.EnumDataNames.T_Agende:
                        try
                        {
                            if (this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0].IsNull("ParametriLista"))
                                mo_ParametriListaAgenda = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<ParametriListaAgenda>("");
                            else
                                mo_ParametriListaAgenda = UnicodeSrl.Scci.Statics.XmlProcs.XmlDeserializeFromString<ParametriListaAgenda>(this.ViewDataBindings.DataBindings.DataSet.Tables[0].Rows[0]["ParametriLista"].ToString());
                        }
                        catch (Exception)
                        {
                        }

                        this.uceTipoRaggruppamentoAgenda16.Value = (int)Enum.Parse(typeof(EnumTipoRaggruppamentoAgenda), mo_ParametriListaAgenda.TipoRaggruppamentoAgenda1.ToString());
                        this.uceTipoRaggruppamentoAgenda26.Value = (int)Enum.Parse(typeof(EnumTipoRaggruppamentoAgenda), mo_ParametriListaAgenda.TipoRaggruppamentoAgenda2.ToString());
                        this.uceTipoRaggruppamentoAgenda36.Value = (int)Enum.Parse(typeof(EnumTipoRaggruppamentoAgenda), mo_ParametriListaAgenda.TipoRaggruppamentoAgenda3.ToString());

                        this.uteDescrizioneRaggruppamentoAgenda1.Text = mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda1;
                        this.uteDescrizioneRaggruppamentoAgenda2.Text = mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda2;
                        this.uteDescrizioneRaggruppamentoAgenda3.Text = mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda3;
                        break;

                    default:
                        break;

                }

            }
            catch (Exception)
            {
            }

        }

        private void LoadRaggruppamentiAgenda(EnumTipoRaggruppamentoAgenda tipo, SerializableDictionary<string, string> raggruppamentoagenda, ref UltraGrid griglia)
        {

            try
            {

                DataTable dt = null;

                UnicodeSrl.Sys.Data2008.SqlStruct SqlSelect = new UnicodeSrl.Sys.Data2008.SqlStruct();
                SqlSelect.SelectString = "";
                SqlSelect.Where += "";
                SqlSelect.GroupBy = "";
                SqlSelect.OrderBy = "";
                SqlSelect.Having = "";

                switch (tipo)
                {

                    case EnumTipoRaggruppamentoAgenda.Nessuno:
                        griglia.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                        SqlSelect.SelectString = "Select '' AS Codice";
                        SqlSelect.Where = "0=1";
                        dt = DataBase.GetDataTable(SqlSelect.Sql);
                        break;

                    case EnumTipoRaggruppamentoAgenda.Campo:
                        griglia.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                        SqlSelect.SelectString = "Select '' AS Codice";
                        SqlSelect.Where = "0=1";
                        dt = DataBase.GetDataTable(SqlSelect.Sql);
                        break;

                    case EnumTipoRaggruppamentoAgenda.Dizionario:
                        griglia.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                        SqlSelect.SelectString = "Select Codice, Descrizione From T_DCDecodifiche";
                        dt = DataBase.GetDataTable(SqlSelect.Sql);
                        break;

                    case EnumTipoRaggruppamentoAgenda.Scheda:
                        griglia.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
                        SqlSelect.SelectString = "Select Codice, Descrizione from T_Schede";
                        SqlSelect.Where = "CodEntita = 'APP'";
                        SqlSelect.OrderBy = "Descrizione";
                        dt = DataBase.GetDataTable(SqlSelect.Sql);
                        DataColumn colWork = new DataColumn("Campo", typeof(string));
                        dt.Columns.Add(colWork);
                        break;

                }

                griglia.DataSource = dt;
                griglia.Refresh();
                griglia.Text = string.Format("{0} ({1:#,##0})", tipo.ToString(), griglia.Rows.Count);
                griglia.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);

                switch (tipo)
                {

                    case EnumTipoRaggruppamentoAgenda.Nessuno:
                        break;

                    case EnumTipoRaggruppamentoAgenda.Campo:
                        SqlSelect.SelectString = "Select '' AS Codice";
                        SqlSelect.Where = "0=1";
                        dt = DataBase.GetDataTable(SqlSelect.Sql);
                        break;

                    case EnumTipoRaggruppamentoAgenda.Dizionario:
                        if (raggruppamentoagenda.Count == 1)
                        {
                            var row = griglia.Rows.FirstOrDefault(r => r.Cells["Codice"].Value.ToString() == raggruppamentoagenda.First().Key);
                            if (row != null) { row.Activate(); }
                        }
                        break;

                    case EnumTipoRaggruppamentoAgenda.Scheda:
                        foreach (KeyValuePair<string, string> kvp in raggruppamentoagenda)
                        {
                            var row = griglia.Rows.FirstOrDefault(r => r.Cells["Codice"].Value.ToString() == kvp.Key);
                            if (row != null)
                            {
                                row.Cells["Campo"].Value = kvp.Value;
                                row.Cells["Campo"].ValueList = LoadCampiScheda(row.Cells["Codice"].Text);
                                row.Update();
                            }
                        }
                        break;

                }

            }
            catch (Exception)
            {

            }

        }

        private void SaveParametriLista()
        {

            switch (_ViewDataNamePU)
            {

                case Enums.EnumDataNames.T_Agende:
                    mo_ParametriListaAgenda.TipoRaggruppamentoAgenda1 = (EnumTipoRaggruppamentoAgenda)Enum.Parse(typeof(EnumTipoRaggruppamentoAgenda), this.uceTipoRaggruppamentoAgenda16.Text);
                    mo_ParametriListaAgenda.TipoRaggruppamentoAgenda2 = (EnumTipoRaggruppamentoAgenda)Enum.Parse(typeof(EnumTipoRaggruppamentoAgenda), this.uceTipoRaggruppamentoAgenda26.Text);
                    mo_ParametriListaAgenda.TipoRaggruppamentoAgenda3 = (EnumTipoRaggruppamentoAgenda)Enum.Parse(typeof(EnumTipoRaggruppamentoAgenda), this.uceTipoRaggruppamentoAgenda36.Text);
                    mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda1 = this.uteDescrizioneRaggruppamentoAgenda1.Text;
                    mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda2 = this.uteDescrizioneRaggruppamentoAgenda2.Text;
                    mo_ParametriListaAgenda.DescrizioneRaggruppamentoAgenda3 = this.uteDescrizioneRaggruppamentoAgenda3.Text;
                    mo_ParametriListaAgenda.RaggruppamentoAgenda1 = SaveRaggruppamento(mo_ParametriListaAgenda.TipoRaggruppamentoAgenda1, this.ugTipoRaggruppamentoAgenda16);
                    mo_ParametriListaAgenda.RaggruppamentoAgenda2 = SaveRaggruppamento(mo_ParametriListaAgenda.TipoRaggruppamentoAgenda2, this.ugTipoRaggruppamentoAgenda26);
                    mo_ParametriListaAgenda.RaggruppamentoAgenda3 = SaveRaggruppamento(mo_ParametriListaAgenda.TipoRaggruppamentoAgenda3, this.ugTipoRaggruppamentoAgenda36);
                    break;

                default:
                    break;

            }

        }

        private SerializableDictionary<string, string> SaveRaggruppamento(EnumTipoRaggruppamentoAgenda tipo, UltraGrid grid)
        {

            SerializableDictionary<string, string> dict = new SerializableDictionary<string, string>();

            try
            {

                switch (tipo)
                {

                    case EnumTipoRaggruppamentoAgenda.Nessuno:
                        break;

                    case EnumTipoRaggruppamentoAgenda.Campo:
                        if (grid.ActiveRow != null)
                        {
                            dict.Add(grid.ActiveRow.Cells["Codice"].Text, grid.ActiveRow.Cells["Codice"].Text);
                        }
                        break;

                    case EnumTipoRaggruppamentoAgenda.Dizionario:
                        if (grid.ActiveRow != null)
                        {
                            dict.Add(grid.ActiveRow.Cells["Codice"].Text, grid.ActiveRow.Cells["Descrizione"].Text);
                        }
                        break;

                    case EnumTipoRaggruppamentoAgenda.Scheda:
                        foreach (UltraGridRow oRow in grid.Rows)
                        {
                            if (oRow.Cells["Campo"].Text != "")
                            {
                                dict.Add(oRow.Cells["Codice"].Text, oRow.Cells["Campo"].Value.ToString());
                            }
                        }
                        break;

                }

            }
            catch (Exception)
            {

            }

            return dict;

        }

        private Infragistics.Win.ValueList LoadCampiScheda(string codice)
        {

            Infragistics.Win.ValueList vl = new Infragistics.Win.ValueList();

            try
            {

                Scheda oScheda = new Scheda(codice, DateTime.Now.Date, MyStatics.Ambiente);
                Gestore oGestore = CoreStatics.GetGestore();
                oGestore.SchedaXML = oScheda.StrutturaXML;
                foreach (DcSezione oDcSezione in oGestore.Scheda.Sezioni.Values)
                {
                    foreach (DcVoce oDcVoce in oDcSezione.Voci.Values)
                    {
                        vl.ValueListItems.Add(oDcVoce.Key, oDcVoce.Descrizione);
                    }
                }
                oGestore = null;
                oScheda = null;

            }
            catch (Exception)
            {

            }

            return vl;

        }

        private void chkLista6_CheckedChanged(object sender, EventArgs e)
        {
            this.tlpLista6.Enabled = chkLista6.Checked;
        }

        private void uceTipoRaggruppamentoAgenda16_ValueChanged(object sender, EventArgs e)
        {
            LoadRaggruppamentiAgenda((EnumTipoRaggruppamentoAgenda)Enum.Parse(typeof(EnumTipoRaggruppamentoAgenda), this.uceTipoRaggruppamentoAgenda16.Text), mo_ParametriListaAgenda.RaggruppamentoAgenda1, ref this.ugTipoRaggruppamentoAgenda16);
        }

        private void uceTipoRaggruppamentoAgenda26_ValueChanged(object sender, EventArgs e)
        {
            LoadRaggruppamentiAgenda((EnumTipoRaggruppamentoAgenda)Enum.Parse(typeof(EnumTipoRaggruppamentoAgenda), this.uceTipoRaggruppamentoAgenda26.Text), mo_ParametriListaAgenda.RaggruppamentoAgenda2, ref this.ugTipoRaggruppamentoAgenda26);
        }

        private void uceTipoRaggruppamentoAgenda36_ValueChanged(object sender, EventArgs e)
        {
            LoadRaggruppamentiAgenda((EnumTipoRaggruppamentoAgenda)Enum.Parse(typeof(EnumTipoRaggruppamentoAgenda), this.uceTipoRaggruppamentoAgenda36.Text), mo_ParametriListaAgenda.RaggruppamentoAgenda3, ref this.ugTipoRaggruppamentoAgenda36);
        }

        private void ugTipoRaggruppamentoAgenda26_BeforeCellActivate(object sender, Infragistics.Win.UltraWinGrid.CancelableCellEventArgs e)
        {

            if (e.Cell.Column.Key == "Campo" && e.Cell.ValueList == null)
            {
                e.Cell.Row.Cells["Campo"].ValueList = LoadCampiScheda(e.Cell.Row.Cells["Codice"].Text);
            }

        }

        private void ugTipoRaggruppamentoAgenda6_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Campo") == true)
                {
                    e.Layout.Bands[0].Columns["Campo"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

    }
}
