using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class ucParametriVitali : UserControl, Interfacce.IViewUserControlMiddle
    {

        private UserControl _ucc = null;
        private ucRichTextBox _ucRichTextBox = null;

        private Dictionary<int, byte[]> oIcone = new Dictionary<int, byte[]>();

        public ucParametriVitali()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
        }

        #region Interface

        public void Aggiorna()
        {
            if (this.IsDisposed == false)
            {
                this.AggiornaGriglia(true);
            }
        }

        public void Carica()
        {
            try
            {

                InizializzaControlli();
                InizializzaUltraGridLayout();
                VerificaSicurezza();
                InizializzaFiltri();

                CaricaGriglia(true);

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

                CoreStatics.SetContesto(EnumEntita.PVT, null);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region private functions

        private void InizializzaControlli()
        {

            try
            {
                this.ubAdd.Appearance.Image = Properties.Resources.Aggiungi_256;
                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.uchkFiltro.Checked = false;

                this.ubAdd.PercImageFill = 0.75F;
                this.ubAdd.ShortcutKey = Keys.Add;
                this.ubAdd.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;

                this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

                if (CoreStatics.CoreApplication.Episodio != null && CoreStatics.CoreApplication.Episodio.DataRicovero > DateTime.MinValue)
                    this.drFiltro.DataEpisodio = CoreStatics.CoreApplication.Episodio.DataRicovero;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }
        }

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

        private void VerificaSicurezza()
        {

            try
            {
                if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                {
                    this.ubAdd.Enabled = false;
                }
                else
                {
                    this.ubAdd.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.ParamV_Inserisci);
                }
            }
            catch (Exception)
            {
            }
        }

        private void InizializzaFiltri()
        {
            if (this.IsDisposed == false)
            {
                try
                {
                    this.drFiltro.Value = null;
                    this.udteFiltroDA.Value = null;
                    this.udteFiltroA.Value = null;

                    if (this.ucEasyGridFiltroTipo.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroTipo.ActiveRow = null;
                        this.ucEasyGridFiltroTipo.Selected.Rows.Clear();
                        CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroTipo, "CodTipo", CoreStatics.GC_TUTTI);
                    }
                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "InizializzaFiltri", this.Name);
                }
            }

        }

        private void CaricaGriglia(bool datiEstesi)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("CodStatoParametroVitale", EnumStatoParametroVitale.ER.ToString());
                if (datiEstesi)
                    op.Parametro.Add("DatiEstesi", "1");
                else
                    op.Parametro.Add("DatiEstesi", "0");

                op.TimeStamp.CodEntita = EnumEntita.PVT.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovParametriVitali", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();
                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICON") == 0)
                        dcCol.ReadOnly = false;
                }

                this.ucEasyGrid.DataSource = null;
                this.ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);

                this.ucEasyGridFiltroTipo.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1], true);
                this.ucEasyGridFiltroTipo.Refresh();



                this.drFiltro.Value = ucEasyDateRange.C_RNG_48H;
                ubApplicaFiltro_Click(this.ubApplicaFiltro, new EventArgs());


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Name);
            }
        }

        private void AggiornaGriglia(bool datiEstesi)
        {

            bool bFiltro = false;
            string codtiposelezionato = string.Empty;

            CoreStatics.SetNavigazione(false);

            try
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                CoreStatics.SetContesto(EnumEntita.PVT, null);

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("CodStatoParametroVitale", EnumStatoParametroVitale.ER.ToString());
                if (datiEstesi)
                    op.Parametro.Add("DatiEstesi", "1");
                else
                    op.Parametro.Add("DatiEstesi", "0");

                if (this.udteFiltroDA.Value != null)
                {
                    op.Parametro.Add("DataInizio", ((DateTime)this.udteFiltroDA.Value).ToString("dd/MM/yyyy HH:mm").Replace(".", ":"));
                    bFiltro = true;
                }
                if (this.udteFiltroA.Value != null)
                {
                    op.Parametro.Add("DataFine", ((DateTime)this.udteFiltroA.Value).ToString("dd/MM/yyyy HH:mm").Replace(".", ":"));
                    bFiltro = true;
                }
                if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipo"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodTipoParametroVitale", this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipo"].Text);
                    bFiltro = true;
                }

                this.uchkFiltro.Checked = bFiltro;

                op.TimeStamp.CodEntita = EnumEntita.PVT.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovParametriVitali", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();
                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICON") == 0)
                        dcCol.ReadOnly = false;
                }
                if (this.ucEasyGrid.DisplayLayout.Bands.Count > 0) { this.ucEasyGrid.DataSource = null; }
                this.ucEasyGrid.DataSource = dtEdit;
                this.ucEasyGrid.Refresh();

                if (datiEstesi)
                {
                    if (this.ucEasyGridFiltroTipo.DisplayLayout.Bands.Count > 0) { this.ucEasyGridFiltroTipo.DisplayLayout.Bands[0].Columns.ClearUnbound(); }
                    if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipo"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                        codtiposelezionato = this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipo"].Text;
                    this.ucEasyGridFiltroTipo.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1], true);
                    this.ucEasyGridFiltroTipo.Refresh();
                    if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["CodTipo"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                        CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroTipo, "CodTipo", codtiposelezionato);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }


            CoreStatics.SetNavigazione(true);


        }

        #endregion

        #region Events

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {
                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4;
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
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
                                oCol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
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
                                oCol.RowLayoutColumnInfo.SpanY = 4;

                                break;

                            case "DescTipo":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
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

                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 4;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DataEvento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
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

                                break;

                            case "DescrUtente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DataUltimaModifica":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.ForeColor = Color.Gray;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.Format = "(dd/MM/yyyy HH:mm)";
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "DescrUtenteUltimaModifica":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.ForeColor = Color.Gray;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 3;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                oCol.RowLayoutColumnInfo.WeightY = 1; break;

                            case "AnteprimaRTF":
                                oCol.Hidden = false;

                                RichTextEditor a = new RichTextEditor();
                                oCol.Editor = a;
                                oCol.CellActivation = Activation.ActivateOnly;
                                oCol.CellClickAction = CellClickAction.CellSelect;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = true;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - (refWidth * 3) - Convert.ToInt32(refBtnWidth * 3.6) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 5;

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


                    colEdit.RowLayoutColumnInfo.OriginX = 3;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
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
                    colEdit.RowLayoutColumnInfo.OriginX = 4;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
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


                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
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
                    colEdit.RowLayoutColumnInfo.OriginX = 6;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_GRAPH))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_GRAPH);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_PARAMETRIVITALIGRAFICO_32);

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


                    colEdit.RowLayoutColumnInfo.OriginX = 7;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
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
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.1);
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
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
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
                    if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_GRAPH)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }
                    else
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL && ocell.Row.Cells["PermessoCancella"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT && ocell.Row.Cells["PermessoModifica"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_GRAPH && ocell.Row.Cells["PermessoGrafico"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {
            try
            {
                switch (e.Cell.Column.Key)
                {
                    case CoreStatics.C_COL_BTN_EDIT:
                        if (e.Cell.Row.Cells["PermessoModifica"].Text == "1")
                        {

                            CoreStatics.CoreApplication.MovParametroVitaleSelezionato = new MovParametroVitale(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);

                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingParametriVitali) == DialogResult.OK)
                            {
                                AggiornaGriglia(true);
                                if (CoreStatics.CoreApplication.MovParametroVitaleSelezionato != null)
                                {
                                    CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovParametroVitaleSelezionato.IDMovParametroVitale);
                                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato = null;
                                }
                            }

                        }

                        break;

                    case CoreStatics.C_COL_BTN_DEL:
                        if (e.Cell.Row.Cells["PermessoCancella"].Text == "1")
                        {

                            if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "il parametro vitale selezionato ?", "Cancellazione Parametri Vitali", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                MovParametroVitale movpv = new MovParametroVitale(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                movpv.CodStatoParametroVitale = @"CA";
                                if (movpv.Cancella())
                                {
                                    this.InizializzaFiltri();
                                    AggiornaGriglia(true);
                                }
                            }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_GRAPH:
                        if (e.Cell.Row.Cells["PermessoGrafico"].Text == "1")
                        {

                            string idepisodio = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.ID : "");
                            DateTime datada = (this.udteFiltroDA.Value != null ? (DateTime)this.udteFiltroDA.Value : DateTime.MinValue);
                            DateTime dataa = (this.udteFiltroA.Value != null ? (DateTime)this.udteFiltroA.Value : DateTime.MinValue);

                            DateTime dataricovero = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.DataRicovero : DateTime.MinValue);


                            CoreStatics.CoreApplication.DefinizioneGraficoSelezionata = new ToolboxPerGrafici(idepisodio, dataricovero, CoreStatics.CoreApplication.Paziente.CodSAC, EnumEntita.PVT, e.Cell.Row.Cells["CodTipo"].Text, datada, dataa);

                            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.GraficoParametriVitali);

                        }
                        break;



                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
            }
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "AnteprimaRTF":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text);

                        Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                        Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                        this.UltraPopupControlContainer.Show(this.ucEasyGrid, oPoint);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCell", this.Name);
            }

        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.SetContesto(EnumEntita.PVT, this.ucEasyGrid.ActiveRow);
        }

        private void ucEasyGridFiltroTipo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "DescTipo":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Tipo";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }

        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
        }



        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            this.AggiornaGriglia(false);
        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                this.InizializzaFiltri();
                this.AggiornaGriglia(false);
            }
            else
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
        }

        private void ubAdd_Click(object sender, EventArgs e)
        {

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                CoreStatics.CoreApplication.MovParametroVitaleSelezionato = new MovParametroVitale(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                                    CoreStatics.CoreApplication.Paziente.ID,
                                                                                    CoreStatics.CoreApplication.Episodio.ID,
                                                                                    CoreStatics.CoreApplication.Trasferimento.ID, CoreStatics.CoreApplication.Ambiente);
                CoreStatics.CoreApplication.MovParametroVitaleSelezionato.Azione = EnumAzioni.INS;
                CoreStatics.CoreApplication.MovParametroVitaleSelezionato.DataEvento = DateTime.Now;

                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoParametriVitali, false) == DialogResult.OK)
                {
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingParametriVitali, false) == DialogResult.OK)
                    {
                        this.InizializzaFiltri();
                        AggiornaGriglia(true);
                        if (CoreStatics.CoreApplication.MovParametroVitaleSelezionato != null)
                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovParametroVitaleSelezionato.IDMovParametroVitale);
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
            }

            CoreStatics.SetNavigazione(true);
        }

        private void ucParametriVitali_Enter(object sender, EventArgs e)
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

        #endregion

    }
}
