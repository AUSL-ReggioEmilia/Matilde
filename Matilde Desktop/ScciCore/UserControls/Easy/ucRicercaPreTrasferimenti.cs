using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using Infragistics.Win.UltraWinGrid;

namespace UnicodeSrl.ScciCore
{
    public partial class ucRicercaPreTrasferimenti : UserControl, Interfacce.IViewUserControlMiddle
    {
        public ucRicercaPreTrasferimenti()
        {
            InitializeComponent();
        }

        #region Declare

        private UserControl _ucc = null;
        private ucRichTextBox _ucRichTextBox = null;
        private Dictionary<string, byte[]> oIcone = new Dictionary<string, byte[]>();

        #endregion

        #region Interface

        public void Aggiorna()
        {

            try
            {
                string sID = string.Empty;
                if (this.UltraGridRicerca.ActiveRow != null)
                {
                    sID = this.UltraGridRicerca.ActiveRow.Cells["IDTrasferimento"].Text;
                }
                if (CoreStatics.CoreApplication.Sessione.RicercaPazienti != string.Empty)
                {
                    this.uteRicerca.Text = CoreStatics.CoreApplication.Sessione.RicercaPazienti;
                }
                this.AggiornaGriglia();

                if (sID != string.Empty && this.UltraGridRicerca.Rows.Count > 0)
                {
                    for (int iRow = 0; iRow < this.UltraGridRicerca.Rows.Count; iRow++)
                    {
                        UltraGridRow item = this.UltraGridRicerca.Rows[iRow];
                        if (item.IsDataRow && !item.IsFilteredOut && item.Cells["IDTrasferimento"].Text.Trim().ToUpper() == sID.Trim().ToUpper())
                        {
                            this.UltraGridRicerca.ActiveRow = item;
                            iRow = this.UltraGridRicerca.Rows.Count + 1;
                        }
                    }
                }
                else
                {
                    this.AggiornaPaziente();
                }

                this.uteRicerca.Focus();

            }
            catch (Exception ex)
            {
                this.AggiornaPaziente();
                CoreStatics.ExGest(ref ex, "Aggiorna", "ucRicercaPreTrasferimenti");
            }

        }

