using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciCore;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using UnicodeSrl.ScciEasy.Updater;
using UnicodeSrl.Framework.Extension;
using UnicodeSrl.Framework.CryptoAPI;
using UnicodeSrl.Framework.Diagnostics;
using System.Threading.Tasks;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.WebChrome;
using System.Reflection;
using System.IO;

namespace UnicodeSrl.ScciEasy
{
    static class Program
    {

        const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(
            IntPtr hInternet,
            int dwOption,
            IntPtr lpBuffer,
            int lpdwBufferLength);




        [STAThread]
        static void Main(string[] args)
        {

            using (Mutex mutex = new Mutex(false, AppDomain.CurrentDomain.FriendlyName))
            {
                if (mutex.WaitOne(0, true) == false)
                {
                    MessageBox.Show(string.Format("{0} già in esecuzione!!", AppDomain.CurrentDomain.FriendlyName), "Scci Easy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Task.Run(() => UnicodeSrl.Evaluator.Evaluator.InitMem());

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT");

                Thread.CurrentThread.CurrentUICulture = culture;

                Thread.CurrentThread.CurrentCulture = culture;

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                easyStatics.SetDpiAwareness();

                Database.ConnectionString = MyStatics.Configurazione.ConnectionString;
                setupDiagnostic();

                bool isConn = MyStatics.Configurazione.TestConnection();

                if (isConn == false)
                {
                    easyStatics.CloseEasySplash();
                    MessageBox.Show("Database NON raggiungibile!", "Scci Easy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    System.Environment.Exit(0);
                }

                string fontName = "Calibri";
                System.Drawing.Font myFont = new System.Drawing.Font(fontName, 16.0f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                if (myFont.Name != fontName)
                {
                    easyStatics.CloseEasySplash();
                    MessageBox.Show(string.Format("Font {0} NON installato sul sistema!" + Environment.NewLine + "Contattare l'assistenza!!!", fontName), "Scci Easy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    System.Environment.Exit(0);
                }

#if DEBUG
                Console.WriteLine("Sono in debug, non faccio il CheckAndUpdate");
#else
                // Verifichiamo se l'utente che sta impegnando SCCI è quello di configurazione del Updater.
                // In caso avvisiamo di chiudere e riavviare
                
                bool isUpdaterUser = ScciUpdater.IsUpdaterUser();

                if (isUpdaterUser)
                {
                    easyStatics.CloseEasySplash();
                    MessageBox.Show("Matilde è stato aggiornato, scegliere OK e riavviare l'applicativo", "Scci Easy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    System.Environment.Exit(0);
                }

                // NOTA: ricordarsi che l'utente del updater deve essere validabile sul dominio corrente
                bool updateOk = ScciUpdater.CheckAndUpdate();

                if (updateOk == false )
                {
                    easyStatics.CloseEasySplash();
                    MessageBox.Show("Si è verificato un errore grave in aggiornamento. Contattare l'assistenza tecnica.", "Scci Easy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(0);
                }

                // Controllo Versione [2016-10-26]
                // Obsoleto
                if (!CoreStatics.ControlloVersioneSCCI(false, true, true))
                {
                    //easyStatics.CloseEasySplash();
                    System.Environment.Exit(0);
                }

#endif

                easyStatics.StartEasySplash();

                try
                {
                    CoreStatics.CoreApplication.Sessione.Utente = new Utente(UnicodeSrl.Framework.Utility.Windows.CurrentUser());

                    if (CoreStatics.CoreApplication.Sessione.Utente.Abilitato == false)
                    {
                        easyStatics.CloseEasySplash();
                        MessageBox.Show("Utente NON abilitato!", "Scci Easy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        System.Environment.Exit(0);
                    }

                    if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null)
                    {
                        CoreStatics.CoreApplication.Ambiente.Codruolo = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice;
                        PluginClientStatics.Pcm = PluginClientStatics.SetPluginClientManager(CoreStatics.CoreApplication.Ambiente,
                                                                                                CoreStatics.CoreApplication.Ambiente.Codruolo,
                                                                                                CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota,
                                                                                                CoreStatics.CoreApplication.Sessione.Computer.IsOSServer);
                    }
                    CoreStatics.CoreApplication.Sessione.Utente.SbloccaTutto();

                    CoreStatics.CoreApplication.Listener = new Listener(CoreStatics.CoreApplication.Sessione.Utente.Codice, Database.ConnectionString, true);
                    CoreStatics.CoreApplication.Listener.InitializeProcess();


                    foreach (string arg in args)
                    {
                        if (arg.ToUpper().StartsWith("/N="))
                        {
                            CoreStatics.CoreApplication.Sessione.Nosologico = arg.Substring(3, arg.Length - 3);
                        }
                        else if (arg.ToUpper().StartsWith("/SAC="))
                        {
                            CoreStatics.CoreApplication.Sessione.CodiceSACApertura = arg.Substring(5);
                        }
                        else if (arg.ToUpper().StartsWith("/MOD="))
                        {
                            EnumCommandLineModules tmpmodule = EnumCommandLineModules.Cartella;
                            if (!Enum.TryParse(arg.Substring(5), true, out tmpmodule))
                                tmpmodule = EnumCommandLineModules.Cartella;
                            CoreStatics.CoreApplication.Sessione.ModuloApertura = tmpmodule;
                        }
                        else if (arg.ToUpper().StartsWith("/SALA"))
                        {
                            CoreStatics.CoreApplication.Sessione.Sala = true;
                        }
                        else if (arg.ToUpper().StartsWith("/LA="))
                        {
                            CoreStatics.CoreApplication.Sessione.ListaAttesa = arg.Substring(4, arg.Length - 4);
                        }
                        else if (arg.ToUpper().StartsWith("/USCITADIRETTA"))
                        {
                            CoreStatics.CoreApplication.Sessione.UscitaDitetta = true;
                        }
                        else if (arg.ToUpper().StartsWith("/NOMSG"))
                        {
                            CoreStatics.CoreApplication.Sessione.NoMsg = true;
                        }

                    }



                    double dblTimerInattivita = Convert.ToDouble(Database.GetConfigTable(EnumConfigTable.TimerInattivita));
                    if (dblTimerInattivita > 0)
                    {
                        CoreStatics.CoreApplication.Inattivita = new FilterMess(dblTimerInattivita);
                        CoreStatics.CoreApplication.Inattivita.InattivitaEvent += Inattivita_InattivitaEvent;
                        Application.AddMessageFilter(CoreStatics.CoreApplication.Inattivita);
                    }

                    DBUtils.nTotCicliNewsHard = Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.MoltiplicatoreNewsHard));
                    DBUtils.nCicliNewsHard = DBUtils.nTotCicliNewsHard;

                    easyStatics.CoefficienteFontDaConfig = CoreStatics.CoreApplication.Sessione.Computer.FontCoefficienteConfigPC;

                    easyStatics.setTreeViewCheckBoxesStyle();

                    CoreStatics.CoreApplicationContext.MainForm = new frmMain();

                    Interlocked.Increment(ref frmMain.CounterNav);

                    if (CoreStatics.CoreApplication.Sessione.Nosologico != string.Empty
                        || CoreStatics.CoreApplication.Sessione.CodiceSACApertura != string.Empty
                        || CoreStatics.CoreApplication.Sessione.ListaAttesa != string.Empty)
                    {

                        string titolo = "ACCESSO DALLA SALA";
                        if (CoreStatics.CoreApplication.Sessione.CodiceSACApertura != string.Empty) titolo = "ACCESSO DA APPLICATIVO ESTERNO";

                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.MenuPrincipale);
                        easyStatics.CloseEasySplash();
                        DialogResult oDialogResult = DialogResult.OK;

                        if (!CoreStatics.CoreApplication.Sessione.NoMsg)
                        {
                            if (easyStatics.EasyMessageBox(string.Format("Stai aprendo Matilde con l’utente seguente:\n\n{0}\n{1}\n\nConfermi l’apertura con questo utente?\n\nRispondere SI per confermare.\nRispondere NO per collegarsi con un altro utente.",
                                                            CoreStatics.CoreApplication.Sessione.Utente.Codice,
                                                            CoreStatics.CoreApplication.Sessione.Utente.Descrizione),
                                                            titolo, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
                                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CambiaUtente);
                            }
                        }
                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.Elementi.Count > 1)
                        {
                            oDialogResult = CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezionaRuolo);
                        }

                        if (oDialogResult == DialogResult.OK && CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null)
                        {

                            bool bAvanti = true;
                            if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.RichiediPassword == true)
                            {
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.RichiediPassword) != DialogResult.OK)
                                {
                                    bAvanti = false;
                                }
                            }

                            if (bAvanti == true)
                            {
                                CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.CaricaReports(CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice, "");
                                if (CoreStatics.CoreApplication.Sessione.Nosologico != string.Empty)
                                {

                                    if (CoreStatics.RicercaCartella())
                                    {
                                        Application.Run(CoreStatics.CoreApplicationContext);
                                    }
                                    else
                                    {
                                        CoreStatics.CoreApplicationContext.Exit();
                                        System.Environment.Exit(0);
                                    }

                                }
                                else if (CoreStatics.CoreApplication.Sessione.ListaAttesa != string.Empty)
                                {

                                    if (CoreStatics.RicercaCartella(CoreStatics.CoreApplication.Sessione.ListaAttesa))
                                    {
                                        Application.Run(CoreStatics.CoreApplicationContext);
                                    }
                                    else
                                    {
                                        CoreStatics.CoreApplicationContext.Exit();
                                        System.Environment.Exit(0);
                                    }

                                }
                                else if (CoreStatics.CoreApplication.Sessione.CodiceSACApertura != string.Empty)
                                {
                                    bool bContinua = true;

                                    if (bContinua && CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_SelezioneUA) != DialogResult.OK)
                                        bContinua = false;

                                    if (bContinua)
                                    {
                                        Scci.DataContracts.PazienteSac oPazienteSac = DBUtils.get_RicercaPazientiSACByID(CoreStatics.CoreApplication.Sessione.CodiceSACApertura);
                                        if (oPazienteSac != null)
                                        {
                                            CoreStatics.CoreApplication.Paziente = new Paziente(oPazienteSac, CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata, CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata);

                                            bool bSACConsensiAbilita = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.SACConsensiAbilita)));
                                            if (bSACConsensiAbilita)
                                            {
                                                if (CoreStatics.CoreApplication.Paziente.PazientiConsensiGenerico.CodStatoConsenso == EnumStatoConsenso.SI.ToString())
                                                {
                                                    bContinua = true;
                                                }
                                                else if (CoreStatics.CoreApplication.Paziente.PazientiConsensiGenerico.CodStatoConsenso == EnumStatoConsenso.NO.ToString())
                                                {
                                                    bContinua = false;
                                                }
                                                else
                                                {
                                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.RichiestaConsenso) == DialogResult.OK)
                                                    {

                                                        CoreStatics.CoreApplication.Paziente = new Paziente(oPazienteSac, CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata, CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata);
                                                        if (CoreStatics.CoreApplication.Paziente.PazientiConsensiGenerico.CodStatoConsenso == EnumStatoConsenso.SI.ToString())
                                                        {
                                                            bContinua = true;
                                                        }
                                                        else
                                                            bContinua = false;

                                                    }
                                                    else
                                                        bContinua = false;
                                                }
                                            }
                                            else
                                            {
                                                bContinua = true;
                                            }
                                        }
                                        else
                                            bContinua = false;
                                    }


