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
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.ScciCore
{
    public partial class ucConsulenzeTrasversali : UserControl, Interfacce.IViewUserControlMiddle
    {

        public ucConsulenzeTrasversali()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
        }

        #region DECLARE

        private enum enumRTFRender
        {
            showPopup = 0,
            resizeRow = 1,
            showScrollBars = 2,
            resizeRowAndPopup = 3
        }
        private enumRTFRender _rtfRender = enumRTFRender.showPopup;

        private UserControl _ucc = null;
        private ucRichTextBox _ucRichTextBox = null;

        Graphics g = null;

        #endregion

        #region INTERFACCIA

        public void Aggiorna()
        {

            if (this.IsDisposed == false)
            {

                CoreStatics.SetNavigazione(false);

                CaricaDati();

                CoreStatics.SetNavigazione(true);

            }

        }

        public void Carica()
        {

            try
            {

                InizializzaControlli();
                InizializzaFiltri();

                CaricaDati();

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

                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);

                this.ubApplicaFiltro.PercImageFill = 0.75F;
                this.ubApplicaFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);

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

                    this.drFiltro.Value = ucEasyDateRange.C_RNG_30G;

                    this.utxtFiltroContenuto.Text = "";

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("DatiEstesi", "1");

                    op.TimeStamp.CodEntita = EnumEntita.DCL.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataSet dsTipi = Database.GetDatasetStoredProc("MSP_SelMovConsulenzeTrasversali", spcoll);

                    this.ucEasyTreeViewFiltroTipo.Nodes.Clear();
                    Infragistics.Win.UltraWinTree.UltraTreeNode oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Tipo");
                    oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                    oNodeRoot.CheckedState = CheckState.Checked;
                    foreach (DataRow oDr in dsTipi.Tables[1].Rows)
                    {
                        Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                        oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                        oNode.CheckedState = CheckState.Checked;
                        oNodeRoot.Nodes.Add(oNode);
                    }
                    this.ucEasyTreeViewFiltroTipo.Nodes.Add(oNodeRoot);
                    this.ucEasyTreeViewFiltroTipo.ExpandAll();

                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("DatiEstesi", "0");

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataSet dsSt = Database.GetDatasetStoredProc("MSP_SelStatoDiario", spcoll);

                    this.ucEasyGridFiltroStato.DataSource = CoreStatics.AggiungiTuttiDataTable(dsSt.Tables[0], false);
                    this.ucEasyGridFiltroStato.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Stato";
                    this.ucEasyGridFiltroStato.Refresh();
                    if (this.ucEasyGridFiltroStato.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroStato.Selected.Rows.Clear();
                        this.ucEasyGridFiltroStato.ActiveRow = null;
                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridFiltroStato, this.ucEasyGridFiltroStato.DisplayLayout.Bands[0].Columns[0].Key, CoreStatics.GC_TUTTI);
                    }

                    this.ucEasyGridFiltroUA.DataSource = CoreStatics.AggiungiTuttiDataTable(dsTipi.Tables[2], false);
                    this.ucEasyGridFiltroUA.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Struttura";
                    this.ucEasyGridFiltroUA.Refresh();
                    if (this.ucEasyGridFiltroUA.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroUA.Selected.Rows.Clear();
                        this.ucEasyGridFiltroUA.ActiveRow = null;
                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridFiltroUA, this.ucEasyGridFiltroUA.DisplayLayout.Bands[0].Columns[0].Key, CoreStatics.GC_TUTTI);
                    }

                }
                catch (Exception)
                {
                }

            }

        }

        private void CaricaDati()
        {

            try
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                DataSet dsDE = null; DataTable dtGriglia = null;

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("DatiEstesi", "0");


                if (this.udteFiltroDA.Value != null)
                {
                    op.Parametro.Add("DataInizio", CoreStatics.getDateTime((DateTime)this.udteFiltroDA.Value));
                }
                if (this.udteFiltroA.Value != null)
                {
                    op.Parametro.Add("DataFine", CoreStatics.getDateTime((DateTime)this.udteFiltroA.Value));
                }


                if (this.ucEasyGridFiltroStato.ActiveRow != null && this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodStatoDiario", this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text);
                }

                if (this.ucEasyGridFiltroUA.ActiveRow != null && this.ucEasyGridFiltroUA.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodUA", this.ucEasyGridFiltroUA.ActiveRow.Cells["Codice"].Text);
                }

                Dictionary<string, string> listatipo = new Dictionary<string, string>();
                foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                {
                    if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                    {
                        listatipo.Add(oNode.Key, oNode.Text);
                    }
                }
                string[] codtipo = listatipo.Keys.ToArray();
                op.ParametroRipetibile.Add("CodTipoVoceDiario", codtipo);

                if (this.utxtFiltroContenuto.Text.Trim() != "")
                {
                    op.Parametro.Add("Descrizione", this.utxtFiltroContenuto.Text);
                }


                if (this.utxtRicerca.Text.Trim() != "")
                {
                    string filtrogenerico = string.Empty;

                    string[] ricerche = this.utxtRicerca.Text.Split(' ');
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

                    if (filtrogenerico != string.Empty)
                        op.Parametro.Add("FiltroGenerico", filtrogenerico);
                }


                op.TimeStamp.CodEntita = EnumEntita.DCL.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                dsDE = Database.GetDatasetStoredProc("MSP_SelMovConsulenzeTrasversali", spcoll);
                dtGriglia = dsDE.Tables[0];

                DataTable dtEdit = dtGriglia.Copy();
                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSO") == 0
                     || dcCol.ColumnName.ToUpper().IndexOf("ICON") >= 0
                     || dcCol.ColumnName.ToUpper().IndexOf("ANTEPRIMARTF") == 0) dcCol.ReadOnly = false;
                }

                if (g == null) g = this.CreateGraphics();

                if (_rtfRender == enumRTFRender.resizeRow || _rtfRender == enumRTFRender.resizeRowAndPopup)
                {
                    this.ucEasyGrid.ColonnaRTFResize = "AnteprimaRTF";
                    this.ucEasyGrid.ColonnaRTFControlloContenuto = true;
                    int iFattore = 20;
                    if (UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF) != "")
                    {
                        try
                        {
                            iFattore = CoreStatics.PointToPixel(DrawingProcs.getFontFromString(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF)).SizeInPoints, g.DpiY);
                            g.Dispose();
                            g = null;
                        }
                        catch (Exception)
                        {
                            iFattore = 20;
                        }
                    }
                    this.ucEasyGrid.FattoreRidimensionamentoRTF = iFattore;
                }

                dtEdit.DefaultView.Sort = "DescrPaziente ASC, DataEvento ASC";
                this.ucEasyGrid.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGrid.DataSource = dtEdit.DefaultView;
                this.ucEasyGrid.Refresh();

                CoreStatics.ImpostaGroupByGriglia(ref this.ucEasyGrid, ref g, "DescrPaziente");
                this.ucEasyGrid.PerformLayout();

                if (this.ucEasyGrid.Rows.Count > 0 && this.ucEasyGrid.Rows[0].ChildBands.Count > 0 && this.ucEasyGrid.Rows[0].ChildBands[0].Rows.Count > 0)
                {
                    this.ucEasyGrid.Rows[0].ChildBands[0].Rows[0].Activate();
                    this.ucEasyGrid.Selected.Rows.Clear();
                    this.ucEasyGrid.Selected.Rows.Add(this.ucEasyGrid.Rows[0].ChildBands[0].Rows[0]);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaDati()", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }
        }

        private void setNavigazione(bool enable)
        {

            try
            {

                CoreStatics.SetNavigazione(enable);
                this.ucEasyGrid.Enabled = enable;
                this.ucEasyTableLayoutPanelFiltro1.Enabled = enable;

            }
            catch
            {
                CoreStatics.SetNavigazione(true);
            }

        }

        #endregion

        #region EVENTI

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

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare = null;
                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_EDIT:

                        if (e.Cell.Row.Cells["PermessoModifica"].Text == "1" && e.Cell.Row.Cells["PermessoModifica"].Text == "1")
                        {


                            string idOriginale = e.Cell.Row.Cells["ID"].Text;

                            MovDiarioClinico movdcorig = new MovDiarioClinico(idOriginale, CoreStatics.CoreApplication.Ambiente);
                            MovDiarioClinico movdccopia = movdcorig.Copia();
                            if (movdccopia != null)
                            {
                                if (easyStatics.EasyMessageBox(@"Confermi la modifica della voce di consulenza selezionata ?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                                {

                                    CoreStatics.CoreApplication.MovDiarioClinicoSelezionato = movdccopia;
                                    if (movdcorig.PermessoUAFirma == 1) CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare = movdcorig;
                                    List<string> customparameters = new List<string>();
                                    customparameters.Add("bloccafirma");

                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingVoceDiDiario, true, customparameters) == DialogResult.OK)
                                    {
                                        if (movdcorig.PermessoUAFirma != 1) movdcorig.Annulla();

                                        CaricaDati();
                                        this.ucEasyGrid.ActiveRow = null;
                                        RowsCollection gridrows = this.ucEasyGrid.Rows;
                                        if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato != null)
                                            CoreStatics.SelezionaRigaInGriglia(ref gridrows, "ID", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario);
                                    }

                                }
                            }
                        }
                        break;

                    case CoreStatics.C_COL_BTN_VALID:
                        {
                            bool bContinua = true;
                            frmSmartCardProgress frmSC = null;
                            MovDiarioClinico movdc = null;
                            try
                            {
                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                                if (e.Cell.Row.Cells["PermessoUAFirma"].Text.Trim() == "1")
                                {
                                    bContinua = false;


                                    setNavigazione(false);
                                    frmSC = new frmSmartCardProgress();
                                    frmSC.InizializzaEMostra(0, 4, this);
                                    frmSC.SetCursore(enum_app_cursors.WaitCursor);

                                    frmSC.SetStato(@"Validazione Diario " + e.Cell.Row.Cells["DescrTipoDiario"].Text + @" del " + Convert.ToDateTime(e.Cell.Row.Cells["DataInserimento"].Value).ToString("dd/MM/yyyy HH:mm"));


                                    try
                                    {
                                        frmSC.SetStato(@"Generazione Documento...");

                                        byte[] pdfContent = CoreStatics.GeneraPDFValidazioneDiario(e.Cell.Row.Cells["ID"].Text, true);

                                        if (pdfContent == null || pdfContent.Length <= 0)
                                        {
                                            frmSC.SetLog(@"Errore Generazione documento", true);
                                        }
                                        else
                                        {
                                            bContinua = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.DCLFM01, "Firma Digitale...", EnumEntita.DCL, e.Cell.Row.Cells["ID"].Text);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (frmSC != null) frmSC.SetLog(@"Errore Generazione documento: " + ex.Message, true);
                                        bContinua = false;
                                    }

                                }
                                if (bContinua)
                                {
                                    movdc = new MovDiarioClinico(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                    Risposta oRispostaElaboraPrima = new Risposta();
                                    oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.DCL_VALIDA_PRIMA.ToString(), new object[1] { movdc }, CommonStatics.UAPadri(movdc.CodUA, CoreStatics.CoreApplication.Ambiente));
                                    if (oRispostaElaboraPrima.Successo)
                                    {
                                        bContinua = movdc.Valida();
                                    }
                                    else
                                    {
                                        bContinua = false;
                                        easyStatics.EasyMessageBox(oRispostaElaboraPrima.ex.Message, "Validazione Diario Clinico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
                            }
                            finally
                            {
                                if (frmSC != null)
                                {
                                    frmSC.Close();
                                    frmSC.Dispose();
                                }

                                setNavigazione(true);

                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                            }

                            if (bContinua && movdc != null)
                            {
                                CaricaDati();
                                this.ucEasyGrid.ActiveRow = null;
                                RowsCollection gridrows = this.ucEasyGrid.Rows;
                                if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato != null) CoreStatics.SelezionaRigaInGriglia(ref gridrows, "ID", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario);
                            }
                        }
                        break;


                    case CoreStatics.C_COL_BTN_DEL:
                        if (e.Cell.Row.Cells["PermessoAnnulla"].Text == "1")
                        {
                            if (easyStatics.EasyMessageBox("Sei sicuro di voler ANNULLARE" + Environment.NewLine + "la voce corrente?", "Annullamento Diario Clinico", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                MovDiarioClinico movdc = new MovDiarioClinico(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                if (movdc.Annulla())
                                {
                                    CaricaDati();
                                    this.ucEasyGrid.ActiveRow = null;
                                    RowsCollection gridrows = this.ucEasyGrid.Rows;
                                    CoreStatics.SelezionaRigaInGriglia(ref gridrows, "ID", movdc.IDMovDiario);

                                }
                                movdc = null;
                            }
                        }
                        break;

                    default:
                        break;

                }

                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato = null;
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

        private void ucEasyGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = (CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4);
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

                            case "DescrPaziente":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "";
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
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
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
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;


                                break;

                            case "DescrValidazione":
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
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }


                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;


                                break;

                            case "DescrAnnullamento":
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
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 2.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }


                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 3;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;


                                break;

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
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 1.5) - Convert.ToInt32(refBtnWidth * 4.25) - 40;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.BorderAlpha = Infragistics.Win.Alpha.Opaque;
                                oCol.CellAppearance.BorderColor = Color.Red;
                                oCol.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
                                oCol.CellAppearance.BackColor = Color.WhiteSmoke;

                                oCol.RowLayoutColumnInfo.OriginX = 1;
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

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_EDIT + @"_SP_Prima"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_EDIT + @"_SP_Prima");
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
                    colEdit.RowLayoutColumnInfo.OriginX = 2;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
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
                    colEdit.CellButtonAppearance.Image = Properties.Resources.Cancella_32;


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
                    colEdit.RowLayoutColumnInfo.OriginX = 6;
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

                if (e.Row.IsGroupByRow)
                {
                    e.Row.ExpansionIndicator = ShowExpansionIndicator.Never;

                    if (g == null) g = this.CreateGraphics();
                    e.Row.Height = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium), g.DpiY) + 10;
                }
                else
                {
                    if (e.Row.IsDataRow)
                    {

                        foreach (UltraGridCell ocell in e.Row.Cells)
                        {
                            if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT)
                            {
                                if (e.Row.Cells.Exists("PermessoModifica") && ocell.Row.Cells["PermessoModifica"].Value.ToString() != "1")
                                    ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                            }
                            else if (ocell.Column.Key == CoreStatics.C_COL_BTN_VALID)
                            {
                                if (e.Row.Cells.Exists("PermessoValida") && ocell.Row.Cells["PermessoValida"].Value.ToString() != "1")
                                {
                                    ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                                }
                                else
                                {
                                    if (e.Row.Cells.Exists("PermessoUAFirma") && ocell.Row.Cells["PermessoUAFirma"].Value.ToString() == "1")
                                        ocell.ButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMA_32);
                                    else
                                        ocell.ButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMA_32);
                                }

                            }
                            else if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL)
                            {
                                if (e.Row.Cells.Exists("PermessoAnnulla") && ocell.Row.Cells["PermessoAnnulla"].Value.ToString() != "1")
                                    ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                            }
                        }

                    }
                }

            }
            catch (Exception)
            {
            }

        }

        private void ucEasyTreeViewFiltroTipo_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            try
            {

                if (e.TreeNode.Key == CoreStatics.GC_TUTTI)
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                        {
                            oNode.CheckedState = e.TreeNode.CheckedState;
                        }
                    }
                }

            }
            catch (Exception)
            {

            }

        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
            CaricaDati();
            this.ucEasyGrid.Focus();
        }

        private void ucEasyGridFiltro_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Override.HeaderClickAction = HeaderClickAction.Select;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                if (oCol.Key.ToUpper().IndexOf("DESC") < 0) oCol.Hidden = true;

            }
        }

        private void ucDiarioClinicoPaziente_Enter(object sender, EventArgs e)
        {
            try
            {
                this.ucEasyGrid.PerformLayout();
            }
            catch (Exception)
            {
            }
        }

        private void utxtFiltroContenuto_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.ubApplicaFiltro.Enabled && e.KeyCode == Keys.Enter) ubApplicaFiltro_Click(this.ubApplicaFiltro, new EventArgs());
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

        private void ucEasyGridFiltroUA_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Override.HeaderClickAction = HeaderClickAction.Select;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                if (oCol.Key.ToUpper().IndexOf("DESC") < 0) oCol.Hidden = true;

            }
        }

        private void utxtRicerca_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.CaricaDati();
                this.ucEasyGrid.Focus();
            }
        }
    }
}
