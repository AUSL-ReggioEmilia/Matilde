using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;

using System.Diagnostics;
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
    public partial class ucTerapieFarmacologiche : UserControl, Interfacce.IViewUserControlMiddle
    {

        private UserControl _ucc = null;
        private bool _popuprtf = false;
        private ucRichTextBox _ucRichTextBox = null;
        private ucEasyGrid _ucEasyGridOrari = null;

        private bool _bFirmaDigitale = false;

        private Dictionary<string, byte[]> oIcone = new Dictionary<string, byte[]>();
        private Dictionary<string, byte[]> oIcone2 = new Dictionary<string, byte[]>();

        public ucTerapieFarmacologiche()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
        }

        #region Interface

        public void Aggiorna()
        {
            if (this.IsDisposed == false)
            {
                string s = string.Empty;
                if (this.ucEasyGrid.ActiveRow != null) { s = this.ucEasyGrid.ActiveRow.Cells["ID"].Text; }
                this.AggiornaGriglia(true);
                if (s != string.Empty) { CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", s); }
                this.CaricaAnteprima();

                setPulsanteMax();
            }


        }

        public void Carica()
        {

            try
            {

                var oPaziente = CoreStatics.CoreApplication.Paziente;
                var oEpisodio = CoreStatics.CoreApplication.Episodio;
                var oTrasferimento = CoreStatics.CoreApplication.Trasferimento;

                _bFirmaDigitale = DBUtils.ModuloUAAbilitato(CoreStatics.CoreApplication.Trasferimento.CodUA, EnumUAModuli.FirmaD_Prescrizioni);

                this.lblPaziente.Text = string.Empty;
                if (oPaziente != null && oPaziente.Attivo == true)
                {
                    this.lblPaziente.Text = string.Format("{0} {1} - Nato il {2} ", oPaziente.Cognome, oPaziente.Nome, oPaziente.DataNascita.ToShortDateString());
                }
                if (oTrasferimento != null && oTrasferimento.Attivo == true)
                {
                    this.lblPaziente.Text += string.Format("- N. Cartella: {0} ", oTrasferimento.NumeroCartella);
                }
                if (oEpisodio != null && oEpisodio.Attivo == true)
                {
                    this.lblPaziente.Text += string.Format("- Nosologico: {0}", (oEpisodio.NumeroEpisodio != string.Empty ? oEpisodio.NumeroEpisodio : oEpisodio.NumeroListaAttesa));
                }

                setPulsanteMax();

                InizializzaControlli();
                InizializzaUltraGridLayout();
                VerificaSicurezza();
                InizializzaFiltri();

                this.CaricaGriglia(true);

                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                {
                    CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione);
                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                }

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

                oIcone = new Dictionary<string, byte[]>();
                oIcone2 = new Dictionary<string, byte[]>();

                CoreStatics.SetContesto(EnumEntita.PRF, null);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region Private Functions

        private void InizializzaControlli()
        {

            try
            {
                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);

                this.ubAdd.Appearance.Image = Properties.Resources.Aggiungi_256;
                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Checked = false;

                this.ubAdd.PercImageFill = 0.75F;
                this.ubAdd.ShortcutKey = Keys.Add;
                this.ubAdd.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubAdd.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubProsegui.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_PROSEGUITERAPIA_256);
                this.ubProsegui.PercImageFill = 0.75F;
                this.ubProsegui.ShortcutKey = Keys.P;
                this.ubProsegui.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubProsegui.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubCopy.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_COPIAPRESCRIZIONI_256);
                this.ubCopy.PercImageFill = 0.75F;
                this.ubCopy.ShortcutKey = Keys.C;
                this.ubCopy.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubCopy.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubApplicaFiltro.PercImageFill = 0.75F;
                this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

                InizializzaGriglieAnteprima();

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
                this.ucEasyGrid.DisplayLayout.Override.RowSpacingBefore = 3;
                this.ucEasyGrid.DisplayLayout.Override.RowSizing = RowSizing.AutoFree;
                this.ucEasyGrid.DisplayLayout.Override.RowSizingAutoMaxLines = 2;

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
                    this.ubProsegui.Enabled = false;
                    this.ubCopy.Enabled = false;
                }
                else
                {
                    this.ubAdd.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Inserisci);
                    this.ubProsegui.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Inserisci);
                    this.ubCopy.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Inserisci);
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "VerificaSicurezza", this.Name);
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

                    if (this.ucEasyGridFiltroViaSomministrazione.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroViaSomministrazione.ActiveRow = null;
                        this.ucEasyGridFiltroViaSomministrazione.Selected.Rows.Clear();
                        CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroViaSomministrazione, "Codice", CoreStatics.GC_TUTTI);
                    }

                    if (this.ucEasyGridFiltroTipo.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroTipo.ActiveRow = null;
                        this.ucEasyGridFiltroTipo.Selected.Rows.Clear();
                        CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroTipo, "Codice", CoreStatics.GC_TUTTI);
                    }


                    if (this.ucEasyTreeViewFiltroStato.Nodes.Count > 0)
                    {
                        this.ucEasyTreeViewFiltroStato.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, false);
                        foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroStato.Nodes[CoreStatics.GC_TUTTI].Nodes)
                        {
                            if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                            {
                                oNode.CheckedState = CheckState.Checked;
                            }
                        }
                        this.ucEasyTreeViewFiltroStato.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.TreeEventIds.AfterCheck, true);
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
                if (datiEstesi)
                    op.Parametro.Add("DatiEstesi", "1");
                else
                    op.Parametro.Add("DatiEstesi", "0");

                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovPrescrizioni", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();

                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                foreach (DataColumn dcCol in dtEdit.Columns)
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
                if (this.ucEasyGrid.DisplayLayout != null)
                {
                    this.ucEasyGrid.DataSource = null;

                    if (datiEstesi)
                    {
                        this.ucEasyGridFiltroViaSomministrazione.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1], true);
                        this.ucEasyGridFiltroViaSomministrazione.Refresh();

                        this.ucEasyGridFiltroTipo.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2], true);
                        this.ucEasyGridFiltroTipo.Refresh();

                        Infragistics.Win.UltraWinTree.UltraTreeNode oNodeRoot = new Infragistics.Win.UltraWinTree.UltraTreeNode(CoreStatics.GC_TUTTI, "Stato");
                        oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                        oNodeRoot.CheckedState = CheckState.Checked;
                        foreach (DataRow oDr in ds.Tables[3].Rows)
                        {
                            Infragistics.Win.UltraWinTree.UltraTreeNode oNode = new Infragistics.Win.UltraWinTree.UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                            oNode.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                            oNode.CheckedState = CheckState.Checked;
                            oNodeRoot.Nodes.Add(oNode);
                        }
                        this.ucEasyTreeViewFiltroStato.Nodes.Add(oNodeRoot);
                        this.ucEasyTreeViewFiltroStato.ExpandAll();

                    }

                    this.ucEasyGrid.DataSource = dtEdit;
                    this.ucEasyGrid.Refresh();
                    this.ucEasyGrid.Focus();
                    this.ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);

                    if (this.ucEasyGrid.Rows.Count > 0)
                        this.ucEasyGrid.ActiveRow = this.ucEasyGrid.Rows[0];
                    else
                        this.ucEasyGrid.ActiveRow = null;

                    this.CaricaAnteprima();

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Name);
            }
        }

        private void AggiornaGriglia(bool datiEstesi)
        {

            string codviasomministrazioneselezionata = string.Empty;
            string codtiposelezionato = string.Empty;
            string codstatoselezionato = string.Empty;

            CoreStatics.SetNavigazione(false);

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                CoreStatics.SetContesto(EnumEntita.PRF, null);

                bool bFiltro = false;

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
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
                if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["Codice"].Text.Trim().Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodTipoPrescrizione", this.ucEasyGridFiltroTipo.ActiveRow.Cells["Codice"].Text);
                    bFiltro = true;
                }
                Dictionary<string, string> listastato = new Dictionary<string, string>();
                foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroStato.Nodes[CoreStatics.GC_TUTTI].Nodes)
                {
                    if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                    {
                        listastato.Add(oNode.Key, oNode.Text);
                    }
                    else
                    {
                        bFiltro = true;
                    }
                }
                string[] codstato = listastato.Keys.ToArray();
                op.ParametroRipetibile.Add("CodStatoPrescrizione", codstato);
                if (this.ucEasyGridFiltroViaSomministrazione.ActiveRow != null && this.ucEasyGridFiltroViaSomministrazione.ActiveRow.Cells["Codice"].Text.Trim().Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodViaSomministrazione", this.ucEasyGridFiltroViaSomministrazione.ActiveRow.Cells["Codice"].Text);
                    bFiltro = true;
                }
                this.uchkFiltro.Checked = bFiltro;

                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovPrescrizioni", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();

                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                foreach (DataColumn dcCol in dtEdit.Columns)
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
                if (this.ucEasyGrid.DisplayLayout != null)
                {
                    this.ucEasyGrid.DataSource = null;
                    this.ucEasyGrid.DataSource = dtEdit;
                    this.ucEasyGrid.Refresh();

                    if (datiEstesi)
                    {

                        if (this.ucEasyGridFiltroViaSomministrazione.ActiveRow != null && this.ucEasyGridFiltroViaSomministrazione.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                            codviasomministrazioneselezionata = this.ucEasyGridFiltroViaSomministrazione.ActiveRow.Cells["Codice"].Text;
                        if (this.ucEasyGridFiltroViaSomministrazione.DisplayLayout.Bands.Count > 0) { this.ucEasyGridFiltroViaSomministrazione.DisplayLayout.Bands[0].Columns.ClearUnbound(); }
                        this.ucEasyGridFiltroViaSomministrazione.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1], true);
                        this.ucEasyGridFiltroViaSomministrazione.Refresh();
                        if (codviasomministrazioneselezionata != null && codviasomministrazioneselezionata.Contains(CoreStatics.GC_TUTTI) != true)
                            CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroViaSomministrazione, "Codice", codviasomministrazioneselezionata);

                        if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                            codtiposelezionato = this.ucEasyGridFiltroTipo.ActiveRow.Cells["Codice"].Text;
                        if (this.ucEasyGridFiltroTipo.DisplayLayout.Bands.Count > 0) { this.ucEasyGridFiltroTipo.DisplayLayout.Bands[0].Columns.ClearUnbound(); }
                        this.ucEasyGridFiltroTipo.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[2], true);
                        this.ucEasyGridFiltroTipo.Refresh();
                        if (codtiposelezionato != null && codtiposelezionato.Contains(CoreStatics.GC_TUTTI) != true)
                            CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroTipo, "Codice", codtiposelezionato);

                    }
                }

                this.CaricaAnteprima();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "AggiornaGriglia", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }


            CoreStatics.SetNavigazione(true);

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
                        this.lblPaziente.Visible = false;
                        this.ucEasyTableLayoutPanel.RowStyles[0].Height = 0;
                        break;
                    case 2:
                        this.ubMaximize.Visible = true;
                        this.ubMaximize.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CTRL_RIDUCI_256);
                        this.ubMaximize.PercImageFill = 0.75F;
                        this.lblPaziente.Visible = true;
                        this.ucEasyTableLayoutPanel.RowStyles[0].Height = 20;
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

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                int refWidth = (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall) * 3;

                Graphics g = this.CreateGraphics();
                int refBtnWidth = (CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 3);
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

                            case "DataEvento":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
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
                                    oCol.MaxWidth = this.ucEasyGrid.Width - (refWidth * 20) - Convert.ToInt32(refBtnWidth * 2.5) - 40;
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

                            case "EPA":
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
                                    oCol.MaxWidth = refWidth * 5;
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
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "Icona2":
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

                                oCol.RowLayoutColumnInfo.OriginX = 5;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

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
                                oCol.RowLayoutColumnInfo.OriginX = 7;
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

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_ICO_STATO))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_ICO_STATO);
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


                    colEdit.RowLayoutColumnInfo.OriginX = 8;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
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
                    colEdit.RowLayoutColumnInfo.OriginX = 9;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_MENU))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_MENU);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_MENUPOPUP_32);

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
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_MENU + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_MENU + @"_SP");
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






















            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeLayout", this.Name);
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

                    case CoreStatics.C_COL_BTN_MENU:

                        var menu = (PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"];

                        ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_EDIT]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);
                        ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_TASK]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_TASKINFERMIERISTICO_32);
                        ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_DEL]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);
                        ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_COPY]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_COPIA_32);
                        ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_STATO]).InstanceProps.AppearancesLarge.Appearance.Image =
                            (e.Cell.Row.Cells["CodStatoContinuazione"].Text == EnumStatoContinuazione.AP.ToString() ? Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32) : Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_32));
                        ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_STATO]).InstanceProps.Caption =
                            (e.Cell.Row.Cells["CodStatoContinuazione"].Text == EnumStatoContinuazione.AP.ToString() ?
                            CoreStatics.GetEnumDescription(EnumStatoContinuazione.CH) : CoreStatics.GetEnumDescription(EnumStatoContinuazione.AP));

                        if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                        {
                            menu.Tools[CoreStatics.C_COL_BTN_EDIT].InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False;
                            menu.Tools[CoreStatics.C_COL_BTN_DEL].InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False;
                            menu.Tools[CoreStatics.C_COL_BTN_COPY].InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False;
                            menu.Tools[CoreStatics.C_COL_BTN_STATO].InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False;
                        }
                        else
                        {
                            menu.Tools[CoreStatics.C_COL_BTN_EDIT].InstanceProps.Visible = (e.Cell.Row.Cells["PermessoModifica"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                            menu.Tools[CoreStatics.C_COL_BTN_TASK].InstanceProps.Visible = (e.Cell.Row.Cells["PermessoTaskInfermiristici"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                            menu.Tools[CoreStatics.C_COL_BTN_DEL].InstanceProps.Visible = (e.Cell.Row.Cells["PermessoCancella"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                            menu.Tools[CoreStatics.C_COL_BTN_COPY].InstanceProps.Visible = (e.Cell.Row.Cells["PermessoCopia"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                            menu.Tools[CoreStatics.C_COL_BTN_STATO].InstanceProps.Visible = (e.Cell.Row.Cells["PermessoModifica"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                        }

                        ((PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"]).ShowPopup();

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

                    case "AnteprimaTXT":
                    case "AnteprimaRTF":
                        _popuprtf = true;
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerMain);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Row.Cells["AnteprimaRTF"].Text);

                        Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                        Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                        this.UltraPopupControlContainerMain.Show(this.ucEasyGrid, oPoint);
                        break;

                    case "DescrStatoPrescrizione":
                    case "EPA":
                        int altezza = 24;
                        int larghezza = 12;
                        _popuprtf = false;
                        _ucEasyGridOrari = null;
                        _ucEasyGridOrari = CoreStatics.getGridOrariPrescrizioni(e.Cell.Row.Cells["ID"].Text);
                        if (_ucEasyGridOrari != null && _ucEasyGridOrari.DataSource != null)
                        {


                            _ucEasyGridOrari.Size = new Size(this.ucEasyGridVALIDATI.Width * larghezza / 10, this.ucEasyGridVALIDATI.Height * altezza / 8);

                            _ucEasyGridOrari.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                            CoreStatics.SetEasyUltraGridLayout(ref _ucEasyGridOrari);
                            _ucEasyGridOrari.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;

                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerMain);

                            uie = e.Cell.GetUIElement();
                            oPoint = new Point(uie.Rect.Left, uie.Rect.Top);


                            this.UltraPopupControlContainerMain.Show(sender as ucEasyGrid, ((ucEasyGrid)sender).PointToScreen(oPoint));


                            _ucEasyGridOrari.DisplayLayout.Bands[0].ClearGroupByColumns();

                            foreach (UltraGridColumn oCol in _ucEasyGridOrari.DisplayLayout.Bands[0].Columns)
                            {

                                oCol.SortIndicator = SortIndicator.Disabled;
                                switch (oCol.Key)
                                {
                                    case "DataRiferimento":
                                        oCol.Hidden = false;
                                        oCol.Header.Caption = "Data";
                                        oCol.Format = "dddd dd/MM/yyyy HH:mm";
                                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                        oCol.Header.VisiblePosition = 1;
                                        break;

                                    case "DescStato":
                                        oCol.Hidden = false;
                                        oCol.Header.Caption = "Stato";
                                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                        oCol.Header.VisiblePosition = 2;
                                        break;

                                    case "DescTipo":
                                        oCol.Hidden = false;
                                        oCol.Header.Caption = "Tipo";
                                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                        oCol.Header.VisiblePosition = 3;
                                        break;

                                    default:
                                        oCol.Hidden = true;
                                        break;
                                }
                            }

                            _ucEasyGridOrari.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
                            _ucEasyGridOrari.Refresh();
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

        private void UltraToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {

            switch (e.Tool.Key)
            {

                case CoreStatics.C_COL_BTN_EDIT:
                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(this.ucEasyGrid.ActiveRow.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);

                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizione) == DialogResult.OK)
                    {
                        this.AggiornaGriglia(true);
                        if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                        {
                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione);
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                        }
                    }
                    break;

                case CoreStatics.C_COL_BTN_TASK:
                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(this.ucEasyGrid.ActiveRow.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);
                    CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera(EnumMaschere.TerapieFarmacologiche, EnumPulsante.PulsanteAvantiBottom, EnumMaschere.WorklistInfermieristica);
                    break;

                case CoreStatics.C_COL_BTN_DEL:
                    if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "la prescrizione selezionata ?", "Cancellazione Prescrizioni", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MovPrescrizione movpr = new MovPrescrizione(this.ucEasyGrid.ActiveRow.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);

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
                            movpr.Salva();

                            this.AggiornaGriglia(true);
                        }
                        else
                        {
                            easyStatics.EasyMessageBox("Non è possibile cancella la prescrizione selezionata a causa di un errore presentatosi durante la cancellazione della tempistica.", "Errore Cancellazione Prescrizioni", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }

                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;


                    }
                    break;

                case CoreStatics.C_COL_BTN_COPY:
                    if (easyStatics.EasyMessageBox("Confermi la copia della prescrizione selezionata ?", "Copia Prescrizione",
    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        MovPrescrizione movprorigine = new MovPrescrizione(this.ucEasyGrid.ActiveRow.Cells["ID"].Text, EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                            CoreStatics.CoreApplication.Paziente.ID,
                                                            CoreStatics.CoreApplication.Episodio.ID,
                                                            CoreStatics.CoreApplication.Trasferimento.ID, CoreStatics.CoreApplication.Ambiente);

                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CopiaDaOrigine(ref movprorigine);
                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Salva();
                        movprorigine = null;

                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizione);

                        this.AggiornaGriglia(true);
                        if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                        {
                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione);
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                        }

                    }
                    break;

                case CoreStatics.C_COL_BTN_STATO:
                    string sazione = (this.ucEasyGrid.ActiveRow.Cells["CodStatoContinuazione"].Text == EnumStatoContinuazione.AP.ToString() ? "Chiudere" : "Aprire");
                    if (easyStatics.EasyMessageBox("Sei sicuro di voler " + sazione + Environment.NewLine + "la prescrizione selezionata ?", sazione + " Prescrizioni", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MovPrescrizione movpr = new MovPrescrizione(this.ucEasyGrid.ActiveRow.Cells["ID"].Text, CoreStatics.CoreApplication.Ambiente);
                        movpr.CodStatoContinuazione = (this.ucEasyGrid.ActiveRow.Cells["CodStatoContinuazione"].Text == EnumStatoContinuazione.AP.ToString() ? EnumStatoContinuazione.CH.ToString() : EnumStatoContinuazione.AP.ToString()); ;
                        if (movpr.Salva())
                        {
                            AggiornaGriglia(true);
                        }
                    }
                    break;

            }

        }

        private void ucEasyGridFiltroViaSomministrazione_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "Descrizione":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Via di Somminist.";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }
        }

        private void ucEasyGridFiltroTipo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "Descrizione":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Tipo";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }
        }

        private void ucEasyTreeViewFiltroStato_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            try
            {

                if (e.TreeNode.Key == CoreStatics.GC_TUTTI)
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewFiltroStato.Nodes[CoreStatics.GC_TUTTI].Nodes)
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
                this.AggiornaGriglia(true);
            }
            else
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
            this.AggiornaGriglia(false);
            this.ucEasyGrid.Focus();
        }

        private void ubAdd_Click(object sender, EventArgs e)
        {

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                            CoreStatics.CoreApplication.Paziente.ID,
                                                            CoreStatics.CoreApplication.Episodio.ID,
                                                            CoreStatics.CoreApplication.Trasferimento.ID, CoreStatics.CoreApplication.Ambiente);

                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Azione = EnumAzioni.INS;
                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DataEvento = DateTime.Now;

                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoPrescrizione, false) == DialogResult.OK)
                {
                    if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizione, false);
                    else
                    {
                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizioniProtocollo, false, CoreStatics.CoreApplication.ListaIDMovPrescrizioniCreate);
                        CoreStatics.CoreApplication.ListaIDMovPrescrizioniCreate = null;
                    }

                    this.AggiornaGriglia(true);

                    if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                    {
                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione);
                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
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

        private void ubProsegui_Click(object sender, EventArgs e)
        {

            try
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ProseguiTerapia) == DialogResult.OK)
                {
                    this.AggiornaGriglia(true);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubProsegui_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

        }

        private void ubCopy_Click(object sender, EventArgs e)
        {

            try
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CopiaPrescrizioni) == DialogResult.OK)
                {
                    this.AggiornaGriglia(true);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubCopy_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

        }

        private void ucTerapieFarmacologiche_Enter(object sender, EventArgs e)
        {
            try
            {
                this.ucEasyGrid.PerformLayout();
            }
            catch (Exception)
            {
            }
        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.SetContesto(EnumEntita.PRF, this.ucEasyGrid.ActiveRow);
            CaricaAnteprima();
        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            if (_popuprtf)
                _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
            else
                _ucEasyGridOrari.ClickCell -= ucEasyGridOrari_ClickCell;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            if (_popuprtf)
            {
                _ucRichTextBox.RtfClick += ucRichTextBox_Click;
                _ucRichTextBox.Focus();
            }
            else
            {
                _ucEasyGridOrari.ClickCell += ucEasyGridOrari_ClickCell;
                _ucEasyGridOrari.Focus();
            }
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;

            if (_popuprtf)
                popup.PopupControl = _ucRichTextBox;
            else
                popup.PopupControl = _ucEasyGridOrari;
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
        }

        private void ucEasyGridOrari_ClickCell(object sender, ClickCellEventArgs e)
        {
            this.UltraPopupControlContainerMain.Close();
        }

        #endregion

        #region ANTEPRIME

        private void InizializzaGriglieAnteprima()
        {
            CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridVALIDATI);
            CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridDaValidare);
        }

        private void CaricaAnteprima()
        {
            string idPrescrizione = new Guid().ToString();
            try
            {
                if (this.ucEasyGrid.ActiveRow != null)
                {
                    idPrescrizione = this.ucEasyGrid.ActiveRow.Cells["ID"].Text;
                }
            }
            catch
            {
            }

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPrescrizione", idPrescrizione);
                op.ParametroRipetibile.Add("CodStatoPrescrizioneTempi", new string[2] { "VA", "SS" }); op.Parametro.Add("DatiEstesi", "0");


                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovPrescrizioniTempi", spcoll);


                DataTable dtEdit = ds.Tables[0].Copy();
                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper() == "DescrUtente".ToUpper() || dcCol.ColumnName.ToUpper() == "DescrUtenteValidazione".ToUpper())
                    {
                        dcCol.MaxLength = 250;
                    }

                }

                if (this.ucEasyGridVALIDATI.DisplayLayout != null)
                {
                    this.ucEasyGridVALIDATI.DisplayLayout.Bands[0].Columns.ClearUnbound();

                    this.ucEasyGridVALIDATI.DataSource = dtEdit;
                    this.ucEasyGridVALIDATI.Refresh();
                    this.ucEasyGridVALIDATI.PerformAction(UltraGridAction.FirstRowInBand, false, false);

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaAnteprima<VALIDATI>", this.Name);
            }

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPrescrizione", idPrescrizione);
                op.Parametro.Add("CodStatoPrescrizioneTempi", "IC"); op.Parametro.Add("DatiEstesi", "0");

                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovPrescrizioniTempi", spcoll);


                DataTable dtEdit = ds.Tables[0].Copy();
                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper() == "DescrUtente".ToUpper() || dcCol.ColumnName.ToUpper() == "DescrUtenteValidazione".ToUpper())
                    {
                        dcCol.MaxLength = 250;
                    }

                }

                if (this.ucEasyGridDaValidare.DisplayLayout != null)
                {
                    this.ucEasyGridDaValidare.DisplayLayout.Bands[0].Columns.ClearUnbound();

                    this.ucEasyGridDaValidare.DataSource = dtEdit;
                    this.ucEasyGridDaValidare.Refresh();
                    this.ucEasyGridDaValidare.PerformAction(UltraGridAction.FirstRowInBand, false, false);

                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaAnteprima<DA Validare>", this.Name);
            }
        }

        private void ucEasyGridVALIDATI_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                int refWidth = ((sender as ucEasyGrid).Width - 30) / 10 * 3; e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
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
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 0.7);
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
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "DescrProtocollo":
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
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 0.4);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "Posologia":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 0.7);
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
                                oCol.RowLayoutColumnInfo.SpanY = 3;

                                break;

                            case "EPA":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 0.5);
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
                                oCol.RowLayoutColumnInfo.SpanY = 3;

                                break;

                            case "DescrUtente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (int)(refWidth * 1.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescrUtenteValidazione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (int)(refWidth * 1.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "DescrUtenteSospensione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (int)(refWidth * 1.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 3;
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
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGridAnteprima_InitializeLayout", this.Name);
            }
        }

        private void ucEasyGridVALIDATI_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                foreach (UltraGridCell ocell in e.Row.Cells)
                {
                    if (ocell.Column.Key.Trim().ToUpper() == "DescrUtente".ToUpper() && ocell.Text.Trim() != "" && ocell.Text.IndexOf("Inserito da") < 0)
                    {
                        ocell.Value = "Inserito da: " + ocell.Value.ToString();
                        if (ocell.Row.Cells["DataEvento"].Text.Trim() != "")
                        {
                            ocell.Value += " in data: " + ((DateTime)ocell.Row.Cells["DataEvento"].Value).ToString("dd/MM/yyyy HH:mm");
                        }
                        e.Row.Update();
                    }

                    if (ocell.Column.Key.Trim().ToUpper() == "DescrUtenteValidazione".ToUpper() && ocell.Text.Trim() != "" && ocell.Text.IndexOf("Validato da") < 0)
                    {
                        ocell.Value = "Validato da: " + ocell.Value.ToString();
                        if (ocell.Row.Cells["DataValidazione"].Text.Trim() != "")
                        {
                            ocell.Value += " in data: " + ((DateTime)ocell.Row.Cells["DataValidazione"].Value).ToString("dd/MM/yyyy HH:mm");
                        }
                        e.Row.Update();
                    }

                    if (ocell.Column.Key.Trim().ToUpper() == "DescrUtenteSospensione".ToUpper() && ocell.Text.Trim() != "" && ocell.Text.IndexOf("Sospeso da") < 0)
                    {
                        ocell.Value = "Sospeso da: " + ocell.Value.ToString();
                        if (ocell.Row.Cells["DataSospensione"].Text.Trim() != "")
                        {
                            ocell.Value += " in data: " + ((DateTime)ocell.Row.Cells["DataSospensione"].Value).ToString("dd/MM/yyyy HH:mm");
                        }
                        e.Row.Update();
                    }
                }
            }
            catch
            {
            }
        }

        private void ucEasyGridDaValidare_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                int refWidth = ((sender as ucEasyGrid).Width - 30) / 10 * 3;
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
                            case "DescrTempo":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth;
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

                            case "DescrProtocollo":
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
                                    oCol.MaxWidth = refWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "Posologia":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (sender as ucEasyGrid).Width - Convert.ToInt32(refWidth * 2) - 30;
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
                                oCol.RowLayoutColumnInfo.SpanY = 2;

                                break;

                            case "DescrUtente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth;
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
                CoreStatics.ExGest(ref ex, "ucEasyGridAnteprima_InitializeLayout", this.Name);
            }
        }

        private void ucEasyGridDaValidare_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                foreach (UltraGridCell ocell in e.Row.Cells)
                {
                    if (ocell.Column.Key.Trim().ToUpper() == "DescrUtente".ToUpper() && ocell.Text.Trim() != "" && ocell.Text.IndexOf("Inserito da") < 0)
                    {
                        ocell.Value = "Inserito da: " + ocell.Value.ToString();
                        if (ocell.Row.Cells["DataEvento"].Text.Trim() != "")
                        {
                            ocell.Value += " in data: " + ((DateTime)ocell.Row.Cells["DataEvento"].Value).ToString("dd/MM/yyyy HH:mm");
                        }
                        e.Row.Update();
                    }
                }
            }
            catch
            {
            }
        }

        private void ucEasyGridDaValidare_ClickCell(object sender, ClickCellEventArgs e)
        {
            bool bVisualizzaOraFine = false;
            bool bVisualizzaEtichetta = false;
            int altezza = 24;
            int larghezza = 6;

            try
            {

                _popuprtf = false;

                _ucEasyGridOrari = null;


                _ucEasyGridOrari = CoreStatics.getGridOrariDaValidare(e.Cell.Row.Cells["ID"].Text, e.Cell.Row.Cells["IDPrescrizione"].Text, ref bVisualizzaOraFine, ref bVisualizzaEtichetta);

                if (_ucEasyGridOrari != null && _ucEasyGridOrari.DataSource != null)
                {
                    if (bVisualizzaOraFine == true)
                        larghezza = larghezza + 6;
                    if (bVisualizzaEtichetta == true)
                        larghezza = larghezza + 9;

                    _ucEasyGridOrari.Size = new Size(this.ucEasyGridDaValidare.Width * larghezza / 10, this.ucEasyGridDaValidare.Height * altezza / 10);

                    _ucEasyGridOrari.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                    CoreStatics.SetEasyUltraGridLayout(ref _ucEasyGridOrari);

                    CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerMain);

                    Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                    Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);


                    this.UltraPopupControlContainerMain.Show(sender as ucEasyGrid, ((ucEasyGrid)sender).PointToScreen(oPoint));


                    _ucEasyGridOrari.DisplayLayout.Bands[0].ClearGroupByColumns();

                    foreach (UltraGridColumn oCol in _ucEasyGridOrari.DisplayLayout.Bands[0].Columns)
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {
                            case "DataOraInizio":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Data/Ora Inizio";
                                oCol.Format = "dddd dd/MM/yyyy HH:mm";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                break;

                            case "DataOraFine":
                                oCol.Hidden = !bVisualizzaOraFine;
                                oCol.Header.Caption = "Data/Ora Fine";
                                oCol.Format = "dddd dd/MM/yyyy HH:mm";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                break;

                            case "EtichettaTempo":
                                oCol.Hidden = !bVisualizzaEtichetta;
                                oCol.Header.Caption = "Descrizione Somministrazione";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                break;

                            default:
                                oCol.Hidden = true;
                                break;
                        }
                    }

                    _ucEasyGridOrari.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
                    _ucEasyGridOrari.Refresh();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGridAnteprima_ClickCell", this.Name);
            }
        }

        private void ucEasyGridVALIDATI_ClickCell(object sender, ClickCellEventArgs e)
        {
            int altezza = 24;
            int larghezza = 12;

            try
            {

                _popuprtf = false;

                _ucEasyGridOrari = null;


                _ucEasyGridOrari = CoreStatics.getGridOrariValidati(e.Cell.Row.Cells["ID"].Text, e.Cell.Row.Cells["IDPrescrizione"].Text);


                if (_ucEasyGridOrari != null && _ucEasyGridOrari.DataSource != null)
                {

                    _ucEasyGridOrari.Size = new Size(this.ucEasyGridVALIDATI.Width * larghezza / 10, this.ucEasyGridVALIDATI.Height * altezza / 8);

                    _ucEasyGridOrari.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                    CoreStatics.SetEasyUltraGridLayout(ref _ucEasyGridOrari);
                    _ucEasyGridOrari.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;

                    CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerMain);

                    Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                    Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);


                    this.UltraPopupControlContainerMain.Show(sender as ucEasyGrid, ((ucEasyGrid)sender).PointToScreen(oPoint));


                    _ucEasyGridOrari.DisplayLayout.Bands[0].ClearGroupByColumns();

                    foreach (UltraGridColumn oCol in _ucEasyGridOrari.DisplayLayout.Bands[0].Columns)
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {
                            case "DataRiferimento":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Data";
                                oCol.Format = "dddd dd/MM/yyyy HH:mm";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.VisiblePosition = 1;
                                break;

                            case "DescStato":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Stato";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.VisiblePosition = 2;
                                break;

                            case "DescTipo":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Tipo";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.VisiblePosition = 3;
                                break;

                            default:
                                oCol.Hidden = true;
                                break;
                        }
                    }

                    _ucEasyGridOrari.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
                    _ucEasyGridOrari.Refresh();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGridAnteprima_ClickCell", this.Name);
            }
        }

        #endregion

        private void ucEasyGrid_MouseEnterElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {


        }

        private void ucEasyGrid_MouseLeaveElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {




        }

        private void PopUpMenu(CellButtonUIElement element)
        {

            try
            {

                var menu = (PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"];

                ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_EDIT]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);
                ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_TASK]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_TASKINFERMIERISTICO_32);
                ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_DEL]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);
                ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_COPY]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_COPIA_32);
                ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_STATO]).InstanceProps.AppearancesLarge.Appearance.Image =
                    (element.Row.Cells["CodStatoContinuazione"].Text == EnumStatoContinuazione.AP.ToString() ? Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32) : Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_32));
                ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_STATO]).InstanceProps.Caption =
                    (element.Row.Cells["CodStatoContinuazione"].Text == EnumStatoContinuazione.AP.ToString() ?
                    CoreStatics.GetEnumDescription(EnumStatoContinuazione.CH) : CoreStatics.GetEnumDescription(EnumStatoContinuazione.AP));

                menu.Tools[CoreStatics.C_COL_BTN_EDIT].InstanceProps.Visible = (element.Row.Cells["PermessoModifica"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                menu.Tools[CoreStatics.C_COL_BTN_TASK].InstanceProps.Visible = (element.Row.Cells["PermessoTaskInfermiristici"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                menu.Tools[CoreStatics.C_COL_BTN_DEL].InstanceProps.Visible = (element.Row.Cells["PermessoCancella"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                menu.Tools[CoreStatics.C_COL_BTN_COPY].InstanceProps.Visible = (element.Row.Cells["PermessoCopia"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                menu.Tools[CoreStatics.C_COL_BTN_STATO].InstanceProps.Visible = (element.Row.Cells["PermessoModifica"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);

                this.ucEasyGrid.Selected.Rows.Clear();
                this.ucEasyGrid.Selected.Rows.Add(element.Row);
                this.ucEasyGrid.ActiveRow = element.Row;
                ((PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"]).ClosePopup();
                ((PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"]).ShowPopup(new Point(element.Rect.Right, element.Rect.Bottom));

                Debug.WriteLine("Rect Element: " + element.Rect.ToString() + Environment.NewLine);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
            }

        }

        private void ucTerapieFarmacologiche_Leave(object sender, EventArgs e)
        {
            if (((PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"]).IsOpen == true)
            {
                ((PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"]).ClosePopup();
            }
        }

    }
}
