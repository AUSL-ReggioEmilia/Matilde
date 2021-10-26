using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.ScciResource;
using UnicodeSrl.Scci.RTF;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUPrescrizioniProtocolloGenerate : frmBaseModale, Interfacce.IViewFormlModal
    {
        #region Declare

        private ucRichTextBox _ucRichTextBox = null;

        private bool _bFirmaDigitale = false;

        private Dictionary<string, byte[]> oIcone = new Dictionary<string, byte[]>();
        private Dictionary<string, byte[]> oIcone2 = new Dictionary<string, byte[]>();

        private List<string> _idGenerati = null;

        #endregion

        public frmPUPrescrizioniProtocolloGenerate()
        {
            InitializeComponent();
        }

                                                private string getTestoToTestoRtf(string testo)
        {

            RtfFiles rtf = new RtfFiles();
            string rtfAnteprima = "";

            try
            {

                System.Drawing.Font f = rtf.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), false, false);
                rtfAnteprima = rtf.initRtf(f);
                rtfAnteprima = rtf.appendRtfText(rtfAnteprima, testo, f);
                
            }
            catch (Exception)
            {

            }

            return rtfAnteprima;

        }

        #region Interface

        public new void Carica()
        {
            try
            {

                PulsanteAvantiTesto = "VALIDA TUTTO";
                PulsanteIndietroTesto = "ELIMINA TUTTO";

                Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                Icon = Risorse.GetIconFromResource(Risorse.GC_PRESCRIZIONE_16);

                try
                {
                    _idGenerati = (List<string>)this.CustomParamaters;
                }
                catch
                {
                    _idGenerati = new List<string>();
                }

                InizializzaUltraGridLayout();
                CaricaGriglia();

                ShowDialog();

            }
            catch (Exception)
            {
                ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        #endregion

        #region Ultragrid

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref ucEasyGrid);

                ucEasyGrid.DisplayLayout.ViewStyle = ViewStyle.MultiBand;
                ucEasyGrid.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;

                ucEasyGrid.DisplayLayout.Override.RowSpacingBefore = 3;
                ucEasyGrid.DisplayLayout.Override.RowSizing = RowSizing.AutoFree;
                ucEasyGrid.DisplayLayout.Override.RowSizingAutoMaxLines = 2;

                ucEasyGrid.DisplayLayout.Override.CellClickAction = CellClickAction.EditAndSelectText;
                ucEasyGrid.DisplayLayout.GroupByBox.Hidden = true;
                ucEasyGrid.DisplayLayout.InterBandSpacing = 10;
                ucEasyGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", Name);
            }
        }

        private void CaricaGriglia()
        {

            try
            {

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.ParametroRipetibile.Add("IDPrescrizione", _idGenerati.ToArray());

                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovProtocolliPrescrizioni", spcoll);

                DataTable dtEditPrescrizioni = ds.Tables[0].Copy();
                dtEditPrescrizioni.TableName = "Prescrizioni";
                DataTable dtEditPrescrizioniTempi = ds.Tables[1].Copy();
                dtEditPrescrizioniTempi.TableName = "PrescrizioniTempi";

                                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEditPrescrizioni.Columns.Add(colsp);

                                foreach (DataColumn dcCol in dtEditPrescrizioni.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("TEMPIDAVALIDARE") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOTASKIFERMIERISTICI") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCOPIA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICONA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("ICONA2") == 0)

                        dcCol.ReadOnly = false;
                }

                                foreach (DataColumn dcCol in dtEditPrescrizioniTempi.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSODAVALIDARE") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOANNULLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCOPIA") == 0)
                        dcCol.ReadOnly = false;


                    if (dcCol.ColumnName.ToUpper() == "DescrUtente".ToUpper() || dcCol.ColumnName.ToUpper() == "DescrUtenteValidazione".ToUpper())
                    {
                        dcCol.MaxLength = 250;
                    }

                }

                if (ucEasyGrid.DisplayLayout != null)
                {
                    ucEasyGrid.DataSource = null;

                                        DataSet dsfinal = new DataSet();
                    dsfinal.Tables.Add(dtEditPrescrizioni);
                    dsfinal.Tables.Add(dtEditPrescrizioniTempi);
                    DataRelation dr = new DataRelation("PrescrizioniTempi", dsfinal.Tables["Prescrizioni"].Columns["ID"], dsfinal.Tables["PrescrizioniTempi"].Columns["IDPrescrizione"], false);
                    dsfinal.Relations.Add(dr);

                                        ucEasyGrid.DataSource = dsfinal;
                    ucEasyGrid.Refresh();
                    ucEasyGrid.Rows.ExpandAll(true);
                    ucEasyGrid.Focus();
                    ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);

                    if (ucEasyGrid.Rows.Count > 0)
                        ucEasyGrid.ActiveRow = ucEasyGrid.Rows[0];
                    else
                        ucEasyGrid.ActiveRow = null;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", Name);
            }
        }

        #endregion

        #region Events Form

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall) * 3;

                Graphics g = CreateGraphics();
                int refBtnWidth = (CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 3);
                g.Dispose();
                g = null;

                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;

                e.Layout.Bands[1].HeaderVisible = false;
                e.Layout.Bands[1].ColHeadersVisible = false;
                e.Layout.Bands[1].RowLayoutStyle = RowLayoutStyle.GroupLayout;

                #region formattazione band 0 di testata prescrizioni

                #region ciclo sulle colonne

                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {
                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {

                            case "DataInizio":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 6;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescrUtente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 6;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.RowLayoutColumnInfo.WeightY = 1;   
                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;


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
                                    oCol.MaxWidth = refWidth + 10;
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

                            case "DescrTipoPrescrizione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 6;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;



                                break;

                            case "AnteprimaRTF":
                                oCol.Hidden = false;

                                RichTextEditor a = new RichTextEditor();
                                oCol.Editor = a;
                                oCol.CellActivation = Activation.ActivateOnly;
                                oCol.CellClickAction = CellClickAction.CellSelect;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = ucEasyGrid.Width - (refWidth * 18) - Convert.ToInt32(refBtnWidth * 2.75) - 40;
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

                            case "DescrStatoPrescrizione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
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

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                                                                                                                                                                                                                                                                                                                    
                                                                                                                                                                                                                                                            
                                                                                                                
                                                        case CoreStatics.C_COL_SPAZIO:
                                oCol.Hidden = false;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall);
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1);
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

                #endregion

                #region colonne icone e pulsanti

                                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_ICO_STATO + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_ICO_STATO + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
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
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
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
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
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
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
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
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 11;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                #endregion

                #endregion

                #region formattazione band 1 di dettaglio tempi

                #region ciclo sulle colonne

                foreach (UltraGridColumn oCol in e.Layout.Bands[1].Columns)
                {

                    try
                    {
                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {
                            case "DescrTempo":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (int)(refWidth * 8);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "PosologiaTXT":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - (refWidth * 9) - Convert.ToInt32(refBtnWidth * 3.75) - 13;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case CoreStatics.C_COL_SPAZIO:
                                oCol.Hidden = false;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall);
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1);
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

                            default:
                                oCol.Hidden = true;
                                break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                #endregion

                #region colonne icone e pulsanti

                if (!e.Layout.Bands[1].Columns.Exists(CoreStatics.C_COL_ICO_STATO + "_tempo"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[1].Columns.Add(CoreStatics.C_COL_ICO_STATO + @"_tempo");
                    colEdit.Hidden = false;

                                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;

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
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }

                if (!e.Layout.Bands[1].Columns.Exists(CoreStatics.C_COL_ICO_STATO + @"_SP" + @"_tempo"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[1].Columns.Add(CoreStatics.C_COL_ICO_STATO + @"_SP" + "_tempo");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
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
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }

                if (!e.Layout.Bands[1].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + "_tempo"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[1].Columns.Add(CoreStatics.C_COL_BTN_EDIT + "_tempo");
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
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }
                if (!e.Layout.Bands[1].Columns.Exists(CoreStatics.C_COL_BTN_MENU + @"_SP" + "_tempo"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[1].Columns.Add(CoreStatics.C_COL_BTN_MENU + @"_SP" + "_tempo");
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
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 6;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }

                if (!e.Layout.Bands[1].Columns.Exists(CoreStatics.C_COL_BTN_DEL + "_tempo"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[1].Columns.Add(CoreStatics.C_COL_BTN_DEL + "_tempo");
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
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 7;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }
                if (!e.Layout.Bands[1].Columns.Exists(CoreStatics.C_COL_BTN_DEL + @"_SP" + "_tempo"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[1].Columns.Add(CoreStatics.C_COL_BTN_DEL + @"_SP" + "_tempo");
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
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 8;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeLayout", Name);
            }
        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {

                if (e.Row.Cells.Exists("Icona") == true)
                {
                    if (oIcone.ContainsKey(e.Row.Cells["CodViaSomministrazione"].Value.ToString()) == false)
                    {
                        oIcone.Add(e.Row.Cells["CodViaSomministrazione"].Value.ToString(), DBUtils.getIconaByViaSomministrazione(e.Row.Cells["CodViaSomministrazione"].Value.ToString()));
                    }
                    e.Row.Cells["Icona"].Value = oIcone[e.Row.Cells["CodViaSomministrazione"].Value.ToString()];
                    e.Row.Update();
                }

                if (e.Row.Cells.Exists("ColoreStatoPrescrizione") == true)
                {

                    if (e.Row.Cells["ColoreStatoPrescrizione"].Value != DBNull.Value)
                    {
                        e.Row.Appearance.BackColor = CoreStatics.GetColorFromString(e.Row.Cells["ColoreStatoPrescrizione"].Value.ToString());
                    }
                }

                if (e.Row.Cells.Exists("Icona2") == true)
                {
                    if (oIcone2.ContainsKey(e.Row.Cells["CodStatoPrescrizione"].Value.ToString()) == false)
                    {
                        oIcone2.Add(e.Row.Cells["CodStatoPrescrizione"].Value.ToString(), DBUtils.getIconaBySelStatoPrescrizione(e.Row.Cells["CodStatoPrescrizione"].Value.ToString()));
                    }
                    if (oIcone2[e.Row.Cells["CodStatoPrescrizione"].Value.ToString()] != null)
                    {
                        e.Row.Cells["Icona2"].Value = oIcone2[e.Row.Cells["CodStatoPrescrizione"].Value.ToString()];
                        e.Row.Update();
                    }
                }                

                foreach (UltraGridCell ocell in e.Row.Cells)
                {
                    if (ocell.Column.Key == CoreStatics.C_COL_ICO_STATO && ocell.Row.Cells["TempiDaValidare"].Value.ToString() == "1")
                        if (_bFirmaDigitale)
                            ocell.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMA_32);
                        else
                            ocell.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_FIRMA_32);                    
                }

                if (e.Row.Cells.Exists("IDPrescrizione") == true)
                {
                                        if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                                                                        e.Row.Cells[CoreStatics.C_COL_BTN_EDIT + "_tempo"].Hidden = true;
                        e.Row.Cells[CoreStatics.C_COL_BTN_DEL + "_tempo"].Hidden = true;
                    }
                    else
                    {
                                                                        e.Row.Cells[CoreStatics.C_COL_BTN_EDIT + "_tempo"].Hidden = e.Row.Cells["PermessoModifica"].Text == "1" ? false : true;
                        e.Row.Cells[CoreStatics.C_COL_BTN_DEL + "_tempo"].Hidden = e.Row.Cells["PermessoCancella"].Text == "1" ? false : true;
                    }
                }
                else
                {
                                        if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                        e.Row.Cells[CoreStatics.C_COL_BTN_EDIT].Hidden = true;
                        e.Row.Cells[CoreStatics.C_COL_BTN_DEL].Hidden = true;
                                                                    }
                    else
                    {
                        e.Row.Cells[CoreStatics.C_COL_BTN_EDIT].Hidden = e.Row.Cells["PermessoModifica"].Text == "1" ? false : true;
                        e.Row.Cells[CoreStatics.C_COL_BTN_DEL].Hidden = e.Row.Cells["PermessoCancella"].Text == "1" ? false : true;
                                                                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", Name);
            }
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {

            try
            {
                Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                switch (e.Cell.Column.Key)
                {

                    case "AnteprimaTXT":
                    case "AnteprimaRTF":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerMain);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Row.Cells["AnteprimaRTF"].Text);

                        this.UltraPopupControlContainerMain.Show(this.ucEasyGrid, oPoint);
                        break;
                    case "PosologiaTXT":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerMain);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Row.Cells["PosologiaRTF"].Text);

                                                if (e.Cell.Row.Cells["PosologiaRTF"].Text == string.Empty)
                        {
                            string sTesto = string.Empty;
                            sTesto = e.Cell.Row.Cells["PosologiaTXT"].Text.ToString();

                            _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(getTestoToTestoRtf(sTesto));
                        }
                        this.UltraPopupControlContainerMain.Show(this.ucEasyGrid, oPoint);
                        break;
                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCell", Name);
            }
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {

            switch (e.Cell.Column.Key)
            {

                case CoreStatics.C_COL_BTN_EDIT:
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(ucEasyGrid.ActiveRow.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);

                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizione, true, false) == DialogResult.OK)
                    {
                        CaricaGriglia();
                        if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                        {
                            CoreStatics.SelezionaRigaInGriglia(ref ucEasyGrid, "ID", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione);
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                        }
                    }
                    break;

                case CoreStatics.C_COL_BTN_DEL:
                                        if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "la prescrizione selezionata ?", "Cancellazione Prescrizioni", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MovPrescrizione movpr = new MovPrescrizione(ucEasyGrid.ActiveRow.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);

                                                                        bool bContinua = true;

                        foreach (MovPrescrizioneTempi movprt in movpr.Elementi)
                        {

                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = movprt;

                                                        Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.PRT_CANCELLA_PRIMA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                            if (oRispostaElaboraPrima.Successo)
                            {
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.CA.ToString();
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Cancella();
                            }
                            else
                            {
                                bContinua = false;
                                break;
                            }

                        }

                                                if (bContinua)
                        {
                            movpr.CodStatoPrescrizione = @"CA";
                            movpr.Azione = EnumAzioni.CAN;
                            if (movpr.Salva())
                            {
                                                                _idGenerati.Remove(ucEasyGrid.ActiveRow.Cells["ID"].Text);
                                CaricaGriglia();
                            }
                        }
                        else
                        {
                            easyStatics.EasyMessageBox("Non è possibile cancella la prescrizione selezionata a causa di un errore presentatosi durante la cancellazione della tempistica.", "Errore Cancellazione Prescrizioni", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }

                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                    }
                    break;

                case CoreStatics.C_COL_BTN_EDIT + "_tempo":
                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(ucEasyGrid.ActiveRow.Cells["IDPrescrizione"].Text, CoreStatics.CoreApplication.Ambiente);

                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata =
                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi.Find(MovPrescrizioneTempi => MovPrescrizioneTempi.IDPrescrizioneTempi == this.ucEasyGrid.ActiveRow.Cells["ID"].Text);

                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizioneTempi, true, false) == DialogResult.OK)
                    {
                        CaricaGriglia();
                        if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata != null)
                        {
                            CoreStatics.SelezionaRigaInGriglia(ref ucEasyGrid, "ID", CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.IDPrescrizioneTempi);
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                        }
                    }

                    break;

                case CoreStatics.C_COL_BTN_DEL + "_tempo":
                    if (easyStatics.EasyMessageBox("Confermi la CANCELLAZIONE della periodicità selezionata ?", "Cancellazione Tempi Prescrizioni", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.ImpostaCursore(enum_app_cursors.WaitCursor);

                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(ucEasyGrid.ActiveRow.Cells["IDPrescrizione"].Text, CoreStatics.CoreApplication.Ambiente);

                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata =
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi.Find(MovPrescrizioneTempi => MovPrescrizioneTempi.IDPrescrizioneTempi == this.ucEasyGrid.ActiveRow.Cells["ID"].Text);

                        bool bContinua = true;

                                                Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.PRT_CANCELLA_PRIMA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                        bContinua = oRispostaElaboraPrima.Successo;

                        if (bContinua)
                        {
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.CA.ToString();                             if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Cancella())
                            {
                                                                CaricaGriglia();
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                            }
                        }

                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;

                        this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                    }

                    break;
            }
        }

        private void frmPUPrescrizioniProtocolloGenerate_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {
                if (easyStatics.EasyMessageBox("Sei sicuro di voler VALIDARE" + Environment.NewLine + "tutti i tempi delle prescrizioni create ?", "Valida Tutte", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                    DataSet ds = (DataSet)this.ucEasyGrid.DataSource;

                    ucEasyProgressBar.Text = "Validazione in corso: [Value] / [Maximum]";
                    ucEasyProgressBar.Minimum = 0;
                    ucEasyProgressBar.Maximum = ds.Tables[1].Rows.Count;
                    ucEasyProgressBar.Value = 0;
                    ucEasyProgressBar.Visible = true;
                    TableLayoutPanelZoom.Refresh();

                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        ucEasyProgressBar.Value += 1;
                        ucEasyProgressBar.Refresh();

                        MovPrescrizioneTempi movp = new MovPrescrizioneTempi(row["ID"].ToString(), row["IDPrescrizione"].ToString(), CoreStatics.CoreApplication.Ambiente);
                        movp.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.VA.ToString();                         movp.Azione = EnumAzioni.VAL;

                        if (movp.Salva())
                            movp.CreaTaskInfermieristici(EnumCodSistema.PRF, Database.GetConfigTable(EnumConfigTable.TipoSchedaTaskDaPrescrizione), EnumTipoRegistrazione.A);

                        this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = movp;
                        Risposta oRispostaValidaDopo = new Risposta();
                        oRispostaValidaDopo.Successo = true;
                        oRispostaValidaDopo = PluginClientStatics.PluginClient(EnumPluginClient.PRT_VALIDA_DOPO_PU.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;

                        this.ImpostaCursore(enum_app_cursors.WaitCursor);
                    }

                    ImpostaCursore(enum_app_cursors.DefaultCursor);
                    ucEasyProgressBar.Visible = false;
                    TableLayoutPanelZoom.Refresh();

                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmPUPrescrizioniProtocolloGenerate_PulsanteAvantiClick", Name);
            }
        }

        private void frmPUPrescrizioniProtocolloGenerate_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {

            bool bContinua = true;

            try
            {
                if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "tutte le prescrizioni create ?", "Elimina Tutte", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                    DataSet ds = (DataSet)this.ucEasyGrid.DataSource;

                    ucEasyProgressBar.Text = "Eliminazione in corso: [Value] / [Maximum]";
                    ucEasyProgressBar.Minimum = 0;
                    ucEasyProgressBar.Maximum = ds.Tables[0].Rows.Count;
                    ucEasyProgressBar.Value = 0;
                    ucEasyProgressBar.Visible = true;
                    TableLayoutPanelZoom.Refresh();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        ucEasyProgressBar.Value += 1;
                        ucEasyProgressBar.Refresh();

                        MovPrescrizione movp = new MovPrescrizione(row["ID"].ToString(), EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);

                        foreach (MovPrescrizioneTempi movprt in movp.Elementi)
                        {
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = movprt;

                                                        Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.PRT_CANCELLA_PRIMA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                            if (oRispostaElaboraPrima.Successo)
                            {
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.CA.ToString();
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Cancella();
                            }
                            else
                            {
                                bContinua = false;
                                break;
                            }
                        }

                        this.ImpostaCursore(enum_app_cursors.WaitCursor);

                        if (bContinua)
                        {
                            movp.CodStatoPrescrizione = @"CA";
                            movp.Azione = EnumAzioni.CAN;
                            movp.Cancella();
                        }
                    }

                    this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                    ucEasyProgressBar.Visible = false;
                    TableLayoutPanelZoom.Refresh();

                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmPUPrescrizioniProtocolloGenerate_PulsanteIndietroClick", Name);
            }
        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.Focus();
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        #endregion
    }
}
