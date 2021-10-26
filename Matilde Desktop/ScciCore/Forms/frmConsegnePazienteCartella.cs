using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    public partial class frmConsegnePazienteCartella : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

        private ucRichTextBox _ucRichTextBox = null;
        private bool _runtimecheck = false;

        private enum enumRTFRender
        {
            showPopup = 0,
            resizeRow = 1,
            showScrollBars = 2,
            resizeRowAndPopup = 3
        }
        private enumRTFRender _rtfRender = enumRTFRender.showPopup;

        private Dictionary<int, byte[]> oIcone = new Dictionary<int, byte[]>();

        #endregion

        public frmConsegnePazienteCartella()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public new void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_CONSEGNEPAZIENTE_16);

                InizializzaControlli();
                InizializzaUltraGridLayout();
                VerificaSicurezza();
                InizializzaFiltri();

                CaricaGriglia(true);

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

        }

        #endregion

        #region PRIVATE

        private void InizializzaControlli()
        {

            try
            {

                this.ubAdd.Appearance.Image = Properties.Resources.Aggiungi_256;
                this.ubAdd.PercImageFill = 0.75F;
                this.ubAdd.ShortcutKey = Keys.Add;
                this.ubAdd.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

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
                this.ubAdd.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.ConsegneP_Inserisci);
            }
            catch (Exception)
            {
            }
        }

        private void InizializzaFiltri()
        {

            try
            {

                this.chkMostraVistati.Checked = false;
                this.chkMostraAnnullati.Checked = false;

                if (this.ucEasyTreeViewTipo.Nodes.Count > 0)
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                        {
                            oNode.CheckedState = CheckState.Checked;
                        }
                    }
                }

                this.drFiltro.Value = null;
                this.drFiltro.DateFuture = false;
                this.udteFiltroDA.Value = null;
                this.udteFiltroA.Value = null;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaFiltri", this.Name);
            }

        }

        private void CaricaGriglia(bool datiEstesi)
        {

            try
            {

                CoreStatics.SetContesto(EnumEntita.CSP, null);

                DataSet ds = null;

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);

                                if (this.chkMostraVistati.Checked == false)
                {
                    op.Parametro.Add("NascondiVistati", "1");
                }

                                List<string> lstStati = new List<string>();
                lstStati.Add("IC");
                if (this.chkMostraAnnullati.Checked)
                {
                    lstStati.Add("AN");
                }
                op.ParametroRipetibile.Add("CodStatoConsegnaPaziente", lstStati.ToArray());

                                Dictionary<string, string> listatipo = new Dictionary<string, string>();
                                if (this.ucEasyTreeViewTipo.Nodes.Count > 0
                    && this.ucEasyTreeViewTipo.Nodes.Exists(CoreStatics.GC_TUTTI)
                    && this.ucEasyTreeViewTipo.Nodes[CoreStatics.GC_TUTTI].CheckedState == CheckState.Unchecked)
                {
                                        foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                        {
                            listatipo.Add(oNode.Key, oNode.Text);
                        }
                    }
                    string[] codtipo = listatipo.Keys.ToArray();
                    op.ParametroRipetibile.Add("CodTipoConsegnaPaziente", codtipo);
                }

                if (this.udteFiltroDA.Value != null)
                {
                    op.Parametro.Add("DataInizio", ((DateTime)this.udteFiltroDA.Value).ToString("dd/MM/yyyy HH:mm").Replace(".", ":"));
                }
                if (this.udteFiltroA.Value != null)
                {
                    op.Parametro.Add("DataFine", ((DateTime)this.udteFiltroA.Value).ToString("dd/MM/yyyy HH:mm").Replace(".", ":"));
                }

                                op.Parametro.Add("DatiEstesi", (datiEstesi ? "1" : "0"));

                op.TimeStamp.CodEntita = EnumEntita.CSP.ToString();                 op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString(); 
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                FwDataParametersList procParams = new FwDataParametersList();
                procParams.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                {
                    ds = conn.Query<DataSet>("MSP_SelMovConsegnePazienteTrasversale", procParams, CommandType.StoredProcedure);
                }

                DataTable dtEdit = ds.Tables[0].Copy();
                                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSO") >= 0
                     || dcCol.ColumnName.ToUpper().IndexOf("ICON") >= 0
                     || dcCol.ColumnName.ToUpper().IndexOf("ANTEPRIMARTF") == 0) dcCol.ReadOnly = false;
                }

                this.ucEasyGrid.DataSource = null;
                this.ucEasyGrid.DataSource = dtEdit;
                this.ucEasyGrid.Refresh();

                if (datiEstesi)
                {
                    CaricaDatiEstesi(ref ds);                    
                }
                
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Name);
            }

        }

        private void CaricaDatiEstesi(ref DataSet datasetmovimenti)
        {

            try
            {

                                this.ucEasyTreeViewTipo.Nodes.Clear();
                Infragistics.Win.UltraWinTree.UltraTreeNode oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Tipo");
                oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                oNodeRoot.CheckedState = CheckState.Checked;
                foreach (DataRow oDr in datasetmovimenti.Tables[1].Rows)
                {
                    Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                    oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                    oNode.CheckedState = CheckState.Checked;
                    oNodeRoot.Nodes.Add(oNode);
                }
                this.ucEasyTreeViewTipo.Nodes.Add(oNodeRoot);
                this.ucEasyTreeViewTipo.ExpandAll();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaDatiEstesi()", this.Name);
            }

        }

        private void InitializeRow(UltraGridRow eRow)
        {

            try
            {

                if (eRow.Cells.Exists("Icona") == true && eRow.Cells["IDIcona"].Value.ToString() != "")
                {
                    if (oIcone.ContainsKey(Convert.ToInt32(eRow.Cells["IDIcona"].Value)) == false)
                    {
                        oIcone.Add(Convert.ToInt32(eRow.Cells["IDIcona"].Value), CoreStatics.GetImageForGrid(Convert.ToInt32(eRow.Cells["IDIcona"].Value), 256));
                    }
                    eRow.Cells["Icona"].Value = oIcone[Convert.ToInt32(eRow.Cells["IDIcona"].Value)];
                    eRow.Update();
                }

                                if (eRow.Cells["PermessoModifica"].Text == "0")
                {
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].CellDisplayStyle = CellDisplayStyle.PlainText;
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Appearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                    string sStato = "";

                    eRow.Cells[CoreStatics.C_COL_BTN_EDIT].Value = sStato;
                }

                                if (eRow.Cells["PermessoAnnulla"].Text == "0" && eRow.Cells["PermessoCancella"].Text == "0")
                {
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Hidden = true;
                }
                else
                {
                    if (eRow.Cells["PermessoAnnulla"].Text == "1")
                    {
                        eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Band.Columns[CoreStatics.C_COL_BTN_ANNULLA].CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
                        eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].ButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
                    }
                    if (eRow.Cells["PermessoCancella"].Text == "1")
                    {
                        eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Band.Columns[CoreStatics.C_COL_BTN_ANNULLA].CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_CANCELLATONDO_32);
                        eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].ButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_CANCELLATONDO_32);
                    }
                }

                                if (eRow.Cells["PermessoVisto"].Text == "0")
                {
                    eRow.Cells[CoreStatics.C_COL_BTN_VISTO].CellDisplayStyle = CellDisplayStyle.PlainText;
                    eRow.Cells[CoreStatics.C_COL_BTN_VISTO].Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                    eRow.Cells[CoreStatics.C_COL_BTN_VISTO].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                    eRow.Cells[CoreStatics.C_COL_BTN_VISTO].Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
                    eRow.Cells[CoreStatics.C_COL_BTN_VISTO].Appearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                    eRow.Cells[CoreStatics.C_COL_BTN_VISTO].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                    string sStato = "";

                    eRow.Cells[CoreStatics.C_COL_BTN_VISTO].Value = sStato;
                }

            }
            catch (Exception ex)
            {
            }

        }

        #endregion

        #region Events Form

        private void frmConsegnePazienteCartella_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            CoreStatics.SetContesto(EnumEntita.CSP, null);
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmConsegnePazienteCartella_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            CoreStatics.SetContesto(EnumEntita.CSP, null);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        #endregion

        #region Events

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.CoreApplication.IDConsegnaSelezionata = "";
            CoreStatics.SetContesto(EnumEntita.CSP, this.ucEasyGrid.ActiveRow);
            CoreStatics.CoreApplication.IDConsegnaSelezionata = this.ucEasyGrid.ActiveRow.Cells["ID"].Text;
        }

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refIcoWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 3);
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
                                oCol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Top;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                try
                                {
                                    oCol.LockedWidth = true;
                                    oCol.MaxWidth = refIcoWidth;
                                    oCol.Width = oCol.MaxWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 4;

                                break;

                            case "DataInserimento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refIcoWidth * 2);
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

                            case "DescrUtenteUltimaModifica":
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
                                    oCol.MaxWidth = Convert.ToInt32(refIcoWidth * 2);
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
                                oCol.CellAppearance.ForeColor = Color.Black;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refIcoWidth * 2);
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

                            case "DescrUA":
                                oCol.Hidden = false;

                                                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = true;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refIcoWidth * 2) - Convert.ToInt32(refBtnWidth * 6) - 30;
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

                            case "DescrRuoli":
                                oCol.Hidden = false;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = true;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refIcoWidth * 2);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 3;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "AnteprimaRTF":
                                oCol.Hidden = false;

                                RichTextEditor a = new RichTextEditor();
                                a.ScrollBars = RichTextBoxScrollBars.Both;
                                oCol.Editor = a;
                                switch (_rtfRender)
                                {
                                    case enumRTFRender.showScrollBars:
                                        oCol.CellClickAction = CellClickAction.EditAndSelectText;
                                        break;
                                    case enumRTFRender.resizeRow:
                                    case enumRTFRender.resizeRowAndPopup:
                                    case enumRTFRender.showPopup:
                                    default:
                                        oCol.CellClickAction = CellClickAction.CellSelect;
                                        break;
                                }
                                                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = true;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refIcoWidth * 2) - Convert.ToInt32(refBtnWidth * 6) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "Info":
                                oCol.Hidden = false;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = true;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refIcoWidth * 2) - Convert.ToInt32(refBtnWidth * 6) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 3;
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
                                if (!e.Layout.Bands[0].Columns.Exists("_Spazio1_"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add("_Spazio1_");
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
                    colEdit.RowLayoutColumnInfo.OriginX = 3;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 4;
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
                    colEdit.RowLayoutColumnInfo.SpanY = 4;
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
                    colEdit.CellButtonAppearance.Image = Properties.Resources.Modifica_32;


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
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }

                                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ANNULLA + @"_SP");
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
                    colEdit.RowLayoutColumnInfo.SpanY = 4;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_ANNULLA))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_ANNULLA);
                    colEdit.Hidden = false;

                                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);


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
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }

                                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VISTO + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VISTO + @"_SP");
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
                    colEdit.RowLayoutColumnInfo.OriginX = 8;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 4;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VISTO))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VISTO);
                    colEdit.Hidden = false;

                                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMA_32);

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


                    colEdit.RowLayoutColumnInfo.OriginX = 9;
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
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.1);
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
                    colEdit.RowLayoutColumnInfo.SpanY = 4;
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

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "AnteprimaRTF":
                        if (_rtfRender == enumRTFRender.showPopup || _rtfRender == enumRTFRender.resizeRowAndPopup)
                        {
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                            _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text);

                            Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                            Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                            this.UltraPopupControlContainer.Show(this.ucEasyGrid, oPoint);
                        }
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

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_EDIT:
                        if (e.Cell.Row.Cells["PermessoModifica"].Text == "1")
                        {
                                                                                                                MovConsegnaPaziente movcsp = new MovConsegnaPaziente(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                                        if (e.Cell.Row.Cells["PermessoAnnulla"].Text == "0")
                            {
                                CoreStatics.SetContesto(EnumEntita.CSP, this.ucEasyGrid.ActiveRow);
                                                                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata = movcsp;
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingConsegnaPaziente) == DialogResult.OK)
                                {
                                    CaricaGriglia(false);
                                    if (CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata != null)
                                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.IDMovConsegnaPaziente);
                                }
                            }
                            else if (e.Cell.Row.Cells["PermessoAnnulla"].Text == "1")
                            {
                                                                string sMsg = $"ATTENZIONE!!!\n\nI seguenti ruoli:\n" +
                                                $"{movcsp.ListaRuoliVistati()}\n" +
                                                $"hanno vistato la consegna.\n\nVuoi annullare questa consegna e crearne una nuova ?";
                                if (easyStatics.EasyMessageBox(sMsg, "Annullamento e Creazione nuova Consegna Paziente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                                                                                                                                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata = new MovConsegnaPaziente(CoreStatics.CoreApplication.Ambiente);
                                    CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CopiaDaOrigine(ref movcsp);
                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingConsegnaPaziente) == DialogResult.OK)
                                    {
                                                                                movcsp.Annulla();
                                        CaricaGriglia(false);
                                        if (CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata != null)
                                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.IDMovConsegnaPaziente);
                                    }
                                }
                            }
                                                        CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata = null;
                            movcsp = null;
                        }
                        break;

                    case CoreStatics.C_COL_BTN_ANNULLA:
                        if (e.Cell.Row.Cells["PermessoAnnulla"].Text == "1")
                        {
                                                        MovConsegnaPaziente movcsp = new MovConsegnaPaziente(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                            string sMsg = $"ATTENZIONE!!!\n\nI seguenti ruoli:\n" +
                                            $"{movcsp.ListaRuoliVistati()}\n" +
                                            $"hanno vistato la consegna.\n\nVuoi annullarla ugualmente ?";
                            if (easyStatics.EasyMessageBox(sMsg, "Annullamento Consegna Paziente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                if (movcsp.Annulla())
                                {
                                    CaricaGriglia(false);
                                }
                            }
                            movcsp = null;
                        }
                        else if (e.Cell.Row.Cells["PermessoCancella"].Text == "1")
                        {
                                                        string sMsg = $"Sei sicuro di voler CANCELLARE\nla consegna {e.Cell.Row.Cells["DescrTipoConsegnaPaziente"].Text}\n" +
                                            $"del paziente {e.Cell.Row.Cells["DescrPaziente"].Text}\n" +
                                            $"del {e.Cell.Row.Cells["DataInserimento"].Text} ?";
                            if (easyStatics.EasyMessageBox(sMsg, "Cancellazione Consegna Paziente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                MovConsegnaPaziente movcsp = new MovConsegnaPaziente(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                if (movcsp.Cancella())
                                {
                                    CaricaGriglia(false);
                                }
                                movcsp = null;
                            }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_VISTO:
                        if (e.Cell.Row.Cells["PermessoVisto"].Text == "1")
                        {
                                                        string sMsg = $"Sei sicuro di voler VISTARE\nla consegna {e.Cell.Row.Cells["DescrTipoConsegnaPaziente"].Text}\n" +
                                            $"del paziente {e.Cell.Row.Cells["DescrPaziente"].Text}\n" +
                                            $"del {e.Cell.Row.Cells["DataInserimento"].Text} ?";
                            if (easyStatics.EasyMessageBox(sMsg, "Visione Consegna Paziente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                MovConsegnaPaziente movcsp = new MovConsegnaPaziente(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                if (movcsp.Vista(CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice))
                                {
                                    CaricaGriglia(false);
                                }
                                movcsp = null;
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
            }

        }

        private void ubAdd_Click(object sender, EventArgs e)
        {

            try
            {

                this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);

                                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata = new MovConsegnaPaziente(CoreStatics.CoreApplication.Ambiente);
                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.Azione = EnumAzioni.INS;
                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.IDEpisodio = CoreStatics.CoreApplication.Episodio.ID;
                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.CodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.AggiungiRuoloCreazione(CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoConsegnaPaziente) == DialogResult.OK)
                {
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingConsegnaPaziente) == DialogResult.OK)
                    {
                        CaricaGriglia(false);
                        if (CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata != null)
                        {
                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata.IDMovConsegnaPaziente);
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
                CoreStatics.CoreApplication.MovConsegnaPazienteSelezionata = null;
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            }

        }

        private void chkMostraVistati_CheckedValueChanged(object sender, EventArgs e)
        {
            CaricaGriglia(false);
        }

        private void chkMostraAnnullati_CheckedValueChanged(object sender, EventArgs e)
        {
            CaricaGriglia(false);
        }

        private void ucEasyTreeViewTipo_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            if (!_runtimecheck)
            {
                _runtimecheck = true;
                try
                {
                    if (e.TreeNode.Key == CoreStatics.GC_TUTTI)
                    {
                        foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in ((ucEasyTreeView)sender).Nodes[CoreStatics.GC_TUTTI].Nodes)
                        {
                            if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                            {
                                oNode.CheckedState = e.TreeNode.CheckedState;
                            }
                        }
                    }
                    else
                    {
                        if (e.TreeNode.CheckedState != CheckState.Checked)
                        {
                                                        ((ucEasyTreeView)sender).Nodes[CoreStatics.GC_TUTTI].CheckedState = CheckState.Unchecked;
                        }
                        else
                        {
                                                        bool bCheckTutti = true;

                            foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in ((ucEasyTreeView)sender).Nodes[CoreStatics.GC_TUTTI].Nodes)
                            {
                                if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                                {
                                    if (oNode.CheckedState != CheckState.Checked) bCheckTutti = false;
                                }
                            }

                            if (bCheckTutti)
                                ((ucEasyTreeView)sender).Nodes[CoreStatics.GC_TUTTI].CheckedState = CheckState.Checked;
                        }
                    }
                    CaricaGriglia(false);
                }
                catch (Exception ex)
                {
                    UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                }
                finally
                {
                    _runtimecheck = false;
                }
            }

        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
            CaricaGriglia(false);
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
