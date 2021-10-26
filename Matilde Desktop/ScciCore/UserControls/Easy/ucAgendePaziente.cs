using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciCore.Common.Extensions;

namespace UnicodeSrl.ScciCore
{
    public partial class ucAgendePaziente : UserControl, Interfacce.IViewUserControlMiddle
    {

        private bool _ambulatoriale = false;

        EnumMaschere _mascheraselezionetipoappuntamento = EnumMaschere.SelezioneTipoAppuntamento;
        EnumMaschere _mascheraselezioneagendeappuntamento = EnumMaschere.SelezioneAgendeAppuntamento;
        EnumMaschere _mascheraselezioneappuntamento = EnumMaschere.SelezioneAppuntamento;
        EnumMaschere _mascheraselezionestatoappuntamento = EnumMaschere.SelezioneStatoAppuntamento;

        private UserControl _ucc = null;

        private Dictionary<int, byte[]> oIcone = new Dictionary<int, byte[]>();

        public ucAgendePaziente()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
        }

        #region Declare

        bool bInserisci = false;
        bool bModifica = false;
        bool bCancella = false;

        #endregion

        #region Interface

        public void Aggiorna()
        {

            if (this.IsDisposed == false)
            {
                CoreStatics.SetNavigazione(false);

                try
                {

                    this.VerificaSicurezza();
                    this.CaricaUltraGrid();
                    this.AggiornaAppuntamento();

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "Aggiorna", this.Name);
                }


                CoreStatics.SetNavigazione(true);
            }
        }

        public void Carica()
        {

            try
            {

                CheckAmbulatoriale();

                Ruoli r = CoreStatics.CoreApplication.Sessione.Utente.Ruoli;
                bInserisci = (r.RuoloSelezionato.Esiste(EnumModules.Agende_Inserisci));
                bModifica = (r.RuoloSelezionato.Esiste(EnumModules.Agende_Modifica));
                bCancella = (r.RuoloSelezionato.Esiste(EnumModules.Agende_Cancella));

                this.ubAdd.Appearance.Image = Properties.Resources.Aggiungi_256;
                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Checked = false;

                this.chkSoloEpisodio.Visible = !_ambulatoriale;
                this.chkSoloEpisodio.Checked = (CoreStatics.CoreApplication.Episodio == null ? false : true);
                this.lblSoloEpisodio.Visible = !_ambulatoriale;

                this.ubAdd.PercImageFill = 0.75F;
                this.ubAdd.ShortcutKey = Keys.Add;
                this.ubAdd.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubApplicaFiltro.PercImageFill = 0.75F;

                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);

                this.ucAnteprimaRtf.rtbRichTextBox.ReadOnly = true;

                this.InizializzaFiltri();
                this.InizializzaUltraGridLayout();
                this.VerificaSicurezza();

                this.drFiltro.Value = ucEasyDateRange.C_RNG_N30G; CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroStato, "Codice", EnumStatoAppuntamento.PR.ToString());
                this.Aggiorna();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

        }

        public void Ferma()
        {

            try
            {

                oIcone = new Dictionary<int, byte[]>();

                CoreStatics.SetContesto(EnumEntita.APP, null);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region SubRoutine

        private void CheckAmbulatoriale()
        {

            _ambulatoriale = false;
            if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.Ambulatoriale_AgendePaziente)
                _ambulatoriale = true;

            _mascheraselezionetipoappuntamento = EnumMaschere.SelezioneTipoAppuntamento;
            _mascheraselezioneagendeappuntamento = EnumMaschere.SelezioneAgendeAppuntamento;
            _mascheraselezioneappuntamento = EnumMaschere.SelezioneAppuntamento;
            _mascheraselezionestatoappuntamento = EnumMaschere.SelezioneStatoAppuntamento;
            if (_ambulatoriale)
            {
                _mascheraselezionetipoappuntamento = EnumMaschere.Ambulatoriale_SelezioneTipoAppuntamento;
                _mascheraselezioneagendeappuntamento = EnumMaschere.Ambulatoriale_SelezioneAgendeAppuntamento;
                _mascheraselezioneappuntamento = EnumMaschere.Ambulatoriale_SelezioneAppuntamento;
                _mascheraselezionestatoappuntamento = EnumMaschere.Ambulatoriale_SelezioneStatoAppuntamento;
            }

        }

        private void InizializzaFiltri()
        {
            if (this.IsDisposed == false)
            {
                try
                {

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                    op.Parametro.Add("DatiEstesi", "1");

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovAppuntamenti", spcoll);

                    this.ucEasyGridFiltroTipo.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1], true);
                    this.ucEasyGridFiltroTipo.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Tipo";
                    this.ucEasyGridFiltroTipo.Refresh();

                    this.ucEasyGridFiltroStato.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2], true);
                    this.ucEasyGridFiltroStato.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Stato";
                    this.ucEasyGridFiltroStato.Refresh();


                    this.ucEasyGridFiltroRisorse.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[3], true);
                    this.ucEasyGridFiltroRisorse.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Risorse";
                    this.ucEasyGridFiltroRisorse.Refresh();

                    this.drFiltro.Value = null;
                    this.drFiltro.DateFuture = true;
                    this.udteFiltroDA.Value = null;
                    this.udteFiltroA.Value = null;

                    this.uchkFiltro.Checked = false;

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "InizializzaFiltri", this.Name);
                }
            }
        }

        private void AggiornaAppuntamento()
        {

            try
            {
                if (ucAnteprimaRtf.IsDisposed == false && ucEasyGrid.IsDisposed == false)
                {
                    if (this.ucEasyGrid.ActiveRow != null && this.ucEasyGrid.ActiveRow.IsDataRow == true)
                    {
                        this.ucAnteprimaRtf.rtbRichTextBox.Rtf = this.ucEasyGrid.ActiveRow.Cells["AnteprimaRTF"].Text;
                    }
                    else
                    {
                        this.ucAnteprimaRtf.rtbRichTextBox.Rtf = string.Empty;
                    }
                }



            }
            catch (Exception)
            {
                this.ucAnteprimaRtf.rtbRichTextBox.Rtf = string.Empty;
            }

        }

        private void VerificaSicurezza()
        {

            try
            {
                if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                {
                    this.ubAdd.Enabled = false;
                }
                else
                {
                    this.ubAdd.Enabled = true;
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region UltraGrid

        private void InizializzaUltraGridLayout()
        {

            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }

        }

        private void CaricaUltraGrid()
        {

            bool bFiltro = false;

            try
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                CoreStatics.SetContesto(EnumEntita.APP, null);

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("DatiEstesi", "0");

                if (_ambulatoriale == false)
                {
                    op.Parametro.Add("SoloEpisodio", this.chkSoloEpisodio.Checked == false ? "0" : "1");


                    op.Parametro.Add("IgnoraFiltroCartella", this.chkSoloEpisodio.Checked == false ? "0" : "1");

                }

                if (this.udteFiltroDA.Value != null)
                {
                    op.Parametro.Add("DataInizio", CoreStatics.getDateTime((DateTime)this.udteFiltroDA.Value));
                    bFiltro = true;
                }
                if (this.udteFiltroA.Value != null)
                {
                    op.Parametro.Add("DataFine", CoreStatics.getDateTime((DateTime)this.udteFiltroA.Value));
                    bFiltro = true;
                }

                if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodTipoAppuntamento", this.ucEasyGridFiltroTipo.ActiveRow.Cells["Codice"].Text);
                    bFiltro = true;
                }

                if (this.ucEasyGridFiltroStato.ActiveRow != null && this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodStatoAppuntamento", this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text);
                    bFiltro = true;
                }

                if (this.ucEasyGridFiltroRisorse.ActiveRow != null && this.ucEasyGridFiltroRisorse.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodAgenda", this.ucEasyGridFiltroRisorse.ActiveRow.Cells["Codice"].Text);
                    bFiltro = true;
                }

                this.uchkFiltro.Checked = bFiltro;

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovAppuntamenti", spcoll);

                foreach (DataColumn dcCol in dt.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOEROGAZIONE") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOANNULLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCOPIA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICAORARIO") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICON") >= 0)
                        dcCol.ReadOnly = false;
                }

                if (this.ucEasyGrid.DisplayLayout.Bands.Count > 0) { this.ucEasyGrid.DataSource = null; }

                this.ucEasyGrid.DataSource = dt;
                this.ucEasyGrid.Refresh();

                this.ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaUltraGrid", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region Events

        private void ultraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);

            int filtroWidth = 40 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.ultraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.ultraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].Size.Height);
            this.ultraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.ultraDockManager.DockAreas[0].Size.Height);
            this.pnlFiltro.Width = filtroWidth;
        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.SetContesto(EnumEntita.APP, this.ucEasyGrid.ActiveRow);
            this.AggiornaAppuntamento();
        }

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = (CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 3);
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    oCol.SortIndicator = SortIndicator.Disabled;

                    switch (oCol.Key)
                    {

                        case "Icona":
                            oCol.Hidden = false;
                            oCol.CellClickAction = CellClickAction.RowSelect;
                            oCol.CellActivation = Activation.NoEdit;
                            oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                            oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                            oCol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Top;
                            oCol.VertScrollBar = false;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            oCol.LockedWidth = true;
                            try
                            {
                                oCol.MaxWidth = refWidth;
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }

                            oCol.RowLayoutColumnInfo.OriginX = 0;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;

                            break;

                        case "DescrData":
                            oCol.Hidden = false;
                            oCol.CellClickAction = CellClickAction.RowSelect;
                            oCol.CellActivation = Activation.NoEdit;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                            oCol.VertScrollBar = false;
                            try
                            {
                                oCol.MaxWidth = refWidth * 5;
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }

                            oCol.RowLayoutColumnInfo.OriginX = 1;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;

                            break;

                        case "ElencoRisorse":
                            oCol.Hidden = false;
                            oCol.CellClickAction = CellClickAction.RowSelect;
                            oCol.CellActivation = Activation.NoEdit;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                            oCol.VertScrollBar = false;
                            try
                            {
                                oCol.MaxWidth = this.ucEasyGrid.Width - (refWidth * 14) - Convert.ToInt32(refBtnWidth * 6.25) - 30;
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }

                            oCol.RowLayoutColumnInfo.OriginX = 2;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;

                            break;

                        case "DescrTipoAppuntamento":
                            oCol.Hidden = false;
                            oCol.CellClickAction = CellClickAction.RowSelect;
                            oCol.CellActivation = Activation.NoEdit;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                            oCol.VertScrollBar = false;
                            try
                            {
                                oCol.MaxWidth = refWidth * 4;
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }

                            oCol.RowLayoutColumnInfo.OriginX = 4;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;

                            break;

                        case "DescrStatoAppuntamento":
                            oCol.Hidden = false;
                            oCol.CellClickAction = CellClickAction.RowSelect;
                            oCol.CellActivation = Activation.NoEdit;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                            oCol.VertScrollBar = false;
                            try
                            {
                                oCol.MaxWidth = refWidth * 4;
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }

                            oCol.RowLayoutColumnInfo.OriginX = 5;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;

                            break;

                        default:
                            oCol.Hidden = true;
                            break;

                    }

                }

                if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.TrackerPaziente)
                {
                    if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_GOTO))
                    {

                        UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_GOTO);
                        colEdit.Hidden = false;

                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                        colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                        colEdit.CellActivation = Activation.AllowEdit;
                        colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                        colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                        colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                        colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                        colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDA_32);


                        colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                        try
                        {
                            colEdit.MinWidth = refBtnWidth;
                            colEdit.MaxWidth = colEdit.MinWidth;
                            colEdit.Width = colEdit.MaxWidth;
                        }
                        catch (Exception)
                        {
                        }
                        colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                        colEdit.LockedWidth = true;

                        colEdit.RowLayoutColumnInfo.OriginX = 6;
                        colEdit.RowLayoutColumnInfo.OriginY = 0;
                        colEdit.RowLayoutColumnInfo.SpanX = 1;
                        colEdit.RowLayoutColumnInfo.SpanY = 2;

                    }
                    if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_GOTO + @"_SP"))
                    {

                        UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_GOTO + @"_SP");
                        colEdit.Hidden = false;
                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                        colEdit.CellActivation = Activation.Disabled;
                        colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                        try
                        {
                            colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                            colEdit.MaxWidth = colEdit.MinWidth;
                            colEdit.Width = colEdit.MaxWidth;
                        }
                        catch (Exception)
                        {
                        }
                        colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                        colEdit.LockedWidth = true;

                        colEdit.RowLayoutColumnInfo.OriginX = 7;
                        colEdit.RowLayoutColumnInfo.OriginY = 0;
                        colEdit.RowLayoutColumnInfo.SpanX = 1;
                        colEdit.RowLayoutColumnInfo.SpanY = 2;

                    }

                }
                else
                {

                    if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_GOTO))
                    {

                        UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_GOTO);
                        colEdit.Hidden = false;
                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                        colEdit.CellActivation = Activation.Disabled;
                        colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                        try
                        {
                            colEdit.MinWidth = refBtnWidth;
                            colEdit.MaxWidth = colEdit.MinWidth;
                            colEdit.Width = colEdit.MaxWidth;
                        }
                        catch (Exception)
                        {
                        }
                        colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                        colEdit.LockedWidth = true;

                        colEdit.RowLayoutColumnInfo.OriginX = 6;
                        colEdit.RowLayoutColumnInfo.OriginY = 0;
                        colEdit.RowLayoutColumnInfo.SpanX = 1;
                        colEdit.RowLayoutColumnInfo.SpanY = 2;

                    }
                    if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_GOTO + @"_SP"))
                    {

                        UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_GOTO + @"_SP");
                        colEdit.Hidden = false;
                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                        colEdit.CellActivation = Activation.Disabled;
                        colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                        try
                        {
                            colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                            colEdit.MaxWidth = colEdit.MinWidth;
                            colEdit.Width = colEdit.MaxWidth;
                        }
                        catch (Exception)
                        {
                        }
                        colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                        colEdit.LockedWidth = true;

                        colEdit.RowLayoutColumnInfo.OriginX = 7;
                        colEdit.RowLayoutColumnInfo.OriginY = 0;
                        colEdit.RowLayoutColumnInfo.SpanX = 1;
                        colEdit.RowLayoutColumnInfo.SpanY = 2;

                    }

                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT))
                {

                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;

                    colEdit.RowLayoutColumnInfo.OriginX = 8;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;

                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;

                    colEdit.RowLayoutColumnInfo.OriginX = 9;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;

                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL))
                {

                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;

                    colEdit.RowLayoutColumnInfo.OriginX = 10;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;

                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL + @"_SP");
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;

                    colEdit.RowLayoutColumnInfo.OriginX = 11;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;

                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_STATO))
                {

                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_STATO);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_CONFIGURAZIONE_TABLE_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;

                    colEdit.RowLayoutColumnInfo.OriginX = 12;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;


                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_STATO + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_STATO + @"_SP");
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;

                    colEdit.RowLayoutColumnInfo.OriginX = 13;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;

                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_COPY))
                {

                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_COPY);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_COPIA_32);

                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;

                    colEdit.RowLayoutColumnInfo.OriginX = 14;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;

                }
                if (!e.Layout.Bands[0].Columns.Exists(@"COLFINE_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(@"COLFINE_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;

                    colEdit.RowLayoutColumnInfo.OriginX = 15;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;

                }

            }
            catch (Exception)
            {
            }

        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells.Exists("Icona") == true && e.Row.Cells["IDIcona"].Value.ToString() != "")
                {
                    if (oIcone.ContainsKey(Convert.ToInt32(e.Row.Cells["IDIcona"].Value)) == false)
                    {
                        oIcone.Add(Convert.ToInt32(e.Row.Cells["IDIcona"].Value), CoreStatics.GetImageForGrid(Convert.ToInt32(e.Row.Cells["IDIcona"].Value), 256));
                    }
                    e.Row.Cells["Icona"].Value = oIcone[Convert.ToInt32(e.Row.Cells["IDIcona"].Value)];
                    e.Row.Update();
                }

                foreach (UltraGridCell ocell in e.Row.Cells)
                {

                    if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_STATO)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_COPY)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }
                    else
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT && ocell.Row.Cells["PermessoModifica"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL && ocell.Row.Cells["PermessoCancella"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_STATO && ocell.Row.Cells["PermessoCambiaStato"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_COPY && ocell.Row.Cells["PermessoCopia"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }

        }

        private void ucEasyGridFiltro_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            e.Layout.Bands[0].HeaderVisible = false;

            if (e.Layout.Bands[0].Columns.Exists("Codice") == true)
            {
                e.Layout.Bands[0].Columns["Codice"].Hidden = true;
            }

        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
            this.Aggiorna();
            this.ucEasyGrid.Focus();
        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
                this.InizializzaFiltri();
                this.Aggiorna();
            }
            else
            {
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
            }
        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
        }

        private void ubAdd_Click(object sender, EventArgs e)
        {

            try
            {

                CoreStatics.SetNavigazione(false);

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                string pazienteid = "";
                string codUA = "";
                string episodioid = "";
                string trasferimentoid = "";
                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    trasferimentoid = CoreStatics.CoreApplication.Trasferimento.ID;
                    codUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }
                else
                {
                    codUA = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                }

                if (CoreStatics.CoreApplication.Paziente != null)
                    pazienteid = CoreStatics.CoreApplication.Paziente.ID;

                if (CoreStatics.CoreApplication.Episodio != null)
                    episodioid = CoreStatics.CoreApplication.Episodio.ID;

                CoreStatics.CoreApplication.MovAppuntamentiGenerati = new List<MovAppuntamento>();
                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(codUA,
                                                                                            pazienteid,
                                                                                            episodioid,
                                                                                            trasferimentoid);
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(_mascheraselezionetipoappuntamento, false) == DialogResult.OK)
                {

                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CaricaAgende();

                    while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(_mascheraselezioneagendeappuntamento, false) == DialogResult.OK)
                    {
                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(_mascheraselezioneappuntamento, false) == DialogResult.OK)
                        {

                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                            string idAppuntamentoPerSelezioneGriglia = "";
                            if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato != null) idAppuntamentoPerSelezioneGriglia = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento;

                            if (CoreStatics.CoreApplication.MovAppuntamentiGenerati != null && CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count > 1)
                            {

                                for (int ma = 0; ma < CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count; ma++)
                                {
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato = CoreStatics.CoreApplication.MovAppuntamentiGenerati[ma];

                                    if (_ambulatoriale == true && CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata != null)
                                    {
                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata, CoreStatics.CoreApplication.Ambiente));
                                    }
                                    else
                                    {
                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                    }
                                }


                            }
                            else
                            {

                                if (_ambulatoriale == true && CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata != null)
                                {
                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata, CoreStatics.CoreApplication.Ambiente));
                                }
                                else
                                {
                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                }

                            }


                            this.uchkFiltro.Checked = false;
                            this.InizializzaFiltri();
                            this.Aggiorna();
                            if (!string.IsNullOrEmpty(idAppuntamentoPerSelezioneGriglia))
                            {
                                CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", idAppuntamentoPerSelezioneGriglia);
                            }

                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                            break;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubAdd_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);

                CoreStatics.SetNavigazione(true);
            }



        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_GOTO:
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(e.Cell.Row.Cells["ID"].Text, EnumAzioni.VIS);
                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AgendeTrasversali);
                        break;


                    case CoreStatics.C_COL_BTN_EDIT:
                        if (e.Cell.Row.Cells["PermessoModifica"].Text == "1")
                        {
                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD);


                            while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(_mascheraselezioneagendeappuntamento) == DialogResult.OK)
                            {
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(_mascheraselezioneappuntamento) == DialogResult.OK)
                                {



                                    CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_MODIFICA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                    CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                                    this.Aggiorna();
                                    CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento);
                                    break;
                                }
                            }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_DEL:
                        if (e.Cell.Row.Cells["PermessoCancella"].Text == "1")
                        {

                            bool bEliminaAppuntamentoSingolo = true;

                            if (e.Cell.Row.Cells["CodSistema"].Text == EnumEntita.APP.ToString() && e.Cell.Row.Cells["IDGruppo"].Text.Trim() != "")
                            {
                                if (easyStatics.EasyMessageBox("L'appuntamento selezionato fa parte di un gruppo." +
    Environment.NewLine + "Eseguire la cancellazione di tutto il gruppo ?", "Cancellazione Appuntamento",
    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {

                                    bEliminaAppuntamentoSingolo = false;

                                    if (easyStatics.EasyMessageBox("Confermi la cancellazione di TUTTO IL GRUPPO dell'Appuntamento selezionato ?", "Cancellazione Appuntamenti",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {

                                        CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.WaitCursor);
                                        CoreStatics.SetNavigazione(false);

                                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                        op.Parametro.Add("IDGruppo", e.Cell.Row.Cells["IDGruppo"].Text);
                                        op.Parametro.Add("CodSistema", EnumEntita.APP.ToString());
                                        op.Parametro.Add("DatiEstesi", "0");

                                        op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                                        op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                                        SqlParameterExt[] spcoll = new SqlParameterExt[1];
                                        string xmlParam = XmlProcs.XmlSerializeToString(op);

                                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                        DataSet dsSerieAppuntamenti = Database.GetDatasetStoredProc("MSP_SelMovAppuntamenti", spcoll);

                                        for (int a = dsSerieAppuntamenti.Tables[0].Rows.Count - 1; a >= 0; a--)
                                        {
                                            DataRow drApp = dsSerieAppuntamenti.Tables[0].Rows[a];
                                            if (!drApp.IsNull("PermessoCancella") && drApp["PermessoCancella"].ToString() == "1")
                                            {
                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(drApp["ID"].ToString(), EnumAzioni.MOD);


                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento = EnumStatoAppuntamento.CA.ToString();
                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));


                                                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                                op.Parametro.Add("IDAppuntamento", drApp["ID"].ToString());
                                                op.Parametro.Add("CodStatoAppuntamento", EnumStatoAppuntamento.CA.ToString());

                                                op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                                                op.TimeStamp.CodAzione = EnumAzioni.CAN.ToString();


                                                spcoll = new SqlParameterExt[1];
                                                xmlParam = XmlProcs.XmlSerializeToString(op);

                                                xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                                Database.ExecStoredProc("MSP_AggMovAppuntamenti", spcoll);

                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_CANCELLA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(drApp["CodUA"].ToString(), CoreStatics.CoreApplication.Ambiente));

                                            }
                                        }

                                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                                        CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.DefaultCursor);
                                        CoreStatics.SetNavigazione(true);

                                        this.Aggiorna();

                                    }
                                }
                            }

                            if (bEliminaAppuntamentoSingolo)
                            {
                                if (easyStatics.EasyMessageBox("Confermi la cancellazione dell'appuntamento selezionato?", "Cancellazione appuntamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.WaitCursor);
                                    CoreStatics.SetNavigazione(false);


                                    string idRiga = e.Cell.Row.Cells["ID"].Text;
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(idRiga, EnumAzioni.MOD);


                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento = EnumStatoAppuntamento.CA.ToString();
                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                    op.Parametro.Add("IDAppuntamento", e.Cell.Row.Cells["ID"].Text);
                                    op.Parametro.Add("CodStatoAppuntamento", EnumStatoAppuntamento.CA.ToString());

                                    op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                                    op.TimeStamp.CodAzione = EnumAzioni.CAN.ToString();

                                    op.MovScheda = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda;

                                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                                    string xmlParam = XmlProcs.XmlSerializeToString(op);

                                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                    Database.ExecStoredProc("MSP_AggMovAppuntamenti", spcoll);

                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_CANCELLA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                    CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                                    CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.DefaultCursor);
                                    CoreStatics.SetNavigazione(true);

                                    this.Aggiorna();
                                }
                            }

                        }
                        break;

                    case CoreStatics.C_COL_BTN_STATO:
                        if (e.Cell.Row.Cells["PermessoCambiaStato"].Text == "1")
                        {

                            string idRiga = e.Cell.Row.Cells["ID"].Text;
                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(idRiga, EnumAzioni.MOD);

                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(_mascheraselezionestatoappuntamento) == DialogResult.OK)
                            {
                                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.AN.ToString())
                                {
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione = EnumAzioni.ANN;

                                    CoreStatics.CompletaDatiAppuntamento(CoreStatics.CoreApplication.MovAppuntamentoSelezionato, CoreStatics.CoreApplication.Ambiente);

                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AnnullaAppuntamento) == DialogResult.OK)
                                    {



                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_ANNULLA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                        this.Aggiorna();
                                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento);
                                    }
                                }
                                else
                                {

                                    if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.ER.ToString())
                                    {
                                        CoreStatics.CompletaDatiAppuntamento(CoreStatics.CoreApplication.MovAppuntamentoSelezionato, CoreStatics.CoreApplication.Ambiente);
                                    }

                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Salva();

                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_MODIFICA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                    if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.ER.ToString())
                                    {
                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Ripianificazione(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente);
                                    }

                                    CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                                    this.Aggiorna();
                                    CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento);

                                }
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = null;
                            }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_COPY:
                        if (e.Cell.Row.Cells["PermessoCopia"].Text == "1")
                        {
                            if (easyStatics.EasyMessageBox("Confermi la copia dell'appuntamento selezionato ?", "Copia Appuntamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {

                                MovAppuntamento movtiorigine = new MovAppuntamento(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD);
                                if (movtiorigine.CodStatoAppuntamento != EnumStatoAppuntamento.DP.ToString())
                                    movtiorigine.CodStatoAppuntamento = EnumStatoAppuntamento.PR.ToString();

                                string pazienteid = "";
                                string trasferimentocodua = "";
                                string episodioid = "";
                                string trasferimentoid = "";
                                if (CoreStatics.CoreApplication.Trasferimento != null)
                                {
                                    trasferimentoid = CoreStatics.CoreApplication.Trasferimento.ID;
                                    trasferimentocodua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                                }

                                if (CoreStatics.CoreApplication.Paziente != null)
                                    pazienteid = CoreStatics.CoreApplication.Paziente.ID;

                                if (CoreStatics.CoreApplication.Episodio != null)
                                    episodioid = CoreStatics.CoreApplication.Episodio.ID;

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(trasferimentocodua,
                                                                                                             pazienteid,
                                                                                                             episodioid,
                                                                                                             trasferimentoid);
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CopiaDaOrigine(ref movtiorigine);
                                movtiorigine = null;

                                while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(_mascheraselezioneagendeappuntamento) == DialogResult.OK)
                                {
                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(_mascheraselezioneappuntamento) == DialogResult.OK)
                                    {


                                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);


                                        if (CoreStatics.CoreApplication.MovAppuntamentiGenerati != null && CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count > 1)
                                        {
                                            for (int ma = 0; ma < CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count; ma++)
                                            {
                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = CoreStatics.CoreApplication.MovAppuntamentiGenerati[ma];

                                                if (_ambulatoriale == true && CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata != null)
                                                {
                                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata, CoreStatics.CoreApplication.Ambiente));
                                                }
                                                else
                                                {
                                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (_ambulatoriale == true && CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata != null)
                                            {
                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata, CoreStatics.CoreApplication.Ambiente));
                                            }
                                            else
                                            {
                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                            }
                                        }


                                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                                        this.Aggiorna();
                                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento);
                                        break;
                                    }
                                }

                            }

                        }
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);

                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.DefaultCursor);
                CoreStatics.SetNavigazione(true);
            }

        }

        private void ucAgendePaziente_Enter(object sender, EventArgs e)
        {
            try
            {
                this.ucEasyGrid.PerformLayout();
            }
            catch (Exception)
            {
            }
        }

        #endregion

    }
}
