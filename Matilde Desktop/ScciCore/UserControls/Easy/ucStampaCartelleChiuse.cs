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
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using System.IO;
using UnicodeSrl.Framework.Diagnostics;

namespace UnicodeSrl.ScciCore
{
    public partial class ucStampaCartelleChiuse : UserControl, Interfacce.IViewUserControlMiddle
    {

        private UserControl _ucc = null;

        private byte[] m_byteImgMtR = null;

        private const string _COL_IMAGE_MTR = "MTR";

        public ucStampaCartelleChiuse()
        {
            InitializeComponent();
            _ucc = (UserControl)this;

            m_byteImgMtR = CoreStatics.ImageToByte(Properties.Resources.msg_info_24);
        }

        #region INTERFACCIA

        public void Aggiorna()
        {

            CoreStatics.SetNavigazione(false);

            try
            {

                string sID = string.Empty;
                if (this.ucEasyGrid.ActiveRow != null)
                {
                    sID = this.ucEasyGrid.ActiveRow.Cells["IDTrasferimento"].Text;
                }

                this.AggiornaGriglia();

                if (sID != string.Empty && this.ucEasyGrid.Rows.Count > 0)
                {
                    for (int iRow = 0; iRow < this.ucEasyGrid.Rows.Count; iRow++)
                    {
                        UltraGridRow item = this.ucEasyGrid.Rows[iRow];
                        if (item.IsDataRow && !item.IsFilteredOut && item.Cells["IDTrasferimento"].Text.Trim().ToUpper() == sID.Trim().ToUpper())
                        {
                            this.ucEasyGrid.ActiveRow = item;
                            iRow = this.ucEasyGrid.Rows.Count + 1;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

            CoreStatics.SetNavigazione(true);

        }

        public void Carica()
        {
            try
            {
                InizializzaControlli();
                InizializzaUltraGridLayout();
                InizializzaFiltri();

                this.CaricaGriglia();
                this.uteRicerca.Focus();

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

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region FUNZIONI

        private void InizializzaControlli()
        {

            try
            {


                this.ucEasyComboEditorFiltroStatoCartella.Items.Clear();
                this.ucEasyComboEditorFiltroStatoCartella.Items.Add(CoreStatics.GC_TUTTI, "Tutte");
                this.ucEasyComboEditorFiltroStatoCartella.Items.Add(EnumStatoCartella.AP.ToString(), "Aperte");
                this.ucEasyComboEditorFiltroStatoCartella.Items.Add(EnumStatoCartella.CH.ToString(), "Chiuse");
                this.ucEasyComboEditorFiltroStatoCartella.Value = EnumStatoCartella.CH.ToString();

                this.ubStampaCartella.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_STAMPACARTELLA_256);
                this.ubStampaCartella.PercImageFill = 0.75F;
                this.ubStampaCartella.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubStampaCartella.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                this.ubStampaCartellaReferti.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_STAMPACARTELLAREFERTI_256);
                this.ubStampaCartellaReferti.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubStampaCartellaReferti.PercImageFill = 0.75F;
                this.ubStampaCartellaReferti.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubStampaCartellaReferti.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                this.ubStampaCartellaReferti.Enabled = false;
                this.ubStampaCartella.Enabled = false;

            }
            catch (Exception)
            {
            }

        }

        private void InizializzaUltraGridLayout()
        {
            try
            {
                this.ucEasyGrid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void InizializzaFiltri()
        {

            try
            {
                this.uteRicerca.Text = "";
            }
            catch (Exception)
            {
            }

        }

        private void AggiornaGriglia()
        {

            try
            {

                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                if (this.uteRicerca.Text != string.Empty)
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

                }

                if (this.ucEasyComboEditorFiltroStatoCartella.Value.ToString() != CoreStatics.GC_TUTTI)
                    op.Parametro.Add("CodStatoCartella", this.ucEasyComboEditorFiltroStatoCartella.Value.ToString());
                else
                    op.ParametroRipetibile.Add("CodStatoCartella", new string[] { EnumStatoCartella.AP.ToString(), EnumStatoCartella.CH.ToString() });

                op.Parametro.Add("SoloCartelleFirmate", "1");

                op.Parametro.Add("Ordinamento", "MD.DataInserimento DESC");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_CercaCartelle", spcoll);

                if (dt.Columns.Contains(_COL_IMAGE_MTR) == false) dt.Columns.Add(_COL_IMAGE_MTR, typeof(Bitmap));


                this.ucEasyGrid.DataSource = dt;
                this.ucEasyGrid.Refresh();

                this.ubStampaCartella.Enabled = (dt.Rows.Count != 0);
                this.ubStampaCartellaReferti.Enabled = (dt.Rows.Count != 0);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }

        }

        private void CaricaGriglia()
        {

            try
            {

                this.AggiornaGriglia();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }

        }

        private void InitializeRow(UltraGridRow eRow)
        {
            try
            {
                if (eRow.Cells.Exists("ColoreStatoCartella") && eRow.Cells["ColoreStatoCartella"].Text != "")
                {
                    eRow.Appearance.BackColor = CoreStatics.GetColorFromString(eRow.Cells["ColoreStatoCartella"].Text);
                    eRow.Appearance.ForeColor = Color.Black;
                    foreach (UltraGridCell oCell in eRow.Cells)
                    {
                        if (!oCell.Hidden)
                        {
                            oCell.Appearance.ForeColor = Color.Black;
                            oCell.ActiveAppearance.BackColor = eRow.Appearance.BackColor;
                            oCell.ActiveAppearance.ForeColor = Color.Blue;
                            oCell.ActiveAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                        }
                    }
                }

                try
                {
                    if (eRow.Cells.Exists("MotivoRiapertura") && !string.IsNullOrEmpty(eRow.Cells["MotivoRiapertura"].Text) && eRow.Cells["MotivoRiapertura"].Text.Trim() != "")
                    {
                        eRow.Cells[_COL_IMAGE_MTR].Value = m_byteImgMtR;
                        eRow.Cells[_COL_IMAGE_MTR].ToolTipText = "Motivo Riapertura: " + Environment.NewLine + eRow.Cells["MotivoRiapertura"].Text;
                    }
                }
                catch
                {
                }

            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region EVENTI

        private void uteRicerca_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) this.Aggiorna();
            }
            catch (Exception)
            {
            }
        }

        private void cmdRicerca_Click(object sender, EventArgs e)
        {
            this.Aggiorna();
            this.ucEasyGrid.Focus();
        }

        private void ucEasyComboEditorFiltroStatoCartella_ValueChanged(object sender, EventArgs e)
        {
            this.Aggiorna();
            this.ucEasyGrid.Focus();
        }

        private void ucEasyGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {
                int refWidth = (int)(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large) * 0.9);

                Graphics g = this.CreateGraphics();
                int refBtnWidth = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 1;
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = true;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;


                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {
                    try
                    {
                        switch (oCol.Key)
                        {
                            case "NumeroCartella":

                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Cartella";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 4;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescrStatoCartella":

                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Stato";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 4;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "Paziente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Paziente";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 12;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescrUA":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Struttura";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 6;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "Data Ingresso Data Ricovero":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.Header.Caption = @"Data Ingresso /" + Environment.NewLine + @"Data Ricovero";
                                oCol.Format = "dd/MM/yyyy";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 8;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Data Dimissione Data Trasferimento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.Header.Caption = @"Data Dimissione /" + Environment.NewLine + @"Data Trasferimento";
                                oCol.Format = "dd/MM/yyyy";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 8;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 5;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;


                            case "DescEpisodio":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Tipo Episodio /" + Environment.NewLine + @"Nosologico";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                try
                                {
                                    oCol.MaxWidth = refWidth * 6;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 6;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "MedicoFirma":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.Header.Caption = @"Firmato da ";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 8;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 7;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DataFirma":
                                oCol.Hidden = false;
                                oCol.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                oCol.Header.Caption = @"Data Firma";
                                oCol.Format = "dd/MM/yyyy";
                                oCol.Header.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 56) - Convert.ToInt32(refBtnWidth * 1) - 30; ;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 8;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case _COL_IMAGE_MTR:
                                oCol.Hidden = false;
                                oCol.Header.Caption = @"";
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refBtnWidth * 1);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.OriginX = 9;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
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
            catch (Exception ex)
            {
                string aa = ex.Message;
            }

        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            InitializeRow(e.Row);
        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            this.ubStampaCartella.Enabled = false;
            this.ubStampaCartellaReferti.Enabled = false;

            if (this.ucEasyGrid.ActiveRow != null && this.ucEasyGrid.ActiveRow.IsDataRow && !this.ucEasyGrid.ActiveRow.IsFilteredOut)
            {
                this.ubStampaCartella.Enabled = true;

                this.ubStampaCartellaReferti.Enabled = true;

            }

        }

        private void ubStampaCartella_Click(object sender, EventArgs e)
        {

            try
            {

                UltraGridRow oUgr = this.ucEasyGrid.ActiveRow;
                if (oUgr != null)
                {

                    if (oUgr.Cells["CodStatoCartella"].Text == EnumStatoCartella.AP.ToString())
                    {
                        if (oUgr.Cells["IDUltimoDocumentoFirmato"].Text.Trim() == "")
                        {
                            easyStatics.EasyMessageBox(@"CARTELLA APERTA" + Environment.NewLine + @"Documento firmato non disponibile.", "Cartella Aperta", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                        else
                        {
                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                            try
                            {
                                string sreftemp = System.IO.Path.Combine(FileStatics.GetSCCITempPath(), "Cartella" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                                MovDocumentiFirmati tmpdoc = new MovDocumentiFirmati(oUgr.Cells["IDUltimoDocumentoFirmato"].Value.ToString());
                                byte[] pdffirmato = (tmpdoc.PDFNonFirmato == null ? tmpdoc.PDFFirmato : tmpdoc.PDFNonFirmato);
                                sreftemp = sreftemp + (tmpdoc.PDFNonFirmato == null ? @".pdf.p7m" : @".pdf");
                                if (pdffirmato == null || pdffirmato.Length <= 0)
                                {
                                    CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                                    easyStatics.EasyMessageBox("Documento non presente.", "Apertura Documento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(sreftemp, ref pdffirmato))
                                    {
                                        if (System.IO.File.Exists(sreftemp))
                                        {
                                            easyStatics.ShellExecute(sreftemp, "", (tmpdoc.PDFNonFirmato == null ? true : false), string.Empty, false);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                                CoreStatics.ExGest(ref ex, @"Apertura Documento non disponibile." + Environment.NewLine + @"Contattare amministratori di sistema", "ucEasyGridFirme_ClickCellButton", this.Name);
                            }
                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                        }

                    }
                    else
                    {

                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                        CoreStatics.CoreApplication.Paziente = new Paziente("", oUgr.Cells["IDEpisodio"].Text);
                        CoreStatics.CoreApplication.Episodio = new Episodio(oUgr.Cells["IDEpisodio"].Text);
                        CoreStatics.CoreApplication.Trasferimento = new Trasferimento(oUgr.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.Cartella = new Cartella(oUgr.Cells["IDCartella"].Text, oUgr.Cells["NumeroCartella"].Text, CoreStatics.CoreApplication.Ambiente);

                        CoreStatics.apriPDFCartella(CoreStatics.CoreApplication.Cartella);

                        CoreStatics.CoreApplication.Paziente = null;
                        CoreStatics.CoreApplication.Episodio = null;
                        CoreStatics.CoreApplication.Trasferimento = null;
                        CoreStatics.CoreApplication.Cartella = null;

                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubStampaCartella_Click", this.Name);
            }

        }

        private void ubStampaCartellaReferti_Click(object sender, EventArgs e)
        {

            try
            {

                UltraGridRow oUgr = this.ucEasyGrid.ActiveRow;
                if (oUgr != null)
                {
                    if (oUgr.Cells["CodStatoCartella"].Text == EnumStatoCartella.AP.ToString())
                    {

                        MovDocumentiFirmati tmpdoc = new MovDocumentiFirmati(oUgr.Cells["IDUltimoDocumentoFirmato"].Value.ToString());
                        if (tmpdoc.PDFNonFirmato != null)
                        {

                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                            CoreStatics.CoreApplication.Paziente = new Paziente("", oUgr.Cells["IDEpisodio"].Text);
                            CoreStatics.CoreApplication.Episodio = new Episodio(oUgr.Cells["IDEpisodio"].Text);
                            CoreStatics.CoreApplication.Trasferimento = new Trasferimento(oUgr.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);
                            CoreStatics.CoreApplication.Cartella = new Cartella(oUgr.Cells["IDCartella"].Text, oUgr.Cells["NumeroCartella"].Text, CoreStatics.CoreApplication.Ambiente);

                            CoreStatics.apriPDFRefertiCartella(CoreStatics.CoreApplication.Episodio, CoreStatics.CoreApplication.Cartella, tmpdoc.PDFNonFirmato);

                            CoreStatics.CoreApplication.Paziente = null;
                            CoreStatics.CoreApplication.Episodio = null;
                            CoreStatics.CoreApplication.Trasferimento = null;
                            CoreStatics.CoreApplication.Cartella = null;

                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                        }

                        else
                        {
                            easyStatics.EasyMessageBox(@"CARTELLA APERTA" + Environment.NewLine + @"Impossibile allegare i referti al documento firmato.", "Cartella Aperta", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }

                    }
                    else
                    {
                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                        CoreStatics.CoreApplication.Paziente = new Paziente("", oUgr.Cells["IDEpisodio"].Text);
                        CoreStatics.CoreApplication.Episodio = new Episodio(oUgr.Cells["IDEpisodio"].Text);
                        CoreStatics.CoreApplication.Trasferimento = new Trasferimento(oUgr.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.Cartella = new Cartella(oUgr.Cells["IDCartella"].Text, oUgr.Cells["NumeroCartella"].Text, CoreStatics.CoreApplication.Ambiente);

                        CoreStatics.apriPDFRefertiCartella(CoreStatics.CoreApplication.Episodio, CoreStatics.CoreApplication.Cartella);

                        CoreStatics.CoreApplication.Paziente = null;
                        CoreStatics.CoreApplication.Episodio = null;
                        CoreStatics.CoreApplication.Trasferimento = null;
                        CoreStatics.CoreApplication.Cartella = null;

                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubStampaCartellaReferti_Click", this.Name);
            }

        }

        #endregion

    }
}
