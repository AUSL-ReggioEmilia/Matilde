using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinSchedule;
using UnicodeSrl.ScciResource;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.IPC;
using UnicodeSrl.Scci.PluginClient;
using System.Diagnostics;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.ScciCore.WebSvc;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.ScciCore.Common.TimersCB;
using UnicodeSrl.ScciCore.ViewController;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.RTFLibrary.Core;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Framework.Collections;
using System.DirectoryServices;
using UnicodeSrl.Framework.Diagnostics;

namespace UnicodeSrl.ScciCore
{

    public class CoreApplicationContext : ApplicationContext
    {

        public CoreApplicationContext()
        {
            CoreStatics.MainWnd.StartTimers();
        }

        public void Exit()
        {
            CoreStatics.CoreApplication.Listener.ChiudiListener();
            CoreStatics.CoreApplication.Sessione.Utente.SalvaConfigUtente();
            CoreStatics.CoreApplication.Sessione.Utente.SvuotaTemp();
            CoreStatics.MainWnd.StopTimers();
            Application.ExitThread();
        }
    }

    [Serializable()]
    public class CoreApplication
    {

        private Sessione _sessione = null;
        private Paziente _paziente = null;
        private Episodio _episodio = null;
        private Trasferimento _trasferimento = null;
        private Cartella _cartella = null;
        private CartellaAmbulatoriale _cartellaambulatoriale = null;

        private ImportaDWHFiltro _importaDWHFiltroSessione = null;

        public CoreApplication()
        {
            this.Ambiente = new Scci.DataContracts.ScciAmbiente();
            this.Listener = null;
            this.Sessione = new Sessione(this.Ambiente);
            this.Navigazione = new Navigazione();
            this.News = new Notizie();
            this.ListaIDMovDiarioClinicoSelezionati = new List<string>();
            this.ParametriVitaliTrasversali = new ParametriVitaliTrasversali();
            this.Schermo = Screen.PrimaryScreen;

            this.MovDiarioClinicoSelezionato = null;
            this.MovDiarioClinicoDaAnnullare = null;
            this.MovParametroVitaleSelezionato = null;
            this.MovEvidenzaClinicaSelezionato = null;
            this.MovAlertAllergieAnamnesiSelezionato = null;
            this.MovAlertGenericoSelezionato = null;
            this.MovSchedaSelezionata = null;
            this.DefinizioneGraficoSelezionata = null;
            this.MovTaskInfermieristicoSelezionato = null;
            this.MovProtocolloAttivitaSelezionato = null;
            this.MovAppuntamentoSelezionato = null;
            this.MovNoteAgendeSelezionata = null;
            this.MovPrescrizioneSelezionata = null;
            this.MovPrescrizioneTempiSelezionata = null;
            this.MovTestoPredefinitoSelezionato = null;
            this.MovAllegatoSelezionato = null;
            this.Consulenza = null;
            this.MovOrdineSelezionato = null;
            this.IDSchedaSelezionata = string.Empty;
            this.ReportSelezionato = null;
            this.IDTaskInfermieristicoSelezionato = string.Empty;
            this.CodUOSelezionata = string.Empty;
            this.AmbulatorialeUACodiceSelezionata = string.Empty;
            this.AmbulatorialeUADescrizioneSelezionata = string.Empty;
            this.PreTrasferimentoUACodiceSelezionata = string.Empty;
            this.PreTrasferimentoUADescrizioneSelezionata = string.Empty;
            this.PreTrasferimentoUOCodiceSelezionata = string.Empty;
            this.PreTrasferimentoUODescrizioneSelezionata = string.Empty;
            this.EpisodioCollegabileSelezionato = null;
            this.TrasferimentoCollegabileSelezionato = null;
            this.CartellaCollegabileSelezionata = null;
            this.MovLinkSelezionato = null;
            this.ConsegneUACodiceSelezionata = string.Empty;
            this.ConsegneUADescrizioneSelezionata = string.Empty;
        }

        public string ConnectionString
        {
            get { return UnicodeSrl.Scci.Statics.Database.ConnectionString; }
            set { UnicodeSrl.Scci.Statics.Database.ConnectionString = value; }
        }

        public Listener Listener { get; set; }

        public Sessione Sessione
        {
            get { return _sessione; }
            set
            {
                _sessione = value;
                if (value != null)
                {
                    if (value.Utente != null) this.Ambiente.Codlogin = value.Utente.Codice;
                    if (value.Utente != null) this.Ambiente.Codruolo = value.Utente.Ruoli.RuoloSelezionato.Codice;
                    if (value.Computer != null) this.Ambiente.Nomepc = value.Computer.Nome;
                    if (value.Computer != null) this.Ambiente.Indirizzoip = value.Computer.Ip;
                }

                if (this.Ambiente.Contesto.ContainsKey("Ambiente") == true)
                {
                    this.Ambiente.Contesto.Remove("Ambiente");
                }
                this.Ambiente.Contesto.Add("Ambiente", this.Ambiente);
            }
        }

        public Navigazione Navigazione { get; set; }

        public Paziente Paziente
        {
            get { return _paziente; }
            set
            {
                _paziente = value;
                if (_paziente != null)
                    this.Ambiente.Idpaziente = _paziente.ID;
                else
                    this.Ambiente.Idpaziente = string.Empty;

                if (this.Ambiente.Contesto.ContainsKey("Paziente") == true)
                {
                    this.Ambiente.Contesto.Remove("Paziente");
                }
                if (_paziente != null) { this.Ambiente.Contesto.Add("Paziente", _paziente); }
            }
        }

        public Episodio Episodio
        {
            get { return _episodio; }
            set
            {
                _episodio = value;
                if (_episodio != null)
                    this.Ambiente.Idepisodio = _episodio.ID;
                else
                    this.Ambiente.Idepisodio = string.Empty;

                if (this.Ambiente.Contesto.ContainsKey("Episodio") == true)
                {
                    this.Ambiente.Contesto.Remove("Episodio");
                }
                if (_episodio != null) { this.Ambiente.Contesto.Add("Episodio", _episodio); }

            }
        }

        public Trasferimento Trasferimento
        {
            get { return _trasferimento; }
            set
            {
                _trasferimento = value;
                if (_trasferimento != null)
                    this.Ambiente.IdTrasferimento = _trasferimento.ID;
                else
                    this.Ambiente.IdTrasferimento = string.Empty;

                if (this.Ambiente.Contesto.ContainsKey("Trasferimento") == true)
                {
                    this.Ambiente.Contesto.Remove("Trasferimento");
                }
                if (_trasferimento != null) { this.Ambiente.Contesto.Add("Trasferimento", _trasferimento); }

            }
        }

        public Cartella Cartella
        {
            get { return _cartella; }
            set
            {
                _cartella = value;
                if (_cartella != null)
                    this.Ambiente.IdCartella = _cartella.ID;
                else
                    this.Ambiente.IdCartella = string.Empty;

                if (this.Ambiente.Contesto.ContainsKey("Cartella") == true)
                {
                    this.Ambiente.Contesto.Remove("Cartella");
                }
                if (_cartella != null) { this.Ambiente.Contesto.Add("Cartella", _cartella); }

            }
        }

        public CartellaAmbulatoriale CartellaAmbulatoriale
        {
            get { return _cartellaambulatoriale; }
            set { _cartellaambulatoriale = value; }
        }

        public string IDSchedaSelezionata { get; set; }

        public string IDTaskInfermieristicoSelezionato { get; set; }

        public string IDDiarioClinicoSelezionato { get; set; }

        public string IDConsegnaSelezionata { get; set; }

        public Report ReportSelezionato { get; set; }

        public Entitas Entitas { get; set; }

        public Notizie News { get; set; }

        public MovDiarioClinico MovDiarioClinicoSelezionato { get; set; }

        public MovDiarioClinico MovDiarioClinicoDaAnnullare { get; set; }

        public MovConsegna MovConsegnaSelezionata { get; set; }

        public MovConsegnaPaziente MovConsegnaPazienteSelezionata { get; set; }

        public MovConsegna MovConsegnaDaAnnullare { get; set; }

        public MovParametroVitale MovParametroVitaleSelezionato { get; set; }

        public MovEvidenzaClinica MovEvidenzaClinicaSelezionato { get; set; }

        public MovAlertAllergieAnamnesi MovAlertAllergieAnamnesiSelezionato { get; set; }

        public MovAlertGenerico MovAlertGenericoSelezionato { get; set; }

        public MovScheda MovSchedaSelezionata { get; set; }
        public List<MovScheda> MovSchedeSelezionate { get; set; }

        public MovSchedaAllegato MovSchedaAllegatoSelezionata { get; set; }

        public ToolboxPerGrafici DefinizioneGraficoSelezionata { get; set; }

        public MovTaskInfermieristico MovTaskInfermieristicoSelezionato { get; set; }

        public MovProtocolloAttivita MovProtocolloAttivitaSelezionato { get; set; }

        public MovAppuntamento MovAppuntamentoSelezionato { get; set; }

        public List<MovAppuntamento> MovAppuntamentiGenerati { get; set; }

        public MovNoteAgende MovNoteAgendeSelezionata { get; set; }

        public MovNota MovNotaSelezionata { get; set; }

        public MovPrescrizione MovPrescrizioneSelezionata { get; set; }

        public MovPrescrizioneTempi MovPrescrizioneTempiSelezionata { get; set; }

        public ProtocolloPrescrizioni MovProtocolloPrescrizioniSelezionato { get; set; }

        public MovLink MovLinkSelezionato { get; set; }

        public List<string> ListaIDMovDiarioClinicoSelezionati { get; set; }

        public ParametriVitaliTrasversali ParametriVitaliTrasversali { get; set; }

        public Scci.DataContracts.ScciAmbiente Ambiente { get; set; }

        public MovTestoPredefinito MovTestoPredefinitoSelezionato { get; set; }

        public MovAllegato MovAllegatoSelezionato { get; set; }

        public Consulenza Consulenza { get; set; }

        public MovOrdine MovOrdineSelezionato { get; set; }

        public MovCartellaInVisione MovCartellaInVisioneSelezionata { get; set; }

        public MovPazienteInVisione MovPazienteInVisioneSelezionato { get; set; }

        public MovPazienteSeguito MovPazienteSeguitoSelezionato { get; set; }

        public List<string> ListaIDMovPrescrizioniCreate { get; set; }

        public string CodUOSelezionata { get; set; }

        public string AmbulatorialeUACodiceSelezionata { get; set; }
        public string AmbulatorialeUADescrizioneSelezionata { get; set; }

        public string PreTrasferimentoUACodiceSelezionata { get; set; }
        public string PreTrasferimentoUADescrizioneSelezionata { get; set; }

        public string PreTrasferimentoUOCodiceSelezionata { get; set; }
        public string PreTrasferimentoUODescrizioneSelezionata { get; set; }

        public Episodio EpisodioCollegabileSelezionato { get; set; }
        public Trasferimento TrasferimentoCollegabileSelezionato { get; set; }
        public Cartella CartellaCollegabileSelezionata { get; set; }

        public string ConsegneUACodiceSelezionata { get; set; }
        public string ConsegneUADescrizioneSelezionata { get; set; }

        public string ConsegnePazienteUACodiceSelezionata { get; set; }
        public string ConsegnePazienteUADescrizioneSelezionata { get; set; }

        [field: NonSerialized()]
        public Screen Schermo;

        [field: NonSerialized()]
        public FilterMess Inattivita;

        public ImportTestiRefertiDWH ImportTestiRefertiDWH { get; set; }

        public ProseguiTerapia ProseguiTerapiaSelezionata { get; set; }

        public ImportaDWHFiltro ImportaDWHFiltroSessione
        {
            get
            {
                if (_importaDWHFiltroSessione == null) _importaDWHFiltroSessione = new ImportaDWHFiltro(DateTime.MinValue, DateTime.MinValue, "");
                return _importaDWHFiltroSessione;
            }
            set { _importaDWHFiltroSessione = value; }
        }

        public MH_Login MH_LoginSelezionato { get; set; }

