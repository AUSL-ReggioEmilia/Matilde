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
    public partial class ucConsegne : UserControl, Interfacce.IViewUserControlMiddle
    {

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

        public ucConsegne()
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

                CaricaDati(true, false, true);


                CoreStatics.SetNavigazione(true);

            }
        }

        public void Carica()
        {
            try
            {
                InizializzaControlli();
                InizializzaUltraGridConsegnaLayout();
                VerificaSicurezza();
                InizializzaFiltri();

                CoreStatics.CoreApplication.IDConsegnaSelezionata = "";

                CaricaDati(false, true, true);
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

                CoreStatics.SetContesto(EnumEntita.CSG, null);

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
                this.ubAdd.Appearance.Image = Properties.Resources.Aggiungi_256;
                this.ubAdd.PercImageFill = 0.75F;
                this.ubAdd.ShortcutKey = Keys.Add;
                this.ubAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubAdd.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                this.uchkFiltro.Checked = false;

            }
            catch (Exception)
            {
            }
        }

        private void VerificaSicurezza()
        {

            try
            {
                if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Consegne_Inserisci))
                    this.ubAdd.Enabled = true;
                else
                    this.ubAdd.Enabled = false;

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

                    this.ubApplicaFiltro.PercImageFill = 0.75F;
                    this.ubApplicaFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

                    this.drFiltro.Value = null;
                    this.udteFiltroDA.Value = null;
                    this.udteFiltroA.Value = null;

                    if (this.ucEasyGridFiltroUA.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroUA.Selected.Rows.Clear();
                        this.ucEasyGridFiltroUA.ActiveRow = null;
                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridFiltroUA, "Codice", CoreStatics.GC_TUTTI);
                    }
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
                    if (this.ucEasyGridFiltroStato.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroStato.Selected.Rows.Clear();
                        this.ucEasyGridFiltroStato.ActiveRow = null;
                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridFiltroStato, this.ucEasyGridFiltroStato.DisplayLayout.Bands[0].Columns[0].Key, CoreStatics.GC_TUTTI);
                    }
                }
                catch (Exception)
                {
                }
            }


        }

        private void CaricaDati(bool filtridamaschera, bool filtropredefinito, bool datiestesi)
        {
            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                CoreStatics.SetContesto(EnumEntita.CSG, null);

                DataSet dsDE = null; DataTable dtGriglia = null; bool bFiltro = false;


                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                if (datiestesi)
                {
                    op.Parametro.Add("DatiEstesi", "1");
                }


                if (filtridamaschera)
                {
                    #region Filtri da Maschera                    

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

                    Dictionary<string, string> listatipo = new Dictionary<string, string>();


                    if (this.ucEasyTreeViewFiltroTipo.Nodes.Count > 0
        && this.ucEasyTreeViewFiltroTipo.Nodes.Exists(CoreStatics.GC_TUTTI)
        && this.ucEasyTreeViewFiltroTipo.Nodes[CoreStatics.GC_TUTTI].CheckedState == CheckState.Unchecked
    )
                    {
                        foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroTipo.Nodes[CoreStatics.GC_TUTTI].Nodes)
                        {
                            if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                            {
                                listatipo.Add(oNode.Key, oNode.Text);
                            }
                            else
                            {
                                bFiltro = true;
                            }
                        }
                        string[] codtipo = listatipo.Keys.ToArray();
                        op.ParametroRipetibile.Add("CodTipoConsegna", codtipo);
                    }



                    if (this.ucEasyGridFiltroUA.ActiveRow != null && this.ucEasyGridFiltroUA.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                    {
                        op.Parametro.Add("CodUA", this.ucEasyGridFiltroUA.ActiveRow.Cells["Codice"].Text);
                        bFiltro = true;
                    }

                    if (this.ucEasyGridFiltroStato.ActiveRow != null && this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                    {
                        op.Parametro.Add("CodStatoConsegna", this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text);
                        bFiltro = true;
                    }
                    #endregion  
                }
                if (filtropredefinito)
                {
                    #region Filtro Predefinito

                    this.drFiltro.Value = ucEasyDateRange.C_RNG_24H;
                    bFiltro = true;

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
                    #endregion  
                }

                this.uchkFiltro.Checked = bFiltro;

                #region carica griglie dati e dati estesi (se necessario)
                op.TimeStamp.CodEntita = EnumEntita.CSG.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                dsDE = Database.GetDatasetStoredProc("MSP_SelMovConsegne", spcoll);
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

                if (datiestesi)
                {
                    CaricaDatiEstesi(ref dsDE);
                }
                #endregion

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
                if (datasetmovimenti == null)
                {
                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("DatiEstesi", "1");

                    op.TimeStamp.CodEntita = EnumEntita.CSG.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    datasetmovimenti = Database.GetDatasetStoredProc("MSP_SelMovConsegne", spcoll);
                }

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

                this.ucEasyGridFiltroStato.DataSource = CoreStatics.AggiungiTuttiDataTable(datasetmovimenti.Tables[2], false);
                this.ucEasyGridFiltroStato.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Stato";
                this.ucEasyGridFiltroStato.Refresh();

                this.ucEasyGridFiltroUA.DataSource = CoreStatics.AggiungiTuttiDataTable(datasetmovimenti.Tables[3], true);
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

                if (eRow.Cells["PermessoAnnulla"].Text == "0")
                {
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].CellDisplayStyle = CellDisplayStyle.PlainText;
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Appearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                    string sStato = "";
                    if (eRow.Cells["CodStatoConsegna"].Text == "AN")
                    {
                        sStato = eRow.Cells["DescrStatoConsegna"].Text;
                        if (eRow.Cells["DataAnnullamento"].Value != System.DBNull.Value)
                        {
                            sStato += @":" + Environment.NewLine + ((DateTime)eRow.Cells["DataAnnullamento"].Value).ToString("dd/MM/yyyy") + Environment.NewLine;
                            sStato += ((DateTime)eRow.Cells["DataAnnullamento"].Value).ToString("HH:mm");
                        }
                    }
                    eRow.Cells[CoreStatics.C_COL_BTN_ANNULLA].Value = sStato;
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

            }
            catch (Exception ex)
            {
            }
        }

        private void setNavigazione(bool enable)
        {
            try
            {
                CoreStatics.SetNavigazione(enable);

                this.ucEasyGrid.Enabled = enable;
                this.ubAdd.Enabled = enable;
                this.uchkFiltro.Enabled = enable;

                this.ucEasyTableLayoutPanelFiltro1.Enabled = enable;

            }
            catch
            {
                CoreStatics.SetNavigazione(true);
            }
        }

        private static bool CheckSelezionaTipoConsegna()
        {
            bool bret = false;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovConsegnaSelezionata.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Ambiente.Codruolo);
                op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovConsegnaSelezionata.Azione.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoConsegna", spcoll);

                if (oDt.Rows.Count == 1)
                {
                    CoreStatics.CoreApplication.MovConsegnaSelezionata.CodTipoConsegna = oDt.Rows[0]["Codice"].ToString();
                    CoreStatics.CoreApplication.MovConsegnaSelezionata.CodScheda = oDt.Rows[0]["CodScheda"].ToString();
                    bret = true;
                }

            }
            catch (Exception)
            {

            }

            return bret;

        }

        private static bool AnnullaConsegnaPrecedente()
        {
            bool bret = false;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovConsegnaSelezionata.CodUA);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Ambiente.Codruolo);
                op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovConsegnaSelezionata.Azione.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoConsegna", spcoll);

                if (oDt.Rows.Count == 1)
                {
                    CoreStatics.CoreApplication.MovConsegnaSelezionata.CodTipoConsegna = oDt.Rows[0]["Codice"].ToString();
                    CoreStatics.CoreApplication.MovConsegnaSelezionata.CodScheda = oDt.Rows[0]["CodScheda"].ToString();
                    bret = true;
                }

            }
            catch (Exception)
            {

            }

            return bret;

        }
        #endregion

        #region EVENTI

        private void ubAdd_Click(object sender, EventArgs e)
        {
            string codUA = string.Empty;
            string descrUA = string.Empty;

            CoreStatics.CoreApplication.MovConsegnaSelezionata = null;

            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Consegne_SelezioneUA, false) == DialogResult.OK)
            {
                codUA = CoreStatics.CoreApplication.ConsegneUACodiceSelezionata;
                descrUA = CoreStatics.CoreApplication.ConsegneUADescrizioneSelezionata;

                CoreStatics.CoreApplication.ConsegneUACodiceSelezionata = "";
                CoreStatics.CoreApplication.ConsegneUADescrizioneSelezionata = "";

                CoreStatics.CoreApplication.MovConsegnaDaAnnullare = null;

                MovConsegna movcsg = new MovConsegna(CoreStatics.CoreApplication.Ambiente);
                movcsg.Azione = EnumAzioni.INS;
                movcsg.DataEvento = DateTime.Now;

                movcsg.CodUA = codUA;
                movcsg.DescrUA = descrUA;

                movcsg.CodStatoConsegna = "IC";
                CoreStatics.CoreApplication.MovConsegnaSelezionata = movcsg;



                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoConsegna, false) == DialogResult.OK)
                {

                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingConsegna, false) == DialogResult.OK)
                    {
                        string idprecedente = string.Empty;
                        idprecedente = CoreStatics.CoreApplication.MovConsegnaSelezionata.CercaPrecedente();
                        if (idprecedente != string.Empty)
                        {
                            if (easyStatics.EasyMessageBox("Esiste una consegna precedente, si desidera annullarla ?", "Annullamento Consegna Precedente ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                MovConsegna movprec = new MovConsegna(idprecedente, CoreStatics.CoreApplication.Ambiente);
                                if (movprec != null)
                                {
                                    movprec.Annulla();
                                    movprec = null;
                                }
                            }
                        }

                        CaricaDati(false, false, true);
                        if (CoreStatics.CoreApplication.MovConsegnaSelezionata != null)
                        {
                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovConsegnaSelezionata.IDMovConsegna);
                        }
                    }
                }

                CoreStatics.SetNavigazione(true);

            }

            CoreStatics.CoreApplication.ConsegneUACodiceSelezionata = "";
            CoreStatics.CoreApplication.ConsegneUADescrizioneSelezionata = "";

        }

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

                            case "DescrUtenteUltimaModificaCalcolato":
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

                            case "DataUltimaModificaCalcolato":
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
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refIcoWidth * 2) - Convert.ToInt32(refBtnWidth * 4) - 30;
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
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refIcoWidth * 2) - Convert.ToInt32(refBtnWidth * 4) - 30;
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
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
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
            catch (Exception ex)
            {
                string aa = ex.Message;
            }
        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.CoreApplication.IDConsegnaSelezionata = "";
            CoreStatics.SetContesto(EnumEntita.CSG, this.ucEasyGrid.ActiveRow);
            CoreStatics.CoreApplication.IDConsegnaSelezionata = this.ucEasyGrid.ActiveRow.Cells["ID"].Text;
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {
            try
            {
                CoreStatics.CoreApplication.MovConsegnaDaAnnullare = null;
                switch (e.Cell.Column.Key)
                {
                    case CoreStatics.C_COL_BTN_EDIT:
                        if (e.Cell.Row.Cells["PermessoModifica"].Text == "1")
                        {

                            MovConsegna movcsg = new MovConsegna(e.Cell.Row.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);
                            CoreStatics.CoreApplication.MovConsegnaSelezionata = movcsg;
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingConsegna) == DialogResult.OK)
                            {
                                CaricaDati(true, false, true);

                                if (CoreStatics.CoreApplication.MovConsegnaSelezionata != null) CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovConsegnaSelezionata.IDMovConsegna);
                            }

                        }
                        break;



                    case CoreStatics.C_COL_BTN_ANNULLA:
                        if (e.Cell.Row.Cells["PermessoAnnulla"].Text == "1")
                        {
                            if (easyStatics.EasyMessageBox("Sei sicuro di voler ANNULLARE" + Environment.NewLine + "la voce corrente?", "Annullamento Consegna", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                MovConsegna movcsg = new MovConsegna(e.Cell.Row.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                                if (movcsg.Annulla())
                                {

                                    e.Cell.Row.Cells["CodStatoConsegna"].Value = movcsg.CodStatoConsegna;
                                    e.Cell.Row.Cells["DescrStatoConsegna"].Value = movcsg.DescrStatoConsegna;
                                    e.Cell.Row.Cells["DataAnnullamento"].Value = movcsg.DataAnnullamento;
                                    e.Cell.Row.Cells["PermessoModifica"].Value = movcsg.PermessoModifica;
                                    e.Cell.Row.Cells["PermessoAnnulla"].Value = movcsg.PermessoAnnulla;
                                    if (movcsg.Icona == null)
                                        e.Cell.Row.Cells["Icona"].Value = System.DBNull.Value;
                                    else
                                        e.Cell.Row.Cells["Icona"].Value = movcsg.Icona;
                                    e.Cell.Row.Cells["IDIcona"].Value = movcsg.IDIcona;
                                    e.Cell.Row.Cells["AnteprimaRTF"].Value = movcsg.MovScheda.AnteprimaRTF;

                                    InitializeRow(e.Cell.Row);

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

            }
            catch (Exception)
            {

            }

        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                this.InizializzaFiltri();
                CaricaDati(false, false, true);
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

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
            CaricaDati(true, false, true);
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

        private void ucConsegne_Enter(object sender, EventArgs e)
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

    }
}