        public void Carica()
        {

            try
            {

                this.uteRicerca.Focus();
                this.Aggiorna();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
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

        #region Properties

        public UltraGridRow RigaPazienteSelezionato
        {
            get
            {

                if (this.UltraGridRicerca.ActiveRow != null)
                {
                    return this.UltraGridRicerca.ActiveRow;
                }
                else
                {
                    return null;
                }

            }
        }

        #endregion

        #region UltraGrid

        private void AggiornaGriglia()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.PreTrasferimentoUACodiceSelezionata);

                if (this.uteRicerca.Text != string.Empty && this.uteRicerca.Text.Length >= 2)
                {

                    string filtrogenerico = string.Empty;

                    string[] ricerche = this.uteRicerca.Text.Split(' ');
                    foreach (string ricerca in ricerche)
                    {

                        string format = "dd/MM/yyyy";
                        DateTime dateTime;
                        if (DateTime.TryParseExact(ricerca, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            op.Parametro.Add("DataNascita", ricerca);
                        }
                        else
                        {
                            filtrogenerico += ricerca + " ";
                        }

                    }

                    op.Parametro.Add("FiltroGenerico", filtrogenerico);

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_CercaPreTrasferimenti", spcoll);

                    this.UltraGridRicerca.DataSource = dt;
                    this.UltraGridRicerca.Refresh();

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void AggiornaPaziente()
        {

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                if (this.UltraGridRicerca.ActiveRow != null)
                {

                    Paziente oPaz = new Paziente("", this.UltraGridRicerca.ActiveRow.Cells["IDEpisodio"].Text);
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
                    op.Parametro.Add("IDEpisodio", this.UltraGridRicerca.ActiveRow.Cells["IDEpisodio"].Text);
                    op.Parametro.Add("Storicizzata", "NO");
                    op.Parametro.Add("SoloRTF", "1");
                    op.Parametro.Add("SoloDatiInRilievoRTF", "1");
                    op.ParametroRipetibile.Add("CodEntita", new string[2] { EnumEntita.PAZ.ToString(), EnumEntita.EPI.ToString() });
                    op.TimeStamp.CodEntita = EnumEntita.CAR.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    this.ucRtfPaziente.ColonnaRTFResize = "DatiRilievoRTF";
                    this.ucRtfPaziente.Dati = Database.GetDataTableStoredProc("MSP_SelMovSchedaAvanzato", spcoll);

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

                if (e.KeyCode == Keys.Enter) ubRicerca_Click(this.ubRicerca, new EventArgs());

            }
            catch (Exception)
            {

            }

        }

        private void ubRicerca_Click(object sender, EventArgs e)
        {

            if (this.uteRicerca.Text != string.Empty && this.uteRicerca.Text.Length >= 2)
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                this.Aggiorna();

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }
            else
            {
                easyStatics.EasyMessageBox("Specificare almeno 2 caratteri per effettuare la ricerca!", "Pre-Trasferimento", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void UltraGridRicerca_AfterRowActivate(object sender, EventArgs e)
        {
            this.AggiornaPaziente();
        }
        private void UltraGridRicerca_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                bool bSwitchGroupHeaders = false;
                UltraGridGroup grpPaziente = null;
                UltraGridGroup grpStruttura = null;

                int refWidth = (int)(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 2.7);

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.UltraGridRicerca.DataRowFontRelativeDimension), g.DpiY) * 3);
                g.Dispose();
                g = null;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;

                e.Layout.Bands[0].ColHeadersVisible = !bSwitchGroupHeaders;
                e.Layout.Bands[0].GroupHeadersVisible = bSwitchGroupHeaders;
                if (bSwitchGroupHeaders)
                {
                    for (int c = e.Layout.Bands[0].Groups.Count - 1; c >= 0; c--)
                    {
                        try
                        {
                            UltraGridGroup grp = e.Layout.Bands[0].Groups[c];
                            e.Layout.Bands[0].Groups.RemoveAt(c);
                            grp.Dispose();
                        }
                        catch
                        {
                        }
                    }
                    e.Layout.Bands[0].Groups.Clear();

                    e.Layout.Bands[0].GroupHeaderLines = 2;
                    grpPaziente = e.Layout.Bands[0].Groups.Add(@"grpPaziente", @"Paziente" + Environment.NewLine + @"Indirizzo");
                    grpStruttura = e.Layout.Bands[0].Groups.Add(@"grpStruttura", @"Struttura" + Environment.NewLine + @"UO - Settore");
                }
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    switch (oCol.Key)
                    {

                        case "Paziente2":
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                            oCol.CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Caption = "Paziente";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = this.UltraGridRicerca.Width - Convert.ToInt32(refWidth * 7.9) - Convert.ToInt32(refBtnWidth * 4) - 30;
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
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpPaziente;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "Paziente3":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Caption = "Sesso, Luogo e Data Nascita";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
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
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpPaziente;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescrUA":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.Header.Caption = "Struttura";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 2);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 1;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpStruttura;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "UO - Settore":
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Hidden = false;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 1;
                            oCol.RowLayoutColumnInfo.OriginY = 1;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = grpStruttura;
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;



                        case "DataIngressoGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Data Ingresso";
                            oCol.Format = "dd/MM/yyyy HH:mm";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.5);
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

                        case "DataRicoveroGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Data Ricovero";
                            oCol.Format = "dd/MM/yyyy HH:mm";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.5);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 2;
                            oCol.RowLayoutColumnInfo.OriginY = 1;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 1;
                            break;

                        case "DescrStatoGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Stato";
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = (int)(refWidth * 1.4);
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 4;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpStato", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescStanzaLetto":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Letto / Stanza";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = refWidth * 1;
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 5;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpStanza", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescEpisodio":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.Header.Caption = @"Tipo Episodio / " + Environment.NewLine + @"Nosologico";
                            oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = refWidth * 1;
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 6;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpEpisodio", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        case "DescrCartellaGriglia":
                            oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                            oCol.Header.Caption = "Cartella";
                            oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                            oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                            try
                            {
                                oCol.MaxWidth = refWidth * 1;
                                oCol.MinWidth = oCol.MaxWidth;
                                oCol.Width = oCol.MaxWidth;
                            }
                            catch (Exception)
                            {
                            }
                            oCol.RowLayoutColumnInfo.OriginX = 7;
                            oCol.RowLayoutColumnInfo.OriginY = 0;
                            oCol.RowLayoutColumnInfo.SpanX = 1;
                            oCol.RowLayoutColumnInfo.SpanY = 2;
                            if (bSwitchGroupHeaders)
                            {
                                oCol.RowLayoutColumnInfo.ParentGroup = e.Layout.Bands[0].Groups.Add(@"grpNumCart", oCol.Header.Caption);
                                oCol.RowLayoutColumnInfo.ParentGroup.Header.Appearance.FontData.SizeInPoints = oCol.Header.Appearance.FontData.SizeInPoints;
                            }
                            break;

                        default:
                            oCol.Hidden = true;
                            break;

                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        private void UltraGridRicerca_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {

            try
            {

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
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

        #endregion

    }
}