        internal void AggiornaIndicatori(TimersCB_Controllo_Data data)
        {

            try
            {

                var oSessione = CoreStatics.CoreApplication.Sessione;
                var oPaziente = CoreStatics.CoreApplication.Paziente;
                var oEpisodio = CoreStatics.CoreApplication.Episodio;

                Form f = Application.OpenForms["frmConnettivita"];

                oSessione.Connettivita = data.Connettivita;

                if (oSessione.Connettivita == true)
                {

                    if (f != null)
                    {
                        System.Delegate d = new MethodInvoker(hideFormConnettivita);
                        CoreStatics.CoreApplicationContext.MainForm.Invoke(d);
                    }

                    if (oSessione.Utente.Ruoli.RuoloSelezionato != null)
                    {
                        oSessione.Utente.Ruoli.RuoloSelezionato.DaFirmare = data.DiarioClinico;
                    }

                    if (oPaziente != null && oPaziente.Attivo == true)
                    {
                        oPaziente.Allergie.Numero = data.Allergie;
                    }

                    if (oEpisodio != null && oEpisodio.Attivo == true)
                    {
                        oEpisodio.AlertsGenerici.DaVistare = data.Alert;
                        oEpisodio.EvidenzeCliniche.DaVistare = data.EvidenzaClinica;
                    }

                    if (oSessione.Utente.Ruoli.RuoloSelezionato != null)
                    {
                        oSessione.Utente.Ruoli.RuoloSelezionato.Bookmarks = data.Segnalibri;
                        oSessione.Utente.Ruoli.RuoloSelezionato.CartelleInVisione = data.CartelleInVisione;
                        oSessione.Utente.Ruoli.RuoloSelezionato.PazientiInVisione = data.PazientiInVisione;
                        oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguiti = data.PazientiSeguiti;
                        oSessione.Utente.Ruoli.RuoloSelezionato.PazienteSeguito = data.PazienteSeguito;
                        oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguitiDaAltri = data.PazientiSeguitiDaAltri;
                        oSessione.Utente.Ruoli.RuoloSelezionato.NewsHard = data.NewsHard;
                        oSessione.Utente.Ruoli.RuoloSelezionato.NewsLite = data.NewsLite;
                        oSessione.Utente.Ruoli.RuoloSelezionato.MatHome = (data.MatHome == 0 ? false : true);
                    }

                }
                else
                {
                    if (f == null)
                    {
                        System.Delegate d = new MethodInvoker(showFormConnettivita);
                        CoreStatics.CoreApplicationContext.MainForm.Invoke(d);
                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        public void showFormConnettivita()
        {
            try
            {
                frmConnettivita _frmConnettivita = new frmConnettivita();

                Form frm = _frmConnettivita;
                easyStatics.maximizeForm(ref frm, FormBorderStyle.None);
                frm.WindowState = FormWindowState.Maximized;

                _frmConnettivita.Carica();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        public void hideFormConnettivita()
        {
            Form f = Application.OpenForms["frmConnettivita"];

            if (f != null)
            {
                f.Close();
                f.Dispose();
                f = null;
            }

        }

        internal void AggiornaIndicatoriAmb(DateTime ora)
        {

            try
            {

                var oSessione = CoreStatics.CoreApplication.Sessione;
                oSessione.Ora = ora;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

    }

    [Serializable()]
    public class Listener
    {

        public Listener(string utente, string connectionstring, bool hide)
        {
            this.Utente = utente;
            this.ConnectionString = connectionstring;
            this.Hide = hide;
            this.Processo = null;
        }

        string Utente { get; set; }

        string ConnectionString { get; set; }

        bool Hide { get; set; }

        Process Processo { get; set; }

        public void InitializeProcess()
        {

            try
            {

                this.CloseAllProcess();

                string fileExec = @"ScciListener.exe";
                string argument = string.Format("\"{0}\" \"{1}\" \"{2}\"", this.Utente, this.ConnectionString, (this.Hide == true ? "S" : "N"));
                this.Processo = new Process();
                this.Processo.StartInfo.FileName = fileExec;
                this.Processo.StartInfo.Arguments = argument;

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void CloseAllProcess()
        {

            try
            {

                Process[] localByName = Process.GetProcessesByName("ScciListener");

                if (localByName != null && localByName.Length > 0)
                {

                    for (int i = localByName.Length - 1; i >= 0; i--)
                    {

                        Process p = Process.GetProcessById(localByName[i].Id);
                        if (p != null) { p.Kill(); }

                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public void ApriMaschera(EnumMaschere maschera, IViewController viewcontroller)
        {

            IpcSender wndSender = new IpcSender();

            ViewControllerData data = new ViewControllerData();
            data.Channel = string.Format(CommonConstants.C_SCCI_CHANNEL, this.Utente);

            data.CommandListener = EnumCommandListener.ApriMaschera;
            data.Maschera = maschera;
            data.Sessione = CoreStatics.CoreApplication.Sessione;

            data.ViewController = viewcontroller;

            wndSender.Send(data);

            wndSender = null;

        }

        public void ChiudiMaschere()
        {

            IpcSender wndSender = new IpcSender();

            ViewControllerData data = new ViewControllerData();
            data.Channel = string.Format(CommonConstants.C_SCCI_CHANNEL, this.Utente);

            data.CommandListener = EnumCommandListener.ChiudiMaschere;

            wndSender.Send(data);

            wndSender = null;

        }

        public void ChiudiListener()
        {

            IpcSender wndSender = new IpcSender();

            ViewControllerData data = new ViewControllerData();
            data.Channel = string.Format(CommonConstants.C_SCCI_CHANNEL, this.Utente);

            data.CommandListener = EnumCommandListener.ChiudiListener;

            wndSender.Send(data);

            wndSender = null;

        }

    }

    [Serializable()]
    public class Sessione
    {

        private Utente _utente = null;
        private Computer _computer = null;
        private DateTime _connessoda;
        private bool _connettivita = false;
        private DateTime _ora = DateTime.MinValue;
        private string _nosologico = string.Empty;
        private int _nosologicoindex = 0;
        private bool _sala = false;
        private string _ricercapazienti = string.Empty;

        private Scci.DataContracts.ScciAmbiente _Ambiente = null;

        public Sessione(Scci.DataContracts.ScciAmbiente ambiente)
        {
            _Ambiente = ambiente;
            _computer = new Computer();
            _connessoda = DateTime.Now;
            _connettivita = false;
            _ora = DateTime.MinValue;
            _nosologico = string.Empty;
            _nosologicoindex = 0;
            _sala = false;
            CodiceSACApertura = string.Empty;
            ModuloApertura = null;
            ListaAttesa = string.Empty;
            UscitaDitetta = false;
            NoMsg = false;
        }

        public Utente Utente
        {
            get { return _utente; }
            set
            {
                _utente = value;
                if (_utente != null)
                    _Ambiente.Codlogin = _utente.Codice;
                else
                    _Ambiente.Codlogin = "";
            }
        }

        public Computer Computer
        {
            get { return _computer; }
            set { _computer = value; }
        }

        public DateTime ConnessoDa
        {
            get { return _connessoda; }
            set { _connessoda = value; }
        }

        public bool Connettivita
        {
            get { return _connettivita; }
            set { _connettivita = value; }
        }

        public DateTime Ora
        {
            get { return _ora; }
            set { _ora = value; }
        }

        public string Nosologico
        {
            get { return _nosologico; }
            set { _nosologico = value; }
        }

        public int NosologicoIndex
        {
            get { return _nosologicoindex; }
            set { _nosologicoindex = value; }
        }

        public bool Sala
        {
            get { return _sala; }
            set { _sala = value; }
        }

        public string ListaAttesa { get; set; } = string.Empty;

        public bool UscitaDitetta { get; set; } = false;

        public bool NoMsg { get; set; } = false;

        public string RicercaPazienti
        {
            get { return _ricercapazienti; }
            set { _ricercapazienti = value; }
        }

        public string CodiceSACApertura { get; set; }

        public EnumCommandLineModules? ModuloApertura { get; set; }

    }

    [Serializable()]
    public class Computer
    {
        public Computer()
        {
            this.ConfigPC = Database.GetConfigPCTable(this.Nome);
        }

        internal ConfigPC ConfigPC
        { get; private set; }


        private int _EnableTrace = -1;

        public string Nome
        {
            get
            {
                try
                {
                    if (this.SessioneRemota)
                        return (this.GetTerminalServicesClientName());
                    else
                        return (Environment.MachineName);
                }
                catch (Exception)
                {
                    return @"";
                }
            }
        }

        public string Ip
        {
            get
            {
                try
                {
                    if (this.SessioneRemota)
                        return (this.GetTerminalServicesClientAddress());
                    else
                        return System.Net.Dns.GetHostEntry(this.Nome).AddressList[0].ToString();
                }
                catch (Exception)
                {
                    return @"127.0.0.1";
                }
            }
        }

        public string NomeDominioCompleto
        {
            get
            {
                string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                if (!this.Nome.Contains(domainName))
                    return this.Nome + "." + domainName;
                else
                    return this.Nome;
            }
        }

        public bool SessioneRemota
        {
            get { return System.Windows.Forms.SystemInformation.TerminalServerSession; }
        }

        public bool IsOSServer
        {
            get { return NativeMethods.IsOS(NativeMethods.OS_ANYSERVER); }
        }

        public bool EnableTrace
        {
            get
            {
                try
                {
                    if (_EnableTrace < 0)
                    {
                        if (this.IsOSServer) _EnableTrace = 0;
                    }

                    if (_EnableTrace < 0)
                    {
                        ConfigPC oTmp = Database.GetConfigPCTable(this.Nome);
                        if (oTmp != null && oTmp.configDebug != null)
                        {
                            if (oTmp.configDebug.EnableTrace)
                                _EnableTrace = 1;
                            else
                                _EnableTrace = 0;
                        }
                    }
                }
                catch
                {
                }
                return (_EnableTrace == 1);
            }
        }

        public float RtfZoom
        {
            get
            {
                ConfigPC oTmp = this.ConfigPC;
                if (oTmp != null && oTmp.configRtf != null)
                {
                    return oTmp.configRtf.Zoom;
                }
                else
                {
                    return 1F;
                }
            }
        }

        public float FontCoefficienteConfigPC
        {
            get
            {
                ConfigPC oTmp = this.ConfigPC;
                if (oTmp != null && oTmp.configFont != null)
                {
                    return oTmp.configFont.Coefficiente;
                }
                else
                {
                    return 1F;
                }
            }
        }

        public string AgendaChiamataNumeri
        {
            get
            {
                ConfigPC oTmp = this.ConfigPC;
                if (oTmp != null && oTmp.configChiamataNumeri != null)
                {
                    return oTmp.configChiamataNumeri.CodiceAgenda;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ConfigPC oTmp = this.ConfigPC;
                if (oTmp == null) oTmp = new ConfigPC();
                if (oTmp.configChiamataNumeri == null) oTmp.configChiamataNumeri = new ConfigChiamataNumeri();

                oTmp.configChiamataNumeri.CodiceAgenda = value;

                Database.SetConfigPCTable(this.Nome, oTmp);
            }
        }

        public bool ApriCartellaSuChiamataNumero
        {
            get
            {
                ConfigPC oTmp = this.ConfigPC;
                if (oTmp != null && oTmp.configChiamataNumeri != null)
                {
                    return oTmp.configChiamataNumeri.ApriCartellaSuChiamata;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                ConfigPC oTmp = this.ConfigPC;
                if (oTmp == null) oTmp = new ConfigPC();
                if (oTmp.configChiamataNumeri == null) oTmp.configChiamataNumeri = new ConfigChiamataNumeri();

                oTmp.configChiamataNumeri.ApriCartellaSuChiamata = value;

                Database.SetConfigPCTable(this.Nome, oTmp);
            }
        }

        public bool SessioneIpovedente
        {
            get
            {
                ConfigPC oTmp = this.ConfigPC;
                if (oTmp != null && oTmp.configIpovedente != null)
                {
                    return oTmp.configIpovedente.Ipovedente;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                ConfigPC oTmp = this.ConfigPC;
                if (oTmp == null) oTmp = new ConfigPC();
                if (oTmp.configIpovedente == null) oTmp.configIpovedente = new ConfigIpovedente();

                oTmp.configIpovedente.Ipovedente = value;

                Database.SetConfigPCTable(this.Nome, oTmp);
            }
        }

        private string GetTerminalServicesClientName()
        {
            IntPtr buffer = IntPtr.Zero;

            string clientName = null;
            int bytesReturned;

            bool success = NativeMethods.WTSQuerySessionInformation(
                NativeMethods.WTS_CURRENT_SERVER_HANDLE,
                NativeMethods.WTS_CURRENT_SESSION,
                NativeMethods.WTS_INFO_CLASS.WTSClientName,
                out buffer,
                out bytesReturned);

            if (success)
            {
                clientName = System.Runtime.InteropServices.Marshal.PtrToStringUni(
                    buffer,
                    bytesReturned / 2 - 1
                    );
                NativeMethods.WTSFreeMemory(buffer);
            }

            return clientName;
        }

        private string GetTerminalServicesClientAddress()
        {
            IntPtr buffer = IntPtr.Zero;
            int bytesReturned;
            string clientaddress = null;
            NativeMethods.WTS_CLIENT_ADDRESS clientaddressstructure = new NativeMethods.WTS_CLIENT_ADDRESS();

            bool success = NativeMethods.WTSQuerySessionInformation(
                NativeMethods.WTS_CURRENT_SERVER_HANDLE,
                NativeMethods.WTS_CURRENT_SESSION,
                NativeMethods.WTS_INFO_CLASS.WTSClientAddress,
                out buffer,
                out bytesReturned);

            if (success)
            {
                clientaddressstructure = (NativeMethods.WTS_CLIENT_ADDRESS)System.Runtime.InteropServices.Marshal.PtrToStructure(buffer, typeof(NativeMethods.WTS_CLIENT_ADDRESS));
                clientaddress = clientaddressstructure.Address[2] + "." + clientaddressstructure.Address[3] + "." +
                    clientaddressstructure.Address[4] + "." + clientaddressstructure.Address[5];
                NativeMethods.WTSFreeMemory(buffer);
            }

            return clientaddress;
        }

    }

    [Serializable()]
    public class Utente
    {

        private string _codice = String.Empty;
        private string _descrizione = String.Empty;
        private string _cognome = String.Empty;
        private string _nome = String.Empty;
        private string _codicefiscale = String.Empty;
        private bool _admin = false;
        private Image _foto = null;
        private bool _abilitato = false;
        private Ruoli _ruoli = null;
        private ConfigUtente _configutente = new ConfigUtente();

        public Utente(string codlogin)
        {
            this.Carica(codlogin);
            if (_abilitato == true)
            {
                _ruoli = new Ruoli(codlogin);
                if (_configutente.RuoloSelezionato != string.Empty)
                {
                    Ruolo oItem = null;
                    foreach (Ruolo tmp in _ruoli.Elementi)
                    {
                        if (tmp.Codice == _configutente.RuoloSelezionato) oItem = tmp;
                    }
                    if (oItem != null)
                    {
                        _ruoli.RuoloSelezionato = oItem;
                    }
                }
            }
        }

        public string Codice
        {
            get { return _codice; }
            set
            {
                _codice = value;
            }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        public string Cognome
        {
            get { return _cognome; }
            set { _cognome = value; }
        }

        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public string CodiceFiscale
        {
            get { return _codicefiscale; }
            set { _codicefiscale = value; }
        }

        public bool Admin
        {
            get { return _admin; }
            set { _admin = value; }
        }

        public Image Foto
        {
            get { return _foto; }
            set { _foto = value; }
        }

        public bool Abilitato
        {
            get { return _abilitato; }
            set { _abilitato = value; }
        }

        public Ruoli Ruoli
        {
            get { return _ruoli; }
            set { _ruoli = value; }
        }

        public ConfigUtente ConfigUtente
        {
            get { return _configutente; }
            set { _configutente = value; }
        }

        public string UserPrincipalName
        {
            get
            {

                string sret = string.Empty;

                try
                {

                    string sdomain = _codice.Split('\\')[0];
                    string userName = _codice.Split('\\')[1];
                    string ldapPath = string.Format(@"LDAP://{0}", sdomain.ToUpper());
                    string sproperty = @"userprincipalname";

                    using (DirectoryEntry root = new DirectoryEntry(ldapPath))
                    {

                        DirectorySearcher searcher = new DirectorySearcher(root);
                        searcher.Filter = string.Format("(&(objectClass=user) (sAMAccountName={0}))", userName);

                        searcher.PropertiesToLoad.Add(sproperty);

                        SearchResult result = searcher.FindOne();
                        if (result != null)
                        {

                            ResultPropertyCollection fields = result.Properties;

                            sret = fields[sproperty][0].ToString();

                        }

                    }

                }
                catch (Exception)
                {
                }

                return sret;

            }
        }
        private void Carica(string sCodLogin)
        {

            try
            {

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                spcoll[0] = new SqlParameterExt("sCodLogin", sCodLogin, ParameterDirection.Input, SqlDbType.VarChar);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelLogin", spcoll);

                if (dt.Rows.Count != 0)
                {
                    this.Codice = dt.Rows[0]["Codice"].ToString();
                    this.Descrizione = dt.Rows[0]["Descrizione"].ToString();
                    this.Cognome = dt.Rows[0]["Cognome"].ToString();
                    this.Nome = dt.Rows[0]["Nome"].ToString();
                    this.CodiceFiscale = dt.Rows[0]["CodiceFiscale"].ToString();
                    this.Foto = DrawingProcs.GetImageFromByte(dt.Rows[0]["Foto"]);
                    if (this.Foto == null) { this.Foto = Risorse.GetImageFromResource(Risorse.GC_LOGIN_256); }
                    if (!dt.Rows[0].IsNull("FlagAdmin")) this.Admin = (bool)dt.Rows[0]["FlagAdmin"];
                    try
                    {
                        this.ConfigUtente = XmlProcs.XmlDeserializeFromString<ConfigUtente>(dt.Rows[0]["Valore"].ToString());
                    }
                    catch (Exception)
                    {
                    }
                    this.Abilitato = true;
                }
                else
                {
                    this.Abilitato = false;
                }

            }
            catch (Exception ex)
            {
                this.Abilitato = false;
                throw new Exception(ex.Message, ex);
            }

        }

        public void SbloccaTutto()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodAzioneLock", EnumLock.UNLOCKALL.ToString());
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_InfoMovLock", spcoll);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public bool SalvaConfigUtente()
        {

            bool bReturn = true;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("Codice", _codice);
                op.Parametro.Add("Valore", XmlProcs.XmlSerializeToString(this.ConfigUtente));

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                Database.ExecStoredProc("MSP_AggConfigUtente", spcoll);

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"Utente.SalvaConfigUtente()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

        public void SvuotaTemp()
        {
            try
            {
                string sdir = FileStatics.GetSCCITempPath(false);
                if (System.IO.Directory.Exists(sdir))
                {
                    System.IO.Directory.Delete(sdir, true);
                }

            }
            catch (Exception)
            {
            }
        }

    }

    [Serializable()]
    public class Ruoli
    {

        private Ruolo _ruoloselezionato = null;
        private List<Ruolo> _elementi = new List<Ruolo>();

        public Ruoli(string codlogin)
        {
            Carica(codlogin);
        }

        public Ruolo RuoloSelezionato
        {
            get { return _ruoloselezionato; }
            set
            {
                _ruoloselezionato = value;
                _ruoloselezionato.NewsLiteChange = true;
            }
        }

        public List<Ruolo> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        internal void Carica(string sCodLogin)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodLogin", sCodLogin);
                op.Parametro.Add("DatiEstesi", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelRuoli", spcoll);

                foreach (DataRow oDr in dt.Rows)
                {
                    _elementi.Add(new Ruolo(oDr["Codice"].ToString(), oDr["Descrizione"].ToString(), oDr["CodTipoDiario"].ToString(), int.Parse(oDr["DaFirmare"].ToString()), int.Parse(oDr["NumMaxCercaEpi"].ToString()), (bool)oDr["RichiediPassword"], (bool)oDr["LimitaEVCAmbulatoriale"]));
                }
                if (_elementi.Count == 1) { this.RuoloSelezionato = _elementi[0]; }


            }
            catch (Exception ex)
            {
                _elementi.Clear();
                throw new Exception(ex.Message, ex);
            }

        }

    }

    [Serializable()]
    public class Ruolo
    {

        private string _codice = string.Empty;
        private string _descrizione = string.Empty;
        private string _codtipodiario = string.Empty;
        private int _nummaxcercaepi = 0;
        private int _dafirmare = 0;
        private int _bookmarks = 0;
        private int _cartelleinvisione = 0;
        private int _pazientiinvisione = 0;
        private int _pazientiseguiti = 0;
        private int _pazienteseguito = 0;
        private int _pazientiseguitidaaltri = 0;
        private bool _richiedipassword = false;
        private bool _limitaevcambulatoriale = false;
        private Moduli _moduli = null;
        private CDSSClient _cdssclient = null;

        public Ruolo(string codice, string descrizione, string codtipodiario, int dafirmare, int nummaxcercaepi, bool richiedipassword, bool limitaevcambulatoriale)
        {
            _codice = codice;
            _descrizione = descrizione;
            _dafirmare = dafirmare;
            _nummaxcercaepi = nummaxcercaepi;
            _richiedipassword = richiedipassword;
            _limitaevcambulatoriale = limitaevcambulatoriale;
            _moduli = new Moduli(codice);
            _cdssclient = new CDSSClient(codice);
            this.NewsHard = 0;
            this.NewsHardInLettura = false;
            this.NewsLite = 0;
            this.NewsLiteInLettura = false;
            this.NewsLiteChange = false;
            this.MatHome = false;
        }

        public string Codice
        {
            get { return _codice; }
            set { _codice = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        public string CodTipoDiario
        {
            get { return _codtipodiario; }
            set { _codtipodiario = value; }
        }

        public int NumMaxCercaEpi
        {
            get { return _nummaxcercaepi; }
            set { _nummaxcercaepi = value; }
        }

        public int DaFirmare
        {
            get { return _dafirmare; }
            set { _dafirmare = value; }
        }

        public int Bookmarks
        {
            get { return _bookmarks; }
            set { _bookmarks = value; }
        }

        public int CartelleInVisione
        {
            get { return _cartelleinvisione; }
            set { _cartelleinvisione = value; }
        }

        public int PazientiInVisione
        {
            get { return _pazientiinvisione; }
            set { _pazientiinvisione = value; }
        }

        public int PazientiSeguiti
        {
            get { return _pazientiseguiti; }
            set { _pazientiseguiti = value; }
        }

        public int PazienteSeguito
        {
            get { return _pazienteseguito; }
            set { _pazienteseguito = value; }
        }

        public int PazientiSeguitiDaAltri
        {
            get { return _pazientiseguitidaaltri; }
            set { _pazientiseguitidaaltri = value; }
        }

        public int NewsHard { get; set; }
        public bool NewsHardInLettura { get; set; }

        public int NewsLite { get; set; }
        public bool NewsLiteInLettura { get; set; }
        public bool NewsLiteChange { get; set; }

        public bool MatHome { get; set; }

        public bool RichiediPassword
        {
            get { return _richiedipassword; }
            set { _richiedipassword = value; }
        }

        public bool LimitaEVCAmbulatoriale
        {
            get { return _limitaevcambulatoriale; }
            set { _limitaevcambulatoriale = value; }
        }

        public Moduli Moduli
        {
            get { return _moduli; }
            set { _moduli = value; }
        }

        public CDSSClient CDSSClient
        {
            get { return _cdssclient; }
            set { _cdssclient = value; }
        }

        public bool Esiste(EnumModules modulo)
        {

            try
            {
                Modulo oItem = this.Moduli.Elementi.Find(Modulo => Modulo.Codice == modulo.ToString());
                return (oItem == null ? false : true);
            }
            catch
            {
                return false;
            }

        }
        public Modulo EsisteElemento(EnumModules modulo)
        {

            try
            {
                Modulo oItem = this.Moduli.Elementi.Find(Modulo => Modulo.Codice == modulo.ToString());
                return oItem;
            }
            catch
            {
                return null;
            }

        }

    }

    [Serializable()]
    public class Moduli
    {

        private List<Modulo> _elementi = new List<Modulo>();

        public Moduli(string codruolo)
        {
            this.Carica(codruolo);

        }

        public List<Modulo> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        private void Carica(string sCodRuolo)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodRuolo", sCodRuolo);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelModuli", spcoll);

                foreach (DataRow oDr in dt.Rows)
                {
                    _elementi.Add(new Modulo(oDr["Codice"].ToString(), oDr["Descrizione"].ToString(),
                                                bool.Parse(int.Parse(oDr["Abilitato"].ToString()) == 0 ? bool.FalseString : bool.TrueString)));
                }

            }
            catch (Exception ex)
            {
                _elementi.Clear();
                throw new Exception(ex.Message, ex);
            }

        }

    }

    [Serializable()]
    public class Modulo
    {

        private string _codice = string.Empty;
        private string _descrizione = string.Empty;
        private bool _abilitato = false;

        public Modulo(string codice, string descrizione, bool abilitato)
        {
            _codice = codice;
            _descrizione = descrizione;
            _abilitato = abilitato;
        }

        public string Codice
        {
            get { return _codice; }
            set { _codice = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        public bool Abilitato
        {
            get { return _abilitato; }
            set { _abilitato = value; }
        }

    }


    [Serializable()]
    public class Navigazione
    {

        private Maschere _maschere = null;

        public Navigazione()
        {
            _maschere = new Maschere();
        }

        public Maschere Maschere
        {
            get { return _maschere; }
            set { _maschere = value; }
        }

    }


    [Serializable()]
    public class Maschera
    {

        private EnumMaschere _id;
        private Interfacce.IViewUserControlMiddle _ctlmiddle;
        private Interfacce.IViewFormlModal _ctlmodal;
        private string _codmaschera;
        private string _descrizione;

        private bool _aggiorna;
        private bool _modale;
        private bool _incache;
        private bool _incachedapercorso;
        private bool _cambiopercorso;
        private int _timerrefresh;

        private bool _segnalibroadd;
        private bool _segnalibrovisualizza;

        private bool _indietro;
        private bool _home;
        private bool _avanti;

        private Reports _reports = null;

        private Maschera _mascherapartenza = null;

        private Maschera()
        {
        }
        public Maschera(EnumMaschere ID, Interfacce.IViewUserControlMiddle uc, string codmaschera, string codruolo, string codua)
        {
            try
            {
                _id = ID;
                _ctlmiddle = uc;
                _codmaschera = codmaschera;
                _indietro = false;
                _home = true;
                _avanti = false;
                _mascherapartenza = null;
                _segnalibroadd = false;
                _segnalibrovisualizza = false;

                this.Massimizzata = false;

                this.Carica();
            }
            catch (Exception)
            {
                throw;
            }
            try
            {
                _reports = new Reports(codmaschera, codruolo, codua);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        public Maschera(EnumMaschere ID, Interfacce.IViewUserControlMiddle uc, string codmaschera, string codruolo, string codua, bool indietro, bool home, bool avanti)
        {
            try
            {
                _id = ID;
                _ctlmiddle = uc;
                _codmaschera = codmaschera;
                _indietro = indietro;
                _home = home;
                _avanti = avanti;
                _mascherapartenza = null;
                this.Massimizzata = false;
                _segnalibroadd = false;
                _segnalibrovisualizza = false;
                this.Carica();
            }
            catch (Exception)
            {
                throw;
            }

            try
            {
                _reports = new Reports(codmaschera, codruolo, codua);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }
        public Maschera(EnumMaschere ID, Interfacce.IViewFormlModal uc, string codmaschera, string codruolo, string codua)
        {
            try
            {
                _id = ID;
                _ctlmodal = uc;
                _codmaschera = codmaschera;
                _indietro = false;
                _home = true;
                _avanti = false;
                _mascherapartenza = null;
                this.Massimizzata = false;
                _segnalibroadd = false;
                _segnalibrovisualizza = false;
                this.Carica();
            }
            catch (Exception)
            {
                throw;
            }

            try
            {
                _reports = new Reports(codmaschera, codruolo, codua);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        public Object CustomParamaters { get; set; }

        public EnumMaschere ID
        {
            get
            {
                return _id;
            }
            private set
            {
                _id = value;
            }
        }

        public Interfacce.IViewUserControlMiddle ControlMiddle
        {
            get
            {
                return _ctlmiddle;
            }
            set
            {
                _ctlmiddle = value;
            }
        }

        public Interfacce.IViewFormlModal ControlModal
        {
            get
            {
                return _ctlmodal;
            }
            set
            {
                _ctlmodal = value;
            }
        }

        public string CodMaschera
        {
            get
            {
                return _codmaschera;
            }
        }

        public string Descrizione
        {
            get
            {
                return _descrizione;
            }
            set
            {
                _descrizione = value;
            }
        }

        public bool Aggiorna
        {
            get
            {
                return _aggiorna;
            }
            set
            {
                _aggiorna = value;
            }
        }

        public bool Modale
        {
            get
            {
                return _modale;
            }
        }

        public bool Massimizzata
        {
            get;
            set;
        }

        public bool InCache
        {
            get
            {
                return _incache;
            }
        }

        public bool InCacheDaPercorso
        {
            get
            {
                return _incachedapercorso;
            }
        }

        public bool CambioPercorso
        {
            get
            {
                return _cambiopercorso;
            }
        }

        public int TimerRefresh
        {
            get
            {
                return _timerrefresh;
            }
        }

        public bool SegnalibroAdd
        {
            get
            {
                return _segnalibroadd;
            }
            set
            {
                _segnalibroadd = value;
            }
        }

        public bool SegnalibroVisualizza
        {
            get
            {
                return _segnalibrovisualizza;
            }
            set
            {
                _segnalibrovisualizza = value;
            }
        }

        public bool Indietro
        {
            get
            {
                return _indietro;
            }
            set
            {
                _indietro = value;
            }
        }

        public bool Home
        {
            get
            {
                return _home;
            }
            set
            {
                _home = value;
            }
        }

        public bool Stampe
        {
            get
            {
                return (_reports == null || _reports.Elementi.Count == 0 ? false : true);
            }
        }

        public bool Avanti
        {
            get
            {
                return _avanti;
            }
            set
            {
                _avanti = value;
            }
        }

        public Reports Reports
        {
            get { return _reports; }
        }

        public Maschera MascheraPartenza
        {
            get
            {
                return _mascherapartenza;
            }
            set
            {
                _mascherapartenza = value;
            }
        }

        private void Carica()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodMaschera", _codmaschera);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMaschere", spcoll);

                if (dt.Rows.Count == 1)
                {
                    _descrizione = dt.Rows[0]["Descrizione"].ToString();
                    _modale = bool.Parse(dt.Rows[0]["Modale"].ToString());
                    _incache = bool.Parse(dt.Rows[0]["InCache"].ToString());

                    if (dt.Columns.Contains("InCacheDaPercorso") && !dt.Rows[0].IsNull("InCacheDaPercorso"))
                    {
                        _incachedapercorso = bool.Parse(dt.Rows[0]["InCacheDaPercorso"].ToString());
                    }
                    if (dt.Columns.Contains("CambioPercorso") && !dt.Rows[0].IsNull("CambioPercorso"))
                    {
                        _cambiopercorso = bool.Parse(dt.Rows[0]["CambioPercorso"].ToString());
                    }

                    _aggiorna = bool.Parse(dt.Rows[0]["Aggiorna"].ToString());
                    _segnalibroadd = bool.Parse(dt.Rows[0]["SegnalibroAdd"].ToString());
                    _segnalibrovisualizza = bool.Parse(dt.Rows[0]["SegnalibroVisualizza"].ToString());

                    _timerrefresh = int.Parse(dt.Rows[0]["TimerRefresh"].ToString());

                    if (dt.Columns.Contains("Massimizzata") && !dt.Rows[0].IsNull("Massimizzata")) this.Massimizzata = bool.Parse(dt.Rows[0]["Massimizzata"].ToString());
                }
                else
                {
                    _descrizione = string.Empty;
                    _modale = false;
                    _incache = false;
                    _incachedapercorso = false;
                    _cambiopercorso = false;
                    _timerrefresh = 0;
                    this.Massimizzata = false;
                    _segnalibroadd = false;
                    _segnalibrovisualizza = false;
                }

            }
            catch (Exception ex)
            {
                _descrizione = string.Empty;
                _modale = false;
                _incache = false;
                _incachedapercorso = false;
                _cambiopercorso = false;
                _timerrefresh = 0;
                this.Massimizzata = false;
                _segnalibroadd = false;
                _segnalibrovisualizza = false;
                throw new Exception(ex.Message, ex);
            }

        }

        public void CaricaReports(string codruolo, string codUA)
        {
            _reports = null;
            _reports = new Reports(_codmaschera, codruolo, codUA);
        }

    }

    [Serializable()]
    public class Paziente
    {

        private string _id = String.Empty;
        private string _idepisodio = String.Empty;
        private string _cognome = String.Empty;
        private string _nome = String.Empty;
        private string _sesso = String.Empty;
        private string _cf = String.Empty;

        private string _codSacFuso = string.Empty;

        private string _codcomunenasc = String.Empty;
        private string _comunenasc = String.Empty;
        private string _provincianasc = String.Empty;
        private DateTime _datanasc;

        private string _indirizzores = String.Empty;
        private string _localitares = String.Empty;
        private string _comuneres = String.Empty;
        private string _provinciares = String.Empty;
        private string _regioneres = String.Empty;
        private string _capres = String.Empty;

        private string _terminazioneCodice = String.Empty;
        private string _terminazioneDescrizione = String.Empty;
        private DateTime _terminazioneData = DateTime.MinValue;
        private DateTime _dataDecesso = DateTime.MinValue;

        private string _codstatoconsensocalcolato = string.Empty;

        private string _indirizzodom = String.Empty;
        private string _localitadom = String.Empty;
        private string _comunedom = String.Empty;
        private string _provinciadom = String.Empty;
        private string _capdom = String.Empty;

        private string _cognomenomemedicobase = String.Empty;
        private string _codfiscmedicobase = String.Empty;

        private Image _foto = null;
        private bool _attivo = false;
        private Allergie _allergie = null;
        private PazienteSac _pazSAC = null;
        private PazienteSacDatiAggiuntivi _pazDatiAgg = null;

        private PazientiConsensi _pazienticonsensigenerico = null;
        private PazientiConsensi _pazienticonsensidossier = null;
        private PazientiConsensi _pazienticonsensidossierstorico = null;

        public Paziente()
        {
        }
        public Paziente(string idpaziente, string idepisodio)
        {
            this.Carica(idpaziente, idepisodio);
            if (_attivo == true) { _allergie = new Allergie(_id); }
        }
        public Paziente(PazienteSac pazientesac, string codUA_Ambulatoriale, string descrUA_Ambulatoriale)
        {
            this.CaricaDaSAC(pazientesac);
            this.CodUAAmbulatoriale = codUA_Ambulatoriale;
            this.DescrUAAmbulatoriale = descrUA_Ambulatoriale;
            if (_attivo == true) { _allergie = new Allergie(_id); }
        }
        public Paziente(string idpaziente, string codUA_Ambulatoriale, string descrUA_Ambulatoriale)
        {
            this.Carica(idpaziente, "");
            this.CodUAAmbulatoriale = codUA_Ambulatoriale;
            this.DescrUAAmbulatoriale = descrUA_Ambulatoriale;
            if (_attivo == true) { _allergie = new Allergie(_id); }
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Cognome
        {
            get { return _cognome; }
            set { _cognome = value; }
        }

        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public string Sesso
        {
            get { return _sesso; }
            set { _sesso = value; }
        }

        public string CodiceFiscale
        {
            get { return _cf; }
            set { _cf = value; }
        }


        public string CodComuneNascita
        {
            get { return _codcomunenasc; }
            set { _codcomunenasc = value; }
        }

        public string ComuneNascita
        {
            get { return _comunenasc; }
            set { _comunenasc = value; }
        }

        public string ProvinciaNascita
        {
            get { return _provincianasc; }
            set { _provincianasc = value; }
        }

        public DateTime DataNascita
        {
            get { return _datanasc; }
            set { _datanasc = value; }
        }



        public string TerminazioneCodice
        {
            get { return _terminazioneCodice; }
            set { _terminazioneCodice = value; }
        }
        public string TerminazioneDescrizione
        {
            get { return _terminazioneDescrizione; }
            set { _terminazioneDescrizione = value; }
        }
        public DateTime TerminazioneData
        {
            get { return _terminazioneData; }
            set { _terminazioneData = value; }
        }
        public DateTime DataDecesso
        {
            get { return _dataDecesso; }
            set { _dataDecesso = value; }
        }

        public string IndirizzoResidenza
        {
            get { return _indirizzores; }
            set { _indirizzores = value; }
        }

        public string LocalitaResidenza
        {
            get { return _localitares; }
            set { _localitares = value; }
        }

        public string ComuneResidenza
        {
            get { return _comuneres; }
            set { _comuneres = value; }
        }

        public string ProvinciaResidenza
        {
            get { return _provinciares; }
            set { _provinciares = value; }
        }

        public string RegioneResidenza
        {
            get { return _regioneres; }
            set { _regioneres = value; }
        }
        public string CAPResidenza
        {
            get { return _capres; }
            set { _capres = value; }
        }

        public string IndirizzoDomicilio
        {
            get { return _indirizzodom; }
            set { _indirizzodom = value; }
        }


        public string LocalitaDomicilio
        {
            get { return _localitadom; }
            set { _localitadom = value; }
        }


        public string ComuneDomicilio
        {
            get { return _comunedom; }
            set { _comunedom = value; }
        }

        public string ProvinciaDomicilio
        {
            get { return _provinciadom; }
            set { _provinciadom = value; }
        }

        public string CAPDomicilio
        {
            get { return _capdom; }
            set { _capdom = value; }
        }

        public string CognomeNomeMedicoBase
        {
            get { return _cognomenomemedicobase; }
            set { _cognomenomemedicobase = value; }
        }

        public string CodFiscMedicoBase
        {
            get { return _codfiscmedicobase; }
            set { _codfiscmedicobase = value; }
        }

        public Image Foto
        {
            get { return _foto; }
            set { _foto = value; }
        }

        public bool Attivo
        {
            get { return _attivo; }
            set { _attivo = value; }
        }

        public string Descrizione
        {
            get
            {
                return string.Format("{1} {0} ({2}), {3}. ({4}), {5}",
                            _nome, _cognome, _sesso, Eta, (_datanasc.Ticks != 0 ? _datanasc.ToShortDateString() : "Indefinita"), _cf);
            }

        }

        public string EtaPediatrico
        {
            get
            {

                string sRet = string.Empty;

                if (_datanasc.Ticks != 0)
                {
                    DateDifference oDd = new DateDifference(_datanasc, DateTime.Now);
                    sRet = oDd.ToString();
                }
                else
                {
                    sRet = "Indefinita";
                }

                return sRet;
            }

        }

        public int EtaAnni
        {
            get
            {


                int iAnni = -1;

                if (_datanasc.Ticks != 0)
                {
                    iAnni = DateTime.Now.Year - _datanasc.Year;

                    if (_datanasc.DayOfYear > DateTime.Now.Date.DayOfYear && iAnni > 0)
                        iAnni -= 1;
                }

                return iAnni;

            }

        }

        public string Eta
        {
            get
            {

                if (this.EtaAnni > 14)
                    return this.EtaAnni.ToString() + "aa";
                else
                    return this.EtaPediatrico;

            }

        }

        public Allergie Allergie
        {
            get { return _allergie; }
            set { _allergie = value; }
        }

        public string CodSAC { get; set; }
        public string CodSACFuso
        {
            get
            {
                if (_codSacFuso != null && _codSacFuso != string.Empty && _codSacFuso.Trim() != "")
                    return _codSacFuso;
                else
                    return CodSAC;
            }
            set { _codSacFuso = value; }
        }
        public string IDPazienteFuso { get; set; }

        public string CodUAAmbulatoriale { get; set; }
        public string DescrUAAmbulatoriale { get; set; }

        public PazienteSac PazienteSac
        {
            get
            {
                if (_pazSAC == null && this.CodSAC != null && this.CodSAC != string.Empty && this.CodSAC.Trim() != "")
                {
                    _pazSAC = DBUtils.get_RicercaPazientiSACByID(this.CodSAC);
                }

                return _pazSAC;
            }
        }

        public PazienteSacDatiAggiuntivi PazienteSacDatiAggiuntivi
        {
            get
            {
                if (_pazDatiAgg == null && this.CodSAC != null && this.CodSAC != string.Empty && this.CodSAC.Trim() != "")
                {
                    _pazDatiAgg = DBUtils.get_PazienteSacDatiAggiuntivi(this.CodSAC);
                }

                return _pazDatiAgg;
            }
        }

        public PazientiConsensi PazientiConsensiGenerico
        {
            get
            {
                if (_pazienticonsensigenerico == null)
                {
                    _pazienticonsensigenerico = new PazientiConsensi(this.IDPazienteFuso, EnumTipoConsenso.Generico.ToString(), CoreStatics.CoreApplication.Ambiente);
                }
                return _pazienticonsensigenerico;
            }
        }

        public PazientiConsensi PazientiConsensiDossier
        {
            get
            {
                if (_pazienticonsensidossier == null)
                {
                    _pazienticonsensidossier = new PazientiConsensi(this.IDPazienteFuso, EnumTipoConsenso.Dossier.ToString(), CoreStatics.CoreApplication.Ambiente);
                }
                return _pazienticonsensidossier;
            }
        }

        public PazientiConsensi PazientiConsensiDossierStorico
        {
            get
            {
                if (_pazienticonsensidossierstorico == null)
                {
                    _pazienticonsensidossierstorico = new PazientiConsensi(this.IDPazienteFuso, EnumTipoConsenso.DossierStorico.ToString(), CoreStatics.CoreApplication.Ambiente);
                }
                return _pazienticonsensidossierstorico;
            }
        }

        public string CodStatoConsensoCalcolato
        {
            get { return _codstatoconsensocalcolato; }
        }

        public Boolean WarningElaborati { get; set; }

        public void AggiornaDatiSAC()
        {
            try
            {
                if (this.CodSACFuso != null && this.CodSACFuso != string.Empty && this.CodSACFuso.Trim() != "")
                {
                    PazienteSac paz = DBUtils.get_RicercaPazientiSACByID(this.CodSACFuso);

                    CaricaDaSAC(paz);

                    _pazDatiAgg = null;
                }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }

        public bool SalvaFoto()
        {
            try
            {
                bool bReturn = false;

                if (this.ID != null && this.ID != string.Empty && this.ID.Trim() != "")
                {
                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("ID", this.ID);
                    if (this.Foto == null)
                        op.Parametro.Add("Foto", "NULL");
                    else
                        op.Parametro.Add("Foto", CoreStatics.ImageToBase64(this.Foto, System.Drawing.Imaging.ImageFormat.Jpeg));

                    op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();
                    op.TimeStamp.CodEntita = EnumEntita.PAZ.ToString();

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggPazienti", spcoll);

                    Carica(_id, _idepisodio);

                    bReturn = true;
                }

                return bReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AggiungiRecenti()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", _id);
                op.Parametro.Add("CodUtente", CoreStatics.CoreApplication.Sessione.Utente.Codice);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                Database.GetDataTableStoredProc("MSP_InsMovPazientiRecenti", spcoll);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public DataTable CercaCartelle()
        {
            try
            {
                DataTable dtRet = null;

                if (ID != null && ID.Trim() != "")
                {
                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDPaziente", ID);

                    op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();
                    op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    dtRet = Database.GetDataTableStoredProc("MSP_CercaCartellaSuPaziente", spcoll);
                }

                return dtRet;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Carica(string idpaziente, string idepisodio)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", idpaziente);
                op.Parametro.Add("IDEpisodio", idepisodio);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelPaziente", spcoll);

                _terminazioneCodice = string.Empty;
                _terminazioneDescrizione = string.Empty;
                _terminazioneData = DateTime.MinValue;
                _dataDecesso = DateTime.MinValue;

                this.CodUAAmbulatoriale = String.Empty;
                this.DescrUAAmbulatoriale = String.Empty;
                this.CodSACFuso = string.Empty;

                if (dt.Rows.Count == 1)
                {
                    _id = dt.Rows[0]["IDPaziente"].ToString();
                    _idepisodio = idepisodio;

                    if (!dt.Rows[0].IsNull("CodSAC")) this.CodSAC = dt.Rows[0]["CodSAC"].ToString();
                    if (dt.Columns.Contains("CodSACFuso") && !dt.Rows[0].IsNull("CodSACFuso")) this.CodSACFuso = dt.Rows[0]["CodSACFuso"].ToString();
                    if (!dt.Rows[0].IsNull("IDPazienteFuso")) this.IDPazienteFuso = dt.Rows[0]["IDPazienteFuso"].ToString();
                    _cognome = dt.Rows[0]["Cognome"].ToString();
                    _nome = dt.Rows[0]["Nome"].ToString();
                    _sesso = dt.Rows[0]["Sesso"].ToString();
                    try
                    {
                        _datanasc = DateTime.Parse(dt.Rows[0]["DataNascita"].ToString());
                    }
                    catch (Exception)
                    {
                    }
                    _cf = dt.Rows[0]["CodiceFiscale"].ToString();

                    _codcomunenasc = dt.Rows[0]["CodComuneNascita"].ToString();
                    _comunenasc = dt.Rows[0]["ComuneNascita"].ToString();
                    _provincianasc = dt.Rows[0]["ProvinciaNascita"].ToString();

                    if (dt.Columns.Contains("DataDecesso") && !dt.Rows[0].IsNull("DataDecesso"))
                    {
                        _dataDecesso = (DateTime)dt.Rows[0]["DataDecesso"];
                    }

                    _indirizzores = dt.Rows[0]["IndirizzoResidenza"].ToString();
                    _localitares = dt.Rows[0]["LocalitaResidenza"].ToString();
                    _comuneres = dt.Rows[0]["ComuneResidenza"].ToString();
                    _provinciares = dt.Rows[0]["ProvinciaResidenza"].ToString();
                    _regioneres = dt.Rows[0]["RegioneResidenza"].ToString();
                    _capres = dt.Rows[0]["CAPResidenza"].ToString();

                    _indirizzodom = dt.Rows[0]["IndirizzoDomicilio"].ToString();
                    _localitadom = dt.Rows[0]["LocalitaDomicilio"].ToString();
                    _comunedom = dt.Rows[0]["ComuneDomicilio"].ToString();
                    _provinciadom = dt.Rows[0]["ProvinciaDomicilio"].ToString();
                    _capdom = dt.Rows[0]["CAPDomicilio"].ToString();

                    _cognomenomemedicobase = dt.Rows[0]["CognomeNomeMedicoBase"].ToString();
                    _codfiscmedicobase = dt.Rows[0]["CodFiscMedicoBase"].ToString();
                    _codstatoconsensocalcolato = dt.Rows[0]["CodStatoConsensoCalcolato"].ToString();

                    _foto = DrawingProcs.GetImageFromByte(dt.Rows[0]["Foto"]);
                    if (_foto == null)
                    {

                        switch (_sesso.ToUpper())
                        {

                            case "F":
                                _foto = Risorse.GetImageFromResource(Risorse.GC_PAZIENTEFEMMINA_256);
                                break;

                            case "M":
                                _foto = Risorse.GetImageFromResource(Risorse.GC_PAZIENTEMASCHIO_256);
                                break;

                            default:
                                _foto = Risorse.GetImageFromResource(Risorse.GC_PAZIENTI_256);
                                break;

                        }

                    }

                    _attivo = true;

                }
                else
                {
                    _attivo = false;
                }

            }
            catch (Exception ex)
            {
                _attivo = false;
                throw new Exception(ex.Message, ex);
            }

        }

        private void CaricaDaSAC(PazienteSac pazientesac)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodSAC", Database.testoSQL(pazientesac.CodSAC));
                op.Parametro.Add("Cognome", Database.testoSQL(pazientesac.Cognome));
                op.Parametro.Add("Nome", Database.testoSQL(pazientesac.Nome));
                op.Parametro.Add("Sesso", Database.testoSQL(pazientesac.Sesso));
                op.Parametro.Add("CodiceFiscale", Database.testoSQL(pazientesac.CodiceFiscale));

                if (pazientesac.DataNascita != DateTime.MinValue)
                {
                    op.Parametro.Add("DataNascita", Database.data105PerParametri(pazientesac.DataNascita));
                }
                op.Parametro.Add("CodComuneNascita", Database.testoSQL(pazientesac.CodComuneNascita));
                op.Parametro.Add("ComuneNascita", Database.testoSQL(pazientesac.ComuneNascita));
                op.Parametro.Add("LocalitaNascita", Database.testoSQL(pazientesac.LocalitaNascita));
                op.Parametro.Add("CodProvinciaNascita", Database.testoSQL(pazientesac.CodProvinciaNascita));
                op.Parametro.Add("ProvinciaNascita", Database.testoSQL(pazientesac.ProvinciaNascita));

                op.Parametro.Add("CAPResidenza", Database.testoSQL(pazientesac.CAPResidenza));
                op.Parametro.Add("CodComuneResidenza", Database.testoSQL(pazientesac.CodComuneResidenza));
                op.Parametro.Add("ComuneResidenza", Database.testoSQL(pazientesac.ComuneResidenza));
                op.Parametro.Add("IndirizzoResidenza", Database.testoSQL(pazientesac.IndirizzoResidenza));
                op.Parametro.Add("LocalitaResidenza", Database.testoSQL(pazientesac.LocalitaResidenza));
                op.Parametro.Add("CodProvinciaResidenza", Database.testoSQL(pazientesac.CodProvinciaResidenza));
                op.Parametro.Add("ProvinciaResidenza", Database.testoSQL(pazientesac.ProvinciaResidenza));
                op.Parametro.Add("CodRegioneResidenza", Database.testoSQL(pazientesac.CodRegioneResidenza));
                op.Parametro.Add("RegioneResidenza", Database.testoSQL(pazientesac.RegioneResidenza));

                op.Parametro.Add("CAPDomicilio", Database.testoSQL(pazientesac.CAPDomicilio));
                op.Parametro.Add("CodComuneDomicilio", Database.testoSQL(pazientesac.CodComuneDomicilio));
                op.Parametro.Add("ComuneDomicilio", Database.testoSQL(pazientesac.ComuneDomicilio));
                op.Parametro.Add("IndirizzoDomicilio", Database.testoSQL(pazientesac.IndirizzoDomicilio));
                op.Parametro.Add("LocalitaDomicilio", Database.testoSQL(pazientesac.LocalitaDomicilio));
                op.Parametro.Add("CodProvinciaDomicilio", Database.testoSQL(pazientesac.CodProvinciaDomicilio));
                op.Parametro.Add("ProvinciaDomicilio", Database.testoSQL(pazientesac.ProvinciaDomicilio));

                op.Parametro.Add("CodRegioneDomicilio", "");
                op.Parametro.Add("RegioneDomicilio", "");

                this.CodSAC = pazientesac.CodSAC;
                if (PazienteSacDatiAggiuntivi != null)
                {
                    op.Parametro.Add("CodMedicoBase", PazienteSacDatiAggiuntivi.CodiceMedicoDiBase.ToString());
                    op.Parametro.Add("CodFiscMedicoBase", Database.testoSQL(PazienteSacDatiAggiuntivi.CodiceFiscaleMedicoDiBase));
                    op.Parametro.Add("CognomeNomeMedicoBase", Database.testoSQL(PazienteSacDatiAggiuntivi.CognomeNomeMedicoDiBase));
                    op.Parametro.Add("DistrettoMedicoBase", Database.testoSQL(PazienteSacDatiAggiuntivi.DistrettoMedicoDiBase));
                    if (PazienteSacDatiAggiuntivi.DataSceltaMedicoDiBase != DateTime.MinValue)
                    {
                        op.Parametro.Add("DataSceltaMedicoBase", Database.dataOra105PerParametri(PazienteSacDatiAggiuntivi.DataSceltaMedicoDiBase));
                    }
                    op.Parametro.Add("ElencoEsenzioni", Database.testoSQL(PazienteSacDatiAggiuntivi.DescrizioniEsenzioni(DateTime.Now)));
                }

                if (pazientesac.DataDecesso != DateTime.MinValue) op.Parametro.Add("DataDecesso", Database.dataOra105PerParametri(pazientesac.DataDecesso));
                else op.Parametro.Add("DataDecesso", "");


                op.TimeStamp.CodAzione = EnumAzioni.INS.ToString();
                op.TimeStamp.CodEntita = EnumEntita.PAZ.ToString();

                op.Parametro.Add("CreaPaziente", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_ControlloPazienteDaSAC", spcoll);

                if (dt.Rows.Count == 1)
                {
                    _pazSAC = pazientesac;

                    string idPaziente = string.Empty;

                    idPaziente = dt.Rows[0]["IDPaziente"].ToString();

                    this.Carica(idPaziente, "");

                    if (PazienteSacDatiAggiuntivi != null)
                    {
                        UnicodeSrl.Scci.Statics.CommonStatics.UpdateConsensiDaSAC(this.IDPazienteFuso, PazienteSacDatiAggiuntivi.Consensi, CoreStatics.CoreApplication.Ambiente);
                        this.Carica(idPaziente, "");
                    }

                    _terminazioneCodice = (pazientesac.TerminazioneCodice != null ? pazientesac.TerminazioneCodice : string.Empty);
                    _terminazioneDescrizione = (pazientesac.TerminazioneDescrizione != null ? pazientesac.TerminazioneDescrizione : string.Empty);
                    _terminazioneData = pazientesac.TerminazioneData;
                    if (_dataDecesso == DateTime.MinValue)
                        _dataDecesso = pazientesac.DataDecesso;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        internal void RicaricaConsensoCalcolato()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", _id);
                op.Parametro.Add("IDEpisodio", _idepisodio);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelPaziente", spcoll);

                if (dt.Rows.Count == 1)
                {
                    _codstatoconsensocalcolato = dt.Rows[0]["CodStatoConsensoCalcolato"].ToString();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

    }

    [Serializable()]
    public class Episodio
    {

        private string _id = String.Empty;
        private bool _attivo = false;
        private AlertsGenerici _alertsgenerici = null;
        private EvidenzeCliniche _evidenzecliniche = null;
        private DateTime _dataricovero = DateTime.MinValue;
        private DateTime _datadimissione = DateTime.MinValue;
        private string _numeroepisodio = string.Empty;
        private string _numerolistaattesa = string.Empty;
        private string _codtipoepisodio = string.Empty;
        private string _descrizionetipoepisodio = string.Empty;
        private string _codazi = string.Empty;
        private string _coduaaccesso = string.Empty;
        private string _descrizioneuaaccesso = string.Empty;
        private string _coduoaccesso = string.Empty;
        private string _descrizioneuoaccesso = string.Empty;

        public Episodio()
        {
        }
        public Episodio(string codepisodio)
        {
            this.Carica(codepisodio);
            if (_attivo == true) { _alertsgenerici = new AlertsGenerici(codepisodio); }
            if (_attivo == true) { _evidenzecliniche = new EvidenzeCliniche(codepisodio); }
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public bool Attivo
        {
            get { return _attivo; }
            set { _attivo = value; }
        }

        public AlertsGenerici AlertsGenerici
        {
            get { return _alertsgenerici; }
            set { _alertsgenerici = value; }
        }

        public EvidenzeCliniche EvidenzeCliniche
        {
            get { return _evidenzecliniche; }
            set { _evidenzecliniche = value; }
        }

        public DateTime DataRicovero
        {
            get { return _dataricovero; }
            set { _dataricovero = value; }
        }

        public DateTime DataDimissione
        {
            get { return _datadimissione; }
            set { _datadimissione = value; }
        }

        public string NumeroEpisodio
        {
            get { return _numeroepisodio; }
            set { _numeroepisodio = value; }
        }

        public string NumeroListaAttesa
        {
            get { return _numerolistaattesa; }
            set { _numerolistaattesa = value; }
        }

        public string CodAzienda
        {
            get { return _codazi; }
            set { _codazi = value; }
        }

        public string CodTipoEpisodio
        {
            get { return _codtipoepisodio; }
            set { _codtipoepisodio = value; }
        }

        public string DescrizioneTipoEpisodio
        {
            get { return _descrizionetipoepisodio; }
            set { _descrizionetipoepisodio = value; }
        }

        public string CodUAAccesso
        {
            get { return _coduaaccesso; }
            set { _coduaaccesso = value; }
        }

        public string DescrizioneUAAccesso
        {
            get { return _descrizioneuaaccesso; }
            set { _descrizioneuaaccesso = value; }
        }

        public string CodUOAccesso
        {
            get { return _coduoaccesso; }
            set { _coduoaccesso = value; }
        }

        public string DescrizioneUOAccesso
        {
            get { return _descrizioneuoaccesso; }
            set { _descrizioneuoaccesso = value; }
        }

        private void AzzeraValori()
        {

            _id = String.Empty;
            _dataricovero = DateTime.MinValue;
            _datadimissione = DateTime.MinValue;
            _numeroepisodio = string.Empty;

        }

        private void Carica(string idepisodio)
        {

            try
            {
                AzzeraValori();
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", idepisodio);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelEpisodio", spcoll);

                if (dt.Rows.Count == 1)
                {
                    _id = dt.Rows[0]["IDEpisodio"].ToString();
                    if (!dt.Rows[0].IsNull("DataRicovero")) _dataricovero = (DateTime)dt.Rows[0]["DataRicovero"];
                    if (!dt.Rows[0].IsNull("DataDimissione")) _datadimissione = (DateTime)dt.Rows[0]["DataDimissione"];
                    if (!dt.Rows[0].IsNull("NumeroNosologico")) _numeroepisodio = dt.Rows[0]["NumeroNosologico"].ToString();
                    if (!dt.Rows[0].IsNull("NumeroListaAttesa")) _numerolistaattesa = dt.Rows[0]["NumeroListaAttesa"].ToString();
                    if (!dt.Rows[0].IsNull("CodTipoEpisodio")) _codtipoepisodio = dt.Rows[0]["CodTipoEpisodio"].ToString();
                    if (!dt.Rows[0].IsNull("DescrizioneTipoEpisodio")) _descrizionetipoepisodio = dt.Rows[0]["DescrizioneTipoEpisodio"].ToString();
                    if (!dt.Rows[0].IsNull("CodAzi")) _codazi = dt.Rows[0]["CodAzi"].ToString();
                    if (!dt.Rows[0].IsNull("CodUAAccesso")) _coduaaccesso = dt.Rows[0]["CodUAAccesso"].ToString();
                    if (!dt.Rows[0].IsNull("DescrizioneUAAccesso")) _descrizioneuaaccesso = dt.Rows[0]["DescrizioneUAAccesso"].ToString();
                    if (!dt.Rows[0].IsNull("CodUOAccesso")) _coduoaccesso = dt.Rows[0]["CodUOAccesso"].ToString();
                    if (!dt.Rows[0].IsNull("DescrizioneUOAccesso")) _descrizioneuoaccesso = dt.Rows[0]["DescrizioneUOAccesso"].ToString();

                    _attivo = true;

                }
                else
                {
                    _attivo = false;
                }

            }
            catch (Exception ex)
            {
                _attivo = false;
                throw new Exception(ex.Message, ex);
            }

        }

    }

    [Serializable()]
    public class Consulenza
    {

        Scci.DataContracts.ScciAmbiente _ambiente = null;

        public Consulenza(string idepisodio, string idtrasferimento, string idpaziente)
        {
            this.Paziente = new Paziente(idpaziente, idepisodio);
            this.Episodio = new Episodio(idepisodio);
            this.Trasferimento = new Trasferimento(idtrasferimento, CoreStatics.CoreApplication.Ambiente);

            _ambiente = null;
            this.Consulente = null;
            this.MovDiarioClinicoConsulenza = null;
        }

        public Episodio Episodio { get; set; }
        public Trasferimento Trasferimento { get; set; }
        public Paziente Paziente { get; set; }

        public Utente Consulente { get; set; }

        public MovDiarioClinico MovDiarioClinicoConsulenza { get; set; }

        public Scci.DataContracts.ScciAmbiente Ambiente
        {
            get
            {
                if (_ambiente == null)
                {
                    _ambiente = new Scci.DataContracts.ScciAmbiente();
                    if (this.Consulente != null)
                    {
                        _ambiente.Codlogin = this.Consulente.Codice;
                        if (this.Consulente.Ruoli != null && this.Consulente.Ruoli.RuoloSelezionato != null) _ambiente.Codruolo = this.Consulente.Ruoli.RuoloSelezionato.Codice;
                    }
                    if (this.Episodio != null) _ambiente.Idepisodio = this.Episodio.ID;
                    if (this.Paziente != null) _ambiente.Idpaziente = this.Paziente.ID;
                    if (this.Trasferimento != null) _ambiente.IdTrasferimento = this.Trasferimento.ID;

                    _ambiente.Indirizzoip = CoreStatics.CoreApplication.Ambiente.Indirizzoip;
                    _ambiente.Nomepc = CoreStatics.CoreApplication.Ambiente.Nomepc;
                }
                return _ambiente;
            }
        }

        public bool InserisciConsulenza()
        {
            try
            {
                bool bOK = true;

                bOK = (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Consulenze_Login) == DialogResult.OK);

                if (bOK)
                {
                    bOK = (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Consulenze_Refertazione) == DialogResult.OK);
                }

                return bOK;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

    [Serializable()]
    public class Allergie
    {

        private int _numero = 0;
        private List<Allergia> _elementi = new List<Allergia>();

        public Allergie(string codpaziente)
        {
            this.Carica(codpaziente);
        }

        public int Numero
        {
            get { return _numero; }
            set { _numero = value; }
        }

        public List<Allergia> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        private void Carica(string codpaziente)
        {

            try
            {

                _numero = 0;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

    }

    [Serializable()]
    public class Allergia
    {

        private string _codice = string.Empty;
        private string _descrizione = string.Empty;

        public Allergia(string codice, string descrizione)
        {
            _codice = codice;
            _descrizione = descrizione;
        }

        public string Codice
        {
            get { return _codice; }
            set { _codice = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

    }

    [Serializable()]
    public class AlertsGenerici
    {

        private int _numero = 0;
        private int _davistare = 0;
        private List<AlertGenerico> _elementi = new List<AlertGenerico>();

        public AlertsGenerici(string codepisodio)
        {
            this.Carica(codepisodio);
        }

        public int Numero
        {
            get { return _numero; }
            set { _numero = value; }
        }

        public int DaVistare
        {
            get { return _davistare; }
            set { _davistare = value; }
        }

        public List<AlertGenerico> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        private void Carica(string codepisodio)
        {

            try
            {

                _numero = 0;
                _davistare = 0;
            }
            catch (Exception ex)
            {
                _numero = 0;
                _davistare = 0;
                throw new Exception(ex.Message, ex);
            }

        }

    }

    [Serializable()]
    public class AlertGenerico
    {

        private string _codice = string.Empty;
        private string _descrizione = string.Empty;

        public AlertGenerico(string codice, string descrizione)
        {
            _codice = codice;
            _descrizione = descrizione;
        }

        public string Codice
        {
            get { return _codice; }
            set { _codice = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

    }

    [Serializable()]
    public class EvidenzeCliniche
    {

        private int _numero = 0;
        private int _davistare = 0;
        private List<EvidenzaClinica> _elementi = new List<EvidenzaClinica>();

        public EvidenzeCliniche(string codepisodio)
        {
            this.Carica(codepisodio);
        }

        public int Numero
        {
            get { return _numero; }
            set { _numero = value; }
        }

        public int DaVistare
        {
            get { return _davistare; }
            set { _davistare = value; }
        }

        public List<EvidenzaClinica> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        private void Carica(string codepisodio)
        {

            try
            {

                _numero = 0;
                _davistare = 0;

            }
            catch (Exception ex)
            {
                _numero = 0;
                _davistare = 0;
                throw new Exception(ex.Message, ex);
            }

        }

    }

    [Serializable()]
    public class EvidenzaClinica
    {

        private string _codice = string.Empty;
        private string _descrizione = string.Empty;

        public EvidenzaClinica(string codice, string descrizione)
        {
            _codice = codice;
            _descrizione = descrizione;
        }

        public string Codice
        {
            get { return _codice; }
            set { _codice = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

    }

    [Serializable()]
    public class Reports
    {

        private Report _reportselezionato = null;

        private int _numero = 0;
        private List<Report> _elementi = new List<Report>();

        public Reports(string codmaschera, string codruolo, string codua)
        {
            this.Carica(codmaschera, codruolo, codua);
        }

        public int Numero
        {
            get { return _numero; }
            set { _numero = value; }
        }

        public Report ReportSelezionato
        {
            get { return _reportselezionato; }
            set { _reportselezionato = value; }
        }

        public List<Report> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        private void Carica(string codmaschera, string codruolo, string codua)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodMaschera", codmaschera);
                op.Parametro.Add("CodRuolo", codruolo);
                op.Parametro.Add("CodUA", codua);
                op.Parametro.Add("DatiEstesi", "0");

                op.TimeStamp.CodEntita = EnumEntita.RPT.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelReport", spcoll);

                foreach (DataRow oDr in dt.Rows)
                {
                    string nomeplugin = "";
                    bool dastoricizzare = false;
                    bool apriBrowser = false;
                    bool apriIE = false;
                    bool richiediStampante = false;
                    byte[] modello = null;
                    if (!oDr.IsNull("NomePlugin")) nomeplugin = oDr["NomePlugin"].ToString();
                    if (!oDr.IsNull("DaStoricizzare")) dastoricizzare = (bool)oDr["DaStoricizzare"];
                    if (!oDr.IsNull("Modello")) modello = (byte[])oDr["Modello"];
                    if (!oDr.IsNull("ApriBrowser")) apriBrowser = (bool)oDr["ApriBrowser"];
                    if (!oDr.IsNull("ApriIE")) apriIE = (bool)oDr["ApriIE"];
                    if (dt.Columns.Contains("FlagRichiediStampante") && !oDr.IsNull("FlagRichiediStampante")) richiediStampante = (bool)oDr["FlagRichiediStampante"];

                    _elementi.Add(new Report(oDr["Codice"].ToString(), oDr["Descrizione"].ToString(), oDr["Path"].ToString(), oDr["CodFormatoReport"].ToString(), oDr["Parametri"].ToString(), nomeplugin, dastoricizzare, modello, apriBrowser, apriIE, richiediStampante));
                }

                _numero = dt.Rows.Count;

            }
            catch (Exception ex)
            {
                _numero = 0;
                throw new Exception(ex.Message, ex);
            }

        }

    }

    [Serializable()]
    public class Report
    {

        private string _codice = string.Empty;
        private string _descrizione = string.Empty;
        private string _path = string.Empty;
        private string _codformatoreport = string.Empty;
        private string _parametriXML = string.Empty;
        private string _nomePlugin = string.Empty;
        private bool _dastoricizzare = false;
        private byte[] _modello = null;
        private bool _apriBrowser = false;
        private bool _apriIE = false;
        private bool _richiediStampante = false;

        public const string COD_REPORT_CARTELLA_PAZIENTE = "CTLPZN1";
        public const string COD_REPORT_ETICHETTA_ALLEGATO = "STPALL1";
        public const string COD_REPORT_SCHEDE_PAZIENTE = "SCHDPZN1";
        public const string COD_REPORT_SCHEDE_PAZIENTE_C = "SCHDPZN2";
        public const string COD_REPORT_SCHEDA_ETICHETTA = "SCHETI1";
        public const string COD_REPORT_DIARIO_CLINICO_RPT = "DCLRP1";
        public const string COD_REPORT_ETICHETTA_CARTELLA = "ETICART1";
        public const string COD_REPORT_ETICHETTA_CARTELLA_QR = "ETICART2";
        public const string COD_REPORT_FRONTESPIZIO_CARTELLA = "ETICAR2";
        public const string COD_REPORT_FRONTESPIZIO_CARTELLA3 = "ETICAR3";
        public const string COD_REPORT_ETICHETTA_CART_INT_REP = "ETICARTIR1";
        public const string COD_REPORT_ETICHETTA_CART_INT_REP_QR = "ETICARTIR2";
        public const string COD_REPORT_SCHEDA_ANTIBLASTICA = "SCHABL1";
        public const string COD_REPORT_PDF_TUTTI_REFERTI = "PDFREF1";
        public const string COD_REPORT_RPTWKIANCI = "RPTWKIANCI";
        public const string COD_REPORT_DIARIO_CLINICO = "FRMDCL1";
        public const string COD_REPORT_SCHEDA_FIRMA_DIGITALE = "SCHDPZN1";
        public const string COD_REPORT_PRESCRIZIONE_TEMPI = "FRMPRT1";
        public const string COD_REPORT_GRAFICI1 = "GRAFICI1";
        public const string COD_REPORT_GRAFICI2 = "GRAFICI2";
        public const string COD_REPORT_BRACCIALE = "BRACC1";
        public const string COD_REPORT_MH_ACCOUNT = "MHACCOUNT";
        public const string COD_REPORT_PERAMB1 = "PERAMB1";

        public const string COD_FORMATO_REPORT_WORD = "WORD";
        public const string COD_FORMATO_REPORT_CABLATO = "CAB";
        public const string COD_FORMATO_REPORT_REM = "REM";
        public const string COD_FORMATO_REPORT_PDF = "PDF";

        public Report(string codice, string descrizione, string path, string codformatoreport, string parametriXML, string nomePlugin, bool dastoricizzare, byte[] modello, bool apriBrowser, bool apriIE, bool richiediStampante)
        {
            _codice = codice;
            _descrizione = descrizione;
            _path = path;
            _codformatoreport = codformatoreport;
            _parametriXML = parametriXML;
            _nomePlugin = nomePlugin;
            _dastoricizzare = dastoricizzare;
            _modello = modello;
            _apriBrowser = apriBrowser;
            _apriIE = apriIE;
            _richiediStampante = richiediStampante;
        }

        public Report(string codice, string descrizione, string path, string codformatoreport, string parametriXML, string nomePlugin, bool dastoricizzare, byte[] modello, bool apriBrowser, bool apriIE)
        {
            _codice = codice;
            _descrizione = descrizione;
            _path = path;
            _codformatoreport = codformatoreport;
            _parametriXML = parametriXML;
            _nomePlugin = nomePlugin;
            _dastoricizzare = dastoricizzare;
            _modello = modello;
            _apriBrowser = apriBrowser;
            _apriIE = apriIE;
            _richiediStampante = false;
        }

        public Report(string codice)
        {
            _codice = codice;
            _descrizione = string.Empty;
            _path = string.Empty;
            _codformatoreport = string.Empty;
            _parametriXML = string.Empty;
            _nomePlugin = string.Empty;
            _dastoricizzare = false;
            _modello = null;
            _apriBrowser = false;
            _apriIE = false;
            _richiediStampante = false;

            CaricaReport();
        }

        public string Codice
        {
            get { return _codice; }
            set { _codice = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public string CodFormatoReport
        {
            get { return _codformatoreport; }
            set { _codformatoreport = value; }
        }

        public string ParametriXML
        {
            get { return _parametriXML; }
            set { _parametriXML = value; }
        }

        public ReportManager.ReportHandler.ReportLinkParams ReportLinkParams
        {
            get
            {

                if (_codformatoreport.Trim().ToUpper() == Report.COD_FORMATO_REPORT_REM)
                {
                    ReportManager.ReportHandler.ReportLinkParams ret = new ReportManager.ReportHandler.ReportLinkParams();
                    if (_parametriXML.Trim() != "")
                        ret = ReportManager.ReportHandler.RHStatics.DeserializeReportLinkXML(_parametriXML);

                    return ret;
                }
                else
                {
                    ReportManager.ReportHandler.ReportLinkParams ret = new ReportManager.ReportHandler.ReportLinkParams();
                    return ret;
                }
            }
        }

        public string NomePlugIn
        {
            get { return _nomePlugin; }
            set { _nomePlugin = value; }
        }

        public byte[] Modello
        {
            get { return _modello; }
            set { _modello = value; }
        }

        public bool DaStoricizzare
        {
            get { return _dastoricizzare; }
            set { _dastoricizzare = value; }
        }

        public bool ApriBrowser
        {
            get { return _apriBrowser; }
            set { _apriBrowser = value; }
        }

        public bool ApriIE
        {
            get { return _apriIE; }
            set { _apriIE = value; }
        }

        public bool RichiediStampante
        {
            get { return _richiediStampante; }
            set { _richiediStampante = value; }
        }

        public void CaricaModello()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("Codice", _codice);
                op.Parametro.Add("DatiEstesi", "1");

                op.TimeStamp.CodEntita = EnumEntita.RPT.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelReport", spcoll);

                if (dt.Rows.Count == 1)
                {
                    if (!dt.Rows[0].IsNull("Modello")) _modello = (byte[])dt.Rows[0]["Modello"];
                }

            }
            catch (Exception)
            {

            }

        }

        public void CaricaReport()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("Codice", _codice);
                op.Parametro.Add("DatiEstesi", "0");

                op.TimeStamp.CodEntita = EnumEntita.RPT.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelReport", spcoll);

                if (dt.Rows.Count == 1)
                {
                    DataRow oDr = dt.Rows[0];

                    if (!oDr.IsNull("CodFormatoReport")) _codformatoreport = oDr["CodFormatoReport"].ToString();
                    if (!oDr.IsNull("Descrizione")) _descrizione = oDr["Descrizione"].ToString();
                    if (!oDr.IsNull("Path")) _path = oDr["Path"].ToString();
                    if (!oDr.IsNull("Parametri")) _parametriXML = oDr["Parametri"].ToString();
                    if (!oDr.IsNull("NomePlugin")) _nomePlugin = oDr["NomePlugin"].ToString();
                    if (!oDr.IsNull("DaStoricizzare")) _dastoricizzare = (bool)oDr["DaStoricizzare"];
                    if (!oDr.IsNull("Modello")) _modello = (byte[])oDr["Modello"];
                    if (!oDr.IsNull("ApriBrowser")) _apriBrowser = (bool)oDr["ApriBrowser"];
                    if (!oDr.IsNull("ApriIE")) _apriIE = (bool)oDr["ApriIE"];
                    if (dt.Columns.Contains("FlagRichiediStampante") && !oDr.IsNull("FlagRichiediStampante")) _richiediStampante = (bool)oDr["FlagRichiediStampante"];

                }

            }
            catch (Exception)
            {

            }

        }

    }

    [Serializable()]
    public class Entitas
    {

        private int _numero = 0;
        private List<Entita> _elementi = new List<Entita>();

        public Entitas()
        {
            this.Carica();
        }

        public List<Entita> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        private void Carica()
        {
            try
            {
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelEntita", spcoll);

                foreach (DataRow oDr in dt.Rows)
                {
                    _elementi.Add(new Entita(oDr["Codice"].ToString(), oDr["Descrizione"].ToString(), bool.Parse(oDr["AbilitaPermessiDettaglio"].ToString())));
                }

                _numero = _elementi.Count;

            }
            catch (Exception ex)
            {
                _elementi.Clear();
                throw new Exception(ex.Message, ex);
            }

        }

    }

    [Serializable()]
    public class Entita
    {

        private string _codice = string.Empty;
        private string _descrizione = string.Empty;
        private bool _abilitapermessidettaglio = false;
        private Azioni _azioni = null;

        public Entita(string codice, string descrizione, bool abilitapermessidettaglio)
        {
            _codice = codice;
            _descrizione = descrizione;
            _abilitapermessidettaglio = abilitapermessidettaglio;
            _azioni = new Azioni(codice);
        }
        public string Codice
        {
            get { return _codice; }
            set { _codice = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        public bool AbilitaPermessiDettaglio
        {
            get { return _abilitapermessidettaglio; }
            set { _abilitapermessidettaglio = value; }
        }

        public Azioni Azioni
        {
            get { return _azioni; }
            set { _azioni = value; }
        }
    }


    [Serializable()]
    public class Azioni
    {

        private int _numero = 0;
        private List<Azione> _elementi = new List<Azione>();

        public Azioni(string codentita)
        {
            this.Carica(codentita);
        }

        public List<Azione> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        private void Carica(string codentita)
        {
            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodEntita", codentita);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelAzioniEntita", spcoll);

                foreach (DataRow oDr in dt.Rows)
                {
                    _elementi.Add(new Azione(oDr["CodAzione"].ToString(), bool.Parse(oDr["AbilitaPermessiDettaglio"].ToString()), bool.Parse(oDr["RegistraTimeStamp"].ToString())));
                }

                _numero = _elementi.Count;

            }
            catch (Exception ex)
            {
                _elementi.Clear();
                throw new Exception(ex.Message, ex);
            }

        }

    }

    [Serializable()]
    public class Azione
    {

        private string _codice = string.Empty;
        private bool _abilitapermessidettaglio = false;
        private bool _registratimestamp = false;
        public Azione(string codice, bool abilitapermessidettaglio, bool registratimestamp)
        {
            _codice = codice;
            _abilitapermessidettaglio = abilitapermessidettaglio;
            _registratimestamp = registratimestamp;
        }

        public string Codice
        {
            get { return _codice; }
            set { _codice = value; }
        }

        public bool AbilitaPermessiDettaglio
        {
            get { return _abilitapermessidettaglio; }
            set { _abilitapermessidettaglio = value; }
        }

        public bool RegistraTimeStamp
        {
            get { return _registratimestamp; }
            set { _registratimestamp = value; }
        }

    }

    [Serializable()]
    public class Notizie
    {

        private Notizia _notiziaSelezionata = null;
        private List<Notizia> _elementi = new List<Notizia>();

        public Notizie()
        {
        }

        public List<Notizia> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        public Notizia NotiziaSelezionata
        {
            get { return _notiziaSelezionata; }
            set { _notiziaSelezionata = value; }
        }

        public void Aggiorna()
        {
            Carica();
        }

        private void Carica()
        {

            try
            {
                if (_elementi == null)
                    _elementi = new List<Notizia>();
                else
                    _elementi.Clear();

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("DataRif", DateTime.Now.ToString("dd-MM-yyyy"));
                op.Parametro.Add("NumRighe", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordNews));
                op.Parametro.Add("DatiEstesi", "0");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovNews", spcoll);

                foreach (DataRow oDr in dt.Rows)
                {
                    bool rilevante = false;
                    string titolo = "";
                    string testoRTF = "";
                    if (!oDr.IsNull("Rilevante")) rilevante = (bool)oDr["Rilevante"];
                    if (!oDr.IsNull("Titolo")) titolo = oDr["Titolo"].ToString();
                    if (!oDr.IsNull("TestoRTF")) testoRTF = oDr["TestoRTF"].ToString();
                    _elementi.Add(new Notizia((decimal)oDr["ID"], (DateTime)oDr["DataOra"], titolo, testoRTF, rilevante));
                }


            }
            catch (Exception ex)
            {
                _elementi.Clear();
                throw new Exception(ex.Message, ex);
            }

        }

    }

    [Serializable()]
    public class Notizia
    {

        private decimal _ID = 0;
        private DateTime _dataOra;
        private string _titolo = string.Empty;
        private string _testoRTF = string.Empty;
        private bool _rilevante = false;
        private bool _inslog = false;
        private string _codnews = string.Empty;

        public Notizia(decimal ID, DateTime dataOra, string titolo, string testoRTF, bool rilevante)
        {
            _ID = ID;
            _dataOra = dataOra;
            _titolo = titolo;
            _testoRTF = testoRTF;
            _rilevante = rilevante;
            _inslog = false;
            _codnews = string.Empty;
        }
        public Notizia(decimal ID, DateTime dataOra, string titolo, string testoRTF, bool rilevante, bool inslog, string codnews)
        {
            _ID = ID;
            _dataOra = dataOra;
            _titolo = titolo;
            _testoRTF = testoRTF;
            _rilevante = rilevante;
            _inslog = inslog;
            _codnews = codnews;
        }

        public decimal ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public DateTime Data
        {
            get { return _dataOra.Date; }
        }

        public DateTime DataOra
        {
            get { return _dataOra; }
            set { _dataOra = value; }
        }

        public string Titolo
        {
            get { return _titolo; }
            set { _titolo = value; }
        }

        public string TestoRTF
        {
            get { return _testoRTF; }
            set { _testoRTF = value; }
        }

        public bool Rilevante
        {
            get { return _rilevante; }
            set { _rilevante = value; }
        }

        public bool InsLog
        {
            get { return _inslog; }
            set { _inslog = value; }
        }

        public string CodNews
        {
            get { return _codnews; }
            set { _codnews = value; }
        }

    }

    [Serializable()]
    public class ToolboxPerGrafici
    {

        private string _idepisodio = string.Empty;
        private string _idSAC = string.Empty;

        private EnumEntita _entitainiziale = EnumEntita.XXX;
        private string _codtipoiniziale = string.Empty;

        private DateTime _dataricovero = DateTime.MinValue;
        private DateTime _datadainiziale = DateTime.MinValue;
        private DateTime _dataainiziale = DateTime.MinValue;

        private string _campoDataOraPerGrafici = "DataOra";

        private Dictionary<string, TipoParametroVitale> _dictiTipiParametroVitale = null;
        private Dictionary<string, DataTable> _dictDataTablesPerGrafici = null;

        private DataTable _datatableMovimentiLAB = null;

        private DataTable _datatablegriglione = null;

        private List<DateTime> _listadate = new List<DateTime>();

        private DateTime _cachedatada = DateTime.MinValue;
        private DateTime _cachedataa = DateTime.MinValue;
        private DataTable _dtTipiLaboratorio = null;

        public ToolboxPerGrafici(string idepisodio, DateTime dataricovero, string idSAC, EnumEntita entitainiziale, string codtipoiniziale, DateTime datadainiziale, DateTime dataainiziale)
        {
            _entitainiziale = entitainiziale;
            _codtipoiniziale = codtipoiniziale;

            _datadainiziale = datadainiziale;
            _dataainiziale = dataainiziale;
            _dataricovero = dataricovero;

            _idepisodio = idepisodio;
            _idSAC = idSAC;

            _cachedatada = DateTime.MinValue;
            _cachedataa = DateTime.MinValue;

        }

        public string IDEpisodio
        {
            get { return _idepisodio; }
        }

        public DateTime DataRicovero
        {
            get { return _dataricovero; }
        }

        public string CodTipoIniziale
        {
            get { return _codtipoiniziale; }
        }

        public EnumEntita EntitaIniziale
        {
            get { return _entitainiziale; }
        }

        public DateTime DataDaIniziale
        {
            get { return _datadainiziale; }
        }

        public DateTime DataAIniziale
        {
            get { return _dataainiziale; }
        }

        public string IDSAC
        {
            get { return _idSAC; }
        }

        public string CampoDataOraPerGrafici
        {
            get
            {
                if (_campoDataOraPerGrafici == null || _campoDataOraPerGrafici == string.Empty || _campoDataOraPerGrafici.Trim() == "") _campoDataOraPerGrafici = "DataOra";
                return _campoDataOraPerGrafici;
            }
            set { _campoDataOraPerGrafici = value; }
        }

        public Dictionary<string, TipoParametroVitale> TipiParametroVitale
        {
            get { return _dictiTipiParametroVitale; }

        }

        public Dictionary<string, DataTable> DataTablesPerGrafici
        {
            get { return _dictDataTablesPerGrafici; }

        }

        public DataTable DataTableGriglione
        {
            get { return _datatablegriglione; }
        }

        public DataTable DataTableTipiParametriVitali(DateTime datada, DateTime dataa)
        {
            DataTable _datatableTipiParametriVitali = null;

            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

            op.Parametro.Add("IDEpisodio", _idepisodio);

            if (datada > DateTime.MinValue)
                op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(datada));

            if (dataa > DateTime.MinValue)
                op.Parametro.Add("DataFine", Database.dataOra105PerParametri(dataa));

            op.Parametro.Add("DatiEstesi", "1");

            SqlParameterExt[] spcoll = new SqlParameterExt[1];

            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

            _datatableTipiParametriVitali = Database.GetDatasetStoredProc("MSP_SelMovParametriVitali", spcoll).Tables[1];

            return _datatableTipiParametriVitali;
        }

        public DataTable DataTableTipiPrescrizione(DateTime datada, DateTime dataa)
        {

            DataTable _dtTipiPrescrizione = new DataTable();
            _dtTipiPrescrizione.Columns.Add("Codice", typeof(string));
            _dtTipiPrescrizione.Columns.Add("Descrizione", typeof(string));

            try
            {

                SortedList<string, string> slTipoPrescrizione = new SortedList<string, string>();

                DataTable dtTaskInfermieristici = DataTableTaskInfermieristici(datada, dataa);

                if (dtTaskInfermieristici != null)
                {

                    var _result = dtTaskInfermieristici.AsEnumerable()
                        .GroupBy(r1 => new
                        {
                            ID = r1.Field<string>("IDSistema")
                        })
                        .Select(g => new
                        {
                            g.Key.ID
                        });

                    foreach (var dr in _result)
                    {

                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDPrescrizione", dr.ID);
                        op.Parametro.Add("DatiEstesi", "1");

                        SqlParameterExt[] spcoll = new SqlParameterExt[1];

                        string xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        DataSet _ds = Database.GetDatasetStoredProc("MSP_SelMovPrescrizioni", spcoll);

                        if (_ds.Tables.Count > 2 && _ds.Tables[2].Rows.Count == 1 && slTipoPrescrizione.ContainsKey(_ds.Tables[2].Rows[0]["Codice"].ToString()) == false)
                        {
                            slTipoPrescrizione.Add(_ds.Tables[2].Rows[0]["Codice"].ToString(), _ds.Tables[2].Rows[0]["Descrizione"].ToString());
                        }

                    }

                    foreach (KeyValuePair<string, string> pair in slTipoPrescrizione)
                    {

                        DataRow oDr = _dtTipiPrescrizione.NewRow();
                        oDr["Codice"] = pair.Key;
                        oDr["Descrizione"] = pair.Value;
                        _dtTipiPrescrizione.Rows.Add(oDr);

                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return _dtTipiPrescrizione;

        }

        private DataTable DataTableTaskInfermieristici(DateTime datada, DateTime dataa)
        {

            DataTable _dt = null;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("IDEpisodio", _idepisodio);
                if (datada > DateTime.MinValue) { op.Parametro.Add("DataErogazioneInizio", Database.dataOra105PerParametri(datada)); }
                if (dataa > DateTime.MinValue) { op.Parametro.Add("DataErogazioneFine", Database.dataOra105PerParametri(dataa)); }
                op.Parametro.Add("CodTipoTaskInfermieristico", "TDP");
                op.Parametro.Add("CodStatoTaskInfermieristico", "ER");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                _dt = Database.GetDatasetStoredProc("MSP_SelMovTaskInfermieristici", spcoll).Tables[0];

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return _dt;

        }

        public DataTable DataTableTipiLaboratorio(DateTime datada, DateTime dataa)
        {

            try
            {
                if (_datatableMovimentiLAB == null || datada != _cachedatada || dataa != _cachedataa)
                {
                    _cachedatada = datada;
                    _cachedataa = dataa;
                    _datatableMovimentiLAB = DBUtils.getRisultatiLaboratorioPazienteDatatable(_idSAC, datada, dataa);

                    if (_dtTipiLaboratorio != null)
                    {
                        _dtTipiLaboratorio.Dispose();
                        _dtTipiLaboratorio = null;
                    }
                }

                if (_dtTipiLaboratorio == null)
                {
                    _dtTipiLaboratorio = new DataTable();
                    _dtTipiLaboratorio.Columns.Add("CodSezione", typeof(string));
                    _dtTipiLaboratorio.Columns.Add("DescrSezione", typeof(string));
                    _dtTipiLaboratorio.Columns.Add("CodPrescrizione", typeof(string));
                    _dtTipiLaboratorio.Columns.Add("DescPrescrizione", typeof(string));


                    _datatableMovimentiLAB.DefaultView.Sort = "CodSezione, CodPrescrizione";
                    foreach (DataRowView drv in _datatableMovimentiLAB.DefaultView)
                    {
                        try
                        {
                            _dtTipiLaboratorio.DefaultView.RowFilter = @"CodSezione = '" + Database.testoSQL(drv["CodSezione"].ToString()) + @"' And CodPrescrizione = '" + Database.testoSQL(drv["CodPrescrizione"].ToString()) + @"'";
                            if (_dtTipiLaboratorio.DefaultView.Count <= 0)
                            {
                                DataRow drNew = _dtTipiLaboratorio.NewRow();
                                drNew["CodSezione"] = drv["CodSezione"];
                                drNew["DescrSezione"] = drv["DescrSezione"];
                                drNew["CodPrescrizione"] = drv["CodPrescrizione"];
                                drNew["DescPrescrizione"] = drv["DescPrescrizione"];

                                _dtTipiLaboratorio.Rows.Add(drNew);
                            }
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            _dtTipiLaboratorio.DefaultView.RowFilter = "";
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return _dtTipiLaboratorio;
        }

        public void CaricaTuttiIDatiFiltrati(DateTime datada, DateTime dataa, Dictionary<string, string> codicitipiPV, Dictionary<string, string> codicitipiLAB)
        {
            try
            {
                if (_dictDataTablesPerGrafici != null) _dictDataTablesPerGrafici.Clear();
                _dictDataTablesPerGrafici = new Dictionary<string, DataTable>();

                _dictiTipiParametroVitale = null;
                inizializzaDataTablePerGriglione(false);

                if (_listadate != null) _listadate.Clear();
                _listadate = new List<DateTime>();

                if (codicitipiPV != null && codicitipiPV.Count > 0)
                {
                    for (int iPV = 0; iPV < codicitipiPV.Count; iPV++)
                    {
                        if (codicitipiPV.ElementAt(iPV).Key != null && codicitipiPV.ElementAt(iPV).Key != string.Empty && codicitipiPV.ElementAt(iPV).Key.Trim() != "")
                        {
                            string keyperdictionary = EnumEntita.PVT.ToString() + @"|" + codicitipiPV.ElementAt(iPV).Key + @"|" + codicitipiPV.ElementAt(iPV).Value + @"|" + iPV.ToString();
                            DataTable dt = generaDataTablePerGrafico(EnumEntita.PVT, datada, dataa, keyperdictionary);
                            _dictDataTablesPerGrafici.Add(keyperdictionary, dt);
                        }
                    }
                }


                if (codicitipiLAB != null && codicitipiLAB.Count > 0)
                {
                    for (int iLAB = 0; iLAB < codicitipiLAB.Count; iLAB++)
                    {
                        if (codicitipiLAB.ElementAt(iLAB).Key != null && codicitipiLAB.ElementAt(iLAB).Key != string.Empty && codicitipiLAB.ElementAt(iLAB).Key.Trim() != "")
                        {
                            string keyperdictionary = EnumEntita.EVC.ToString() + @"|" + codicitipiLAB.ElementAt(iLAB).Key + @"|" + codicitipiLAB.ElementAt(iLAB).Value + @"|" + iLAB.ToString();
                            DataTable dt = generaDataTablePerGrafico(EnumEntita.EVC, datada, dataa, keyperdictionary);
                            _dictDataTablesPerGrafici.Add(keyperdictionary, dt);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception(@"<CaricaDataTablesDatiPerGrafici>" + Environment.NewLine + ex.Message, ex);
            }
        }

        public DataTable DataTableTipiLaboratorioXAsync(ref DataTable rdtmovimentilab)
        {

            DataTable dtTipiLab = null;
            try
            {

                dtTipiLab = new DataTable();
                dtTipiLab.Columns.Add("CodSezione", typeof(string));
                dtTipiLab.Columns.Add("DescrSezione", typeof(string));
                dtTipiLab.Columns.Add("CodPrescrizione", typeof(string));
                dtTipiLab.Columns.Add("DescPrescrizione", typeof(string));
                dtTipiLab.Columns.Add("UM", typeof(string));

                rdtmovimentilab.DefaultView.Sort = "CodSezione, CodPrescrizione";
                foreach (DataRowView drv in rdtmovimentilab.DefaultView)
                {
                    try
                    {
                        dtTipiLab.DefaultView.RowFilter = @"CodSezione = '" + Database.testoSQL(drv["CodSezione"].ToString()) + @"' And CodPrescrizione = '" + Database.testoSQL(drv["CodPrescrizione"].ToString()) + @"'";
                        if (dtTipiLab.DefaultView.Count <= 0)
                        {
                            DataRow drNew = dtTipiLab.NewRow();
                            drNew["CodSezione"] = drv["CodSezione"];
                            drNew["DescrSezione"] = drv["DescrSezione"];
                            drNew["CodPrescrizione"] = drv["CodPrescrizione"];
                            drNew["DescPrescrizione"] = drv["DescPrescrizione"];
                            drNew["UM"] = drv["UM"];

                            dtTipiLab.Rows.Add(drNew);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        dtTipiLab.DefaultView.RowFilter = "";
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return dtTipiLab;
        }

        public void CaricaTuttiIDatiFiltratiXAsync(ref DataTable rdtmovimentilab, DateTime datada, DateTime dataa,
            Dictionary<string, string> codicitipiPV,
            Dictionary<string, string> codicitipiLAB,
            bool soloLAB)
        {
            try
            {
                if (_dictDataTablesPerGrafici != null) _dictDataTablesPerGrafici.Clear();
                _dictDataTablesPerGrafici = new Dictionary<string, DataTable>();

                _dictiTipiParametroVitale = null;

                inizializzaDataTablePerGriglione(soloLAB);


                if (_listadate != null && !soloLAB) _listadate.Clear();
                if (_listadate == null) _listadate = new List<DateTime>();

                if (codicitipiPV != null && codicitipiPV.Count > 0)
                {
                    for (int iPV = 0; iPV < codicitipiPV.Count; iPV++)
                    {
                        if (codicitipiPV.ElementAt(iPV).Key != null && codicitipiPV.ElementAt(iPV).Key != string.Empty && codicitipiPV.ElementAt(iPV).Key.Trim() != "")
                        {
                            string keyperdictionary = EnumEntita.PVT.ToString() + @"|" + codicitipiPV.ElementAt(iPV).Key + @"|" + codicitipiPV.ElementAt(iPV).Value + @"|" + iPV.ToString();
                            DataTable dt = generaDataTablePerGraficoXAsync(ref rdtmovimentilab, EnumEntita.PVT, datada, dataa, keyperdictionary);
                            _dictDataTablesPerGrafici.Add(keyperdictionary, dt);
                        }
                    }

                }




                if (codicitipiLAB != null && codicitipiLAB.Count > 0)
                {
                    for (int iLAB = 0; iLAB < codicitipiLAB.Count; iLAB++)
                    {
                        if (codicitipiLAB.ElementAt(iLAB).Key != null && codicitipiLAB.ElementAt(iLAB).Key != string.Empty && codicitipiLAB.ElementAt(iLAB).Key.Trim() != "")
                        {
                            string keyperdictionary = EnumEntita.EVC.ToString() + @"|" + codicitipiLAB.ElementAt(iLAB).Key + @"|" + codicitipiLAB.ElementAt(iLAB).Value + @"|" + iLAB.ToString();
                            DataTable dt = generaDataTablePerGraficoXAsync(ref rdtmovimentilab, EnumEntita.EVC, datada, dataa, keyperdictionary);
                            _dictDataTablesPerGrafici.Add(keyperdictionary, dt);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception(@"<CaricaDataTablesDatiPerGrafici>" + Environment.NewLine + ex.Message, ex);
            }
        }

        private DataTable generaDataTablePerGraficoXAsync(ref DataTable rdtmovimentilab, EnumEntita entita, DateTime datada, DateTime dataa, string keyfordictionary)
        {
            try
            {
                DataTable dtReturn = null;
                string nomecampodataoraminsec = "";
                string nomecampovalori = "";

                string[] arrkey = keyfordictionary.Split('|');

                string codentita = "";
                string descentita = "";
                string codtipo = "";
                string desctipo = "";
                string codprescrizionelab = "";
                string descprescrizionelab = "";

                codentita = arrkey[0];

                if (codentita == EnumEntita.PVT.ToString())
                {
                    descentita = "Parametri Vitali";

                    codtipo = arrkey[1];
                    desctipo = arrkey[2];

                }
                else if (codentita == EnumEntita.EVC.ToString())
                {
                    descentita = "Laboratorio";
                    codtipo = arrkey[1];
                    desctipo = arrkey[3];

                    codprescrizionelab = arrkey[2];
                    descprescrizionelab = arrkey[4];
                }
                else if (codentita == EnumEntita.WKI.ToString())
                {
                    descentita = "Terapia";

                    codtipo = arrkey[1];
                    desctipo = arrkey[2];

                }

                dtReturn = new DataTable();
                DataColumn oDc = new DataColumn(this.CampoDataOraPerGrafici, typeof(DateTime));
                dtReturn.Columns.Add(oDc);

                if (entita == EnumEntita.PVT)
                {

                    DataTable datatableMovimentiParametriVitali = null;

                    nomecampodataoraminsec = @"DataEvento";
                    nomecampovalori = @"ValoriGrafici";


                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodTipoParametroVitale", codtipo);
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataTable dtDimensioni = Database.GetDataTableStoredProc("MSP_SelInfoParametroVitale", spcoll);
                    if (dtDimensioni != null)
                    {
                        if (dtDimensioni.Rows.Count == 1 && !dtDimensioni.Rows[0].IsNull("CampiGrafici") && dtDimensioni.Rows[0]["CampiGrafici"].ToString() != "")
                        {
                            TipoParametroVitale tpvg = new TipoParametroVitale();
                            tpvg = XmlProcs.XmlDeserializeFromString<TipoParametroVitale>(dtDimensioni.Rows[0]["CampiGrafici"].ToString());
                            foreach (DimensionePerGrafico dpg in tpvg.DimensioniPerGrafico)
                            {
                                oDc = new DataColumn(dpg.Nome, typeof(float));
                                dtReturn.Columns.Add(oDc);
                            }
                            if (_dictiTipiParametroVitale == null) _dictiTipiParametroVitale = new Dictionary<string, TipoParametroVitale>();
                            if (!_dictiTipiParametroVitale.ContainsKey(keyfordictionary)) _dictiTipiParametroVitale.Add(keyfordictionary, tpvg);
                        }
                        dtDimensioni.Dispose();
                    }




                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                    op.Parametro.Add("IDEpisodio", _idepisodio);
                    op.Parametro.Add("CodTipoParametroVitale", codtipo);
                    if (datada > DateTime.MinValue) op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(datada));
                    if (dataa > DateTime.MinValue) op.Parametro.Add("DataFine", Database.dataOra105PerParametri(dataa));
                    op.Parametro.Add("DatiEstesi", "0");
                    op.Parametro.Add("VisualizzazioneGrafici", "1");

                    op.Parametro.Add("CodStatoParametroVitale", EnumStatoParametroVitale.ER.ToString());

                    op.TimeStamp.CodEntita = EnumEntita.PVT.ToString();
                    op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    datatableMovimentiParametriVitali = Database.GetDataTableStoredProc("MSP_SelMovParametriVitali", spcoll);

                    if (datatableMovimentiParametriVitali != null && datatableMovimentiParametriVitali.Rows.Count > 0)
                    {

                        string sRowFilter = "";
                        if (sRowFilter.Trim() != "") sRowFilter += @" AND ";
                        sRowFilter += @"(" + nomecampovalori + @" Is Not Null AND " + nomecampovalori + @" <> '')";
                        datatableMovimentiParametriVitali.DefaultView.RowFilter = sRowFilter;

                        if (datatableMovimentiParametriVitali.DefaultView.Count > 0)
                        {


                            datatableMovimentiParametriVitali.DefaultView.Sort = nomecampodataoraminsec + @" ASC";
                            foreach (DataRowView drv in datatableMovimentiParametriVitali.DefaultView)
                            {
                                try
                                {
                                    if (!drv.Row.IsNull(nomecampovalori) && drv[nomecampovalori].ToString().Trim() != "")
                                    {

                                        bool bNewRow = false;
                                        DataRow drDatiPerGrafico = null;
                                        DateTime dataoraminutisec = (DateTime)drv[nomecampodataoraminsec];
                                        dataoraminutisec = new DateTime(dataoraminutisec.Year, dataoraminutisec.Month, dataoraminutisec.Day, dataoraminutisec.Hour, dataoraminutisec.Minute, dataoraminutisec.Second);
                                        for (int iFind = 0; iFind < dtReturn.Rows.Count; iFind++)
                                        {
                                            if ((DateTime)dtReturn.Rows[iFind][this.CampoDataOraPerGrafici] == dataoraminutisec)
                                            {
                                                drDatiPerGrafico = dtReturn.Rows[iFind];
                                                iFind = dtReturn.Rows.Count;
                                            }
                                        }
                                        if (drDatiPerGrafico == null)
                                        {
                                            drDatiPerGrafico = dtReturn.NewRow();
                                            drDatiPerGrafico[this.CampoDataOraPerGrafici] = dataoraminutisec;
                                            bNewRow = true;
                                        }


                                        string nomecolonnadataoraminsecgriglione = dataoraminutisec.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");
                                        if (!_listadate.Contains(dataoraminutisec))
                                        {
                                            _listadate.Add(dataoraminutisec);
                                            _listadate.Sort();

                                            DataColumn coldataora = new DataColumn(nomecolonnadataoraminsecgriglione, typeof(string));
                                            coldataora.AllowDBNull = true;
                                            coldataora.Unique = false;
                                            _datatablegriglione.Columns.Add(coldataora);
                                            _datatablegriglione.Columns[nomecolonnadataoraminsecgriglione].SetOrdinal(6 + _listadate.IndexOf(dataoraminutisec));
                                        }


                                        try
                                        {
                                            string xmlvalori = drv[nomecampovalori].ToString();
                                            ValoriPVT valori = XmlProcs.XmlDeserializeFromString<ValoriPVT>(xmlvalori);
                                            foreach (ValorePVT valpg in valori.Valori)
                                            {
                                                string dimensione = valpg.Nome;

                                                if (dtReturn.Columns.Contains(dimensione))
                                                {

                                                    if (!bNewRow && !drDatiPerGrafico.IsNull(dimensione))
                                                    {
                                                        bool bFoundNewDateTime = false;
                                                        while (!bFoundNewDateTime)
                                                        {
                                                            dataoraminutisec = dataoraminutisec.AddSeconds(1);
                                                            bFoundNewDateTime = true;

                                                            for (int iFind = 0; iFind < dtReturn.Rows.Count; iFind++)
                                                            {
                                                                if ((DateTime)dtReturn.Rows[iFind][this.CampoDataOraPerGrafici] == dataoraminutisec)
                                                                {
                                                                    if (!dtReturn.Rows[iFind].IsNull(dimensione)) bFoundNewDateTime = false;

                                                                    iFind = dtReturn.Rows.Count;

                                                                }
                                                            }
                                                        }

                                                        drDatiPerGrafico = dtReturn.NewRow();
                                                        drDatiPerGrafico[this.CampoDataOraPerGrafici] = dataoraminutisec;
                                                        bNewRow = true;

                                                        nomecolonnadataoraminsecgriglione = dataoraminutisec.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");
                                                        if (!_listadate.Contains(dataoraminutisec))
                                                        {

                                                            _listadate.Add(dataoraminutisec);
                                                            _listadate.Sort();

                                                            DataColumn coldataora = new DataColumn(nomecolonnadataoraminsecgriglione, typeof(string));
                                                            coldataora.AllowDBNull = true;
                                                            coldataora.Unique = false;
                                                            _datatablegriglione.Columns.Add(coldataora);
                                                            _datatablegriglione.Columns[nomecolonnadataoraminsecgriglione].SetOrdinal(6 + _listadate.IndexOf(dataoraminutisec));
                                                        }

                                                    }


                                                    float valore = 0;
                                                    if (float.TryParse(valpg.Valore.Replace(".", ","), out valore))
                                                        drDatiPerGrafico[dimensione] = valore;
                                                    else
                                                        drDatiPerGrafico[dimensione] = System.DBNull.Value;


                                                    try
                                                    {
                                                        bool baddrigadimensione = true;
                                                        for (int iRO = 0; iRO < _datatablegriglione.Rows.Count; iRO++)
                                                        {
                                                            if (_datatablegriglione.Rows[iRO]["CodEntita"].ToString().ToUpper() == codentita.ToUpper()
                                                            && _datatablegriglione.Rows[iRO]["CodTipo"].ToString().ToUpper() == codtipo.ToUpper()
                                                            && _datatablegriglione.Rows[iRO]["Dimensione"].ToString().ToUpper() == dimensione.ToUpper())
                                                            {
                                                                baddrigadimensione = false;

                                                                _datatablegriglione.Rows[iRO][nomecolonnadataoraminsecgriglione] = drDatiPerGrafico[dimensione];

                                                                iRO = _datatablegriglione.Rows.Count + 1;
                                                            }
                                                        }
                                                        if (baddrigadimensione)
                                                        {
                                                            DataRow drGriglioneNew = _datatablegriglione.NewRow();

                                                            drGriglioneNew["CodEntita"] = codentita;
                                                            drGriglioneNew["Entita"] = descentita;
                                                            drGriglioneNew["CodTipo"] = codtipo;
                                                            drGriglioneNew["DescTipo"] = desctipo;
                                                            drGriglioneNew["Dimensione"] = dimensione;
                                                            drGriglioneNew[nomecolonnadataoraminsecgriglione] = drDatiPerGrafico[dimensione];

                                                            _datatablegriglione.Rows.Add(drGriglioneNew);
                                                        }

                                                    }
                                                    catch (Exception)
                                                    {
                                                    }

                                                }



                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        if (bNewRow) dtReturn.Rows.Add(drDatiPerGrafico);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }

                        }

                        datatableMovimentiParametriVitali.DefaultView.RowFilter = "";

                    }

                }
                else if (entita == EnumEntita.EVC)
                {


                    nomecampodataoraminsec = @"Data";
                    nomecampovalori = @"Quantita";

                    oDc = new DataColumn(descprescrizionelab, typeof(float));
                    dtReturn.Columns.Add(oDc);

                    if (rdtmovimentilab != null && rdtmovimentilab.Rows.Count > 0)
                    {

                        string sRowFilter = "";
                        if (sRowFilter.Trim() != "") sRowFilter += @" AND ";
                        sRowFilter += @"(" + nomecampovalori + @" Is Not Null)";
                        sRowFilter += @" AND (CodPrescrizione = '" + Database.testoSQL(codprescrizionelab) + @"')";
                        rdtmovimentilab.DefaultView.RowFilter = sRowFilter;

                        if (rdtmovimentilab.DefaultView.Count > 0)
                        {


                            rdtmovimentilab.DefaultView.Sort = nomecampodataoraminsec + @" ASC";
                            foreach (DataRowView drv in rdtmovimentilab.DefaultView)
                            {
                                try
                                {
                                    if (!drv.Row.IsNull(nomecampovalori) && drv[nomecampovalori].ToString().Trim() != "")
                                    {

                                        bool bNewRow = false;
                                        DataRow drDatiPerGrafico = null;
                                        DateTime dataoraminutisec = (DateTime)drv[nomecampodataoraminsec];
                                        dataoraminutisec = new DateTime(dataoraminutisec.Year, dataoraminutisec.Month, dataoraminutisec.Day, dataoraminutisec.Hour, dataoraminutisec.Minute, dataoraminutisec.Second);
                                        for (int iFind = 0; iFind < dtReturn.Rows.Count; iFind++)
                                        {
                                            if ((DateTime)dtReturn.Rows[iFind][this.CampoDataOraPerGrafici] == dataoraminutisec)
                                            {
                                                drDatiPerGrafico = dtReturn.Rows[iFind];
                                                iFind = dtReturn.Rows.Count;
                                            }
                                        }
                                        if (drDatiPerGrafico == null)
                                        {
                                            drDatiPerGrafico = dtReturn.NewRow();
                                            drDatiPerGrafico[this.CampoDataOraPerGrafici] = dataoraminutisec;
                                            bNewRow = true;
                                        }


                                        string nomecolonnadataoraminsecgriglione = dataoraminutisec.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");
                                        if (!_listadate.Contains(dataoraminutisec))
                                        {
                                            _listadate.Add(dataoraminutisec);
                                            _listadate.Sort();

                                            DataColumn coldataora = new DataColumn(nomecolonnadataoraminsecgriglione, typeof(string));
                                            coldataora.AllowDBNull = true;
                                            coldataora.Unique = false;
                                            _datatablegriglione.Columns.Add(coldataora);
                                            _datatablegriglione.Columns[nomecolonnadataoraminsecgriglione].SetOrdinal(6 + _listadate.IndexOf(dataoraminutisec));
                                        }


                                        try
                                        {


                                            if (!bNewRow && !drDatiPerGrafico.IsNull(descprescrizionelab))
                                            {
                                                bool bFoundNewDateTime = false;
                                                while (!bFoundNewDateTime)
                                                {
                                                    dataoraminutisec = dataoraminutisec.AddSeconds(1);
                                                    bFoundNewDateTime = true;

                                                    for (int iFind = 0; iFind < dtReturn.Rows.Count; iFind++)
                                                    {
                                                        if ((DateTime)dtReturn.Rows[iFind][this.CampoDataOraPerGrafici] == dataoraminutisec)
                                                        {
                                                            if (!dtReturn.Rows[iFind].IsNull(descprescrizionelab)) bFoundNewDateTime = false;

                                                            iFind = dtReturn.Rows.Count;

                                                        }
                                                    }
                                                }

                                                drDatiPerGrafico = dtReturn.NewRow();
                                                drDatiPerGrafico[this.CampoDataOraPerGrafici] = dataoraminutisec;
                                                bNewRow = true;

                                                nomecolonnadataoraminsecgriglione = dataoraminutisec.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");
                                                if (!_listadate.Contains(dataoraminutisec))
                                                {

                                                    _listadate.Add(dataoraminutisec);
                                                    _listadate.Sort();

                                                    DataColumn coldataora = new DataColumn(nomecolonnadataoraminsecgriglione, typeof(string));
                                                    coldataora.AllowDBNull = true;
                                                    coldataora.Unique = false;
                                                    _datatablegriglione.Columns.Add(coldataora);
                                                    _datatablegriglione.Columns[nomecolonnadataoraminsecgriglione].SetOrdinal(6 + _listadate.IndexOf(dataoraminutisec));
                                                }
                                            }

                                            if (!(bool)drv["RisultatoNumericoAssente"])
                                            {
                                                drDatiPerGrafico[descprescrizionelab] = drv[nomecampovalori];
                                            }

                                            try
                                            {
                                                bool baddrigadimensione = true;
                                                for (int iRO = 0; iRO < _datatablegriglione.Rows.Count; iRO++)
                                                {
                                                    if (_datatablegriglione.Rows[iRO]["CodEntita"].ToString().ToUpper() == codentita.ToUpper()
                                                    && _datatablegriglione.Rows[iRO]["CodTipo"].ToString().ToUpper() == codtipo.ToUpper()
                                                    && _datatablegriglione.Rows[iRO]["CodDimensione"].ToString().ToUpper() == codprescrizionelab.ToUpper())
                                                    {
                                                        baddrigadimensione = false;

                                                        if ((bool)drv["RisultatoNumericoAssente"])
                                                        {
                                                            _datatablegriglione.Rows[iRO][nomecolonnadataoraminsecgriglione] = drv["Risultato"].ToString();
                                                        }
                                                        else
                                                        {
                                                            _datatablegriglione.Rows[iRO][nomecolonnadataoraminsecgriglione] = drv[nomecampovalori];
                                                        }

                                                        iRO = _datatablegriglione.Rows.Count + 1;
                                                    }
                                                }
                                                if (baddrigadimensione)
                                                {
                                                    DataRow drGriglioneNew = _datatablegriglione.NewRow();

                                                    drGriglioneNew["CodEntita"] = codentita;
                                                    drGriglioneNew["Entita"] = descentita;
                                                    drGriglioneNew["CodTipo"] = codtipo;
                                                    drGriglioneNew["DescTipo"] = desctipo;
                                                    drGriglioneNew["CodDimensione"] = codprescrizionelab;
                                                    drGriglioneNew["Dimensione"] = descprescrizionelab;
                                                    if ((bool)drv["RisultatoNumericoAssente"])
                                                    {
                                                        drGriglioneNew[nomecolonnadataoraminsecgriglione] = drv["Risultato"].ToString();
                                                    }
                                                    else
                                                    {
                                                        drGriglioneNew[nomecolonnadataoraminsecgriglione] = drv[nomecampovalori];
                                                    }

                                                    _datatablegriglione.Rows.Add(drGriglioneNew);
                                                }

                                            }
                                            catch (Exception)
                                            {
                                            }

                                        }
                                        catch (Exception)
                                        {
                                        }

                                        if (bNewRow) dtReturn.Rows.Add(drDatiPerGrafico);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }

                        }

                        rdtmovimentilab.DefaultView.RowFilter = "";

                    }

                }

                return dtReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(@"<DataTableDatiPerGrafico>" + Environment.NewLine + ex.Message, ex);
            }
        }

        internal List<MSP_SelMovSommGraficoExt> GetSelMovSomministrazioniGraficoExt(DateTime datada, DateTime dataa)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Ambiente.Idpaziente);
                op.Parametro.Add("DatiEstesi", "1");
                op.Parametro.Add("CodStatoTaskInfermieristico", EnumStatoTaskInfermieristico.ER.ToString());
                if (datada > DateTime.MinValue) op.Parametro.Add("DataErogazioneInizio", Database.dataOra105PerParametri(datada));
                if (dataa > DateTime.MinValue) op.Parametro.Add("DataErogazioneFine", Database.dataOra105PerParametri(dataa));

                op.TimeStamp.CodEntita = EnumEntita.WKI.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                string xmlParam = XmlProcs.XmlSerializeToString(op);

                FwDataParametersList procParams = new FwDataParametersList();
                procParams.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                FwDataBufferedList<MSP_SelMovSommGraficoExt> res = null;

                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                {
                    res = conn.Query<FwDataBufferedList<MSP_SelMovSommGraficoExt>>("MSP_SelMovSomministrazioniGrafico", procParams, CommandType.StoredProcedure);
                }

                return res.Buffer;

            }
            catch (Exception ex)
            {
                throw new Exception(@"<DataTableDatiPerGrafico>" + Environment.NewLine + ex.Message, ex);
            }

        }

        internal DataTable DataTableSelMovSomministrazioniGrafico(DateTime datada, DateTime dataa, List<String> idPrescrizioni)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Ambiente.Idpaziente);
                op.Parametro.Add("DatiEstesi", "0");
                op.Parametro.Add("CodStatoTaskInfermieristico", EnumStatoTaskInfermieristico.ER.ToString());
                if (datada > DateTime.MinValue) op.Parametro.Add("DataErogazioneInizio", Database.dataOra105PerParametri(datada));
                if (dataa > DateTime.MinValue) op.Parametro.Add("DataErogazioneFine", Database.dataOra105PerParametri(dataa));

                if ((idPrescrizioni != null) && (idPrescrizioni.Count > 0))
                    op.ParametroRipetibile.Add("IDPrescrizione", idPrescrizioni.ToArray());

                op.TimeStamp.CodEntita = EnumEntita.WKI.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dtReturn = Database.GetDataTableStoredProc("MSP_SelMovSomministrazioniGrafico", spcoll);


                return dtReturn;

            }
            catch (Exception ex)
            {
                throw new Exception(@"<DataTableDatiPerGrafico>" + Environment.NewLine + ex.Message, ex);
            }

        }

        private void inizializzaDataTablePerGriglione(bool soloLAB)
        {
            try
            {
                if (_datatablegriglione != null)
                {
                    if (soloLAB)
                    {

                        for (int iRow = _datatablegriglione.Rows.Count - 1; iRow >= 0; iRow--)
                        {
                            try
                            {
                                if (_datatablegriglione.Rows[iRow]["CodEntita"].ToString() == EnumEntita.EVC.ToString())
                                    _datatablegriglione.Rows[iRow].Delete();
                            }
                            catch
                            {
                            }
                        }

                        for (int iCol = _datatablegriglione.Columns.Count - 1; iCol > 5; iCol--)
                        {
                            try
                            {
                                bool bDel = true;
                                for (int iRow = 0; iRow < _datatablegriglione.Rows.Count; iRow++)
                                {
                                    if (_datatablegriglione.Rows[iRow].RowState != DataRowState.Deleted && !_datatablegriglione.Rows[iRow].IsNull(iCol))
                                    {
                                        bDel = false;
                                        iRow = _datatablegriglione.Rows.Count;
                                    }
                                }
                                if (bDel)
                                {
                                    if (_listadate != null && _listadate.Count > 0)
                                    {
                                        for (int idom = _listadate.Count - 1; idom >= 0; idom--)
                                        {
                                            if (_listadate[idom].ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":") == _datatablegriglione.Columns[iCol].ColumnName) _listadate.RemoveAt(idom);
                                        }
                                    }
                                    _datatablegriglione.Columns.RemoveAt(iCol);
                                }
                            }
                            catch
                            {
                            }
                        }

                    }
                    else
                    {

                        _datatablegriglione.Dispose();
                        _datatablegriglione = null;

                    }

                }




                if (_datatablegriglione == null)
                {
                    _datatablegriglione = new DataTable();

                    DataColumn col = new DataColumn("CodEntita", typeof(string));
                    col.AllowDBNull = false;
                    col.Unique = false;
                    col.MaxLength = 20;
                    col.DefaultValue = "";
                    _datatablegriglione.Columns.Add(col);
                    col = new DataColumn("Entita", typeof(string));
                    col.AllowDBNull = true;
                    col.Unique = false;
                    col.MaxLength = 255;
                    col.DefaultValue = "";
                    _datatablegriglione.Columns.Add(col);
                    col = new DataColumn("CodTipo", typeof(string));
                    col.AllowDBNull = false;
                    col.Unique = false;
                    col.MaxLength = 100;
                    col.DefaultValue = "";
                    _datatablegriglione.Columns.Add(col);
                    col = new DataColumn("DescTipo", typeof(string));
                    col.AllowDBNull = true;
                    col.Unique = false;
                    col.MaxLength = 500;
                    col.DefaultValue = "";
                    _datatablegriglione.Columns.Add(col);
                    col = new DataColumn("CodDimensione", typeof(string));
                    col.AllowDBNull = false;
                    col.Unique = false;
                    col.MaxLength = 20;
                    col.DefaultValue = "";
                    _datatablegriglione.Columns.Add(col);
                    col = new DataColumn("Dimensione", typeof(string));
                    col.AllowDBNull = true;
                    col.Unique = false;
                    col.MaxLength = 255;
                    col.DefaultValue = "";
                    _datatablegriglione.Columns.Add(col);

                }

            }
            catch (Exception ex)
            {
                throw new Exception(@"<inizializzaDataTablePerGriglione>" + Environment.NewLine + ex.Message, ex);
            }
        }

        private DataTable generaDataTablePerGrafico(EnumEntita entita, DateTime datada, DateTime dataa, string keyfordictionary)
        {
            try
            {
                DataTable dtReturn = null;
                string nomecampodataoraminsec = "";
                string nomecampovalori = "";

                string[] arrkey = keyfordictionary.Split('|');

                string codentita = "";
                string descentita = "";
                string codtipo = "";
                string desctipo = "";
                string codprescrizionelab = "";
                string descprescrizionelab = "";

                codentita = arrkey[0];

                if (codentita == EnumEntita.PVT.ToString())
                {
                    descentita = "Parametri Vitali";

                    codtipo = arrkey[1];
                    desctipo = arrkey[2];

                }
                else if (codentita == EnumEntita.EVC.ToString())
                {
                    descentita = "Laboratorio";
                    codtipo = arrkey[1];
                    desctipo = arrkey[3];

                    codprescrizionelab = arrkey[2];
                    descprescrizionelab = arrkey[4];
                }


                dtReturn = new DataTable();
                DataColumn oDc = new DataColumn(this.CampoDataOraPerGrafici, typeof(DateTime));
                dtReturn.Columns.Add(oDc);

                if (entita == EnumEntita.PVT)
                {

                    DataTable datatableMovimentiParametriVitali = null;

                    nomecampodataoraminsec = @"DataEvento";
                    nomecampovalori = @"ValoriGrafici";


                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodTipoParametroVitale", codtipo);
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataTable dtDimensioni = Database.GetDataTableStoredProc("MSP_SelInfoParametroVitale", spcoll);
                    if (dtDimensioni != null)
                    {
                        if (dtDimensioni.Rows.Count == 1 && !dtDimensioni.Rows[0].IsNull("CampiGrafici") && dtDimensioni.Rows[0]["CampiGrafici"].ToString() != "")
                        {
                            TipoParametroVitale tpvg = new TipoParametroVitale();
                            tpvg = XmlProcs.XmlDeserializeFromString<TipoParametroVitale>(dtDimensioni.Rows[0]["CampiGrafici"].ToString());
                            foreach (DimensionePerGrafico dpg in tpvg.DimensioniPerGrafico)
                            {
                                oDc = new DataColumn(dpg.Nome, typeof(float));
                                dtReturn.Columns.Add(oDc);
                            }
                            if (_dictiTipiParametroVitale == null) _dictiTipiParametroVitale = new Dictionary<string, TipoParametroVitale>();
                            if (!_dictiTipiParametroVitale.ContainsKey(keyfordictionary)) _dictiTipiParametroVitale.Add(keyfordictionary, tpvg);
                        }
                        dtDimensioni.Dispose();
                    }




                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                    op.Parametro.Add("IDEpisodio", _idepisodio);
                    op.Parametro.Add("CodTipoParametroVitale", codtipo);
                    if (datada > DateTime.MinValue) op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(datada));
                    if (dataa > DateTime.MinValue) op.Parametro.Add("DataFine", Database.dataOra105PerParametri(dataa));
                    op.Parametro.Add("DatiEstesi", "0");
                    op.Parametro.Add("VisualizzazioneGrafici", "1");

                    op.Parametro.Add("CodStatoParametroVitale", "ER");

                    op.TimeStamp.CodEntita = EnumEntita.PVT.ToString();
                    op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    datatableMovimentiParametriVitali = Database.GetDataTableStoredProc("MSP_SelMovParametriVitali", spcoll);

                    if (datatableMovimentiParametriVitali != null && datatableMovimentiParametriVitali.Rows.Count > 0)
                    {

                        string sRowFilter = "";
                        if (sRowFilter.Trim() != "") sRowFilter += @" AND ";
                        sRowFilter += @"(" + nomecampovalori + @" Is Not Null AND " + nomecampovalori + @" <> '')";
                        datatableMovimentiParametriVitali.DefaultView.RowFilter = sRowFilter;

                        if (datatableMovimentiParametriVitali.DefaultView.Count > 0)
                        {


                            datatableMovimentiParametriVitali.DefaultView.Sort = nomecampodataoraminsec + @" ASC";
                            foreach (DataRowView drv in datatableMovimentiParametriVitali.DefaultView)
                            {
                                try
                                {
                                    if (!drv.Row.IsNull(nomecampovalori) && drv[nomecampovalori].ToString().Trim() != "")
                                    {

                                        bool bNewRow = false;
                                        DataRow drDatiPerGrafico = null;
                                        DateTime dataoraminutisec = (DateTime)drv[nomecampodataoraminsec];
                                        dataoraminutisec = new DateTime(dataoraminutisec.Year, dataoraminutisec.Month, dataoraminutisec.Day, dataoraminutisec.Hour, dataoraminutisec.Minute, dataoraminutisec.Second);
                                        for (int iFind = 0; iFind < dtReturn.Rows.Count; iFind++)
                                        {
                                            if ((DateTime)dtReturn.Rows[iFind][this.CampoDataOraPerGrafici] == dataoraminutisec)
                                            {
                                                drDatiPerGrafico = dtReturn.Rows[iFind];
                                                iFind = dtReturn.Rows.Count;
                                            }
                                        }
                                        if (drDatiPerGrafico == null)
                                        {
                                            drDatiPerGrafico = dtReturn.NewRow();
                                            drDatiPerGrafico[this.CampoDataOraPerGrafici] = dataoraminutisec;
                                            bNewRow = true;
                                        }


                                        string nomecolonnadataoraminsecgriglione = dataoraminutisec.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");
                                        if (!_listadate.Contains(dataoraminutisec))
                                        {
                                            _listadate.Add(dataoraminutisec);
                                            _listadate.Sort();

                                            DataColumn coldataora = new DataColumn(nomecolonnadataoraminsecgriglione, typeof(string));
                                            coldataora.AllowDBNull = true;
                                            coldataora.Unique = false;
                                            _datatablegriglione.Columns.Add(coldataora);
                                            _datatablegriglione.Columns[nomecolonnadataoraminsecgriglione].SetOrdinal(6 + _listadate.IndexOf(dataoraminutisec));
                                        }


                                        try
                                        {
                                            string xmlvalori = drv[nomecampovalori].ToString();
                                            ValoriPVT valori = XmlProcs.XmlDeserializeFromString<ValoriPVT>(xmlvalori);
                                            foreach (ValorePVT valpg in valori.Valori)
                                            {
                                                string dimensione = valpg.Nome;

                                                if (dtReturn.Columns.Contains(dimensione))
                                                {

                                                    if (!bNewRow && !drDatiPerGrafico.IsNull(dimensione))
                                                    {
                                                        bool bFoundNewDateTime = false;
                                                        while (!bFoundNewDateTime)
                                                        {
                                                            dataoraminutisec = dataoraminutisec.AddSeconds(1);
                                                            bFoundNewDateTime = true;

                                                            for (int iFind = 0; iFind < dtReturn.Rows.Count; iFind++)
                                                            {
                                                                if ((DateTime)dtReturn.Rows[iFind][this.CampoDataOraPerGrafici] == dataoraminutisec)
                                                                {
                                                                    if (!dtReturn.Rows[iFind].IsNull(dimensione)) bFoundNewDateTime = false;

                                                                    iFind = dtReturn.Rows.Count;

                                                                }
                                                            }
                                                        }

                                                        drDatiPerGrafico = dtReturn.NewRow();
                                                        drDatiPerGrafico[this.CampoDataOraPerGrafici] = dataoraminutisec;
                                                        bNewRow = true;

                                                        nomecolonnadataoraminsecgriglione = dataoraminutisec.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");
                                                        if (!_listadate.Contains(dataoraminutisec))
                                                        {

                                                            _listadate.Add(dataoraminutisec);
                                                            _listadate.Sort();

                                                            DataColumn coldataora = new DataColumn(nomecolonnadataoraminsecgriglione, typeof(string));
                                                            coldataora.AllowDBNull = true;
                                                            coldataora.Unique = false;
                                                            _datatablegriglione.Columns.Add(coldataora);
                                                            _datatablegriglione.Columns[nomecolonnadataoraminsecgriglione].SetOrdinal(6 + _listadate.IndexOf(dataoraminutisec));
                                                        }

                                                    }

                                                    float valore = 0;
                                                    if (float.TryParse(valpg.Valore.Replace(".", ","), out valore))
                                                        drDatiPerGrafico[dimensione] = valore;
                                                    else
                                                        drDatiPerGrafico[dimensione] = System.DBNull.Value;


                                                    try
                                                    {
                                                        bool baddrigadimensione = true;
                                                        for (int iRO = 0; iRO < _datatablegriglione.Rows.Count; iRO++)
                                                        {
                                                            if (_datatablegriglione.Rows[iRO]["CodEntita"].ToString().ToUpper() == codentita.ToUpper()
                                                            && _datatablegriglione.Rows[iRO]["CodTipo"].ToString().ToUpper() == codtipo.ToUpper()
                                                            && _datatablegriglione.Rows[iRO]["Dimensione"].ToString().ToUpper() == dimensione.ToUpper())
                                                            {
                                                                baddrigadimensione = false;

                                                                _datatablegriglione.Rows[iRO][nomecolonnadataoraminsecgriglione] = drDatiPerGrafico[dimensione];

                                                                iRO = _datatablegriglione.Rows.Count + 1;
                                                            }
                                                        }
                                                        if (baddrigadimensione)
                                                        {
                                                            DataRow drGriglioneNew = _datatablegriglione.NewRow();

                                                            drGriglioneNew["CodEntita"] = codentita;
                                                            drGriglioneNew["Entita"] = descentita;
                                                            drGriglioneNew["CodTipo"] = codtipo;
                                                            drGriglioneNew["DescTipo"] = desctipo;
                                                            drGriglioneNew["CodDimensione"] = "";
                                                            drGriglioneNew["Dimensione"] = dimensione;
                                                            drGriglioneNew[nomecolonnadataoraminsecgriglione] = drDatiPerGrafico[dimensione];

                                                            _datatablegriglione.Rows.Add(drGriglioneNew);
                                                        }

                                                    }
                                                    catch (Exception)
                                                    {
                                                    }

                                                }



                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        if (bNewRow) dtReturn.Rows.Add(drDatiPerGrafico);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }

                        }

                        datatableMovimentiParametriVitali.DefaultView.RowFilter = "";

                    }

                }
                else if (entita == EnumEntita.EVC)
                {



                    if (_datatableMovimentiLAB == null || _cachedatada != datada || _cachedataa != dataa)
                    {
                        _datatableMovimentiLAB = DBUtils.getRisultatiLaboratorioPazienteDatatable(_idSAC, datada, dataa);
                        _cachedatada = datada;
                        _cachedataa = dataa;


                        if (_dtTipiLaboratorio != null)
                        {
                            _dtTipiLaboratorio.Dispose();
                            _dtTipiLaboratorio = null;
                        }
                    }

                    nomecampodataoraminsec = @"Data";
                    nomecampovalori = @"Quantita";

                    oDc = new DataColumn(descprescrizionelab, typeof(float));
                    dtReturn.Columns.Add(oDc);

                    if (_datatableMovimentiLAB != null && _datatableMovimentiLAB.Rows.Count > 0)
                    {

                        string sRowFilter = "";
                        if (sRowFilter.Trim() != "") sRowFilter += @" AND ";
                        sRowFilter += @"(" + nomecampovalori + @" Is Not Null)";
                        sRowFilter += @" AND (CodSezione = '" + Database.testoSQL(codtipo) + @"' AND CodPrescrizione = '" + Database.testoSQL(codprescrizionelab) + @"')";
                        _datatableMovimentiLAB.DefaultView.RowFilter = sRowFilter;

                        if (_datatableMovimentiLAB.DefaultView.Count > 0)
                        {


                            _datatableMovimentiLAB.DefaultView.Sort = nomecampodataoraminsec + @" ASC";
                            foreach (DataRowView drv in _datatableMovimentiLAB.DefaultView)
                            {
                                try
                                {
                                    if (!drv.Row.IsNull(nomecampovalori) && drv[nomecampovalori].ToString().Trim() != "")
                                    {

                                        bool bNewRow = false;
                                        DataRow drDatiPerGrafico = null;
                                        DateTime dataoraminutisec = (DateTime)drv[nomecampodataoraminsec];
                                        dataoraminutisec = new DateTime(dataoraminutisec.Year, dataoraminutisec.Month, dataoraminutisec.Day, dataoraminutisec.Hour, dataoraminutisec.Minute, dataoraminutisec.Second);
                                        for (int iFind = 0; iFind < dtReturn.Rows.Count; iFind++)
                                        {
                                            if ((DateTime)dtReturn.Rows[iFind][this.CampoDataOraPerGrafici] == dataoraminutisec)
                                            {
                                                drDatiPerGrafico = dtReturn.Rows[iFind];
                                                iFind = dtReturn.Rows.Count;
                                            }
                                        }
                                        if (drDatiPerGrafico == null)
                                        {
                                            drDatiPerGrafico = dtReturn.NewRow();
                                            drDatiPerGrafico[this.CampoDataOraPerGrafici] = dataoraminutisec;
                                            bNewRow = true;
                                        }


                                        string nomecolonnadataoraminsecgriglione = dataoraminutisec.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");
                                        if (!_listadate.Contains(dataoraminutisec))
                                        {
                                            _listadate.Add(dataoraminutisec);
                                            _listadate.Sort();

                                            DataColumn coldataora = new DataColumn(nomecolonnadataoraminsecgriglione, typeof(string));
                                            coldataora.AllowDBNull = true;
                                            coldataora.Unique = false;
                                            _datatablegriglione.Columns.Add(coldataora);
                                            _datatablegriglione.Columns[nomecolonnadataoraminsecgriglione].SetOrdinal(6 + _listadate.IndexOf(dataoraminutisec));
                                        }


                                        try
                                        {

                                            if (!bNewRow && !drDatiPerGrafico.IsNull(descprescrizionelab))
                                            {
                                                bool bFoundNewDateTime = false;
                                                while (!bFoundNewDateTime)
                                                {
                                                    dataoraminutisec = dataoraminutisec.AddSeconds(1);
                                                    bFoundNewDateTime = true;

                                                    for (int iFind = 0; iFind < dtReturn.Rows.Count; iFind++)
                                                    {
                                                        if ((DateTime)dtReturn.Rows[iFind][this.CampoDataOraPerGrafici] == dataoraminutisec)
                                                        {
                                                            if (!dtReturn.Rows[iFind].IsNull(descprescrizionelab)) bFoundNewDateTime = false;

                                                            iFind = dtReturn.Rows.Count;

                                                        }
                                                    }
                                                }

                                                drDatiPerGrafico = dtReturn.NewRow();
                                                drDatiPerGrafico[this.CampoDataOraPerGrafici] = dataoraminutisec;
                                                bNewRow = true;

                                                nomecolonnadataoraminsecgriglione = dataoraminutisec.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");
                                                if (!_listadate.Contains(dataoraminutisec))
                                                {

                                                    _listadate.Add(dataoraminutisec);
                                                    _listadate.Sort();

                                                    DataColumn coldataora = new DataColumn(nomecolonnadataoraminsecgriglione, typeof(string));
                                                    coldataora.AllowDBNull = true;
                                                    coldataora.Unique = false;
                                                    _datatablegriglione.Columns.Add(coldataora);
                                                    _datatablegriglione.Columns[nomecolonnadataoraminsecgriglione].SetOrdinal(6 + _listadate.IndexOf(dataoraminutisec));
                                                }

                                            }

                                            drDatiPerGrafico[descprescrizionelab] = drv[nomecampovalori];

                                            try
                                            {
                                                bool baddrigadimensione = true;
                                                for (int iRO = 0; iRO < _datatablegriglione.Rows.Count; iRO++)
                                                {
                                                    if (_datatablegriglione.Rows[iRO]["CodEntita"].ToString().ToUpper() == codentita.ToUpper()
                                                    && _datatablegriglione.Rows[iRO]["CodTipo"].ToString().ToUpper() == codtipo.ToUpper()
                                                    && _datatablegriglione.Rows[iRO]["CodDimensione"].ToString().ToUpper() == codprescrizionelab.ToUpper())
                                                    {
                                                        baddrigadimensione = false;

                                                        _datatablegriglione.Rows[iRO][nomecolonnadataoraminsecgriglione] = drDatiPerGrafico[descprescrizionelab];

                                                        iRO = _datatablegriglione.Rows.Count + 1;
                                                    }
                                                }
                                                if (baddrigadimensione)
                                                {
                                                    DataRow drGriglioneNew = _datatablegriglione.NewRow();

                                                    drGriglioneNew["CodEntita"] = codentita;
                                                    drGriglioneNew["Entita"] = descentita;
                                                    drGriglioneNew["CodTipo"] = codtipo;
                                                    drGriglioneNew["DescTipo"] = desctipo;
                                                    drGriglioneNew["CodDimensione"] = codprescrizionelab;
                                                    drGriglioneNew["Dimensione"] = descprescrizionelab;
                                                    drGriglioneNew[nomecolonnadataoraminsecgriglione] = drDatiPerGrafico[descprescrizionelab];

                                                    _datatablegriglione.Rows.Add(drGriglioneNew);
                                                }

                                            }
                                            catch (Exception)
                                            {
                                            }

                                        }
                                        catch (Exception)
                                        {
                                        }

                                        if (bNewRow) dtReturn.Rows.Add(drDatiPerGrafico);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }

                        }

                        _datatableMovimentiLAB.DefaultView.RowFilter = "";

                    }

                }


                return dtReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(@"<DataTableDatiPerGrafico>" + Environment.NewLine + ex.Message, ex);
            }
        }

    }

    [Serializable()]
    public class ImportTestiRefertiDWH
    {

        private EnumTipoContenutiReferto _tipoContenuto = EnumTipoContenutiReferto.Testo;
        private string _testoSelezionato = string.Empty;

        public ImportTestiRefertiDWH(EnumTipoContenutiReferto tipoContenuto)
        {
            _tipoContenuto = tipoContenuto;
            _testoSelezionato = "";
        }

        public EnumTipoContenutiReferto TipoContenuto
        {
            get { return _tipoContenuto; }
            set { _tipoContenuto = value; }
        }

        public string TestoSelezionato
        {
            get { return _testoSelezionato; }
            set { _testoSelezionato = value; }
        }


    }



    [Serializable()]
    public class MovAlertGenerico
    {

        private string _codua = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idtrasferimento = string.Empty;
        private string _idmovalertagenerico = string.Empty;
        private string _idpaziente = string.Empty;
        private string _codtipo = string.Empty;
        private string _codstato = string.Empty;
        private string _codutenterilevazione = string.Empty;
        private DateTime _dataevento = DateTime.MinValue;
        private DateTime _datavisto = DateTime.MinValue;
        private string _codutentevisto = string.Empty;

        private string _descrtipo = string.Empty;
        private string _descrstato = string.Empty;
        private string _descrutenterilevazione = string.Empty;
        private string _descrutentevisto = string.Empty;

        private int _permessoCancella = 0;
        private int _permessoVista = 0;

        private byte[] _icona;

        private string _idscheda = string.Empty;
        private string _codscheda = string.Empty;
        private int _versionescheda = 0;

        private EnumAzioni _azione = EnumAzioni.MOD;

        private MovScheda _movScheda = null;

        public MovAlertGenerico(string idmovalertgenerico, string codua, string idtrasferimento)
        {
            _azione = EnumAzioni.MOD;
            _idmovalertagenerico = idmovalertgenerico;
            _idtrasferimento = idtrasferimento;
            _codua = codua;
            this.Carica(idmovalertgenerico);
        }
        public MovAlertGenerico(string idmovalertgenerico, EnumAzioni azione, string codua, string idtrasferimento)
        {
            _azione = azione;
            _idmovalertagenerico = idmovalertgenerico;
            _idtrasferimento = idtrasferimento;
            _codua = codua;
            this.Carica(idmovalertgenerico);
        }
        public MovAlertGenerico(string idpaziente, string idepisodio, string codua, string idtrasferimento)
        {
            _azione = EnumAzioni.INS;
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _codua = codua;
            _idmovalertagenerico = "";
            _codscheda = "";
            _codtipo = "";
            _codstato = "DV";
            _movScheda = null;
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDMovAlertGenerico
        {
            get { return _idmovalertagenerico; }
            set
            {
                if (_idmovalertagenerico != value && value != "") _movScheda = null;
                _idmovalertagenerico = value;
            }
        }

        public string CodUA
        {
            get { return _codua; }
            set { _codua = value; }
        }

        public string IDEpisodio
        {
            get { return _idepisodio; }
            set { _idepisodio = value; }
        }

        public string IDTrasferimento
        {
            get { return _idtrasferimento; }
            set { _idtrasferimento = value; }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
        }

        public string IDScheda
        {
            get { return _idscheda; }
        }

        public string CodScheda
        {
            get { return _codscheda; }
            set
            {
                if (_codscheda != value) _movScheda = null;
                _codscheda = value;
            }
        }

        public int VersioneScheda
        {
            get { return _versionescheda; }
        }

        public MovScheda MovScheda
        {
            get
            {
                if (_movScheda == null)
                {
                    if (_idmovalertagenerico != null && _idmovalertagenerico != string.Empty && _idmovalertagenerico.Trim() != "")
                    {
                        _movScheda = new MovScheda(EnumEntita.ALG.ToString(), _idmovalertagenerico, CoreStatics.CoreApplication.Ambiente);
                    }
                    else if (_codscheda != null && _codscheda != string.Empty && _codscheda.Trim() != "")
                    {
                        _movScheda = new MovScheda(_codscheda, EnumEntita.ALG, _codua, _idpaziente, _idepisodio, _idtrasferimento, CoreStatics.CoreApplication.Ambiente);
                        Gestore oGestore = CoreStatics.GetGestore();
                        oGestore.SchedaXML = _movScheda.Scheda.StrutturaXML;
                        oGestore.SchedaLayoutsXML = _movScheda.Scheda.LayoutXML;
                        oGestore.Decodifiche = _movScheda.Scheda.DizionarioValori();
                        oGestore.NuovaScheda();
                        _movScheda.DatiXML = oGestore.SchedaDatiXML;
                    }
                }

                return _movScheda;
            }
        }

        public DateTime DataEvento
        {
            get { return _dataevento; }
            set { _dataevento = value; }
        }

        public DateTime DataVisto
        {
            get { return _datavisto; }
            set { _datavisto = value; }
        }

        public string CodUtenteRilevazione
        {
            get { return _codutenterilevazione; }
        }

        public string CodUtenteVisto
        {
            get { return _codutentevisto; }
        }

        public string CodStato
        {
            get { return _codstato; }
            set { _codstato = value; }
        }

        public string CodTipo
        {
            get { return _codtipo; }
            set { _codtipo = value; }
        }

        public string DescrStato
        {
            get { return _descrstato; }
        }

        public string DescrTipo
        {
            get { return _descrtipo; }
            set { _descrtipo = value; }
        }

        public string DescrUtenteRilevazione
        {
            get { return _descrutenterilevazione; }
        }

        public string DescrUtenteVisto
        {
            get { return _descrutentevisto; }
        }

        public int PermessoCancella
        {
            get { return _permessoCancella; }
            set { _permessoCancella = value; }
        }

        public int PermessoVista
        {
            get { return _permessoVista; }
            set { _permessoVista = value; }
        }

        public byte[] Icona
        {
            get { return _icona; }
        }

        private void resetValori()
        {
            _idepisodio = string.Empty;
            _idpaziente = string.Empty;
            _codtipo = string.Empty;
            _codstato = string.Empty;
            _codutenterilevazione = string.Empty;
            _codutentevisto = string.Empty;
            _dataevento = DateTime.MinValue;
            _datavisto = DateTime.MinValue;

            _descrtipo = string.Empty;
            _descrstato = string.Empty;
            _descrutentevisto = string.Empty;

            _permessoCancella = 0;
            _permessoVista = 0;

            _icona = null;

            _idscheda = string.Empty;
            _codscheda = string.Empty;
            _versionescheda = 0;
            _movScheda = null;
        }

        private void Carica(string idmovalertgenerico)
        {
            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDAlertGenerico", idmovalertgenerico);
                op.Parametro.Add("DatiEstesi", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovAlertGenerici", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();

                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();

                    if (!dt.Rows[0].IsNull("CodTipo")) _codtipo = dt.Rows[0]["CodTipo"].ToString();
                    if (!dt.Rows[0].IsNull("DescrTipo")) _descrtipo = dt.Rows[0]["DescrTipo"].ToString();
                    if (!dt.Rows[0].IsNull("CodStatoAlertGenerico")) _codstato = dt.Rows[0]["CodStatoAlertGenerico"].ToString();
                    if (!dt.Rows[0].IsNull("DescrStato")) _descrstato = dt.Rows[0]["DescrStato"].ToString();
                    if (!dt.Rows[0].IsNull("CodUtente")) _codutenterilevazione = dt.Rows[0]["CodUtente"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtente")) _descrutenterilevazione = dt.Rows[0]["DescrUtente"].ToString();

                    if (!dt.Rows[0].IsNull("DataEvento")) _dataevento = (DateTime)dt.Rows[0]["DataEvento"];

                    if (dt.Columns.Contains("DataVisto") && !dt.Rows[0].IsNull("DataVisto")) _datavisto = (DateTime)dt.Rows[0]["DataVisto"];
                    if (dt.Columns.Contains("CodUtenteVisto") && !dt.Rows[0].IsNull("CodUtenteVisto")) _codutentevisto = dt.Rows[0]["CodUtenteVisto"].ToString();
                    if (dt.Columns.Contains("DescrUtenteVisto") && !dt.Rows[0].IsNull("DescrUtenteVisto")) _descrutentevisto = dt.Rows[0]["DescrUtenteVisto"].ToString();


                    if (!dt.Rows[0].IsNull("PermessoCancella")) _permessoCancella = (int)dt.Rows[0]["PermessoCancella"];
                    if (!dt.Rows[0].IsNull("PermessoVista")) _permessoVista = (int)dt.Rows[0]["PermessoVista"];

                    if (!dt.Rows[0].IsNull("Icona")) _icona = (byte[])dt.Rows[0]["Icona"];

                    if (dt.Columns.Contains("IDScheda") && !dt.Rows[0].IsNull("IDScheda")) _idscheda = dt.Rows[0]["IDScheda"].ToString();
                    if (dt.Columns.Contains("CodScheda") && !dt.Rows[0].IsNull("CodScheda")) _codscheda = dt.Rows[0]["CodScheda"].ToString();
                    if (dt.Columns.Contains("Versione") && !dt.Rows[0].IsNull("Versione")) _versionescheda = (int)dt.Rows[0]["Versione"];

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public bool Cancella()
        {
            try
            {
                CodStato = "CA";
                Azione = EnumAzioni.CAN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovalertagenerico);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Vista()
        {
            try
            {
                CodStato = "VS";
                Azione = EnumAzioni.VIS;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovalertagenerico);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Salva()
        {
            try
            {
                bool bReturn = true;
                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";
                if (_idmovalertagenerico != string.Empty && _idmovalertagenerico.Trim() != "")
                {
                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDAlertGenerico", _idmovalertagenerico);
                    op.Parametro.Add("CodTipoAlertGenerico", _codtipo);
                    op.Parametro.Add("CodStatoAlertGenerico", _codstato);

                    if (_azione == EnumAzioni.ANN)
                    {
                        this.MovScheda.Azione = EnumAzioni.ANN;
                    }
                    if (_azione == EnumAzioni.CAN)
                        this.MovScheda.Azione = EnumAzioni.CAN;

                    op.TimeStamp.CodEntita = EnumEntita.ALG.ToString();
                    op.TimeStamp.IDEntita = _idmovalertagenerico;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    op.MovScheda = this.MovScheda;

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggMovAlertGenerici", spcoll);

                    this.MovScheda.Salva();

                    Carica(_idmovalertagenerico);
                }
                else
                {

                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDEpisodio", _idepisodio);
                    op.Parametro.Add("IDTrasferimento", _idtrasferimento);
                    op.Parametro.Add("CodTipoAlertGenerico", _codtipo);
                    op.Parametro.Add("CodStatoAlertAllergiaAnamnesi", _codstato);

                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));

                    op.TimeStamp.CodEntita = EnumEntita.ALG.ToString();
                    op.TimeStamp.CodAzione = _azione.ToString();


                    op.MovScheda = this.MovScheda;

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovAlertGenerici", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _idmovalertagenerico = dt.Rows[0][0].ToString();

                        this.MovScheda.IDEntita = _idmovalertagenerico;
                        this.MovScheda.Salva();

                        _azione = EnumAzioni.MOD;
                        Carica(_idmovalertagenerico);
                    }
                }

                return bReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(@"MovAlertGenerico.Salva()" + Environment.NewLine + ex.Message, ex);
            }
        }

    }

    [Serializable()]
    public class MovAllegato
    {
        public const string FORMATO_ELETTRONICO = "E";
        public const string FORMATO_VIRTUALE = "C";

        private string _idmovallegato = string.Empty;

        private string _idpaziente = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idtrasferimento = string.Empty;
        private string _codua = string.Empty;
        private string _ua = string.Empty;
        private string _codentita = string.Empty;

        private string _idfolder = string.Empty;
        private string _folder = string.Empty;

        private string _iddocumento = string.Empty;
        private string _codstatoallegato = string.Empty;
        private string _codtipoallegato = string.Empty;
        private string _codformatoallegato = string.Empty;
        private string _codutenterilevazione = string.Empty;
        private string _codutenteultimamodifica = string.Empty;
        private DateTime _dataevento = DateTime.MinValue;
        private DateTime _dataultimamodifica = DateTime.MinValue;

        private string _descrtipoallegato = string.Empty;
        private string _descrstatoallegato = string.Empty;
        private string _descrformatoallegato = string.Empty;
        private string _descrutenterilevazione = string.Empty;
        private string _descrutenteultimamodifica = string.Empty;

        private byte[] _icona = null;
        private byte[] _documento = null;

        private int _idicona = 0;
        private int _idiconaformato = 0;

        private string _nomefile = string.Empty;

        private string _iddocumentofirmato = string.Empty;
        private string _id = string.Empty;
        private string _infofirmadigitale = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;

        private string _testortf = string.Empty;
        private string _notartf = string.Empty;

        private string _testotxt = string.Empty;
        private string _notatxt = string.Empty;

        private int _permessoModifica = 0;
        private int _permessoCancella = 0;
        private int _permessoVisualizza = 0;

        public MovAllegato(string idmovallegato)
        {
            _azione = EnumAzioni.MOD;
            _idmovallegato = idmovallegato;
            this.Carica(idmovallegato);
        }
        public MovAllegato(string idmovallegato, EnumAzioni azione)
        {
            _azione = azione;
            _idmovallegato = idmovallegato;
            this.Carica(idmovallegato);
        }
        public MovAllegato(string idpaziente, string idepisodio, string idtrasferimento, string codformatoallegato)
        {
            _azione = EnumAzioni.INS;
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _codformatoallegato = codformatoallegato;
            _idmovallegato = "";
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDMovAllegato
        {
            get { return _idmovallegato; }
            set
            {
                _idmovallegato = value;
            }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
        }

        public string IDEpisodio
        {
            get { return _idepisodio; }
        }

        public string IDTrasferimento
        {
            get { return _idtrasferimento; }
        }

        public string IDFolder
        {
            get { return _idfolder; }
            set { _idfolder = value; }
        }

        public string Folder
        {
            get { return _folder; }
            set { _folder = value; }
        }

        public string CodUA
        {
            get { return _codua; }
            set { _codua = value; }
        }

        public string UA
        {
            get { return _ua; }
        }

        public string CodEntita
        {
            get { return _codentita; }
            set { _codentita = value; }
        }

        public DateTime DataEvento
        {
            get { return _dataevento; }
            set { _dataevento = value; }
        }

        public DateTime DataRilevazione { get; set; }

        public DateTime DataModifica
        {
            get { return _dataultimamodifica; }
        }

        public string IDDocumento
        {
            get { return _iddocumento; }
            set
            {
                _iddocumento = value;
            }
        }

        public string CodFormatoAllegato
        {
            get { return _codformatoallegato; }
            set { _codformatoallegato = value; }
        }

        public string DescrFormatoAllegato
        {
            get { return _descrformatoallegato; }
        }

        public string CodTipoAllegato
        {
            get { return _codtipoallegato; }
            set
            {
                _codtipoallegato = value;
            }

        }

        public string DescrTipoAllegato
        {
            get { return _descrtipoallegato; }
            set { _descrtipoallegato = value; }
        }

        public string CodStatoAllegato
        {
            get { return _codstatoallegato; }
            set { _codstatoallegato = value; }
        }

        public string DescrStatoAllegato
        {
            get { return _descrstatoallegato; }
        }

        public string CodUtenteRilevazione
        {
            get { return _codutenterilevazione; }
            set { _codutenterilevazione = value; }
        }

        public string DescrUtenteRilevazione
        {
            get { return _descrutenterilevazione; }
        }

        public string CodUtenteUltimaModifica
        {
            get { return _codutenteultimamodifica; }
            set { _codutenteultimamodifica = value; }
        }

        public string DescrUtenteUltimaModifica
        {
            get { return _descrutenteultimamodifica; }
        }

        public byte[] Documento
        {
            get { return _documento; }
            set { _documento = value; }
        }

        public string NomeFile
        {
            get { return _nomefile; }
            set { _nomefile = value; }
        }

        public string EstensioneFile
        {
            get
            {
                string ext = "";
                if (_nomefile.Trim() != "")
                    ext = System.IO.Path.GetExtension(_nomefile).Replace(".", "");
                return ext;
            }
        }

        public string TestoRTF
        {
            get { return _testortf; }
            set { _testortf = value; }
        }

        public string NotaRTF
        {
            get { return _notartf; }
            set { _notartf = value; }
        }

        public string TestoTXT
        {
            get
            {
                RtfTree Tree = new RtfTree();
                Tree.LoadRtfText(_testortf);

                if (Tree != null && Tree.Rtf != string.Empty)
                {
                    _testotxt = (Tree.Text != "" ? Tree.Text : "");
                }
                else
                {
                    _testotxt = string.Empty;
                }

                Tree = null;
                return _testotxt;
            }
            set { _testotxt = value; }
        }

        public string NotaTXT
        {
            get
            {
                RtfTree Tree = new RtfTree();
                Tree.LoadRtfText(_notartf);

                if (Tree != null && Tree.Rtf != string.Empty)
                {
                    _notatxt = (Tree.Text != "" ? Tree.Text : "");
                }
                else
                {
                    _notatxt = string.Empty;
                }

                Tree = null;
                return _notatxt;
            }
            set { _notatxt = value; }
        }

        public byte[] Icona
        {
            get { return _icona; }
        }

        public string IDDocumentoFirmato
        {
            get { return _iddocumentofirmato; }
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string InfoFirmaDigitale
        {
            get { return _infofirmadigitale; }
        }

        public int PermessoModifica
        {
            get { return _permessoModifica; }
            set { _permessoModifica = value; }
        }

        public int PermessoCancella
        {
            get { return _permessoCancella; }
            set { _permessoCancella = value; }
        }

        public int PermessoVisualizza
        {
            get { return _permessoVisualizza; }
            set { _permessoVisualizza = value; }
        }

        public int IDIcona
        {
            get { return _idicona; }
        }

        public int IDIconaFormato
        {
            get { return _idiconaformato; }
        }

        public bool Cancella()
        {
            try
            {
                Azione = EnumAzioni.CAN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovallegato);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {
                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";
                if (_idmovallegato != string.Empty && _idmovallegato.Trim() != "")
                {
                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDAllegato", _idmovallegato);
                    op.Parametro.Add("CodStatoAllegato", _codstatoallegato);
                    op.Parametro.Add("CodTipoAllegato", _codtipoallegato);

                    if (_idfolder != string.Empty)
                    {
                        op.Parametro.Add("IDFolder", _idfolder);
                    }

                    op.Parametro.Add("CodEntita", _codentita);
                    op.Parametro.Add("CodUA", _codua);

                    op.Parametro.Add("NumeroDocumento", _iddocumento);
                    op.Parametro.Add("TestoRTF", RtfProcs.EncodeTo64(_testortf));
                    op.Parametro.Add("NotaRTF", RtfProcs.EncodeTo64(_notartf));

                    op.Parametro.Add("TestoTXT", RtfProcs.EncodeTo64(this.TestoTXT));
                    op.Parametro.Add("NotaTXT", RtfProcs.EncodeTo64(this.NotaTXT));

                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));

                    op.TimeStamp.CodEntita = EnumEntita.ALL.ToString();
                    op.TimeStamp.IDEntita = _idmovallegato;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggMovAllegati", spcoll);

                }
                else
                {

                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDPaziente", _idpaziente);
                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));

                    if (_idepisodio != string.Empty && _idepisodio.Trim() != "" && _idepisodio.Trim() != "NULL")
                        op.Parametro.Add("IDEpisodio", _idepisodio);
                    else if (CoreStatics.CoreApplication.Episodio != null)
                        op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);

                    if (_idtrasferimento != string.Empty && _idtrasferimento.Trim() != "" && _idtrasferimento.Trim() != "NULL")
                        op.Parametro.Add("IDTrasferimento", _idtrasferimento);
                    else if (CoreStatics.CoreApplication.Trasferimento != null)
                        op.Parametro.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);

                    if (_id != string.Empty) { op.Parametro.Add("ID", _id); }

                    op.Parametro.Add("CodEntita", _codentita);
                    op.Parametro.Add("CodUA", _codua);

                    if (_idfolder != string.Empty)
                    {
                        op.Parametro.Add("IDFolder", _idfolder);
                    }

                    op.Parametro.Add("NumeroDocumento", _iddocumento);

                    op.Parametro.Add("CodTipoAllegato", _codtipoallegato);
                    op.Parametro.Add("CodFormatoAllegato", _codformatoallegato);

                    if (_codstatoallegato != string.Empty && _codstatoallegato.Trim() != "")
                        op.Parametro.Add("CodStatoAllegato", _codstatoallegato);

                    op.Parametro.Add("TestoRTF", RtfProcs.EncodeTo64(_testortf));
                    op.Parametro.Add("NotaRTF", RtfProcs.EncodeTo64(_notartf));

                    op.Parametro.Add("TestoTXT", RtfProcs.EncodeTo64(this.TestoTXT));
                    op.Parametro.Add("NotaTXT", RtfProcs.EncodeTo64(this.NotaTXT));

                    if (_documento != null)
                        op.Parametro.Add("Documento", Convert.ToBase64String(_documento));

                    op.Parametro.Add("NomeFile", RtfProcs.EncodeTo64(_nomefile));
                    op.Parametro.Add("Estensione", this.EstensioneFile);

                    op.TimeStamp.CodEntita = EnumEntita.ALL.ToString();
                    op.TimeStamp.CodAzione = _azione.ToString();

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovAllegati", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _idmovallegato = dt.Rows[0][0].ToString();

                        _azione = EnumAzioni.MOD;
                        Carica(_idmovallegato);
                    }
                    else
                        bReturn = false;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovAllegato.Salva()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;

        }

        private void resetValori()
        {
            _iddocumento = string.Empty;
            _idmovallegato = string.Empty;
            _codua = string.Empty;
            _ua = string.Empty;
            _codentita = string.Empty;
            _idpaziente = string.Empty;
            _idepisodio = string.Empty;
            _idtrasferimento = string.Empty;
            _codstatoallegato = string.Empty;
            _codformatoallegato = string.Empty;
            _codtipoallegato = string.Empty;
            _codutenterilevazione = string.Empty;
            _dataevento = DateTime.MinValue;
            _dataultimamodifica = DateTime.MinValue;
            this.DataRilevazione = DateTime.MinValue;

            _idfolder = string.Empty;
            _folder = string.Empty;

            _descrtipoallegato = string.Empty;
            _descrstatoallegato = string.Empty;
            _descrutenterilevazione = string.Empty;
            _descrformatoallegato = string.Empty;

            _permessoModifica = 0;
            _permessoCancella = 0;
            _permessoVisualizza = 0;

            _icona = null;
            _documento = null;

            _iddocumentofirmato = string.Empty;
            _id = string.Empty;
            _infofirmadigitale = string.Empty;

            _testortf = string.Empty;
            _notartf = string.Empty;

            _testotxt = string.Empty;
            _notatxt = string.Empty;

            _nomefile = string.Empty;
        }

        private void Carica(string idmovallegato)
        {
            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDAllegato", idmovallegato);
                op.Parametro.Add("Documenti", "1");

                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                op.TimeStamp.CodEntita = EnumEntita.ALL.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovAllegati", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();
                    _idmovallegato = dt.Rows[0]["ID"].ToString();
                    if (!dt.Rows[0].IsNull("IDPaziente")) _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    if (!dt.Rows[0].IsNull("IDEpisodio")) _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    if (!dt.Rows[0].IsNull("IDTrasferimento")) _idtrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();
                    if (!dt.Rows[0].IsNull("CodUA")) _codua = dt.Rows[0]["CodUA"].ToString();
                    if (!dt.Rows[0].IsNull("UA")) _ua = dt.Rows[0]["UA"].ToString();
                    if (!dt.Rows[0].IsNull("CodEntita")) _codentita = dt.Rows[0]["CodEntita"].ToString();

                    if (!dt.Rows[0].IsNull("IDFolder")) _idfolder = dt.Rows[0]["IDFolder"].ToString();
                    if (!dt.Rows[0].IsNull("Folder")) _folder = dt.Rows[0]["Folder"].ToString();

                    if (!dt.Rows[0].IsNull("NumeroDocumento")) _iddocumento = dt.Rows[0]["NumeroDocumento"].ToString();

                    if (!dt.Rows[0].IsNull("CodStatoAllegato")) _codstatoallegato = dt.Rows[0]["CodStatoAllegato"].ToString();
                    if (!dt.Rows[0].IsNull("DescrStatoAllegato")) _descrstatoallegato = dt.Rows[0]["DescrStatoAllegato"].ToString();

                    if (!dt.Rows[0].IsNull("CodTipoAllegato")) _codtipoallegato = dt.Rows[0]["CodTipoAllegato"].ToString();
                    if (!dt.Rows[0].IsNull("DescrTipoAllegato")) _descrtipoallegato = dt.Rows[0]["DescrTipoAllegato"].ToString();

                    if (!dt.Rows[0].IsNull("CodFormatoAllegato")) _codformatoallegato = dt.Rows[0]["CodFormatoAllegato"].ToString();
                    if (!dt.Rows[0].IsNull("DescrFormatoAllegato")) _descrformatoallegato = dt.Rows[0]["DescrFormatoAllegato"].ToString();

                    if (!dt.Rows[0].IsNull("CodUtenteRilevazione")) _codutenterilevazione = dt.Rows[0]["CodUtenteRilevazione"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtenteRilevazione")) _descrutenterilevazione = dt.Rows[0]["DescrUtenteRilevazione"].ToString();

                    if (!dt.Rows[0].IsNull("DataEvento")) _dataevento = (DateTime)dt.Rows[0]["DataEvento"];
                    if (!dt.Rows[0].IsNull("DataRilevazione")) this.DataRilevazione = (DateTime)dt.Rows[0]["DataRilevazione"];

                    if (!dt.Rows[0].IsNull("CodUtenteUltimaModifica")) _codutenteultimamodifica = dt.Rows[0]["CodUtenteUltimaModifica"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtenteModifica")) _descrutenteultimamodifica = dt.Rows[0]["DescrUtenteModifica"].ToString();

                    if (!dt.Rows[0].IsNull("DataUltimaModifica")) _dataultimamodifica = (DateTime)dt.Rows[0]["DataUltimaModifica"];

                    if (!dt.Rows[0].IsNull("Icona")) _icona = (byte[])dt.Rows[0]["Icona"];

                    if (!dt.Rows[0].IsNull("TestoRTF")) _testortf = dt.Rows[0]["TestoRTF"].ToString();
                    if (!dt.Rows[0].IsNull("NotaRTF")) _notartf = dt.Rows[0]["NotaRTF"].ToString();

                    if (!dt.Rows[0].IsNull("TestoTXT")) _testotxt = dt.Rows[0]["TestoTXT"].ToString();
                    if (!dt.Rows[0].IsNull("NotaTXT")) _notatxt = dt.Rows[0]["NotaTXT"].ToString();

                    if (!dt.Rows[0].IsNull("Documento")) _documento = (byte[])dt.Rows[0]["Documento"];
                    if (!dt.Rows[0].IsNull("NomeFile")) _nomefile = dt.Rows[0]["NomeFile"].ToString();

                    if (!dt.Rows[0].IsNull("IDDocumentoFirmato")) _iddocumentofirmato = dt.Rows[0]["IDDocumentoFirmato"].ToString();
                    if (!dt.Rows[0].IsNull("InfoFirmaDigitale")) _infofirmadigitale = dt.Rows[0]["InfoFirmaDigitale"].ToString();

                    if (!dt.Rows[0].IsNull("PermessoCancella")) _permessoCancella = (int)dt.Rows[0]["PermessoCancella"];
                    if (!dt.Rows[0].IsNull("PermessoModifica")) _permessoModifica = (int)dt.Rows[0]["PermessoModifica"];
                    if (!dt.Rows[0].IsNull("PermessoVisualizza")) _permessoVisualizza = (int)dt.Rows[0]["PermessoVisualizza"];

                    if (!dt.Rows[0].IsNull("IDIcona")) _idicona = int.Parse(dt.Rows[0]["IDIcona"].ToString());
                    if (!dt.Rows[0].IsNull("IDIconaFormato")) _idiconaformato = int.Parse(dt.Rows[0]["IDIconaFormato"].ToString());

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }

    [Serializable()]
    public class MovFolder
    {

        private string _idmovfolder = string.Empty;

        private string _idpaziente = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idtrasferimento = string.Empty;
        private string _idfolderpadre = string.Empty;

        private string _descrizione = string.Empty;
        private string _codstatofolder = string.Empty;
        private string _descrstatofolder = string.Empty;

        private string _codentita = string.Empty;

        private string _codua = string.Empty;
        private string _ua = string.Empty;

        private string _codutenterilevazione = string.Empty;
        private string _codutenteultimamodifica = string.Empty;
        private string _descrutenterilevazione = string.Empty;
        private string _descrutenteultimamodifica = string.Empty;
        private DateTime _datarilevazione = DateTime.MinValue;
        private DateTime _dataultimamodifica = DateTime.MinValue;

        private int _permessoModifica = 0;
        private int _permessoCancella = 0;

        private EnumAzioni _azione = EnumAzioni.MOD;

        public MovFolder(string idmovfolder)
        {
            _azione = EnumAzioni.MOD;
            _idmovfolder = idmovfolder;
            this.Carica(idmovfolder);
        }
        public MovFolder(string idmovfolder, EnumAzioni azione)
        {
            _azione = azione;
            _idmovfolder = idmovfolder;
            this.Carica(idmovfolder);
        }
        public MovFolder(string idpaziente, string idepisodio, string idtrasferimento, string idfolderpadre)
        {
            _azione = EnumAzioni.INS;
            _idmovfolder = "";
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _idfolderpadre = idfolderpadre;
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDMovFolder
        {
            get { return _idmovfolder; }
            set { _idmovfolder = value; }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
        }

        public string IDEpisodio
        {
            get { return _idepisodio; }
        }

        public string IDTrasferimento
        {
            get { return _idtrasferimento; }
        }

        public string IDFolderPadre
        {
            get { return _idfolderpadre; }
            set { _idfolderpadre = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        public string CodStatoFolder
        {
            get { return _codstatofolder; }
            set { _codstatofolder = value; }
        }

        public string DescrStatoFolder
        {
            get { return _descrstatofolder; }
        }

        public string CodEntita
        {
            get { return _codentita; }
            set { _codentita = value; }

        }

        public string CodUA
        {
            get { return _codua; }
            set { _codua = value; }
        }

        public string UA
        {
            get { return _ua; }
            set { _ua = value; }

        }

        public string CodUtenteRilevazione
        {
            get { return _codutenterilevazione; }
        }

        public string DescrUtenteRilevazione
        {
            get { return _descrutenterilevazione; }
        }

        public string CodUtenteUltimaModifica
        {
            get { return _codutenteultimamodifica; }
        }

        public string DescrUtenteUltimaModifica
        {
            get { return _descrutenteultimamodifica; }
        }

        public DateTime DataRilevazione
        {
            get { return _datarilevazione; }
        }

        public DateTime DataUltimaModifica
        {
            get { return _dataultimamodifica; }
        }

        public int PermessoModifica
        {
            get { return _permessoModifica; }
        }

        public int PermessoCancella
        {
            get { return _permessoCancella; }
        }

        public bool Cancella()
        {
            try
            {
                Azione = EnumAzioni.CAN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovfolder);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {

                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";
                if (_idmovfolder != string.Empty && _idmovfolder.Trim() != "")
                {

                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDFolder", _idmovfolder);
                    op.Parametro.Add("Descrizione", _descrizione);
                    op.Parametro.Add("CodStatoFolder", _codstatofolder);

                    op.Parametro.Add("IDFolderPadre", _idfolderpadre);

                    op.TimeStamp.CodEntita = EnumEntita.ALL.ToString();
                    op.TimeStamp.IDEntita = _idmovfolder;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggMovFolder", spcoll);

                }
                else
                {

                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDPaziente", _idpaziente);

                    if (_idepisodio != string.Empty && _idepisodio.Trim() != "" && _idepisodio.Trim() != "NULL")
                        op.Parametro.Add("IDEpisodio", _idepisodio);

                    if (_idtrasferimento != string.Empty && _idtrasferimento.Trim() != "" && _idtrasferimento.Trim() != "NULL")
                        op.Parametro.Add("IDTrasferimento", _idtrasferimento);

                    if (_idfolderpadre != string.Empty && _idfolderpadre.Trim() != "" && _idfolderpadre.Trim() != "NULL")
                        op.Parametro.Add("IDFolderPadre", _idfolderpadre);

                    op.Parametro.Add("Descrizione", _descrizione);

                    if (_codstatofolder != string.Empty && _codstatofolder.Trim() != "")
                        op.Parametro.Add("CodStatoFolder", _codstatofolder);

                    op.Parametro.Add("CodEntita", _codentita);

                    op.Parametro.Add("CodUA", _codua);

                    op.TimeStamp.CodEntita = EnumEntita.ALL.ToString();
                    op.TimeStamp.CodAzione = _azione.ToString();

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovFolder", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _idmovfolder = dt.Rows[0][0].ToString();

                        _azione = EnumAzioni.MOD;
                        Carica(_idmovfolder);
                    }
                    else
                        bReturn = false;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovFolder.Salva()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;

        }

        private void resetValori()
        {
            _idmovfolder = string.Empty;
            _idpaziente = string.Empty;
            _idepisodio = string.Empty;
            _idtrasferimento = string.Empty;
            _idfolderpadre = string.Empty;
            _descrizione = string.Empty;
            _codstatofolder = string.Empty;
            _descrstatofolder = string.Empty;
        }

        private void Carica(string idmovfolder)
        {
            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDFolder", idmovfolder);

                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                op.TimeStamp.CodEntita = EnumEntita.ALL.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovFolder", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();
                    _idmovfolder = dt.Rows[0]["ID"].ToString();
                    if (!dt.Rows[0].IsNull("IDPaziente")) _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    if (!dt.Rows[0].IsNull("IDEpisodio")) _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    if (!dt.Rows[0].IsNull("IDTrasferimento")) _idtrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();
                    if (!dt.Rows[0].IsNull("IDFolderPadre")) _idfolderpadre = dt.Rows[0]["IDFolderPadre"].ToString();

                    if (!dt.Rows[0].IsNull("Descrizione")) _descrizione = dt.Rows[0]["Descrizione"].ToString();
                    if (!dt.Rows[0].IsNull("CodStatoFolder")) _codstatofolder = dt.Rows[0]["CodStatoFolder"].ToString();
                    if (!dt.Rows[0].IsNull("StatoFolder")) _descrstatofolder = dt.Rows[0]["StatoFolder"].ToString();

                    if (!dt.Rows[0].IsNull("CodEntita")) _codentita = dt.Rows[0]["CodEntita"].ToString();

                    if (!dt.Rows[0].IsNull("CodUA")) _codua = dt.Rows[0]["CodUA"].ToString();
                    if (!dt.Rows[0].IsNull("UA")) _ua = dt.Rows[0]["UA"].ToString();

                    if (!dt.Rows[0].IsNull("DataRilevazione")) _datarilevazione = (DateTime)dt.Rows[0]["DataRilevazione"];
                    if (!dt.Rows[0].IsNull("CodUtenteRilevazione")) _codutenterilevazione = dt.Rows[0]["CodUtenteRilevazione"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtenteRilevazione")) _descrutenterilevazione = dt.Rows[0]["DescrUtenteRilevazione"].ToString();
                    if (!dt.Rows[0].IsNull("DataUltimaModifica")) _dataultimamodifica = (DateTime)dt.Rows[0]["DataUltimaModifica"];
                    if (!dt.Rows[0].IsNull("CodUtenteUltimaModifica")) _codutenteultimamodifica = dt.Rows[0]["CodUtenteUltimaModifica"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtenteModifica")) _descrutenteultimamodifica = dt.Rows[0]["DescrUtenteModifica"].ToString();

                    if (!dt.Rows[0].IsNull("PermessoCancella")) _permessoCancella = (int)dt.Rows[0]["PermessoCancella"];
                    if (!dt.Rows[0].IsNull("PermessoModifica")) _permessoModifica = (int)dt.Rows[0]["PermessoModifica"];

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }

    [Serializable()]
    public class MovEvidenzaClinica
    {
        private string _idmovevidenzaclinica = string.Empty;
        private string _idpaziente = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idtrasferimento = string.Empty;
        private string _idrefertoDWH = string.Empty;
        private string _codstatoevidenzaclinica = string.Empty;
        private string _codstatoevidenzaclinicavisione = string.Empty;
        private string _codtipoevidenzaclinica = string.Empty;
        private string _codutentevisione = string.Empty;
        private DateTime _datareferto = DateTime.MinValue;
        private DateTime _datavisione = DateTime.MinValue;
        private DateTime _dataeventodwh = DateTime.MinValue;

        private string _descrtipoevidenzaclinica = string.Empty;
        private string _descrstatoevidenzaclinica = string.Empty;
        private string _descrstatoevidenzaclinicavisione = string.Empty;
        private string _descrutentevisione = string.Empty;

        private int _permessoVista = 0;
        private int _permessoGrafico = 0;

        private byte[] _icona = null;
        private byte[] _pdf = null;

        private string _numeroOrdine = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;

        private string _anteprima = string.Empty;

        public MovEvidenzaClinica(string idmovevidenzaclinica, string idrefertoDWH)
        {
            resetValori();
            _azione = EnumAzioni.MOD;
            _idmovevidenzaclinica = idmovevidenzaclinica;
            this.Carica(idmovevidenzaclinica);

            if ((_idrefertoDWH == null || _idrefertoDWH == string.Empty || _idrefertoDWH.Trim() == "")
                && (idrefertoDWH != null && idrefertoDWH != string.Empty && idrefertoDWH.Trim() != "")) _idrefertoDWH = idrefertoDWH;
        }
        public MovEvidenzaClinica(string idmovevidenzaclinica, string idrefertoDWH, EnumAzioni azione)
        {
            resetValori();
            _azione = azione;
            _idmovevidenzaclinica = idmovevidenzaclinica;
            this.Carica(idmovevidenzaclinica);

            if ((_idrefertoDWH == null || _idrefertoDWH == string.Empty || _idrefertoDWH.Trim() == "")
                && (idrefertoDWH != null && idrefertoDWH != string.Empty && idrefertoDWH.Trim() != "")) _idrefertoDWH = idrefertoDWH;
        }
        public MovEvidenzaClinica(string idrefertoDWH,
                                  string codtipoevidenzaclinica, string descrtipoevidenzaclinica,
                                  string codstatoevidenzaclinica, string descrstatoevidenzaclinica,
                                  string anteprima,
                                  DateTime datareferto, DateTime dataeventodwh)
        {
            resetValori();
            _azione = EnumAzioni.VIS;
            _idmovevidenzaclinica = "";
            _idpaziente = "";
            _idepisodio = "";
            _idtrasferimento = "";
            _idrefertoDWH = idrefertoDWH;
            _codstatoevidenzaclinica = codstatoevidenzaclinica;
            _descrstatoevidenzaclinica = descrstatoevidenzaclinica;
            _codtipoevidenzaclinica = codtipoevidenzaclinica;
            _descrtipoevidenzaclinica = descrtipoevidenzaclinica;
            _codstatoevidenzaclinicavisione = "";
            _descrstatoevidenzaclinicavisione = "";
            _anteprima = anteprima;
            _datareferto = datareferto;
            _dataeventodwh = dataeventodwh;

            _permessoVista = 0;
            _permessoGrafico = 0;
            if (codtipoevidenzaclinica.Trim().ToUpper() == EnumCodTipoEvidenzaClinica.LAB.ToString().ToUpper()) _permessoGrafico = 1;

            _numeroOrdine = "";

        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDMovEvidenzaClinica
        {
            get { return _idmovevidenzaclinica; }
            set
            {
                _idmovevidenzaclinica = value;
            }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
        }

        public string IDEpisodio
        {
            get { return _idepisodio; }
        }

        public string IDTrasferimento
        {
            get { return _idtrasferimento; }
        }

        public string IDRefertoDWH
        {
            get { return _idrefertoDWH; }
            set { _idrefertoDWH = value; }
        }

        public DateTime DataReferto
        {
            get { return _datareferto; }
            set { _datareferto = value; }
        }

        public DateTime DataVisione
        {
            get { return _datavisione; }
        }

        public DateTime DataEventoDWH
        {
            get { return _dataeventodwh; }
            set { _dataeventodwh = value; }
        }

        public string CodTipoEvidenzaClinica
        {
            get { return _codtipoevidenzaclinica; }
            set
            {
                _codtipoevidenzaclinica = value;
            }

        }

        public string DescrTipoEvidenzaClinica
        {
            get { return _descrtipoevidenzaclinica; }
            set { _descrtipoevidenzaclinica = value; }
        }

        public string CodStatoEvidenzaClinica
        {
            get { return _codstatoevidenzaclinica; }
            set { _codstatoevidenzaclinica = value; }
        }

        public string DescrStatoEvidenzaClinica
        {
            get { return _descrstatoevidenzaclinica; }
        }

        public string CodStatoEvidenzaClinicaVisione
        {
            get { return _codstatoevidenzaclinicavisione; }
            set { _codstatoevidenzaclinicavisione = value; }
        }

        public string DescrStatoEvidenzaClinicaVisione
        {
            get { return _descrstatoevidenzaclinicavisione; }
        }

        public string DescrUtenteVisione
        {
            get { return _descrutentevisione; }
        }
        public string CodUtenteVisione
        {
            get { return _codutentevisione; }
        }

        public int PermessoVista
        {
            get { return _permessoVista; }
            set { _permessoVista = value; }
        }

        public int PermessoGrafico
        {
            get { return _permessoGrafico; }
            set { _permessoGrafico = value; }
        }

        public byte[] Icona
        {
            get { return _icona; }
            set { _icona = value; }
        }

        public string NumeroOrdine
        {
            get
            {
                if ((_numeroOrdine == null || _numeroOrdine.Trim() == "")
                && (_idrefertoDWH != null && _idrefertoDWH != string.Empty && _idrefertoDWH.Trim() != ""))
                    this.CaricaDWHDettaglio(_idrefertoDWH);

                return _numeroOrdine;
            }
        }

        public byte[] PDF
        {
            get
            {
                if ((_pdf == null || _pdf.Length <= 0)
                && (_idrefertoDWH != null && _idrefertoDWH != string.Empty && _idrefertoDWH.Trim() != ""))
                    this.CaricaDWH(_idrefertoDWH);

                return _pdf;
            }
        }

        public bool AbilitaAperturaPDF
        {
            get
            {
                if ((_pdf != null && _pdf.Length > 0)
                || (_idrefertoDWH != null && _idrefertoDWH != string.Empty && _idrefertoDWH.Trim() != ""))
                    return true;
                else
                    return false;
            }
        }

        public string URLRefertoDWH
        {
            get
            {
                if (_idrefertoDWH != null && _idrefertoDWH != string.Empty && _idrefertoDWH.Trim() != "")
                {
                    string sURL = Database.GetConfigTable(EnumConfigTable.URLAccessoRefertiDWH);
                    if (sURL == null || sURL.Trim() == "") sURL = @"https://di.asmn.net/DwhClinico/AccessoDiretto/Referto.aspx?IdReferto=§";
                    sURL = sURL.Replace(@"§", _idrefertoDWH);
                    return sURL;

                }
                else
                    return "";
            }
        }

        public string AccessNumber { get; set; }

        public bool Vista()
        {
            try
            {
                _azione = EnumAzioni.VAL;
                _codstatoevidenzaclinicavisione = "VS";
                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovevidenzaclinica);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {
                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";
                if (_idmovevidenzaclinica != string.Empty && _idmovevidenzaclinica.Trim() != "")
                {
                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDEvidenzaClinica", _idmovevidenzaclinica);

                    if (_azione == EnumAzioni.VAL)
                    {
                        op.Parametro.Add("CodStatoEvidenzaClinicaVisione", "VS");
                        op.Parametro.Add("CodUtenteVisione", CoreStatics.CoreApplication.Sessione.Utente.Codice);
                    }




                    op.TimeStamp.CodEntita = EnumEntita.EVC.ToString();
                    op.TimeStamp.IDEntita = _idmovevidenzaclinica;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggMovEvidenzaClinica", spcoll);
                }


            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovEvidenzaClinica.Salva()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;

        }

        public bool Cancella()
        {

            bool bReturn = true;

            try
            {

                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";
                if (_idmovevidenzaclinica != string.Empty && _idmovevidenzaclinica.Trim() != "")
                {

                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDEvidenzaClinica", _idmovevidenzaclinica);
                    op.Parametro.Add("CodStatoEvidenzaClinica", "CA");

                    op.TimeStamp.CodEntita = EnumEntita.EVC.ToString();
                    op.TimeStamp.IDEntita = _idmovevidenzaclinica;
                    op.TimeStamp.CodAzione = EnumAzioni.CAN.ToString();

                    spcoll = new SqlParameterExt[1];
                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggMovEvidenzaClinica", spcoll);

                }


            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovEvidenzaClinica.Salva()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;

        }

        private void resetValori()
        {
            _idmovevidenzaclinica = string.Empty;
            _idpaziente = string.Empty;
            _idepisodio = string.Empty;
            _idtrasferimento = string.Empty;
            _codstatoevidenzaclinica = string.Empty;
            _codstatoevidenzaclinicavisione = string.Empty;
            _codtipoevidenzaclinica = string.Empty;
            _codutentevisione = string.Empty;
            _datareferto = DateTime.MinValue;
            _datavisione = DateTime.MinValue;
            _dataeventodwh = DateTime.MinValue;

            _descrtipoevidenzaclinica = string.Empty;
            _descrstatoevidenzaclinica = string.Empty;
            _descrstatoevidenzaclinicavisione = string.Empty;
            _descrutentevisione = string.Empty;

            _idrefertoDWH = string.Empty;

            _permessoVista = 0;
            _permessoGrafico = 0;

            _icona = null;
            _pdf = null;

            _anteprima = string.Empty;

            this.AccessNumber = string.Empty;
        }

        private void Carica(string idmovevidenzaclinica)
        {
            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEvidenzaClinica", idmovevidenzaclinica);
                op.Parametro.Add("PDFReferto", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovEvidenzaClinica", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();
                    _idmovevidenzaclinica = dt.Rows[0]["ID"].ToString();
                    if (!dt.Rows[0].IsNull("IDPaziente")) _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    if (!dt.Rows[0].IsNull("IDEpisodio")) _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    if (!dt.Rows[0].IsNull("IDTrasferimento")) _idtrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();

                    if (!dt.Rows[0].IsNull("CodStato")) _codstatoevidenzaclinica = dt.Rows[0]["CodStato"].ToString();
                    if (!dt.Rows[0].IsNull("DescrStato")) _descrstatoevidenzaclinica = dt.Rows[0]["DescrStato"].ToString();
                    if (!dt.Rows[0].IsNull("CodTipo")) _codtipoevidenzaclinica = dt.Rows[0]["CodTipo"].ToString();
                    if (!dt.Rows[0].IsNull("DescrTipo")) _descrtipoevidenzaclinica = dt.Rows[0]["DescrTipo"].ToString();
                    if (!dt.Rows[0].IsNull("CodStatoVisione")) _codstatoevidenzaclinicavisione = dt.Rows[0]["CodStatoVisione"].ToString();
                    if (!dt.Rows[0].IsNull("DescrStatoVisione")) _descrstatoevidenzaclinicavisione = dt.Rows[0]["DescrStatoVisione"].ToString();
                    if (!dt.Rows[0].IsNull("CodUtenteVisione")) _codutentevisione = dt.Rows[0]["CodUtenteVisione"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtenteVisione")) _descrutentevisione = dt.Rows[0]["DescrUtenteVisione"].ToString();

                    if (dt.Columns.Contains("IDRefertoDWH") && !dt.Rows[0].IsNull("IDRefertoDWH")) _idrefertoDWH = dt.Rows[0]["IDRefertoDWH"].ToString();

                    if (!dt.Rows[0].IsNull("DataEvento")) _datareferto = (DateTime)dt.Rows[0]["DataEvento"];
                    if (!dt.Rows[0].IsNull("DataVisione")) _datavisione = (DateTime)dt.Rows[0]["DataVisione"];
                    if (!dt.Rows[0].IsNull("DataEventoDWH")) _dataeventodwh = (DateTime)dt.Rows[0]["DataEventoDWH"];

                    if (!dt.Rows[0].IsNull("PermessoVista")) _permessoVista = (int)dt.Rows[0]["PermessoVista"];
                    if (!dt.Rows[0].IsNull("PermessoGrafico")) _permessoGrafico = (int)dt.Rows[0]["PermessoGrafico"];

                    if (!dt.Rows[0].IsNull("Icona")) _icona = (byte[])dt.Rows[0]["Icona"];
                    if (!dt.Rows[0].IsNull("PDFDWH")) _pdf = (byte[])dt.Rows[0]["PDFDWH"];

                    if (!dt.Rows[0].IsNull("Anteprima")) _anteprima = dt.Rows[0]["Anteprima"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void CaricaDWH(string idrefertoDWH)
        {
            SvcRefertiDWH.ScciRefertiDWHClient dwh = ScciSvcRef.GetSvcRefertiDWH();

            try
            {
                if (idrefertoDWH != null && idrefertoDWH != string.Empty && idrefertoDWH.Trim() != "")
                {

                    List<AllegatoRefertoDWH> oDwhAllegatoList = new List<AllegatoRefertoDWH>();

                    oDwhAllegatoList.AddRange(dwh.CaricaRefertoDWH(idrefertoDWH));

                    DataTable oDt = CoreStatics.CreateDataTable<AllegatoRefertoDWH>();
                    CoreStatics.FillDataTable<AllegatoRefertoDWH>(oDt, oDwhAllegatoList);

                    if (oDt != null && oDt.Rows.Count > 0)
                    {
                        for (int i = 0; i < oDt.Rows.Count; i++)
                        {
                            if (oDt.Columns.Contains("IdOrderEntry") && !oDt.Rows[i].IsNull("IdOrderEntry") && oDt.Rows[i]["IdOrderEntry"].ToString() != "")
                                _numeroOrdine = oDt.Rows[i]["IdOrderEntry"].ToString().Trim();

                            if (oDt.Rows[i]["MimeType"].ToString().ToUpper().IndexOf("PDF") >= 0 && !oDt.Rows[i].IsNull("FileData"))
                            {
                                _pdf = (byte[])oDt.Rows[i]["FileData"];
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (dwh != null)
                    dwh.Close();

                dwh = null;
            }
        }

        private void CaricaDWHDettaglio(string idrefertoDWH)
        {
            SvcRefertiDWH.ScciRefertiDWHClient dwh = ScciSvcRef.GetSvcRefertiDWH();

            try
            {
                if (idrefertoDWH != null && idrefertoDWH != string.Empty && idrefertoDWH.Trim() != "")
                {
                    RefertoDWHDetailed oReferto = new RefertoDWHDetailed();
                    oReferto = dwh.CaricaRefertoDWHDettaglio(idrefertoDWH);
                    if (oReferto != null)
                    {
                        _numeroOrdine = oReferto.IdOrderEntry;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (dwh != null)
                    dwh.Close();

                dwh = null;
            }
        }

    }

    [Serializable()]
    public class MovAppuntamento
    {

        private string _idmovappuntamento = string.Empty;
        private string _codua = string.Empty;
        private string _idpaziente = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idtrasferimento = string.Empty;
        private DateTime _datainizio = DateTime.MinValue;
        private DateTime _datafine = DateTime.MinValue;
        private string _oggetto = string.Empty;
        private string _codtipoappuntamento = string.Empty;
        private string _descrtipoappuntamento = string.Empty;
        private int _timeslotinterval = 0;
        private string _codstatoappuntamento = string.Empty;
        private string _descrstatoappuntamento = string.Empty;
        private string _elencorisorse = string.Empty;
        private string _titolo = string.Empty;
        private T_TipoAppuntamento _tipoappuntamento = null;


        private bool _multiplo = false;
        private bool _senzaData = false;
        private bool _senzaDataSempre = false;
        private bool _settimanale = false;

        private string _codagendapartenza = string.Empty;

        private List<MovAppuntamentoAgende> _elementi = new List<MovAppuntamentoAgende>();
        private List<MovAppuntamentoAgende> _elementialtri = new List<MovAppuntamentoAgende>();

        private EnumAzioni _azione = EnumAzioni.MOD;

        private string _idscheda = string.Empty;
        private string _codscheda = string.Empty;
        private int _versionescheda = 0;

        private string _codsistema = string.Empty;
        private string _idsistema = string.Empty;
        private string _idgruppo = string.Empty;
        private string _infosistema = string.Empty;

        private MovScheda _movScheda = null;

        public MovAppuntamento(string idmovappuntamento)
        {
            _azione = EnumAzioni.MOD;
            _idmovappuntamento = idmovappuntamento;
            this.Carica(idmovappuntamento);
            this.CaricaAgende();
        }
        public MovAppuntamento(string idmovappuntamento, EnumAzioni azione)
        {
            _azione = azione;
            _idmovappuntamento = idmovappuntamento;
            this.Carica(idmovappuntamento);
            this.CaricaAgende();
        }
        public MovAppuntamento(string codua, string idpaziente, string idepisodio, string idtrasferimento)
        {
            _azione = EnumAzioni.INS;
            _codua = codua;
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _datainizio = DateTime.Now;
            _datafine = DateTime.Now;
            this.CaricaOggetto();
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string CodUA
        {
            get { return _codua; }
            set { _codua = value; }
        }

        public string IDAppuntamento
        {
            get { return _idmovappuntamento; }
            set { _idmovappuntamento = value; }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
        }

        public string IDEpisodio
        {
            get { return _idepisodio; }
            set { _idepisodio = value; }
        }

        public string IDTrasferimento
        {
            get { return _idtrasferimento; }
            set { _idtrasferimento = value; }
        }

        public DateTime DataInizio
        {
            get { return _datainizio; }
            set { _datainizio = value; }
        }

        public DateTime DataFine
        {
            get { return _datafine; }
            set { _datafine = value; }
        }

        public string Oggetto
        {
            get { return _oggetto; }
            set { _oggetto = value; }
        }

        public string CodTipoAppuntamento
        {
            get { return _codtipoappuntamento; }
            set { _codtipoappuntamento = value; }
        }

        public string DescrTipoAppuntamento
        {
            get { return _descrtipoappuntamento; }
            set { _descrtipoappuntamento = value; }
        }

        public int TimeSlotInterval
        {
            get { return _timeslotinterval; }
            set { _timeslotinterval = value; }
        }

        public string CodStatoAppuntamento
        {
            get { return _codstatoappuntamento; }
            set { _codstatoappuntamento = value; }
        }

        public string DescrStatoAppuntamento
        {
            get { return _descrstatoappuntamento; }
        }

        public string ElencoRisorse
        {
            get { return _elencorisorse; }
            set { _elencorisorse = value; }
        }

        public string Titolo
        {
            get
            {
                if (this.TipoAppuntamento.FormulaTitolo != "")
                {
                    this.CalcolaTitolo();
                }
                return _titolo;
            }
            set { _titolo = value; }
        }

        public string CodAgendaPartenza
        {
            get { return _codagendapartenza; }
            set { _codagendapartenza = value; }
        }

        public List<MovAppuntamentoAgende> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        public List<MovAppuntamentoAgende> ElementiAltri
        {
            get { return _elementialtri; }
            set { _elementialtri = value; }
        }

        public string IDScheda
        {
            get { return _idscheda; }
        }

        public string CodScheda
        {
            get { return _codscheda; }
            set
            {
                if (_codscheda != value) _movScheda = null;
                _codscheda = value;
            }
        }

        public int VersioneScheda
        {
            get { return _versionescheda; }
        }

        public string CodSistema
        {
            get { return _codsistema; }
            set { _codsistema = value; }
        }

        public string IDSistema
        {
            get { return _idsistema; }
            set { _idsistema = value; }
        }

        public string IDGruppo
        {
            get { return _idgruppo; }
            set { _idgruppo = value; }
        }

        public string InfoSistema
        {
            get { return _infosistema; }
            set { _infosistema = value; }
        }

        public bool Multiplo
        {
            get { return _multiplo; }
            set { _multiplo = value; }
        }

        public bool SenzaData
        {
            get { return _senzaData; }
            set { _senzaData = value; }
        }

        public bool SenzaDataSempre
        {
            get { return _senzaDataSempre; }
            set { _senzaDataSempre = value; }
        }

        public bool Settimanale
        {
            get { return _settimanale; }
            set { _settimanale = value; }
        }

        public MovScheda MovScheda
        {
            get
            {
                if (_movScheda == null)
                {
                    if (_idscheda != null && _idscheda != string.Empty && _idscheda.Trim() != "")
                    {
                        _movScheda = new MovScheda(_idscheda, CoreStatics.CoreApplication.Ambiente);
                    }
                    else if (_codscheda != null && _codscheda != string.Empty && _codscheda.Trim() != "")
                    {
                        _movScheda = new MovScheda(_codscheda, EnumEntita.APP, _codua, _idpaziente, _idepisodio, _idtrasferimento, CoreStatics.CoreApplication.Ambiente);
                        Gestore oGestore = CoreStatics.GetGestore();
                        oGestore.SchedaXML = _movScheda.Scheda.StrutturaXML;
                        oGestore.SchedaLayoutsXML = _movScheda.Scheda.LayoutXML;
                        oGestore.Decodifiche = _movScheda.Scheda.DizionarioValori();
                        oGestore.NuovaScheda();
                        _movScheda.DatiXML = oGestore.SchedaDatiXML;
                    }
                }

                return _movScheda;

            }
            set { _movScheda = value; }
        }

        public T_TipoAppuntamento TipoAppuntamento
        {
            get
            {
                if (_tipoappuntamento == null)
                {
                    _tipoappuntamento = new T_TipoAppuntamento();
                    _tipoappuntamento.Codice = _codtipoappuntamento;
                    _tipoappuntamento.Select();
                }
                return _tipoappuntamento;
            }
        }

        private void Carica(string idmovappuntamento)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDAppuntamento", idmovappuntamento);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovAppuntamenti", spcoll);

                if (dt.Rows.Count > 0)
                {

                    _idmovappuntamento = dt.Rows[0]["ID"].ToString();
                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    _idtrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();

                    if (!dt.Rows[0].IsNull("CodUA")) _codua = dt.Rows[0]["CodUA"].ToString();
                    if (!dt.Rows[0].IsNull("DataInizio")) _datainizio = (DateTime)dt.Rows[0]["DataInizio"];
                    if (!dt.Rows[0].IsNull("DataFine")) _datafine = (DateTime)dt.Rows[0]["DataFine"];
                    if (!dt.Rows[0].IsNull("Oggetto")) _oggetto = dt.Rows[0]["Oggetto"].ToString();
                    if (!dt.Rows[0].IsNull("CodTipoAppuntamento")) _codtipoappuntamento = dt.Rows[0]["CodTipoAppuntamento"].ToString();
                    if (!dt.Rows[0].IsNull("DescrTipoAppuntamento")) _descrtipoappuntamento = dt.Rows[0]["DescrTipoAppuntamento"].ToString();

                    if (!dt.Rows[0].IsNull("TimeSlotInterval")) _timeslotinterval = (int)dt.Rows[0]["TimeSlotInterval"];

                    if (!dt.Rows[0].IsNull("CodStatoAppuntamento")) _codstatoappuntamento = dt.Rows[0]["CodStatoAppuntamento"].ToString();
                    if (!dt.Rows[0].IsNull("DescrStatoAppuntamento")) _descrstatoappuntamento = dt.Rows[0]["DescrStatoAppuntamento"].ToString();
                    if (!dt.Rows[0].IsNull("ElencoRisorse")) _elencorisorse = dt.Rows[0]["ElencoRisorse"].ToString();
                    if (!dt.Rows[0].IsNull("Titolo")) _titolo = dt.Rows[0]["Titolo"].ToString();

                    if (!dt.Rows[0].IsNull("IDScheda")) _idscheda = dt.Rows[0]["IDScheda"].ToString();
                    if (!dt.Rows[0].IsNull("CodScheda")) _codscheda = dt.Rows[0]["CodScheda"].ToString();
                    if (!dt.Rows[0].IsNull("Versione")) _versionescheda = (int)dt.Rows[0]["Versione"];

                    if (!dt.Rows[0].IsNull("CodSistema")) _codsistema = dt.Rows[0]["CodSistema"].ToString();
                    if (!dt.Rows[0].IsNull("IDSistema")) _idsistema = dt.Rows[0]["IDSistema"].ToString();
                    if (!dt.Rows[0].IsNull("IDGruppo")) _idgruppo = dt.Rows[0]["IDGruppo"].ToString();
                    if (!dt.Rows[0].IsNull("InfoSistema")) _infosistema = dt.Rows[0]["InfoSistema"].ToString();

                    if (!dt.Rows[0].IsNull("Multiplo")) _multiplo = (bool)dt.Rows[0]["Multiplo"];
                    if (!dt.Rows[0].IsNull("SenzaData")) _senzaData = (bool)dt.Rows[0]["SenzaData"];
                    if (!dt.Rows[0].IsNull("SenzaDataSempre")) _senzaDataSempre = (bool)dt.Rows[0]["SenzaDataSempre"];
                    if (dt.Columns.Contains("Settimanale") && !dt.Rows[0].IsNull("Settimanale")) _settimanale = (bool)dt.Rows[0]["Settimanale"];
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void CaricaOggetto()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", IDPaziente);
                op.Parametro.Add("IDEpisodio", IDEpisodio);
                op.Parametro.Add("IDTrasferimento", IDTrasferimento);
                if (IDEpisodio == string.Empty)
                {
                    op.Parametro.Add("CodEntita", EnumEntita.PAZ.ToString());
                }
                else
                {
                    op.Parametro.Add("CodEntita", EnumEntita.EPI.ToString());
                }
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("[MSP_SelOggettoAppuntamento]", spcoll);

                if (dt.Rows.Count > 0)
                {

                    if (!dt.Rows[0].IsNull("Oggetto")) _oggetto = dt.Rows[0]["Oggetto"].ToString();

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void CalcolaTitolo()
        {

            try
            {

                string sret = string.Empty;

                if (this.TipoAppuntamento.FormulaTitolo != null && this.TipoAppuntamento.FormulaTitolo != string.Empty)
                {

                    Gestore oGestore = CoreStatics.GetGestore(true);
                    oGestore.SchedaXML = this.MovScheda.Scheda.StrutturaXML;
                    oGestore.SchedaLayoutsXML = this.MovScheda.Scheda.LayoutXML;
                    oGestore.Decodifiche = this.MovScheda.Scheda.DizionarioValori();
                    oGestore.SchedaDatiXML = this.MovScheda.DatiXML;

                    sret = oGestore.Valutatore.Process(this.TipoAppuntamento.FormulaTitolo, oGestore.Contesto);
                    oGestore = null;

                }

                if (sret != "" && sret != _titolo)
                {
                    _titolo = sret;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public void CaricaAgende()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                if (_idmovappuntamento == string.Empty)
                {
                    op.Parametro.Add("CodUA", _codua);
                    op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                    op.Parametro.Add("CodAzione", _azione.ToString());
                    op.Parametro.Add("CodTipoAppuntamento", this.CodTipoAppuntamento);
                    op.Parametro.Add("DatiEstesi", "1");
                }
                else
                {
                    op.Parametro.Add("CodUA", _codua);
                    if ((CoreStatics.CoreApplication.Sessione.Utente != null) &&
                            (CoreStatics.CoreApplication.Sessione.Utente.Ruoli != null) &&
                            (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null)
                        )
                    {
                        op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                    }
                    else
                    {
                        op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Ambiente.Codruolo);
                    }
                    op.Parametro.Add("CodAzione", _azione.ToString());
                    op.Parametro.Add("IDAppuntamento", _idmovappuntamento);
                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet oDs = Database.GetDatasetStoredProc("MSP_SelAgende", spcoll);

                _elementi = new List<MovAppuntamentoAgende>();
                _elementialtri = new List<MovAppuntamentoAgende>();
                foreach (DataRow oDr in oDs.Tables[0].Rows)
                {
                    _elementi.Add(new MovAppuntamentoAgende(oDr["IDAppuntamentoAgenda"].ToString(), oDr["Codice"].ToString(), oDr["Descrizione"].ToString(), (oDr.IsNull("Icona") == true ? null : (byte[])oDr["Icona"]), (oDr["Selezionata"].ToString() == "1" ? true : false),
                                                            oDr["CodRaggr1"].ToString(), oDr["DescrRaggr1"].ToString(), oDr["CodRaggr2"].ToString(), oDr["DescrRaggr2"].ToString(), oDr["CodRaggr3"].ToString(), oDr["DescrRaggr3"].ToString()));
                }

                if (oDs.Tables.Count == 2)
                {
                    foreach (DataRow oDr in oDs.Tables[1].Rows)
                    {
                        _elementialtri.Add(new MovAppuntamentoAgende(oDr["IDAppuntamentoAgenda"].ToString(), oDr["Codice"].ToString(), oDr["Descrizione"].ToString(), (oDr.IsNull("Icona") == true ? null : (byte[])oDr["Icona"]), (oDr["Selezionata"].ToString() == "1" ? true : false),
                                                                        oDr["CodRaggr1"].ToString(), oDr["DescrRaggr1"].ToString(), oDr["CodRaggr2"].ToString(), oDr["DescrRaggr2"].ToString(), oDr["CodRaggr3"].ToString(), oDr["DescrRaggr3"].ToString()));
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public bool CopiaDaOrigine(ref MovAppuntamento movtiorigine)
        {

            bool bReturn = true;

            try
            {

                _codua = movtiorigine.CodUA;
                _idmovappuntamento = string.Empty;
                _idpaziente = movtiorigine.IDPaziente;
                _idepisodio = movtiorigine.IDEpisodio;
                _idtrasferimento = movtiorigine.IDTrasferimento;
                _datainizio = movtiorigine.DataInizio;
                _datafine = movtiorigine.DataFine;
                _oggetto = movtiorigine.Oggetto;
                _codtipoappuntamento = movtiorigine.CodTipoAppuntamento;
                _descrtipoappuntamento = movtiorigine.DescrTipoAppuntamento;
                _timeslotinterval = movtiorigine.TimeSlotInterval;
                _codstatoappuntamento = movtiorigine.CodStatoAppuntamento;
                _descrstatoappuntamento = movtiorigine.DescrStatoAppuntamento;
                _elencorisorse = movtiorigine.ElencoRisorse;
                _titolo = movtiorigine.Titolo;
                _multiplo = movtiorigine.Multiplo;
                _senzaData = movtiorigine.SenzaData;
                _senzaDataSempre = movtiorigine.SenzaDataSempre;
                _settimanale = movtiorigine.Settimanale;

                _elementi = movtiorigine.Elementi;
                foreach (MovAppuntamentoAgende oMaa in _elementi)
                {
                    if (oMaa.Selezionata == true) { oMaa.Modificata = true; }
                }
                _elementialtri = movtiorigine.ElementiAltri;

                this.MovScheda = new MovScheda(movtiorigine.MovScheda.CodScheda,
                                                    (EnumEntita)Enum.Parse(typeof(EnumEntita), movtiorigine.MovScheda.CodEntita),
                                                    movtiorigine.MovScheda.CodUA, movtiorigine.MovScheda.IDPaziente,
                                                    movtiorigine.MovScheda.IDEpisodio, movtiorigine.MovScheda.IDTrasferimento, movtiorigine.MovScheda.Versione,
                                                    CoreStatics.CoreApplication.Ambiente);
                this.MovScheda.CopiaDaOrigine(movtiorigine.MovScheda, 1);

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovAppuntamento.CopiaDaOrigine()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;
        }

        public bool Salva()
        {
            return Salva(true, true);
        }
        public bool Salva(bool RicaricaMovimento, bool RicaricaScheda)
        {

            bool bReturn = true;

            try
            {

                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";

                switch (_azione)
                {

                    case EnumAzioni.INS:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("CodUA", _codua);
                        op.Parametro.Add("IDPaziente", _idpaziente);
                        op.Parametro.Add("IDEpisodio", _idepisodio);
                        op.Parametro.Add("IDTrasferimento", _idtrasferimento);
                        if (_datainizio > DateTime.MinValue) op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                        if (_datafine > DateTime.MinValue) op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                        op.Parametro.Add("CodTipoAppuntamento", _codtipoappuntamento);
                        op.Parametro.Add("CodStatoAppuntamento", _codstatoappuntamento);
                        op.Parametro.Add("ElencoRisorse", _elencorisorse);
                        op.Parametro.Add("Titolo", RtfProcs.EncodeTo64(this.Titolo));
                        op.Parametro.Add("CodSistema", _codsistema);
                        op.Parametro.Add("IDSistema", _idsistema);
                        op.Parametro.Add("IDGruppo", _idgruppo);
                        op.Parametro.Add("InfoSistema", _infosistema);

                        op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                        op.TimeStamp.CodAzione = _azione.ToString();

                        op.MovScheda = this.MovScheda;

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);

                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovAppuntamenti", spcoll);

                        if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                        {
                            _idmovappuntamento = dt.Rows[0][0].ToString();

                            foreach (MovAppuntamentoAgende oMaa in _elementi)
                            {
                                if (oMaa.Selezionata == true && oMaa.Modificata == true)
                                {
                                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                    op.Parametro.Add("IDAppuntamento", _idmovappuntamento);
                                    op.Parametro.Add("CodAgenda", oMaa.CodAgenda);
                                    op.Parametro.Add("CodRaggr1", oMaa.CodRaggr1);
                                    op.Parametro.Add("DescrRaggr1", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(oMaa.DescrRaggr1));
                                    op.Parametro.Add("CodRaggr2", oMaa.CodRaggr2);
                                    op.Parametro.Add("DescrRaggr2", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(oMaa.DescrRaggr2));
                                    op.Parametro.Add("CodRaggr3", oMaa.CodRaggr3);
                                    op.Parametro.Add("DescrRaggr3", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(oMaa.DescrRaggr3));
                                    op.TimeStamp.CodEntita = EnumEntita.AGE.ToString();
                                    op.TimeStamp.CodAzione = _azione.ToString();

                                    op.MovScheda = this.MovScheda;

                                    spcoll = new SqlParameterExt[1];
                                    xmlParam = XmlProcs.XmlSerializeToString(op);

                                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                    DataTable dtAge = Database.GetDataTableStoredProc("MSP_InsMovAppuntamentiAgende", spcoll);

                                }
                            }

                            this.MovScheda.IDEntita = _idmovappuntamento;
                            this.MovScheda.Azione = _azione;

                            this.MovScheda.Salva(RicaricaScheda);

                            if (RicaricaMovimento)
                            {
                                Carica(_idmovappuntamento);
                            }
                        }
                        else
                        {
                            bReturn = false;
                        }
                        break;

                    case EnumAzioni.MOD:
                    case EnumAzioni.ANN:
                    case EnumAzioni.CAN:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDAppuntamento", _idmovappuntamento);
                        op.Parametro.Add("CodUA", _codua);
                        op.Parametro.Add("IDPaziente", _idpaziente);
                        op.Parametro.Add("IDEpisodio", _idepisodio);
                        op.Parametro.Add("IDTrasferimento", _idtrasferimento);
                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                        op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                        op.Parametro.Add("CodTipoAppuntamento", _codtipoappuntamento);
                        op.Parametro.Add("CodStatoAppuntamento", _codstatoappuntamento);
                        op.Parametro.Add("ElencoRisorse", _elencorisorse);
                        op.Parametro.Add("Titolo", RtfProcs.EncodeTo64(this.Titolo));
                        op.Parametro.Add("CodSistema", _codsistema);
                        op.Parametro.Add("IDSistema", _idsistema);
                        op.Parametro.Add("IDGruppo", _idgruppo);
                        op.Parametro.Add("InfoSistema", _infosistema);

                        op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                        op.TimeStamp.CodAzione = _azione.ToString();

                        op.MovScheda = this.MovScheda;

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);

                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        Database.ExecStoredProc("MSP_AggMovAppuntamenti", spcoll);

                        if (_azione == EnumAzioni.MOD)
                        {
                            foreach (MovAppuntamentoAgende oMaa in _elementi)
                            {
                                if (oMaa.Selezionata == true && oMaa.Modificata == true)
                                {
                                    if (oMaa.ID == "")
                                    {
                                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                        op.Parametro.Add("IDAppuntamento", _idmovappuntamento);
                                        op.Parametro.Add("CodAgenda", oMaa.CodAgenda);
                                        op.Parametro.Add("CodRaggr1", oMaa.CodRaggr1);
                                        op.Parametro.Add("DescrRaggr1", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(oMaa.DescrRaggr1));
                                        op.Parametro.Add("CodRaggr2", oMaa.CodRaggr2);
                                        op.Parametro.Add("DescrRaggr2", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(oMaa.DescrRaggr2));
                                        op.Parametro.Add("CodRaggr3", oMaa.CodRaggr3);
                                        op.Parametro.Add("DescrRaggr3", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(oMaa.DescrRaggr3));
                                        op.TimeStamp.CodEntita = EnumEntita.AGE.ToString();
                                        op.TimeStamp.CodAzione = _azione.ToString();

                                        op.MovScheda = this.MovScheda;

                                        spcoll = new SqlParameterExt[1];
                                        xmlParam = XmlProcs.XmlSerializeToString(op);

                                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                        DataTable dtIns = Database.GetDataTableStoredProc("MSP_InsMovAppuntamentiAgende", spcoll);
                                    }
                                    else
                                    {
                                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                        op.Parametro.Add("IDAppuntamentoAgenda", oMaa.ID);
                                        op.Parametro.Add("IDAppuntamento", _idmovappuntamento);
                                        op.Parametro.Add("CodAgenda", oMaa.CodAgenda);
                                        op.Parametro.Add("CodRaggr1", oMaa.CodRaggr1);
                                        op.Parametro.Add("DescrRaggr1", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(oMaa.DescrRaggr1));
                                        op.Parametro.Add("CodRaggr2", oMaa.CodRaggr2);
                                        op.Parametro.Add("DescrRaggr2", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(oMaa.DescrRaggr2));
                                        op.Parametro.Add("CodRaggr3", oMaa.CodRaggr3);
                                        op.Parametro.Add("DescrRaggr3", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(oMaa.DescrRaggr3));
                                        op.TimeStamp.CodEntita = EnumEntita.AGE.ToString();
                                        op.TimeStamp.CodAzione = _azione.ToString();

                                        op.MovScheda = this.MovScheda;

                                        spcoll = new SqlParameterExt[1];
                                        xmlParam = XmlProcs.XmlSerializeToString(op);

                                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                        DataTable dtIns = Database.GetDataTableStoredProc("MSP_AggMovAppuntamentiAgende", spcoll);
                                    }
                                }
                                else if (oMaa.Selezionata == false && oMaa.Modificata == true)
                                {
                                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                    op.Parametro.Add("IDAppuntamentoAgenda", oMaa.ID);
                                    op.Parametro.Add("IDAppuntamento", _idmovappuntamento);
                                    op.Parametro.Add("CodAgenda", oMaa.CodAgenda);
                                    op.Parametro.Add("CodStatoAppuntamentoAgenda", EnumStatoAppuntamentoAgenda.CA.ToString());

                                    op.TimeStamp.CodEntita = EnumEntita.AGE.ToString();
                                    op.TimeStamp.CodAzione = _azione.ToString();

                                    op.MovScheda = this.MovScheda;

                                    spcoll = new SqlParameterExt[1];
                                    xmlParam = XmlProcs.XmlSerializeToString(op);

                                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                    DataTable dtAgg = Database.GetDataTableStoredProc("MSP_AggMovAppuntamentiAgende", spcoll);
                                }
                            }
                        }
                        else if (_azione == EnumAzioni.CAN)
                        {
                            foreach (MovAppuntamentoAgende oMaa in _elementi)
                            {
                                if (oMaa.Selezionata == true)
                                {
                                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                    op.Parametro.Add("IDAppuntamentoAgenda", oMaa.ID);
                                    op.Parametro.Add("IDAppuntamento", _idmovappuntamento);
                                    op.Parametro.Add("CodAgenda", oMaa.CodAgenda);
                                    op.Parametro.Add("CodStatoAppuntamentoAgenda", EnumStatoAppuntamentoAgenda.CA.ToString());
                                    op.TimeStamp.CodEntita = EnumEntita.AGE.ToString();
                                    op.TimeStamp.CodAzione = _azione.ToString();

                                    op.MovScheda = this.MovScheda;

                                    spcoll = new SqlParameterExt[1];
                                    xmlParam = XmlProcs.XmlSerializeToString(op);

                                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                    DataTable dtAgg = Database.GetDataTableStoredProc("MSP_AggMovAppuntamentiAgende", spcoll);
                                }
                            }
                        }
                        this.MovScheda.Azione = _azione;
                        this.MovScheda.Salva();
                        break;

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovAppuntamento.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

    }

    [Serializable()]
    public class MovAppuntamentoAgende
    {

        private string _idmovappuntamentoagende = string.Empty;
        private string _codagenda = string.Empty;
        private string _descrizione = string.Empty;
        private byte[] _icona = null;
        private bool _selezionata = false;
        private bool _modificata = false;
        private string _codraggr1 = string.Empty;
        private string _descrraggr1 = string.Empty;
        private string _codraggr2 = string.Empty;
        private string _descrraggr2 = string.Empty;
        private string _codraggr3 = string.Empty;
        private string _descrraggr3 = string.Empty;

        public MovAppuntamentoAgende()
        {
        }
        public MovAppuntamentoAgende(string id, string codagenda, string descrizione, byte[] icona, bool selezionata)
        {
            _idmovappuntamentoagende = id;
            _codagenda = codagenda;
            _descrizione = descrizione;
            _icona = icona;
            _selezionata = selezionata;
        }
        public MovAppuntamentoAgende(string id, string codagenda, string descrizione, byte[] icona, bool selezionata,
                                        string codraggr1, string descrraggr1, string codraggr2, string descrraggr2, string codraggr3, string descrraggr3)
        {
            _idmovappuntamentoagende = id;
            _codagenda = codagenda;
            _descrizione = descrizione;
            _icona = icona;
            _selezionata = selezionata;
            _codraggr1 = codraggr1;
            _descrraggr1 = descrraggr1;
            _codraggr2 = codraggr2;
            _descrraggr2 = descrraggr2;
            _codraggr3 = codraggr3;
            _descrraggr3 = descrraggr3;
        }

        public string ID
        {
            get { return _idmovappuntamentoagende; }
            set { _idmovappuntamentoagende = value; }
        }

        public string CodAgenda
        {
            get { return _codagenda; }
            set { _codagenda = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        public byte[] Icona
        {
            get { return _icona; }
            set { _icona = value; }
        }

        public bool Selezionata
        {
            get { return _selezionata; }
            set { _selezionata = value; }
        }

        public bool Modificata
        {
            get { return _modificata; }
            set { _modificata = value; }
        }

        public string CodRaggr1
        {
            get { return _codraggr1; }
            set { _codraggr1 = value; }
        }

        public string DescrRaggr1
        {
            get { return _descrraggr1; }
            set { _descrraggr1 = value; }
        }

        public string CodRaggr2
        {
            get { return _codraggr2; }
            set { _codraggr2 = value; }
        }

        public string DescrRaggr2
        {
            get { return _descrraggr2; }
            set { _descrraggr2 = value; }
        }

        public string CodRaggr3
        {
            get { return _codraggr3; }
            set { _codraggr3 = value; }
        }

        public string DescrRaggr3
        {
            get { return _descrraggr3; }
            set { _descrraggr3 = value; }
        }

    }

    [Serializable()]
    public class MovNoteAgende
    {

        private string _idmovnota = string.Empty;

        private string _oggetto = string.Empty;
        private string _descrizione = string.Empty;
        private DateTime _datainizio = DateTime.MinValue;
        private DateTime _datafine = DateTime.MinValue;
        private string _colore = string.Empty;
        private string _codstatonota = string.Empty;
        private string _codagenda = string.Empty;
        private string _descragenda = string.Empty;
        private string _idgruppo = string.Empty;
        private bool _tuttoilgiorno = false;
        private bool _escludidisponibilita = false;

        private AppointmentRecurrence _appointmentrecurrence = null;

        private EnumAzioni _azione = EnumAzioni.MOD;

        public MovNoteAgende()
        {
        }
        public MovNoteAgende(string idmovnota, EnumAzioni azione)
        {
            _azione = azione;
            _idmovnota = idmovnota;
            this.Carica(idmovnota);
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDNota
        {
            get { return _idmovnota; }
            set { _idmovnota = value; }
        }

        public string Oggetto
        {
            get { return _oggetto; }
            set { _oggetto = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        public DateTime DataInizio
        {
            get { return _datainizio; }
            set { _datainizio = value; }
        }

        public DateTime DataFine
        {
            get { return _datafine; }
            set { _datafine = value; }
        }

        public string Colore
        {
            get { return _colore; }
            set { _colore = value; }
        }

        public string CodStatoNota
        {
            get { return _codstatonota; }
            set { _codstatonota = value; }
        }

        public string CodAgenda
        {
            get { return _codagenda; }
            set { _codagenda = value; }
        }

        public string DescrAgenda
        {
            get { return _descragenda; }
            set { _descragenda = value; }
        }

        public string IDGruppo
        {
            get { return _idgruppo; }
            set { _idgruppo = value; }
        }

        public AppointmentRecurrence AppointmentRecurrence
        {
            get { return _appointmentrecurrence; }
            set { _appointmentrecurrence = value; }
        }

        public bool TuttoIlGiorno
        {
            get { return _tuttoilgiorno; }
            set { _tuttoilgiorno = value; }
        }

        public bool EscludiDisponibilita
        {
            get { return _escludidisponibilita; }
            set { _escludidisponibilita = value; }
        }

        private void Carica(string idmovnota)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDNota", idmovnota);
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                op.TimeStamp.CodEntita = EnumEntita.NTE.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovNoteAgende", spcoll);

                if (dt.Rows.Count > 0)
                {

                    _idmovnota = dt.Rows[0]["ID"].ToString();
                    if (!dt.Rows[0].IsNull("Oggetto")) _oggetto = dt.Rows[0]["Oggetto"].ToString();
                    if (!dt.Rows[0].IsNull("Descrizione")) _descrizione = dt.Rows[0]["Descrizione"].ToString();
                    if (!dt.Rows[0].IsNull("DataInizio")) _datainizio = (DateTime)dt.Rows[0]["DataInizio"];
                    if (!dt.Rows[0].IsNull("DataFine")) _datafine = (DateTime)dt.Rows[0]["DataFine"];
                    if (!dt.Rows[0].IsNull("Colore")) _colore = dt.Rows[0]["Colore"].ToString();
                    if (!dt.Rows[0].IsNull("CodStatoNota")) _codstatonota = dt.Rows[0]["CodStatoNota"].ToString();
                    if (!dt.Rows[0].IsNull("CodAgenda")) _codagenda = dt.Rows[0]["CodAgenda"].ToString();
                    if (!dt.Rows[0].IsNull("DescrAgenda")) _descragenda = dt.Rows[0]["DescrAgenda"].ToString();
                    if (!dt.Rows[0].IsNull("IDGruppo")) _idgruppo = dt.Rows[0]["IDGruppo"].ToString();
                    if (!dt.Rows[0].IsNull("TuttoIlGiorno")) _tuttoilgiorno = (bool)dt.Rows[0]["TuttoIlGiorno"];
                    if (!dt.Rows[0].IsNull("EscludiDisponibilita")) _escludidisponibilita = (bool)dt.Rows[0]["EscludiDisponibilita"];

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void AddRecurrence()
        {

            try
            {

                MovNoteAgende _App = null;

                DateTime _DtStartDate = new DateTime(1, 1, 1, _datainizio.Hour, _datainizio.Minute, _datainizio.Second);
                _DtStartDate = _DtStartDate.AddDays(_appointmentrecurrence.RangeStartDate.Day - 1);
                _DtStartDate = _DtStartDate.AddMonths(_appointmentrecurrence.RangeStartDate.Month - 1);
                _DtStartDate = _DtStartDate.AddYears(_appointmentrecurrence.RangeStartDate.Year - 1);
                DateTime _DtEndDate = new DateTime(1, 1, 1, _datafine.Hour, _datafine.Minute, _datafine.Second);
                _DtEndDate = _DtEndDate.AddDays(_appointmentrecurrence.RangeStartDate.Day - 1);
                _DtEndDate = _DtEndDate.AddMonths(_appointmentrecurrence.RangeStartDate.Month - 1);
                _DtEndDate = _DtEndDate.AddYears(_appointmentrecurrence.RangeStartDate.Year - 1);

                if (_appointmentrecurrence.RangeLimit == RecurrenceRangeLimit.LimitByNumberOfOccurrences)
                {

                    for (int x = 1; x <= _appointmentrecurrence.RangeMaxOccurrences - 1; x++)
                    {

                        if (_appointmentrecurrence.PatternFrequency == RecurrencePatternFrequency.Daily)
                        {

                            if (_appointmentrecurrence.PatternDaysOfWeek == RecurrencePatternDaysOfWeek.All)
                            {
                                _DtStartDate = _DtStartDate.AddDays(_appointmentrecurrence.PatternInterval);
                                _DtEndDate = _DtEndDate.AddDays(_appointmentrecurrence.PatternInterval);
                            }
                            else if (_appointmentrecurrence.PatternDaysOfWeek == RecurrencePatternDaysOfWeek.AllWeekdays)
                            {
                                _DtStartDate = _DtStartDate.AddDays(1);
                                _DtEndDate = _DtEndDate.AddDays(1);
                                if (_DtStartDate.DayOfWeek == System.DayOfWeek.Saturday)
                                {
                                    _DtStartDate = _DtStartDate.AddDays(2);
                                    _DtEndDate = _DtEndDate.AddDays(2);
                                }
                                else if (_DtStartDate.DayOfWeek == System.DayOfWeek.Sunday)
                                {
                                    _DtStartDate = _DtStartDate.AddDays(1);
                                    _DtEndDate = _DtEndDate.AddDays(1);
                                }
                            }
                            _App = new MovNoteAgende();
                            _App.CopiaDaOrigine(this);
                            _App.Azione = EnumAzioni.INS;
                            _App.DataInizio = _DtStartDate;
                            _App.DataFine = _DtEndDate;
                            _App.Salva();
                        }
                        else if (_appointmentrecurrence.PatternFrequency == RecurrencePatternFrequency.Weekly)
                        {

                            Boolean bCheckDay = false;
                            while (!(bCheckDay == true))
                            {

                                _DtStartDate = _DtStartDate.AddDays(1);
                                _DtEndDate = _DtEndDate.AddDays(1);

                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Monday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Monday) == RecurrencePatternDaysOfWeek.Monday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Tuesday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Tuesday) == RecurrencePatternDaysOfWeek.Tuesday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Wednesday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Wednesday) == RecurrencePatternDaysOfWeek.Wednesday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Thursday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Thursday) == RecurrencePatternDaysOfWeek.Thursday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Friday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Friday) == RecurrencePatternDaysOfWeek.Friday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Saturday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Saturday) == RecurrencePatternDaysOfWeek.Saturday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Sunday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Sunday) == RecurrencePatternDaysOfWeek.Sunday)
                                {
                                    bCheckDay = true;
                                }

                                if (bCheckDay == true)
                                {
                                    _App = new MovNoteAgende();
                                    _App.CopiaDaOrigine(this);
                                    _App.Azione = EnumAzioni.INS;
                                    _App.DataInizio = _DtStartDate;
                                    _App.DataFine = _DtEndDate;
                                    _App.Salva();
                                }
                                if (_DtStartDate.DayOfWeek == System.DayOfWeek.Sunday)
                                {
                                    _DtStartDate = _DtStartDate.AddDays(7 * (_appointmentrecurrence.PatternInterval - 1));
                                    _DtEndDate = _DtEndDate.AddDays(7 * (_appointmentrecurrence.PatternInterval - 1));
                                }

                            }

                        }

                    }

                }
                else if (_appointmentrecurrence.RangeLimit == RecurrenceRangeLimit.LimitByDate)
                {

                    while (_DtStartDate <= _appointmentrecurrence.RangeEndDate)
                    {

                        if (_appointmentrecurrence.PatternFrequency == RecurrencePatternFrequency.Daily)
                        {

                            if (_appointmentrecurrence.PatternDaysOfWeek == RecurrencePatternDaysOfWeek.All)
                            {
                                _DtStartDate = _DtStartDate.AddDays(_appointmentrecurrence.PatternInterval);
                                _DtEndDate = _DtEndDate.AddDays(_appointmentrecurrence.PatternInterval);
                            }
                            else if (_appointmentrecurrence.PatternDaysOfWeek == RecurrencePatternDaysOfWeek.AllWeekdays)
                            {
                                _DtStartDate = _DtStartDate.AddDays(1);
                                _DtEndDate = _DtEndDate.AddDays(1);
                                if (_DtStartDate.DayOfWeek == System.DayOfWeek.Saturday)
                                {
                                    _DtStartDate = _DtStartDate.AddDays(2);
                                    _DtEndDate = _DtEndDate.AddDays(2);
                                }
                                else if (_DtStartDate.DayOfWeek == System.DayOfWeek.Sunday)
                                {
                                    _DtStartDate = _DtStartDate.AddDays(1);
                                    _DtEndDate = _DtEndDate.AddDays(1);
                                }
                            }

                            if (_DtStartDate.Date > _appointmentrecurrence.RangeEndDate)
                            {
                                break;
                            }
                            _App = new MovNoteAgende();
                            _App.CopiaDaOrigine(this);
                            _App.Azione = EnumAzioni.INS;
                            _App.DataInizio = _DtStartDate;
                            _App.DataFine = _DtEndDate;
                            _App.Salva();
                        }
                        else if (_appointmentrecurrence.PatternFrequency == RecurrencePatternFrequency.Weekly)
                        {

                            Boolean bCheckDay = false;
                            while (!(bCheckDay == true))
                            {

                                _DtStartDate = _DtStartDate.AddDays(1);
                                _DtEndDate = _DtEndDate.AddDays(1);

                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Monday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Monday) == RecurrencePatternDaysOfWeek.Monday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Tuesday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Tuesday) == RecurrencePatternDaysOfWeek.Tuesday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Wednesday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Wednesday) == RecurrencePatternDaysOfWeek.Wednesday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Thursday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Thursday) == RecurrencePatternDaysOfWeek.Thursday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Friday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Friday) == RecurrencePatternDaysOfWeek.Friday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Saturday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Saturday) == RecurrencePatternDaysOfWeek.Saturday)
                                {
                                    bCheckDay = true;
                                }
                                if ((_DtStartDate.DayOfWeek == System.DayOfWeek.Sunday) & (_appointmentrecurrence.PatternDaysOfWeek & RecurrencePatternDaysOfWeek.Sunday) == RecurrencePatternDaysOfWeek.Sunday)
                                {
                                    bCheckDay = true;
                                }

                                if (bCheckDay == true)
                                {
                                    _App = new MovNoteAgende();
                                    _App.CopiaDaOrigine(this);
                                    _App.Azione = EnumAzioni.INS;
                                    _App.DataInizio = _DtStartDate;
                                    _App.DataFine = _DtEndDate;
                                    _App.Salva();
                                }
                                if (_DtStartDate.DayOfWeek == System.DayOfWeek.Sunday)
                                {
                                    _DtStartDate = _DtStartDate.AddDays(7 * (_appointmentrecurrence.PatternInterval - 1));
                                    _DtEndDate = _DtEndDate.AddDays(7 * (_appointmentrecurrence.PatternInterval - 1));
                                }
                                if (_DtStartDate.Date >= _appointmentrecurrence.RangeEndDate)
                                {
                                    bCheckDay = true;
                                }

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(@"MovNoteAgende.AddRecurrence()" + Environment.NewLine + ex.Message, ex);
            }

        }

        private void CopiaDaOrigine(MovNoteAgende movnotaagendaorigine)
        {

            try
            {

                _oggetto = movnotaagendaorigine.Oggetto;
                _descrizione = movnotaagendaorigine.Descrizione;
                _datainizio = movnotaagendaorigine.DataInizio;
                _datafine = movnotaagendaorigine.DataFine;
                _colore = movnotaagendaorigine.Colore;
                _codstatonota = movnotaagendaorigine.CodStatoNota;
                _codagenda = movnotaagendaorigine.CodAgenda;
                _idgruppo = movnotaagendaorigine.IDGruppo;
                _tuttoilgiorno = movnotaagendaorigine.TuttoIlGiorno;
                _escludidisponibilita = movnotaagendaorigine.EscludiDisponibilita;

            }
            catch (Exception ex)
            {
                throw new Exception(@"MovNoteAgende.CopiaDaOrigine()" + Environment.NewLine + ex.Message, ex);
            }

        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {

                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";

                switch (_azione)
                {

                    case EnumAzioni.INS:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("Oggetto", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(_oggetto));
                        op.Parametro.Add("Descrizione", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(_descrizione));

                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                        op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                        op.Parametro.Add("Colore", RtfProcs.EncodeTo64(_colore));
                        op.Parametro.Add("CodStatoNota", _codstatonota);
                        op.Parametro.Add("CodAgenda", _codagenda);
                        op.Parametro.Add("IDGruppo", _idgruppo);
                        op.Parametro.Add("Ricorrenza", (_appointmentrecurrence == null ? "0" : "1"));
                        op.Parametro.Add("TuttoIlGiorno", (_tuttoilgiorno == false ? "0" : "1"));
                        op.Parametro.Add("EscludiDisponibilita", (_escludidisponibilita == false ? "0" : "1"));

                        op.TimeStamp.CodAzione = _azione.ToString();
                        op.TimeStamp.CodEntita = EnumEntita.NTE.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovNoteAgende", spcoll);

                        if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                        {
                            _idmovnota = dt.Rows[0][0].ToString();
                            Carica(_idmovnota);
                            if (_appointmentrecurrence != null) { AddRecurrence(); }
                        }
                        else
                        {
                            bReturn = false;
                        }
                        break;

                    case EnumAzioni.MOD:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDNotaAgenda", _idmovnota);
                        op.Parametro.Add("Oggetto", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(_oggetto));
                        op.Parametro.Add("Descrizione", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(_descrizione));

                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                        op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                        op.Parametro.Add("Colore", RtfProcs.EncodeTo64(_colore));
                        op.Parametro.Add("CodStatoNota", _codstatonota);
                        op.Parametro.Add("CodAgenda", _codagenda);
                        op.Parametro.Add("IDGruppo", _idgruppo);
                        op.Parametro.Add("TuttoIlGiorno", (_tuttoilgiorno == false ? "0" : "1"));
                        op.Parametro.Add("EscludiDisponibilita", (_escludidisponibilita == false ? "0" : "1"));
                        op.TimeStamp.CodAzione = _azione.ToString();
                        op.TimeStamp.CodEntita = EnumEntita.NTE.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        Database.ExecStoredProc("MSP_AggMovNoteAgende", spcoll);

                        break;

                    case EnumAzioni.CAN:
                        if (_idgruppo == string.Empty)
                        {
                            op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("IDNotaAgenda", _idmovnota);
                            op.Parametro.Add("Oggetto", RtfProcs.EncodeTo64(_oggetto));
                            op.Parametro.Add("Descrizione", RtfProcs.EncodeTo64(_descrizione));
                            op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                            op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                            op.Parametro.Add("Colore", RtfProcs.EncodeTo64(_colore));
                            op.Parametro.Add("CodStatoNota", _codstatonota);
                            op.Parametro.Add("CodAgenda", _codagenda);
                            op.Parametro.Add("IDGruppo", _idgruppo);
                            op.TimeStamp.CodAzione = _azione.ToString();
                            op.TimeStamp.CodEntita = EnumEntita.NTE.ToString();

                            spcoll = new SqlParameterExt[1];
                            xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            Database.ExecStoredProc("MSP_AggMovNoteAgende", spcoll);
                        }
                        else
                        {
                            op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("IDGruppo", _idgruppo);
                            op.TimeStamp.CodAzione = _azione.ToString();
                            op.TimeStamp.CodEntita = EnumEntita.NTE.ToString();

                            spcoll = new SqlParameterExt[1];
                            xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            Database.ExecStoredProc("MSP_AggMovNoteGruppo", spcoll);
                        }

                        break;

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovNoteAgende.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

    }

    [Serializable()]
    public class MovNota
    {

        private string _idmovnota = string.Empty;

        private string _idpaziente = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idtrasferimento = string.Empty;
        private string _codentita = string.Empty;
        private string _identita = string.Empty;

        private string _oggetto = string.Empty;
        private string _descrizione = string.Empty;
        private DateTime _datainizio = DateTime.MinValue;
        private DateTime _datafine = DateTime.MinValue;
        private string _codstatonota = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;

        public MovNota(string idpaziente, string idepisodio, string idtrasferimento, string codentita, string identita)
        {
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _codentita = codentita;
            _identita = identita;
            _codstatonota = EnumStatoNota.PR.ToString();
        }
        public MovNota(string idmovnota, EnumAzioni azione)
        {
            _azione = azione;
            _idmovnota = idmovnota;
            this.Carica(idmovnota);
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
            set { _idpaziente = value; }
        }

        public string IDEpisodio
        {
            get { return _idepisodio; }
            set { _idepisodio = value; }
        }

        public string IDTrasferimento
        {
            get { return _idtrasferimento; }
            set { _idtrasferimento = value; }
        }

        public string CodEntita
        {
            get { return _codentita; }
            set { _codentita = value; }
        }

        public string IDEntita
        {
            get { return _identita; }
            set { _identita = value; }
        }

        public string IDNota
        {
            get { return _idmovnota; }
            set { _idmovnota = value; }
        }

        public string Oggetto
        {
            get { return _oggetto; }
            set { _oggetto = value; }
        }
        public string OggettoTXTxParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.Oggetto); }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        public DateTime DataInizio
        {
            get { return _datainizio; }
            set { _datainizio = value; }
        }

        public DateTime DataFine
        {
            get { return _datafine; }
            set { _datafine = value; }
        }

        public string CodStatoNota
        {
            get { return _codstatonota; }
            set { _codstatonota = value; }
        }

        private void Carica(string idmovnota)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDNota", idmovnota);
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                op.TimeStamp.CodEntita = EnumEntita.NTG.ToString();
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovNote", spcoll);

                if (dt.Rows.Count > 0)
                {

                    _idmovnota = dt.Rows[0]["ID"].ToString();

                    if (!dt.Rows[0].IsNull("IDPaziente")) _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    if (!dt.Rows[0].IsNull("IDEpisodio")) _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    if (!dt.Rows[0].IsNull("IDTrasferimento")) _idtrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();
                    if (!dt.Rows[0].IsNull("CodEntita")) _codentita = dt.Rows[0]["CodEntita"].ToString();
                    if (!dt.Rows[0].IsNull("IDEntita")) _identita = dt.Rows[0]["IDEntita"].ToString();

                    if (!dt.Rows[0].IsNull("Oggetto")) _oggetto = dt.Rows[0]["Oggetto"].ToString();
                    if (!dt.Rows[0].IsNull("Descrizione")) _descrizione = dt.Rows[0]["Descrizione"].ToString();
                    if (!dt.Rows[0].IsNull("DataInizio")) _datainizio = (DateTime)dt.Rows[0]["DataInizio"];
                    if (!dt.Rows[0].IsNull("DataFine")) _datafine = (DateTime)dt.Rows[0]["DataFine"];
                    if (!dt.Rows[0].IsNull("CodStatoNota")) _codstatonota = dt.Rows[0]["CodStatoNota"].ToString();

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {

                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";

                switch (_azione)
                {

                    case EnumAzioni.INS:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDPaziente", _idpaziente);
                        op.Parametro.Add("IDEpisodio", _idepisodio);
                        op.Parametro.Add("IDTrasferimento", _idtrasferimento);
                        op.Parametro.Add("CodEntita", _codentita);
                        op.Parametro.Add("IDEntita", _identita);
                        op.Parametro.Add("Oggetto", this.OggettoTXTxParamStored);
                        op.Parametro.Add("Descrizione", RtfProcs.EncodeTo64(_descrizione));
                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                        op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                        op.Parametro.Add("CodStatoNota", _codstatonota);

                        op.TimeStamp.CodAzione = _azione.ToString();
                        op.TimeStamp.CodEntita = EnumEntita.NTG.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovNote", spcoll);

                        if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                        {
                            _idmovnota = dt.Rows[0][0].ToString();
                            Carica(_idmovnota);
                        }
                        else
                        {
                            bReturn = false;
                        }
                        break;

                    case EnumAzioni.MOD:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDNota", _idmovnota);
                        op.Parametro.Add("IDPaziente", _idpaziente);
                        op.Parametro.Add("IDEpisodio", _idepisodio);
                        op.Parametro.Add("IDTrasferimento", _idtrasferimento);
                        op.Parametro.Add("CodEntita", _codentita);
                        op.Parametro.Add("IDEntita", _identita);
                        op.Parametro.Add("Oggetto", this.OggettoTXTxParamStored);
                        op.Parametro.Add("Descrizione", RtfProcs.EncodeTo64(_descrizione));
                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                        op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                        op.Parametro.Add("CodStatoNota", _codstatonota);
                        op.TimeStamp.CodAzione = _azione.ToString();
                        op.TimeStamp.CodEntita = EnumEntita.NTG.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        Database.ExecStoredProc("MSP_AggMovNote", spcoll);

                        break;

                    case EnumAzioni.CAN:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDNota", _idmovnota);
                        op.Parametro.Add("IDPaziente", _idpaziente);
                        op.Parametro.Add("IDEpisodio", _idepisodio);
                        op.Parametro.Add("IDTrasferimento", _idtrasferimento);
                        op.Parametro.Add("CodEntita", _codentita);
                        op.Parametro.Add("IDEntita", _identita);
                        op.Parametro.Add("Oggetto", this.OggettoTXTxParamStored);
                        op.Parametro.Add("Descrizione", RtfProcs.EncodeTo64(_descrizione));
                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                        op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                        op.Parametro.Add("CodStatoNota", _codstatonota);
                        op.TimeStamp.CodAzione = _azione.ToString();
                        op.TimeStamp.CodEntita = EnumEntita.NTG.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        Database.ExecStoredProc("MSP_AggMovNote", spcoll);

                        break;

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovNote.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

    }

    [Serializable()]
    public class MovTestoPredefinito
    {

        public MovTestoPredefinito(string codentita, string codua, string codruolo, string codtipoentita, string idcampo)
        {
            this.CodEntita = codentita;
            this.CodUA = codua;
            this.CodRuolo = codruolo;
            this.CodTipoEntita = codtipoentita;
            this.IDCampo = idcampo;
            this.RitornoRTF = string.Empty;
        }

        public string CodEntita { get; set; }
        public string CodUA { get; set; }
        public string CodRuolo { get; set; }
        public string CodTipoEntita { get; set; }
        public string IDCampo { get; set; }
        public string RitornoRTF { get; set; }

    }

    [Serializable()]
    public class MovCartellaInVisione
    {

        private string _idmovcartellainvisione = string.Empty;
        private string _idcartella = string.Empty;
        private string _codruoloinvisione = string.Empty;
        private string _descrruoloinvisione = string.Empty;
        private DateTime _datainizio = DateTime.MinValue;
        private DateTime _datafine = DateTime.MinValue;
        private string _codstatocartellainvisione = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;

        public MovCartellaInVisione()
        {
            _azione = EnumAzioni.INS;
            _codstatocartellainvisione = EnumStatoCartelleInVisione.IC.ToString();
        }
        public MovCartellaInVisione(string idmovcartellainvisione)
        {
            _azione = EnumAzioni.MOD;
            _idmovcartellainvisione = idmovcartellainvisione;
            this.Carica(idmovcartellainvisione);
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDMovCartellaInVisione
        {
            get { return _idmovcartellainvisione; }
            set { _idmovcartellainvisione = value; }
        }

        public string IDCartella
        {
            get { return _idcartella; }
            set { _idcartella = value; }
        }

        public string CodRuoloInVisione
        {
            get { return _codruoloinvisione; }
            set { _codruoloinvisione = value; }
        }

        public string DescrRuoloInVisione
        {
            get { return _descrruoloinvisione; }
            set { _descrruoloinvisione = value; }
        }

        public DateTime DataInizio
        {
            get { return _datainizio; }
            set { _datainizio = value; }
        }

        public DateTime DataFine
        {
            get { return _datafine; }
            set { _datafine = value; }
        }

        public string CodStatoCartellaInVisione
        {
            get { return _codstatocartellainvisione; }
            set { _codstatocartellainvisione = value; }
        }

        private void Carica(string idmovcartellainvisione)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDCartellaInVisione", idmovcartellainvisione);

                op.TimeStamp.CodEntita = EnumEntita.CIV.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovCartelleInVisione", spcoll);

                if (dt.Rows.Count > 0)
                {

                    _idmovcartellainvisione = dt.Rows[0]["ID"].ToString();
                    _idcartella = dt.Rows[0]["IDCartella"].ToString();
                    _codruoloinvisione = dt.Rows[0]["CodRuoloInVisione"].ToString();
                    _descrruoloinvisione = dt.Rows[0]["DescrRuoloInVisione"].ToString();
                    if (!dt.Rows[0].IsNull("DataInizio")) _datainizio = (DateTime)dt.Rows[0]["DataInizio"];
                    if (!dt.Rows[0].IsNull("DataFine")) _datafine = (DateTime)dt.Rows[0]["DataFine"];
                    _codstatocartellainvisione = dt.Rows[0]["CodStatoCartellaInVisione"].ToString();

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {

                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";

                switch (_azione)
                {

                    case EnumAzioni.INS:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDCartella", _idcartella);
                        op.Parametro.Add("CodRuoloInVisione", _codruoloinvisione);
                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                        op.Parametro.Add("DataInizioUTC", Database.dataOra105PerParametri(_datainizio.ToUniversalTime()));
                        op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                        op.Parametro.Add("DataFineUTC", Database.dataOra105PerParametri(_datafine.ToUniversalTime()));
                        op.Parametro.Add("CodStatoCartellaInVisione", _codstatocartellainvisione);

                        op.TimeStamp.CodEntita = EnumEntita.CIV.ToString();
                        op.TimeStamp.CodAzione = _azione.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovCartelleInVisione", spcoll);

                        if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                        {
                            _idmovcartellainvisione = dt.Rows[0][0].ToString();
                        }
                        else
                        {
                            bReturn = false;
                        }
                        break;

                    case EnumAzioni.MOD:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDCartellaInVisione", _idmovcartellainvisione);
                        op.Parametro.Add("IDCartella", _idcartella);
                        op.Parametro.Add("CodRuoloInVisione", _codruoloinvisione);
                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                        op.Parametro.Add("DataInizioUTC", Database.dataOra105PerParametri(_datainizio.ToUniversalTime()));
                        op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                        op.Parametro.Add("DataFineUTC", Database.dataOra105PerParametri(_datafine.ToUniversalTime()));
                        op.Parametro.Add("CodStatoCartellaInVisione", _codstatocartellainvisione);

                        op.TimeStamp.CodEntita = EnumEntita.CIV.ToString();
                        op.TimeStamp.CodAzione = _azione.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_AggMovCartelleInVisione", spcoll);
                        break;

                    case EnumAzioni.CAN:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDCartellaInVisione", _idmovcartellainvisione);
                        op.Parametro.Add("CodStatoCartellaInVisione", EnumStatoCartelleInVisione.CA.ToString());

                        op.TimeStamp.CodEntita = EnumEntita.CIV.ToString();
                        op.TimeStamp.CodAzione = _azione.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_AggMovCartelleInVisione", spcoll);
                        break;

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovCartelleInVisione.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

    }

    [Serializable()]
    public class MovPazienteInVisione
    {

        private string _idmovpazienteinvisione = string.Empty;
        private string _idpaziente = string.Empty;
        private string _codruoloinvisione = string.Empty;
        private string _descrruoloinvisione = string.Empty;
        private DateTime _datainizio = DateTime.MinValue;
        private DateTime _datafine = DateTime.MinValue;
        private string _codstatopazienteinvisione = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;

        public MovPazienteInVisione()
        {
            _azione = EnumAzioni.INS;
            _codstatopazienteinvisione = EnumStatoPazientiInVisione.IC.ToString();
        }
        public MovPazienteInVisione(string idmovpazienteinvisione)
        {
            _azione = EnumAzioni.MOD;
            _idmovpazienteinvisione = idmovpazienteinvisione;
            this.Carica(idmovpazienteinvisione);
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDMovPazienteInVisione
        {
            get { return _idmovpazienteinvisione; }
            set { _idmovpazienteinvisione = value; }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
            set { _idpaziente = value; }
        }

        public string CodRuoloInVisione
        {
            get { return _codruoloinvisione; }
            set { _codruoloinvisione = value; }
        }

        public string DescrRuoloInVisione
        {
            get { return _descrruoloinvisione; }
            set { _descrruoloinvisione = value; }
        }

        public DateTime DataInizio
        {
            get { return _datainizio; }
            set { _datainizio = value; }
        }

        public DateTime DataFine
        {
            get { return _datafine; }
            set { _datafine = value; }
        }

        public string CodStatoPazienteInVisione
        {
            get { return _codstatopazienteinvisione; }
            set { _codstatopazienteinvisione = value; }
        }

        private void Carica(string idmovpazienteinvisione)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPazienteInVisione", idmovpazienteinvisione);

                op.TimeStamp.CodEntita = EnumEntita.CIV.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovPazientiInVisione", spcoll);

                if (dt.Rows.Count > 0)
                {

                    _idmovpazienteinvisione = dt.Rows[0]["ID"].ToString();
                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    _codruoloinvisione = dt.Rows[0]["CodRuoloInVisione"].ToString();
                    _descrruoloinvisione = dt.Rows[0]["DescrRuoloInVisione"].ToString();
                    if (!dt.Rows[0].IsNull("DataInizio")) _datainizio = (DateTime)dt.Rows[0]["DataInizio"];
                    if (!dt.Rows[0].IsNull("DataFine")) _datafine = (DateTime)dt.Rows[0]["DataFine"];
                    _codstatopazienteinvisione = dt.Rows[0]["CodStatoPazienteInVisione"].ToString();

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {

                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";

                switch (_azione)
                {

                    case EnumAzioni.INS:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDPaziente", _idpaziente);
                        op.Parametro.Add("CodRuoloInVisione", _codruoloinvisione);
                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                        op.Parametro.Add("DataInizioUTC", Database.dataOra105PerParametri(_datainizio.ToUniversalTime()));
                        op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                        op.Parametro.Add("DataFineUTC", Database.dataOra105PerParametri(_datafine.ToUniversalTime()));
                        op.Parametro.Add("CodStatoPazienteInVisione", _codstatopazienteinvisione);

                        op.TimeStamp.CodEntita = EnumEntita.PIV.ToString();
                        op.TimeStamp.CodAzione = _azione.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovPazientiInVisione", spcoll);

                        if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                        {
                            _idmovpazienteinvisione = dt.Rows[0][0].ToString();
                        }
                        else
                        {
                            bReturn = false;
                        }
                        break;

                    case EnumAzioni.MOD:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDPazienteInVisione", _idmovpazienteinvisione);
                        op.Parametro.Add("CodRuoloInVisione", _codruoloinvisione);
                        op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(_datainizio));
                        op.Parametro.Add("DataInizioUTC", Database.dataOra105PerParametri(_datainizio.ToUniversalTime()));
                        op.Parametro.Add("DataFine", Database.dataOra105PerParametri(_datafine));
                        op.Parametro.Add("DataFineUTC", Database.dataOra105PerParametri(_datafine.ToUniversalTime()));
                        op.Parametro.Add("CodStatoPazienteInVisione", _codstatopazienteinvisione);

                        op.TimeStamp.CodEntita = EnumEntita.PIV.ToString();
                        op.TimeStamp.CodAzione = _azione.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_AggMovPazientiInVisione", spcoll);
                        break;

                    case EnumAzioni.CAN:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDPazienteInVisione", _idmovpazienteinvisione);
                        op.Parametro.Add("CodStatoPazienteInVisione", EnumStatoPazientiInVisione.CA.ToString());

                        op.TimeStamp.CodEntita = EnumEntita.PIV.ToString();
                        op.TimeStamp.CodAzione = _azione.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_AggMovPazientiInVisione", spcoll);
                        break;

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovPazientiInVisione.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

    }

    [Serializable()]
    public class MovPazienteSeguito
    {

        private string _idmovpazienteseguito = string.Empty;
        private string _idpaziente = string.Empty;
        private string _codutente = string.Empty;
        private string _codruolo = string.Empty;
        private string _codstatopazienteseguito = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;

        public MovPazienteSeguito()
        {
            _azione = EnumAzioni.INS;
            _codstatopazienteseguito = EnumStatoPazientiSeguiti.IC.ToString();
        }
        public MovPazienteSeguito(string idmovpazienteseguito)
        {
            _azione = EnumAzioni.MOD;
            _idmovpazienteseguito = idmovpazienteseguito;
            this.Carica(idmovpazienteseguito);
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDMovPazienteSeguito
        {
            get { return _idmovpazienteseguito; }
            set { _idmovpazienteseguito = value; }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
            set { _idpaziente = value; }
        }

        public string CodUtente
        {
            get { return _codutente; }
            set { _codutente = value; }
        }

        public string CodRuolo
        {
            get { return _codruolo; }
            set { _codruolo = value; }
        }

        public string CodStatoPazienteSeguito
        {
            get { return _codstatopazienteseguito; }
            set { _codstatopazienteseguito = value; }
        }

        private void Carica(string idmovpazienteseguito)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPazienteSeguito", idmovpazienteseguito);

                op.TimeStamp.CodEntita = EnumEntita.PZS.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovPazientiSeguiti", spcoll);

                if (dt.Rows.Count > 0)
                {

                    _idmovpazienteseguito = dt.Rows[0]["ID"].ToString();
                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    _codutente = dt.Rows[0]["CodUtente"].ToString();
                    _codruolo = dt.Rows[0]["CodRuolo"].ToString();
                    _codstatopazienteseguito = dt.Rows[0]["CodStatoPazienteSeguito"].ToString();

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {

                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";

                switch (_azione)
                {

                    case EnumAzioni.INS:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDPaziente", _idpaziente);
                        op.Parametro.Add("CodUtente", _codutente);
                        op.Parametro.Add("CodRuolo", _codruolo);
                        op.Parametro.Add("CodStatoPazienteSeguito", _codstatopazienteseguito);

                        op.TimeStamp.CodEntita = EnumEntita.PZS.ToString();
                        op.TimeStamp.CodAzione = _azione.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovPazientiSeguiti", spcoll);

                        break;

                    case EnumAzioni.CAN:
                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDPazienteSeguito", _idmovpazienteseguito);
                        op.Parametro.Add("CodStatoPazienteSeguito", EnumStatoPazientiSeguiti.CA.ToString());

                        op.TimeStamp.CodEntita = EnumEntita.PZS.ToString();
                        op.TimeStamp.CodAzione = _azione.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_AggMovPazientiSeguiti", spcoll);
                        break;

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovCartelleInVisione.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

    }

    [Serializable()]
    public class ConfigUtente
    {

        private string _ruoloselezionato = string.Empty;
        private Calendari _calendari = new Calendari();

        private string[] _unitaselezionate;
        private string[] _parametriselezionati;

        private FoglioUnico _fogliounico = new FoglioUnico();

        private string _coduaambulatorialeselezionata = string.Empty;

        private string _consegnecoduaselezionata = string.Empty;

        public ConfigUtente()
        {

        }

        public String CodScreenSelezionato { get; set; }

        public string RuoloSelezionato
        {
            get
            {
                return _ruoloselezionato;
            }
            set
            {
                _ruoloselezionato = value;
            }
        }

        public Calendari Calendari
        {
            get
            {
                return _calendari;
            }
            set
            {
                _calendari = value;
            }
        }

        public string[] UnitaSelezionate
        {
            get
            {
                return _unitaselezionate;
            }
            set
            {
                _unitaselezionate = value;
            }
        }

        public string[] ParametriSelezionati
        {
            get
            {
                return _parametriselezionati;
            }
            set
            {
                _parametriselezionati = value;
            }
        }

        public FoglioUnico FoglioUnico
        {
            get
            {
                return _fogliounico;
            }
            set
            {
                _fogliounico = value;
            }
        }

        public string CodUAAmbulatorialeSelezionata
        {
            get
            {
                return _coduaambulatorialeselezionata;
            }
            set
            {
                _coduaambulatorialeselezionata = value;
            }
        }

        public string ConsegneCodUASelezionata
        {
            get
            {
                return _consegnecoduaselezionata;
            }
            set
            {
                _consegnecoduaselezionata = value;
            }
        }

        public string[] AgendeSelAppuntamentiAccessi { get; set; }

    }

    [Serializable()]
    public class DateDifference
    {

        private int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        private DateTime fromDate;

        private DateTime toDate;

        private int year;
        private int month;
        private int day;

        public DateDifference(DateTime d1, DateTime d2)
        {

            int increment;

            if (d1 > d2)
            {
                this.fromDate = d2;
                this.toDate = d1;
            }
            else
            {
                this.fromDate = d1;
                this.toDate = d2;
            }

            increment = 0;

            if (this.fromDate.Day > this.toDate.Day)
            {
                increment = this.monthDay[this.fromDate.Month - 1];
            }
            if (increment == -1)
            {
                if (DateTime.IsLeapYear(this.fromDate.Year))
                {
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }
            if (increment != 0)
            {
                day = (this.toDate.Day + increment) - this.fromDate.Day;
                increment = 1;
            }
            else
            {
                day = this.toDate.Day - this.fromDate.Day;
            }

            if ((this.fromDate.Month + increment) > this.toDate.Month)
            {
                this.month = (this.toDate.Month + 12) - (this.fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                this.month = (this.toDate.Month) - (this.fromDate.Month + increment);
                increment = 0;
            }

            this.year = this.toDate.Year - (this.fromDate.Year + increment);

        }

        public override string ToString()
        {

            string sRet = "";

            if (this.year != 0)
            {
                sRet = string.Format("{0}aa {1}m {2}g", this.year, this.month, this.day);
            }
            else if (this.month != 0)
            {
                sRet = string.Format("{0}m {1}g", this.month, this.day);
            }
            else
            {
                sRet = string.Format("{0}g", this.day);
            }

            return sRet;

        }

        public int Years
        {
            get
            {
                return this.year;
            }
        }

        public int Months
        {
            get
            {
                return this.month;
            }
        }

        public int Days
        {
            get
            {
                return this.day;
            }
        }

    }

    [Serializable()]
    public class ParametriVitaliTrasversali
    {

        private Dictionary<string, TipoParametroVitale> _listatipiparametriselezionati = new Dictionary<string, TipoParametroVitale>();
        private Dictionary<string, string> _listUAselezionate = new Dictionary<string, string>();

        public ParametriVitaliTrasversali()
        {
            _listatipiparametriselezionati = new Dictionary<string, TipoParametroVitale>();
            _listUAselezionate = new Dictionary<string, string>();
        }

        public Dictionary<string, TipoParametroVitale> ListaTipiParametriSelezionati
        {
            get
            {
                if (_listatipiparametriselezionati == null) _listatipiparametriselezionati = new Dictionary<string, TipoParametroVitale>();
                return _listatipiparametriselezionati;
            }
            set { _listatipiparametriselezionati = value; }
        }

        public Dictionary<string, string> ListaUASelezionate
        {
            get
            {
                if (_listUAselezionate == null) _listUAselezionate = new Dictionary<string, string>();
                return _listUAselezionate;
            }
            set { _listUAselezionate = value; }
        }

    }

    [Serializable()]
    public class WorkingHourTime
    {

        private int[] _workingMinutes = new int[7];

        public string[] HourI = new string[7];
        public string[] HourF = new string[7];

        public int[] WorkingMinutes
        {
            get
            {
                try
                {
                    if (_workingMinutes == null || _workingMinutes.Length < 7)
                    {
                        _workingMinutes = new int[7];
                        _workingMinutes[0] = -1;
                        _workingMinutes[1] = -1;
                        _workingMinutes[2] = -1;
                        _workingMinutes[3] = -1;
                        _workingMinutes[4] = -1;
                        _workingMinutes[5] = -1;
                        _workingMinutes[6] = -1;
                    }

                    for (int d = 0; d < 7; d++)
                    {
                        if (_workingMinutes[d] < 0)
                        {
                            _workingMinutes[d] = CalcolaMinuti(HourI[d], HourF[d]);
                        }
                    }
                }
                catch
                {
                    _workingMinutes = new int[7];
                }

                return _workingMinutes;
            }

            set { _workingMinutes = value; }
        }

        public string TimeSeparator
        {
            get
            {
                return @":";
            }
        }

        public WorkingHourTime()
        {
            HourI[0] = "00:00";
            HourI[1] = "08:00";
            HourI[2] = "08:00";
            HourI[3] = "08:00";
            HourI[4] = "08:00";
            HourI[5] = "08:00";
            HourI[6] = "08:00";

            HourF[0] = "00:00";
            HourF[1] = "18:00";
            HourF[2] = "18:00";
            HourF[3] = "18:00";
            HourF[4] = "18:00";
            HourF[5] = "18:00";
            HourF[6] = "18:00";

            _workingMinutes[0] = -1;
            _workingMinutes[1] = -1;
            _workingMinutes[2] = -1;
            _workingMinutes[3] = -1;
            _workingMinutes[4] = -1;
            _workingMinutes[5] = -1;
            _workingMinutes[6] = -1;
        }

        public static int CalcolaMinuti(string hourI, string hourF)
        {
            int iTotMinuti = 0;
            try
            {
                if (hourI != null && hourI != ""
                    && hourF != null && hourF != ""
                    && hourF != "00:00"
                    && hourI != hourF)
                {
                    DateTime tmI = DateTime.MinValue;
                    DateTime tmF = DateTime.MinValue;
                    if (DateTime.TryParse(hourI, out tmI) && DateTime.TryParse(hourF, out tmF))
                    {
                        if (tmF > tmI) iTotMinuti = (int)tmF.Subtract(tmI).TotalMinutes;
                    }

                }
            }
            catch
            {
            }
            return iTotMinuti;
        }

    }

    [Serializable()]
    public class ParametriListaAgenda
    {

        public ParametriListaAgenda()
        {

            TipoRaggruppamentoAgenda1 = EnumTipoRaggruppamentoAgenda.Nessuno;
            TipoRaggruppamentoAgenda2 = EnumTipoRaggruppamentoAgenda.Nessuno;
            TipoRaggruppamentoAgenda3 = EnumTipoRaggruppamentoAgenda.Nessuno;

            DescrizioneRaggruppamentoAgenda1 = string.Empty;
            DescrizioneRaggruppamentoAgenda2 = string.Empty;
            DescrizioneRaggruppamentoAgenda3 = string.Empty;

            RaggruppamentoAgenda1 = new SerializableDictionary<string, string>();
            RaggruppamentoAgenda2 = new SerializableDictionary<string, string>();
            RaggruppamentoAgenda3 = new SerializableDictionary<string, string>();

        }

        public EnumTipoRaggruppamentoAgenda TipoRaggruppamentoAgenda1 { get; set; }

        public EnumTipoRaggruppamentoAgenda TipoRaggruppamentoAgenda2 { get; set; }

        public EnumTipoRaggruppamentoAgenda TipoRaggruppamentoAgenda3 { get; set; }

        public string DescrizioneRaggruppamentoAgenda1 { get; set; }

        public string DescrizioneRaggruppamentoAgenda2 { get; set; }

        public string DescrizioneRaggruppamentoAgenda3 { get; set; }

        public SerializableDictionary<string, string> RaggruppamentoAgenda1 { get; set; }

        public SerializableDictionary<string, string> RaggruppamentoAgenda2 { get; set; }

        public SerializableDictionary<string, string> RaggruppamentoAgenda3 { get; set; }

    }

    [Serializable()]
    public class WorkingHourTimeDaysOfWeek
    {

        public WorkingHourTimeDaysOfWeek()
        {
            this.Clear();
        }

        public WorkingHourTimeDaysOfWeek(int daysofweek, string houri, string hourf)
        {
            this.Clear();
            this.DaysOfWeek = daysofweek;
            this.HourI = houri;
            this.HourF = hourf;
            this.CalcolaMinuti();
        }

        private void Clear()
        {
            this.DaysOfWeek = 1;
            this.HourI = "08:00";
            this.HourF = "18:00";
            this.CalcolaMinuti();
        }

        public int DaysOfWeek { get; set; }

        public string HourI { get; set; }

        public string HourF { get; set; }

        public int WorkingMinutes { get; set; }

        public void CalcolaMinuti()
        {

            this.WorkingMinutes = 0;
            try
            {
                if (this.HourI != null && this.HourI != ""
                    && this.HourF != null && this.HourF != ""
                    && this.HourF != "00:00"
                    && this.HourI != this.HourF)
                {
                    DateTime tmI = DateTime.MinValue;
                    DateTime tmF = DateTime.MinValue;
                    if (DateTime.TryParse(this.HourI, out tmI) && DateTime.TryParse(this.HourF, out tmF))
                    {
                        if (tmF > tmI) this.WorkingMinutes = (int)tmF.Subtract(tmI).TotalMinutes;
                    }

                }
            }
            catch
            {

            }

        }

    }

    [Serializable()]
    public class WorkingHourTimeRangeDaysOfWeek
    {

        public WorkingHourTimeRangeDaysOfWeek()
        {
            this.Clear();
        }

        public WorkingHourTimeRangeDaysOfWeek(DateTime datai, DateTime dataf)
        {
            this.Clear();
            this.DataI = datai;
            this.DataF = dataf;
        }

        private void Clear()
        {
            this.DataI = DateTime.Now.Date;
            this.DataI = DateTime.Now.Date;
            this.WorkingHourTimes = new List<WorkingHourTime>();
        }

        public DateTime DataI { get; set; }

        public DateTime DataF { get; set; }

        public List<WorkingHourTime> WorkingHourTimes { get; set; }

    }

    [Serializable()]
    public class MassimaliAgenda
    {

        private int[] _massimale = new int[7];

        public MassimaliAgenda()
        {
            _massimale[0] = 0;
            _massimale[1] = 0;
            _massimale[2] = 0;
            _massimale[3] = 0;
            _massimale[4] = 0;
            _massimale[5] = 0;
            _massimale[6] = 0;
        }

        public int[] Massimale
        {
            get
            {
                return _massimale;
            }
            set
            {
                _massimale = value;
            }
        }

    }

    [Serializable()]
    public class Calendario
    {

        public Calendario()
        {
            TipoCalendario = "Settimana";
            CodAgenda = "";
            ActiveDay = DateTime.Now.Date;
        }

        public string TipoCalendario { get; set; }

        public string CodAgenda { get; set; }

        public DateTime ActiveDay { get; set; }

    }

    [Serializable()]
    public class Calendari
    {

        public Calendari()
        {
            Tipo = 1;
            Calendario1 = new Calendario();
            Calendario2 = new Calendario();
            Calendario3 = new Calendario();
            Calendario4 = new Calendario();
            ComandiPin = true;
        }

        public int Tipo { get; set; }

        public Calendario Calendario1 { get; set; }

        public Calendario Calendario2 { get; set; }

        public Calendario Calendario3 { get; set; }

        public Calendario Calendario4 { get; set; }

        public bool ComandiPin { get; set; }

    }

    [Serializable()]
    public class FoglioUnico
    {

        public FoglioUnico()
        {
            Range = 1;
            Step = 0;
            AltezzaRighe = 0;
            Sezioni = new Dictionary<string, string>().Keys.ToArray();
            MostraNotte = false;
            SoloAttivi = false;
            TipoVisualizzazione = true;
        }

        public int Range { get; set; }

        public int Step { get; set; }

        public int AltezzaRighe { get; set; }

        public string[] Sezioni { get; set; }

        public bool MostraNotte { get; set; }

        public bool SoloAttivi { get; set; }

        public bool TipoVisualizzazione { get; set; }

    }

    [Serializable()]
    public class MovLink
    {

        public MovLink(string Url)
        {
            this.Uri = new Uri(Url);
        }

        public Uri Uri { get; set; }

        public string Url
        {
            get
            {
                if (this.Uri != null)
                    return this.Uri.AbsoluteUri.ToString();
                else
                    return string.Empty;
            }
        }

    }

    [Serializable()]
    public class MovDocumentiFirmati
    {

        public MovDocumentiFirmati()
        {

            this.IDDocumento = string.Empty;
            this.CodEntita = string.Empty;
            this.IDEntita = string.Empty;
            this.CodTipoDocumentoFirmato = string.Empty;
            this.CodStatoEntita = string.Empty;
            this.PDFFirmato = null;
            this.PDFNonFirmato = null;

            this.Azione = EnumAzioni.INS;

        }

        public MovDocumentiFirmati(string iddocumento)
        {
            this.Carica(iddocumento);
            this.Azione = EnumAzioni.VIS;
        }

        public string IDDocumento { get; set; }
        public string CodEntita { get; set; }
        public string IDEntita { get; set; }
        public string CodTipoDocumentoFirmato { get; set; }

        public string CodStatoEntita { get; set; }
        public byte[] PDFFirmato { get; set; }
        public byte[] PDFNonFirmato { get; set; }

        public EnumAzioni Azione { get; set; }

        private void Carica(string iddocumento)
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDDocumento", iddocumento);

                op.TimeStamp.CodEntita = EnumEntita.DCF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovDocumentiFirmati", spcoll);

                if (dt.Rows.Count > 0)
                {

                    this.IDDocumento = dt.Rows[0]["ID"].ToString();
                    this.CodEntita = dt.Rows[0]["CodEntita"].ToString();
                    this.IDEntita = dt.Rows[0]["IDEntita"].ToString();
                    this.CodTipoDocumentoFirmato = dt.Rows[0]["CodTipoDocumentoFirmato"].ToString();
                    this.CodStatoEntita = dt.Rows[0]["CodStatoEntita"].ToString();
                    if (!dt.Rows[0].IsNull("PDFFirmato")) this.PDFFirmato = (byte[])dt.Rows[0]["PDFFirmato"];
                    if (!dt.Rows[0].IsNull("PDFNonFirmato")) this.PDFNonFirmato = (byte[])dt.Rows[0]["PDFNonFirmato"];

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {

                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodEntita", this.CodEntita);
                op.Parametro.Add("IDEntita", this.IDEntita);
                op.Parametro.Add("CodTipoDocumentoFirmato", this.CodTipoDocumentoFirmato);
                op.Parametro.Add("CodStatoEntita", this.CodStatoEntita);
                if (this.PDFFirmato != null) { op.Parametro.Add("PDFFirmato", Convert.ToBase64String(this.PDFFirmato)); }
                if (this.PDFNonFirmato != null) { op.Parametro.Add("PDFNonFirmato", Convert.ToBase64String(this.PDFNonFirmato)); }

                op.TimeStamp.CodEntita = EnumEntita.DCF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.INS.ToString();

                spcoll = new SqlParameterExt[1];
                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovDocumentiFirmati", spcoll);

                if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                {
                    this.IDDocumento = dt.Rows[0][0].ToString();
                }
                else
                {
                    bReturn = false;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovDocumentiFirmati.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

        public string salvadoctmp(string fullfilepath)
        {
            string returnfilepath = "";
            try
            {
                if (this.PDFFirmato != null && this.PDFFirmato.Length > 0)
                {
                    if (fullfilepath == null || fullfilepath.Trim() == "")
                        returnfilepath = FileStatics.GetSCCITempPath() + @"TMP" + DateTime.Now.ToString(@"yyyyMMddHHmmssfff") + @".pdf.p7m";
                    else
                        returnfilepath = fullfilepath;

                    if (System.IO.File.Exists(returnfilepath)) System.IO.File.Delete(returnfilepath);

                    System.IO.File.WriteAllBytes(returnfilepath, this.PDFFirmato);

                }

                return returnfilepath;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

    [Serializable()]
    public class ProseguiTerapia
    {

        public ProseguiTerapia(DateTime dtI, DateTime dtF, DateTime dtIP, DateTime dtFP)
        {

            this.DataInizio = dtI;
            this.DataFine = dtF;
            this.DataInizioProsecuzione = dtIP;
            this.DataFineProsecuzione = dtFP;

        }

        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }

        public DateTime DataInizioProsecuzione { get; set; }
        public DateTime DataFineProsecuzione { get; set; }

    }

    [Serializable()]
    public class ImportaDWHFiltro
    {

        public ImportaDWHFiltro(DateTime dtDataDa, DateTime dtDataA, string sValoreRange)
        {

            this.DataDa = dtDataDa;
            this.DataA = dtDataA;
            this.ValoreRange = sValoreRange;
        }

        public DateTime DataDa { get; set; }
        public DateTime DataA { get; set; }

        public string ValoreRange { get; set; }

    }

    [Serializable()]
    public class SelezioniGrafici
    {

        public string RangeDate { get; set; }
        public DateTime DataDa { get; set; }
        public DateTime DataA { get; set; }

        public List<string> CodiciTipoPVT { get; set; }
        public List<string> CodiciLAB { get; set; }

        public List<string> CodiciTipiSomm { get; set; }

        public SelezioniGrafici()
        {
            this.DataDa = DateTime.MinValue;
            this.DataA = DateTime.MinValue;
            this.RangeDate = "";

            this.CodiciTipoPVT = new List<string>();
            this.CodiciLAB = new List<string>();
            this.CodiciTipiSomm = new List<string>();

        }
        public SelezioniGrafici(string rangeDate, DateTime dtDataDa, DateTime dtDataA, List<string> codiciTipoPVT, List<string> codiciLAB, List<string> codiciTipoSomm = null)
        {
            this.DataDa = dtDataDa;
            this.DataA = dtDataA;
            this.RangeDate = rangeDate;

            this.CodiciTipoPVT = codiciLAB;
            this.CodiciLAB = codiciTipoPVT;
            this.CodiciTipiSomm = codiciTipoSomm;
        }
        public SelezioniGrafici(string xmlSelezioni)
        {
            SelezioniGrafici tmp = XmlProcs.XmlDeserializeFromString<SelezioniGrafici>(xmlSelezioni);
            this.CodiciLAB = tmp.CodiciLAB;
            this.CodiciTipoPVT = tmp.CodiciTipoPVT;
            this.CodiciTipiSomm = tmp.CodiciTipiSomm;
            this.DataA = tmp.DataA;
            this.DataDa = tmp.DataDa;
            this.RangeDate = tmp.RangeDate;

        }

        public string ToXmlString()
        {
            return ToXmlString(false);
        }
        public string ToXmlString(bool cleanXML)
        {
            SelezioniGrafici sg = this;
            string sXML = XmlProcs.XmlSerializeToString(sg);
            if (cleanXML)
            {
                sXML = sXML.Replace(@"xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""", "");
                sXML = sXML.Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>" + Environment.NewLine, "");
            }
            return sXML;
        }

    }

    [Serializable()]
    public class Selezione
    {

        private bool _permessoModifica = false;

        private string _selezioniXML { get; set; }
        private object _objSelezioni { get; set; }

        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public string CodTipoSelezione { get; set; }
        public string CodUtenteInserimento { get; set; }
        public string CodRuoloInserimento { get; set; }
        public DateTime DataInserimento { get; set; }
        public bool FlagSistema { get; set; }

        public EnumAzioni Azione { get; set; }

        public string SelezioniXML
        {
            get
            {
                if (_selezioniXML == null || _selezioniXML.Trim() == "")
                {
                    if (_objSelezioni != null)
                    {
                        if (_objSelezioni is SelezioniGrafici)
                        {
                            _selezioniXML = ((SelezioniGrafici)_objSelezioni).ToXmlString(true);
                        }

                    }
                }
                return _selezioniXML;
            }
            set
            {
                _objSelezioni = null;
                _selezioniXML = value;
            }
        }

        public object Selezioni
        {
            get
            {
                if (_objSelezioni == null)
                {
                    if (CodTipoSelezione != null && CodTipoSelezione.Trim().ToUpper() == EnumTipoSelezione.GRAF.ToString())
                    {
                        if (_selezioniXML != null && _selezioniXML.Trim() != "")
                            _objSelezioni = new SelezioniGrafici(_selezioniXML);
                        else
                            _objSelezioni = new SelezioniGrafici();
                    }
                }
                return _objSelezioni;
            }
            set
            {
                _objSelezioni = value;
                _selezioniXML = "";
            }
        }

        public bool PERMESSOMODIFICA
        {
            get
            {
                return _permessoModifica;
            }
        }

        public Selezione(EnumTipoSelezione codTipoSelezione, string codRuolo)
        {
            this.Codice = DateTime.Now.ToString(@"yyyyMMddHHmmssfff") + codRuolo.Trim().ToUpper();
            this.Descrizione = "";
            this.CodTipoSelezione = codTipoSelezione.ToString();
            this.CodUtenteInserimento = CoreStatics.CoreApplication.Sessione.Utente.Codice;
            this.CodRuoloInserimento = codRuolo;
            this.DataInserimento = DateTime.MinValue;
            this.FlagSistema = false;
            _permessoModifica = true;
            _selezioniXML = string.Empty;

            this.Azione = EnumAzioni.INS;

        }

        public Selezione(string codSelezione)
        {
            this.Carica(codSelezione);
            this.Azione = EnumAzioni.VIS;
        }

        private void Carica(string codSelezione)
        {

            try
            {
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("Codice", codSelezione);
                op.Parametro.Add("DatiEstesi", "1");
                op.Parametro.Add("SoloUtente", "0");

                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelSelezioni", spcoll);

                if (dt.Rows.Count > 0)
                {

                    this.Codice = dt.Rows[0]["Codice"].ToString();
                    this.Descrizione = dt.Rows[0]["Descrizione"].ToString();
                    this.CodTipoSelezione = dt.Rows[0]["CodTipoSelezione"].ToString();

                    if (!dt.Rows[0].IsNull("CodUtenteInserimento")) this.CodUtenteInserimento = dt.Rows[0]["CodUtenteInserimento"].ToString();
                    if (!dt.Rows[0].IsNull("CodRuoloInserimento")) this.CodRuoloInserimento = dt.Rows[0]["CodRuoloInserimento"].ToString();
                    if (!dt.Rows[0].IsNull("DataInserimento")) this.DataInserimento = (DateTime)dt.Rows[0]["DataInserimento"];
                    if (!dt.Rows[0].IsNull("FlagSistema")) this.FlagSistema = (bool)dt.Rows[0]["FlagSistema"];

                    if (!dt.Rows[0].IsNull("Selezioni")) this.SelezioniXML = dt.Rows[0]["Selezioni"].ToString();
                    if (!dt.Rows[0].IsNull("PERMESSOMODIFICA")) _permessoModifica = (bool)dt.Rows[0]["PERMESSOMODIFICA"];


                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {

                string sSql = "";
                if (this.Azione == EnumAzioni.INS)
                {
                    Parametri op = null;
                    SqlParameterExt[] spcoll;
                    string xmlParam = "";

                    op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("Codice", this.Codice);
                    op.Parametro.Add("Descrizione", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.Descrizione));
                    op.Parametro.Add("CodTipoSelezione", this.CodTipoSelezione);
                    if (this.FlagSistema)
                        op.Parametro.Add("FlagSistema", "1");
                    else
                        op.Parametro.Add("FlagSistema", "0");

                    op.Parametro.Add("Selezioni", this.SelezioniXML);

                    op.TimeStamp.CodAzione = EnumAzioni.INS.ToString();

                    spcoll = new SqlParameterExt[1];
                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_InsSelezioni", spcoll);
                    this.Azione = EnumAzioni.MOD;
                    bReturn = true;

                }
                else if (this.Azione == EnumAzioni.CAN)
                {
                    if (this.PERMESSOMODIFICA)
                    {
                        Parametri op = null;
                        SqlParameterExt[] spcoll;
                        string xmlParam = "";

                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("Codice", this.Codice);

                        op.TimeStamp.CodAzione = EnumAzioni.CAN.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_DelSelezioni", spcoll);
                        bReturn = true;
                    }
                }
                else
                {
                    if (this.PERMESSOMODIFICA)
                    {

                        Parametri op = null;
                        SqlParameterExt[] spcoll;
                        string xmlParam = "";

                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("Codice", this.Codice);
                        op.Parametro.Add("Descrizione", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.Descrizione));
                        op.Parametro.Add("CodTipoSelezione", this.CodTipoSelezione);
                        op.Parametro.Add("Selezioni", this.SelezioniXML);

                        op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();

                        spcoll = new SqlParameterExt[1];
                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_AggSelezioni", spcoll);
                        bReturn = true;
                    }
                }
            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"Selezione.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

    }

    [Serializable()]
    public class MH_Login
    {

        private string _codice = string.Empty;
        private string _passwordaccesso = string.Empty;
        private byte[] _passwordaccessobin = null;
        private string _idpaziente = string.Empty;
        private string _codstatomhlogin = string.Empty;
        private DateTime _datascadenza = DateTime.MinValue;

        private string _statomhlogin = string.Empty;
        private string _coloremhlogin = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;

        public MH_Login(string idpaziente, EnumAzioni azione)
        {
            _idpaziente = idpaziente;
            _azione = azione;
            if (_azione == EnumAzioni.INS)
            {
                this.CaricaNuovo();
            }
            else
            {
                this.Carica(idpaziente);
            }
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string Codice
        {
            get { return _codice; }
            set { _codice = value; }
        }

        public string PasswordAccesso
        {
            get { return _passwordaccesso; }
            set { _passwordaccesso = value; }
        }

        public byte[] PasswordAccessoBin
        {
            get { return _passwordaccessobin; }
            set { _passwordaccessobin = value; }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
            set { _idpaziente = value; }
        }

        public string CodStatoMHLogin
        {
            get { return _codstatomhlogin; }
            set { _codstatomhlogin = value; }
        }

        public string StatoMHLogin
        {
            get { return _statomhlogin; }
            set { _statomhlogin = value; }

        }

        public string ColoreMHLogin
        {
            get { return _coloremhlogin; }
            set { _coloremhlogin = value; }

        }

        public DateTime DataScadenza
        {
            get { return _datascadenza; }
            set { _datascadenza = value; }
        }

        public bool Esiste { get; set; }

        private void CaricaNuovo()
        {

            try
            {

                this.Esiste = false;

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                TimeStamp ts = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                ts.CodAzione = EnumAzioni.INS.ToString();
                ts.CodEntita = EnumEntita.XXX.ToString();
                string xmlParam = XmlProcs.XmlSerializeToString(ts);
                spcoll[0] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_MH_SelNuovoLogin", spcoll);

                if (dt.Rows.Count > 0)
                {

                    if (!dt.Rows[0].IsNull("Codice")) _codice = dt.Rows[0]["Codice"].ToString();
                    if (!dt.Rows[0].IsNull("PasswordAccesso")) _passwordaccesso = dt.Rows[0]["PasswordAccesso"].ToString();
                    if (!dt.Rows[0].IsNull("DataScadenza")) _datascadenza = (DateTime)dt.Rows[0]["DataScadenza"];
                    _codstatomhlogin = "AT";

                    this.Esiste = true;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void Carica(string idpaziente)
        {

            try
            {

                this.Esiste = false;

                SqlParameterExt[] spcoll = new SqlParameterExt[2];
                spcoll[0] = new SqlParameterExt("uIDPaziente", new Guid(idpaziente), ParameterDirection.Input, SqlDbType.UniqueIdentifier);
                TimeStamp ts = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                ts.CodAzione = EnumAzioni.VIS.ToString();
                ts.CodEntita = EnumEntita.XXX.ToString();
                string xmlParam = XmlProcs.XmlSerializeToString(ts);
                spcoll[1] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_MH_SelLogin", spcoll);

                if (dt.Rows.Count > 0)
                {

                    if (!dt.Rows[0].IsNull("Codice")) _codice = dt.Rows[0]["Codice"].ToString();
                    if (!dt.Rows[0].IsNull("PasswordAccesso")) _passwordaccessobin = (byte[])dt.Rows[0]["PasswordAccesso"];
                    if (!dt.Rows[0].IsNull("IDPaziente")) _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    if (!dt.Rows[0].IsNull("CodStatoMHLogin")) _codstatomhlogin = dt.Rows[0]["CodStatoMHLogin"].ToString();
                    if (!dt.Rows[0].IsNull("DataScadenza")) _datascadenza = (DateTime)dt.Rows[0]["DataScadenza"];

                    if (!dt.Rows[0].IsNull("StatoMHLogin")) _statomhlogin = dt.Rows[0]["StatoMHLogin"].ToString();
                    if (!dt.Rows[0].IsNull("ColoreMHLogin")) _coloremhlogin = dt.Rows[0]["ColoreMHLogin"].ToString();

                    this.Esiste = true;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {

                SqlParameterExt[] spcoll;
                string xmlParam = "";

                switch (_azione)
                {

                    case EnumAzioni.INS:
                        spcoll = new SqlParameterExt[7];
                        spcoll[0] = new SqlParameterExt("sCodice", _codice, ParameterDirection.Input, SqlDbType.VarChar);
                        if (_passwordaccesso != string.Empty)
                        {
                            spcoll[1] = new SqlParameterExt("binPasswordAccesso", (byte[])Database.GetDatatable("Select dbo.MF_MH_CriptaPassword('" + _passwordaccesso + "')").Rows[0][0], ParameterDirection.Input, SqlDbType.VarBinary);
                        }
                        else
                        {
                            spcoll[1] = new SqlParameterExt("binPasswordAccesso", DBNull.Value, ParameterDirection.Input, SqlDbType.VarBinary);
                        }
                        spcoll[2] = new SqlParameterExt("sCodStatoMHLogin", _codstatomhlogin, ParameterDirection.Input, SqlDbType.VarChar);
                        spcoll[3] = new SqlParameterExt("dDataScadenza", _datascadenza, ParameterDirection.Input, SqlDbType.DateTime);
                        spcoll[4] = new SqlParameterExt("dDataScadenzaUTC", _datascadenza.ToUniversalTime(), ParameterDirection.Input, SqlDbType.DateTime);
                        spcoll[5] = new SqlParameterExt("uIDPaziente", _idpaziente, ParameterDirection.Input, SqlDbType.VarChar);
                        TimeStamp tsins = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                        tsins.CodAzione = _azione.ToString();
                        tsins.CodEntita = EnumEntita.XXX.ToString();
                        xmlParam = XmlProcs.XmlSerializeToString(tsins);
                        spcoll[6] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        DataTable dt = Database.GetDataTableStoredProc("MSP_MH_InsLogin", spcoll);

                        if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                        {
                            Carica(_idpaziente);
                        }
                        else
                        {
                            bReturn = false;
                        }
                        break;

                    case EnumAzioni.MOD:
                        spcoll = new SqlParameterExt[7];
                        spcoll[0] = new SqlParameterExt("sCodice", _codice, ParameterDirection.Input, SqlDbType.VarChar);
                        if (_passwordaccesso != string.Empty)
                        {
                            spcoll[1] = new SqlParameterExt("binPasswordAccesso", (byte[])Database.GetDatatable("Select dbo.MF_MH_CriptaPassword('" + _passwordaccesso + "')").Rows[0][0], ParameterDirection.Input, SqlDbType.VarBinary);
                        }
                        else
                        {
                            spcoll[1] = new SqlParameterExt("binPasswordAccesso", DBNull.Value, ParameterDirection.Input, SqlDbType.VarBinary);
                        }
                        spcoll[2] = new SqlParameterExt("sCodStatoMHLogin", _codstatomhlogin, ParameterDirection.Input, SqlDbType.VarChar);
                        spcoll[3] = new SqlParameterExt("dDataScadenza", _datascadenza, ParameterDirection.Input, SqlDbType.DateTime);
                        spcoll[4] = new SqlParameterExt("dDataScadenzaUTC", _datascadenza.ToUniversalTime(), ParameterDirection.Input, SqlDbType.DateTime);
                        spcoll[5] = new SqlParameterExt("uIDPaziente", _idpaziente, ParameterDirection.Input, SqlDbType.VarChar);
                        TimeStamp tsmod = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                        tsmod.CodAzione = _azione.ToString();
                        tsmod.CodEntita = EnumEntita.XXX.ToString();
                        xmlParam = XmlProcs.XmlSerializeToString(tsmod);
                        spcoll[6] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        Database.ExecStoredProc("MSP_MH_AggLogin", spcoll);

                        Carica(_idpaziente);

                        break;

                    case EnumAzioni.CAN:
                        spcoll = new SqlParameterExt[7];
                        spcoll[0] = new SqlParameterExt("sCodice", _codice, ParameterDirection.Input, SqlDbType.VarChar);
                        spcoll[1] = new SqlParameterExt("binPasswordAccesso", DBNull.Value, ParameterDirection.Input, SqlDbType.VarBinary);
                        spcoll[2] = new SqlParameterExt("sCodStatoMHLogin", _codstatomhlogin, ParameterDirection.Input, SqlDbType.VarChar);
                        spcoll[3] = new SqlParameterExt("dDataScadenza", null, ParameterDirection.Input, SqlDbType.DateTime);
                        spcoll[4] = new SqlParameterExt("dDataScadenzaUTC", null, ParameterDirection.Input, SqlDbType.DateTime);
                        spcoll[5] = new SqlParameterExt("uIDPaziente", null, ParameterDirection.Input, SqlDbType.VarChar);
                        TimeStamp tscan = new TimeStamp(CoreStatics.CoreApplication.Ambiente);
                        tscan.CodAzione = _azione.ToString();
                        tscan.CodEntita = EnumEntita.XXX.ToString();
                        xmlParam = XmlProcs.XmlSerializeToString(tscan);
                        spcoll[6] = new SqlParameterExt("xTimeStamp", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                        Database.ExecStoredProc("MSP_MH_AggLogin", spcoll);

                        Carica(_idpaziente);

                        break;

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MH_Login.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

        public void GeneraNuovaPassword()
        {

            try
            {

                FwDataParametersList fplist = new FwDataParametersList();
                fplist.Add("sPassword", null, ParameterDirection.Output, DbType.String, 50);
                List<FwDataParameter> oRet = Database.ExecStoredProcOutput("MSP_MH_GeneraPassword", fplist);

                if (oRet != null && oRet.Count == 1)
                {
                    _passwordaccesso = oRet[0].Value.ToString();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

    }

}
