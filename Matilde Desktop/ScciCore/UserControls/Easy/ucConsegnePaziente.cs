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
    public partial class ucConsegnePaziente : UserControl, Interfacce.IViewUserControlMiddle
    {

        #region Declare

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

        private Dictionary<int, byte[]> oIcone = new Dictionary<int, byte[]>();

        private Graphics g = null;

        #endregion

        public ucConsegnePaziente()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
        }

        #region INTERFACCIA

        public void Aggiorna()
        {

            if (this.IsDisposed == false)
            {

                CoreStatics.SetNavigazione(false);

                CaricaGriglia(false);

                CoreStatics.SetNavigazione(true);

            }

        }

        public void Carica()
        {

            try
            {

                CoreStatics.CoreApplication.IDConsegnaSelezionata = "";

                InizializzaControlli();
                InizializzaUltraGridConsegnaLayout();
                InizializzaFiltri();
                setPulsanteMax();

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

                CoreStatics.SetContesto(EnumEntita.CSP, null);

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

                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                this.uchkFiltro.Checked = true;

                this.ubApplicaFiltro.PercImageFill = 0.75F;
                this.ubApplicaFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

            }
            catch (Exception)
            {
            }

        }

        private void InizializzaUltraGridConsegnaLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridConsegnaLayout", this.Name);
            }
        }

        private void InizializzaFiltri()
        {

            if (this.IsDisposed == false)
            {

                try
                {

                    this.chkMostraVistati.Checked = false;
                    this.chkMostraAnnullati.Checked = false;
                    this.chkOrdinaLetto.Checked = false;

                    if (this.ucEasyTreeViewFiltroTipo.Nodes.Count > 0)
                    {
                        this.ucEasyTreeViewFiltroTipo.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, false);
                        foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                        {
                            if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                            {
                                oNode.CheckedState = CheckState.Checked;
                            }
                        }
                        this.ucEasyTreeViewFiltroTipo.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, true);
                    }

                    if (this.ucEasyGridFiltroUA.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroUA.Selected.Rows.Clear();
                        this.ucEasyGridFiltroUA.ActiveRow = null;
                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridFiltroUA, "Codice", CoreStatics.GC_TUTTI);
                    }

                    if (this.ucEasyComboEditorFiltriSpeciali.Items.Count <= 0)
                    {
                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                        op.Parametro.Add("CodTipoFiltroSpeciale", EnumTipoFiltroSpeciale.CSPTRA.ToString());

                        SqlParameterExt[] spcoll = new SqlParameterExt[1];

                        string xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        DataTable dt = Database.GetDataTableStoredProc("MSP_SelFiltriSpeciali", spcoll);

                        this.ucEasyComboEditorFiltriSpeciali.ValueMember = "Codice";
                        this.ucEasyComboEditorFiltriSpeciali.DisplayMember = "Descrizione";
                        this.ucEasyComboEditorFiltriSpeciali.DataSource = CoreStatics.AggiungiTuttiDataTable(dt, false);
                        this.ucEasyComboEditorFiltriSpeciali.Refresh();
                    }
                    this.ucEasyComboEditorFiltriSpeciali.SelectedIndex = 0;

                    this.drFiltro.Value = ucEasyDateRange.C_RNG_60G;
                    this.drFiltro.DateFuture = false;

                }
                catch (Exception)
                {
                }

            }

        }

        private void CaricaGriglia(bool datiestesi)
        {

            try
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                CoreStatics.SetContesto(EnumEntita.CSP, null);

                DataSet dsDE = null; DataTable dtGriglia = null;
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                if (this.txtRicerca.Text != string.Empty)
                {

                    string filtrogenerico = string.Empty;
                    string[] ricerche = this.txtRicerca.Text.Split(' ');
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

                    filtrogenerico = filtrogenerico.Trim();
                    op.Parametro.Add("FiltroGenerico", filtrogenerico);

                }

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
                if (this.ucEasyTreeViewFiltroTipo.Nodes.Count > 0
    && this.ucEasyTreeViewFiltroTipo.Nodes.Exists(CoreStatics.GC_TUTTI)
    && this.ucEasyTreeViewFiltroTipo.Nodes[CoreStatics.GC_TUTTI].CheckedState == CheckState.Unchecked)
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                        {
                            listatipo.Add(oNode.Key, oNode.Text);
                        }
                    }
                    string[] codtipo = listatipo.Keys.ToArray();
                    op.ParametroRipetibile.Add("CodTipoConsegnaPaziente", codtipo);
                }

                if (this.ucEasyGridFiltroUA.ActiveRow != null && this.ucEasyGridFiltroUA.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodUA", this.ucEasyGridFiltroUA.ActiveRow.Cells["Codice"].Text);
                }

                if (this.ucEasyComboEditorFiltriSpeciali.Text.Trim() != CoreStatics.GC_TUTTI)
                {
                    op.Parametro.Add("CodFiltroSpeciale", this.ucEasyComboEditorFiltriSpeciali.Value.ToString());
                }

                if (this.udteFiltroDA.Value != null)
                {
                    op.Parametro.Add("DataInizio", CoreStatics.getDateTime((DateTime)this.udteFiltroDA.Value));
                }
                if (this.udteFiltroA.Value != null)
                {
                    op.Parametro.Add("DataFine", CoreStatics.getDateTime((DateTime)this.udteFiltroA.Value));
                }

                op.Parametro.Add("OrdinaLetto", (this.chkOrdinaLetto.Checked == true ? "1" : "0"));

                op.Parametro.Add("DatiEstesi", (datiestesi ? "1" : "0"));

                this.uchkFiltro.Checked = true;

                op.TimeStamp.CodEntita = EnumEntita.CSP.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                FwDataParametersList procParams = new FwDataParametersList();
                procParams.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                {
                    dsDE = conn.Query<DataSet>("MSP_SelMovConsegnePazienteTrasversale", procParams, CommandType.StoredProcedure);
                }

                dtGriglia = dsDE.Tables[0];
                DataTable dtEdit = dtGriglia.Copy();

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

                if (g == null) g = this.CreateGraphics();
                CoreStatics.ImpostaGroupByGriglia(ref this.ucEasyGrid, ref g, "DescrPaziente", easyStatics.easyRelativeDimensions.Small);

                this.ucEasyGrid.DisplayLayout.Bands[0].Override.GroupByRowPadding = 0;
                this.ucEasyGrid.DisplayLayout.Bands[0].Override.GroupByRowSpacingBefore = 0;
                this.ucEasyGrid.DisplayLayout.Bands[0].Override.RowSpacingBefore = 0;
                this.ucEasyGrid.PerformLayout();

                if (datiestesi)
                {
                    CaricaDatiEstesi(ref dsDE);
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

        private void CaricaDatiEstesi(ref DataSet datasetmovimenti)
        {

            try
            {

                this.ucEasyTreeViewFiltroTipo.Nodes.Clear();
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
                this.ucEasyTreeViewFiltroTipo.Nodes.Add(oNodeRoot);
                this.ucEasyTreeViewFiltroTipo.ExpandAll();

                this.ucEasyGridFiltroUA.DataSource = CoreStatics.AggiungiTuttiDataTable(datasetmovimenti.Tables[2], false);
                this.ucEasyGridFiltroUA.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Struttura";
                this.ucEasyGridFiltroUA.Refresh();


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
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].CellDisplayStyle = CellDisplayStyle.PlainText;
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Appearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                    string sStato = "";
                    if (eRow.Cells["CodStatoConsegnaPaziente"].Text == "AN")
                    {
                        sStato = eRow.Cells["DescrStatoConsegnaPaziente"].Text;
                        if (eRow.Cells["DataAnnullamento"].Value != System.DBNull.Value)
                        {
                            sStato += @":" + Environment.NewLine + ((DateTime)eRow.Cells["DataAnnullamento"].Value).ToString("dd/MM/yyyy") + Environment.NewLine;
                            sStato += ((DateTime)eRow.Cells["DataAnnullamento"].Value).ToString("HH:mm");
                        }
                    }
                    else if (eRow.Cells["CodStatoConsegnaPaziente"].Text == "CA")
                    {
                        sStato = eRow.Cells["DescrStatoConsegnaPaziente"].Text;
                        if (eRow.Cells["DataCancellazione"].Value != System.DBNull.Value)
                        {
                            sStato += @":" + Environment.NewLine + ((DateTime)eRow.Cells["DataCancellazione"].Value).ToString("dd/MM/yyyy") + Environment.NewLine;
                            sStato += ((DateTime)eRow.Cells["DataCancellazione"].Value).ToString("HH:mm");
                        }
                    }
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Value = sStato;
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

        private void setPulsanteMax()
        {
            int iStato = 0; try
            {
                Form frm = this.FindForm();
                if (frm != null)
                {
                    if (frm is Interfacce.IViewFormMain)
                    {
                        if ((frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato)
                            iStato = 2;
                        else
                            iStato = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            try
            {
                switch (iStato)
                {
                    case 1:
                        this.ubMaximize.Visible = true;
                        this.ubMaximize.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CTRL_INGRANDISCI_256);
                        this.ubMaximize.PercImageFill = 0.75F;
                        break;
                    case 2:
                        this.ubMaximize.Visible = true;
                        this.ubMaximize.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CTRL_RIDUCI_256);
                        this.ubMaximize.PercImageFill = 0.75F;
                        break;
                    default:
                        this.ubMaximize.Visible = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        #endregion

        #region EVENTI

        private void ultraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);

            int filtroWidth = 30 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.ultraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.ultraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].Size.Height);
            this.ultraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.ultraDockManager.DockAreas[0].Size.Height);
            this.pnlFiltro.Width = filtroWidth;

        }

        private void txtRicerca_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CaricaGriglia(false);
                    this.ucEasyGrid.Focus();
                }
            }
            catch (Exception)
            {
            }
        }

        private void ubMaximize_Click(object sender, EventArgs e)
        {
            try
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);
                Form frm = this.FindForm();
                if (frm != null && frm is Interfacce.IViewFormMain)
                {
                    (frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato = !(frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato;


                    setPulsanteMax();
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubMaximize_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
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

        private void uchkFiltro_Click(object sender, EventArgs e)
        {

            this.InizializzaFiltri();
            CaricaGriglia(true);

        }

        private void ucEasyGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {
                int refIcoWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XLarge) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = Convert.ToInt32(CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4.3);
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
                                oCol.RowLayoutColumnInfo.SpanY = 3;

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
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refIcoWidth * 2) - Convert.ToInt32(refBtnWidth * 5.5) - 30;
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
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
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

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 3;
                                oCol.RowLayoutColumnInfo.SpanX = 2;
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
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refIcoWidth * 2) - Convert.ToInt32(refBtnWidth * 5.5) - 30;
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
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refIcoWidth * 2) - Convert.ToInt32(refBtnWidth * 5.5) - 30;
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

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.CoreApplication.IDConsegnaSelezionata = "";
            CoreStatics.SetContesto(EnumEntita.CSP, this.ucEasyGrid.ActiveRow);
            if (this.ucEasyGrid.ActiveRow.IsDataRow)
            {
                CoreStatics.CoreApplication.IDConsegnaSelezionata = this.ucEasyGrid.ActiveRow.Cells["ID"].Text;
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

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            InitializeRow(e.Row);
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
            CaricaGriglia(false);
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

        private void txtFiltroUA_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                CoreStatics.SetGridWizardFilter(ref this.ucEasyGridFiltroUA,
                                                this.ucEasyGridFiltroUA.DisplayLayout.Bands[0].Columns[1].Key,
                                                this.txtFiltroUA.Text);
            }
            catch (Exception)
            {
            }
        }

        private void ucConsegnePaziente_Enter(object sender, EventArgs e)
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
