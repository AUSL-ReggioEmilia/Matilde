using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinTree;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;

namespace UnicodeSrl.ScciCore
{
    public partial class ucSchede : UserControl, Interfacce.IViewUserControlMiddle
    {

        public ucSchede()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            _ucc = (UserControl)this;
        }

        internal List<String> XpFiltro_CodScheda { get; set; }

        #region Declare

        private bool _ambulatoriale = false;

        EnumMaschere _mascheraselezionetiposcheda = EnumMaschere.SelezioneTipoScheda;
        EnumMaschere _mascherascheda = EnumMaschere.Scheda;

        private UserControl _ucc = null;

        bool _bRuoloAbilitatoInserisci = false;
        bool _bRuoloAbilitatoModifica = false;
        bool _bRuoloAbilitatoCancella = false;


        private Dictionary<string, Bitmap> oIcone = new Dictionary<string, Bitmap>();

        #endregion

        #region Interface

        public void Aggiorna()
        {
            if (this.IsDisposed == false)
            {
                CoreStatics.SetNavigazione(false);

                try
                {
                    this.BloccaPulsanti();

                    this.CaricaUltraTreeView();

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "Aggiorna", this.Name);
                }

                CoreStatics.SetNavigazione(true);
            }

        }

        public void Carica()
        {



            CoreStatics.SetNavigazione(false);

            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
            CoreStatics.CoreApplication.IDSchedaSelezionata = "";

            this.BloccaPulsanti();

            try
            {
                CheckAmbulatoriale();

                Ruoli r = CoreStatics.CoreApplication.Sessione.Utente.Ruoli;
                _bRuoloAbilitatoInserisci = (r.RuoloSelezionato.Esiste(EnumModules.Schede_Inserisci));
                _bRuoloAbilitatoModifica = (r.RuoloSelezionato.Esiste(EnumModules.Schede_Modifica));
                _bRuoloAbilitatoCancella = (r.RuoloSelezionato.Esiste(EnumModules.Schede_Cancella));

                this.ucEasyButtonNuovo.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NUOVO_256);
                this.ucEasyButtonModifica.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_256);
                this.ucEasyButtonRilievo.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_256);
                this.ucEasyButtonZoom.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CTRL_INGRANDISCI_256);
                this.ucEasyButtonValida.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_256);
                this.ucEasyButtonRevisione.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NUOVAREVISIONE_256);
                this.ucEasyButtonElimina.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_256);

                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Checked = false;

                this.ucEasyButtonNuovo.PercImageFill = 0.75F;
                this.ucEasyButtonNuovo.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ucEasyButtonModifica.PercImageFill = 0.75F;
                this.ucEasyButtonModifica.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ucEasyButtonRilievo.PercImageFill = 0.75F;
                this.ucEasyButtonRilievo.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ucEasyButtonZoom.PercImageFill = 0.75F;
                this.ucEasyButtonZoom.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ucEasyButtonValida.PercImageFill = 0.75F;
                this.ucEasyButtonValida.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ucEasyButtonRevisione.PercImageFill = 0.75F;
                this.ucEasyButtonRevisione.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ucEasyButtonElimina.PercImageFill = 0.75F;
                this.ucEasyButtonElimina.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);

                this.InizializzaFiltri();
                this.InizializzaUltraTreeView();

                this.uchkDaCompilare.Checked = false;

                if (this.ucEasyGridFiltroEpisodio.Rows != null)
                    if (this.ucEasyGridFiltroEpisodio.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroEpisodio.ActiveRow = null;
                        this.ucEasyGridFiltroEpisodio.Selected.Rows.Clear();
                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridFiltroEpisodio, this.ucEasyGridFiltroEpisodio.DisplayLayout.Bands[0].Columns[0].Key, "1");
                    }

                this.Aggiorna();

                if (CoreStatics.CoreApplication.Episodio != null)
                {
                    string skey = string.Format(@"{0}\{1}", EnumEntita.EPI.ToString(), CoreStatics.CoreApplication.Episodio.ID);
                    UltraTreeNode oNode = this.ucEasyTreeView.GetNodeByKey(skey);
                    if (oNode != null)
                    {
                        oNode.Selected = true;
                        this.ucEasyTreeView.ActiveNode = oNode;
                        this.ucEasyTreeView.PerformAction(UltraTreeAction.ExpandNode, false, false);
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

            CoreStatics.SetNavigazione(true);

            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

        }

        public void Ferma()
        {

            try
            {

                oIcone = new Dictionary<string, Bitmap>();

                CoreStatics.SetContesto(EnumEntita.SCH, null);
                CoreStatics.SetContesto(EnumEntita.APP, null);
                CoreStatics.SetContesto(EnumEntita.EPI, null);
                CoreStatics.SetContesto(EnumEntita.PAZ, null);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region SubRoutine

        private void CheckAmbulatoriale()
        {
            _ambulatoriale = false;
            if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.Ambulatoriale_Schede) _ambulatoriale = true;

            _mascheraselezionetiposcheda = EnumMaschere.SelezioneTipoScheda;
            _mascherascheda = EnumMaschere.Scheda;
            if (_ambulatoriale)
            {
                _mascheraselezionetiposcheda = EnumMaschere.Ambulatoriale_SelezioneTipoScheda;
                _mascherascheda = EnumMaschere.Ambulatoriale_Scheda;
            }

        }

        private void InizializzaFiltri()
        {
            if (this.IsDisposed == false)
            {
                try
                {
                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                    op.Parametro.Add("DatiEstesi", "1");

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataSet ds = Database.GetDatasetStoredProc("MSP_SelAlberoSchede", spcoll);

                    this.ucEasyGridFiltroTipo.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[4], true);
                    this.ucEasyGridFiltroTipo.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Tipo";
                    this.ucEasyGridFiltroTipo.Refresh();

                    List<KeyValuePair<string, string>> oItems = new List<KeyValuePair<string, string>>();
                    oItems.Add(new KeyValuePair<string, string>(CoreStatics.GC_TUTTI, CoreStatics.GC_TUTTI));
                    oItems.Add(new KeyValuePair<string, string>("1", "Attivi"));
                    oItems.Add(new KeyValuePair<string, string>("2", "Chiusi"));
                    oItems.Add(new KeyValuePair<string, string>("3", "Nessuno"));
                    this.ucEasyGridFiltroEpisodio.DataSource = oItems;
                    this.ucEasyGridFiltroEpisodio.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                    this.ucEasyGridFiltroEpisodio.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Episodio";
                    this.ucEasyGridFiltroEpisodio.Refresh();

                    this.ucEasyGridFiltroAgende.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[5], true);
                    this.ucEasyGridFiltroAgende.DisplayLayout.Bands[0].Columns["Descrizione"].Header.Caption = "Agende";
                    this.ucEasyGridFiltroAgende.Refresh();

                    oItems = new List<KeyValuePair<string, string>>();
                    oItems.Add(new KeyValuePair<string, string>(CoreStatics.GC_TUTTI, CoreStatics.GC_TUTTI));
                    oItems.Add(new KeyValuePair<string, string>("1", "Aperti"));
                    oItems.Add(new KeyValuePair<string, string>("2", "Chiusi"));
                    this.ucEasyGridFiltraAppuntamenti.DataSource = oItems;
                    this.ucEasyGridFiltraAppuntamenti.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                    this.ucEasyGridFiltraAppuntamenti.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Appuntamenti";
                    this.ucEasyGridFiltraAppuntamenti.Refresh();

                    this.ubApplicaFiltro.PercImageFill = 0.75F;
                    this.ubApplicaFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                    this.drFiltro.Value = null;
                    this.udteFiltroDA.Value = null;
                    this.udteFiltroA.Value = null;

                    this.uchkDaCompilare.Checked = true;

                    this.uchkFiltro.Checked = false;

                    if (this.ucEasyGridFiltroEpisodio.Rows.Count > 0)
                    {
                        this.ucEasyGridFiltroEpisodio.ActiveRow = null;
                        this.ucEasyGridFiltroEpisodio.Selected.Rows.Clear();
                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGridFiltroEpisodio, this.ucEasyGridFiltroEpisodio.DisplayLayout.Bands[0].Columns[0].Key, CoreStatics.GC_TUTTI);
                    }

                }
                catch (Exception)
                {

                }
            }

        }

        private void BloccaPulsanti()
        {
            if (this.IsDisposed == false)
            {
                this.ucEasyButtonNuovo.Enabled = false;
                this.ucEasyButtonModifica.Enabled = false;
                this.ucEasyButtonRilievo.Enabled = false;
                this.ucEasyButtonZoom.Enabled = false;
                this.ucEasyButtonElimina.Enabled = false;
                this.ucEasyButtonValida.Enabled = false;
                this.ucEasyButtonRevisione.Enabled = false;
            }

        }

        private void ControllaPulsanti()
        {

            try
            {

                if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                {
                    BloccaPulsanti();
                    return;
                }

                if (this.ucEasyTreeView.ActiveNode.Tag.GetType() == typeof(DataRow))
                {

                    DataRow oDr = (DataRow)this.ucEasyTreeView.ActiveNode.Tag;
                    if (this.ucAnteprimaRtfStorico.MovScheda != null)
                    {
                        this.ucEasyButtonNuovo.Enabled = (_bRuoloAbilitatoInserisci && int.Parse(oDr["PermessoInserisci"].ToString()) == 1);
                        this.ucEasyButtonModifica.Enabled = (_bRuoloAbilitatoModifica == true && int.Parse(oDr["PermessoModifica"].ToString()) == 1 && !this.ucAnteprimaRtfStorico.Storicizzata);
                        this.ucEasyButtonRilievo.Enabled = (_bRuoloAbilitatoModifica == true && int.Parse(oDr["PermessoModifica"].ToString()) == 1 && !this.ucAnteprimaRtfStorico.Storicizzata);
                        this.ucEasyButtonElimina.Enabled = (_bRuoloAbilitatoCancella == true && int.Parse(oDr["PermessoCancella"].ToString()) == 1 && !this.ucAnteprimaRtfStorico.Storicizzata);
                        this.ucEasyButtonValida.Enabled = (int.Parse(oDr["PermessoValida"].ToString()) == 1 || int.Parse(oDr["PermessoSvalida"].ToString()) == 1 || int.Parse(oDr["PermessoChiusuraCartellaAmbulatoriale"].ToString()) == 1);
                        this.ucEasyButtonRevisione.Enabled = (int.Parse(oDr["PermessoRevisione"].ToString()) == 1);
                        this.ucEasyButtonZoom.Enabled = true;
                    }
                    else
                    {
                        this.ucEasyButtonNuovo.Enabled = (_bRuoloAbilitatoInserisci && int.Parse(oDr["PermessoInserisci"].ToString()) == 1);
                        this.ucEasyButtonModifica.Enabled = (_bRuoloAbilitatoModifica == true && int.Parse(oDr["PermessoModifica"].ToString()) == 1);
                        this.ucEasyButtonRilievo.Enabled = (_bRuoloAbilitatoModifica == true && int.Parse(oDr["PermessoModifica"].ToString()) == 1);
                        this.ucEasyButtonElimina.Enabled = (_bRuoloAbilitatoCancella == true && int.Parse(oDr["PermessoCancella"].ToString()) == 1);
                        this.ucEasyButtonValida.Enabled = (int.Parse(oDr["PermessoValida"].ToString()) == 1 || int.Parse(oDr["PermessoSvalida"].ToString()) == 1 || int.Parse(oDr["PermessoChiusuraCartellaAmbulatoriale"].ToString()) == 1);
                        this.ucEasyButtonRevisione.Enabled = (int.Parse(oDr["PermessoRevisione"].ToString()) == 1);
                        this.ucEasyButtonZoom.Enabled = false;
                    }

                    if (int.Parse(oDr["InEvidenza"].ToString()) == 1)
                        this.ucEasyButtonRilievo.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_INRILIEVORIMUOVI_256);
                    else
                        this.ucEasyButtonRilievo.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_256);

                    if (int.Parse(oDr["Validata"].ToString()) == 1)
                    {
                        if (this.ucAnteprimaRtfStorico.MovScheda != null && this.ucAnteprimaRtfStorico.MovScheda.PermessoUAFirma == 0)
                        {
                            this.ucEasyButtonValida.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_256);
                        }
                        else
                        {
                            this.ucEasyButtonValida.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSODFIRMA_256);
                        }
                    }
                    else
                    {
                        if (this.ucAnteprimaRtfStorico.MovScheda != null && this.ucAnteprimaRtfStorico.MovScheda.PermessoUAFirma == 0)
                        {
                            this.ucEasyButtonValida.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_256);
                        }
                        else
                        {
                            this.ucEasyButtonValida.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTOFIRMA_256);
                        }
                    }

                }
                else
                {

                    this.ucEasyButtonRilievo.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_256);

                    if (this.ucEasyTreeView.ActiveNode.Tag.ToString() == CoreStatics.TV_ROOT)
                    {
                        this.ucEasyButtonNuovo.Enabled = _bRuoloAbilitatoInserisci;
                        this.ucEasyButtonModifica.Enabled = false;
                        this.ucEasyButtonRilievo.Enabled = false;
                        this.ucEasyButtonElimina.Enabled = false;
                        this.ucEasyButtonValida.Enabled = false;
                        this.ucEasyButtonRevisione.Enabled = false;
                        this.ucEasyButtonZoom.Enabled = false;
                    }
                    else if (this.ucEasyTreeView.ActiveNode.Tag.ToString() == CoreStatics.TV_AGENDE)
                    {
                        this.ucEasyButtonNuovo.Enabled = false;
                        this.ucEasyButtonModifica.Enabled = false;
                        this.ucEasyButtonRilievo.Enabled = false;
                        this.ucEasyButtonElimina.Enabled = false;
                        this.ucEasyButtonValida.Enabled = false;
                        this.ucEasyButtonRevisione.Enabled = false;
                        this.ucEasyButtonZoom.Enabled = false;
                    }
                    else if (this.ucEasyTreeView.ActiveNode.Tag.ToString() == CoreStatics.TV_CARTELLA)
                    {
                        this.ucEasyButtonNuovo.Enabled = false;
                        this.ucEasyButtonModifica.Enabled = false;
                        this.ucEasyButtonRilievo.Enabled = false;
                        this.ucEasyButtonElimina.Enabled = false;
                        this.ucEasyButtonValida.Enabled = false;
                        this.ucEasyButtonRevisione.Enabled = false;
                        this.ucEasyButtonZoom.Enabled = false;
                    }
                }

            }
            catch (Exception)
            {

            }

        }



















        private Image getImageStatoCalcolato(string codscheda, string codstato, string colorestato)
        {

            Bitmap oret = new Bitmap(16, 16);

            try
            {

                if (oIcone.ContainsKey(codscheda + codstato) == false)
                {
                    oIcone.Add(codscheda + codstato, CoreStatics.CreateSolidBitmap(CoreStatics.GetColorFromString(colorestato)));
                }

                oret = oIcone[codscheda + codstato];

            }
            catch (Exception)
            {

            }

            return oret;

        }

        #endregion

        #region Controlli per-Cancellazione

        private bool isNodoRoot(ref UltraTreeNode roNode)
        {
            bool bRoot = false;

            if (this.ucEasyTreeView.ActiveNode.Tag.GetType() != typeof(DataRow))
            {
                bRoot = true;
            }

            return bRoot;
        }

        private bool isActiveNodeCancellabile()
        {
            bool bDelete = false;

            if (CoreStatics.CoreApplication.Cartella == null || !CoreStatics.CoreApplication.Cartella.CartellaChiusa)
            {

                if (this.ucEasyTreeView.ActiveNode.Tag.GetType() == typeof(DataRow))
                {

                    DataRow oDr = (DataRow)this.ucEasyTreeView.ActiveNode.Tag;
                    if (this.ucAnteprimaRtfStorico.MovScheda != null)
                    {
                        bDelete = (_bRuoloAbilitatoCancella == true && int.Parse(oDr["PermessoCancella"].ToString()) == 1 && !this.ucAnteprimaRtfStorico.Storicizzata);
                    }
                    else
                    {
                        bDelete = (_bRuoloAbilitatoCancella == true && int.Parse(oDr["PermessoCancella"].ToString()) == 1);
                    }

                }

                bDelete = true;
            }

            return bDelete;
        }

        private void stopTimer()
        {
            if (CoreStatics.CoreApplicationContext.MainForm != null && CoreStatics.CoreApplicationContext.MainForm is frmMain && (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh != 0)
                (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = 0;
        }
        private void startTimer()
        {
            if (CoreStatics.CoreApplicationContext.MainForm != null && CoreStatics.CoreApplicationContext.MainForm is frmMain)
                (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.TimerRefresh;
        }

        #endregion

        #region UltraTree

        private void InizializzaUltraTreeView()
        {
            this.ucEasyTreeView.Override.Multiline = Infragistics.Win.DefaultableBoolean.True;
            this.ucEasyTreeView.AllowDrop = true;
            this.ucEasyTreeView.PerformLayout();
        }

        private void CaricaUltraTreeView()
        {

            UltraTreeNode oNodeRoot = null;
            UltraTreeNode oNodeParent = null;
            UltraTreeNode oNode = null;
            UltraTreeNode oNodeCartella = null;
            UltraTreeNode oNodeAgende = null;

            string oNodeKeySelezionato = string.Empty;

            string sKey = "";
            bool bFiltro = false;

            try
            {
                if (CoreStatics.CoreApplication.Paziente != null && this.IsDisposed == false)
                {
                    CoreStatics.SetContesto(EnumEntita.SCH, null);
                    CoreStatics.SetContesto(EnumEntita.APP, null);
                    CoreStatics.SetContesto(EnumEntita.EPI, null);
                    CoreStatics.SetContesto(EnumEntita.PAZ, null);


                    CoreStatics.CoreApplication.IDSchedaSelezionata = "";
                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                    op.Parametro.Add("DatiEstesi", "0");

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

                    if (this.ucEasyGridFiltroTipo.ActiveRow != null && this.ucEasyGridFiltroTipo.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                    {
                        op.Parametro.Add("CodScheda", this.ucEasyGridFiltroTipo.ActiveRow.Cells["Codice"].Text);
                        bFiltro = true;
                    }

                    if (this.ucEasyGridFiltroEpisodio.ActiveRow != null && this.ucEasyGridFiltroEpisodio.ActiveRow.Cells["Key"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                    {
                        op.Parametro.Add("StatoEpisodio", this.ucEasyGridFiltroEpisodio.ActiveRow.Cells["Key"].Text);
                        bFiltro = true;
                    }

                    if (this.ucEasyGridFiltroAgende.ActiveRow != null && this.ucEasyGridFiltroAgende.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                    {
                        op.Parametro.Add("CodAgenda", this.ucEasyGridFiltroAgende.ActiveRow.Cells["Codice"].Text);
                        bFiltro = true;
                    }

                    if (this.ucEasyGridFiltraAppuntamenti.ActiveRow != null && this.ucEasyGridFiltraAppuntamenti.ActiveRow.Cells["Key"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                    {
                        op.Parametro.Add("StatoAppuntamento", this.ucEasyGridFiltraAppuntamenti.ActiveRow.Cells["Key"].Text);
                        bFiltro = true;
                    }

                    if (this.uchkDaCompilare.Checked == false)
                    {
                        op.Parametro.Add("DaCompilare", "0");
                        bFiltro = true;
                    }

                    this.uchkFiltro.Checked = bFiltro;

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataSet ds = Database.GetDatasetStoredProc("MSP_SelAlberoSchede", spcoll);

                    if (!this.ucEasyTreeView.IsDisposed)
                    {

                        if (this.ucEasyTreeView.ActiveNode != null) { oNodeKeySelezionato = this.ucEasyTreeView.ActiveNode.Key; }

                        this.ucEasyTreeView.Nodes.Clear();

                        oNodeRoot = new UltraTreeNode(CoreStatics.TV_ROOT, CoreStatics.CoreApplication.Paziente.Cognome + " " + CoreStatics.CoreApplication.Paziente.Nome);
                        oNodeRoot.LeftImages.Add(CoreStatics.CoreApplication.Paziente.Sesso.ToUpper() == "M" ? Risorse.GetImageFromResource(Risorse.GC_PAZIENTEMASCHIO_32) : Risorse.GetImageFromResource(Risorse.GC_PAZIENTEFEMMINA_32));
                        oNodeRoot.Tag = CoreStatics.TV_ROOT;
                        this.ucEasyTreeView.Nodes.Add(oNodeRoot);

                        #region Schede Paziente
                        bool bAddNode = true;
                        bool bChild = false;
                        while (bAddNode == true)
                        {

                            bAddNode = false;

                            foreach (DataRow oDr in ds.Tables[0].Rows)
                            {

                                if (!this.ucEasyTreeView.IsDisposed)
                                {

                                    try
                                    {
                                        if (bChild == false)
                                        {
                                            if (oDr["IDSchedaPadre"] == DBNull.Value)
                                            {
                                                if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                {
                                                    sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                    oNodeCartella = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNodeCartella == null)
                                                    {
                                                        oNodeCartella = new UltraTreeNode(sKey, oDr["DescrizioneRaggruppamento"].ToString());
                                                        oNodeCartella.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32));
                                                        oNodeCartella.Tag = CoreStatics.TV_CARTELLA;
                                                        oNodeRoot.Nodes.Add(oNodeCartella);
                                                    }
                                                }
                                                oNode = new UltraTreeNode(oDr["CodEntita"].ToString() + @"\" + oDr["IDScheda"].ToString(), oDr["Descrizione"].ToString());

                                                if (oDr.IsNull("IDCartellaAmbulatoriale"))
                                                {
                                                    if (int.Parse(oDr["Contenitore"].ToString()) == 1)
                                                    { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32)); }
                                                    else
                                                    { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32)); }
                                                }
                                                else
                                                {
                                                    if (oDr["CodStatoCartellaAmbulatoriale"].ToString() == "AP")
                                                    {
                                                        oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDACARTELLAAMBULATORIALE_32));
                                                    }
                                                    else
                                                    {
                                                        oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDACARTELLAAMBULATORIALECHIUSAD_32));
                                                    }
                                                }

                                                if (oDr["CodStatoSchedaCalcolato"].ToString() != string.Empty) oNode.LeftImages.Add(getImageStatoCalcolato(oDr["CodScheda"].ToString(), oDr["CodStatoSchedaCalcolato"].ToString(), oDr["ColoreStatoCalcolato"].ToString()));
                                                if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                if (int.Parse(oDr["Riservata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_STATUSFLAGRED_32));

                                                if (int.Parse(oDr["Validata"].ToString()) == 1)
                                                {
                                                    if (oDr["IDDocumentoFirmato"] != DBNull.Value)
                                                        oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSOFIRMA_32));
                                                    else
                                                        oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32));
                                                }

                                                if (int.Parse(oDr["PermessoValida"].ToString()) == 1 && int.Parse(oDr["Validata"].ToString()) == 0) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_32));
                                                oNode.Tag = oDr;
                                                if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                {
                                                    oNodeCartella.Nodes.Add(oNode);
                                                }
                                                else
                                                {
                                                    oNodeRoot.Nodes.Add(oNode);
                                                }
                                                bAddNode = true;
                                            }
                                        }
                                        else
                                        {
                                            if (oDr["IDSchedaPadre"] != DBNull.Value)
                                            {
                                                sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNodeParent != null)
                                                {
                                                    sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDScheda"].ToString();
                                                    oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNode == null)
                                                    {
                                                        if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                        {
                                                            oNodeCartella = this.ucEasyTreeView.GetNodeByKey(oDr["CodEntita"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString());
                                                            if (oNodeCartella == null)
                                                            {
                                                                oNodeCartella = new UltraTreeNode(oDr["CodEntita"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString(), oDr["DescrizioneRaggruppamento"].ToString());
                                                                oNodeCartella.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32));
                                                                oNodeCartella.Tag = CoreStatics.TV_CARTELLA;
                                                                oNodeParent.Nodes.Add(oNodeCartella);
                                                            }
                                                        }
                                                        oNode = new UltraTreeNode(sKey, oDr["Descrizione"].ToString());

                                                        if (oDr.IsNull("IDCartellaAmbulatoriale"))
                                                        {
                                                            if (int.Parse(oDr["Contenitore"].ToString()) == 1)
                                                            { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32)); }
                                                            else
                                                            { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32)); }
                                                        }
                                                        else
                                                        {
                                                            if (oDr["CodStatoCartellaAmbulatoriale"].ToString() == "AP")
                                                            {
                                                                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDACARTELLAAMBULATORIALE_32));
                                                            }
                                                            else
                                                            {
                                                                oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDACARTELLAAMBULATORIALECHIUSAD_32));
                                                            }
                                                        }

                                                        if (oDr["CodStatoSchedaCalcolato"].ToString() != string.Empty) oNode.LeftImages.Add(getImageStatoCalcolato(oDr["CodScheda"].ToString(), oDr["CodStatoSchedaCalcolato"].ToString(), oDr["ColoreStatoCalcolato"].ToString()));
                                                        if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                        if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                        if (int.Parse(oDr["Riservata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_STATUSFLAGRED_32));

                                                        if (int.Parse(oDr["Validata"].ToString()) == 1)
                                                        {
                                                            if (oDr["IDDocumentoFirmato"] != DBNull.Value)
                                                                oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSOFIRMA_32));
                                                            else
                                                                oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32));
                                                        }

                                                        if (int.Parse(oDr["PermessoValida"].ToString()) == 1 && int.Parse(oDr["Validata"].ToString()) == 0) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_32));
                                                        oNode.Tag = oDr;
                                                        if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                        {
                                                            oNodeCartella.Nodes.Add(oNode);
                                                        }
                                                        else
                                                        {
                                                            oNodeParent.Nodes.Add(oNode);
                                                        }
                                                        bAddNode = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                                        bAddNode = false;
                                    }

                                }

                            }

                            bChild = true;

                        }
                        #endregion

                        #region Schede Appuntamenti
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            oNodeAgende = new UltraTreeNode(@"APP\AGENDE", "Agende");
                            oNodeAgende.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_AGENDA_32));
                            oNodeAgende.Tag = CoreStatics.TV_AGENDE;
                            oNodeRoot.Nodes.Add(oNodeAgende);
                        }
                        bAddNode = true;
                        bChild = false;
                        while (bAddNode == true)
                        {

                            bAddNode = false;

                            foreach (DataRow oDr in ds.Tables[1].Rows)
                            {

                                if (!this.ucEasyTreeView.IsDisposed)
                                {

                                    try
                                    {
                                        if (bChild == false)
                                        {
                                            if (oDr["IDSchedaPadre"] == DBNull.Value)
                                            {

                                                sKey = oDr["CodEntita"].ToString() + @"\" + oDr["CodAgenda"].ToString();
                                                oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNodeParent == null)
                                                {
                                                    oNodeParent = new UltraTreeNode(sKey, oDr["Agenda"].ToString());
                                                    oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_AGENDA_32));
                                                    oNodeParent.Tag = CoreStatics.TV_AGENDE;
                                                    oNodeAgende.Nodes.Add(oNodeParent);
                                                    bAddNode = true;
                                                }
                                                sKey = sKey + @"\" + oDr["IDAppuntamento"].ToString();
                                                oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNode == null)
                                                {
                                                    oNode = new UltraTreeNode(sKey, oDr["Appuntamento"].ToString());
                                                    oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_APPUNTAMENTO_32));
                                                    oNode.Tag = oDr;
                                                    oNodeParent.Nodes.Add(oNode);
                                                    bAddNode = true;
                                                }
                                                oNodeParent = oNode;

                                                if (oDr["IDScheda"] != DBNull.Value)
                                                {
                                                    if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                    {
                                                        sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                        oNodeCartella = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                        if (oNodeCartella == null)
                                                        {
                                                            oNodeCartella = new UltraTreeNode(sKey, oDr["DescrizioneRaggruppamento"].ToString());
                                                            oNodeCartella.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32));
                                                            oNodeCartella.Tag = CoreStatics.TV_CARTELLA;
                                                            oNodeParent.Nodes.Add(oNodeCartella);
                                                        }
                                                    }
                                                    oNode = new UltraTreeNode(sKey + @"\" + oDr["IDScheda"].ToString(), oDr["Descrizione"].ToString());

                                                    if (int.Parse(oDr["Contenitore"].ToString()) == 1)
                                                    { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32)); }
                                                    else
                                                    { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32)); }

                                                    if (oDr["CodStatoSchedaCalcolato"].ToString() != string.Empty) oNode.LeftImages.Add(getImageStatoCalcolato(oDr["CodScheda"].ToString(), oDr["CodStatoSchedaCalcolato"].ToString(), oDr["ColoreStatoCalcolato"].ToString()));
                                                    if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                    if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                    if (int.Parse(oDr["Riservata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_STATUSFLAGRED_32));
                                                    if (int.Parse(oDr["PermessoValida"].ToString()) == 1 && int.Parse(oDr["Validata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32));
                                                    if (int.Parse(oDr["PermessoValida"].ToString()) == 1 && int.Parse(oDr["Validata"].ToString()) == 0) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_32));
                                                    oNode.Tag = oDr;
                                                    if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                    {
                                                        oNodeCartella.Nodes.Add(oNode);
                                                    }
                                                    else
                                                    {
                                                        oNodeParent.Nodes.Add(oNode);
                                                    }
                                                    bAddNode = true;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            if (oDr["IDSchedaPadre"] != DBNull.Value)
                                            {
                                                sKey = oDr["CodEntita"].ToString() + @"\" + oDr["CodAgenda"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNodeParent != null)
                                                {
                                                    sKey = oDr["CodEntita"].ToString() + @"\" + oDr["CodAgenda"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDScheda"].ToString();
                                                    oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNode == null)
                                                    {
                                                        if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                        {
                                                            oNodeCartella = this.ucEasyTreeView.GetNodeByKey(oDr["CodEntita"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString());
                                                            if (oNodeCartella == null)
                                                            {
                                                                oNodeCartella = new UltraTreeNode(oDr["CodEntita"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString(), oDr["DescrizioneRaggruppamento"].ToString());
                                                                oNodeCartella.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32));
                                                                oNodeCartella.Tag = CoreStatics.TV_CARTELLA;
                                                                oNodeParent.Nodes.Add(oNodeCartella);
                                                            }
                                                        }
                                                        oNode = new UltraTreeNode(sKey, oDr["Descrizione"].ToString());

                                                        if (int.Parse(oDr["Contenitore"].ToString()) == 1)
                                                        { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32)); }
                                                        else
                                                        { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32)); }

                                                        if (oDr["CodStatoSchedaCalcolato"].ToString() != string.Empty) oNode.LeftImages.Add(getImageStatoCalcolato(oDr["CodScheda"].ToString(), oDr["CodStatoSchedaCalcolato"].ToString(), oDr["ColoreStatoCalcolato"].ToString()));
                                                        if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                        if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                        if (int.Parse(oDr["Riservata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_STATUSFLAGRED_32));
                                                        if (int.Parse(oDr["Validata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32));
                                                        if (int.Parse(oDr["PermessoValida"].ToString()) == 1 && int.Parse(oDr["Validata"].ToString()) == 0) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_32));
                                                        oNode.Tag = oDr;
                                                        if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                        {
                                                            oNodeCartella.Nodes.Add(oNode);
                                                        }
                                                        else
                                                        {
                                                            oNodeParent.Nodes.Add(oNode);
                                                        }
                                                        bAddNode = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                                        bAddNode = false;
                                    }

                                }

                            }

                            bChild = true;

                        }
                        #endregion

                        #region Schede di tipo Episodio
                        bAddNode = true;
                        bChild = false;
                        while (bAddNode == true)
                        {

                            bAddNode = false;

                            foreach (DataRow oDr in ds.Tables[2].Rows)
                            {

                                if (!this.ucEasyTreeView.IsDisposed)
                                {

                                    try
                                    {
                                        if (bChild == false)
                                        {
                                            if (oDr["IDSchedaPadre"] == DBNull.Value)
                                            {

                                                sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDEpisodio"].ToString();
                                                oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNodeParent == null)
                                                {
                                                    oNodeParent = new UltraTreeNode(sKey, oDr["Ricovero"].ToString());

                                                    switch (oDr["Dimesso"].ToString())
                                                    {
                                                        case "0":
                                                            oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_LETTO_32));
                                                            break;

                                                        case "1":
                                                            oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_LETTOD_32));
                                                            break;
                                                        case "2":
                                                            oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_LETTODANNULLATO_32));
                                                            break;
                                                        default:
                                                            break;
                                                    }

                                                    oNodeParent.Tag = oDr;
                                                    oNodeRoot.Nodes.Add(oNodeParent);
                                                    bAddNode = true;
                                                }

                                                if (oDr["IDScheda"] != DBNull.Value)
                                                {
                                                    if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                    {
                                                        sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                        oNodeCartella = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                        if (oNodeCartella == null)
                                                        {
                                                            oNodeCartella = new UltraTreeNode(sKey, oDr["DescrizioneRaggruppamento"].ToString());
                                                            oNodeCartella.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32));
                                                            oNodeCartella.Tag = CoreStatics.TV_CARTELLA;
                                                            oNodeParent.Nodes.Add(oNodeCartella);
                                                        }
                                                    }
                                                    oNode = new UltraTreeNode(sKey + @"\" + oDr["IDScheda"].ToString(), oDr["Descrizione"].ToString());

                                                    if (int.Parse(oDr["Contenitore"].ToString()) == 1)
                                                    { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32)); }
                                                    else
                                                    { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32)); }

                                                    if (oDr["CodStatoSchedaCalcolato"].ToString() != string.Empty) oNode.LeftImages.Add(getImageStatoCalcolato(oDr["CodScheda"].ToString(), oDr["CodStatoSchedaCalcolato"].ToString(), oDr["ColoreStatoCalcolato"].ToString()));
                                                    if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                    if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                    if (int.Parse(oDr["Riservata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_STATUSFLAGRED_32));

                                                    if (int.Parse(oDr["Validata"].ToString()) == 1)
                                                    {
                                                        if (oDr["IDDocumentoFirmato"] != DBNull.Value)
                                                            oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSOFIRMA_32));
                                                        else
                                                            oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32));
                                                    }

                                                    if (int.Parse(oDr["PermessoValida"].ToString()) == 1 && int.Parse(oDr["Validata"].ToString()) == 0) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_32));
                                                    oNode.Tag = oDr;
                                                    if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                    {
                                                        oNodeCartella.Nodes.Add(oNode);
                                                    }
                                                    else
                                                    {
                                                        oNodeParent.Nodes.Add(oNode);
                                                    }
                                                    bAddNode = true;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            if (oDr["IDSchedaPadre"] != DBNull.Value)
                                            {
                                                sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                if (oNodeParent != null)
                                                {
                                                    sKey = oDr["CodEntita"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["IDScheda"].ToString();
                                                    oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNode == null)
                                                    {
                                                        if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                        {
                                                            oNodeCartella = this.ucEasyTreeView.GetNodeByKey(oDr["CodEntita"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString());
                                                            if (oNodeCartella == null)
                                                            {
                                                                oNodeCartella = new UltraTreeNode(oDr["CodEntita"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString(), oDr["DescrizioneRaggruppamento"].ToString());
                                                                oNodeCartella.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32));
                                                                oNodeCartella.Tag = CoreStatics.TV_CARTELLA;
                                                                oNodeParent.Nodes.Add(oNodeCartella);
                                                            }
                                                        }
                                                        oNode = new UltraTreeNode(sKey, oDr["Descrizione"].ToString());

                                                        if (int.Parse(oDr["Contenitore"].ToString()) == 1)
                                                        { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32)); }
                                                        else
                                                        { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32)); }

                                                        if (oDr["CodStatoSchedaCalcolato"].ToString() != string.Empty) oNode.LeftImages.Add(getImageStatoCalcolato(oDr["CodScheda"].ToString(), oDr["CodStatoSchedaCalcolato"].ToString(), oDr["ColoreStatoCalcolato"].ToString()));
                                                        if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                        if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                        if (int.Parse(oDr["Riservata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_STATUSFLAGRED_32));
                                                        if (int.Parse(oDr["Validata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32));
                                                        if (int.Parse(oDr["PermessoValida"].ToString()) == 1 && int.Parse(oDr["Validata"].ToString()) == 0) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_32));
                                                        oNode.Tag = oDr;
                                                        if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                        {
                                                            oNodeCartella.Nodes.Add(oNode);
                                                        }
                                                        else
                                                        {
                                                            oNodeParent.Nodes.Add(oNode);
                                                        }
                                                        bAddNode = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                                        bAddNode = false;
                                    }

                                }

                            }

                            bChild = true;

                        }
                        #endregion

                        #region Schede di Episdio Appuntamenti
                        bAddNode = true;
                        bChild = false;
                        while (bAddNode == true)
                        {

                            bAddNode = false;

                            foreach (DataRow oDr in ds.Tables[3].Rows)
                            {

                                if (!this.ucEasyTreeView.IsDisposed)
                                {

                                    try
                                    {
                                        sKey = oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDEpisodio"].ToString();
                                        oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                        if (oNodeParent != null)
                                        {

                                            if (bChild == false)
                                            {
                                                if (oDr["IDSchedaPadre"] == DBNull.Value)
                                                {

                                                    sKey = oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["CodAgenda"].ToString();
                                                    oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNode == null)
                                                    {
                                                        string sKeyage = oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\AGENDE";
                                                        oNodeAgende = this.ucEasyTreeView.GetNodeByKey(sKeyage);
                                                        if (oNodeAgende == null)
                                                        {
                                                            oNodeAgende = new UltraTreeNode(sKeyage, "Agende");
                                                            oNodeAgende.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_AGENDA_32));
                                                            oNodeAgende.Tag = CoreStatics.TV_AGENDE;
                                                            oNodeParent.Nodes.Add(oNodeAgende);
                                                        }
                                                        oNode = new UltraTreeNode(sKey, oDr["Agenda"].ToString());
                                                        oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_AGENDA_32));
                                                        oNode.Tag = CoreStatics.TV_AGENDE;
                                                        oNodeAgende.Nodes.Add(oNode);
                                                        bAddNode = true;
                                                    }
                                                    oNodeParent = oNode;

                                                    sKey = sKey + @"\" + oDr["IDAppuntamento"].ToString();
                                                    oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNode == null)
                                                    {
                                                        oNode = new UltraTreeNode(sKey, oDr["Appuntamento"].ToString());
                                                        oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_APPUNTAMENTO_32));
                                                        oNode.Tag = oDr;
                                                        oNodeParent.Nodes.Add(oNode);
                                                        bAddNode = true;
                                                    }
                                                    oNodeParent = oNode;

                                                    if (oDr["IDScheda"] != DBNull.Value)
                                                    {
                                                        if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                        {
                                                            sKey = oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["CodAgenda"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                            oNodeCartella = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                            if (oNodeCartella == null)
                                                            {
                                                                oNodeCartella = new UltraTreeNode(sKey, oDr["DescrizioneRaggruppamento"].ToString());
                                                                oNodeCartella.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32));
                                                                oNodeCartella.Tag = CoreStatics.TV_CARTELLA;
                                                                oNodeParent.Nodes.Add(oNodeCartella);
                                                            }
                                                        }
                                                        oNode = new UltraTreeNode(sKey + @"\" + oDr["IDScheda"].ToString(), oDr["Descrizione"].ToString());

                                                        if (int.Parse(oDr["Contenitore"].ToString()) == 1)
                                                        { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32)); }
                                                        else
                                                        { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32)); }

                                                        if (oDr["CodStatoSchedaCalcolato"].ToString() != string.Empty) oNode.LeftImages.Add(getImageStatoCalcolato(oDr["CodScheda"].ToString(), oDr["CodStatoSchedaCalcolato"].ToString(), oDr["ColoreStatoCalcolato"].ToString()));
                                                        if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                        if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                        if (int.Parse(oDr["Riservata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_STATUSFLAGRED_32));
                                                        if (int.Parse(oDr["Validata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32));
                                                        if (int.Parse(oDr["PermessoValida"].ToString()) == 1 && int.Parse(oDr["Validata"].ToString()) == 0) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_32));
                                                        oNode.Tag = oDr;
                                                        if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                        {
                                                            oNodeCartella.Nodes.Add(oNode);
                                                        }
                                                        else
                                                        {
                                                            oNodeParent.Nodes.Add(oNode);
                                                        }
                                                        bAddNode = true;
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                if (oDr["IDSchedaPadre"] != DBNull.Value)
                                                {

                                                    sKey = oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["CodAgenda"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString();
                                                    oNodeParent = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                    if (oNodeParent != null)
                                                    {
                                                        sKey = oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["CodAgenda"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDScheda"].ToString();
                                                        oNode = this.ucEasyTreeView.GetNodeByKey(sKey);
                                                        if (oNode == null)
                                                        {
                                                            if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                            {
                                                                oNodeCartella = this.ucEasyTreeView.GetNodeByKey(oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["CodAgenda"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString());
                                                                if (oNodeCartella == null)
                                                                {
                                                                    oNodeCartella = new UltraTreeNode(oDr["CodEntitaPadre"].ToString() + @"\" + oDr["IDRaggruppamento"].ToString() + @"\" + oDr["IDEpisodio"].ToString() + @"\" + oDr["CodAgenda"].ToString() + @"\" + oDr["IDAppuntamento"].ToString() + @"\" + oDr["IDSchedaPadre"].ToString(), oDr["DescrizioneRaggruppamento"].ToString());
                                                                    oNodeCartella.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32));
                                                                    oNodeCartella.Tag = CoreStatics.TV_CARTELLA;
                                                                    oNodeParent.Nodes.Add(oNodeCartella);
                                                                }
                                                            }
                                                            oNode = new UltraTreeNode(sKey, oDr["Descrizione"].ToString());

                                                            if (int.Parse(oDr["Contenitore"].ToString()) == 1)
                                                            { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_32)); }
                                                            else
                                                            { oNode.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_SCHEDA_32)); }

                                                            if (oDr["CodStatoSchedaCalcolato"].ToString() != string.Empty) oNode.LeftImages.Add(getImageStatoCalcolato(oDr["CodScheda"].ToString(), oDr["CodStatoSchedaCalcolato"].ToString(), oDr["ColoreStatoCalcolato"].ToString()));
                                                            if (int.Parse(oDr["InEvidenza"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_32));
                                                            if (int.Parse(oDr["DatiMancanti"].ToString()) == 1) { oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_32)); }
                                                            if (int.Parse(oDr["Riservata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_STATUSFLAGRED_32));
                                                            if (int.Parse(oDr["Validata"].ToString()) == 1) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32));
                                                            if (int.Parse(oDr["PermessoValida"].ToString()) == 1 && int.Parse(oDr["Validata"].ToString()) == 0) oNode.RightImages.Add(Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_32));
                                                            oNode.Tag = oDr;
                                                            if (oDr["IDRaggruppamento"] != DBNull.Value)
                                                            {
                                                                oNodeCartella.Nodes.Add(oNode);
                                                            }
                                                            else
                                                            {
                                                                oNodeParent.Nodes.Add(oNode);
                                                            }
                                                            bAddNode = true;
                                                        }
                                                    }
                                                }
                                            }

                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                                        bAddNode = false;
                                    }

                                }

                            }

                            bChild = true;

                        }
                        #endregion

                        if (!this.ucEasyTreeView.IsDisposed)
                        {

                            this.ucEasyTreeView.PerformAction(UltraTreeAction.FirstNode, false, false);
                            this.ucEasyTreeView.PerformAction(UltraTreeAction.ExpandNode, false, false);

                            oNode = null;
                            if (oNodeKeySelezionato != string.Empty)
                            {
                                oNode = this.ucEasyTreeView.GetNodeByKey(oNodeKeySelezionato);
                            }

                            if (oNode != null)
                            {
                                oNode.Selected = true;
                                this.ucEasyTreeView.ActiveNode = oNode;
                                this.ucEasyTreeView.PerformAction(UltraTreeAction.ExpandNode, false, false);
                            }

                            if (this.txtTreeFilter.Text != null)
                            {
                                this.filterNodes();
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private bool RicercaNodo(UltraTreeNode oNodeRoot, string sID)
        {

            DataRow oDr = null;
            bool bRet = false;

            try
            {

                if (oNodeRoot == null)
                {
                    oNodeRoot = this.ucEasyTreeView.Nodes[CoreStatics.TV_ROOT];
                }

                foreach (UltraTreeNode oNode in oNodeRoot.Nodes)
                {

                    if (oNode.Tag.GetType() == typeof(DataRow))
                    {
                        oDr = (DataRow)oNode.Tag;
                    }
                    if (oDr != null && oDr["IDScheda"].ToString() == sID)
                    {
                        oNode.Selected = true;
                        this.ucEasyTreeView.ActiveNode = oNode;
                        this.ucEasyTreeView.PerformAction(UltraTreeAction.ExpandNode, false, false);
                        bRet = true;
                        break;
                    }

                    if (oNode.HasNodes == true)
                    {
                        bRet = this.RicercaNodo(oNode, sID);
                        if (bRet == true) { break; }
                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return bRet;

        }

        private void CancellaScheda(UltraTreeNode nodo)
        {

            try
            {
                if (isNodoRoot(ref nodo))
                {
                    return;
                }

                foreach (UltraTreeNode child in nodo.Nodes)
                {
                    this.CancellaScheda(child);
                }

                CoreStatics.CoreApplication.MovSchedaSelezionata = new MovScheda(((DataRow)nodo.Tag)["IDScheda"].ToString(), CoreStatics.CoreApplication.Ambiente);

                if (CoreStatics.CoreApplication.MovSchedaSelezionata.Cancella())
                {

                    string sCodUA = string.Empty;

                    if (CoreStatics.CoreApplication.Trasferimento != null)
                    {
                        sCodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                    }
                    else
                    {
                        sCodUA = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata;
                    }

                    Risposta oRispostaElabora = PluginClientStatics.PluginClient(EnumPluginClient.SCH_CANCELLA_DOPO.ToString(),
                                                                                new object[1] { new object() },
                                                                                CommonStatics.UAPadri(sCodUA, CoreStatics.CoreApplication.Ambiente));

                }

                CoreStatics.CoreApplication.MovSchedaSelezionata = null;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CancellaScheda", this.Name);
            }

        }

        private bool SalvaEValidaMovSchedaSelezionata()
        {
            bool bOK = false;

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

            if (CoreStatics.CoreApplication.MovSchedaSelezionata.CartellaAmbulatorialeCodificata &&
                CoreStatics.CoreApplication.MovSchedaSelezionata.Azione == EnumAzioni.INS &&
                CoreStatics.CoreApplication.MovSchedaSelezionata.IDMovScheda == string.Empty)
            {
                if (CoreStatics.CaricaCartellaAmbulatoriale())
                {
                    CoreStatics.CoreApplication.MovSchedaSelezionata.IDCartellaAmbulatoriale = CoreStatics.CoreApplication.CartellaAmbulatoriale.ID;
                    CoreStatics.CoreApplication.MovSchedaSelezionata.Salva(false);
                    bOK = true;
                }
            }
            else
            {
                CoreStatics.CoreApplication.MovSchedaSelezionata.Salva(false);
                bOK = true;
            }

            if (CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.Validabile == true)
            {
                if (CoreStatics.CoreApplication.MovSchedaSelezionata.Validata != CoreStatics.CoreApplication.MovSchedaSelezionata.ValidataNew)
                {
                    bool bValidataNew = CoreStatics.CoreApplication.MovSchedaSelezionata.ValidataNew;
                    CoreStatics.Validazione(CoreStatics.CoreApplication.MovSchedaSelezionata.IDMovScheda, (bValidataNew == true ? "0" : "1"), ref _ucc, false);
                }
            }
            else
            {
                string sCodUA = string.Empty;

                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    sCodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }
                else
                {
                    sCodUA = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata;
                }

                Risposta oRispostaElabora = PluginClientStatics.PluginClient(EnumPluginClient.SCH_NUOVA_DOPO.ToString(),
                                                                            new object[1] { new object() },
                                                                            CommonStatics.UAPadri(sCodUA, CoreStatics.CoreApplication.Ambiente));
            }

            return bOK;

        }

        #endregion

        #region Events

        private void ucEasyTreeView_AfterActivate(object sender, NodeEventArgs e)
        {
            if (this.IsDisposed == false)
            {
                this.BloccaPulsanti();

                this.ucEasyTreeView.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.EventGroups.AllEvents, false);
                if (e.TreeNode.HasNodes && !e.TreeNode.Expanded) e.TreeNode.Expanded = true;

                try
                {
                    this.ucEasyTreeView.Enabled = false;

                    CoreStatics.SetContesto(EnumEntita.SCH, null);
                    CoreStatics.SetContesto(EnumEntita.APP, null);
                    CoreStatics.SetContesto(EnumEntita.EPI, null);
                    CoreStatics.SetContesto(EnumEntita.PAZ, null);

                    CoreStatics.CoreApplication.IDSchedaSelezionata = "";
                    DataRow oDr = null;
                    if (e.TreeNode.Tag.GetType() == typeof(DataRow))
                    {
                        oDr = (DataRow)e.TreeNode.Tag;
                    }
                    if (oDr != null)
                    {
                        CoreStatics.SetContesto((EnumEntita)Enum.Parse(typeof(EnumEntita), oDr["CodEntita"].ToString()), oDr["IDEntita"].ToString());
                    }
                    if (oDr != null && oDr["IDScheda"] != DBNull.Value)
                    {

                        CoreStatics.SetContesto(EnumEntita.SCH, oDr["IDScheda"].ToString());

                        CoreStatics.CoreApplication.IDSchedaSelezionata = oDr["IDScheda"].ToString();
                        this.ucAnteprimaRtfStorico.MovScheda = new MovScheda(oDr["IDScheda"].ToString(), CoreStatics.CoreApplication.Ambiente);
                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("CodAzioneLock", EnumLock.INFO.ToString());
                        op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                        op.TimeStamp.IDEntita = ((DataRow)e.TreeNode.Tag)["IDScheda"].ToString();
                        SqlParameterExt[] spcoll = new SqlParameterExt[1];
                        string xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        DataTable dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);
                        if (dt.Rows.Count == 1)
                        {
                            DateTime oDt = DateTime.Parse(dt.Rows[0]["DataUltimaModifica"].ToString());


                            this.ucEasyLabelRiga1.Text = dt.Rows[0]["DescrScheda"].ToString();


                            if ((int)dt.Rows[0]["QtaSchedeTotali"] > 1)
                            {
                                this.ucEasyLabelRiga1.Text += string.Format(" ({0} di {1}",
                                dt.Rows[0]["Numero"].ToString(),
                                dt.Rows[0]["QtaSchedeTotali"].ToString());

                                if ((int)dt.Rows[0]["QtaSchedeAttive"] != (int)dt.Rows[0]["QtaSchedeTotali"])
                                {
                                    this.ucEasyLabelRiga1.Text += ", attive " + dt.Rows[0]["QtaSchedeAttive"];
                                }
                                this.ucEasyLabelRiga1.Text += ")";

                                this.ucScrollBarVInfo.Visible = ((int)dt.Rows[0]["QtaSchedeAttive"] > 1 ? true : false);

                            }
                            else
                            {
                                this.ucScrollBarVInfo.Visible = false;
                            }

                            this.ucEasyLabelRiga2.Text = string.Format("Ultima Modifica: {0} Utente: {1}",
CoreStatics.getDateTime(oDt),
dt.Rows[0]["DescrUtenteModifica"].ToString());
                            if (dt.Rows[0]["DataLock"] == DBNull.Value)
                            {
                                this.ucEasyLabelRiga3.Text = "";
                                this.PictureBox.Image = null;
                            }
                            else
                            {
                                oDt = DateTime.Parse(dt.Rows[0]["DataLock"].ToString());
                                this.ucEasyLabelRiga3.Text = string.Format("SCHEDA IN USO DALL'UTENTE {0} DALLE ORE {1}{2}SU {3}",
                                                                dt.Rows[0]["DescrUtenteLock"].ToString(),
                                                                CoreStatics.getDateTime(oDt),
                                                                Environment.NewLine,
                                                                dt.Rows[0]["NomePCLock"].ToString());
                                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32);
                            }
                            ucEasyTableLayoutPanelInfo.Visible = true;
                        }
                        else
                        {
                            this.ucEasyLabelRiga1.Text = "";
                            this.ucEasyLabelRiga2.Text = "";
                            this.ucEasyLabelRiga3.Text = "";
                            this.PictureBox.Image = null;
                            ucEasyTableLayoutPanelInfo.Visible = false;
                        }
                    }
                    else
                    {
                        this.ucAnteprimaRtfStorico.MovScheda = null;
                        this.ucEasyLabelRiga1.Text = "";
                        this.ucEasyLabelRiga2.Text = "";
                        this.ucEasyLabelRiga3.Text = "";
                        this.PictureBox.Image = null;
                        ucEasyTableLayoutPanelInfo.Visible = false;
                    }

                    if (this.IsDisposed == false)
                    {
                        this.ucAnteprimaRtfStorico.RefreshRTF(CoreStatics.CoreApplication.Sessione.Computer.RtfZoom);
                    }

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "ucEasyTreeView_AfterActivate", this.Name);
                }
                finally
                {
                    this.ucEasyTreeView.Enabled = true;
                }

                this.ControllaPulsanti();
                this.ucEasyTreeView.EventManager.SetEnabled(Infragistics.Win.UltraWinTree.EventGroups.AllEvents, true);
            }
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

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                this.InizializzaFiltri();
                this.Aggiorna();
            }
            else
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
        }

        private void ucEasyGridFiltro_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            e.Layout.Bands[0].Override.HeaderClickAction = HeaderClickAction.Select;

            if (e.Layout.Bands[0].Columns.Exists("Codice") == true)
            {
                e.Layout.Bands[0].Columns["Codice"].Hidden = true;
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
            this.Aggiorna();
            this.ucEasyTreeView.Focus();
        }

        private void ucAnteprimaRtfStorico_StoricoChange(object sender, EventArgs e)
        {
            this.ControllaPulsanti();
        }

        private void ucEasyButtonNuovo_Click(object sender, EventArgs e)
        {

            string sIDEntita = string.Empty;
            EnumEntita ee = EnumEntita.XXX;
            DataRow oDr = null;
            string sIDMovScheda = string.Empty;

            bool bRicerca = false;

            try
            {

                if (this.ucEasyTreeView.ActiveNode != null)
                {

                    if (this.ucEasyTreeView.ActiveNode.Tag.GetType() == typeof(DataRow))
                    {
                        oDr = (DataRow)this.ucEasyTreeView.ActiveNode.Tag;
                        sIDEntita = oDr["IDEntita"].ToString();
                        ee = (EnumEntita)Enum.Parse(typeof(EnumEntita), oDr["CodEntita"].ToString());
                        sIDMovScheda = oDr["IDScheda"].ToString();
                    }
                    else
                    {
                        switch (this.ucEasyTreeView.ActiveNode.Tag.ToString())
                        {

                            case CoreStatics.TV_ROOT:
                                sIDEntita = CoreStatics.CoreApplication.Paziente.ID;
                                ee = EnumEntita.PAZ;
                                break;

                            case CoreStatics.TV_AGENDE:
                                break;

                            case CoreStatics.TV_APPUNTAMENTI:
                                string[] appkey = this.ucEasyTreeView.ActiveNode.Key.Split(@"\".ToCharArray());
                                sIDEntita = appkey[appkey.Length - 1];
                                ee = EnumEntita.APP;
                                break;

                            case CoreStatics.TV_RICOVERO:
                                string[] rickey = this.ucEasyTreeView.ActiveNode.Key.Split(@"\".ToCharArray());
                                sIDEntita = rickey[rickey.Length - 1];
                                ee = EnumEntita.EPI;
                                break;

                        }
                    }

                    string pazienteid = "";
                    string codua = "";
                    string episodioid = "";
                    string trasferimentoid = "";
                    if (CoreStatics.CoreApplication.Trasferimento != null)
                    {
                        trasferimentoid = CoreStatics.CoreApplication.Trasferimento.ID;

                        codua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                    }
                    else
                    {
                        codua = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                    }

                    if (CoreStatics.CoreApplication.Paziente != null)
                        pazienteid = CoreStatics.CoreApplication.Paziente.ID;

                    if (CoreStatics.CoreApplication.Episodio != null)
                        episodioid = CoreStatics.CoreApplication.Episodio.ID;

                    CoreStatics.CoreApplication.MovSchedeSelezionate = new List<MovScheda>();
                    CoreStatics.CoreApplication.MovSchedaSelezionata = new MovScheda("", ee,
                                                                                        codua,
                                                                                        pazienteid,
                                                                                        episodioid,
                                                                                        trasferimentoid, CoreStatics.CoreApplication.Ambiente);
                    CoreStatics.CoreApplication.MovSchedaSelezionata.IDEntita = sIDEntita;
                    CoreStatics.CoreApplication.MovSchedaSelezionata.IDSchedaPadre = sIDMovScheda;

                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(_mascheraselezionetiposcheda, false, this.XpFiltro_CodScheda) == System.Windows.Forms.DialogResult.OK)
                    {
                        CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                        bool bEdit = true;

                        if (CoreStatics.CoreApplication.MovSchedeSelezionate != null
                            && CoreStatics.CoreApplication.MovSchedeSelezionate.Count > 1)
                        {
                            bEdit = false;
                            foreach (MovScheda scheda in CoreStatics.CoreApplication.MovSchedeSelezionate)
                            {
                                CoreStatics.CoreApplication.MovSchedaSelezionata = scheda;
                                bRicerca = SalvaEValidaMovSchedaSelezionata();
                            }

                        }
                        else if (CoreStatics.CoreApplication.MovSchedeSelezionate != null
                            && CoreStatics.CoreApplication.MovSchedeSelezionate.Count == 1)
                        {
                            bEdit = true;
                            CoreStatics.CoreApplication.MovSchedaSelezionata = CoreStatics.CoreApplication.MovSchedeSelezionate[0];
                        }


                        if (bEdit)
                        {
                            if (CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.Contenitore == false)
                            {
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(_mascherascheda, false) == DialogResult.OK)
                                {

                                    bRicerca = SalvaEValidaMovSchedaSelezionata();

                                }
                            }
                            else
                            {
                                Gestore oGestore = CoreStatics.GetGestore();
                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.StrutturaXML;
                                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.LayoutXML;
                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.DizionarioValori();
                                oGestore.SchedaDati = new DcSchedaDati();
                                if (oGestore.SchedaDati.Dati.Count == 0) { oGestore.NuovaScheda(); }
                                CoreStatics.CoreApplication.MovSchedaSelezionata.DatiXML = oGestore.SchedaDatiXML;
                                oGestore = null;

                                bRicerca = SalvaEValidaMovSchedaSelezionata();

                            }
                        }
                    }

                    this.Aggiorna();

                    if (bRicerca == true)
                    {
                        this.RicercaNodo(null, CoreStatics.CoreApplication.MovSchedaSelezionata.IDMovScheda);
                    }

                    CoreStatics.CoreApplication.MovSchedaSelezionata = null;
                    CoreStatics.CoreApplication.MovSchedeSelezionate = null;

                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyButtonNuovo_Click", this.Text);
            }

            CoreStatics.SetNavigazione(true);
        }

        private void ucEasyButtonModifica_Click(object sender, EventArgs e)
        {

            try
            {

                CoreStatics.SetNavigazione(false);

                CoreStatics.CaricaPopup(_mascherascheda, EnumEntita.SCH, ((DataRow)this.ucEasyTreeView.ActiveNode.Tag)["IDScheda"].ToString(), ref _ucc, false, false);

                this.Aggiorna();


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyButtonModifica_Click", "ucSchede");
            }

            CoreStatics.SetNavigazione(true);
        }

        private void ucEasyButtonRilievo_Click(object sender, EventArgs e)
        {
            Parametri op = null;
            SqlParameterExt[] spcoll;
            string xmlParam = "";

            DataRow odr = ((DataRow)this.ucEasyTreeView.ActiveNode.Tag);
            string idmovscheda = odr["IDScheda"].ToString();
            string inevidenza = odr["InEvidenza"].ToString();

            try
            {
                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDScheda", idmovscheda);
                op.Parametro.Add("InEvidenza", inevidenza == "1" ? "0" : "1");
                op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_AggMovSchede", spcoll);

                this.Aggiorna();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyButtonRilievo_Click", "ucSchede");
            }
        }

        private void ucEasyButtonZoom_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ucAnteprimaRtfStorico.MovScheda != null)
                {
                    CoreStatics.CoreApplication.MovSchedaSelezionata = this.ucAnteprimaRtfStorico.MovScheda;

                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ZoomAnteprimaRTFScheda) == DialogResult.OK)
                    {

                    }
                }
                else
                {
                    easyStatics.EasyMessageBox("Selezionare una Scheda!", "Zoom Scheda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyButtonZoom_Click", "ucSchede");
            }
        }

        private void ucEasyButtonValida_Click(object sender, EventArgs e)
        {

            DataRow odr = ((DataRow)this.ucEasyTreeView.ActiveNode.Tag);
            string idmovscheda = odr["IDScheda"].ToString();
            string validata = odr["Validata"].ToString();
            string idcartellaambulatoriale = odr["IDCartellaAmbulatoriale"].ToString();
            int permessochiusuracartellaambulatoriale = int.Parse(odr["PermessoChiusuraCartellaAmbulatoriale"].ToString());

            if (permessochiusuracartellaambulatoriale == 0)
            {
                CoreStatics.Validazione(idmovscheda, validata, ref _ucc, true);
            }
            else
            {
                CoreStatics.ChiusuraCartellaAmbulatoriale(idcartellaambulatoriale, ref _ucc);
            }

            this.Aggiorna();
        }

        private void ucEasyButtonRevisione_Click(object sender, EventArgs e)
        {

            try
            {

                CoreStatics.SetNavigazione(false);


                Dictionary<string, object> parametriAggiuntivi = new Dictionary<string, object>();
                parametriAggiuntivi.Add(CoreStatics.C_PARAM_NEW_REV, true);

                CoreStatics.CaricaPopup(_mascherascheda, EnumEntita.SCH, ((DataRow)this.ucEasyTreeView.ActiveNode.Tag)["IDScheda"].ToString(), ref _ucc, false, false, parametriAggiuntivi);

                this.Aggiorna();


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyButtonModifica_Click", "ucSchede");
            }

            CoreStatics.SetNavigazione(true);











        }

        private void ucEasyButtonElimina_Click(object sender, EventArgs e)
        {

            try
            {

                if (isActiveNodeCancellabile())
                {

                    stopTimer();

                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                    CoreStatics.SetNavigazione(false);

                    this.ucEasyTreeView.Enabled = false;

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodAzioneLock", EnumLock.LOCK.ToString());
                    op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                    op.TimeStamp.IDEntita = ((DataRow)this.ucEasyTreeView.ActiveNode.Tag)["IDScheda"].ToString();
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataTable dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);

                    if (dt.Rows.Count == 1 && int.Parse(dt.Rows[0]["Esito"].ToString()) == 1)
                    {

                        if (easyStatics.EasyMessageBox("Sei sicuro di voler eliminare la scheda" + Environment.NewLine + "'" + this.ucEasyTreeView.ActiveNode.Text + "' ?", "Elimina Scheda", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {

                            if (isActiveNodeCancellabile())
                            {
                                this.CancellaScheda(this.ucEasyTreeView.ActiveNode);

                                try
                                {

                                    this.ucEasyTreeView.Enabled = true;

                                    UltraTreeNode oNode = this.ucEasyTreeView.ActiveNode.Parent;
                                    oNode.Selected = true;
                                    this.ucEasyTreeView.ActiveNode = oNode;
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }

                    }
                    else
                    {
                        easyStatics.EasyMessageBox("Scheda bloccata da altro operatore!", "Informazioni Scheda");
                    }

                    op.Parametro.Remove("CodAzioneLock");
                    op.Parametro.Add("CodAzioneLock", EnumLock.UNLOCK.ToString());

                    spcoll = new SqlParameterExt[1];
                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);

                    this.InizializzaFiltri();
                    this.Aggiorna();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyButtonElimina_Click", "ucSchede");
            }

            this.ucEasyTreeView.Enabled = true;

            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);

            CoreStatics.SetNavigazione(true);

            startTimer();

        }

        private void ucScrollBarVInfo_Button(object sender, ScrollbarEventArgs e)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                op.TimeStamp.IDEntita = ((DataRow)this.ucEasyTreeView.ActiveNode.Tag)["IDScheda"].ToString();

                switch (e.TypeButton)
                {

                    case ScrollbarEventArgs.EnumTypeButton.Su:
                        op.Parametro.Add("CodAzioneLock", EnumLock.PRECEDENTE.ToString());
                        break;

                    case ScrollbarEventArgs.EnumTypeButton.Giu:
                        op.Parametro.Add("CodAzioneLock", EnumLock.SUCCESSIVA.ToString());
                        break;

                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);
                if (dt.Rows.Count == 1 && dt.Rows[0]["IDSchedaTrovata"].ToString() != op.TimeStamp.IDEntita)
                {
                    foreach (UltraTreeNode oNode in this.ucEasyTreeView.ActiveNode.Parent.Nodes)
                    {
                        if (oNode.Key.Contains(dt.Rows[0]["IDSchedaTrovata"].ToString()) == true)
                        {
                            oNode.Selected = true;
                            this.ucEasyTreeView.ActiveNode = oNode;
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucScrollBarVInfo_Button", "ucSchede");
            }

        }

        #endregion

        #region DRAG & DROP

        private bool ConsentiDrop(ref Infragistics.Win.UltraWinTree.SelectedNodesCollection refSourceNodes,
                                  ref UltraTreeNode refDestNode,
                                  ref DataRow refdrDestTag,
                                  ref string refsDestTag,
                                  bool mostraMessaggi)
        {
            try
            {
                bool bConsentiDrop = false;

                if (this.IsDisposed == false)
                {


                    bConsentiDrop = true;

                    if (bConsentiDrop && refDestNode == null) bConsentiDrop = false;

                    if (bConsentiDrop && refDestNode.Key == refSourceNodes[0].Key) bConsentiDrop = false;

                    if (bConsentiDrop)
                    {
                        if (refDestNode.Tag.GetType() == typeof(DataRow))
                        {
                            refdrDestTag = (DataRow)refDestNode.Tag;
                        }
                        else if (refDestNode.Tag.GetType() == typeof(string))
                        {
                            refsDestTag = refDestNode.Tag.ToString();
                        }

                        if (bConsentiDrop && refdrDestTag == null && refsDestTag.Trim() == "") bConsentiDrop = false;

                        if (bConsentiDrop && refsDestTag == CoreStatics.TV_AGENDE) bConsentiDrop = false;

                        if (bConsentiDrop && refsDestTag == CoreStatics.TV_CARTELLA) bConsentiDrop = false;

                        if (bConsentiDrop && refdrDestTag != null && (EnumEntita)Enum.Parse(typeof(EnumEntita), refdrDestTag["CodEntita"].ToString()) == EnumEntita.APP) bConsentiDrop = false;

                        if (bConsentiDrop)
                        {
                            bConsentiDrop = false;

                            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("IDSchedaOrigine", ((DataRow)refSourceNodes[0].Tag)["IDScheda"].ToString());
                            if (refdrDestTag != null)
                            {
                                op.Parametro.Add("CodEntitaDestinazione", refdrDestTag["CodEntita"].ToString());
                                op.Parametro.Add("IDEntitaDestinazione", refdrDestTag["IDEntita"].ToString());

                                if (!refdrDestTag.IsNull("IDScheda"))
                                    op.Parametro.Add("IDSchedaDestinazione", refdrDestTag["IDScheda"].ToString());

                            }
                            else if (refsDestTag == CoreStatics.TV_ROOT)
                            {
                                op.Parametro.Add("CodEntitaDestinazione", EnumEntita.PAZ.ToString());
                                op.Parametro.Add("IDEntitaDestinazione", CoreStatics.CoreApplication.Paziente.ID);
                            }


                            string scodUA = "";

                            if (CoreStatics.CoreApplication.Trasferimento != null)
                                scodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                            else
                            {
                                if (CoreStatics.CoreApplication.Paziente != null && CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale != null && CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale.Trim() != "")
                                    scodUA = CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale;
                            }
                            op.Parametro.Add("CodUA", scodUA);

                            SqlParameterExt[] spcoll = new SqlParameterExt[1];
                            string xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            DataTable dt = null;
                            DataSet ds = Database.GetDatasetStoredProc("MSP_ControlloIncollaScheda", spcoll);
                            if (ds != null && ds.Tables.Count > 0) dt = ds.Tables[ds.Tables.Count - 1];
                            if (dt != null && dt.Rows.Count == 1)
                            {
                                if (dt.Rows[0].IsNull("Esito") || !(bool)dt.Rows[0]["Esito"])
                                {
                                    bConsentiDrop = false;
                                    if (mostraMessaggi && !dt.Rows[0].IsNull("Messaggio") && dt.Rows[0]["Messaggio"].ToString() != "")
                                    {
                                        easyStatics.EasyMessageBox(dt.Rows[0]["Messaggio"].ToString(), "Sposta Scheda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    }
                                }
                                else
                                    bConsentiDrop = true;
                            }
                        }
                    }

                }
                return bConsentiDrop;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ucEasyTreeView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (this.IsDisposed == false)
                {

                    if (e.Data.GetDataPresent(typeof(Infragistics.Win.UltraWinTree.SelectedNodesCollection)))
                    {
                        Infragistics.Win.UltraWinTree.SelectedNodesCollection oSourceNodes = (Infragistics.Win.UltraWinTree.SelectedNodesCollection)e.Data.GetData(typeof(Infragistics.Win.UltraWinTree.SelectedNodesCollection));
                        Point PointInTree = this.ucEasyTreeView.PointToClient(new Point(e.X, e.Y));
                        UltraTreeNode oDestNode = this.ucEasyTreeView.GetNodeFromPoint(PointInTree);

                        DataRow drDestTag = null;
                        string sDestTag = null;

                        bool bConsentiSpostamento = ConsentiDrop(ref oSourceNodes, ref oDestNode, ref drDestTag, ref sDestTag, true);

                        if (bConsentiSpostamento)
                        {

                            string sMsg = "";
                            sMsg += @"Confermi lo spostamento della Scheda """ + oSourceNodes[0].Text + @"""";
                            sMsg += @" in  """ + oDestNode.Text + @"""?";
                            if (easyStatics.EasyMessageBox(sMsg, "Sposta Scheda", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {

                                CoreStatics.SetNavigazione(false);
                                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                                bool bSchedaSpostata = false;
                                string sIDSchedaOrigine = ((DataRow)oSourceNodes[0].Tag)["IDScheda"].ToString();

                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                op.Parametro.Add("IDSchedaOrigine", sIDSchedaOrigine);
                                if (drDestTag != null)
                                {
                                    op.Parametro.Add("CodEntitaDestinazione", drDestTag["CodEntita"].ToString());
                                    op.Parametro.Add("IDEntitaDestinazione", drDestTag["IDEntita"].ToString());

                                    if (!drDestTag.IsNull("IDScheda"))
                                        op.Parametro.Add("IDSchedaDestinazione", drDestTag["IDScheda"].ToString());

                                }
                                else if (sDestTag == CoreStatics.TV_ROOT)
                                {
                                    op.Parametro.Add("CodEntitaDestinazione", EnumEntita.PAZ.ToString());
                                    op.Parametro.Add("IDEntitaDestinazione", CoreStatics.CoreApplication.Paziente.ID);
                                }

                                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                                string xmlParam = XmlProcs.XmlSerializeToString(op);
                                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                DataTable dt = null;
                                DataSet ds = Database.GetDatasetStoredProc("MSP_SpostaScheda", spcoll);
                                if (ds != null && ds.Tables.Count > 0) dt = ds.Tables[ds.Tables.Count - 1];
                                if (dt != null && dt.Rows.Count == 1)
                                {
                                    if (dt.Rows[0].IsNull("Esito") || !(bool)dt.Rows[0]["Esito"])
                                    {
                                        if (!dt.Rows[0].IsNull("Messaggio") && dt.Rows[0]["Messaggio"].ToString() != "")
                                        {
                                            easyStatics.EasyMessageBox(dt.Rows[0]["Messaggio"].ToString(), "Sposta Scheda", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        }
                                    }
                                    else
                                        bSchedaSpostata = true;
                                }

                                if (bSchedaSpostata)
                                {
                                    Aggiorna();
                                    this.RicercaNodo(null, sIDSchedaOrigine);
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"ucEasyTreeView_DragDrop", "ucSchede");
            }
            finally
            {

                CoreStatics.SetNavigazione(true);
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }
        }

        private void ucEasyTreeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (this.IsDisposed == false)
                {
                    if (e.Data.GetDataPresent(typeof(Infragistics.Win.UltraWinTree.SelectedNodesCollection)))
                    {
                        DataRow drDestTag = null;
                        string sDestTag = null;
                        Infragistics.Win.UltraWinTree.SelectedNodesCollection oSourceNodes = (Infragistics.Win.UltraWinTree.SelectedNodesCollection)e.Data.GetData(typeof(Infragistics.Win.UltraWinTree.SelectedNodesCollection));
                        Point PointInTree = this.ucEasyTreeView.PointToClient(new Point(e.X, e.Y));
                        UltraTreeNode oDestNode = this.ucEasyTreeView.GetNodeFromPoint(PointInTree);

                        bool bConsentiDrop = ConsentiDrop(ref oSourceNodes, ref oDestNode, ref drDestTag, ref sDestTag, false);

                        if (bConsentiDrop)
                            e.Effect = DragDropEffects.Move;
                        else
                            e.Effect = DragDropEffects.None;

                    }
                }
            }
            catch
            {
            }
        }

        private void ucEasyTreeView_SelectionDragStart(object sender, EventArgs e)
        {
            try
            {
                if (this.IsDisposed == false)
                {
                    bool bAvviaDrag = false;
                    if (this.ucEasyTreeView.SelectedNodes.Count == 1)
                    {



                        DataRow oDr = null;
                        if (this.ucEasyTreeView.SelectedNodes[0].Tag.GetType() == typeof(DataRow))
                        {
                            oDr = (DataRow)this.ucEasyTreeView.SelectedNodes[0].Tag;
                        }

                        if (oDr != null && oDr["IDScheda"] != DBNull.Value)
                        {

                            if (oDr.IsNull("IDCartellaAmbulatoriale"))
                            {
                                switch ((EnumEntita)Enum.Parse(typeof(EnumEntita), oDr["CodEntita"].ToString()))
                                {
                                    case EnumEntita.APP:
                                        bAvviaDrag = false;
                                        break;
                                    default:
                                        bAvviaDrag = true;
                                        break;
                                }
                            }
                            else
                            {
                                bAvviaDrag = false;
                            }
                        }

                        if (bAvviaDrag)
                        {
                            bAvviaDrag = false;

                            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("IDScheda", ((DataRow)this.ucEasyTreeView.SelectedNodes[0].Tag)["IDScheda"].ToString());

                            string scodUA = "";

                            if (CoreStatics.CoreApplication.Trasferimento != null)
                                scodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;
                            else
                            {
                                if (CoreStatics.CoreApplication.Paziente != null && CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale != null && CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale.Trim() != "")
                                    scodUA = CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale;
                            }
                            op.Parametro.Add("CodUA", scodUA);

                            SqlParameterExt[] spcoll = new SqlParameterExt[1];
                            string xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            DataTable dt = null;
                            DataSet ds = Database.GetDatasetStoredProc("MSP_ControlloTagliaScheda", spcoll);
                            if (ds != null && ds.Tables.Count > 0) dt = ds.Tables[ds.Tables.Count - 1];
                            if (dt != null && dt.Rows.Count == 1)
                            {
                                if (dt.Rows[0].IsNull("Esito") || !(bool)dt.Rows[0]["Esito"])
                                {


                                }
                                else
                                    bAvviaDrag = true;
                            }
                        }

                    }

                    if (bAvviaDrag)
                        this.ucEasyTreeView.DoDragDrop(this.ucEasyTreeView.SelectedNodes, DragDropEffects.Move);
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        #endregion

        private void txtTreeFilter_KeyUp(object sender, KeyEventArgs e)
        {
            filterNodes();

        }

        private void filterNodes()
        {
            try
            {
                string match = this.txtTreeFilter.Text;

                foreach (UltraTreeNode node in this.ucEasyTreeView.Nodes)
                {
                    this.checkMatchingNode(node);
                }

                foreach (UltraTreeNode node in this.ucEasyTreeView.Nodes)
                {
                    this.nodeSetVisibleChildren(node);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucScrollBarVInfo_Button", "ucSchede");
            }
        }

        private void checkMatchingNode(UltraTreeNode parent)
        {
            if (parent == null) return;
            if (parent.Nodes == null) return;

            string match = this.txtTreeFilter.Text.ToUpper();

            foreach (UltraTreeNode node in parent.Nodes)
            {
                if (
                        (node.Tag.ToString() != CoreStatics.TV_CARTELLA) &&
                        (node.Tag.ToString() != CoreStatics.TV_ROOT) &&
                        (node.Text.ToUpper().Contains(match) == false) &&
                        (node.IsActive == false)
                    )
                {
                    node.Visible = false;
                }
                else
                {
                    node.Visible = true;

                    nodeSetVisibleFather(node);
                }

                checkMatchingNode(node);
            }
        }

        private void nodeSetVisibleFather(UltraTreeNode child)
        {
            if (child.Parent != null)
            {
                child.Parent.Visible = true;
                nodeSetVisibleFather(child.Parent);
            }
        }

        private void nodeSetVisibleChildren(UltraTreeNode node)
        {
            bool isFolder = false;
            if (node.Tag is DataRow)
            {
                DataRow row = (DataRow)node.Tag;

                if (row.Table.Columns.Contains("IdScheda"))
                    isFolder = Convert.ToBoolean(String.IsNullOrEmpty(row["IdScheda"].ToString()));
            }

            foreach (UltraTreeNode child in node.Nodes)
            {
                if (
                        (node.Tag.ToString() != CoreStatics.TV_CARTELLA) &&
                        (node.Tag.ToString() != CoreStatics.TV_ROOT) &&
                        (isFolder == false) &&
                        (node.Visible == true)
                    )
                {
                    child.Visible = true;
                }

                nodeSetVisibleChildren(child);
            }
        }

    }
}