                                    if (bContinua)
                                    {
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;

                                        CoreStatics.CoreApplication.Paziente.AggiungiRecenti();

                                        CoreStatics.CoreApplication.Navigazione.Maschere.TornaARicercaAbilitato = false;

                                        EnumMaschere mascheraapertura = EnumMaschere.Ambulatoriale_Cartella;
                                        if (CoreStatics.CoreApplication.Sessione.ModuloApertura != null && CoreStatics.CoreApplication.Sessione.ModuloApertura.HasValue)
                                            mascheraapertura = CommonStatics.RecuperaMascheraApertura(CoreStatics.CoreApplication.Sessione.ModuloApertura.Value, true);

                                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(mascheraapertura);

                                        CoreStatics.CoreApplication.Navigazione.Maschere.RimuoviMaschera(EnumMaschere.Ambulatoriale_SelezioneUA);

                                        Application.Run(CoreStatics.CoreApplicationContext);
                                    }
                                    else
                                    {
                                        CoreStatics.CoreApplicationContext.Exit();
                                        System.Environment.Exit(0);
                                    }

                                }

                            }
                        }
                        else
                        {
                            CoreStatics.CoreApplicationContext.Exit();
                            System.Environment.Exit(0);
                        }

                    }
                    else
                    {

                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato == null)
                        {
                            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezionaRuolo);
                        }

                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null)
                        {
                            if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.RichiediPassword == true)
                            {
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.RichiediPassword) != DialogResult.OK)
                                {
                                    CoreStatics.CoreApplicationContext.Exit();
                                    System.Environment.Exit(0);
                                }
                            }
                            if (CoreStatics.CoreApplication.Sessione.Computer.SessioneIpovedente)
                            {
                                Risorse.GetPlaySoundFromResource(Risorse.GC_WAV_APERTURAMATILDE);
                            }

                            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.MenuPrincipale);
                            easyStatics.CloseEasySplash();
                            Application.Run(CoreStatics.CoreApplicationContext);
                            if (CoreStatics.CoreApplication.Sessione.Computer.SessioneIpovedente)
                            {
                                Risorse.GetPlaySoundFromResource(Risorse.GC_WAV_CHIUSURAMATILDE);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ruolo NON selezionato!", "Scci Easy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            CoreStatics.CoreApplicationContext.Exit();
                            System.Environment.Exit(0);
                        }

                    }

                }
                catch (Exception ex)
                {
                    DiagnosticStatics.AddDebugInfo(ex);
                }




            }

        }

        private static void setupDiagnostic()
        {
            DiagnosticStatics.SetDiagnosticsDefaults();

            string dbConnString = Database.GetConfigTable(EnumConfigTable.DiagnosticsDbConn);
            string sWebServiceUrl = Database.GetConfigTable(EnumConfigTable.Diagnostics);
            string sLogPath = Database.GetConfigTable(EnumConfigTable.DiagnosticsClientPercorso);

            string ddvalue = Database.GetConfigTable(EnumConfigTable.DiagnosticsClientGiorni);
            if (Int32.TryParse(ddvalue, out int daysToLog))
            {
                DiagnosticStatics.DefDbgSettings.CustomLogMaxItems = daysToLog;
            }

            bool bDiagEvtLog = (Database.GetConfigTable(EnumConfigTable.DiagnosticsClientEventiLog) == "1" ? true : false);

            if (dbConnString.IsNotNullOrEmpty())
            {
                if (dbConnString.IsBase64String())
                {
                    Encryption c = new UnicodeSrl.Scci.Encryption(Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, Scci.Statics.EncryptionStatics.GC_INITVECTOR);
                    dbConnString = c.DecryptString(dbConnString);
                }

                DiagnosticStatics.DefDbgSettings.DbConnNameOrString = dbConnString;
                DiagnosticStatics.DefDbgSettings.SaveToDatabase = true;
                DiagnosticStatics.DefDbgSettings.WebServiceUrl = "";
                DiagnosticStatics.DefDbgSettings.SendToWebService = false;
            }
            else if (sWebServiceUrl != "")
            {
                DiagnosticStatics.DefDbgSettings.DbConnNameOrString = "";
                DiagnosticStatics.DefDbgSettings.SaveToDatabase = false;
                DiagnosticStatics.DefDbgSettings.WebServiceUrl = sWebServiceUrl;
                DiagnosticStatics.DefDbgSettings.SendToWebService = true;
            }


            if (sLogPath != "")
                DiagnosticStatics.DefDbgSettings.CustomLogPath = sLogPath;

            DiagnosticStatics.DefDbgSettings.SaveToEventLog = bDiagEvtLog;

        }

        static void Inattivita_InattivitaEvent(object sender, EventArgs e)
        {

            try
            {

                System.Delegate d = new MethodInvoker(showFormInattivita);
                CoreStatics.CoreApplicationContext.MainForm.Invoke(d);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        static void showFormInattivita()
        {

            try
            {

                frmInattivita _frmInattivita = new frmInattivita();
                _frmInattivita.WindowState = FormWindowState.Maximized;
                _frmInattivita.Carica();
                if (_frmInattivita.DialogResult == DialogResult.OK)
                {
                    CoreStatics.CoreApplication.Inattivita.Attivato = false;
                }
                else
                {
                    CoreStatics.CoreApplicationContext.Exit();
                    System.Environment.Exit(0);
                }

            }
            catch (Exception ex)
            {
                Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }


    }
}
