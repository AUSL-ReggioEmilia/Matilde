using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;
using System.Windows.Forms;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.ScciCore.ViewController;
using System.Threading;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore
{
    [Serializable()]
    public class Maschere
    {

        #region Declare

        private Maschera _mascheraselezionata = null;

        private List<Maschera> _elementi = new List<Maschera>();

        public static int _navigare = 0;
        #endregion

        #region Costructor

        public Maschere()
        {

            Interlocked.Exchange(ref _navigare, 1);

            this.TornaARicercaAbilitato = false;
            this.TornaACartellaAbilitato = false;
            this.TornaACartellaInVisioneAbilitato = false;
            this.TornaAPercorsoAmbulatoriale_RicercaPaziente = false;
        }

        #endregion

        #region Property

        public List<Maschera> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }


        public Maschera MascheraSelezionata
        {
            get { return _mascheraselezionata; }
            set { _mascheraselezionata = value; }
        }

        public bool TornaARicercaAbilitato { get; set; }
        public bool TornaACartellaAbilitato { get; set; }
        public bool TornaACartellaInVisioneAbilitato { get; set; }
        public bool TornaAPercorsoAmbulatoriale_RicercaPaziente { get; set; }

        #endregion

        #region Method

        internal void TracciaNavigazione(string sInfo)
        {
            TracciaNavigazione(sInfo, false);
        }
        internal void TracciaNavigazione(string sInfo, bool addNewLine)
        {

            try
            {
                if (CoreStatics.CoreApplication.Sessione.Computer.EnableTrace)
                {
                    string sTraceInfo = "";
                    sTraceInfo = sInfo;
                    try
                    {
                        long memK = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
                        long memM = Convert.ToInt64(memK / 1024);
                        sTraceInfo += @" - MEMORY=" + memM.ToString(@"#,##0") + @" MB";
                    }
                    catch
                    {
                    }
                    try
                    {
                        sTraceInfo += @" - HANDLE APERTI=" + System.Diagnostics.Process.GetCurrentProcess().HandleCount.ToString(@"#,##0");
                    }
                    catch
                    {
                    }
                    sTraceInfo += @" - " + CoreStatics.CoreApplication.Sessione.Utente.Codice;

                    if (CoreStatics.CoreApplication.Paziente != null || CoreStatics.CoreApplication.Episodio != null || CoreStatics.CoreApplication.Trasferimento != null)
                        sTraceInfo += @" [";

                    if (CoreStatics.CoreApplication.Paziente != null)
                    {
                        sTraceInfo += @"ID Paziente = " + CoreStatics.CoreApplication.Paziente.ID;
                    }
                    if (CoreStatics.CoreApplication.Episodio != null)
                    {
                        sTraceInfo += @"; ID Episodio = " + CoreStatics.CoreApplication.Episodio.ID;
                    }
                    if (CoreStatics.CoreApplication.Trasferimento != null)
                    {
                        sTraceInfo += @"; ID Trasferimento = " + CoreStatics.CoreApplication.Trasferimento.ID;
                        sTraceInfo += @"; ID Cartella = " + CoreStatics.CoreApplication.Trasferimento.IDCartella;
                    }

                    if (CoreStatics.CoreApplication.Paziente != null || CoreStatics.CoreApplication.Episodio != null || CoreStatics.CoreApplication.Trasferimento != null)
                        sTraceInfo += @"]";


                    if (addNewLine) sTraceInfo += Environment.NewLine;

                    Debugger.Trace.AddMessage(sTraceInfo);
                }
            }
            catch
            {
            }
        }

        public DialogResult ControllaWarningsPreAperturaMaschera(EnumMaschere maschera)
        {
            DialogResult drReturn = DialogResult.Yes;

            switch (maschera)
            {
                case EnumMaschere.CartellaPaziente:
                    if ((CoreStatics.CoreApplication.Cartella != null) && (CoreStatics.CoreApplication.Cartella.WarningElaborati == false))
                    {
                        CoreStatics.CoreApplication.Cartella.WarningElaborati = true;
                        drReturn = CoreStatics.CheckWarnings(EnumTipoFiltroSpeciale.EPICAR);
                    }

                    break;

                case EnumMaschere.Ambulatoriale_Cartella:
                    if ((CoreStatics.CoreApplication.Paziente != null) && (CoreStatics.CoreApplication.Paziente.WarningElaborati == false))
                    {
                        CoreStatics.CoreApplication.Paziente.WarningElaborati = true;
                        drReturn = CoreStatics.CheckWarnings(EnumTipoFiltroSpeciale.AMBCAR);
                    }

                    break;

                default:
                    break;
            }

            return drReturn;
        }

        public DialogResult CaricaMaschera(EnumMaschere maschera)
        {
            return CaricaMaschera(maschera, true);
        }
        public DialogResult CaricaMaschera(EnumMaschere maschera, bool attivanavigazione, object customParameters = null)
        {
            string sTrace = "";

            DialogResult drReturn = DialogResult.Cancel;

            DialogResult warningsResult = ControllaWarningsPreAperturaMaschera(maschera);

            switch (warningsResult)
            {
                case DialogResult.Cancel:
                case DialogResult.Abort:
                case DialogResult.No:
                    drReturn = warningsResult;
                    break;

                default:

                    Form frmc = CoreStatics.CoreApplicationContext.MainForm;
                    CoreStatics.impostaCursore(ref frmc, enum_app_cursors.WaitCursor);

                    try
                    {

                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = 0;

                        var utc = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).UltraTabControlMenu;
                        var ucT = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucTop;
                        var ucB = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucBottom;

                        if (this.Elementi.Any(Maschera => Maschera.ID == maschera))
                        {

                            if (Interlocked.Equals(_navigare, 1))
                            {


                                ucT.Enabled = false;
                                ucB.Enabled = false;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                var oMaschera = this.Elementi.Single(Maschera => Maschera.ID == maschera);
                                oMaschera.MascheraPartenza = this.MascheraSelezionata;
                                this.MascheraSelezionata = oMaschera;

                                sTrace = @"Maschera: " + maschera;
                                if (oMaschera != null && oMaschera.ControlMiddle != null) sTrace += @" {" + oMaschera.ControlMiddle.ToString() + "}";

                                this.ResetUltraTabControl();
                                UltraTab custTab = utc.Tabs.Add(oMaschera.ID.ToString());
                                custTab.TabPage.Controls.Add((UserControl)oMaschera.ControlMiddle);

                                this.CaricaContesto(maschera);
                                if (oMaschera.Aggiorna == true) { oMaschera.ControlMiddle.Aggiorna(); }

                                if (CoreStatics.CoreApplicationContext.MainForm is Interfacce.IViewFormMain)
                                {
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato = oMaschera.Massimizzata;
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).CodiceMaschera = oMaschera.CodMaschera;
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = oMaschera.TimerRefresh;
                                }

                                ucT.Enabled = true;
                                ucB.Enabled = true;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                            }

                        }
                        else
                        {

                            Maschera oMaschera = this.CaricaUserControl(maschera);
                            oMaschera.CustomParamaters = customParameters;

                            if (oMaschera.Modale == false)
                            {
                                if (Interlocked.Equals(_navigare, 1))
                                {
                                    ucT.Enabled = false;
                                    ucB.Enabled = false;
                                    ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                    this.Elementi.Add(oMaschera);
                                    oMaschera.MascheraPartenza = this.MascheraSelezionata;
                                    this.MascheraSelezionata = oMaschera;
                                    sTrace = @"Maschera: " + maschera;
                                    if (oMaschera != null && oMaschera.ControlMiddle != null) sTrace += @" {" + oMaschera.ControlMiddle.ToString() + "}";

                                    this.ResetUltraTabControl();
                                    UltraTab custTab = utc.Tabs.Add(oMaschera.ID.ToString());
                                    custTab.TabPage.Controls.Add((UserControl)oMaschera.ControlMiddle);

                                    this.CaricaContesto(maschera);

                                    if (CoreStatics.CoreApplicationContext.MainForm is Interfacce.IViewFormMain)
                                    {
                                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato = oMaschera.Massimizzata;
                                    }

                                    oMaschera.ControlMiddle.Carica();

                                    if (CoreStatics.CoreApplicationContext.MainForm is Interfacce.IViewFormMain)
                                    {
                                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).CodiceMaschera = oMaschera.CodMaschera;
                                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = oMaschera.TimerRefresh;
                                    }


                                    ucT.Enabled = true;
                                    ucB.Enabled = true;
                                    ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;
                                }
                            }
                            else
                            {




                                ucT.Enabled = false;
                                ucB.Enabled = false;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                oMaschera.MascheraPartenza = this.MascheraSelezionata;
                                this.MascheraSelezionata = oMaschera;
                                this.CaricaContesto(maschera);

                                Form frm = (Form)oMaschera.ControlModal;
                                easyStatics.maximizeForm(ref frm);

                                oMaschera.ControlModal.CodiceMaschera = oMaschera.CodMaschera;

                                sTrace = @"Maschera: " + maschera;
                                if (oMaschera != null && oMaschera.ControlModal != null) sTrace += @" {" + oMaschera.ControlModal.ToString() + "}";
                                TracciaNavigazione(sTrace);
                                sTrace = "";

                                oMaschera.ControlModal.CustomParamaters = oMaschera.CustomParamaters;
                                oMaschera.ControlModal.Carica();


                                drReturn = ((Form)oMaschera.ControlModal).DialogResult;

                                this.MascheraSelezionata = this.MascheraSelezionata.MascheraPartenza;
                                if (!this.MascheraSelezionata.Modale)
                                {
                                    this.MascheraSelezionata.MascheraPartenza = null;
                                    this.CaricaContesto(MascheraSelezionata.ID);
                                }

                                if (this.MascheraSelezionata != null && this.MascheraSelezionata.Modale == false)
                                {



                                    if (drReturn == DialogResult.OK && this.MascheraSelezionata.Aggiorna && attivanavigazione)
                                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(this.MascheraSelezionata.ID);

                                }

                                if (CoreStatics.CoreApplicationContext.MainForm is Interfacce.IViewFormMain)
                                {
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = this.MascheraSelezionata.TimerRefresh;
                                }

                                ucT.Enabled = attivanavigazione;
                                ucB.Enabled = attivanavigazione;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                frm.Dispose();
                                frm = null;

                            }
                        }

                        if (sTrace != null && sTrace.Trim() != "") TracciaNavigazione(sTrace);

                    }
                    catch (Exception ex)
                    {
                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                    }
                    CoreStatics.impostaCursore(ref frmc, enum_app_cursors.DefaultCursor);


                    break;
            }


            return drReturn;
        }
        public DialogResult CaricaMaschera(EnumMaschere maschera, bool attivanavigazione, Maschera MascheraExt)
        {

            string sTrace = "";

            DialogResult drReturn = DialogResult.Cancel;

            DialogResult warningsResult = ControllaWarningsPreAperturaMaschera(maschera);
            switch (warningsResult)
            {
                case DialogResult.Cancel:
                case DialogResult.Abort:
                case DialogResult.No:
                    drReturn = warningsResult;
                    break;

                default:
                    Form frmc = CoreStatics.CoreApplicationContext.MainForm;
                    CoreStatics.impostaCursore(ref frmc, enum_app_cursors.WaitCursor);

                    try
                    {

                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = 0;

                        var utc = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).UltraTabControlMenu;
                        var ucT = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucTop;
                        var ucB = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucBottom;

                        if (this.Elementi.Any(Maschera => Maschera.ID == maschera))
                        {

                            if (Interlocked.Equals(_navigare, 1))
                            {


                                ucT.Enabled = false;
                                ucB.Enabled = false;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                var oMaschera = this.Elementi.Single(Maschera => Maschera.ID == maschera);
                                oMaschera.MascheraPartenza = this.MascheraSelezionata;
                                this.MascheraSelezionata = oMaschera;

                                sTrace = @"Maschera: " + maschera;
                                if (oMaschera != null && oMaschera.ControlMiddle != null) sTrace += @" {" + oMaschera.ControlMiddle.ToString() + "}";

                                this.ResetUltraTabControl();
                                UltraTab custTab = utc.Tabs.Add(oMaschera.ID.ToString());
                                custTab.TabPage.Controls.Add((UserControl)oMaschera.ControlMiddle);

                                this.CaricaContesto(maschera);
                                if (oMaschera.Aggiorna == true) { oMaschera.ControlMiddle.Aggiorna(); }

                                if (CoreStatics.CoreApplicationContext.MainForm is Interfacce.IViewFormMain)
                                {
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato = oMaschera.Massimizzata;
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).CodiceMaschera = oMaschera.CodMaschera;
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = oMaschera.TimerRefresh;
                                }

                                ucT.Enabled = true;
                                ucB.Enabled = true;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                            }

                        }
                        else
                        {

                            Maschera oMaschera = MascheraExt;
                            if (oMaschera.Modale == false)
                            {
                                if (Interlocked.Equals(_navigare, 1))
                                {
                                    ucT.Enabled = false;
                                    ucB.Enabled = false;
                                    ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                    this.Elementi.Add(oMaschera);
                                    oMaschera.MascheraPartenza = this.MascheraSelezionata;
                                    this.MascheraSelezionata = oMaschera;
                                    sTrace = @"Maschera: " + maschera;
                                    if (oMaschera != null && oMaschera.ControlMiddle != null) sTrace += @" {" + oMaschera.ControlMiddle.ToString() + "}";

                                    this.ResetUltraTabControl();
                                    UltraTab custTab = utc.Tabs.Add(oMaschera.ID.ToString());
                                    custTab.TabPage.Controls.Add((UserControl)oMaschera.ControlMiddle);

                                    this.CaricaContesto(maschera);
                                    oMaschera.ControlMiddle.Carica();

                                    if (CoreStatics.CoreApplicationContext.MainForm is Interfacce.IViewFormMain)
                                    {
                                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato = oMaschera.Massimizzata;
                                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).CodiceMaschera = oMaschera.CodMaschera;
                                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = oMaschera.TimerRefresh;
                                    }


                                    ucT.Enabled = true;
                                    ucB.Enabled = true;
                                    ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;
                                }
                            }
                            else
                            {




                                ucT.Enabled = false;
                                ucB.Enabled = false;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                oMaschera.MascheraPartenza = this.MascheraSelezionata;
                                this.MascheraSelezionata = oMaschera;
                                this.CaricaContesto(maschera);

                                Form frm = (Form)oMaschera.ControlModal;
                                easyStatics.maximizeForm(ref frm);

                                oMaschera.ControlModal.CodiceMaschera = oMaschera.CodMaschera;

                                sTrace = @"Maschera: " + maschera;
                                if (oMaschera != null && oMaschera.ControlModal != null) sTrace += @" {" + oMaschera.ControlModal.ToString() + "}";
                                TracciaNavigazione(sTrace);
                                sTrace = "";

                                oMaschera.ControlModal.Carica();
                                drReturn = ((Form)oMaschera.ControlModal).DialogResult;

                                this.MascheraSelezionata = this.MascheraSelezionata.MascheraPartenza;
                                if (!this.MascheraSelezionata.Modale)
                                {
                                    this.MascheraSelezionata.MascheraPartenza = null;
                                }

                                if (this.MascheraSelezionata != null && this.MascheraSelezionata.Modale == false)
                                {



                                    if (drReturn == DialogResult.OK && this.MascheraSelezionata.Aggiorna && attivanavigazione)
                                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(this.MascheraSelezionata.ID);

                                }

                                if (CoreStatics.CoreApplicationContext.MainForm is Interfacce.IViewFormMain)
                                {
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = this.MascheraSelezionata.TimerRefresh;
                                }

                                ucT.Enabled = attivanavigazione;
                                ucB.Enabled = attivanavigazione;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                frm.Dispose();
                                frm = null;

                            }
                        }

                        if (sTrace != null && sTrace.Trim() != "") TracciaNavigazione(sTrace);

                    }
                    catch (Exception ex)
                    {
                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                    }
                    CoreStatics.impostaCursore(ref frmc, enum_app_cursors.DefaultCursor);

                    break;
            }

            return drReturn;

        }
        public IViewController CaricaMaschera(EnumMaschere maschera, bool attivanavigazione, IViewController viewcontroller)
        {
            string sTrace = "";

            DialogResult drReturn = DialogResult.Cancel;
            IViewController viewcontrollerreturn = viewcontroller;

            DialogResult warningsResult = ControllaWarningsPreAperturaMaschera(maschera);
            switch (warningsResult)
            {
                case DialogResult.Cancel:
                case DialogResult.Abort:
                case DialogResult.No:
                    drReturn = warningsResult;
                    break;

                default:

                    Form frmc = CoreStatics.CoreApplicationContext.MainForm;
                    CoreStatics.impostaCursore(ref frmc, enum_app_cursors.WaitCursor);

                    try
                    {

                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = 0;

                        var utc = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).UltraTabControlMenu;
                        var ucT = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucTop;
                        var ucB = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucBottom;

                        if (this.Elementi.Any(Maschera => Maschera.ID == maschera))
                        {

                            if (Interlocked.Equals(_navigare, 1))
                            {


                                ucT.Enabled = false;
                                ucB.Enabled = false;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                var oMaschera = this.Elementi.Single(Maschera => Maschera.ID == maschera);
                                oMaschera.MascheraPartenza = this.MascheraSelezionata;
                                this.MascheraSelezionata = oMaschera;

                                sTrace = @"Maschera: " + maschera;
                                if (oMaschera != null && oMaschera.ControlMiddle != null) sTrace += @" {" + oMaschera.ControlMiddle.ToString() + "}";

                                this.ResetUltraTabControl();
                                UltraTab custTab = utc.Tabs.Add(oMaschera.ID.ToString());
                                custTab.TabPage.Controls.Add((UserControl)oMaschera.ControlMiddle);

                                this.CaricaContesto(maschera);
                                if (oMaschera.Aggiorna == true) { oMaschera.ControlMiddle.Aggiorna(); }

                                if (CoreStatics.CoreApplicationContext.MainForm is Interfacce.IViewFormMain)
                                {
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato = oMaschera.Massimizzata;
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).CodiceMaschera = oMaschera.CodMaschera;
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = oMaschera.TimerRefresh;
                                }

                                ucT.Enabled = true;
                                ucB.Enabled = true;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                            }

                        }
                        else
                        {

                            Maschera oMaschera = this.CaricaUserControl(maschera);
                            if (oMaschera.Modale == false)
                            {
                                if (Interlocked.Equals(_navigare, 1))
                                {
                                    ucT.Enabled = false;
                                    ucB.Enabled = false;
                                    ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                    this.Elementi.Add(oMaschera);
                                    oMaschera.MascheraPartenza = this.MascheraSelezionata;
                                    this.MascheraSelezionata = oMaschera;
                                    sTrace = @"Maschera: " + maschera;
                                    if (oMaschera != null && oMaschera.ControlMiddle != null) sTrace += @" {" + oMaschera.ControlMiddle.ToString() + "}";

                                    this.ResetUltraTabControl();
                                    UltraTab custTab = utc.Tabs.Add(oMaschera.ID.ToString());
                                    custTab.TabPage.Controls.Add((UserControl)oMaschera.ControlMiddle);

                                    this.CaricaContesto(maschera);

                                    if (CoreStatics.CoreApplicationContext.MainForm is Interfacce.IViewFormMain)
                                    {
                                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato = oMaschera.Massimizzata;
                                    }

                                    oMaschera.ControlMiddle.Carica();

                                    if (CoreStatics.CoreApplicationContext.MainForm is Interfacce.IViewFormMain)
                                    {
                                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).CodiceMaschera = oMaschera.CodMaschera;
                                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = oMaschera.TimerRefresh;
                                    }


                                    ucT.Enabled = true;
                                    ucB.Enabled = true;
                                    ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;
                                }
                            }
                            else
                            {




                                ucT.Enabled = false;
                                ucB.Enabled = false;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                oMaschera.MascheraPartenza = this.MascheraSelezionata;
                                this.MascheraSelezionata = oMaschera;
                                this.CaricaContesto(maschera);

                                Form frm = (Form)oMaschera.ControlModal;
                                easyStatics.maximizeForm(ref frm);

                                oMaschera.ControlModal.CodiceMaschera = oMaschera.CodMaschera;

                                sTrace = @"Maschera: " + maschera;
                                if (oMaschera != null && oMaschera.ControlModal != null) sTrace += @" {" + oMaschera.ControlModal.ToString() + "}";
                                TracciaNavigazione(sTrace);
                                sTrace = "";

                                IViewController myVC = oMaschera.ControlModal as IViewController;
                                if (myVC != null && viewcontroller != null)
                                {
                                    myVC.Maschera = oMaschera;
                                    myVC.InitViewController(viewcontroller);
                                }

                                oMaschera.ControlModal.Carica();
                                drReturn = ((Form)oMaschera.ControlModal).DialogResult;
                                if (myVC != null)
                                {
                                    viewcontrollerreturn = myVC.SaveViewController();
                                }

                                this.MascheraSelezionata = this.MascheraSelezionata.MascheraPartenza;
                                if (!this.MascheraSelezionata.Modale)
                                {
                                    this.MascheraSelezionata.MascheraPartenza = null;
                                }

                                if (this.MascheraSelezionata != null && this.MascheraSelezionata.Modale == false)
                                {



                                    if (drReturn == DialogResult.OK && this.MascheraSelezionata.Aggiorna && attivanavigazione)
                                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(this.MascheraSelezionata.ID);

                                }

                                if (CoreStatics.CoreApplicationContext.MainForm is Interfacce.IViewFormMain)
                                {
                                    (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = this.MascheraSelezionata.TimerRefresh;
                                }

                                ucT.Enabled = attivanavigazione;
                                ucB.Enabled = attivanavigazione;
                                ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                                frm.Dispose();
                                frm = null;

                            }
                        }

                        if (sTrace != null && sTrace.Trim() != "") TracciaNavigazione(sTrace);

                    }
                    catch (Exception ex)
                    {
                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                    }
                    CoreStatics.impostaCursore(ref frmc, enum_app_cursors.DefaultCursor);


                    break;
            }


            return viewcontrollerreturn;
        }

        public void CaricaMascheraNM(EnumMaschere maschera, IViewController viewcontroller)
        {
            DialogResult warningsResult = ControllaWarningsPreAperturaMaschera(maschera);
            switch (warningsResult)
            {
                case DialogResult.Cancel:
                case DialogResult.Abort:
                case DialogResult.No:
                    break;

                default:

                    Form frmc = CoreStatics.CoreApplicationContext.MainForm;
                    CoreStatics.impostaCursore(ref frmc, enum_app_cursors.WaitCursor);

                    try
                    {

                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = 0;

                        var utc = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).UltraTabControlMenu;
                        var ucT = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucTop;
                        var ucB = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucBottom;

                        ucT.Enabled = false;
                        ucB.Enabled = false;
                        ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;


                        Maschera oMaschera = this.CaricaUserControl(maschera);
                        oMaschera.MascheraPartenza = this.MascheraSelezionata;
                        oMaschera.ControlModal.CodiceMaschera = oMaschera.CodMaschera;
                        IViewController myVC = oMaschera.ControlModal as IViewController;
                        if (myVC != null && viewcontroller != null)
                        {
                            viewcontroller.Maschera = oMaschera;
                            myVC.InitViewController(viewcontroller);
                        }

                        this.CaricaContesto(maschera, oMaschera);

                        Form frm = (Form)oMaschera.ControlModal;
                        easyStatics.maximizeForm(ref frm, FormBorderStyle.Sizable);

                        oMaschera.ControlModal.Carica();

                        (CoreStatics.CoreApplicationContext.MainForm as Interfacce.IViewFormMain).ControlloCentraleTimerRefresh = this.MascheraSelezionata.TimerRefresh;

                        ucT.Enabled = true;
                        ucB.Enabled = true;
                        ucB.MenuPulsanteAvantiEnabled = ucB.Enabled;

                    }
                    catch (Exception ex)
                    {
                        UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                    }
                    CoreStatics.impostaCursore(ref frmc, enum_app_cursors.DefaultCursor);

                    break;
            }

        }
        public void CaricaMascheraNMListener(EnumMaschere maschera, IViewController viewcontroller)
        {

            try
            {


                Maschera oMaschera = this.CaricaUserControl(maschera);
                oMaschera.ControlModal.CodiceMaschera = oMaschera.CodMaschera;
                IViewController myVC = oMaschera.ControlModal as IViewController;
                if (myVC != null && viewcontroller != null)
                {
                    viewcontroller.Maschera = oMaschera;
                    myVC.InitViewController(viewcontroller);
                }

                this.CaricaContesto(maschera, oMaschera);

                Form frm = (Form)oMaschera.ControlModal;
                easyStatics.maximizeForm(ref frm, FormBorderStyle.Sizable);
                frm.Focus();

                oMaschera.ControlModal.Carica();

                easyStatics.SetForegroundWindow(frm.Handle);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public void CaricaMascheraNMListener_1(ref Form f, EnumMaschere maschera, IViewController viewcontroller)
        {

            try
            {


                Maschera oMaschera = this.CaricaUserControl(maschera);
                oMaschera.ControlModal.CodiceMaschera = oMaschera.CodMaschera;
                IViewController myVC = oMaschera.ControlModal as IViewController;
                if (myVC != null && viewcontroller != null)
                {
                    viewcontroller.Maschera = oMaschera;
                    myVC.InitViewController(viewcontroller);
                }

                this.CaricaContesto(maschera, oMaschera);

                Form frm = (Form)oMaschera.ControlModal;
                easyStatics.maximizeForm(ref frm, FormBorderStyle.Sizable);
                frm.Focus();
                frm.MdiParent = f;

                oMaschera.ControlModal.Carica();

                easyStatics.SetForegroundWindow(frm.Handle);


            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public void CloseAllFormNM()
        {

            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {

                IViewController myVC = Application.OpenForms[i] as IViewController;
                if (myVC != null)
                {
                    Application.OpenForms[i].Close();
                }

            }

            if (CoreStatics.CoreApplication.Listener != null)
                CoreStatics.CoreApplication.Listener.ChiudiMaschere();

        }

        public void CloseAllForm()
        {

            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {

                if (Application.OpenForms[i].Name != "frmMain")
                {
                    Application.OpenForms[i].Close();
                }

            }

            if (CoreStatics.CoreApplication.Listener != null)
                CoreStatics.CoreApplication.Listener.ChiudiMaschere();

        }
        public void SetNavigare(bool enable)
        {

            if (enable == false)
            {
                Interlocked.Exchange(ref _navigare, 0);
            }
            else
            {
                Interlocked.Exchange(ref _navigare, 1);
            }

        }

        private void CaricaContesto(EnumMaschere maschera)
        {
            try
            {



                Ruoli r = CoreStatics.CoreApplication.Sessione.Utente.Ruoli;
                var ucT = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucTop;
                var ucB = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucBottom;
                ucB.ubIndietro.ShortcutKey = Keys.F1;

                switch (maschera)
                {

                    case EnumMaschere.MenuPrincipale:
                        /*
                         * Scaricamento oggetti prima del refresh
                        */

                        CoreStatics.CoreApplication.Paziente = null;
                        CoreStatics.CoreApplication.Episodio = null;
                        CoreStatics.CoreApplication.Trasferimento = null;
                        CoreStatics.CoreApplication.Cartella = null;

                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        this.TornaARicercaAbilitato = false;
                        this.TornaACartellaAbilitato = false;
                        this.TornaACartellaInVisioneAbilitato = false;
                        this.TornaAPercorsoAmbulatoriale_RicercaPaziente = false;
                        ucB.ubIndietro.Text = "ESCI";
                        ucB.ubIndietro.ShortcutKey = Keys.Escape;
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.WorklistInfermieristicaTrasversale:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Visible = true;
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Visible = false;
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.ConsulenzeTrasversali:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Visible = true;
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Visible = false;
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.OrderEntryTrasversale:
                    case EnumMaschere.OrderEntryMonitorTrasversale:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Visible = true;
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Visible = false;
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.EvidenzaClinicaTrasversale:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Visible = true;
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Visible = false;
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.ParametriVitaliTrasversali:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "RILEVA PARAMETRI";
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.IdentificazioneIterataPaziente:
                        var ucdnTModalIIP = ((frmIdentificazioneIterataPaziente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalIIP = ((frmIdentificazioneIterataPaziente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalIIP.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalIIP.ubIndietro.Visible = true;
                        ucdnBModalIIP.ubIndietro.Text = "INDIETRO";
                        ucdnBModalIIP.ubAvanti.Visible = false;
                        ucdnBModalIIP.Refresh();
                        break;

                    case EnumMaschere.Consulenze_RicercaPaziente:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "CONSULENZA";
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.RicercaPazienti:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        this.TornaARicercaAbilitato = false;
                        this.TornaACartellaAbilitato = false;
                        this.TornaACartellaInVisioneAbilitato = false;
                        this.TornaAPercorsoAmbulatoriale_RicercaPaziente = false;
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "CARTELLA PAZIENTE";
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.Ambulatoriale_RicercaPaziente:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        this.TornaARicercaAbilitato = false;
                        this.TornaACartellaAbilitato = false;
                        this.TornaACartellaInVisioneAbilitato = false;
                        this.TornaAPercorsoAmbulatoriale_RicercaPaziente = false;
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "CARTELLA AMBULATORIALE";
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.PercorsoAmbulatoriale_RicercaPaziente:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        this.TornaARicercaAbilitato = false;
                        this.TornaACartellaAbilitato = false;
                        this.TornaACartellaInVisioneAbilitato = false;
                        this.TornaAPercorsoAmbulatoriale_RicercaPaziente = false;
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "PERCORSO AMBULATORIALE";
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.MatHome_RicercaPaziente:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        this.TornaARicercaAbilitato = false;
                        this.TornaACartellaAbilitato = false;
                        this.TornaACartellaInVisioneAbilitato = false;
                        this.TornaAPercorsoAmbulatoriale_RicercaPaziente = false;
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "SELEZIONA";
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.MatHome_GestioneAccount:
                        var ucTopModaleGA = ((frmPUGestioneAccount)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleGA = ((frmPUGestioneAccount)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleGA.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleGA.Refresh();
                        ucBottomModaleGA.ubIndietro.Visible = false;
                        ucBottomModaleGA.ubIndietro.Text = "";
                        ucBottomModaleGA.ubAvanti.Visible = true;
                        ucBottomModaleGA.ubAvanti.Text = "CHIUDI";
                        break;

                    case EnumMaschere.PreTrasferimento_RicercaPaziente:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        this.TornaARicercaAbilitato = false;
                        this.TornaACartellaAbilitato = false;
                        this.TornaACartellaInVisioneAbilitato = false;
                        this.TornaAPercorsoAmbulatoriale_RicercaPaziente = false;
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "CARTELLA PAZIENTE";
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.ChiusuraCartelle:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "";
                        ucB.ubAvanti.Visible = false;
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.FirmaCartelleAperte:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "";
                        ucB.ubAvanti.Visible = false;
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.StampaCartelleChiuse:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "";
                        ucB.ubAvanti.Visible = false;
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.Consegne:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "";
                        ucB.ubAvanti.Visible = false;
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.ConsegnePaziente:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Text = "";
                        ucB.ubAvanti.Visible = false;
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.CartellaPaziente:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        this.TornaACartellaAbilitato = false;
                        if (CoreStatics.CoreApplication.Sessione.Nosologico == string.Empty && CoreStatics.CoreApplication.Sessione.ListaAttesa == string.Empty)
                        {
                            ucB.ubIndietro.Text = "RICERCA PAZIENTI";
                        }
                        else
                        {
                            ucB.ubIndietro.Text = "ESCI";
                        }
                        CaricaMenuAvanti(ref ucB, ref r, maschera);
                        ucB.Refresh();
                        break;

                    case EnumMaschere.CartelleInVisione:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Visible = true;
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.ubAvanti.Visible = false;
                        ucB.ubAvanti.Text = "CARTELLA";
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.CartellaInVisione:
                        var ucTopModaleCIV = ((frmPUCartellaInVisione)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleCIV = ((frmPUCartellaInVisione)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleCIV.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleCIV.Refresh();
                        ucBottomModaleCIV.ubIndietro.Visible = true;
                        ucBottomModaleCIV.ubIndietro.Text = "ANNULLA";
                        ucBottomModaleCIV.ubAvanti.Visible = true;
                        ucBottomModaleCIV.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.CartellaPazienteChiusa:
                        var ucTopModaleCPC = ((frmCartellaChiusa)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleCPC = ((frmCartellaChiusa)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleCPC.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleCPC.Refresh();
                        ucBottomModaleCPC.ubIndietro.Visible = false;
                        ucBottomModaleCPC.ubIndietro.Text = "";
                        ucBottomModaleCPC.ubAvanti.Visible = true;
                        ucBottomModaleCPC.ubAvanti.Text = "CHIUDI";
                        break;

                    case EnumMaschere.Ambulatoriale_Cartella:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        this.TornaACartellaAbilitato = false;
                        if (CoreStatics.CoreApplication.Sessione.CodiceSACApertura == string.Empty)
                        {
                            ucB.ubIndietro.Text = "RICERCA PAZIENTI";
                        }
                        else
                        {
                            ucB.ubIndietro.Text = "ESCI";
                        }
                        CaricaMenuAvanti(ref ucB, ref r, maschera);

                        ucB.Refresh();
                        break;

                    case EnumMaschere.AgendeTrasversali:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Text = "INDIETRO";
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.TrackerPaziente:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Text = "AGENDE";
                        ucB.MenuPulsanteAvanti.Clear();
                        ucB.MenuPulsanteAvantiCDSS.Clear();
                        ucB.Refresh();
                        break;

                    case EnumMaschere.SelezioneNota:
                        var ucTopModaleN = ((frmPUNota)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleN = ((frmPUNota)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleN.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleN.Refresh();
                        ucBottomModaleN.ubIndietro.Visible = true;
                        ucBottomModaleN.ubIndietro.Text = "AGENDE";
                        ucBottomModaleN.ubAvanti.Visible = true;
                        ucBottomModaleN.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.Schede:
                    case EnumMaschere.AgendePaziente:
                    case EnumMaschere.FoglioUnico:
                    case EnumMaschere.WorklistInfermieristica:
                    case EnumMaschere.TerapieFarmacologiche:
                    case EnumMaschere.ParametriVitali:
                    case EnumMaschere.DiarioClinico:
                    case EnumMaschere.GestioneOrdini:
                    case EnumMaschere.EvidenzaClinica:
                    case EnumMaschere.Allegati:
                    case EnumMaschere.EBM:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucB.ubIndietro.Text = "CARTELLA PAZIENTE";
                        CaricaMenuAvanti(ref ucB, ref r, maschera);
                        ucB.Refresh();
                        break;

                    case EnumMaschere.Ambulatoriale_AgendePaziente:
                    case EnumMaschere.Ambulatoriale_Schede:
                    case EnumMaschere.Ambulatoriale_GestioneOrdini:
                    case EnumMaschere.Ambulatoriale_EvidenzaClinica:
                    case EnumMaschere.Ambulatoriale_Allegati:
                    case EnumMaschere.Ambulatoriale_EBM:
                        /* 
                         * Contesto Top 
                         */
                        ucT.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        if (!CoreStatics.CoreApplication.Sessione.UscitaDitetta)
                        {
                            ucB.ubIndietro.Text = "CARTELLA AMBULATORIALE";
                        }
                        else
                        {
                            ucB.ubIndietro.Text = "ESCI";
                        }
                        CaricaMenuAvanti(ref ucB, ref r, maschera);
                        ucB.Refresh();
                        break;

                    case EnumMaschere.Ambulatoriale_SelezioneTipoAppuntamento:
                    case EnumMaschere.SelezioneTipoAppuntamento:
                        var ucTopModaleTA = ((frmSelezionaTipoAppuntamento)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleTA = ((frmSelezionaTipoAppuntamento)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleTA.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleTA.Refresh();
                        ucBottomModaleTA.ubIndietro.Visible = true;
                        ucBottomModaleTA.ubIndietro.Text = "ANNULLA";
                        ucBottomModaleTA.ubAvanti.Visible = true;
                        ucBottomModaleTA.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.Ambulatoriale_SelezioneAgendeAppuntamento:
                    case EnumMaschere.SelezioneAgendeAppuntamento:
                        var ucTopModaleAA = ((frmSelezionaAgendeAppuntamento)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleAA = ((frmSelezionaAgendeAppuntamento)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleAA.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleAA.Refresh();
                        ucBottomModaleAA.ubIndietro.Visible = true;
                        ucBottomModaleAA.ubIndietro.Text = "ANNULLA";
                        ucBottomModaleAA.ubAvanti.Visible = true;
                        ucBottomModaleAA.ubAvanti.Text = "DATA ORA";
                        break;

                    case EnumMaschere.Ambulatoriale_SelezioneStatoAppuntamento:
                    case EnumMaschere.SelezioneStatoAppuntamento:
                        var ucTopModaleSA = ((frmSelezionaStatoAppuntamento)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleSA = ((frmSelezionaStatoAppuntamento)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleSA.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleSA.Refresh();
                        ucBottomModaleSA.ubIndietro.Visible = true;
                        ucBottomModaleSA.ubIndietro.Text = "ANNULLA";
                        ucBottomModaleSA.ubAvanti.Visible = true;
                        ucBottomModaleSA.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.Ambulatoriale_SelezioneAppuntamento:
                    case EnumMaschere.SelezioneAppuntamento:
                    case EnumMaschere.AnnullaAppuntamento:
                        var ucTopModaleA = ((frmSelezionaAppuntamento)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleA = ((frmSelezionaAppuntamento)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleA.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleA.Refresh();
                        ucBottomModaleA.ubIndietro.Visible = true;
                        ucBottomModaleA.ubIndietro.Text = "AGENDE PAZIENTE";
                        ucBottomModaleA.ubAvanti.Visible = true;
                        ucBottomModaleA.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.RicorrenzaNota:
                        var ucTopModaleRN = ((frmPURicorrenza)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleRN = ((frmPURicorrenza)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleRN.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleRN.Refresh();
                        ucBottomModaleRN.ubIndietro.Visible = true;
                        ucBottomModaleRN.ubIndietro.Text = "ANNULLA";
                        ucBottomModaleRN.ubAvanti.Visible = true;
                        ucBottomModaleRN.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.RicercaSAC:
                        var ucTopModaleSPS = ((frmRicercaSAC)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleSPS = ((frmRicercaSAC)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleSPS.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleSPS.Refresh();
                        ucBottomModaleSPS.ubIndietro.Visible = true;
                        ucBottomModaleSPS.ubIndietro.Text = "ANNULLA";
                        ucBottomModaleSPS.ubAvanti.Visible = true;
                        ucBottomModaleSPS.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.RichiestaConsenso:
                        var ucTopModaleCon = ((frmRichiestaConsenso)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleCon = ((frmRichiestaConsenso)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleCon.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleCon.Refresh();
                        ucBottomModaleCon.ubIndietro.Visible = true;
                        ucBottomModaleCon.ubIndietro.Text = "ANNULLA";
                        ucBottomModaleCon.ubAvanti.Visible = true;
                        ucBottomModaleCon.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.DettaglioNews:
                        var ucdnTModal = ((frmDettaglioNews)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModal = ((frmDettaglioNews)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModal.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModal.ubIndietro.Visible = false;
                        ucdnBModal.ubAvanti.Text = "CHIUDI";
                        ucdnBModal.Refresh();
                        break;

                    case EnumMaschere.ZoomAnteprimaRTFScheda:
                        var uczaTModal = ((frmPUZoomScheda)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var uczaBModal = ((frmPUZoomScheda)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        uczaTModal.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        uczaBModal.ubIndietro.Visible = false;
                        uczaBModal.ubAvanti.Text = "CHIUDI";
                        uczaBModal.Refresh();
                        break;

                    case EnumMaschere.SelezionaRuolo:
                        var ucTModalSelezioneRuolo = ((frmSelezionaRuolo)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalSelezioneRuolo = ((frmSelezionaRuolo)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalSelezioneRuolo.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalSelezioneRuolo.Refresh();
                        break;

                    case EnumMaschere.SelezionaCartella:
                        var ucTModalSelezioneCartella = ((frmSelezionaCartella)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalSelezioneCartella = ((frmSelezionaCartella)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalSelezioneCartella.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalSelezioneCartella.Refresh();
                        break;

                    case EnumMaschere.SelezionaCartellaCollegabile:
                        var ucTModalSelezioneCartellaC = ((frmSelezionaCartellaCollegabile)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalSelezioneCartellaC = ((frmSelezionaCartellaCollegabile)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalSelezioneCartellaC.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalSelezioneCartellaC.Refresh();
                        break;

                    case EnumMaschere.SelezionaTipoVoceDiario:
                        var ucTModalSelezioneTVD = ((frmSelezionaTipoVoceDiario)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalSelezioneTVD = ((frmSelezionaTipoVoceDiario)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalSelezioneTVD.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalSelezioneTVD.Refresh();
                        ucBModalSelezioneTVD.ubIndietro.Visible = true;
                        ucBModalSelezioneTVD.ubIndietro.Text = "ANNULLA";
                        ucBModalSelezioneTVD.ubAvanti.Visible = true;
                        ucBModalSelezioneTVD.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.SelezioneTipoAllegato:
                        var ucTModalSelezioneTAO = ((frmSelezionaTipoAllegato)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalSelezioneTAO = ((frmSelezionaTipoAllegato)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalSelezioneTAO.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalSelezioneTAO.Refresh();
                        ucBModalSelezioneTAO.ubIndietro.Visible = true;
                        ucBModalSelezioneTAO.ubIndietro.Text = "ANNULLA";
                        ucBModalSelezioneTAO.ubAvanti.Visible = true;
                        ucBModalSelezioneTAO.ubAvanti.Text = "CONFERMA";
                        break;


                    case EnumMaschere.CambiaUtente:
                        var ucTModalCambiaUtente = ((frmCambiaUtente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalCambiaUtente = ((frmCambiaUtente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalCambiaUtente.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalCambiaUtente.Refresh();
                        break;

                    case EnumMaschere.SelezionaReport:
                        var ucTModalSelezioneReport = ((frmSelezionaReport)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalSelezioneReport = ((frmSelezionaReport)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalSelezioneReport.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalSelezioneReport.Refresh();
                        break;

                    case EnumMaschere.Report:
                        var ucTModalReport = ((frmReport)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalReport = ((frmReport)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalReport.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalReport.Refresh();
                        break;

                    case EnumMaschere.WebBrowser:
                        var ucTModalWebBrowser = ((frmWebBrowser)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalWebBrowser = ((frmWebBrowser)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalWebBrowser.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalWebBrowser.Refresh();
                        ucBModalWebBrowser.ubIndietro.Visible = false;
                        ucBModalWebBrowser.ubAvanti.Visible = true;
                        ucBModalWebBrowser.ubAvanti.Text = "CHIUDI";
                        break;

                    case EnumMaschere.StoricoReport:
                        var ucTModalStoricoReport = ((frmStoricoReport)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalStoricoReport = ((frmStoricoReport)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalStoricoReport.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalStoricoReport.Refresh();
                        ucBModalStoricoReport.ubIndietro.Visible = false;
                        ucBModalStoricoReport.ubAvanti.Visible = true;
                        ucBModalStoricoReport.ubAvanti.Text = "CHIUDI";
                        break;

                    case EnumMaschere.Consulenze_Refertazione:
                        var ucdnTModalCRF = ((frmPUConsulenza)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalCRF = ((frmPUConsulenza)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalCRF.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalCRF.ubIndietro.Visible = true;
                        ucdnBModalCRF.ubIndietro.Text = "INDIETRO";
                        ucdnBModalCRF.ubAvanti.Visible = true;
                        ucdnBModalCRF.ubAvanti.Text = "FIRMA E CHIUDI";
                        ucdnBModalCRF.Refresh();
                        break;

                    case EnumMaschere.EditingVoceDiDiario:
                        var ucdnTModalEVD = ((frmPUDiarioClinico)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalEVD = ((frmPUDiarioClinico)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalEVD.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalEVD.ubIndietro.Visible = true;
                        ucdnBModalEVD.ubIndietro.Text = "INDIETRO";
                        ucdnBModalEVD.ubAvanti.Visible = true;
                        ucdnBModalEVD.ubAvanti.Text = "CONFERMA";
                        ucdnBModalEVD.Refresh();
                        break;

                    case EnumMaschere.ValidazioneVociDiDiario:
                        var ucdnTModalVal = ((frmValidazioni)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalVal = ((frmValidazioni)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalVal.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalVal.ubIndietro.Visible = true;
                        ucdnBModalVal.ubIndietro.Text = "INDIETRO";
                        ucdnBModalVal.ubAvanti.Visible = true;
                        ucdnBModalVal.ubAvanti.Text = "VALIDA TUTTI";
                        ucdnBModalVal.Refresh();
                        break;

                    case EnumMaschere.DatiEpisodio:
                    case EnumMaschere.DatiAnagraficiPaziente:
                        var ucdnTModalIP = ((frmInfoPaziente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalIP = ((frmInfoPaziente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalIP.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalIP.ubIndietro.Visible = true;
                        ucdnBModalIP.ubIndietro.Text = "INDIETRO";
                        ucdnBModalIP.ubAvanti.Visible = false;
                        ucdnBModalIP.Refresh();
                        break;

                    case EnumMaschere.Tracker:
                        var ucdnTModalTC = ((frmTracker)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalTC = ((frmTracker)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalTC.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalTC.ubIndietro.Visible = true;
                        ucdnBModalTC.ubIndietro.Text = "INDIETRO";
                        ucdnBModalTC.ubAvanti.Visible = false;
                        ucdnBModalTC.Refresh();
                        break;

                    case EnumMaschere.NoteAnamnesticheAlertAllergie:
                        var ucdnTModalAA = ((frmAlertAllergie)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalAA = ((frmAlertAllergie)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalAA.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalAA.ubIndietro.Visible = false;
                        ucdnBModalAA.ubIndietro.Text = "INDIETRO";
                        ucdnBModalAA.ubAvanti.Visible = true;
                        ucdnBModalAA.ubAvanti.Text = "CHIUDI";
                        ucdnBModalAA.Refresh();
                        break;

                    case EnumMaschere.TestiPredefiniti:
                        var ucdnTModalTP = ((frmTestiTipo)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalTP = ((frmTestiTipo)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalTP.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalTP.ubIndietro.Visible = true;
                        ucdnBModalTP.ubIndietro.Text = "INDIETRO";
                        ucdnBModalTP.ubAvanti.Visible = true;
                        ucdnBModalTP.ubAvanti.Text = "CONFERMA";
                        ucdnBModalTP.Refresh();
                        break;

                    case EnumMaschere.AcquisizioneImmaginePaziente:
                        var ucdnTModalAIP = ((frmFotoPaziente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalAIP = ((frmFotoPaziente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalAIP.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalAIP.ubIndietro.Visible = true;
                        ucdnBModalAIP.ubIndietro.Text = "INDIETRO";
                        ucdnBModalAIP.ubAvanti.Visible = true;
                        ucdnBModalAIP.ubAvanti.Text = "CONFERMA";
                        ucdnBModalAIP.Refresh();
                        break;

                    case EnumMaschere.AlertGenerici:
                        var ucdnTModalAG = ((frmAlertGenerici)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalAG = ((frmAlertGenerici)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalAG.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalAG.ubIndietro.Visible = false;
                        ucdnBModalAG.ubIndietro.Text = "INDIETRO";
                        ucdnBModalAG.ubAvanti.Visible = true;
                        ucdnBModalAG.ubAvanti.Text = "CHIUDI";
                        ucdnBModalAG.Refresh();
                        break;

                    case EnumMaschere.ConsegnePazienteCartella:
                        var ucdnTModalCPC = ((frmConsegnePazienteCartella)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalCPC = ((frmConsegnePazienteCartella)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalCPC.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalCPC.ubIndietro.Visible = false;
                        ucdnBModalCPC.ubIndietro.Text = "INDIETRO";
                        ucdnBModalCPC.ubAvanti.Visible = true;
                        ucdnBModalCPC.ubAvanti.Text = "CHIUDI";
                        ucdnBModalCPC.Refresh();
                        break;

                    case EnumMaschere.SelezioneTipoConsegnaPaziente:
                        var ucdnTModalCPCSEL = ((frmSelezionaTipoConsegnaPaziente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalCPCSEL = ((frmSelezionaTipoConsegnaPaziente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalCPCSEL.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalCPCSEL.ubIndietro.Visible = true;
                        ucdnBModalCPCSEL.ubIndietro.Text = "INDIETRO";
                        ucdnBModalCPCSEL.ubAvanti.Visible = true;
                        ucdnBModalCPCSEL.ubAvanti.Text = "AVANTI";
                        ucdnBModalCPCSEL.Refresh();
                        break;

                    case EnumMaschere.EditingConsegnaPaziente:
                        var ucdnTModalCPCPU = ((frmPUConsegnePazienteCartella)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalCPCPU = ((frmPUConsegnePazienteCartella)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalCPCPU.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalCPCPU.ubIndietro.Visible = true;
                        ucdnBModalCPCPU.ubIndietro.Text = "ANNULLA";
                        ucdnBModalCPCPU.ubAvanti.Visible = true;
                        ucdnBModalCPCPU.ubAvanti.Text = "CONFERMA";
                        ucdnBModalCPCPU.Refresh();
                        break;

                    case EnumMaschere.EditingParametriVitali:
                        var ucdnTModalEPVT = ((frmPUParametriVitali)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalEPVT = ((frmPUParametriVitali)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalEPVT.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalEPVT.ubIndietro.Visible = true;
                        ucdnBModalEPVT.ubIndietro.Text = "INDIETRO";
                        ucdnBModalEPVT.ubAvanti.Visible = true;
                        ucdnBModalEPVT.ubAvanti.Text = "CONFERMA";
                        ucdnBModalEPVT.Refresh();
                        break;

                    case EnumMaschere.EditingEvidenzaClinica:
                        var ucdnTModalEEC = ((frmPUEvidenzaClinica)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalEEC = ((frmPUEvidenzaClinica)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalEEC.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalEEC.ubIndietro.Visible = true;
                        ucdnBModalEEC.ubIndietro.Text = "INDIETRO";
                        ucdnBModalEEC.ubAvanti.Visible = true;
                        ucdnBModalEEC.ubAvanti.Text = "CONFERMA";
                        ucdnBModalEEC.Refresh();
                        break;

                    case EnumMaschere.EvidenzaClinicaPDF:
                        var ucdnTModalECP = ((frmPUPDFReferto)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalECP = ((frmPUPDFReferto)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalECP.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalECP.ubIndietro.Visible = true;
                        ucdnBModalECP.ubIndietro.Text = "INDIETRO";
                        ucdnBModalECP.ubAvanti.Visible = false;
                        ucdnBModalECP.ubAvanti.Text = "";
                        ucdnBModalECP.Refresh();
                        break;

                    case EnumMaschere.EvidenzaClinicaPACS:
                        var ucdnTModalECPX = ((frmPUPACS)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalECPX = ((frmPUPACS)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalECPX.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalECPX.ubIndietro.Visible = true;
                        ucdnBModalECPX.ubIndietro.Text = "INDIETRO";
                        ucdnBModalECPX.ubAvanti.Visible = false;
                        ucdnBModalECPX.ubAvanti.Text = "CONFERMA";
                        ucdnBModalECPX.Refresh();
                        break;

                    case EnumMaschere.LetteraDiDimissione:
                        var ucdnTModalLDD = ((frmPULettaraDimissione)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalLDD = ((frmPULettaraDimissione)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalLDD.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalLDD.ubIndietro.Visible = false;
                        ucdnBModalLDD.ubIndietro.Text = "INDIETRO";
                        ucdnBModalLDD.ubAvanti.Visible = true;
                        ucdnBModalLDD.ubAvanti.Text = "CHIUDI";
                        ucdnBModalLDD.Refresh();
                        break;

                    case EnumMaschere.Psc:
                        var ucTopModalePsc = ((frmPUFarmaci)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModalePsc = ((frmPUFarmaci)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModalePsc.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModalePsc.ubIndietro.Visible = false;
                        ucBottomModalePsc.ubAvanti.Visible = true;
                        ucBottomModalePsc.ubAvanti.Text = "CHIUDI";
                        ucBottomModalePsc.Refresh();
                        break;

                    case EnumMaschere.Consulenze_Login:
                        var ucdnTModalCLG = ((frmLoginConsulente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalCLG = ((frmLoginConsulente)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalCLG.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalCLG.ubIndietro.Visible = true;
                        ucdnBModalCLG.ubIndietro.Text = "INDIETRO";
                        ucdnBModalCLG.ubAvanti.Visible = true;
                        ucdnBModalCLG.ubAvanti.Text = "ACCEDI";
                        ucdnBModalCLG.Refresh();
                        break;

                    case EnumMaschere.AllegatiInserisciElettronico:
                    case EnumMaschere.AllegatiInserisciVirtuale:
                    case EnumMaschere.AllegatiEditing:
                        var ucdnTModalALL = ((frmPUAllegato)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalALL = ((frmPUAllegato)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalALL.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalALL.ubIndietro.Visible = true;
                        ucdnBModalALL.ubIndietro.Text = "INDIETRO";
                        ucdnBModalALL.ubAvanti.Visible = true;
                        ucdnBModalALL.ubAvanti.Text = "CONFERMA";
                        ucdnBModalALL.Refresh();
                        break;

                    case EnumMaschere.AllegatiAcquisizione:
                        var ucdnTModalAllACQ = ((frmPUAcquisizione)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalAllACQ = ((frmPUAcquisizione)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalAllACQ.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalAllACQ.ubIndietro.Visible = true;
                        ucdnBModalAllACQ.ubIndietro.Text = "ANNULLA";
                        ucdnBModalAllACQ.ubAvanti.Visible = true;
                        ucdnBModalAllACQ.ubAvanti.Text = "CONFERMA";
                        ucdnBModalAllACQ.Refresh();
                        break;

                    case EnumMaschere.SelezioneTipoParametriVitali:
                        var ucTModalSelezionePVT = ((frmSelezionaTipoParametroVitale)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalSelezionePVT = ((frmSelezionaTipoParametroVitale)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalSelezionePVT.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalSelezionePVT.Refresh();
                        ucBModalSelezionePVT.ubIndietro.Visible = true;
                        ucBModalSelezionePVT.ubIndietro.Text = "ANNULLA";
                        ucBModalSelezionePVT.ubAvanti.Visible = true;
                        ucBModalSelezionePVT.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.ErogazioneTaskInfermieristici:
                        var ucTModalSelezioneETI = ((frmPUTaskInfermieristiciEroga)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalSelezioneETI = ((frmPUTaskInfermieristiciEroga)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalSelezioneETI.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalSelezioneETI.Refresh();
                        ucBModalSelezioneETI.ubIndietro.Visible = true;
                        ucBModalSelezioneETI.ubAvanti.Visible = true;
                        ucBModalSelezioneETI.ubIndietro.Text = "INDIETRO";

                        if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione == EnumAzioni.VAL)
                        {
                            ucBModalSelezioneETI.ubAvanti.Text = "EROGA";
                        }
                        else if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione == EnumAzioni.VIS)
                        {
                            if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodStatoTaskInfermieristico == EnumStatoTaskInfermieristico.ER.ToString())
                            {
                                ucBModalSelezioneETI.ubAvanti.Text = "EROGA";
                            }
                            else if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodStatoTaskInfermieristico == EnumStatoTaskInfermieristico.AN.ToString())
                            {
                                ucBModalSelezioneETI.ubAvanti.Text = "ANNULLA ATT.";
                            }
                        }
                        else
                        {
                            ucBModalSelezioneETI.ubAvanti.Text = "ANNULLA ATT.";
                        }

                        break;

                    case EnumMaschere.SelezioneTipoNoteAnamnesticheAlertAllergie:
                        var ucTModalSelezioneTAA = ((frmSelezionaTipoAlertAllergie)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalSelezioneTAA = ((frmSelezionaTipoAlertAllergie)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalSelezioneTAA.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalSelezioneTAA.Refresh();
                        ucBModalSelezioneTAA.ubIndietro.Visible = true;
                        ucBModalSelezioneTAA.ubIndietro.Text = "ANNULLA";
                        ucBModalSelezioneTAA.ubAvanti.Visible = true;
                        ucBModalSelezioneTAA.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.EditingNoteAnamnesticheAlertAllergie:
                        var ucdnTModalEAA = ((frmPUAlertAllergie)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalEAA = ((frmPUAlertAllergie)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalEAA.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalEAA.ubIndietro.Visible = true;
                        ucdnBModalEAA.ubIndietro.Text = "INDIETRO";
                        ucdnBModalEAA.ubAvanti.Visible = true;
                        ucdnBModalEAA.ubAvanti.Text = "CONFERMA";
                        ucdnBModalEAA.Refresh();
                        break;

                    case EnumMaschere.SelezioneTipoAlertGenerico:
                        var ucTModalSelezioneTAG = ((frmSelezionaTipoAlertGenerico)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalSelezioneTAG = ((frmSelezionaTipoAlertGenerico)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalSelezioneTAG.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalSelezioneTAG.Refresh();
                        ucBModalSelezioneTAG.ubIndietro.Visible = true;
                        ucBModalSelezioneTAG.ubIndietro.Text = "ANNULLA";
                        ucBModalSelezioneTAG.ubAvanti.Visible = true;
                        ucBModalSelezioneTAG.ubAvanti.Text = "CONFERMA";
                        break;

                    case EnumMaschere.EditingAlertGenerico:
                        var ucdnTModalEAG = ((frmPUAlertGenerico)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalEAG = ((frmPUAlertGenerico)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalEAG.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalEAG.ubIndietro.Visible = true;
                        ucdnBModalEAG.ubIndietro.Text = "INDIETRO";
                        ucdnBModalEAG.ubAvanti.Visible = true;
                        ucdnBModalEAG.ubAvanti.Text = "CONFERMA";
                        ucdnBModalEAG.Refresh();
                        break;

                    case EnumMaschere.GraficoParametriVitali:
                        var ucdnTModalGPV = ((frmPUGrafico)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalGPV = ((frmPUGrafico)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalGPV.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalGPV.ubIndietro.Visible = true;
                        ucdnBModalGPV.ubIndietro.Text = "INDIETRO";
                        ucdnBModalGPV.ubAvanti.Visible = false;
                        ucdnBModalGPV.Refresh();
                        break;

                    case EnumMaschere.GraficiEvidenzaClinica:
                        var ucdnTModalGEC = ((frmPUGrafico)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalGEC = ((frmPUGrafico)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalGEC.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalGEC.ubIndietro.Visible = true;
                        ucdnBModalGEC.ubIndietro.Text = "INDIETRO";
                        ucdnBModalGEC.ubAvanti.Visible = false;
                        ucdnBModalGEC.Refresh();
                        break;

                    case EnumMaschere.Ambulatoriale_Scheda:
                    case EnumMaschere.Scheda:
                        var ucTModalPUScheda = ((frmPUScheda)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalPUScheda = ((frmPUScheda)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalPUScheda.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalPUScheda.ubIndietro.Visible = true;
                        ucBModalPUScheda.ubIndietro.Text = "ANNULLA";
                        ucBModalPUScheda.ubAvanti.Visible = true;
                        ucBModalPUScheda.ubAvanti.Text = "CONFERMA";
                        ucBModalPUScheda.Refresh();
                        break;

                    case EnumMaschere.EditingTaskInfermieristiciProtocollo:
                        ucTopModale ucdnTModalEWKIP = null;
                        ucBottomModale ucdnBModalEWKIP = null;

                        ucdnTModalEWKIP = ((frmPUProtocolloAttivita)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        ucdnBModalEWKIP = ((frmPUProtocolloAttivita)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;

                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalEWKIP.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalEWKIP.ubIndietro.Visible = true;
                        ucdnBModalEWKIP.ubIndietro.Text = "INDIETRO";
                        ucdnBModalEWKIP.ubAvanti.Visible = true;
                        ucdnBModalEWKIP.ubAvanti.Text = "CONFERMA";
                        ucdnBModalEWKIP.Refresh();
                        break;

                    case EnumMaschere.EditingTaskInfermieristici:
                        ucTopModale ucdnTModalEWKI = null;
                        ucBottomModale ucdnBModalEWKI = null;
                        ucdnTModalEWKI = ((frmPUTaskInfermieristici)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        ucdnBModalEWKI = ((frmPUTaskInfermieristici)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;

                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalEWKI.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalEWKI.ubIndietro.Visible = true;
                        ucdnBModalEWKI.ubIndietro.Text = "INDIETRO";
                        ucdnBModalEWKI.ubAvanti.Visible = true;
                        ucdnBModalEWKI.ubAvanti.Text = "CONFERMA";
                        ucdnBModalEWKI.Refresh();
                        break;

                    case EnumMaschere.MultiTaskInfermieristici:
                        ucTopModale ucdnTModalEWKIMulti = null;
                        ucBottomModale ucdnBModalEWKIMulti = null;
                        ucdnTModalEWKI = ((frmPUMultiTaskInfermieristici)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        ucdnBModalEWKI = ((frmPUMultiTaskInfermieristici)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;

                        /* 
                         * Contesto Top Modale
                         */
                        ucdnTModalEWKIMulti.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucdnBModalEWKIMulti.ubIndietro.Visible = true;
                        ucdnBModalEWKIMulti.ubIndietro.Text = "INDIETRO";
                        ucdnBModalEWKIMulti.ubAvanti.Visible = true;
                        ucdnBModalEWKIMulti.ubAvanti.Text = "CONFERMA";
                        ucdnBModalEWKIMulti.Refresh();
                        break;

                    case EnumMaschere.ConfermaPresainCarico:
                        var ucdnTModalApertura = ((frmPUAperturaCartella)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalApertura = ((frmPUAperturaCartella)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top 
                         */
                        ucdnTModalApertura.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucdnBModalApertura.ubIndietro.Text = "ANNULLA APERTURA";
                        ucdnBModalApertura.ubIndietro.Visible = true;
                        ucdnBModalApertura.ubAvanti.Text = "CONFERMA APERTURA";
                        ucdnBModalApertura.ubAvanti.Visible = true;
                        ucdnBModalApertura.Refresh();
                        break;

                    case EnumMaschere.ConfermaPresainCaricoAmbulatoriale:
                        var ucdnTModalAperturaA = ((frmPUAperturaCartellaAmbulatoriale)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucdnBModalAperturaA = ((frmPUAperturaCartellaAmbulatoriale)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top 
                         */
                        ucdnTModalAperturaA.Refresh();
                        /* 
                         * Contesto Bottom 
                         */
                        ucdnBModalAperturaA.ubIndietro.Text = "ANNULLA APERTURA";
                        ucdnBModalAperturaA.ubIndietro.Visible = true;
                        ucdnBModalAperturaA.ubAvanti.Text = "CONFERMA APERTURA";
                        ucdnBModalAperturaA.ubAvanti.Visible = true;
                        ucdnBModalAperturaA.Refresh();

                        break;
                    case EnumMaschere.EditingOrdine:
                    case EnumMaschere.Ambulatoriale_EditingOrdine:
                        var ucTopModaleOrdineE = ((frmPUOrdine)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleOrdineE = ((frmPUOrdine)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleOrdineE.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleOrdineE.Refresh();
                        ucBottomModaleOrdineE.ubIndietro.Visible = true;
                        ucBottomModaleOrdineE.ubIndietro.Text = "INDIETRO";
                        ucBottomModaleOrdineE.ubAvanti.Visible = true;
                        ucBottomModaleOrdineE.ubAvanti.Text = "DATI AGGIUNTIVI";
                        break;

                    case EnumMaschere.EditingOrdineDatiAggiuntivi:
                    case EnumMaschere.Ambulatoriale_EditingOrdineDatiAggiuntivi:
                        var ucTopModaleOrdineEDatiAgg = ((frmPUOrdineDatiAggiuntivi)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleOrdineEDatiAgg = ((frmPUOrdineDatiAggiuntivi)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleOrdineEDatiAgg.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleOrdineEDatiAgg.Refresh();
                        ucBottomModaleOrdineEDatiAgg.ubIndietro.Visible = true;
                        ucBottomModaleOrdineEDatiAgg.ubIndietro.Text = "INDIETRO";
                        ucBottomModaleOrdineEDatiAgg.ubAvanti.Visible = true;
                        ucBottomModaleOrdineEDatiAgg.ubAvanti.Text = "SALVA BOZZA";
                        break;

                    case EnumMaschere.EditingOrdineDatiAggiuntiviTrasfusionale:
                    case EnumMaschere.Ambulatoriale_EditingOrdineDatiAggiuntiviTrasfusionale:
                        var ucTopModaleOrdineEDatiAggTrasf = ((frmPUOrdineDatiAggiuntivi)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleOrdineEDatiAggTrasf = ((frmPUOrdineDatiAggiuntivi)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleOrdineEDatiAggTrasf.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleOrdineEDatiAggTrasf.Refresh();
                        ucBottomModaleOrdineEDatiAggTrasf.ubIndietro.Visible = true;
                        ucBottomModaleOrdineEDatiAggTrasf.ubIndietro.Text = "CANCELLA ORDINE";
                        ucBottomModaleOrdineEDatiAggTrasf.ubAvanti.Visible = false;
                        ucBottomModaleOrdineEDatiAggTrasf.ubAvanti.Text = "INOLTRA ORDINE";
                        break;

                    case EnumMaschere.VisualizzaOrdine:
                        var ucTopModaleVisualizzaOrdine = ((frmPUOrdineVisualizza)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBottomModaleVisualizzaOrdine = ((frmPUOrdineVisualizza)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTopModaleVisualizzaOrdine.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucTopModaleVisualizzaOrdine.Refresh();
                        ucBottomModaleVisualizzaOrdine.ubIndietro.Visible = false;
                        ucBottomModaleVisualizzaOrdine.ubAvanti.Visible = true;
                        ucBottomModaleVisualizzaOrdine.ubAvanti.Text = "CHIUDI";
                        break;

                    case EnumMaschere.ProseguiTerapia:
                        var ucBottomModaleProseguiTerapia = ((frmPUProseguiTerapia)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;

                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBottomModaleProseguiTerapia.Refresh();
                        ucBottomModaleProseguiTerapia.ubAvanti.Text = "VALIDA TERAPIA";
                        break;

                    case EnumMaschere.RichiediPassword:
                        var ucTModalRichiediPassword = ((frmRichiediPassword)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucTopModale;
                        var ucBModalRichiediPassword = ((frmRichiediPassword)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlModal).ucBottomModale;
                        /* 
                         * Contesto Top Modale
                         */
                        ucTModalRichiediPassword.Refresh();
                        /* 
                         * Contesto Bottom Modale
                         */
                        ucBModalRichiediPassword.Refresh();
                        ucBModalRichiediPassword.ubIndietro.Visible = true;
                        ucBModalRichiediPassword.ubIndietro.Text = "ESCI";
                        ucBModalRichiediPassword.ubAvanti.Visible = true;
                        ucBModalRichiediPassword.ubAvanti.Text = "ENTRA";
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        private void CaricaContesto(EnumMaschere maschera, Maschera omaschera)
        {

            try
            {

                switch (maschera)
                {

                    case EnumMaschere.VoceDiDiarioNM:
                        var TopVoceDiDiarioNM = ((frmPUDiarioClinicoNM)omaschera.ControlModal).ucTopNonModale;
                        var BottomVoceDiDiarioNM = ((frmPUDiarioClinicoNM)omaschera.ControlModal).ucBottomNonModale;
                        /* 
                         * Contesto Top Non Modale
                         */
                        TopVoceDiDiarioNM.Refresh();
                        /* 
                         * Contesto Bottom Non Modale
                         */
                        BottomVoceDiDiarioNM.ubIndietro.Visible = false;
                        BottomVoceDiDiarioNM.ubIndietro.Text = "";
                        BottomVoceDiDiarioNM.ubAvanti.Visible = true;
                        BottomVoceDiDiarioNM.ubAvanti.Text = "CHIUDI";
                        BottomVoceDiDiarioNM.Refresh();
                        break;

                    case EnumMaschere.SchedaNM:
                        var TopSchedaNM = ((frmPUSchedaNM)omaschera.ControlModal).ucTopNonModale;
                        var BottomSchedaNM = ((frmPUSchedaNM)omaschera.ControlModal).ucBottomNonModale;
                        /* 
                         * Contesto Top Non Modale
                         */
                        TopSchedaNM.Refresh();
                        /* 
                         * Contesto Bottom Non Modale
                         */
                        BottomSchedaNM.ubIndietro.Visible = false;
                        BottomSchedaNM.ubIndietro.Text = "";
                        BottomSchedaNM.ubAvanti.Visible = true;
                        BottomSchedaNM.ubAvanti.Text = "CHIUDI";
                        BottomSchedaNM.Refresh();
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void CaricaMenuAvanti(ref ucBottom ucB, ref Ruoli r, EnumMaschere mascheracorrente)
        {
            try
            {
                bool ambulatoriale = false;

                switch (mascheracorrente)
                {
                    case EnumMaschere.Ambulatoriale_EBM:
                    case EnumMaschere.Ambulatoriale_Schede:
                    case EnumMaschere.Ambulatoriale_Scheda:
                    case EnumMaschere.Ambulatoriale_SelezioneTipoScheda:
                    case EnumMaschere.Ambulatoriale_AgendePaziente:
                    case EnumMaschere.Ambulatoriale_SelezioneAgendeAppuntamento:
                    case EnumMaschere.Ambulatoriale_SelezioneAppuntamento:
                    case EnumMaschere.Ambulatoriale_SelezioneStatoAppuntamento:
                    case EnumMaschere.Ambulatoriale_SelezioneTipoAppuntamento:
                    case EnumMaschere.Ambulatoriale_GestioneOrdini:
                    case EnumMaschere.Ambulatoriale_Cartella:
                    case EnumMaschere.Ambulatoriale_RicercaPaziente:
                    case EnumMaschere.PercorsoAmbulatoriale_RicercaPaziente:
                    case EnumMaschere.Ambulatoriale_EvidenzaClinica:
                    case EnumMaschere.Ambulatoriale_Allegati:
                    case EnumMaschere.Ambulatoriale_SelezioneUA:
                        ambulatoriale = true;
                        break;
                    default:
                        ambulatoriale = false;
                        break;
                }


                ucB.ubAvanti.Visible = true;
                ucB.ubAvanti.Text = "MENU";
                ucB.MenuPulsanteAvanti.Clear();
                ucB.MenuPulsanteAvantiCDSS.Clear();
                if (ambulatoriale)
                {

                    if (mascheracorrente != EnumMaschere.Ambulatoriale_Schede && r.RuoloSelezionato.Esiste(EnumModules.Schede_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Ambulatoriale_Schede.ToString(), new MenuToolDefinition(EnumMaschere.Ambulatoriale_Schede.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Ambulatoriale_Schede), Shortcut.AltF1));





                    if (mascheracorrente != EnumMaschere.Ambulatoriale_AgendePaziente && r.RuoloSelezionato.Esiste(EnumModules.Agende_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Ambulatoriale_AgendePaziente.ToString(), new MenuToolDefinition(EnumMaschere.Ambulatoriale_AgendePaziente.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Ambulatoriale_AgendePaziente), Shortcut.AltF6));

                    if (mascheracorrente != EnumMaschere.Ambulatoriale_EvidenzaClinica && r.RuoloSelezionato.Esiste(EnumModules.EvidenzaC_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Ambulatoriale_EvidenzaClinica.ToString(), new MenuToolDefinition(EnumMaschere.Ambulatoriale_EvidenzaClinica.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Ambulatoriale_EvidenzaClinica), Shortcut.AltF7));

                    if (mascheracorrente != EnumMaschere.Ambulatoriale_GestioneOrdini && r.RuoloSelezionato.Esiste(EnumModules.Ordini_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Ambulatoriale_GestioneOrdini.ToString(), new MenuToolDefinition(EnumMaschere.Ambulatoriale_GestioneOrdini.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Ambulatoriale_GestioneOrdini), Shortcut.AltF8));


                    if (mascheracorrente != EnumMaschere.Ambulatoriale_Allegati && r.RuoloSelezionato.Esiste(EnumModules.Allegati_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Ambulatoriale_Allegati.ToString(), new MenuToolDefinition(EnumMaschere.Ambulatoriale_Allegati.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Ambulatoriale_Allegati), Shortcut.AltF10));

                    if (mascheracorrente != EnumMaschere.Ambulatoriale_EBM && r.RuoloSelezionato.Esiste(EnumModules.Ebm_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Ambulatoriale_EBM.ToString(), new MenuToolDefinition(EnumMaschere.Ambulatoriale_EBM.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Ambulatoriale_EBM), Shortcut.AltF11));



                    if (mascheracorrente != EnumMaschere.Ambulatoriale_Mow && r.RuoloSelezionato.Esiste(EnumModules.Mow_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Ambulatoriale_Mow.ToString(), new MenuToolDefinition(EnumMaschere.Ambulatoriale_Mow.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Ambulatoriale_Mow), Shortcut.CtrlShiftM));

                    if (r.RuoloSelezionato.Esiste(EnumModules.AltreFunzioni_Menu))
                    {
                        RispostaCerca oRispostaCerca = PluginClientStatics.PluginClientMenu(EnumPluginClient.MENU_AMBULATORIALE.ToString(), CommonStatics.UAPadri(CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata, CoreStatics.CoreApplication.Ambiente));
                        foreach (Plugin oPlugin in oRispostaCerca.Plugin)
                        {
                            if (ucB.MenuPulsanteAvantiCDSS.ContainsKey(oPlugin.Codice) == false)
                            {
                                ucB.MenuPulsanteAvantiCDSS.Add(oPlugin.Codice, oPlugin);
                            }
                        }
                    }

                }
                else
                {
                    if (mascheracorrente != EnumMaschere.Schede && r.RuoloSelezionato.Esiste(EnumModules.Schede_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Schede.ToString(), new MenuToolDefinition(EnumMaschere.Schede.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Schede), Shortcut.AltF1));

                    if (mascheracorrente != EnumMaschere.DiarioClinico && r.RuoloSelezionato.Esiste(EnumModules.DiarioC_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.DiarioClinico.ToString(), new MenuToolDefinition(EnumMaschere.DiarioClinico.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.DiarioClinico), Shortcut.AltF2));

                    if (mascheracorrente != EnumMaschere.ParametriVitali && r.RuoloSelezionato.Esiste(EnumModules.ParamV_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.ParametriVitali.ToString(), new MenuToolDefinition(EnumMaschere.ParametriVitali.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.ParametriVitali), Shortcut.AltF3));

                    if (mascheracorrente != EnumMaschere.TerapieFarmacologiche && r.RuoloSelezionato.Esiste(EnumModules.Prescr_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.TerapieFarmacologiche.ToString(), new MenuToolDefinition(EnumMaschere.TerapieFarmacologiche.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.TerapieFarmacologiche), Shortcut.AltF4));

                    if (mascheracorrente != EnumMaschere.WorklistInfermieristica && r.RuoloSelezionato.Esiste(EnumModules.WorkL_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.WorklistInfermieristica.ToString(), new MenuToolDefinition(EnumMaschere.WorklistInfermieristica.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.WorklistInfermieristica), Shortcut.AltF5));

                    if (mascheracorrente != EnumMaschere.AgendePaziente && r.RuoloSelezionato.Esiste(EnumModules.Agende_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.AgendePaziente.ToString(), new MenuToolDefinition(EnumMaschere.AgendePaziente.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.AgendePaziente), Shortcut.AltF6));

                    if (mascheracorrente != EnumMaschere.EvidenzaClinica && r.RuoloSelezionato.Esiste(EnumModules.EvidenzaC_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.EvidenzaClinica.ToString(), new MenuToolDefinition(EnumMaschere.EvidenzaClinica.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.EvidenzaClinica), Shortcut.AltF7));

                    if (mascheracorrente != EnumMaschere.GestioneOrdini && r.RuoloSelezionato.Esiste(EnumModules.Ordini_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.GestioneOrdini.ToString(), new MenuToolDefinition(EnumMaschere.GestioneOrdini.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.GestioneOrdini), Shortcut.AltF8));

                    if (mascheracorrente != EnumMaschere.LetteraDiDimissione && r.RuoloSelezionato.Esiste(EnumModules.LetteraD_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.LetteraDiDimissione.ToString(), new MenuToolDefinition(EnumMaschere.LetteraDiDimissione.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.LetteraDiDimissione), Shortcut.AltF9));

                    if (mascheracorrente != EnumMaschere.Allegati && r.RuoloSelezionato.Esiste(EnumModules.Allegati_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Allegati.ToString(), new MenuToolDefinition(EnumMaschere.Allegati.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Allegati), Shortcut.AltF10));

                    if (mascheracorrente != EnumMaschere.EBM && r.RuoloSelezionato.Esiste(EnumModules.Ebm_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.EBM.ToString(), new MenuToolDefinition(EnumMaschere.EBM.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.EBM), Shortcut.AltF11));

                    if (mascheracorrente != EnumMaschere.FoglioUnico && r.RuoloSelezionato.Esiste(EnumModules.FoglioUT_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.FoglioUnico.ToString(), new MenuToolDefinition(EnumMaschere.FoglioUnico.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.FoglioUnico), Shortcut.AltF12));

                    if (mascheracorrente != EnumMaschere.Consulenze_Login && r.RuoloSelezionato.Esiste(EnumModules.Consulenza_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Consulenze_Login.ToString(), new MenuToolDefinition(EnumMaschere.Consulenze_Login.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Consulenze_Refertazione), Shortcut.CtrlR));

                    if (mascheracorrente != EnumMaschere.Mow && r.RuoloSelezionato.Esiste(EnumModules.Mow_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Mow.ToString(), new MenuToolDefinition(EnumMaschere.Mow.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Mow), Shortcut.CtrlShiftM));

                    if (mascheracorrente != EnumMaschere.Mow && r.RuoloSelezionato.Esiste(EnumModules.Psc_Menu))
                        ucB.MenuPulsanteAvanti.Add(EnumMaschere.Psc.ToString(), new MenuToolDefinition(EnumMaschere.Psc.ToString(), CoreStatics.GetEnumDescription(EnumMaschere.Psc), Shortcut.CtrlP));

                    if (r.RuoloSelezionato.Esiste(EnumModules.AltreFunzioni_Menu))
                    {
                        RispostaCerca oRispostaCerca = PluginClientStatics.PluginClientMenu(EnumPluginClient.MENU_CARTELLA.ToString(), CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                        foreach (Plugin oPlugin in oRispostaCerca.Plugin)
                        {
                            if (ucB.MenuPulsanteAvantiCDSS.ContainsKey(oPlugin.Codice) == false)
                            {
                                ucB.MenuPulsanteAvantiCDSS.Add(oPlugin.Codice, oPlugin);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private Maschera CaricaUserControl(EnumMaschere maschera)
        {

            try
            {

                Maschera oMaschera = null;

                string scodRuolo = "";
                if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null)
                    scodRuolo = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice;

                string scodUA = "";
                if (CoreStatics.CoreApplication.Trasferimento != null)
                    scodUA = CoreStatics.CoreApplication.Trasferimento.CodUA;

                if (CoreStatics.CoreApplication.Paziente != null && CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale != null && CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale.Trim() != "")
                    scodUA = CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale;

                switch (maschera)
                {

                    case EnumMaschere.MenuPrincipale:
                        ucMenu oMenu = new ucMenu();
                        oMenu.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oMenu, "0", scodRuolo, scodUA, true, false, false);
                        return oMaschera;

                    case EnumMaschere.WorklistInfermieristicaTrasversale:
                        ucWorkListInfermieristicaTrasversale oWorkListInfermieristicaTrasversale = new ucWorkListInfermieristicaTrasversale();
                        oWorkListInfermieristicaTrasversale.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oWorkListInfermieristicaTrasversale, "3", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;

                        return oMaschera;

                    case EnumMaschere.ConsulenzeTrasversali:
                        ucConsulenzeTrasversali oConsulenzeTrasversali = new ucConsulenzeTrasversali();
                        oConsulenzeTrasversali.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oConsulenzeTrasversali, "19", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;

                        return oMaschera;

                    case EnumMaschere.EvidenzaClinicaTrasversale:
                        ucEvidenzaClinicaTrasversale oEvidenzaClinicaTrasversale = new ucEvidenzaClinicaTrasversale();
                        oEvidenzaClinicaTrasversale.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oEvidenzaClinicaTrasversale, "8", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;

                        return oMaschera;

                    case EnumMaschere.ParametriVitaliTrasversali:
                        ucParametriVitaliTrasversali oParametriVitaliTrasversali = new ucParametriVitaliTrasversali();
                        oParametriVitaliTrasversali.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oParametriVitaliTrasversali, "2", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.IdentificazioneIterataPaziente:
                        frmIdentificazioneIterataPaziente ofrmIdentificazioneIterataPaziente = new frmIdentificazioneIterataPaziente();
                        oMaschera = new Maschera(maschera, ofrmIdentificazioneIterataPaziente, "2-1 (M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Consulenze_RicercaPaziente:
                        ucRicercaPazienti oConsRicercaPazienti = new ucRicercaPazienti();
                        oConsRicercaPazienti.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oConsRicercaPazienti, "5", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.RicercaPazienti:
                        ucRicercaPazienti oRicercaPazienti = new ucRicercaPazienti();
                        oRicercaPazienti.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oRicercaPazienti, "1", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.ChiusuraCartelle:
                        ucChiusuraCartelle oucChiusuraCartelle = new ucChiusuraCartelle();
                        oucChiusuraCartelle.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oucChiusuraCartelle, "6", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;

                        return oMaschera;

                    case EnumMaschere.FirmaCartelleAperte:
                        ucFirmaCartelleAperte oucFirmaCartelleAperte = new ucFirmaCartelleAperte();
                        oucFirmaCartelleAperte.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oucFirmaCartelleAperte, "17", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;

                        return oMaschera;

                    case EnumMaschere.CartelleInVisione:
                        ucCartelleInVisioneMenu oCartelleInVisioneMenu = new ucCartelleInVisioneMenu();
                        oCartelleInVisioneMenu.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oCartelleInVisioneMenu, "16", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_SelezioneUA:
                        frmSelezionaUA ofrmSelezionaUA = new frmSelezionaUA();
                        oMaschera = new Maschera(maschera, ofrmSelezionaUA, "7-0(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_RicercaPaziente:
                        ucRicercaSAC oAmbRicercaPazienti = new ucRicercaSAC();
                        oAmbRicercaPazienti.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oAmbRicercaPazienti, "7", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.PercorsoAmbulatoriale_RicercaPaziente:
                        ucPercorsoAmbulatoriale oPercorsoAmbulatoriale = new ucPercorsoAmbulatoriale();
                        oPercorsoAmbulatoriale.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oPercorsoAmbulatoriale, "7-2", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.MatHome_RicercaPaziente:
                        ucRicercaSAC oMatHomeRicercaPazienti = new ucRicercaSAC();
                        oMatHomeRicercaPazienti.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oMatHomeRicercaPazienti, "80", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.MatHome_GestioneAccount:
                        frmPUGestioneAccount ofrmPUGestioneAccount = new frmPUGestioneAccount();
                        oMaschera = new Maschera(maschera, ofrmPUGestioneAccount, "80-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.PreTrasferimento_SelezioneUAUO:
                        frmSelezionaUAUO ofrmSelezionaUAUOPT = new frmSelezionaUAUO();
                        oMaschera = new Maschera(maschera, ofrmSelezionaUAUOPT, "14-0(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.PreTrasferimento_RicercaPaziente:
                        ucRicercaPreTrasferimenti oRicercaPreTrasferimenti = new ucRicercaPreTrasferimenti();
                        oRicercaPreTrasferimenti.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oRicercaPreTrasferimenti, "14", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;


                    case EnumMaschere.Ambulatoriale_Cartella:
                        ucCartellaAmbulatoriale oCartellaAmbu = new ucCartellaAmbulatoriale();
                        oCartellaAmbu.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oCartellaAmbu, "7-1", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.StampaCartelleChiuse:
                        ucStampaCartelleChiuse oucucStampaCartelleChiuse = new ucStampaCartelleChiuse();
                        oucucStampaCartelleChiuse.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oucucStampaCartelleChiuse, "10", scodRuolo, scodUA);
                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;

                        return oMaschera;

                    case EnumMaschere.CartellaPaziente:
                        oMaschera = this.ActionCreateMaschera(this.cf_CartellaPaziente, scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.CartellaInVisione:
                        frmPUCartellaInVisione ofrmPUCartellaInVisione = new frmPUCartellaInVisione();
                        oMaschera = new Maschera(maschera, ofrmPUCartellaInVisione, "1-1-0(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.PazienteInVisione:
                        frmPUPazienteInVisione ofrmPUPazienteInVisione = new frmPUPazienteInVisione();
                        oMaschera = new Maschera(maschera, ofrmPUPazienteInVisione, "16-1-0(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.CartellaPazienteChiusa:
                        frmCartellaChiusa ofrmCartellaChiusa = new frmCartellaChiusa();
                        oMaschera = new Maschera(maschera, ofrmCartellaChiusa, "1-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.DiarioClinico:
                        ucDiarioClinicoPaziente oDiarioClinicoPaziente = new ucDiarioClinicoPaziente();
                        oDiarioClinicoPaziente.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oDiarioClinicoPaziente, "1-1-2", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.Consegne:
                        ucConsegne oConsegne = new ucConsegne();
                        oConsegne.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oConsegne, "18", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;

                        return oMaschera;

                    case EnumMaschere.ConsegnePaziente:
                        ucConsegnePaziente oConsegneP = new ucConsegnePaziente();
                        oConsegneP.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oConsegneP, "20", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;

                        return oMaschera;

                    case EnumMaschere.Consegne_SelezioneUA:
                        frmSelezionaUAConsegne ofrmSelezionaUAConsegne = new frmSelezionaUAConsegne();
                        oMaschera = new Maschera(maschera, ofrmSelezionaUAConsegne, "18-3(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.ParametriVitali:
                        oMaschera = this.ActionCreateMaschera(this.cf_ParametriVitali, scodRuolo, scodUA);
                        return oMaschera;


                    case EnumMaschere.EvidenzaClinica:
                        ucEvidenzaClinica oEvidenzaClinica = new ucEvidenzaClinica();
                        oEvidenzaClinica.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oEvidenzaClinica, "1-1-7", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_EvidenzaClinica:
                        ucEvidenzaClinica oAmbEvidenzaClinica = new ucEvidenzaClinica();
                        oAmbEvidenzaClinica.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oAmbEvidenzaClinica, "7-1-1-7", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.Allegati:
                        ucAllegati oAllegati = new ucAllegati();
                        oAllegati.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oAllegati, "1-1-10", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_Allegati:
                        ucAllegati oAmbAllegati = new ucAllegati();
                        oAmbAllegati.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oAmbAllegati, "7-1-1-10", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.Schede:
                        ucSchede oSchede = new ucSchede();
                        oSchede.Dock = DockStyle.Fill;

                        oMaschera = new Maschera(maschera, oSchede, "1-1-1", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.SchedeModal:
                        frmPUSchede oPUSchedeM = new frmPUSchede();
                        oMaschera = new Maschera(maschera, oPUSchedeM, "1-1-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_Schede:
                        ucSchede oSchedeAmb = new ucSchede();
                        oSchedeAmb.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oSchedeAmb, "7-1-1-1", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.FoglioUnico:
                        ucFoglioUnico oFoglioUnico = new ucFoglioUnico();
                        oFoglioUnico.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oFoglioUnico, "1-1-12", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.AgendeTrasversali:
                        ucAgendeTrasversale oAgendeTrasversale = new ucAgendeTrasversale();
                        oAgendeTrasversale.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oAgendeTrasversale, "4", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;

                        return oMaschera;

                    case EnumMaschere.TrackerPaziente:
                        ucAgendePaziente oTrackerPaziente = new ucAgendePaziente();
                        oTrackerPaziente.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oTrackerPaziente, "4-4", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;

                        return oMaschera;

                    case EnumMaschere.SelezioneNota:
                        frmPUNota oPUNota = new frmPUNota();
                        oMaschera = new Maschera(maschera, oPUNota, "4-2", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.AgendePaziente:
                        ucAgendePaziente oAgendePaziente = new ucAgendePaziente();
                        oAgendePaziente.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oAgendePaziente, "1-1-6", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.SelezioneTipoAppuntamento:
                        frmSelezionaTipoAppuntamento oSelezionaTipoAppuntamento = new frmSelezionaTipoAppuntamento();
                        oMaschera = new Maschera(maschera, oSelezionaTipoAppuntamento, "1-1-6-0(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneAgendeAppuntamento:
                        frmSelezionaAgendeAppuntamento oSelezionaAgendeAppuntamento = new frmSelezionaAgendeAppuntamento();
                        oMaschera = new Maschera(maschera, oSelezionaAgendeAppuntamento, "1-1-6-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneStatoAppuntamento:
                        frmSelezionaStatoAppuntamento oSelezionaStatoAppuntamento = new frmSelezionaStatoAppuntamento();
                        oMaschera = new Maschera(maschera, oSelezionaStatoAppuntamento, "1-1-6-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneAppuntamento:
                        frmSelezionaAppuntamento oSelezionaAppuntamento = new frmSelezionaAppuntamento();
                        oMaschera = new Maschera(maschera, oSelezionaAppuntamento, "1-1-6-1-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.AnnullaAppuntamento:
                        frmSelezionaAppuntamento oAnnullaAppuntamento = new frmSelezionaAppuntamento();
                        oMaschera = new Maschera(maschera, oAnnullaAppuntamento, "1-1-6-1-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_AgendePaziente:
                        ucAgendePaziente oAgendePazienteAmb = new ucAgendePaziente();
                        oAgendePazienteAmb.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oAgendePazienteAmb, "7-1-1-6", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_SelezioneTipoAppuntamento:
                        frmSelezionaTipoAppuntamento oSelezionaTipoAppuntamentoAmb = new frmSelezionaTipoAppuntamento();
                        oMaschera = new Maschera(maschera, oSelezionaTipoAppuntamentoAmb, "7-1-1-6-0(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_SelezioneAgendeAppuntamento:
                        frmSelezionaAgendeAppuntamento oSelezionaAgendeAppuntamentoAmb = new frmSelezionaAgendeAppuntamento();
                        oMaschera = new Maschera(maschera, oSelezionaAgendeAppuntamentoAmb, "7-1-1-6-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_SelezioneStatoAppuntamento:
                        frmSelezionaStatoAppuntamento oSelezionaStatoAppuntamentoAmb = new frmSelezionaStatoAppuntamento();
                        oMaschera = new Maschera(maschera, oSelezionaStatoAppuntamentoAmb, "7-1-1-6-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_SelezioneAppuntamento:
                        frmSelezionaAppuntamento oSelezionaAppuntamentoAmb = new frmSelezionaAppuntamento();
                        oMaschera = new Maschera(maschera, oSelezionaAppuntamentoAmb, "7-1-1-6-1-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.RicorrenzaNota:
                        frmPURicorrenza oPURicorrenza = new frmPURicorrenza();
                        oMaschera = new Maschera(maschera, oPURicorrenza, "4-2-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.TestiNotePredefiniti:
                        frmSelezionaTestiNotePredefiniti oSelezionaTestiNotePredefinit = new frmSelezionaTestiNotePredefiniti();
                        oMaschera = new Maschera(maschera, oSelezionaTestiNotePredefinit, "4-2-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.RicercaSAC:
                        frmRicercaSAC oRicercaSAC = new frmRicercaSAC();
                        oMaschera = new Maschera(maschera, oRicercaSAC, "4-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezionaRuolo:
                        frmSelezionaRuolo oSelezionaRuolo = new frmSelezionaRuolo();
                        oMaschera = new Maschera(maschera, oSelezionaRuolo, "0-3(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.CambiaUtente:
                        frmCambiaUtente oCambiaUtente = new frmCambiaUtente();
                        oMaschera = new Maschera(maschera, oCambiaUtente, "0-4(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezionaReport:
                        this.MascheraSelezionata.CaricaReports(scodRuolo, scodUA);
                        frmSelezionaReport oSelezionaReport = new frmSelezionaReport();
                        oMaschera = new Maschera(maschera, oSelezionaReport, "X1-0(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Report:
                        frmReport oReport = new frmReport();
                        oMaschera = new Maschera(maschera, oReport, "X1-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.WebBrowser:
                        frmWebBrowser oWebBrowser = new frmWebBrowser();
                        oMaschera = new Maschera(maschera, oWebBrowser, "X9(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.StoricoReport:
                        frmStoricoReport oStoricoReport = new frmStoricoReport();
                        oMaschera = new Maschera(maschera, oStoricoReport, "X1-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.DettaglioNews:
                        frmDettaglioNews ofrmDettaglioNews = new frmDettaglioNews();
                        oMaschera = new Maschera(maschera, ofrmDettaglioNews, "0-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.ZoomAnteprimaRTFScheda:
                        frmPUZoomScheda ofrmPUZoomScheda = new frmPUZoomScheda();
                        oMaschera = new Maschera(maschera, ofrmPUZoomScheda, "1-1-1-8(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezionaTipoVoceDiario:
                        frmSelezionaTipoVoceDiario ofrmSelezionaTipoVoceDiario = new frmSelezionaTipoVoceDiario();
                        oMaschera = new Maschera(maschera, ofrmSelezionaTipoVoceDiario, "1-1-2-4(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneTipoAllegato:
                        frmSelezionaTipoAllegato ofrmSelezionaTipoAllegato = new frmSelezionaTipoAllegato();
                        oMaschera = new Maschera(maschera, ofrmSelezionaTipoAllegato, "1-1-10-0(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingConsegna:
                        frmPUConsegna ofrmPUConsegna = new frmPUConsegna();
                        oMaschera = new Maschera(maschera, ofrmPUConsegna, "18-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneTipoConsegna:
                        frmSelezionaTipoConsegna ofrmSelezionaTipoConsegna = new frmSelezionaTipoConsegna();
                        oMaschera = new Maschera(maschera, ofrmSelezionaTipoConsegna, "18-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingVoceDiDiario:
                        frmPUDiarioClinico ofrmPUDiarioClinico = new frmPUDiarioClinico();
                        oMaschera = new Maschera(maschera, ofrmPUDiarioClinico, "1-1-2-3(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.VoceDiDiarioNM:
                        frmPUDiarioClinicoNM ofrmPUDiarioClinicoNM = new frmPUDiarioClinicoNM();
                        oMaschera = new Maschera(maschera, ofrmPUDiarioClinicoNM, "1-1-2-3(NM)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.ValidazioneVociDiDiario:
                        frmValidazioni ofrmValidazioni = new frmValidazioni();
                        oMaschera = new Maschera(maschera, ofrmValidazioni, "1-1-2-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Consulenze_Refertazione:
                        frmPUConsulenza ofrmPUConsulenza = new frmPUConsulenza();
                        oMaschera = new Maschera(maschera, ofrmPUConsulenza, "5-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.DatiEpisodio:
                        frmInfoPaziente ofrmInfoEpisodio = new frmInfoPaziente();
                        ofrmInfoEpisodio.Sezione = ucInfoSchede.enumInfoSezione.infoEpisodio;
                        oMaschera = new Maschera(maschera, ofrmInfoEpisodio, "X4(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Tracker:
                        frmTracker ofrmTracker = new frmTracker();
                        oMaschera = new Maschera(maschera, ofrmTracker, "X4-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.DatiAnagraficiPaziente:
                        frmInfoPaziente ofrmInfoPaziente = new frmInfoPaziente();
                        ofrmInfoPaziente.Sezione = ucInfoSchede.enumInfoSezione.infoPaziente;
                        oMaschera = new Maschera(maschera, ofrmInfoPaziente, "X3(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.NoteAnamnesticheAlertAllergie:
                        frmAlertAllergie ofrmAlertAllergie = new frmAlertAllergie();
                        oMaschera = new Maschera(maschera, ofrmAlertAllergie, "X5(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.AlertGenerici:
                        frmAlertGenerici ofrmAlertGenerici = new frmAlertGenerici();
                        oMaschera = new Maschera(maschera, ofrmAlertGenerici, "X6(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingConsegnaPaziente:
                        frmPUConsegnePazienteCartella ofrmPUConsegnePazienteCartella = new frmPUConsegnePazienteCartella();
                        oMaschera = new Maschera(maschera, ofrmPUConsegnePazienteCartella, "20-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneTipoConsegnaPaziente:
                        frmSelezionaTipoConsegnaPaziente ofrmSelezionaTipoConsegnaPaziente = new frmSelezionaTipoConsegnaPaziente();
                        oMaschera = new Maschera(maschera, ofrmSelezionaTipoConsegnaPaziente, "20-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.ConsegnePazienteCartella:
                        frmConsegnePazienteCartella ofrmConsegnePazienteCartella = new frmConsegnePazienteCartella();
                        oMaschera = new Maschera(maschera, ofrmConsegnePazienteCartella, "20-4(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.TestiPredefiniti:
                        frmTestiTipo ofrmTestiTipo = new frmTestiTipo();
                        oMaschera = new Maschera(maschera, ofrmTestiTipo, "X2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.AcquisizioneImmaginePaziente:
                        frmFotoPaziente ofrmFotoPaziente = new frmFotoPaziente();
                        oMaschera = new Maschera(maschera, ofrmFotoPaziente, "X8(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingParametriVitali:
                        frmPUParametriVitali ofrmPUParametriVitali = new frmPUParametriVitali();
                        oMaschera = new Maschera(maschera, ofrmPUParametriVitali, "1-1-3-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingEvidenzaClinica:
                        frmPUEvidenzaClinica ofrmPUEvidenzaClinica = new frmPUEvidenzaClinica();
                        oMaschera = new Maschera(maschera, ofrmPUEvidenzaClinica, "1-1-7-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.GraficiEvidenzaClinica:
                        frmPUGrafico ofrmPUGraficoEC = new frmPUGrafico();
                        oMaschera = new Maschera(maschera, ofrmPUGraficoEC, "1-1-7-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EvidenzaClinicaPDF:
                        frmPUPDFReferto ofrmPUPDFReferto = new frmPUPDFReferto();
                        oMaschera = new Maschera(maschera, ofrmPUPDFReferto, "1-1-7-3(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EvidenzaClinicaPACS:
                        frmPUPACS ofrmPUPACS = new frmPUPACS();
                        oMaschera = new Maschera(maschera, ofrmPUPACS, "1-1-7-4(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.AllegatiInserisciVirtuale:
                        frmPUAllegato ofrmPUAllegatoIV = new frmPUAllegato();
                        oMaschera = new Maschera(maschera, ofrmPUAllegatoIV, "1-1-10-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.AllegatiInserisciElettronico:
                        frmPUAllegato ofrmPUAllegatoIE = new frmPUAllegato();
                        oMaschera = new Maschera(maschera, ofrmPUAllegatoIE, "1-1-10-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.AllegatiEditing:
                        frmPUAllegato ofrmPUAllegatoEd = new frmPUAllegato();
                        oMaschera = new Maschera(maschera, ofrmPUAllegatoEd, "1-1-10-3(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.AllegatiAcquisizione:
                        frmPUAcquisizione ofrmPUAcquisizione = new frmPUAcquisizione();
                        oMaschera = new Maschera(maschera, ofrmPUAcquisizione, "1-1-10-4(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.GraficoParametriVitali:
                        frmPUGrafico ofrmPUGrafico = new frmPUGrafico();
                        oMaschera = new Maschera(maschera, ofrmPUGrafico, "1-1-3-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneTipoParametriVitali:
                        frmSelezionaTipoParametroVitale ofrmSelezionaTipoParametroVitale = new frmSelezionaTipoParametroVitale();
                        oMaschera = new Maschera(maschera, ofrmSelezionaTipoParametroVitale, "1-1-3-3(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.LetteraDiDimissione:
                        frmPULettaraDimissione ofrmPULettaraDimissione = new frmPULettaraDimissione();
                        oMaschera = new Maschera(maschera, ofrmPULettaraDimissione, "1-1-9(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneTipoNoteAnamnesticheAlertAllergie:
                        frmSelezionaTipoAlertAllergie ofrmSelezionaTipoAlertAllergie = new frmSelezionaTipoAlertAllergie();
                        oMaschera = new Maschera(maschera, ofrmSelezionaTipoAlertAllergie, "X5-0(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingNoteAnamnesticheAlertAllergie:
                        frmPUAlertAllergie ofrmPUAlertAllergie = new frmPUAlertAllergie();
                        oMaschera = new Maschera(maschera, ofrmPUAlertAllergie, "X5-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Consulenze_Login:
                        frmLoginConsulente ofrmLoginConsulente = new frmLoginConsulente();
                        oMaschera = new Maschera(maschera, ofrmLoginConsulente, "5-1-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneTipoAlertGenerico:
                        frmSelezionaTipoAlertGenerico ofrmSelezionaTipoAlertGenerico = new frmSelezionaTipoAlertGenerico();
                        oMaschera = new Maschera(maschera, ofrmSelezionaTipoAlertGenerico, "X6-0(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingAlertGenerico:
                        frmPUAlertGenerico ofrmPUAlertGenerico = new frmPUAlertGenerico();
                        oMaschera = new Maschera(maschera, ofrmPUAlertGenerico, "X6-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Scheda:
                        frmPUScheda oPUScheda = new frmPUScheda();
                        oMaschera = new Maschera(maschera, oPUScheda, "1-1-1-3(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SchedaNM:
                        frmPUSchedaNM oPUSchedaNM = new frmPUSchedaNM();
                        oMaschera = new Maschera(maschera, oPUSchedaNM, "1-1-1-3(NM)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneTipoScheda:
                        frmSelezionaTipoScheda ofrmSelezionaTipoScheda = new frmSelezionaTipoScheda();
                        oMaschera = new Maschera(maschera, ofrmSelezionaTipoScheda, "1-1-1-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.ImportaDWH:

                    case EnumMaschere.Ambulatoriale_Scheda:
                        frmPUScheda oPUSchedaAmb = new frmPUScheda();
                        oMaschera = new Maschera(maschera, oPUSchedaAmb, "7-1-1-1-3(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_SelezioneTipoScheda:
                        frmSelezionaTipoScheda ofrmSelezionaTipoSchedaAmb = new frmSelezionaTipoScheda();
                        oMaschera = new Maschera(maschera, ofrmSelezionaTipoSchedaAmb, "7-1-1-1-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.WorklistInfermieristica:
                        ucWorkListInfermieristica oWorkListInfermieristica = new ucWorkListInfermieristica();
                        oWorkListInfermieristica.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oWorkListInfermieristica, "1-1-5", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.EditingTaskInfermieristici:
                        frmPUTaskInfermieristici ofrmPUTaskInfermieristici = new frmPUTaskInfermieristici();
                        oMaschera = new Maschera(maschera, ofrmPUTaskInfermieristici, "1-1-5-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.MultiTaskInfermieristici:
                        frmPUMultiTaskInfermieristici ofrmPUMultiTaskInfermieristici = new frmPUMultiTaskInfermieristici();
                        oMaschera = new Maschera(maschera, ofrmPUMultiTaskInfermieristici, "1-1-5-7(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingTaskInfermieristiciProtocollo:
                        frmPUProtocolloAttivita ofrmPUProtocolloAttivita = new frmPUProtocolloAttivita();
                        oMaschera = new Maschera(maschera, ofrmPUProtocolloAttivita, "1-1-5-6(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneTipoTaskInfermieristici:
                        frmSelezionaTipoTaskInfermieristico ofrmSelezionaTipoTaskInfermieristico = new frmSelezionaTipoTaskInfermieristico();
                        oMaschera = new Maschera(maschera, ofrmSelezionaTipoTaskInfermieristico, "1-1-5-4(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneTipoTaskInfermieristiciDaWKITrasversale:
                        frmSelezionaTipoTaskInfermieristico ofrmSelezionaTipoTaskInfermieristicoWKITrasversale = new frmSelezionaTipoTaskInfermieristico();
                        oMaschera = new Maschera(maschera, ofrmSelezionaTipoTaskInfermieristicoWKITrasversale, "1-1-5-4(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SpostaTaskInAltraCartella:
                        frmSpostaTaskInAltraCartella ofrmSelezionaCartella = new frmSpostaTaskInAltraCartella();
                        oMaschera = new Maschera(maschera, ofrmSelezionaCartella, "1-1-5-5(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.ErogazioneTaskInfermieristici:
                        frmPUTaskInfermieristiciEroga ofrmPUTaskInfermieristiciEroga = new frmPUTaskInfermieristiciEroga();
                        oMaschera = new Maschera(maschera, ofrmPUTaskInfermieristiciEroga, "1-1-5-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.TerapieFarmacologiche:
                        ucTerapieFarmacologiche oTerapieFarmacologiche = new ucTerapieFarmacologiche();
                        oTerapieFarmacologiche.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oTerapieFarmacologiche, "1-1-4", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.SelezioneTipoPrescrizione:
                        frmSelezionaTipoPrescrizione oSelezionaTipoPrescrizione = new frmSelezionaTipoPrescrizione();
                        oMaschera = new Maschera(maschera, oSelezionaTipoPrescrizione, "1-1-4-3(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezioneViaSomministrazione:
                        frmSelezionaViaSomministrazione oSelezionaViaSomministrazione = new frmSelezionaViaSomministrazione();
                        oMaschera = new Maschera(maschera, oSelezionaViaSomministrazione, "1-1-4-4(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingPrescrizione:
                        frmPUPrescrizioni ofrmPUPrescrizioni = new frmPUPrescrizioni();
                        oMaschera = new Maschera(maschera, ofrmPUPrescrizioni, "1-1-4-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingPrescrizioneTempi:
                        frmPUPrescrizioniTempi ofrmPuPrescrizioniTempi = new frmPUPrescrizioniTempi();
                        oMaschera = new Maschera(maschera, ofrmPuPrescrizioniTempi, "1-1-4-1-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingPrescrizioniProtocollo:
                        frmPUPrescrizioniProtocolloGenerate ofrmPUPrescrizioniProtocolloGenerate = new frmPUPrescrizioniProtocolloGenerate();
                        oMaschera = new Maschera(maschera, ofrmPUPrescrizioniProtocolloGenerate, "1-1-4-7(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.AnnullaPrescrizioneTempi:
                        frmPUAnnullaPrescrizioneTempi ofrmPUAnnullaPrescrizioneTempi = new frmPUAnnullaPrescrizioneTempi();
                        oMaschera = new Maschera(maschera, ofrmPUAnnullaPrescrizioneTempi, "1-1-4-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.ProseguiTerapia:
                        frmPUProseguiTerapia ofrmPUProseguiTerapia = new frmPUProseguiTerapia();
                        oMaschera = new Maschera(maschera, ofrmPUProseguiTerapia, "1-1-4-6(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.CopiaPrescrizioni:
                        frmCopiaPrescrizioni ofrmCopiaPrescrizioni = new frmCopiaPrescrizioni();
                        oMaschera = new Maschera(maschera, ofrmCopiaPrescrizioni, "1-1-4-5(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Immagine:
                        frmImmagine ofrmImmagine = new frmImmagine();
                        oMaschera = new Maschera(maschera, ofrmImmagine, "1-1-1-9(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.ConfermaPresainCarico:
                        frmPUAperturaCartella ofrmPUAperturaCartella = new frmPUAperturaCartella();
                        oMaschera = new Maschera(maschera, ofrmPUAperturaCartella, "X7(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.ConfermaPresainCaricoAmbulatoriale:
                        frmPUAperturaCartellaAmbulatoriale ofrmPUAperturaCartellaAmbulatoriale = new frmPUAperturaCartellaAmbulatoriale();
                        oMaschera = new Maschera(maschera, ofrmPUAperturaCartellaAmbulatoriale, "X7-1(M)", scodRuolo, scodUA);

                        return oMaschera;
                    case EnumMaschere.SelezionaCartella:
                        frmSelezionaCartella oSelezionaCartella = new frmSelezionaCartella();
                        oMaschera = new Maschera(maschera, oSelezionaCartella, "9(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.SelezionaCartellaCollegabile:
                        frmSelezionaCartellaCollegabile oSelezionaCartellaC = new frmSelezionaCartellaCollegabile();
                        oMaschera = new Maschera(maschera, oSelezionaCartellaC, "X7-0(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.GestioneOrdini:
                        ucOrderEntry oOrderEntry = new ucOrderEntry();
                        oOrderEntry.Dock = DockStyle.Fill;
                        oOrderEntry.PercorsoAmbulatoriale = false;
                        oMaschera = new Maschera(maschera, oOrderEntry, "1-1-8", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.EditingOrdine:
                        frmPUOrdine ofrmPUOrdine = new frmPUOrdine();
                        ofrmPUOrdine.PercorsoAmbulatoriale = false;
                        oMaschera = new Maschera(maschera, ofrmPUOrdine, "1-1-8-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingOrdineDatiAggiuntivi:
                        frmPUOrdineDatiAggiuntivi ofrmPUOrdineDatiAggiuntivi = new frmPUOrdineDatiAggiuntivi();
                        ofrmPUOrdineDatiAggiuntivi.PercorsoAmbulatoriale = false;
                        oMaschera = new Maschera(maschera, ofrmPUOrdineDatiAggiuntivi, "1-1-8-1-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EditingOrdineDatiAggiuntiviTrasfusionale:
                        frmPUOrdineDatiAggiuntivi ofrmPUOrdineDatiAggiuntiviTrasfusionale = new frmPUOrdineDatiAggiuntivi();
                        ofrmPUOrdineDatiAggiuntiviTrasfusionale.PercorsoAmbulatoriale = false;
                        oMaschera = new Maschera(maschera, ofrmPUOrdineDatiAggiuntiviTrasfusionale, "1-1-8-1-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.VisualizzaOrdine:
                        frmPUOrdineVisualizza ofrmPUOrdineVisualizza = new frmPUOrdineVisualizza();
                        oMaschera = new Maschera(maschera, ofrmPUOrdineVisualizza, "1-1-8-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Mow:
                        frmPUMow ofrmPUMow = new frmPUMow();
                        oMaschera = new Maschera(maschera, ofrmPUMow, "1-1-13(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Psc:
                        frmPUFarmaci ofrmPUFarmaci = new frmPUFarmaci();
                        oMaschera = new Maschera(maschera, ofrmPUFarmaci, "1-1-14(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_Mow:
                        frmPUMow ofrmPUMowA = new frmPUMow();
                        oMaschera = new Maschera(maschera, ofrmPUMowA, "7-1-13(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_GestioneOrdini:
                        ucOrderEntry oOrderEntryAmb = new ucOrderEntry();
                        oOrderEntryAmb.Dock = DockStyle.Fill;
                        oOrderEntryAmb.PercorsoAmbulatoriale = true;
                        oMaschera = new Maschera(maschera, oOrderEntryAmb, "7-1-1-8", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_SelezioneUOOrdini:
                        frmSelezionaUO ofrmSelezionaUO = new frmSelezionaUO();
                        oMaschera = new Maschera(maschera, ofrmSelezionaUO, "7-1-1-8-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_EditingOrdine:
                        frmPUOrdine ofrmPUOrdineAmb = new frmPUOrdine();
                        ofrmPUOrdineAmb.PercorsoAmbulatoriale = true;
                        oMaschera = new Maschera(maschera, ofrmPUOrdineAmb, "7-1-1-8-1-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_EditingOrdineDatiAggiuntivi:
                        frmPUOrdineDatiAggiuntivi ofrmPUOrdineDatiAggiuntiviAmb = new frmPUOrdineDatiAggiuntivi();
                        ofrmPUOrdineDatiAggiuntiviAmb.PercorsoAmbulatoriale = true;
                        oMaschera = new Maschera(maschera, ofrmPUOrdineDatiAggiuntiviAmb, "7-1-1-8-1-2(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_EditingOrdineDatiAggiuntiviTrasfusionale:
                        frmPUOrdineDatiAggiuntivi ofrmPUOrdineDatiAggiuntiviTrasfAmb = new frmPUOrdineDatiAggiuntivi();
                        ofrmPUOrdineDatiAggiuntiviTrasfAmb.PercorsoAmbulatoriale = true;
                        oMaschera = new Maschera(maschera, ofrmPUOrdineDatiAggiuntiviTrasfAmb, "7-1-1-8-1-3(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.EBM:
                        ucBancheDati oBancheDati = new ucBancheDati();
                        oBancheDati.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oBancheDati, "1-1-11", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.Ambulatoriale_EBM:
                        ucBancheDati oBancheDatiAmb = new ucBancheDati();
                        oBancheDatiAmb.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oBancheDatiAmb, "7-1-1-11", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = true;

                        return oMaschera;

                    case EnumMaschere.RichiediPassword:
                        frmRichiediPassword oRichiediPassword = new frmRichiediPassword();
                        oMaschera = new Maschera(maschera, oRichiediPassword, "0-7(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.RichiestaConsenso:
                        frmRichiestaConsenso oRichiestaConsenso = new frmRichiestaConsenso();
                        oMaschera = new Maschera(maschera, oRichiestaConsenso, "X3-1(M)", scodRuolo, scodUA);
                        return oMaschera;

                    case EnumMaschere.OrderEntryTrasversale:
                        ucOrderEntryTrasversale oOrderEntryTrasversale = new ucOrderEntryTrasversale();
                        oOrderEntryTrasversale.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oOrderEntryTrasversale, "15", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;

                        return oMaschera;

                    case EnumMaschere.OrderEntryMonitorTrasversale:
                        ucOrderEntryMonitorTrasversale oOrderEntryMonitorTrasversale = new ucOrderEntryMonitorTrasversale();
                        oOrderEntryMonitorTrasversale.Dock = DockStyle.Fill;
                        oMaschera = new Maschera(maschera, oOrderEntryMonitorTrasversale, "13", scodRuolo, scodUA);

                        oMaschera.Indietro = true;
                        oMaschera.Home = true;
                        oMaschera.Avanti = false;


                        return oMaschera;

                    default:
                        return null;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return null;
            }

        }

        public void NavigaMaschera(EnumMaschere mascheraprovenienza, EnumPulsante pulsante)
        {

            try
            {

                if (mascheraprovenienza == EnumMaschere.WorklistInfermieristica && CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;

                if (mascheraprovenienza == EnumMaschere.CartellaPaziente) this.TornaACartellaAbilitato = true;
                if (mascheraprovenienza == EnumMaschere.CartellaInVisione) this.TornaACartellaInVisioneAbilitato = true;

                switch (pulsante)
                {
                    case EnumPulsante.PulsanteVociDiarioClinicoTop:
                        this.CaricaMaschera(EnumMaschere.ValidazioneVociDiDiario);
                        break;

                    case EnumPulsante.PulsanteAllergieTop:
                        this.CaricaMaschera(EnumMaschere.NoteAnamnesticheAlertAllergie);
                        break;

                    case EnumPulsante.PulsanteAlertTop:
                        this.CaricaMaschera(EnumMaschere.AlertGenerici);
                        break;

                    case EnumPulsante.PulsanteEvidenzaClinicaTop:
                        this.CaricaMaschera(EnumMaschere.EvidenzaClinica);
                        break;


                    case EnumPulsante.PulsanteInfoPazienteTop:
                        this.CaricaMaschera(EnumMaschere.DatiAnagraficiPaziente);
                        break;

                    case EnumPulsante.PulsanteInfoEpisodioTop:
                        this.CaricaMaschera(EnumMaschere.DatiEpisodio);
                        break;

                    case EnumPulsante.PulsantePazienteTop:
                        this.CaricaMaschera(EnumMaschere.AcquisizioneImmaginePaziente);
                        break;

                    case EnumPulsante.PulsanteStampeBottom:
                        if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true && mascheraprovenienza == EnumMaschere.CartellaPaziente)
                        {
                            this.CaricaMaschera(EnumMaschere.CartellaPazienteChiusa);
                        }
                        else
                        {
                            this.CaricaMaschera(EnumMaschere.SelezionaReport);
                        }
                        break;

                    case EnumPulsante.PulsanteConsensiTop:
                        this.CaricaMaschera(EnumMaschere.RichiestaConsenso);
                        break;

                    case EnumPulsante.PulsanteConsegneTop:
                        this.CaricaMaschera(EnumMaschere.ConsegnePazienteCartella);
                        break;

                    default:

                        switch (mascheraprovenienza)
                        {

                            case EnumMaschere.MenuPrincipale:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        if (CoreStatics.CheckEsci() == true)
                                        {
                                            CoreStatics.CoreApplication.Navigazione.Maschere.TracciaNavigazione("Chiusura Programma.", true);
                                            CoreStatics.CoreApplicationContext.Exit();
                                        }
                                        break;

                                    case EnumPulsante.PulsanteCartellaPazienteBottom:
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                }
                                break;

                            case EnumMaschere.WorklistInfermieristicaTrasversale:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato = "";
                                        CoreStatics.CoreApplication.Paziente = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato = "";
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato = "";
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteAvantiBottom:
                                        break;

                                }
                                break;

                            case EnumMaschere.ConsulenzeTrasversali:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteAvantiBottom:
                                        break;

                                }
                                break;

                            case EnumMaschere.OrderEntryTrasversale:
                            case EnumMaschere.OrderEntryMonitorTrasversale:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.MovOrdineSelezionato = null;
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;
                                }

                                break;

                            case EnumMaschere.EvidenzaClinicaTrasversale:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteAvantiBottom:
                                        break;

                                }
                                break;

                            case EnumMaschere.AgendeTrasversali:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteAvantiBottom:
                                        break;

                                }
                                break;

                            case EnumMaschere.TrackerPaziente:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        this.CaricaMaschera(EnumMaschere.AgendeTrasversali);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteAvantiBottom:
                                        break;

                                }
                                break;

                            case EnumMaschere.ParametriVitaliTrasversali:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaTipiParametriSelezionati.Clear();
                                        CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaUASelezionate.Clear();
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaTipiParametriSelezionati.Clear();
                                        CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaUASelezionate.Clear();
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaTipiParametriSelezionati.Clear();
                                        CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaUASelezionate.Clear();
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteAvantiBottom:
                                        CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaTipiParametriSelezionati = ((ucParametriVitaliTrasversali)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle).ListaTipiParametriVitaliSelezionati;
                                        if (CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaTipiParametriSelezionati.Count > 0)
                                        {
                                            CoreStatics.CoreApplication.ParametriVitaliTrasversali.ListaUASelezionate = ((ucParametriVitaliTrasversali)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle).ListaUnitaAtomicheSelezionate;
                                            this.CaricaMaschera(EnumMaschere.IdentificazioneIterataPaziente);

                                        }
                                        else
                                            easyStatics.EasyMessageBox("Selezionare almeno un Parametro Vitale!", "Parametri Vitali", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                                        break;

                                }
                                break;

                            case EnumMaschere.Consulenze_RicercaPaziente:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.Consulenza = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.Consulenza = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.Consulenza = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteAvantiBottom:
                                        UltraGridRow oUgr = ((ucRicercaPazienti)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle).UltraGridRicerca.ActiveRow;
                                        if (oUgr != null)
                                        {


                                            if (oUgr.Cells["CodStatoCartella"].Text != Scci.Enums.EnumStatoCartella.AP.ToString())
                                            {

                                                easyStatics.EasyMessageBox("Occorre prima aprire la cartella!", "Consulenze", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                            }
                                            else
                                            {
                                                CoreStatics.CoreApplication.Paziente = new Paziente("", oUgr.Cells["IDEpisodio"].Text);
                                                CoreStatics.CoreApplication.Episodio = new Episodio(oUgr.Cells["IDEpisodio"].Text);
                                                CoreStatics.CoreApplication.Trasferimento = new Trasferimento(oUgr.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);
                                                CoreStatics.CoreApplication.Consulenza = new Consulenza(oUgr.Cells["IDEpisodio"].Text, oUgr.Cells["IDTrasferimento"].Text, oUgr.Cells["IDPaziente"].Text);
                                                CoreStatics.CoreApplication.Consulenza.InserisciConsulenza();
                                                CoreStatics.CoreApplication.Paziente = null;
                                                CoreStatics.CoreApplication.Episodio = null;
                                                CoreStatics.CoreApplication.Trasferimento = null;
                                                this.CloseAllFormNM();
                                            }
                                        }
                                        else
                                            easyStatics.EasyMessageBox("Selezionare almeno un Episodio!", "Consulenze", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        break;

                                }
                                break;

                            case EnumMaschere.RicercaPazienti:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteAvantiBottom:
                                        CoreStatics.CoreApplication.Consulenza = null;
                                        UltraGridRow oUgr = ((ucRicercaPazienti)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle).UltraGridRicerca.ActiveRow;
                                        if (oUgr != null)
                                        {
                                            CoreStatics.CoreApplication.Paziente = new Paziente("", oUgr.Cells["IDEpisodio"].Text);
                                            CoreStatics.CoreApplication.Episodio = new Episodio(oUgr.Cells["IDEpisodio"].Text);
                                            CoreStatics.CoreApplication.Trasferimento = new Trasferimento(oUgr.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);
                                            if (oUgr.Cells["IDCartella"].Text != "")
                                            {
                                                CoreStatics.CoreApplication.Cartella = new Cartella(oUgr.Cells["IDCartella"].Text, oUgr.Cells["NumeroCartella"].Text, CoreStatics.CoreApplication.Ambiente);
                                            }

                                            bool bSACConsensiAbilita = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.SACConsensiAbilita)));
                                            if (bSACConsensiAbilita)
                                            {

                                                if (CoreStatics.CoreApplication.Paziente.CodStatoConsensoCalcolato == EnumStatoConsensoCalcolato.NO.ToString())
                                                {
                                                    easyStatics.EasyMessageBox("Il paziente ha NEGATO il consenso, impossibile procedere.", "Consenso Paziente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                                    CoreStatics.CoreApplication.Paziente = null;
                                                    CoreStatics.CoreApplication.Episodio = null;
                                                    CoreStatics.CoreApplication.Trasferimento = null;
                                                    CoreStatics.CoreApplication.Cartella = null;
                                                    this.CloseAllFormNM();
                                                }
                                                else
                                                {

                                                    if (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossierStorico.CodStatoConsenso == EnumStatoConsenso.ND.ToString())
                                                    {
                                                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.RichiestaConsenso) == DialogResult.OK)
                                                        {
                                                            CoreStatics.CoreApplication.Paziente = new Paziente(CoreStatics.CoreApplication.Paziente.ID, CoreStatics.CoreApplication.Episodio.ID);

                                                            UnicodeSrl.Scci.Statics.CommonStatics.UpdateConsensiDaSAC(CoreStatics.CoreApplication.Paziente.IDPazienteFuso, CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.Consensi, CoreStatics.CoreApplication.Ambiente);

                                                            CoreStatics.CoreApplication.Paziente = new Paziente(CoreStatics.CoreApplication.Paziente.ID, CoreStatics.CoreApplication.Episodio.ID);
                                                        }
                                                    }


                                                    if (!CoreStatics.CaricaCartellaPaziente(((DataRowView)oUgr.ListObject).Row, mascheraprovenienza))
                                                    {
                                                        CoreStatics.CoreApplication.Paziente = null;
                                                        CoreStatics.CoreApplication.Episodio = null;
                                                        CoreStatics.CoreApplication.Trasferimento = null;
                                                        CoreStatics.CoreApplication.Cartella = null;
                                                        this.CloseAllFormNM();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!CoreStatics.CaricaCartellaPaziente(((DataRowView)oUgr.ListObject).Row, mascheraprovenienza))
                                                {
                                                    CoreStatics.CoreApplication.Paziente = null;
                                                    CoreStatics.CoreApplication.Episodio = null;
                                                    CoreStatics.CoreApplication.Trasferimento = null;
                                                    CoreStatics.CoreApplication.Cartella = null;
                                                    this.CloseAllFormNM();
                                                }
                                            }
                                        }
                                        break;

                                }
                                break;

                            case EnumMaschere.CartelleInVisione:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteAvantiBottom:
                                        UltraGridRow oUgr = ((ucCartelleInVisioneMenu)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle).RigaPazienteSelezionato;
                                        if (oUgr != null)
                                        {
                                            if (oUgr.Cells.Exists("IDCartella"))
                                            {
                                                CoreStatics.CoreApplication.Paziente = new Paziente("", oUgr.Cells["IDEpisodio"].Text);
                                                CoreStatics.CoreApplication.Episodio = new Episodio(oUgr.Cells["IDEpisodio"].Text);
                                                CoreStatics.CoreApplication.Trasferimento = new Trasferimento(oUgr.Cells["IDTrasferimento"].Text, CoreStatics.CoreApplication.Ambiente);
                                                if (oUgr.Cells["IDCartella"].Text != "")
                                                {
                                                    CoreStatics.CoreApplication.Cartella = new Cartella(oUgr.Cells["IDCartella"].Text, oUgr.Cells["NumeroCartella"].Text, CoreStatics.CoreApplication.Ambiente);
                                                }
                                                this.TornaACartellaInVisioneAbilitato = true;
                                                if (!CoreStatics.CaricaCartellaPaziente(((DataRowView)oUgr.ListObject).Row, mascheraprovenienza))
                                                {
                                                    CoreStatics.CoreApplication.Paziente = null;
                                                    CoreStatics.CoreApplication.Episodio = null;
                                                    CoreStatics.CoreApplication.Trasferimento = null;
                                                    CoreStatics.CoreApplication.Cartella = null;
                                                    this.CloseAllFormNM();
                                                }
                                            }
                                            else
                                            {
                                                PazienteSac oPazienteSac = DBUtils.get_RicercaPazientiSACByID(oUgr.Cells["CodSAC"].Text);
                                                CoreStatics.CoreApplication.Paziente = new Paziente(oPazienteSac, CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata, CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata);
                                                CoreStatics.CoreApplication.Episodio = null;
                                                CoreStatics.CoreApplication.Trasferimento = null;
                                                CoreStatics.CoreApplication.Cartella = null;

                                                CoreStatics.CoreApplication.Paziente.AggiungiRecenti();

                                                this.TornaACartellaInVisioneAbilitato = true;
                                                this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                                this.RimuoviMaschera(mascheraprovenienza);
                                            }
                                        }
                                        break;

                                }
                                break;

                            case EnumMaschere.Ambulatoriale_RicercaPaziente:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteAvantiBottom:
                                        PazienteSac oPazienteSac = ((ucRicercaSAC)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle).RigaPazienteSelezionato;
                                        if (oPazienteSac != null)
                                        {
                                            CoreStatics.CoreApplication.Paziente = new Paziente(oPazienteSac, CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata, CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata);

                                            bool bSACConsensiAbilita = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.SACConsensiAbilita)));
                                            if (bSACConsensiAbilita)
                                            {


                                                if (CoreStatics.CoreApplication.Paziente.CodStatoConsensoCalcolato == EnumStatoConsensoCalcolato.NO.ToString())
                                                {
                                                    easyStatics.EasyMessageBox("Il paziente ha NEGATO il consenso, impossibile procedere.", "Consenso Paziente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                                    CoreStatics.CoreApplication.Paziente = null;
                                                    CoreStatics.CoreApplication.Episodio = null;
                                                    CoreStatics.CoreApplication.Trasferimento = null;
                                                    CoreStatics.CoreApplication.Cartella = null;
                                                    this.CloseAllFormNM();
                                                }
                                                else
                                                {

                                                    if (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossierStorico.CodStatoConsenso == EnumStatoConsenso.ND.ToString())
                                                    {
                                                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.RichiestaConsenso) == DialogResult.OK)
                                                        {

                                                            CoreStatics.CoreApplication.Paziente = new Paziente(oPazienteSac, CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata, CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata);
                                                            if (CoreStatics.CoreApplication.Paziente.PazientiConsensiGenerico.CodStatoConsenso == EnumStatoConsenso.SI.ToString())
                                                            {
                                                                CoreStatics.CoreApplication.Episodio = null;
                                                                CoreStatics.CoreApplication.Trasferimento = null;
                                                                CoreStatics.CoreApplication.Cartella = null;

                                                                CoreStatics.CoreApplication.Paziente.AggiungiRecenti();

                                                                this.TornaARicercaAbilitato = true;

                                                                this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                                                this.RimuoviMaschera(mascheraprovenienza);
                                                            }
                                                        }
                                                    }

                                                }

                                                CoreStatics.CoreApplication.Episodio = null;
                                                CoreStatics.CoreApplication.Trasferimento = null;
                                                CoreStatics.CoreApplication.Cartella = null;

                                                CoreStatics.CoreApplication.Paziente.AggiungiRecenti();

                                                this.TornaARicercaAbilitato = true;

                                                this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                                this.RimuoviMaschera(mascheraprovenienza);
                                            }
                                            else
                                            {

                                                CoreStatics.CoreApplication.Episodio = null;
                                                CoreStatics.CoreApplication.Trasferimento = null;
                                                CoreStatics.CoreApplication.Cartella = null;

                                                CoreStatics.CoreApplication.Paziente.AggiungiRecenti();

                                                this.TornaARicercaAbilitato = true;

                                                this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                                this.RimuoviMaschera(mascheraprovenienza);
                                            }
                                        }
                                        break;
                                }
                                break;

                            case EnumMaschere.PercorsoAmbulatoriale_RicercaPaziente:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteAvantiBottom:
                                        UltraGridRow oPaziente = ((ucPercorsoAmbulatoriale)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle).RigaPazienteSelezionato;
                                        if (oPaziente != null)
                                        {

                                            CoreStatics.CoreApplication.Paziente = new Paziente(oPaziente.Cells["IDPaziente"].Text, oPaziente.Cells["CodUA"].Text, oPaziente.Cells["DescrUA"].Text);

                                            bool bSACConsensiAbilita = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.SACConsensiAbilita)));
                                            if (bSACConsensiAbilita)
                                            {


                                                if (CoreStatics.CoreApplication.Paziente.CodStatoConsensoCalcolato == EnumStatoConsensoCalcolato.NO.ToString())
                                                {
                                                    easyStatics.EasyMessageBox("Il paziente ha NEGATO il consenso, impossibile procedere.", "Consenso Paziente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                                    CoreStatics.CoreApplication.Paziente = null;
                                                    CoreStatics.CoreApplication.Episodio = null;
                                                    CoreStatics.CoreApplication.Trasferimento = null;
                                                    CoreStatics.CoreApplication.Cartella = null;
                                                    this.CloseAllFormNM();
                                                }
                                                else
                                                {

                                                    if (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossierStorico.CodStatoConsenso == EnumStatoConsenso.ND.ToString())
                                                    {
                                                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.RichiestaConsenso) == DialogResult.OK)
                                                        {

                                                            CoreStatics.CoreApplication.Paziente = new Paziente(oPaziente.Cells["IDPaziente"].Text, oPaziente.Cells["CodUA"].Text, oPaziente.Cells["DescrUA"].Text);
                                                            if (CoreStatics.CoreApplication.Paziente.PazientiConsensiGenerico.CodStatoConsenso == EnumStatoConsenso.SI.ToString())
                                                            {
                                                                CoreStatics.CoreApplication.Episodio = null;
                                                                CoreStatics.CoreApplication.Trasferimento = null;
                                                                CoreStatics.CoreApplication.Cartella = null;

                                                                this.TornaAPercorsoAmbulatoriale_RicercaPaziente = true;

                                                                this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                                                this.RimuoviMaschera(mascheraprovenienza);
                                                            }
                                                        }
                                                    }

                                                }

                                                CoreStatics.CoreApplication.Episodio = null;
                                                CoreStatics.CoreApplication.Trasferimento = null;
                                                CoreStatics.CoreApplication.Cartella = null;

                                                this.TornaAPercorsoAmbulatoriale_RicercaPaziente = true;

                                                this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                                this.RimuoviMaschera(mascheraprovenienza);
                                            }
                                            else
                                            {

                                                CoreStatics.CoreApplication.Episodio = null;
                                                CoreStatics.CoreApplication.Trasferimento = null;
                                                CoreStatics.CoreApplication.Cartella = null;

                                                this.TornaAPercorsoAmbulatoriale_RicercaPaziente = true;

                                                this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                                this.RimuoviMaschera(mascheraprovenienza);
                                            }
                                        }
                                        break;

                                }
                                break;

                            case EnumMaschere.MatHome_RicercaPaziente:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteAvantiBottom:
                                        PazienteSac oPazienteSac = ((ucRicercaSAC)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle).RigaPazienteSelezionato;
                                        if (oPazienteSac != null)
                                        {
                                            CoreStatics.CoreApplication.Paziente = new Paziente(oPazienteSac, "", "");
                                            CoreStatics.CoreApplication.Episodio = null;
                                            CoreStatics.CoreApplication.Trasferimento = null;
                                            CoreStatics.CoreApplication.Cartella = null;

                                            CoreStatics.CoreApplication.Paziente.AggiungiRecenti();

                                            this.TornaARicercaAbilitato = true;

                                            CoreStatics.CoreApplication.MH_LoginSelezionato = new MH_Login(CoreStatics.CoreApplication.Paziente.ID, EnumAzioni.MOD);
                                            bool bAvanti = true;
                                            if (CoreStatics.CoreApplication.MH_LoginSelezionato.Esiste == false)
                                            {
                                                if (easyStatics.EasyMessageBox("Confermi la creazione dell'account del paziente selezionato?", "Account", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                                {
                                                    bAvanti = false;
                                                }
                                                else
                                                {
                                                    CoreStatics.CoreApplication.MH_LoginSelezionato = new MH_Login(CoreStatics.CoreApplication.Paziente.ID, EnumAzioni.INS);
                                                    CoreStatics.CoreApplication.MH_LoginSelezionato.Salva();
                                                    CoreStatics.CoreApplication.MH_LoginSelezionato.Azione = EnumAzioni.MOD;
                                                }
                                            }
                                            if (bAvanti)
                                            {
                                                this.CaricaMaschera(EnumMaschere.MatHome_GestioneAccount);
                                                CoreStatics.CoreApplication.Paziente = null;
                                                CoreStatics.CoreApplication.Episodio = null;
                                                CoreStatics.CoreApplication.Trasferimento = null;
                                                CoreStatics.CoreApplication.Cartella = null;
                                                this.CloseAllFormNM();
                                            }
                                            CoreStatics.CoreApplication.MH_LoginSelezionato = null;
                                        }
                                        break;

                                }
                                break;

                            case EnumMaschere.PreTrasferimento_RicercaPaziente:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.PreTrasferimentoUACodiceSelezionata = string.Empty;
                                        CoreStatics.CoreApplication.PreTrasferimentoUADescrizioneSelezionata = string.Empty;
                                        CoreStatics.CoreApplication.PreTrasferimentoUOCodiceSelezionata = string.Empty;
                                        CoreStatics.CoreApplication.PreTrasferimentoUODescrizioneSelezionata = string.Empty;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.PreTrasferimentoUACodiceSelezionata = string.Empty;
                                        CoreStatics.CoreApplication.PreTrasferimentoUADescrizioneSelezionata = string.Empty;
                                        CoreStatics.CoreApplication.PreTrasferimentoUOCodiceSelezionata = string.Empty;
                                        CoreStatics.CoreApplication.PreTrasferimentoUODescrizioneSelezionata = string.Empty;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteStampeBottom:
                                        break;

                                    case EnumPulsante.PulsanteAvantiBottom:
                                        CoreStatics.CoreApplication.Consulenza = null;
                                        UltraGridRow oUgr = ((ucRicercaPreTrasferimenti)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle).RigaPazienteSelezionato;
                                        if (oUgr != null)
                                        {
                                            string sIDTrasferimento = CoreStatics.CreaPreTraferimento(((DataRowView)oUgr.ListObject).Row);
                                            if (sIDTrasferimento != string.Empty)
                                            {
                                                CoreStatics.CoreApplication.Paziente = new Paziente("", oUgr.Cells["IDEpisodio"].Text);
                                                CoreStatics.CoreApplication.Episodio = new Episodio(oUgr.Cells["IDEpisodio"].Text);
                                                CoreStatics.CoreApplication.Trasferimento = new Trasferimento(sIDTrasferimento, CoreStatics.CoreApplication.Ambiente);
                                                CoreStatics.CoreApplication.Cartella = null;

                                                if (!CoreStatics.CaricaCartellaPaziente(EnumStatoTrasferimento.PT.ToString(), EnumStatoCartella.DA.ToString(), sIDTrasferimento, CoreStatics.GetEnumDescription(EnumStatoTrasferimento.PT), mascheraprovenienza))
                                                {
                                                    CoreStatics.CoreApplication.Paziente = null;
                                                    CoreStatics.CoreApplication.Episodio = null;
                                                    CoreStatics.CoreApplication.Trasferimento = null;
                                                    CoreStatics.CoreApplication.Cartella = null;
                                                    this.CloseAllFormNM();
                                                }
                                            }
                                        }
                                        break;

                                }
                                break;

                            case EnumMaschere.ChiusuraCartelle:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                }
                                break;

                            case EnumMaschere.FirmaCartelleAperte:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                }
                                break;

                            case EnumMaschere.Consegne:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                }
                                break;

                            case EnumMaschere.ConsegnePaziente:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.StampaCartelleChiuse:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.CartellaPaziente:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteAvantiBottom:
                                        break;

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        if (CoreStatics.CoreApplication.Sessione.Nosologico == string.Empty && CoreStatics.CoreApplication.Sessione.ListaAttesa == string.Empty)
                                        {
                                            CoreStatics.CoreApplication.Paziente = null;
                                            CoreStatics.CoreApplication.Episodio = null;
                                            CoreStatics.CoreApplication.Trasferimento = null;
                                            CoreStatics.CoreApplication.Cartella = null;
                                            this.CloseAllFormNM();
                                            if (this.TornaACartellaInVisioneAbilitato)
                                            {
                                                this.CaricaMaschera(EnumMaschere.CartelleInVisione);
                                            }
                                            else
                                            {
                                                this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                            }
                                            this.RimuoviMaschera(mascheraprovenienza);
                                        }
                                        else
                                        {
                                            if (CoreStatics.CheckEsci() == true)
                                            {
                                                CoreStatics.CoreApplication.Navigazione.Maschere.TracciaNavigazione("Chiusura Programma da Nosologico.", true);
                                                CoreStatics.CoreApplicationContext.Exit();
                                                System.Environment.Exit(0);
                                            }
                                        }
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        if (this.TornaACartellaInVisioneAbilitato)
                                        {
                                            this.CaricaMaschera(EnumMaschere.CartelleInVisione);
                                        }
                                        else
                                        {
                                            this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        }
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.Ambulatoriale_Cartella:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteAvantiBottom:
                                        break;

                                    case EnumPulsante.PulsanteIndietroBottom:

                                        if (CoreStatics.CoreApplication.Sessione.CodiceSACApertura == string.Empty)
                                        {
                                            CoreStatics.CoreApplication.Paziente = null;
                                            CoreStatics.CoreApplication.Episodio = null;
                                            CoreStatics.CoreApplication.Trasferimento = null;
                                            CoreStatics.CoreApplication.Cartella = null;
                                            this.CloseAllFormNM();
                                            if (this.TornaACartellaInVisioneAbilitato)
                                            {
                                                this.CaricaMaschera(EnumMaschere.CartelleInVisione);
                                            }
                                            else
                                            {
                                                this.CaricaMaschera(EnumMaschere.Ambulatoriale_RicercaPaziente);
                                            }
                                            this.RimuoviMaschera(mascheraprovenienza);
                                        }
                                        else
                                        {
                                            if (CoreStatics.CheckEsci() == true)
                                            {
                                                CoreStatics.CoreApplication.Navigazione.Maschere.TracciaNavigazione("Chiusura Programma da Paziente SAC.", true);
                                                CoreStatics.CoreApplicationContext.Exit();
                                                System.Environment.Exit(0);
                                            }
                                        }


                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        if (this.TornaAPercorsoAmbulatoriale_RicercaPaziente)
                                        {
                                            this.CaricaMaschera(EnumMaschere.PercorsoAmbulatoriale_RicercaPaziente);
                                        }
                                        else
                                        {
                                            this.CaricaMaschera(EnumMaschere.Ambulatoriale_RicercaPaziente);
                                        }
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;


                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.Schede:
                            case EnumMaschere.AgendePaziente:
                            case EnumMaschere.FoglioUnico:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.IDSchedaSelezionata = "";
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.IDSchedaSelezionata = "";
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteCartellaPazienteBottom:
                                        CoreStatics.CoreApplication.IDSchedaSelezionata = "";
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.IDSchedaSelezionata = "";
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.Ambulatoriale_Schede:
                            case EnumMaschere.Ambulatoriale_AgendePaziente:
                            case EnumMaschere.Ambulatoriale_GestioneOrdini:
                            case EnumMaschere.Ambulatoriale_EBM:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        if (!CoreStatics.CoreApplication.Sessione.UscitaDitetta)
                                        {
                                            this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                            this.RimuoviMaschera(mascheraprovenienza);
                                        }
                                        else
                                        {
                                            if (CoreStatics.CheckEsci() == true)
                                            {
                                                CoreStatics.CoreApplication.Navigazione.Maschere.TracciaNavigazione("Chiusura Programma da Schede.", true);
                                                CoreStatics.CoreApplicationContext.Exit();
                                                System.Environment.Exit(0);
                                            }
                                        }
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        if (this.TornaAPercorsoAmbulatoriale_RicercaPaziente)
                                        {
                                            this.CaricaMaschera(EnumMaschere.PercorsoAmbulatoriale_RicercaPaziente);
                                        }
                                        else
                                        {
                                            this.CaricaMaschera(EnumMaschere.Ambulatoriale_RicercaPaziente);
                                        }
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteCartellaPazienteBottom:
                                        this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.DiarioClinico:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteCartellaPazienteBottom:
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.ParametriVitali:
                            case EnumMaschere.GestioneOrdini:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteCartellaPazienteBottom:
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.EvidenzaClinica:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteCartellaPazienteBottom:
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.Allegati:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteCartellaPazienteBottom:
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.Ambulatoriale_EvidenzaClinica:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        if (this.TornaAPercorsoAmbulatoriale_RicercaPaziente)
                                        {
                                            this.CaricaMaschera(EnumMaschere.PercorsoAmbulatoriale_RicercaPaziente);
                                        }
                                        else
                                        {
                                            this.CaricaMaschera(EnumMaschere.Ambulatoriale_RicercaPaziente);
                                        }
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteCartellaPazienteBottom:
                                        this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.Ambulatoriale_Allegati:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        if (this.TornaAPercorsoAmbulatoriale_RicercaPaziente)
                                        {
                                            this.CaricaMaschera(EnumMaschere.PercorsoAmbulatoriale_RicercaPaziente);
                                        }
                                        else
                                        {
                                            this.CaricaMaschera(EnumMaschere.Ambulatoriale_RicercaPaziente);
                                        }
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteCartellaPazienteBottom:
                                        this.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.WorklistInfermieristica:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato = "";
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato = "";
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteCartellaPazienteBottom:
                                        CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato = "";
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.IDTaskInfermieristicoSelezionato = "";
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            case EnumMaschere.TerapieFarmacologiche:
                            case EnumMaschere.EBM:
                                switch (pulsante)
                                {

                                    case EnumPulsante.PulsanteIndietroBottom:
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteElencoPazientiBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteCartellaPazienteBottom:
                                        this.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteHomeBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.MenuPrincipale);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                    case EnumPulsante.PulsanteChiusuraCartelleBottom:
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                                        this.CloseAllFormNM();
                                        this.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
                                        this.RimuoviMaschera(mascheraprovenienza);
                                        break;

                                }
                                break;

                            default:
                                break;

                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }
        public void NavigaMaschera(EnumMaschere mascheraprovenienza, EnumPulsante pulsante, EnumMaschere mascheradestinazione)
        {

            try
            {
                if (mascheraprovenienza != mascheradestinazione)
                {

                    if (mascheraprovenienza == EnumMaschere.WorklistInfermieristica && mascheradestinazione != EnumMaschere.TerapieFarmacologiche &&
                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                    {
                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                    }

                    switch (mascheraprovenienza)
                    {

                        case EnumMaschere.EBM:
                        case EnumMaschere.GestioneOrdini:
                        case EnumMaschere.LetteraDiDimissione:
                        case EnumMaschere.EvidenzaClinica:
                        case EnumMaschere.Schede:
                        case EnumMaschere.ParametriVitali:
                        case EnumMaschere.WorklistInfermieristica:
                        case EnumMaschere.TerapieFarmacologiche:
                        case EnumMaschere.AgendePaziente:
                        case EnumMaschere.Allegati:
                        case EnumMaschere.DiarioClinico:
                        case EnumMaschere.FoglioUnico:
                        case EnumMaschere.CartellaPaziente:

                            if (mascheraprovenienza == EnumMaschere.CartellaPaziente) this.TornaACartellaAbilitato = true;


                            switch (mascheradestinazione)
                            {
                                case EnumMaschere.Consulenze_Login:
                                    CoreStatics.CoreApplication.Consulenza = new Consulenza(CoreStatics.CoreApplication.Episodio.ID, CoreStatics.CoreApplication.Trasferimento.ID, CoreStatics.CoreApplication.Paziente.ID);
                                    if (CoreStatics.CoreApplication.Consulenza.InserisciConsulenza())
                                    {
                                    }
                                    break;

                                case EnumMaschere.LetteraDiDimissione:
                                case EnumMaschere.Mow:
                                case EnumMaschere.Psc:
                                    this.CaricaMaschera(mascheradestinazione);
                                    break;

                                default:
                                    this.CaricaMaschera(mascheradestinazione);
                                    this.RimuoviMaschera(mascheraprovenienza);
                                    break;
                            }

                            break;


                        case EnumMaschere.Ambulatoriale_EBM:
                        case EnumMaschere.Ambulatoriale_Schede:
                        case EnumMaschere.Ambulatoriale_AgendePaziente:
                        case EnumMaschere.Ambulatoriale_GestioneOrdini:
                        case EnumMaschere.Ambulatoriale_Allegati:
                        case EnumMaschere.Ambulatoriale_EvidenzaClinica:
                        case EnumMaschere.Ambulatoriale_Cartella:

                            if (mascheraprovenienza == EnumMaschere.Ambulatoriale_Cartella) this.TornaACartellaAbilitato = true;

                            switch (mascheradestinazione)
                            {
                                case EnumMaschere.Ambulatoriale_Mow:
                                    this.CaricaMaschera(mascheradestinazione);
                                    break;

                                default:
                                    this.CaricaMaschera(mascheradestinazione);
                                    this.RimuoviMaschera(mascheraprovenienza);
                                    break;
                            }

                            break;

                        default:
                            break;

                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public void RimuoviMaschera(EnumMaschere maschera)
        {

            /* 
             * Rimuove UserControl dalla cache
             */
            try
            {





                if (this.Elementi.Any(Maschera => Maschera.ID == maschera))
                {
                    var oItem = this.Elementi.Single(Maschera => Maschera.ID == maschera);
                    oItem.ControlMiddle.Ferma();
                    if (!oItem.InCache)
                    {
                        this.Elementi.Remove(oItem);
                        ((UserControl)oItem.ControlMiddle).Dispose();
                    }
                }

                if (this.MascheraSelezionata.CambioPercorso == true)
                {
                    this.RimuoviMaschereDaPersorso();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public void RimuoviMaschereMassimizzabili()
        {
            CoreStatics.CoreApplication.Navigazione.Maschere.RimuoviMaschera(EnumMaschere.FoglioUnico);
            CoreStatics.CoreApplication.Navigazione.Maschere.RimuoviMaschera(EnumMaschere.TerapieFarmacologiche);
        }

        public void Reset()
        {

            try
            {

                var utc = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).UltraTabControlMenu;

                foreach (UltraTab oUt in utc.Tabs)
                {
                    foreach (Control ctrl in oUt.TabPage.Controls)
                    {

                        if (ctrl is Interfacce.IViewUserControlMiddle)
                        {
                            oUt.TabPage.Controls.Remove(ctrl);
                            ctrl.Dispose();
                            break;
                        }
                    }

                }

                utc.ResetTabs();

                CoreStatics.CoreApplication.Navigazione = new Navigazione();
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.MenuPrincipale);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }


        }

        private void ResetUltraTabControl()
        {

            try
            {

                var utc = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).UltraTabControlMenu;

                foreach (UltraTab oUt in utc.Tabs)
                {
                    foreach (Control ctrl in oUt.TabPage.Controls)
                    {

                        if (ctrl is Interfacce.IViewUserControlMiddle)
                        {
                            oUt.TabPage.Controls.Remove(ctrl);
                        }
                    }

                }

                utc.ResetTabs();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void RimuoviMaschereDaPersorso()
        {

            /* 
            * Rimuove UserControl di Percorso dalla cache
            */

            bool bfound = true;

            try
            {

                while (bfound == true)
                {

                    bfound = false;
                    foreach (Maschera oItem in this.Elementi)
                    {

                        if (oItem.InCacheDaPercorso == true)
                        {
                            oItem.ControlMiddle.Ferma();
                            this.Elementi.Remove(oItem);
                            ((UserControl)oItem.ControlMiddle).Dispose();
                            bfound = true;
                            break;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region         Maschera Spex

        protected Maschera ActionCreateMaschera(Func<string, string, Maschera> ptrAction, string codRuolo, string codUA)
        {
            try
            {
                return ptrAction(codRuolo, codUA);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                return null;
            }
            finally
            {
            }
        }

        #endregion      Maschera Spex

        #region     Action Creazione Maschera

        private Maschera cf_CartellaPaziente(string codRuolo, string codUA)
        {
            Maschera oMaschera = null;

            FwDataBufferedList<T_ScreenRow> rowsScreen = null;

            using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
            {
                rowsScreen = conn.MSP_SelScreen(codUA, codRuolo, en_TipoScreen.EPICAR);
            }

            if (rowsScreen.Buffer.Count > 0)
            {

                T_ScreenRow screenObj = null;

                ConfigUtente cfg = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente;

                if (String.IsNullOrEmpty(cfg.CodScreenSelezionato) == false)
                {
                    screenObj = rowsScreen.Buffer.FirstOrDefault(sc => sc.Codice == cfg.CodScreenSelezionato);
                }

                if (screenObj == null) screenObj = rowsScreen.Buffer.First();

                cfg.CodScreenSelezionato = screenObj.Codice;

                if (cfg.CodScreenSelezionato != screenObj.Codice)
                    CoreStatics.CoreApplication.Sessione.Utente.SalvaConfigUtente();

                ucScreenView cScreen = new ucScreenView(codUA, codRuolo, screenObj);
                cScreen.Dock = DockStyle.Fill;

                oMaschera = new Maschera(EnumMaschere.CartellaPaziente, cScreen, "1-1S", codRuolo, codUA);

            }
            else
            {
                ucCartellaPaziente oCartellaPaziente = new ucCartellaPaziente();
                oCartellaPaziente.Dock = DockStyle.Fill;

                oMaschera = new Maschera(EnumMaschere.CartellaPaziente, oCartellaPaziente, "1-1", codRuolo, codUA);
            }

            oMaschera.Indietro = true;
            oMaschera.Home = true;
            oMaschera.Avanti = true;

            return oMaschera;
        }

        private Maschera cf_ParametriVitali(string codRuolo, string codUA)
        {
            Maschera oMaschera = null;

            ucParametriVitali oParametriVitali = new ucParametriVitali();
            oParametriVitali.Dock = DockStyle.Fill;
            oMaschera = new Maschera(EnumMaschere.ParametriVitali, oParametriVitali, "1-1-3", codRuolo, codUA);

            oMaschera.Indietro = true;
            oMaschera.Home = true;
            oMaschera.Avanti = true;

            return oMaschera;
        }

        #endregion  Action Creazione Maschera

    }
}
