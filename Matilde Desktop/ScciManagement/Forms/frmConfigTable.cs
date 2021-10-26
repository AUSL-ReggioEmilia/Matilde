using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UDL;

using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciResource;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmConfigTable : Form, Interfacce.IViewFormBase
    {
        public frmConfigTable()
        {
            InitializeComponent();
        }

        #region Interface

        public Icon ViewIcon
        {
            get
            {
                return this.Icon;
            }
            set
            {
                this.Icon = value;
            }
        }

        public string ViewText
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
                this.UltraTabControl.Tabs["tab1"].Text = value;
            }
        }

        public void ViewInit()
        {

            this.SuspendLayout();

            MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);
            this.PicView.Image = Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_CONFIGURAZIONE_AMBIENTE, Enums.EnumImageSize.isz256));

            this.UltraTabControl.Tabs["tab2"].Text = "Web Service";
            this.UltraTabControl.Tabs["tab8"].Text = "Web Service 2";
            this.UltraTabControl.Tabs["tab9"].Text = "Web Service 3";
            this.UltraTabControl.Tabs["tab3"].Text = "Preferenze";
            this.UltraTabControl.Tabs["tab4"].Text = "Appuntamenti";
            this.UltraTabControl.Tabs["tab5"].Text = "Loghi";
            this.UltraTabControl.Tabs["tab6"].Text = "Stampe";
            this.UltraTabControl.Tabs["tab7"].Text = "Altro";
            this.UltraTabControl.Tabs["tab10"].Text = "Splash";
            this.UltraTabControl.Tabs["tab11"].Text = "App Web";

            this.ucPictureSelectLogoEasy.ViewInit();
            this.ucPictureSelectLogoFornitori.ViewInit();

            this.ucPictureSelectSplashEasy.ViewInit();
            this.ucPictureSelectSplashManagement.ViewInit();

            this.ucrtfIntestazioneStampe.ViewInit();
            this.ucrtfIntestazioneStampeSint.ViewInit();

            this.LoadConfig();

            this.ResumeLayout();

        }

        #endregion

        #region Subroutine

        private void LoadConfig()
        {

            this.uceBloccoUscita.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.BloccoUscita);
            this.uteDominio.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.DominioPredefinito);

            this.uteNNews.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordNews);
            this.uteNPazAmb.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroUltimeRicercheAmbulatoriali);
            this.uteNParametri.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordParametriVitali);
            this.uteNAllergie.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordAllergie);
            this.uteNWarning.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordWarning);
            this.uteNTaskInfermieristici.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordTaskInfermieristici);
            this.uteNOrdini.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordOrdini);
            this.uteNAppuntamenti.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordAppuntamenti);
            this.uteNEvidenzaClinica.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordEvidenzaClinica);
            this.uteNumRecordDWH.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHNumRecords);
            this.uteNumGiorniDWH.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroGiorniEvidenzaClinica);
            this.uteNumRecordOE.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOENumRecords);
            this.uteNumRecordSAC.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACNumRecords);

            this.uteDiagnosticsClientPercorso.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.DiagnosticsClientPercorso);
            this.uteDiagnosticsClientGiorni.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.DiagnosticsClientGiorni);
            string stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.DiagnosticsClientEventiLog);
            this.uchkDiagnosticsClientEventiLog.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);

            this.uteAnalisiDB.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AnalisiConnectionString);

            this.uosTipoSalvataggioScheda.Value = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoSalvataggioScheda);
            this.utePathSalvataggioScheda.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PathSalvataggioScheda);

            this.uteVersioneSCCI.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.VersioneSCCI);
            stemp = "";
            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AbilitaControlloVersioneSCCI);
            this.uchkAbilitaControlloVersioneSCCI.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);

            this.uteVersioneSCCIManagement.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.VersioneSCCIManagement);
            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AbilitaControlloVersioneSCCIManagement);
            this.uchkAbilitaControlloVersioneSCCIManagement.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);

            this.uteFontPredefinitoForm.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoForm);
            this.uteFontPredefinitoRTF.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF);

            Scci.Encryption ocrypt = new Encryption(Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, Scci.Statics.EncryptionStatics.GC_INITVECTOR);

            this.uteDiagnostics.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.Diagnostics);

            this.uteWSScci.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCI);
            this.uteWSScciUserName.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCIUserName);
            this.uteWSScciPassword.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCIPassword));
            stemp = "";
            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCIHTTPS);
            this.uchkWsSCCIHttps.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);

            this.uteWSSAC.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSAC);
            this.uteWSSACConsensi.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACConsensi);

            this.uteUserNameWSSAC.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACUserName);
            this.utePasswordWSSAC.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACPassword));
            this.uteWSDWH.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWH);
            this.uteWSDWHLAB.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHLAB);
            this.uteUserNameWSDWH.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHUserName);
            this.utePasswordWSDWH.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHPassword));
            this.uteWSOE.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOE);
            this.uteUserNameWSOE.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOEUserName);
            this.utePasswordWSOE.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOEPassword));
            this.uteWSOERRErogante.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOERR);
            this.uteWebServiceOERRUserName.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOERRUserName);
            this.uteWebServiceOERRPassword.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOERRPassword));
            this.uteWSOEOSUErogante.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOEOSU);
            this.uteWSAB.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceAB);
            this.uteWSABUserName.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceABUserName);
            this.uteWSABPassword.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceABPassword));
            this.uteWSVNA.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceVNA);
            this.uteWSVNAUserName.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceVNAUserName);
            this.uteWSVNAPassword.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceVNAPassword));
            this.uteWebRefertiDWH.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.URLAccessoRefertiDWH);
            this.uteWSUFA.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceUFA);
            this.uteWSUFAUserName.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceUFAUserName);
            this.uteWSUFAPassword.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceUFAPassword));

            this.uteUpdaterAddress.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterAddress);
            this.uteUpdaterDomain.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterDomain);
            this.uteUpdaterUserName.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterUserName);
            this.uteUpdaterPassword.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterPassword));
            this.uteUpdaterSvcSecDomain.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterSvcSecDomain);
            this.uteUpdaterSvcSecUsername.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterSvcSecUserName);
            this.uteUpdaterSvcSecPassword.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterSvcSecPassword));
            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterShowUI);
            this.uchkUpdaterShowUI.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);
            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterUseUserFolder);
            this.uchkUpdaterUserFolder.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);
            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterAlwaysCheckFiles);
            this.uchkUpdaterAlwaysCheckFiles.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);

            this.uteWSPSC.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSC);
            this.uteWSPSCUserName.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSCUserName);
            this.uteWSPSCPassword.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSCPassword));
            this.uteWSPSCAUSL.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSCAUSL);
            this.uteWSPSCUserNameAUSL.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSCUserNameAUSL);
            this.uteWSPSCPasswordAUSL.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSCPasswordAUSL));
            this.uteWSMow.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceMOW);
            this.uteWSMowAUSL.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceMOWAUSL);
            this.utxtLetteraDimissioneURL.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.LetteraDimissioneURL);
            this.utxtLetteraDimissioneURLAUSL.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.LetteraDimissioneURLAUSL);

            this.utxtPscURL.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PscURL);
            this.utxtPscURLAUSL.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PscURLAUSL);

            this.utePACSRDPServer.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PACSRDPServer);
            this.utePACSRDPUserName.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PACSRDPUserName);
            this.utePACSRDPPassword.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PACSRDPPassword));

            this.uteReMViewer.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ReMViewer);
            this.uteReMConnString.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ReMConnectionString);

            this.uteTimerAggiornamenti.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TimerApplicazione);
            this.uteMoltiplicatoreNewsHard.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.MoltiplicatoreNewsHard);
            this.uteTimerAggiornamentiAmb.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TimerAmbiente);
            this.uteTimerFUT.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TimerFUT);
            this.uteTimerInattivita.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TimerInattivita);

            this.uteTempoParametriVitaliTrasversali.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.MinutiRicercaParametriVitaliTrasversali);
            this.uteTempoDimessiTrasferiti.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TempoDimessiTrasferiti);

            this.uteAppEpisodio.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ElencoCampiAppuntamentoEpisodio);
            this.uteAppPaziente.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ElencoCampiAppuntamentoPaziente);
            this.uteAppMinOccorrenze.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AppuntamentiMinOccorrenze);
            this.uteAppMAXOccorrenze.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AppuntamentiMAXOccorrenze);

            this.ucPictureSelectLogoEasy.ViewImage = Scci.Statics.Database.GetConfigTableImage(UnicodeSrl.Scci.Enums.EnumConfigTable.LogoEasy);
            this.ucPictureSelectLogoFornitori.ViewImage = Scci.Statics.Database.GetConfigTableImage(UnicodeSrl.Scci.Enums.EnumConfigTable.LogoFornitore);

            this.ucPictureSelectSplashEasy.ViewImage = Scci.Statics.Database.GetConfigTableImage(UnicodeSrl.Scci.Enums.EnumConfigTable.SplashEasy);
            this.ucPictureSelectSplashManagement.ViewImage = Scci.Statics.Database.GetConfigTableImage(UnicodeSrl.Scci.Enums.EnumConfigTable.SplashManagement);

            if (Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != null
&& Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != ""
&& Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.RTFIntestazioneStampe) == "")
                this.ucrtfIntestazioneStampe.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));
            this.ucrtfIntestazioneStampe.ViewRtf = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.RTFIntestazioneStampe);

            if (Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != null
            && Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF) != ""
            && Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.RTFIntestazioneStampeSintetica) == "")
                this.ucrtfIntestazioneStampeSint.ViewFont = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF));
            this.ucrtfIntestazioneStampeSint.ViewRtf = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.RTFIntestazioneStampeSintetica);

            this.utxtSchedaTestataEpisodio.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoSchedaTestataEpisodio);
            this.utxtSchedaTestataPaziente.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoSchedaTestataPaziente);

            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ApriDOCXtramiteshell);
            this.uchkApriDOCXShell.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);
            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ApriPDFtramiteshell);
            this.uchkApriPDFShell.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);

            this.utxtVoceDiarioinfermieristico.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TIpoVoceDiarioInfermieristico);
            this.utxtVoceDiarioMedico.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoVoceDiarioMedico);
            this.utxtTipoConsegnaPaziente.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoConsegnaDaDCL);

            this.utxtTipoTaskDaPrescrizione.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoSchedaTaskDaPrescrizione);
            this.utxtTipoPrescrizioneSemplice.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoPrescrizioneSemplice);
            this.utxtRefertiDWHdaEscludere.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.RefertiDWHdaEscludere);
            this.utxtCampoSchedaPrescrizioneSemplice.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.CampoSchedaDaPrescrizioneSemplice);
            this.uteAltezzaFotoPaziente.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FotoPazienteMaxHeight);
            this.uteLarghezzaFotoPaziente.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FotoPazienteMaxWidth);
            this.uteNMaxOwnerFUT.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroMassimoOwnerFUT);
            this.uteNMaxCaratteriOwnerFUT.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroMassimoCaratteriOwnerFUT);
            this.utePercentualeAltezzaRighaNormaleFUT.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PercentualeAltezzaRighaNormaleFUT);
            this.utePercentualeAltezzaRighaCompattaFUT.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PercentualeAltezzaRighaCompattaFUT);
            this.uteMaxMbAllegati.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiDimensioneMassimaMb);
            this.uteAllegatiProssimoID.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiProssimoID);
            this.uteSplineTension.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FattoreCurvaturaGrafici);
            this.uteMaxGiorniTickmark.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.GraficiMaxGiorniTickmark);
            this.utxtCodRuoloConsulenze.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.RuoloConsulente);
            this.uteMBMin.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.MbMINNetworkAvailable);

            this.ucpColoreFestivitaCalendari.Value = MyStatics.GetColorFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ColoreFestivitaCalendari));

            this.ucpFUTColoreCambioGiorno.Value = MyStatics.GetColorFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTColoreCambioGiorno));

            this.ucpFUTColoreUltimaSommProgrammata.Value = MyStatics.GetColorFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTColoreUltimaSommProgrammata));
            this.uteFUTCarattereUltimaSommProgrammata.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTCarattereUltimaSommProgrammata);
            this.ucpFUTColoreUltimaSommErogata.Value = MyStatics.GetColorFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTColoreUltimaSommErogata));
            this.uteFUTCarattereUltimaSommErogata.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTCarattereUltimaSommErogata);

            this.ucpColoreChiusuraCartelle.Value = MyStatics.GetColorFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ColoreChiusuraCartelle));

            this.uteLarghezzaSchedeAnteprima.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiSchedeAntemprimaWidth);
            this.uteAltezzaSchedeAnteprima.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiSchedeAntemprimaHeight);

            this.uteLarghezzaSchedeStampa.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiSchedeStampaWidth);
            this.uteAltezzaSchedeStampa.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiSchedeStampaHeight);

            this.ucpPazientiSeguitiColoreSfondo.Value = MyStatics.GetColorFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ColorePazienteSeguito));

            this.ucpFUTColoreUltimaSommProgrChiusa.Value = MyStatics.GetColorFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTColoreUltimaSommProgrChiusa));

            this.uteDeltaOreErogazioneTask.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.DeltaOreErogazioneTask);

            this.utxtAzienda.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.Azienda);

            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.GestAutoNumCartella);
            this.uchkGestAutoNumCartella.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);
            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AssStessoNumCartella);
            this.uchkAssStessoNumCartella.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);
            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AggiornaSACAperturaCartella);
            this.uchkAggiornaSACAperturaCartella.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);

            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.SACConsensiAbilita);
            this.uchkConsenso.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);

            this.uteGGPregressiEVCConsultazione.Text = Scci.Statics.Database.GetConfigTable(EnumConfigTable.GGPregressiEVCConsultazione);

            stemp = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.Debug);
            this.uchkDebug.Checked = (stemp != "" ? (stemp == "0" ? false : true) : false);

            this.utxtUtenteRifElab.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ElabSistemiUserName);
            this.utxtRuoloUtenteRifElab.Text = Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ElabSistemiCodRuolo);

            uteURLFastDRG2.Text = Scci.Statics.Database.GetConfigTable(EnumConfigTable.WebSiteFastDRG2);

            this.uteScciWebPathIcone1.Text = Scci.Statics.Database.GetConfigTable(EnumConfigTable.ScciWebPathIcone1);
            this.uteScciWebPathIcone2.Text = Scci.Statics.Database.GetConfigTable(EnumConfigTable.ScciWebPathIcone2);
            this.uteScciWebPathIconeUtente.Text = Scci.Statics.Database.GetConfigTable(EnumConfigTable.ScciWebPathIconeUtente);
            this.uteScciWebPathIconePassword.Text = ocrypt.DecryptString(Scci.Statics.Database.GetConfigTable(EnumConfigTable.ScciWebPathIconePassword));

            ocrypt = null;

        }

        private bool CheckInput()
        {
            bool bRet = true;

            if (bRet)
            {
                if (this.utxtSchedaTestataPaziente.Text != "" && this.lblSchedaTestataPazienteDes.Text == "")
                {
                    bRet = false;
                    MessageBox.Show(@"Inserire un Codice Tipo Scheda Testata Paziente valido!", this.lblSchedaTestataPaziente.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.utxtSchedaTestataPaziente.Focus();
                }
            }

            if (bRet)
            {
                if (this.utxtSchedaTestataEpisodio.Text != "" && this.lblSchedaTestataEpisodioDes.Text == "")
                {
                    bRet = false;
                    MessageBox.Show(@"Inserire un Codice Tipo Scheda Testata Episodio valido!", this.lblSchedaTestataEpisodio.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.utxtSchedaTestataEpisodio.Focus();
                }
            }

            if (bRet)
            {
                if (this.utxtTipoTaskDaPrescrizione.Text != "" && this.lblTipoTaskDaPrescrizioneDes.Text == "")
                {
                    bRet = false;
                    MessageBox.Show(@"Inserire un Codice Tipo Task da Prescrizione valido!", this.lblTipoTaskDaPrescrizione.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.utxtTipoTaskDaPrescrizione.Focus();
                }
            }

            if (bRet)
            {
                if (this.utxtTipoPrescrizioneSemplice.Text != "" && this.lblTipoPrescrizioneSempliceDes.Text == "")
                {
                    bRet = false;
                    MessageBox.Show(@"Inserire un Codice Tipo Prescrizione Semplice valido!", this.lblTipoTaskDaPrescrizione.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.utxtTipoPrescrizioneSemplice.Focus();
                }
            }


            if (bRet)
            {
                if (this.utxtCodRuoloConsulenze.Text != "" && this.lblCodRuoloConsulenzeDes.Text == "")
                {
                    bRet = false;
                    MessageBox.Show(@"Inserire un Ruolo per Consulenze valido!", this.lblCodRuoloConsulenze.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.utxtCodRuoloConsulenze.Focus();
                }
            }


            if (bRet)
            {
                if (this.uteAppMinOccorrenze.Text.Trim() == "" || this.uteAppMinOccorrenze.Text == "0")
                {
                    bRet = false;
                    MessageBox.Show(@"Inserire un valore numerico intero maggiore di 0 (zero)!", "Min. Occorrenze", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteAppMinOccorrenze.Focus();
                }
            }
            if (bRet)
            {
                if (this.uteAppMinOccorrenze.Text.Trim() != "")
                {
                    int itmp = 0;
                    if (!int.TryParse(this.uteAppMinOccorrenze.Text, out itmp)) itmp = 0;

                    if (itmp <= 0)
                    {
                        bRet = false;
                        MessageBox.Show(@"Inserire un valore numerico intero maggiore di 0 (zero)!", "Min. Occorrenze", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteAppMinOccorrenze.Focus();
                    }
                }
            }
            if (bRet)
            {
                if (this.uteAppMAXOccorrenze.Text.Trim() == "" || this.uteAppMAXOccorrenze.Text == "0")
                {
                    bRet = false;
                    MessageBox.Show(@"Inserire un valore numerico intero maggiore di 0 (zero)!", "MAX. Occorrenze", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteAppMAXOccorrenze.Focus();
                }
            }
            if (bRet)
            {
                if (this.uteAppMAXOccorrenze.Text.Trim() != "")
                {
                    int itmp = 0;
                    if (!int.TryParse(this.uteAppMAXOccorrenze.Text, out itmp)) itmp = 0;

                    if (itmp <= 0)
                    {
                        bRet = false;
                        MessageBox.Show(@"Inserire un valore numerico intero maggiore di 0 (zero)!", "MAX. Occorrenze", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.uteAppMAXOccorrenze.Focus();
                    }
                }
            }
            if (bRet)
            {
                int itmpmin = 0;
                int itmpMAX = 0;
                if (!int.TryParse(this.uteAppMinOccorrenze.Text, out itmpmin)) itmpmin = 0;
                if (!int.TryParse(this.uteAppMAXOccorrenze.Text, out itmpMAX)) itmpMAX = 0;

                if (itmpMAX < itmpmin)
                {
                    bRet = false;
                    MessageBox.Show(@"Inserire valori numerici coerenti!", "Min e MAX Occorrenze", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.uteAppMAXOccorrenze.Focus();
                }
            }

            return bRet;
        }

        private void SaveConfig()
        {

            this.Cursor = Cursors.WaitCursor;

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.BloccoUscita, this.uceBloccoUscita.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.DominioPredefinito, this.uteDominio.Text);

            Scci.Encryption ocrypt = new Encryption(Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, Scci.Statics.EncryptionStatics.GC_INITVECTOR);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.Diagnostics, this.uteDiagnostics.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCI, this.uteWSScci.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCIUserName, this.uteWSScciUserName.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCIPassword, ocrypt.EncryptString(this.uteWSScciPassword.Text));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCIHTTPS, (this.uchkWsSCCIHttps.Checked ? "1" : "0"));

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AnalisiConnectionString, this.uteAnalisiDB.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoSalvataggioScheda, this.uosTipoSalvataggioScheda.Value.ToString());
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PathSalvataggioScheda, this.utePathSalvataggioScheda.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSAC, this.uteWSSAC.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACConsensi, this.uteWSSACConsensi.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACUserName, this.uteUserNameWSSAC.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACPassword, ocrypt.EncryptString(this.utePasswordWSSAC.Text));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWH, this.uteWSDWH.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHLAB, this.uteWSDWHLAB.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHUserName, this.uteUserNameWSDWH.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHPassword, ocrypt.EncryptString(this.utePasswordWSDWH.Text));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOE, this.uteWSOE.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOEUserName, this.uteUserNameWSOE.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOEPassword, ocrypt.EncryptString(this.utePasswordWSOE.Text));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOERR, this.uteWSOERRErogante.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOERRUserName, this.uteWebServiceOERRUserName.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOERRPassword, ocrypt.EncryptString(this.uteWebServiceOERRPassword.Text));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOEOSU, this.uteWSOEOSUErogante.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceAB, this.uteWSAB.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceABUserName, this.uteWSABUserName.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceABPassword, ocrypt.EncryptString(this.uteWSABPassword.Text));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceVNA, this.uteWSVNA.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceVNAUserName, this.uteWSVNAUserName.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceVNAPassword, ocrypt.EncryptString(this.uteWSVNAPassword.Text));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.URLAccessoRefertiDWH, this.uteWebRefertiDWH.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceUFA, this.uteWSUFA.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceUFAUserName, this.uteWSUFAUserName.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceUFAPassword, ocrypt.EncryptString(this.uteWSUFAPassword.Text));

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterAddress, this.uteUpdaterAddress.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterDomain, this.uteUpdaterDomain.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterUserName, this.uteUpdaterUserName.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterPassword, ocrypt.EncryptString(this.uteUpdaterPassword.Text));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterShowUI, (this.uchkUpdaterShowUI.Checked ? "1" : "0"));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterSvcSecDomain, this.uteUpdaterSvcSecDomain.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterSvcSecUserName, this.uteUpdaterSvcSecUsername.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterSvcSecPassword, ocrypt.EncryptString(this.uteUpdaterSvcSecPassword.Text));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterUseUserFolder, (this.uchkUpdaterUserFolder.Checked ? "1" : "0"));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WSUpdaterAlwaysCheckFiles, (this.uchkUpdaterAlwaysCheckFiles.Checked ? "1" : "0"));

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSC, this.uteWSPSC.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSCUserName, this.uteWSPSCUserName.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSCPassword, ocrypt.EncryptString(this.uteWSPSCPassword.Text));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSCAUSL, this.uteWSPSCAUSL.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSCUserNameAUSL, this.uteWSPSCUserNameAUSL.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServicePSCPasswordAUSL, ocrypt.EncryptString(this.uteWSPSCPasswordAUSL.Text));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceMOW, this.uteWSMow.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceMOWAUSL, this.uteWSMowAUSL.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.LetteraDimissioneURL, this.utxtLetteraDimissioneURL.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.LetteraDimissioneURLAUSL, this.utxtLetteraDimissioneURLAUSL.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PscURL, this.utxtPscURL.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PscURLAUSL, this.utxtPscURLAUSL.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PACSRDPServer, this.utePACSRDPServer.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PACSRDPUserName, this.utePACSRDPUserName.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PACSRDPPassword, ocrypt.EncryptString(this.utePACSRDPPassword.Text));

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ReMViewer, this.uteReMViewer.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ReMConnectionString, this.uteReMConnString.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordNews, this.uteNNews.Text);
            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroUltimeRicercheAmbulatoriali, this.uteNPazAmb.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordAllergie, this.uteNAllergie.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordWarning, this.uteNWarning.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordParametriVitali, this.uteNParametri.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordTaskInfermieristici, this.uteNTaskInfermieristici.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordOrdini, this.uteNOrdini.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordAppuntamenti, this.uteNAppuntamenti.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroRecordEvidenzaClinica, this.uteNEvidenzaClinica.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceDWHNumRecords, this.uteNumRecordDWH.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroGiorniEvidenzaClinica, this.uteNumGiorniDWH.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceOENumRecords, this.uteNumRecordOE.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACNumRecords, this.uteNumRecordSAC.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.DiagnosticsClientPercorso, this.uteDiagnosticsClientPercorso.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.DiagnosticsClientGiorni, this.uteDiagnosticsClientGiorni.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.DiagnosticsClientEventiLog, (this.uchkDiagnosticsClientEventiLog.Checked ? "1" : "0"));

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.VersioneSCCI, this.uteVersioneSCCI.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AbilitaControlloVersioneSCCI, (this.uchkAbilitaControlloVersioneSCCI.Checked ? "1" : "0"));

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.VersioneSCCIManagement, this.uteVersioneSCCIManagement.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AbilitaControlloVersioneSCCIManagement, (this.uchkAbilitaControlloVersioneSCCIManagement.Checked ? "1" : "0"));

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoForm, this.uteFontPredefinitoForm.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FontPredefinitoRTF, this.uteFontPredefinitoRTF.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TimerApplicazione, this.uteTimerAggiornamenti.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.MoltiplicatoreNewsHard, this.uteMoltiplicatoreNewsHard.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TimerAmbiente, this.uteTimerAggiornamentiAmb.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TimerFUT, this.uteTimerFUT.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TimerInattivita, this.uteTimerInattivita.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.MinutiRicercaParametriVitaliTrasversali, this.uteTempoParametriVitaliTrasversali.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TempoDimessiTrasferiti, this.uteTempoDimessiTrasferiti.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ElencoCampiAppuntamentoEpisodio, this.uteAppEpisodio.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ElencoCampiAppuntamentoPaziente, this.uteAppPaziente.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AppuntamentiMinOccorrenze, this.uteAppMinOccorrenze.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AppuntamentiMAXOccorrenze, this.uteAppMAXOccorrenze.Text);


            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.LogoEasy, "", this.ucPictureSelectLogoEasy.ViewImage);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.LogoFornitore, "", this.ucPictureSelectLogoFornitori.ViewImage);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.SplashEasy, "", this.ucPictureSelectSplashEasy.ViewImage);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.SplashManagement, "", this.ucPictureSelectSplashManagement.ViewImage);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.RTFIntestazioneStampe, this.ucrtfIntestazioneStampe.ViewRtf);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.RTFIntestazioneStampeSintetica, this.ucrtfIntestazioneStampeSint.ViewRtf);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoSchedaTestataEpisodio, this.utxtSchedaTestataEpisodio.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoSchedaTestataPaziente, this.utxtSchedaTestataPaziente.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoSchedaTaskDaPrescrizione, this.utxtTipoTaskDaPrescrizione.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoPrescrizioneSemplice, this.utxtTipoPrescrizioneSemplice.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.RefertiDWHdaEscludere, this.utxtRefertiDWHdaEscludere.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.CampoSchedaDaPrescrizioneSemplice, this.utxtCampoSchedaPrescrizioneSemplice.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FotoPazienteMaxHeight, this.uteAltezzaFotoPaziente.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FotoPazienteMaxWidth, this.uteLarghezzaFotoPaziente.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroMassimoOwnerFUT, this.uteNMaxOwnerFUT.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PercentualeAltezzaRighaNormaleFUT, this.utePercentualeAltezzaRighaNormaleFUT.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.PercentualeAltezzaRighaCompattaFUT, this.utePercentualeAltezzaRighaCompattaFUT.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.NumeroMassimoCaratteriOwnerFUT, this.uteNMaxCaratteriOwnerFUT.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiDimensioneMassimaMb, this.uteMaxMbAllegati.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiProssimoID, this.uteAllegatiProssimoID.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FattoreCurvaturaGrafici, this.uteSplineTension.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.GraficiMaxGiorniTickmark, this.uteMaxGiorniTickmark.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.RuoloConsulente, this.utxtCodRuoloConsulenze.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.MbMINNetworkAvailable, this.uteMBMin.Text);

            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ColoreFestivitaCalendari, this.ucpColoreFestivitaCalendari.Value.ToString());

            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTColoreCambioGiorno, this.ucpFUTColoreCambioGiorno.Value.ToString());

            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ColoreChiusuraCartelle, this.ucpColoreChiusuraCartelle.Value.ToString());

            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiSchedeAntemprimaWidth, this.uteLarghezzaSchedeAnteprima.Text);
            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiSchedeAntemprimaHeight, this.uteAltezzaSchedeAnteprima.Text);

            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiSchedeStampaWidth, this.uteLarghezzaSchedeStampa.Text);
            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AllegatiSchedeStampaHeight, this.uteAltezzaSchedeStampa.Text);

            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTColoreUltimaSommProgrammata, this.ucpFUTColoreUltimaSommProgrammata.Value.ToString());
            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTCarattereUltimaSommProgrammata, this.uteFUTCarattereUltimaSommProgrammata.Text);
            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTColoreUltimaSommErogata, this.ucpFUTColoreUltimaSommErogata.Value.ToString());
            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTCarattereUltimaSommErogata, this.uteFUTCarattereUltimaSommErogata.Text);

            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ColorePazienteSeguito, this.ucpPazientiSeguitiColoreSfondo.Value.ToString());

            Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTColoreUltimaSommProgrChiusa, this.ucpFUTColoreUltimaSommProgrChiusa.Value.ToString());

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.DeltaOreErogazioneTask, this.uteDeltaOreErogazioneTask.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ApriDOCXtramiteshell, (this.uchkApriDOCXShell.Checked ? "1" : "0"));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ApriPDFtramiteshell, (this.uchkApriPDFShell.Checked ? "1" : "0"));

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TIpoVoceDiarioInfermieristico, this.utxtVoceDiarioinfermieristico.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoVoceDiarioMedico, this.utxtVoceDiarioMedico.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.TipoConsegnaDaDCL, this.utxtTipoConsegnaPaziente.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.Azienda, this.utxtAzienda.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.GestAutoNumCartella, (this.uchkGestAutoNumCartella.Checked ? "1" : "0"));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AssStessoNumCartella, (this.uchkAssStessoNumCartella.Checked ? "1" : "0"));
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.AggiornaSACAperturaCartella, (this.uchkAggiornaSACAperturaCartella.Checked ? "1" : "0"));

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.SACConsensiAbilita, (this.uchkConsenso.Checked ? "1" : "0"));

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.GGPregressiEVCConsultazione, this.uteGGPregressiEVCConsultazione.Text);

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.Debug, (this.uchkDebug.Checked ? "1" : "0"));

            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ElabSistemiUserName, this.utxtUtenteRifElab.Text);
            UnicodeSrl.Scci.Statics.Database.SetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.ElabSistemiCodRuolo, this.utxtRuoloUtenteRifElab.Text);

            Scci.Statics.Database.SetConfigTable(EnumConfigTable.WebSiteFastDRG2, uteURLFastDRG2.Text);

            Scci.Statics.Database.SetConfigTable(EnumConfigTable.ScciWebPathIcone1, this.uteScciWebPathIcone1.Text);
            Scci.Statics.Database.SetConfigTable(EnumConfigTable.ScciWebPathIcone2, this.uteScciWebPathIcone2.Text);
            Scci.Statics.Database.SetConfigTable(EnumConfigTable.ScciWebPathIconeUtente, this.uteScciWebPathIconeUtente.Text);
            Scci.Statics.Database.SetConfigTable(EnumConfigTable.ScciWebPathIconePassword, ocrypt.EncryptString(this.uteScciWebPathIconePassword.Text));

            ocrypt = null;

            if (TableCache.IsInTableCache("T_Config") == true)
                TableCache.RemoveFromCache("T_Config");

            this.Cursor = Cursors.Default;
        }

        #endregion

        #region Events

        private void ubAnnulla_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ubConferma_Click(object sender, EventArgs e)
        {
            if (this.CheckInput())
            {
                this.SaveConfig();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void uteInteger_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
        }

        private void ubReMConnString_Click(object sender, EventArgs e)
        {
            try
            {

                ConnectionStringDialog fd = new ConnectionStringDialog();
                fd.Provider = "System.Data.SqlClient";
                fd.ConnectionString = this.uteReMConnString.Text;

                if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteReMConnString.Text = fd.ConnectionString;
                }

            }
            catch (Exception)
            {

            }
        }

        private void ubFontPredefinito_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.uteFontPredefinitoRTF.Text.Trim() != "")
                {
                    fontDialog.Font = UnicodeSrl.Scci.Statics.DrawingProcs.getFontFromString(this.uteFontPredefinitoRTF.Text);
                }
                else
                {
                    fontDialog.Font = null;
                }
                fontDialog.ShowApply = true;
                if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteFontPredefinitoRTF.Text = UnicodeSrl.Scci.Statics.DrawingProcs.getStringFromFont(fontDialog.Font);
                }
            }
            catch (Exception)
            {
            }
        }

        private void utxtSchedaTestataPaziente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblSchedaTestataPaziente.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_TipoScheda";
                f.ViewSqlStruct.Where = "Codice <> '" + this.utxtSchedaTestataPaziente.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtSchedaTestataPaziente.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void utxtSchedaTestataPaziente_ValueChanged(object sender, EventArgs e)
        {
            this.lblSchedaTestataPazienteDes.Text = DataBase.FindValue("Descrizione", "T_TipoScheda", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtSchedaTestataPaziente.Text)), "");
        }

        private void utxtSchedaTestataEpisodio_ValueChanged(object sender, EventArgs e)
        {
            this.lblSchedaTestataEpisodioDes.Text = DataBase.FindValue("Descrizione", "T_TipoScheda", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtSchedaTestataEpisodio.Text)), "");
        }

        private void utxtSchedaTaskDaPrescrizione_ValueChanged(object sender, EventArgs e)
        {
            this.lblTipoTaskDaPrescrizioneDes.Text = DataBase.FindValue("Descrizione", "T_TipoTaskInfermieristico", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtTipoTaskDaPrescrizione.Text)), "");
        }

        private void utxtSchedaTestataEpisodio_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblSchedaTestataEpisodio.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_TipoScheda";
                f.ViewSqlStruct.Where = "Codice <> '" + this.utxtSchedaTestataEpisodio.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtSchedaTestataEpisodio.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void utxtSchedaTaskDaPrescrizione_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblTipoTaskDaPrescrizione.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_TipoTaskInfermieristico";
                f.ViewSqlStruct.Where = "Codice <> '" + this.utxtTipoTaskDaPrescrizione.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtTipoTaskDaPrescrizione.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void utxtCodRuoloConsulenze_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblCodRuoloConsulenze.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Ruoli";
                f.ViewSqlStruct.Where = "Codice <> '" + this.utxtCodRuoloConsulenze.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtCodRuoloConsulenze.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void utxtCodRuoloConsulenze_ValueChanged(object sender, EventArgs e)
        {
            this.lblCodRuoloConsulenzeDes.Text = DataBase.FindValue("Descrizione", "T_Ruoli", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtCodRuoloConsulenze.Text)), "");
        }

        private void utxtAzienda_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblAzienda.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Aziende";
                f.ViewSqlStruct.Where = "Codice <> '" + this.utxtAzienda.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtAzienda.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void utxtAzienda_ValueChanged(object sender, EventArgs e)
        {
            this.lblAziendaDes.Text = DataBase.FindValue("Descrizione", "T_Aziende", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtAzienda.Text)), "");
        }

        private void utxtLetteraDimissioneURL_ValueChanged(object sender, EventArgs e)
        {

        }

        private void utxtVoceDiarioMedico_ValueChanged(object sender, EventArgs e)
        {
            this.lblVoceDiarioMedico.Text = DataBase.FindValue("Descrizione", "T_TipoVoceDiario", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtVoceDiarioMedico.Text)), "");
        }

        private void utxtVoceDiarioinfermieristico_ValueChanged(object sender, EventArgs e)
        {
            this.lblVoceDiarioInfermieristico.Text = DataBase.FindValue("Descrizione", "T_TipoVoceDiario", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtVoceDiarioinfermieristico.Text)), "");
        }

        private void utxtVoceDiarioMedico_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblVoceDiarioMedico.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select * From T_TipoVoceDiario ";
                f.ViewSqlStruct.Where = "CodTipoDiario = 'M'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtVoceDiarioMedico.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void utxtVoceDiarioinfermieristico_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblVoceDiarioInfermieristico.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select * From T_TipoVoceDiario ";
                f.ViewSqlStruct.Where = "CodTipoDiario = 'I'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtVoceDiarioinfermieristico.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }


        private void utxtTipoConsegnaPaziente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblTipoConsegnaPaziente.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select * From T_TipoConsegnaPaziente";
                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtTipoConsegnaPaziente.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void utxtTipoConsegnaPaziente_ValueChanged(object sender, EventArgs e)
        {
            this.lblTipoConsegnaPaziente.Text = DataBase.FindValue("Descrizione", "T_TipoConsegnaPaziente", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtTipoConsegnaPaziente.Text)), "");
        }

        private void ucpColore_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.ColorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ((Infragistics.Win.UltraWinEditors.UltraColorPicker)sender).Value = this.ColorDialog.Color;
            }
        }

        private void utxtUtenteRifElab_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblUtenteRifElab.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Login";
                f.ViewSqlStruct.Where = "Codice <> '" + this.utxtUtenteRifElab.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtUtenteRifElab.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void utxtUtenteRifElab_ValueChanged(object sender, EventArgs e)
        {
            this.lblUtenteRifElabDes.Text = DataBase.FindValue("Descrizione", "T_Login", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtUtenteRifElab.Text)), "");
        }

        private void utxtRuoloUtenteRifElab_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblRuoloUtenteRifElab.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_Ruoli";
                f.ViewSqlStruct.Where = "Codice <> '" + this.utxtRuoloUtenteRifElab.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtRuoloUtenteRifElab.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void utxtRuoloUtenteRifElab_ValueChanged(object sender, EventArgs e)
        {
            this.lblRuoloUtenteRifElabDes.Text = DataBase.FindValue("Descrizione", "T_Ruoli", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtRuoloUtenteRifElab.Text)), "");
        }

        private void lblTipoPrescrizioneSempliceDes_VisibleChanged(object sender, EventArgs e)
        {
            this.lblTipoPrescrizioneSempliceDes.Text = DataBase.FindValue("Descrizione", "T_TipoPrescrizione", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtTipoPrescrizioneSemplice.Text)), "");

        }

        private void utxtTipoPrescrizioneSemplice_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblTipoPrescrizioneSempliceDes.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_TipoPrescrizione";
                f.ViewSqlStruct.Where = "Codice <> '" + this.utxtTipoPrescrizioneSemplice.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.utxtTipoPrescrizioneSemplice.Text = f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void utxtTipoPrescrizioneSemplice_ValueChanged(object sender, EventArgs e)
        {
            this.lblTipoPrescrizioneSempliceDes.Text = DataBase.FindValue("Descrizione", "T_TipoPrescrizione", string.Format("Codice = '{0}'", DataBase.Ax2(this.utxtTipoPrescrizioneSemplice.Text)), "");
        }

        private void lblRefertiDWHdaEscludereDes_VisibleChanged(object sender, EventArgs e)
        {
            loadRefertiDWHdaEscludereDes();
        }

        private void utxtRefertiDWHdaEscludere_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {

                frmZoom f = new frmZoom();
                f.ViewText = this.lblRefertiDWHdaEscludereDes.Text;
                f.ViewIcon = this.ViewIcon;
                f.ViewSqlStruct.Sql = "Select Codice, Descrizione From T_TipoEvidenzaClinica";
                f.ViewSqlStruct.Where = "Codice <> '" + this.utxtRefertiDWHdaEscludere.Text + @"'";

                f.ViewInit();
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    if (this.utxtRefertiDWHdaEscludere.Text.Trim() != "") this.utxtRefertiDWHdaEscludere.Text += @"|";
                    this.utxtRefertiDWHdaEscludere.Text += f.ViewActiveRow.Cells["Codice"].Text;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void utxtRefertiDWHdaEscludere_ValueChanged(object sender, EventArgs e)
        {
            loadRefertiDWHdaEscludereDes();
        }

        private void loadRefertiDWHdaEscludereDes()
        {
            string sDescr = "";
            try
            {
                string sCodes = this.utxtRefertiDWHdaEscludere.Text;
                if (sCodes != null && sCodes.Trim() != "")
                {

                    string[] arrCodes = sCodes.Split('|');

                    for (int i = arrCodes.GetLowerBound(0); i <= arrCodes.GetUpperBound(0); i++)
                    {
                        if (arrCodes[i] != null && arrCodes[i].Trim() != "")
                        {
                            if (sDescr != "") sDescr += @", ";
                            sDescr += DataBase.FindValue("Descrizione", "T_TipoEvidenzaClinica", string.Format("Codice = '{0}'", DataBase.Ax2(arrCodes[i])), @"[" + arrCodes[i] + @"]");
                        }
                    }

                }
            }
            catch
            {
                sDescr = this.utxtRefertiDWHdaEscludere.Text;
            }

            this.lblRefertiDWHdaEscludereDes.Text = sDescr;
        }

        private void ubAnalisiDB_Click(object sender, EventArgs e)
        {
            try
            {

                ConnectionStringDialog fd = new ConnectionStringDialog();
                fd.Provider = "System.Data.SqlClient";
                fd.ConnectionString = this.uteAnalisiDB.Text;

                if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    this.uteAnalisiDB.Text = fd.ConnectionString;
                }

            }
            catch (Exception)
            {

            }
        }

        private void uosTipoSalvataggioScheda_ValueChanged(object sender, EventArgs e)
        {

            switch ((EnumTipoSalvataggioScheda)Enum.Parse(typeof(EnumTipoSalvataggioScheda), this.uosTipoSalvataggioScheda.Value.ToString()))
            {

                case UnicodeSrl.Scci.Enums.EnumTipoSalvataggioScheda.N:
                case UnicodeSrl.Scci.Enums.EnumTipoSalvataggioScheda.L:
                    this.utePathSalvataggioScheda.Enabled = false;
                    break;

                case UnicodeSrl.Scci.Enums.EnumTipoSalvataggioScheda.R:
                    this.utePathSalvataggioScheda.Enabled = true;
                    break;
            }

        }

        private void ucpPazientiSeguitiColoreSfondo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.ColorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ((Infragistics.Win.UltraWinEditors.UltraColorPicker)sender).Value = this.ColorDialog.Color;
            }
        }

        #endregion

    }
}
