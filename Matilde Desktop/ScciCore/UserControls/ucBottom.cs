using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinToolbars;
using UnicodeSrl.ScciResource;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.ScciCore
{
    public partial class ucBottom : UserControl
    {
        public ucBottom()
        {
            InitializeComponent();
        }

        #region Declare

        public event PulsanteBottomClickHandler PulsanteIndietroClick;
        public event PulsanteBottomClickHandler PulsanteAvantiClick;

        public event ToolClickEventHandler PulsanteAvantiToolClick;

        public event ImmagineBottomClickHandler ImmagineClick;

        private IDictionary<string, object> _MenuPulsanteAvanti = new Dictionary<string, object>();
        private IDictionary<string, object> _MenuPulsanteAvantiCDSS = new Dictionary<string, object>();

        #endregion

        #region Property

        public IDictionary<string, object> MenuPulsanteAvanti
        {
            get
            {
                return _MenuPulsanteAvanti;
            }
            set
            {
                _MenuPulsanteAvanti = value;
            }
        }

        public IDictionary<string, object> MenuPulsanteAvantiCDSS
        {
            get
            {
                return _MenuPulsanteAvantiCDSS;
            }
            set
            {
                _MenuPulsanteAvantiCDSS = value;
            }
        }

        public bool MenuPulsanteAvantiEnabled
        {
            get
            {
                return this.UltraToolbarsManager.Enabled;
            }
            set
            {
                this.UltraToolbarsManager.Enabled = value;
                foreach (ToolBase oTool in this.UltraToolbarsManager.Tools)
                {
                    try
                    {
                        oTool.SharedProps.Enabled = value;
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion

        #region Events override

        public override void Refresh()
        {

            try
            {

                var oSessione = CoreStatics.CoreApplication.Sessione;
                var oPaziente = CoreStatics.CoreApplication.Paziente;
                var oEpisodio = CoreStatics.CoreApplication.Episodio;
                var oNavigazione = CoreStatics.CoreApplication.Navigazione;

                this.ubIndietro.Visible = oNavigazione.Maschere.MascheraSelezionata.Indietro;

                this.pbHome.Visible = (oNavigazione.Maschere.MascheraSelezionata.Home
                                       && CoreStatics.CoreApplication.Sessione.Nosologico == string.Empty
                                       && CoreStatics.CoreApplication.Sessione.CodiceSACApertura == string.Empty
                                       && CoreStatics.CoreApplication.Sessione.ListaAttesa == string.Empty);

                if (oSessione.Utente.Ruoli.RuoloSelezionato != null)
                {
                    if (oSessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Pazienti_Menu))
                    {
                        this.pbElencoPazienti.Visible = (oNavigazione.Maschere.TornaARicercaAbilitato || oNavigazione.Maschere.TornaAPercorsoAmbulatoriale_RicercaPaziente) && oNavigazione.Maschere.TornaACartellaInVisioneAbilitato == false && CoreStatics.CoreApplication.Sessione.Nosologico == string.Empty && CoreStatics.CoreApplication.Sessione.ListaAttesa == string.Empty;
                    }
                    else
                    {
                        this.pbElencoPazienti.Visible = false;
                    }
                    if (oSessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.ChiusuraCartella_Menu))
                    {
                        if (oEpisodio != null)
                        {
                            this.pbCartelleChiuse.Visible = true;
                        }
                        else
                        {
                            this.pbCartelleChiuse.Visible = false;
                        }
                    }
                    else
                    {
                        this.pbCartelleChiuse.Visible = false;
                    }
                }
                else
                {
                    this.pbElencoPazienti.Visible = false;
                    this.pbCartelleChiuse.Visible = false;
                }

                if (oPaziente != null && oPaziente.Attivo == true)
                {
                    this.pbCartellaPaziente.Image = (oPaziente.Sesso.ToUpper() == "M" ? Risorse.GetImageFromResource(Risorse.GC_PAZIENTEMASCHIO_256) : Risorse.GetImageFromResource(Risorse.GC_PAZIENTEFEMMINA_256));
                    this.pbCartellaPaziente.Visible = oNavigazione.Maschere.TornaACartellaAbilitato;
                }
                else
                {
                    this.pbCartellaPaziente.Visible = false;
                }
                this.pbStampe.Visible = oNavigazione.Maschere.MascheraSelezionata.Stampe;

                this.TableLayoutPanelHome.ColumnStyles[1].Width = (this.pbHome.Visible == false ? 0 : 20);
                this.TableLayoutPanelHome.ColumnStyles[2].Width = (this.pbElencoPazienti.Visible == false ? 0 : 20);
                this.TableLayoutPanelHome.ColumnStyles[3].Width = (this.pbCartellaPaziente.Visible == false ? 0 : 20);
                this.TableLayoutPanelHome.ColumnStyles[4].Width = (this.pbStampe.Visible == false ? 0 : 20);
                this.TableLayoutPanelHome.ColumnStyles[5].Width = (this.pbCartelleChiuse.Visible == false ? 0 : 20);

                this.pbHome.Width = Convert.ToInt32(Convert.ToDouble(this.pbHome.Height) * 1.2D);
                this.pbElencoPazienti.Width = Convert.ToInt32(Convert.ToDouble(this.pbElencoPazienti.Height) * 1.2D);
                this.pbCartellaPaziente.Width = Convert.ToInt32(Convert.ToDouble(this.pbCartellaPaziente.Height) * 1.2D);
                this.pbStampe.Width = Convert.ToInt32(Convert.ToDouble(this.pbStampe.Height) * 1.2D);
                this.pbCartelleChiuse.Width = Convert.ToInt32(Convert.ToDouble(this.pbCartelleChiuse.Height) * 1.4D);

                this.SetMenuButtonRight();
                this.ubAvanti.Visible = oNavigazione.Maschere.MascheraSelezionata.Avanti;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            base.Refresh();

        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (this.DesignMode == false)
            {
                this.pbHome.Image = Risorse.GetImageFromResource(Risorse.GC_HOME_256);
                this.pbElencoPazienti.Image = Risorse.GetImageFromResource(Risorse.GC_PAZIENTI_256);
                this.pbStampe.Image = Risorse.GetImageFromResource(Risorse.GC_REPORT_256);
                this.pbCartelleChiuse.Image = Risorse.GetImageFromResource(Risorse.GC_CHIUSURACARTELLA_256);
                CoreStatics.MainWnd.TimerRefreshBottom += new ScciMain.TimerRefreshBottomEvtHandler(UpdateEventProc);
            }
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (this.DesignMode == false)
            {
                this.pbHome.Image = null;
                this.pbHome.ShortcutKey = Keys.None;
                this.pbElencoPazienti.Image = null;
                this.pbStampe.Image = null;
                this.pbCartelleChiuse.Image = null;
                CoreStatics.MainWnd.TimerRefreshBottom -= UpdateEventProc;
            }
            base.OnHandleDestroyed(e);
        }

        #endregion

        #region Method

        private void SetMenuButtonRight()
        {

            try
            {

                this.UltraToolbarsManager.Tools.Clear();
                PopupMenuTool oMr = new PopupMenuTool("MenuRight");
                this.UltraToolbarsManager.Tools.Add(oMr);

                var utbm = (PopupMenuTool)this.UltraToolbarsManager.Tools["MenuRight"];

                utbm.Settings.UseLargeImages = Infragistics.Win.DefaultableBoolean.True;
                utbm.Settings.Appearance.FontData.SizeInPoints = this.ubAvanti.Appearance.FontData.SizeInPoints;
                utbm.Tools.Clear();

                foreach (var pair in _MenuPulsanteAvanti)
                {

                    if (this.UltraToolbarsManager.Tools.Exists(pair.Key) == false)
                    {

                        ButtonTool oBt = new ButtonTool(pair.Key);
                        oBt.SharedProps.Caption = ((MenuToolDefinition)pair.Value).Caption;
                        oBt.SharedProps.AppearancesLarge.Appearance.BackColor = Color.WhiteSmoke;
                        oBt.SharedProps.AppearancesLarge.Appearance.Image = CoreStatics.GetImageFromMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), pair.Key));
                        oBt.SharedProps.Shortcut = ((MenuToolDefinition)pair.Value).Shortcut;
                        this.UltraToolbarsManager.Tools.Add(oBt);
                    }

                    utbm.Tools.AddTool(this.UltraToolbarsManager.Tools[pair.Key].Key);

                }

                if (_MenuPulsanteAvantiCDSS.Count > 0)
                {

                    PopupMenuTool oMCdss = new PopupMenuTool(EnumModules.AltreFunzioni_Menu.ToString());
                    oMCdss.SharedProps.Caption = CoreStatics.GetEnumDescription(EnumModules.AltreFunzioni_Menu);
                    oMCdss.SharedProps.AppearancesLarge.Appearance.BackColor = Color.WhiteSmoke;
                    oMCdss.SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CDSSPLUGIN_32);
                    this.UltraToolbarsManager.Tools.Add(oMCdss);

                    var utbmcdss = (PopupMenuTool)this.UltraToolbarsManager.Tools[EnumModules.AltreFunzioni_Menu.ToString()];

                    utbmcdss.Settings.UseLargeImages = Infragistics.Win.DefaultableBoolean.True;
                    utbmcdss.Settings.Appearance.FontData.SizeInPoints = this.ubAvanti.Appearance.FontData.SizeInPoints;
                    utbmcdss.Tools.Clear();

                    foreach (var pair in _MenuPulsanteAvantiCDSS)
                    {

                        if (this.UltraToolbarsManager.Tools.Exists(pair.Key) == false)
                        {

                            ButtonTool oBt = new ButtonTool(pair.Key);
                            oBt.SharedProps.Caption = ((Plugin)pair.Value).Descrizione;
                            oBt.SharedProps.AppearancesLarge.Appearance.BackColor = Color.WhiteSmoke;

                            byte[] imgIcona = ((Plugin)pair.Value).Icona;

                            if (imgIcona != null)
                                oBt.SharedProps.AppearancesLarge.Appearance.Image = CoreStatics.ByteToImage(imgIcona);

                            this.UltraToolbarsManager.Tools.Add(oBt);
                        }

                        utbmcdss.Tools.AddTool(this.UltraToolbarsManager.Tools[pair.Key].Key);
                        utbmcdss.Tools[this.UltraToolbarsManager.Tools[pair.Key].Key].InstanceProps.Tag = pair.Value;

                    }

                    utbm.Tools.AddTool(this.UltraToolbarsManager.Tools[EnumModules.AltreFunzioni_Menu.ToString()].Key);

                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events

        private void ubIndietro_Click(object sender, EventArgs e)
        {
            if (PulsanteIndietroClick != null) { PulsanteIndietroClick(sender, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro)); }
        }

        private void ubAvanti_Click(object sender, EventArgs e)
        {
            if (((PopupMenuTool)this.UltraToolbarsManager.Tools["MenuRight"]).Tools.Count != 0)
            {
                Point p = this.ubAvanti.PointToScreen(this.ubAvanti.Parent.Location);
                p = new Point(p.X + this.ubAvanti.Width / 5 * 4, p.Y);
                ((PopupMenuTool)this.UltraToolbarsManager.Tools["MenuRight"]).ShowPopup(p);
            }
            else
            {
                if (PulsanteAvantiClick != null) { PulsanteAvantiClick(sender, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti)); }
            }
        }

        private void UltraToolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            if (e.Tool.EnabledResolved && PulsanteAvantiToolClick != null) { PulsanteAvantiToolClick(sender, e); }
        }

        private void pbHome_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineBottomClickEventArgs(EnumImmagineBottom.Home)); }
        }

        private void pbElencoPazienti_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineBottomClickEventArgs(EnumImmagineBottom.ElencoPazienti)); }
        }

        private void pbCartellaPaziente_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineBottomClickEventArgs(EnumImmagineBottom.CartellaPaziente)); }
        }

        private void pbStampe_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineBottomClickEventArgs(EnumImmagineBottom.Stampe)); }
        }

        private void pbCartelleChiuse_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineBottomClickEventArgs(EnumImmagineBottom.CartelleChiuse)); }
        }

        #endregion

        #region Events Update

        private void UpdateEventProc()
        {

            try
            {

                if (this.Disposing || this.IsDisposed) return;

                if (this.InvokeRequired)
                {
                    ScciMain.TimerRefreshBottomEvtHandler pFunc = new ScciMain.TimerRefreshBottomEvtHandler(_update);
                    this.Invoke(pFunc, new object[] { });
                }
                else
                {
                    this._update();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void _update()
        {
            this.Refresh();
        }

        #endregion

    }
}
