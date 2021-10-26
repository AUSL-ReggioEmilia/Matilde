#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;

using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciCore.ThreadingObj;
using System.Threading;
using UnicodeSrl.Framework.Threading;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.ScciCore.Common.Extensions;

namespace UnicodeSrl.ScciCore
{
    public partial class ucPercorsoAmbulatoriale : UserControl, Interfacce.IViewUserControlMiddle
    {

        #region Declare

        private UserControl _ucc = null;
        private ucRichTextBox _ucRichTextBox = null;
        private Dictionary<string, byte[]> oIcone = new Dictionary<string, byte[]>();
        private bool _flagskiploadevts = true;

        #endregion

        public ucPercorsoAmbulatoriale()
        {
            InitializeComponent();
            _ucc = (UserControl)this;
        }

        #region Interface

        public void Aggiorna()
        {

            CoreStatics.CoreApplication.IDSchedaSelezionata = "";

            if (this.ubRicerca.Enabled == true)
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                try
                {

                    string s = string.Empty;
                    if (this.UltraGridRicerca.ActiveRow != null) { s = this.UltraGridRicerca.ActiveRow.Cells["IDScheda"].Text; }

                    this.AggiornaGriglia();

                    if (s != string.Empty) { CoreStatics.SelezionaRigaInGriglia(ref this.UltraGridRicerca, "IDScheda", s); }

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "Aggiorna", "ucPercorsoAmbulatoriale");
                }
                finally
                {
                    this.AggiornaPaziente();
                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                }

            }

        }

        public void Carica()
        {

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
            _flagskiploadevts = true;
            CoreStatics.CoreApplication.IDSchedaSelezionata = "";

            try
            {
                this.InizializzaUltraGridLayout();
                this.CaricaGriglia();
                this.uteRicerca.Focus();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                _flagskiploadevts = false;
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

        }

        public void Ferma()
        {

            try
            {
                oIcone = new Dictionary<string, byte[]>();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region PROPS

        public UltraGridRow RigaPazienteSelezionato
        {
            get
            {
                return this.UltraGridRicerca.ActiveRow;
            }
        }

        #endregion

        #region private functions

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.UltraGridRicerca);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void CaricaGriglia()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("DatiEstesi", "1");

                op.TimeStamp.CodEntita = EnumEntita.CAC.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_CercaPercorsoAmbulatoriale", spcoll);
                ds.Tables[0].Columns.Add("NA", typeof(Bitmap));
                ds.Tables[0].Columns.Add("CIV", typeof(Bitmap));
                ds.Tables[0].Columns.Add("SCC", typeof(Bitmap));

                this.UltraGridRicerca.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[0], true);

                this.UltraGridSchedePercorsi.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1], true);
                if (this.UltraGridSchedePercorsi.Rows.Count > 0)
                {
                    this.UltraGridSchedePercorsi.ActiveRow = null;
                    this.UltraGridSchedePercorsi.Selected.Rows.Clear();
                    CoreStatics.SelezionaRigaInGriglia(ref UltraGridSchedePercorsi, "Codice", CoreStatics.GC_TUTTI);
                }

                this.ucEasyComboEditorFiltroStatoCartella.ValueMember = "Codice";
                this.ucEasyComboEditorFiltroStatoCartella.DisplayMember = "Descrizione";
                this.ucEasyComboEditorFiltroStatoCartella.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2], false);
                this.ucEasyComboEditorFiltroStatoCartella.Refresh();
                if (this.ucEasyComboEditorFiltroStatoCartella.Items.Count > 0)
                {
                    CoreStatics.SelezionaItemInComboEditor(ref ucEasyComboEditorFiltroStatoCartella, CoreStatics.GC_TUTTI);
                }

                this.ucEasyComboEditorFiltriSpeciali.ValueMember = "Codice";
                this.ucEasyComboEditorFiltriSpeciali.DisplayMember = "Descrizione";

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodTipoFiltroSpeciale", "PERCAC");
                this.ucEasyComboEditorFiltriSpeciali.DataSource = CoreStatics.AggiungiTuttiDataTable(Database.GetDataTableStoredProc("MSP_SelFiltriSpeciali", Database.GetFwDataParametersList(op)), false);

                this.ucEasyComboEditorFiltriSpeciali.Refresh();
                if (this.ucEasyComboEditorFiltriSpeciali.Items.Count > 0)
                {
                    CoreStatics.SelezionaItemInComboEditor(ref ucEasyComboEditorFiltriSpeciali, CoreStatics.GC_TUTTI);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Name);
            }

        }

        private void AggiornaGriglia()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("DatiEstesi", "0");

                if (this.uteRicerca.Text != string.Empty)
                {

                    string filtrogenerico = string.Empty;
                    string[] ricerche = this.uteRicerca.Text.Split(' ');
                    foreach (string ricerca in ricerche)
                    {
                        string format = "dd/MM/yyyy";
                        if (DateTime.TryParseExact(ricerca, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                        {
                            op.Parametro.Add("DataNascita", ricerca);
                        }
                        else
                        {
                            filtrogenerico += ricerca + " ";
                        }
                    }
                    op.Parametro.Add("FiltroGenerico", filtrogenerico);

                }

                if (this.UltraGridSchedePercorsi.ActiveRow != null && this.UltraGridSchedePercorsi.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodScheda", this.UltraGridSchedePercorsi.ActiveRow.Cells["Codice"].Text);
                }

                if (this.ucEasyComboEditorFiltroStatoCartella.Value.ToString().Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodStatoCartella", this.ucEasyComboEditorFiltroStatoCartella.Value.ToString());
                }

                if (this.ucEasyComboEditorFiltriSpeciali.Value.ToString().Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodFiltroSpeciale", this.ucEasyComboEditorFiltriSpeciali.Value.ToString());
                }

                op.TimeStamp.CodEntita = EnumEntita.CAC.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_CercaPercorsoAmbulatoriale", spcoll);
                ds.Tables[0].Columns.Add("NA", typeof(Bitmap));
                ds.Tables[0].Columns.Add("CIV", typeof(Bitmap));
                ds.Tables[0].Columns.Add("SCC", typeof(Bitmap));

                this.UltraGridRicerca.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[0], true);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "AggiornaGriglia", this.Name);
            }

        }

        private void AggiornaPaziente()
        {

            CoreStatics.CoreApplication.IDSchedaSelezionata = "";

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                if (this.UltraGridRicerca.ActiveRow != null)
                {

                    Paziente oPaz = new Paziente(this.UltraGridRicerca.ActiveRow.Cells["IDPaziente"].Text, "");
                    this.ucEasyPictureBoxPaziente.Image = oPaz.Foto;
                    this.ucEasyLabelPaziente.Text = oPaz.Descrizione;
                    oPaz = null;
                    if (oIcone.ContainsKey(Risorse.GC_INRILIEVO_256) == false)
                    {
                        oIcone.Add(Risorse.GC_INRILIEVO_256, CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_256)));
                    }
                    this.ucRtfPaziente.Immagine = CoreStatics.ByteToImage(oIcone[Risorse.GC_INRILIEVO_256]);
                    this.ucRtfPaziente.Titolo = "Dati di Rilievo";

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDPaziente", this.UltraGridRicerca.ActiveRow.Cells["IDPaziente"].Text);
                    op.Parametro.Add("Storicizzata", "NO");
                    op.Parametro.Add("SoloRTF", "1");
                    op.Parametro.Add("SoloDatiInRilievoRTF", "1");
                    op.ParametroRipetibile.Add("CodEntita", new string[2] { EnumEntita.PAZ.ToString(), EnumEntita.EPI.ToString() });
                    op.TimeStamp.CodEntita = EnumEntita.CAR.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    FwDataParametersList fplist = new FwDataParametersList
                            {
                                { "xParametri", xmlParam, ParameterDirection.Input, DbType.Xml }
                            };

                    this.ucRtfPaziente.ColonnaRTFResize = "DatiRilievoRTF";
                    this.ucRtfPaziente.FattoreRidimensionamentoRTF = 26;
                    this.ucRtfPaziente.Dati = Database.GetDataTableStoredProc("MSP_SelMovSchedaAvanzato", fplist);

                    CoreStatics.CoreApplication.IDSchedaSelezionata = this.UltraGridRicerca.ActiveRow.Cells["IDScheda"].Text;

                }
                else
                {
                    this.ucEasyPictureBoxPaziente.Image = null;
                    this.ucEasyLabelPaziente.Text = "";
                    this.ucRtfPaziente.Immagine = null;
                    this.ucRtfPaziente.Titolo = "";
                    this.ucRtfPaziente.Dati = null;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region Events

        private void uteRicerca_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {

                if (this.ubRicerca.Enabled && e.KeyCode == Keys.Enter)
                {
                    this.Aggiorna();
                }

            }
            catch (Exception)
            {
            }

        }

        private void ubRicerca_Click(object sender, EventArgs e)
        {
            this.Aggiorna();
        }

        private void ubAzzera_Click(object sender, EventArgs e)
        {
            this.CaricaGriglia();
        }

        private void UltraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("Codice") == true)
                {
                    e.Layout.Bands[0].Columns["Codice"].Hidden = true;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void UltraGridSchedePercorsi_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (_flagskiploadevts == false) this.Aggiorna();
        }

        private void ucEasyComboEditorFiltriSpeciali_ValueChanged(object sender, EventArgs e)
        {
            if (_flagskiploadevts == false) this.Aggiorna();
        }

        private void ucEasyComboEditorFiltroStatoCartella_ValueChanged(object sender, EventArgs e)
        {
            if (_flagskiploadevts == false) this.Aggiorna();
        }

        private void UltraGridRicerca_AfterRowActivate(object sender, EventArgs e)
        {
            this.AggiornaPaziente();
        }
        private void UltraGridRicerca_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 2.4);
                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.UltraGridRicerca.DataRowFontRelativeDimension), g.DpiY) * 2.6F);
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = true;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {
                    try
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;

                        switch (oCol.Key)
                        {


                            case "Paziente2":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium);
                                oCol.CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Paziente";
                                try
                                {

                                    oCol.MaxWidth = this.UltraGridRicerca.Width - (refWidth * 10) - Convert.ToInt32(refBtnWidth * 3) - 20;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Paziente3":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Sesso, Luogo e Data Nascita";
                                try
                                {
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "DescrScheda":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Percorso";
                                try
                                {
                                    oCol.MaxWidth = (int)(refWidth * 6.0);
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

                            case "DataApertura":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Data Apertura";
                                oCol.Format = "dd/MM/yyyy";
                                try
                                {
                                    oCol.MaxWidth = (int)(refWidth * 2.0);
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

                            case "DataChiusura":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Data Chiusura";
                                oCol.Format = "dd/MM/yyyy";
                                try
                                {
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "NumeroCartella":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.VertScrollBar = false;
                                oCol.Header.Caption = "Cartella";
                                try
                                {
                                    oCol.MaxWidth = (int)(refWidth * 2.0);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;
                                break;

                            case "NA":
                                oCol.Header.Caption = @"";
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                try
                                {
                                    oCol.LockedWidth = true;
                                    oCol.MaxWidth = refBtnWidth;
                                    oCol.Width = oCol.MaxWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;
                                break;

                            case "CIV":
                                oCol.Header.Caption = @"";
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                try
                                {
                                    oCol.LockedWidth = true;
                                    oCol.MaxWidth = refBtnWidth;
                                    oCol.Width = oCol.MaxWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 5;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;
                                break;

                            case "SCC":
                                oCol.Header.Caption = @"";
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                try
                                {
                                    oCol.LockedWidth = true;
                                    oCol.MaxWidth = refBtnWidth;
                                    oCol.Width = oCol.MaxWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 6;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;
                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }

                    }
                    catch (Exception)
                    {
                    }

                }

            }
            catch (Exception)
            {
            }

        }
        private void UltraGridRicerca_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

                if (e.Row.Cells.Exists("NumAllergie") == true)
                {
                    if ((int)e.Row.Cells["NumAllergie"].Value != 0)
                    {
                        if (oIcone.ContainsKey(Risorse.GC_ALERTALLERGIA_16) == false)
                        {
                            oIcone.Add(Risorse.GC_ALERTALLERGIA_16, CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_16)));
                        }
                        e.Row.Cells["NA"].Value = oIcone[Risorse.GC_ALERTALLERGIA_16];
                    }
                }

                if (e.Row.Cells.Exists("FlagCartellaInVisione") == true && e.Row.Cells.Exists("FlagHoDatoCartellaInVisione") == true)
                {
                    if ((int)e.Row.Cells["FlagCartellaInVisione"].Value != 0)
                    {
                        if (oIcone.ContainsKey(Risorse.GC_OCCHIOAPERTO_16) == false)
                        {
                            oIcone.Add(Risorse.GC_OCCHIOAPERTO_16, CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_OCCHIOAPERTO_16)));
                        }
                        e.Row.Cells["CIV"].Value = oIcone[Risorse.GC_OCCHIOAPERTO_16];
                    }
                    else
                    {
                        if ((int)e.Row.Cells["FlagHoDatoCartellaInVisione"].Value != 0)
                        {
                            if (oIcone.ContainsKey(Risorse.GC_OCCHIOAPERTOFRECCIA_16) == false)
                            {
                                oIcone.Add(Risorse.GC_OCCHIOAPERTOFRECCIA_16, CoreStatics.ImageToByte(Risorse.GetImageFromResource(Risorse.GC_OCCHIOAPERTOFRECCIA_16)));
                            }
                            e.Row.Cells["CIV"].Value = oIcone[Risorse.GC_OCCHIOAPERTOFRECCIA_16];
                        }
                    }

                }

                if (e.Row.Cells.Exists("CodStatoConsensoCalcolato") == true)
                {
                    if (e.Row.Cells["CodStatoConsensoCalcolato"].Value.ToString() != "")
                    {
                        if (oIcone.ContainsKey(e.Row.Cells["CodStatoConsensoCalcolato"].Value.ToString()) == false)
                        {
                            oIcone.Add(e.Row.Cells["CodStatoConsensoCalcolato"].Value.ToString(), DBUtils.getIcona16ByTipoStato(EnumEntita.CNC, e.Row.Cells["CodStatoConsensoCalcolato"].Value.ToString(), ""));
                        }

                        if (e.Row.Cells["CodStatoConsensoCalcolato"].Value != null)
                        {
                            var objImageBytes = oIcone[e.Row.Cells["CodStatoConsensoCalcolato"].Value.ToString()];
                            if (objImageBytes != null) e.Row.Cells["SCC"].Value = objImageBytes;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick += ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
        }

        private void ucRtfPaziente_ClickCell(object sender, ClickCellEventArgs e)
        {

            Infragistics.Win.UIElement uie;
            Point oPoint;

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "DatiRilievoRTF":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text);

                        uie = e.Cell.GetUIElement();
                        oPoint = new Point(uie.Rect.Left + ((ucEasyGrid)sender).Parent.Parent.Parent.Parent.Location.X, uie.Rect.Top + ((ucEasyGrid)sender).Parent.Parent.Parent.Parent.Location.Y);

                        this.UltraPopupControlContainer.Show((ucEasyGrid)sender, oPoint);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucRtfPaziente_ClickCell", this.Name);
            }

        }

        private void ucRtfPaziente_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                foreach (UltraGridColumn oUgc in e.Layout.Bands[0].Columns)
                {
                    oUgc.Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("Descrizione") == true)
                {
                    e.Layout.Bands[0].Columns["Descrizione"].Hidden = false;
                    e.Layout.Bands[0].Columns["Descrizione"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                }

                if (e.Layout.Bands[0].Columns.Exists("DatiRilievoRTF") == true)
                {

                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    RichTextEditor a = new RichTextEditor();
                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].Editor = a;
                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].Hidden = false;


                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        private void ucRtfPaziente_InitializeRow(object sender, InitializeRowEventArgs e)
        {

        }

        #endregion

    }
}

