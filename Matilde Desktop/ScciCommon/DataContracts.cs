using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Text;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.Scci.DataContracts
{

    [Serializable()]
    public class ScciAmbiente
    {

        public ScciAmbiente()
        {

            Codlogin = string.Empty;
            Codruolo = string.Empty;
            Nomepc = string.Empty;
            Indirizzoip = string.Empty;
            Idpaziente = string.Empty;
            Idepisodio = string.Empty;
            IdTrasferimento = string.Empty;
            IdCartella = string.Empty;
            Contesto = new Dictionary<string, object>();

        }

        public string Codlogin { get; set; }

        public string Codruolo { get; set; }

        public string Nomepc { get; set; }

        public string Indirizzoip { get; set; }

        public string Idpaziente { get; set; }

        public string Idepisodio
        {
            get;
            set;
        }

        public string IdTrasferimento { get; set; }

        public string IdCartella { get; set; }

        [XmlIgnore]
        public Dictionary<string, object> Contesto { get; set; }

    }

    [DataContract(Name = "PazienteSac")]
    [Serializable]
    public class PazienteSac
    {

        public PazienteSac()
        {
            CodSAC = string.Empty;
            Cognome = string.Empty;
            Nome = string.Empty;
            Sesso = string.Empty;
            CodiceFiscale = string.Empty;
            Nazionalita = string.Empty;
            Paziente = string.Empty;
            DataNascita = DateTime.MinValue;
            CodComuneNascita = string.Empty;
            ComuneNascita = string.Empty;
            LocalitaNascita = string.Empty;
            CodProvinciaNascita = string.Empty;
            ProvinciaNascita = string.Empty;
            NascitaDescrizione = string.Empty;
            CAPResidenza = string.Empty;
            CodComuneResidenza = string.Empty;
            ComuneResidenza = string.Empty;
            IndirizzoResidenza = string.Empty;
            LocalitaResidenza = string.Empty;
            CodProvinciaResidenza = string.Empty;
            ProvinciaResidenza = string.Empty;
            CodRegioneResidenza = string.Empty;
            RegioneResidenza = string.Empty;
            CAPDomicilio = string.Empty;
            CodComuneDomicilio = string.Empty;
            ComuneDomicilio = string.Empty;
            IndirizzoDomicilio = string.Empty;
            LocalitaDomicilio = string.Empty;
            CodProvinciaDomicilio = string.Empty;
            ProvinciaDomicilio = string.Empty;
            TerminazioneCodice = string.Empty;
            TerminazioneDescrizione = string.Empty;
            TerminazioneData = DateTime.MinValue;
            DataDecesso = DateTime.MinValue;
        }

        [DataMember]
        public string CodSAC { get; set; }
        [DataMember]
        public string Cognome { get; set; }
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public string Sesso { get; set; }
        [DataMember]
        public string CodiceFiscale { get; set; }
        [DataMember]
        public string Nazionalita { get; set; }
        [DataMember]
        public string Paziente { get; set; }

        [DataMember]
        public DateTime DataNascita { get; set; }
        [DataMember]
        public string CodComuneNascita { get; set; }
        [DataMember]
        public string ComuneNascita { get; set; }
        [DataMember]
        public string LocalitaNascita { get; set; }
        [DataMember]
        public string CodProvinciaNascita { get; set; }
        [DataMember]
        public string ProvinciaNascita { get; set; }
        [DataMember]
        public string NascitaDescrizione { get; set; }

        [DataMember]
        public string TerminazioneCodice { get; set; }
        [DataMember]
        public string TerminazioneDescrizione { get; set; }
        [DataMember]
        public DateTime TerminazioneData { get; set; }
        [DataMember]
        public DateTime DataDecesso { get; set; }

        [DataMember]
        public string CAPResidenza { get; set; }
        [DataMember]
        public string CodComuneResidenza { get; set; }
        [DataMember]
        public string ComuneResidenza { get; set; }
        [DataMember]
        public string IndirizzoResidenza { get; set; }
        [DataMember]
        public string LocalitaResidenza { get; set; }
        [DataMember]
        public string CodProvinciaResidenza { get; set; }
        [DataMember]
        public string ProvinciaResidenza { get; set; }
        [DataMember]
        public string CodRegioneResidenza { get; set; }
        [DataMember]
        public string RegioneResidenza { get; set; }

        [DataMember]
        public string CAPDomicilio { get; set; }
        [DataMember]
        public string CodComuneDomicilio { get; set; }
        [DataMember]
        public string ComuneDomicilio { get; set; }
        [DataMember]
        public string IndirizzoDomicilio { get; set; }
        [DataMember]
        public string LocalitaDomicilio { get; set; }
        [DataMember]
        public string CodProvinciaDomicilio { get; set; }
        [DataMember]
        public string ProvinciaDomicilio { get; set; }

    }

    [DataContract(Name = "PazienteSacDatiAggiuntivi")]
    [Serializable]
    public class PazienteSacDatiAggiuntivi
    {

        public PazienteSacDatiAggiuntivi()
        {

            Telefono1 = string.Empty;
            Telefono2 = string.Empty;
            Telefono3 = string.Empty;

            CodiceMedicoDiBase = 0;
            CodiceFiscaleMedicoDiBase = string.Empty;
            CognomeNomeMedicoDiBase = string.Empty;
            DistrettoMedicoDiBase = string.Empty;
            DataSceltaMedicoDiBase = DateTime.MinValue;

            Esenzioni = new List<PazienteSacEsenzioni>();
            Consensi = new List<PazienteSacConsensi>();

        }

        [DataMember]
        public string Telefono1 { get; set; }
        [DataMember]
        public string Telefono2 { get; set; }
        [DataMember]
        public string Telefono3 { get; set; }

        [DataMember]
        public int CodiceMedicoDiBase { get; set; }
        [DataMember]
        public string CodiceFiscaleMedicoDiBase { get; set; }
        [DataMember]
        public string CognomeNomeMedicoDiBase { get; set; }
        [DataMember]
        public string DistrettoMedicoDiBase { get; set; }
        [DataMember]
        public DateTime DataSceltaMedicoDiBase { get; set; }
        [DataMember]
        public List<PazienteSacEsenzioni> Esenzioni { get; set; }
        [DataMember]
        public List<PazienteSacConsensi> Consensi { get; set; }

        public string DescrizioniEsenzioni(DateTime datacompetenza)
        {

            string sret = string.Empty;

            foreach (PazienteSacEsenzioni oesenzione in Esenzioni)
            {

                if (datacompetenza >= oesenzione.DataInizioValidita &&
                    (oesenzione.DataFineValidita == DateTime.MinValue || datacompetenza <= oesenzione.DataFineValidita))
                {
                    sret += oesenzione.TestoEsenzione;
                    sret += " (" + oesenzione.CodiceEsenzione + ") - ";
                    sret += " (dal " + oesenzione.DataInizioValidita.ToString("dd/MM/yyyy");
                    if (oesenzione.DataFineValidita != DateTime.MinValue) sret += " al " + oesenzione.DataFineValidita.ToString("dd/MM/yyyy");
                    sret += ")";
                    if (oesenzione.DecodificaEsenzioneDiagnosi != string.Empty || oesenzione.CodiceDiagnosi != string.Empty)
                    {
                        sret += " - ";
                        sret += oesenzione.DecodificaEsenzioneDiagnosi;
                        sret += " (" + oesenzione.CodiceDiagnosi + ")";
                    }

                    sret += Environment.NewLine;
                }
            }

            return sret;
        }
    }

    [DataContract(Name = "PazienteSacEsenzioni")]
    [Serializable]
    public class PazienteSacEsenzioni
    {

        public PazienteSacEsenzioni()
        {

            CodiceEsenzione = string.Empty;
            TestoEsenzione = string.Empty;
            DataInizioValidita = DateTime.MinValue;
            DataFineValidita = DateTime.MinValue;
            CodiceDiagnosi = string.Empty;
            DecodificaEsenzioneDiagnosi = string.Empty;

        }

        [DataMember]
        public string CodiceEsenzione { get; set; }
        [DataMember]
        public string TestoEsenzione { get; set; }
        [DataMember]
        public DateTime DataInizioValidita { get; set; }
        [DataMember]
        public DateTime DataFineValidita { get; set; }
        [DataMember]
        public string CodiceDiagnosi { get; set; }
        [DataMember]
        public string DecodificaEsenzioneDiagnosi { get; set; }

    }

    public class PazienteSacConsensi
    {

        public PazienteSacConsensi()
        {

            Provenienza = string.Empty;
            IDProvenienza = string.Empty;
            Tipo = string.Empty;
            DataStato = DateTime.MinValue;
            Stato = false;
            OperatoreId = string.Empty;
            OperatoreCognome = string.Empty;
            OperatoreNome = string.Empty;
            OperatoreComputer = string.Empty;

        }

        [DataMember]
        public string Provenienza { get; set; }

        [DataMember]
        public string IDProvenienza { get; set; }

        [DataMember]
        public string Tipo { get; set; }

        [DataMember]
        public DateTime DataStato { get; set; }

        [DataMember]
        public bool Stato { get; set; }

        [DataMember]
        public string OperatoreId { get; set; }

        [DataMember]
        public string OperatoreCognome { get; set; }

        [DataMember]
        public string OperatoreNome { get; set; }

        [DataMember]
        public string OperatoreComputer { get; set; }

    }


    [DataContract(Name = "RefertoDWH")]
    public class RefertoDWH
    {

        public RefertoDWH()
        {

            IDReferto = string.Empty;
            AziendaErogante = string.Empty;
            SistemaErogante = string.Empty;
            RepartoErogante = string.Empty;
            DataReferto = DateTime.MinValue;
            NumeroReferto = string.Empty;
            NumeroNosologico = string.Empty;
            NumeroPrenotazione = string.Empty;
            DWHCodRepartoRichiedente = string.Empty;
            DWHDescRepartoRichiedente = string.Empty;
            DWHCodStatoRichiesta = string.Empty;
            DWHDescStatoRichiesta = string.Empty;
            DWHCodTipoRichiesta = string.Empty;
            DWHDescTipoRichiesta = string.Empty;
            CodTipoEvidenzaClinica = string.Empty;
            DescTipoEvidenzaClinica = string.Empty;
            CodStatoEvidenzaClinica = string.Empty;
            DescStatoEvidenzaClinica = string.Empty;
            TestoAnteprima = string.Empty;
            DataEventoDWH = DateTime.MinValue;
            Firmato = string.Empty;

        }

        [DataMember]
        public string IDReferto { get; set; }
        [DataMember]
        public string AziendaErogante { get; set; }
        [DataMember]
        public string SistemaErogante { get; set; }
        [DataMember]
        public string RepartoErogante { get; set; }
        [DataMember]
        public DateTime DataReferto { get; set; }
        [DataMember]
        public string NumeroReferto { get; set; }
        [DataMember]
        public string NumeroNosologico { get; set; }
        [DataMember]
        public string NumeroPrenotazione { get; set; }
        [DataMember]
        public string DWHCodRepartoRichiedente { get; set; }
        [DataMember]
        public string DWHDescRepartoRichiedente { get; set; }
        [DataMember]
        public string DWHCodStatoRichiesta { get; set; }
        [DataMember]
        public string DWHDescStatoRichiesta { get; set; }
        [DataMember]
        public string DWHCodTipoRichiesta { get; set; }
        [DataMember]
        public string DWHDescTipoRichiesta { get; set; }
        [DataMember]
        public string CodTipoEvidenzaClinica { get; set; }
        [DataMember]
        public string DescTipoEvidenzaClinica { get; set; }
        [DataMember]
        public string CodStatoEvidenzaClinica { get; set; }
        [DataMember]
        public string DescStatoEvidenzaClinica { get; set; }
        [DataMember]
        public string TestoAnteprima { get; set; }
        [DataMember]
        public DateTime DataEventoDWH { get; set; }
        [DataMember]
        public string Firmato { get; set; }

    }

    [DataContract(Name = "AllegatoRefertoDWH")]
    public class AllegatoRefertoDWH
    {

        public AllegatoRefertoDWH()
        {
            IDReferto = string.Empty;
            DataReferto = DateTime.MinValue;
            NumeroReferto = string.Empty;
            IDAllegato = string.Empty;
            IdPaziente = string.Empty;
            DataInserimento = DateTime.MinValue;
            DataModifica = DateTime.MinValue;
            NomeFile = string.Empty;
            DescrizioneFile = string.Empty;
            CodStatoAllegato = string.Empty;
            DescrStatoAllegato = string.Empty;
            MimeType = string.Empty;
            FileData = null;
            IdOrderEntry = string.Empty;
        }

        [DataMember]
        public string IDReferto { get; set; }
        [DataMember]
        public DateTime DataReferto { get; set; }
        [DataMember]
        public string NumeroReferto { get; set; }
        [DataMember]
        public string IDAllegato { get; set; }
        [DataMember]
        public string IdPaziente { get; set; }
        [DataMember]
        public DateTime DataInserimento { get; set; }
        [DataMember]
        public DateTime DataModifica { get; set; }
        [DataMember]
        public string NomeFile { get; set; }
        [DataMember]
        public string DescrizioneFile { get; set; }
        [DataMember]
        public string CodStatoAllegato { get; set; }
        [DataMember]
        public string DescrStatoAllegato { get; set; }
        [DataMember]
        public string MimeType { get; set; }
        [DataMember]
        public byte[] FileData { get; set; }
        [DataMember]
        public string IdOrderEntry { get; set; }

    }


    [DataContract(Name = "RicoveroDWHSintetico")]
    public class RicoveroDWHSintetico
    {

        public RicoveroDWHSintetico()
        {
            IDRicovero = string.Empty;
            IDPaziente = string.Empty;
            Nosologico = string.Empty;
            AziendaErogante = string.Empty;
            DataInizioRicovero = DateTime.MinValue;
            DataFineRicovero = DateTime.MinValue;
            Diagnosi = string.Empty;
            DescTipoEpisodio = string.Empty;
            DescRepartoAmmissione = string.Empty;
            DescRepartoDimissione = string.Empty;
        }

        [DataMember]
        public string IDRicovero { get; set; }

        [DataMember]
        public string IDPaziente { get; set; }

        [DataMember]
        public string Nosologico { get; set; }

        [DataMember]
        public string AziendaErogante { get; set; }

        [DataMember]
        public string DescTipoEpisodio { get; set; }

        [DataMember]
        public DateTime DataInizioRicovero { get; set; }

        [DataMember]
        public string Diagnosi { get; set; }

        [DataMember]
        public string DescRepartoAmmissione { get; set; }

        [DataMember]
        public DateTime DataFineRicovero { get; set; }

        [DataMember]
        public string DescRepartoDimissione { get; set; }

    }

    [DataContract(Name = "RicoveroDWH")]
    public class RicoveroDWH : RicoveroDWHSintetico
    {

        public RicoveroDWH()
        {
            IDRicovero = string.Empty;
            IDPaziente = string.Empty;
            Nosologico = string.Empty;
            AziendaErogante = string.Empty;
            DataInizioRicovero = DateTime.MinValue;
            DataFineRicovero = DateTime.MinValue;
            Diagnosi = string.Empty;
            DescTipoEpisodio = string.Empty;
            DescRepartoAmmissione = string.Empty;
            DescRepartoDimissione = string.Empty;

            UltimoEvento = string.Empty;
            EpisodioOrigine = string.Empty;
            ProvenienzaPaziente = string.Empty;
            MotivoRicovero = string.Empty;
            TipoRicovero = string.Empty;
            EventiDWH = new List<EventoDWH>();

        }

        [DataMember]
        public string UltimoEvento { get; set; }

        [DataMember]
        public string EpisodioOrigine { get; set; }

        [DataMember]
        public string ProvenienzaPaziente { get; set; }

        [DataMember]
        public string MotivoRicovero { get; set; }

        [DataMember]
        public string TipoRicovero { get; set; }

        [DataMember]
        public List<EventoDWH> EventiDWH { get; set; }

    }

    [DataContract(Name = "EventoDWH")]
    public class EventoDWH
    {

        public EventoDWH()
        {

            IDEvento = string.Empty;
            Nosologico = string.Empty;
            AziendaErogante = string.Empty;
            SistemaErogante = string.Empty;
            RepartoErogante = string.Empty;
            DataEvento = DateTime.MinValue;
            CodTipoEvento = string.Empty;
            DescTipoEvento = string.Empty;
            CodTipoEpisodio = string.Empty;
            DescTipoEpisodio = string.Empty;
            CodReparto = string.Empty;
            DescReparto = string.Empty;
            CodSettore = string.Empty;
            DescSettore = string.Empty;
            CodLetto = string.Empty;
            DescLetto = string.Empty;
            Diagnosi = string.Empty;

        }

        [DataMember]
        public string IDEvento { get; set; }

        [DataMember]
        public string Nosologico { get; set; }

        [DataMember]
        public string AziendaErogante { get; set; }

        [DataMember]
        public string SistemaErogante { get; set; }

        [DataMember]
        public string RepartoErogante { get; set; }

        [DataMember]
        public DateTime DataEvento { get; set; }

        [DataMember]
        public string CodTipoEvento { get; set; }

        [DataMember]
        public string DescTipoEvento { get; set; }

        [DataMember]
        public string CodTipoEpisodio { get; set; }

        [DataMember]
        public string DescTipoEpisodio { get; set; }

        [DataMember]
        public string CodReparto { get; set; }

        [DataMember]
        public string DescReparto { get; set; }

        [DataMember]
        public string CodSettore { get; set; }

        [DataMember]
        public string DescSettore { get; set; }

        [DataMember]
        public string CodLetto { get; set; }

        [DataMember]
        public string DescLetto { get; set; }

        [DataMember]
        public string Diagnosi { get; set; }

    }

    [DataContract(Name = "RisultatiLab")]
    public class RisultatiLab
    {

        public RisultatiLab()
        {
            IdReferto = string.Empty;
            CodSezione = string.Empty;
            DescrSezione = string.Empty;
            CodPrescrizione = string.Empty;
            DescPrescrizione = string.Empty;
            Data = DateTime.MinValue;
            Quantita = 0;
        }

        [DataMember]
        public string IdReferto { get; set; }
        [DataMember]
        public string CodSezione { get; set; }
        [DataMember]
        public string DescrSezione { get; set; }
        [DataMember]
        public string CodPrescrizione { get; set; }
        [DataMember]
        public string DescPrescrizione { get; set; }
        [DataMember]
        public DateTime Data { get; set; }
        [DataMember]
        public Double Quantita { get; set; }

    }

    [DataContract(Name = "RisultatiLabUm")]
    public class RisultatiLabUM
    {

        public RisultatiLabUM()
        {
            IdReferto = string.Empty;
            CodSezione = string.Empty;
            DescrSezione = string.Empty;
            CodPrescrizione = string.Empty;
            DescPrescrizione = string.Empty;
            Data = DateTime.MinValue;
            Quantita = 0;
            UM = string.Empty;
        }

        [DataMember]
        public string IdReferto { get; set; }
        [DataMember]
        public string CodSezione { get; set; }
        [DataMember]
        public string DescrSezione { get; set; }
        [DataMember]
        public string CodPrescrizione { get; set; }
        [DataMember]
        public string DescPrescrizione { get; set; }
        [DataMember]
        public DateTime Data { get; set; }
        [DataMember]
        public Double Quantita { get; set; }
        [DataMember]
        public string UM { get; set; }
    }

    [DataContract(Name = "RefertoDWHDetailed")]
    public class RefertoDWHDetailed : RefertoDWH
    {

        public RefertoDWHDetailed()
        {

            IDReferto = string.Empty;
            AziendaErogante = string.Empty;
            SistemaErogante = string.Empty;
            RepartoErogante = string.Empty;
            DataReferto = DateTime.MinValue;
            NumeroReferto = string.Empty;
            NumeroNosologico = string.Empty;
            NumeroPrenotazione = string.Empty;
            DWHCodRepartoRichiedente = string.Empty;
            DWHDescRepartoRichiedente = string.Empty;
            DWHCodStatoRichiesta = string.Empty;
            DWHDescStatoRichiesta = string.Empty;
            DWHCodTipoRichiesta = string.Empty;
            DWHDescTipoRichiesta = string.Empty;
            CodTipoEvidenzaClinica = string.Empty;
            DescTipoEvidenzaClinica = string.Empty;
            CodStatoEvidenzaClinica = string.Empty;
            DescStatoEvidenzaClinica = string.Empty;
            TestoAnteprima = string.Empty;

            Sesso = string.Empty;
            CodiceSAUB = string.Empty;
            CodiceSanitario = string.Empty;
            AccessNumber = string.Empty;
            PrestazioniReferto = new List<PrestazioneReferto>();
            Priorita = string.Empty;
            IdOrderEntry = string.Empty;
        }

        [DataMember]
        public string Sesso { get; set; }
        [DataMember]
        public string CodiceSAUB { get; set; }
        [DataMember]
        public string CodiceSanitario { get; set; }
        [DataMember]
        public string AccessNumber { get; set; }
        [DataMember]
        public List<PrestazioneReferto> PrestazioniReferto { get; set; }
        [DataMember]
        public string Priorita { get; set; }
        [DataMember]
        public string IdOrderEntry { get; set; }

    }

    [DataContract(Name = "PrestazioneReferto")]
    public class PrestazioneReferto
    {

        public PrestazioneReferto()
        {

            ID = string.Empty;
            SezioneDescrizione = string.Empty;
            PrestazioneCodice = string.Empty;
            PrestazioneDescrizione = string.Empty;
            Risultato = string.Empty;
            ValoriRiferimento = string.Empty;
            GravitaDescrizione = string.Empty;
            AccessNumber = string.Empty;
            SezionePosizione = 0;
            PrestazionePosizione = 0;
        }

        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string SezioneDescrizione { get; set; }

        [DataMember]
        public string PrestazioneCodice { get; set; }

        [DataMember]
        public string PrestazioneDescrizione { get; set; }

        [DataMember]
        public string Risultato { get; set; }

        [DataMember]
        public string ValoriRiferimento { get; set; }

        [DataMember]
        public string GravitaDescrizione { get; set; }

        [DataMember]
        public string AccessNumber { get; set; }

        [DataMember]
        public int SezionePosizione { get; set; }

        [DataMember]
        public int PrestazionePosizione { get; set; }

    }

    [DataContract(Name = "OEOrdineTestata")]
    public class OEOrdineTestata
    {

        [DataMember]
        public DateTime DataOrdine { get; set; }
        [DataMember]
        public DateTime DataModifica { get; set; }

        [DataMember]
        public string Operatore { get; set; }
        [DataMember]
        public string NumeroOrdine { get; set; }
        [DataMember]
        public string IdOrdine { get; set; }
        [DataMember]
        public string DescrizioneSistemaRichiedente { get; set; }

        [DataMember]
        public string Nosologico { get; set; }

        [DataMember]
        public OEPrioritaOrdine Priorita { get; set; }

        [DataMember]
        public string PrioritaDesc { get; set; }

        [DataMember]
        public string Regime { get; set; }

        [DataMember]
        public DateTime DataOraProgrammata { get; set; }
        [DataMember]
        public DateTime DataOraProgrammataCalcolata { get; set; }
        [DataMember]
        public DateTime DataOraPreferita { get; set; }

        [DataMember]
        public string PazienteId { get; set; }
        [DataMember]
        public string PazienteNome { get; set; }
        [DataMember]
        public string PazienteCognome { get; set; }
        [DataMember]
        public DateTime PazienteDataNascita { get; set; }
        [DataMember]
        public string PazienteCF { get; set; }
        [DataMember]
        public string PazienteSesso { get; set; }

        [DataMember]
        public string UnitaOperativaCodice { get; set; }
        [DataMember]
        public string UnitaOperativaDescrizione { get; set; }
        [DataMember]
        public string UnitaOperativaAziendaCodice { get; set; }
        [DataMember]
        public string UnitaOperativaAziendaDescrizione { get; set; }

        [DataMember]
        public OEValiditaOrdine StatoValidazione { get; set; }
        [DataMember]
        public string DescrizioneStatoValidazione { get; set; }

        [DataMember]
        public OEStato Stato { get; set; }

        [DataMember]
        public string Eroganti { get; set; }

        [DataMember]
        public List<OESistemaErogante> ErogantiLista { get; set; }

        [DataMember]
        public int NumeroPrestazioni { get; set; }
        [DataMember]
        public string AnteprimaPrestazioni { get; set; }

        [DataMember]
        public bool Cancellabile { get; set; }

        public OEOrdineTestata()
        {
            InitBasicVars();
        }

        private void InitBasicVars()
        {
            DataOrdine = new DateTime();
            DataModifica = new DateTime();

            Operatore = "";
            NumeroOrdine = "";

            IdOrdine = "";
            DescrizioneSistemaRichiedente = "";

            Nosologico = "";

            Priorita = OEPrioritaOrdine.NN;
            PrioritaDesc = string.Empty;

            Regime = "";

            DataOraProgrammata = new DateTime();
            DataOraProgrammataCalcolata = new DateTime();

            PazienteId = "";
            PazienteNome = "";
            PazienteCognome = "";
            PazienteDataNascita = DateTime.MinValue;
            PazienteCF = "";
            PazienteSesso = "";

            UnitaOperativaCodice = "";
            UnitaOperativaDescrizione = "";
            UnitaOperativaAziendaCodice = "";
            UnitaOperativaAziendaDescrizione = "";

            StatoValidazione = OEValiditaOrdine.NN;
            DescrizioneStatoValidazione = "";

            Stato = OEStato.NN;

            Eroganti = "";
            ErogantiLista = new List<OESistemaErogante>();

            NumeroPrestazioni = 0;
            AnteprimaPrestazioni = "";

        }

    }

    [DataContract(Name = "OEPrestazione")]
    public class OEPrestazione
    {

        [DataMember]
        public string Codice { get; set; }
        [DataMember]
        public string Descrizione { get; set; }

        [DataMember]
        public OEPrestazioneTipo Tipo { get; set; }

        [DataMember]
        public OESistemaErogante Erogante { get; set; }

        public OEPrestazione()
        {
            InitBasicVars();
        }

        public OEPrestazione(string s_codice_p, string s_codice_azienda_p)
        {
            InitBasicVars();

            Codice = s_codice_p;
            Erogante.Codice = "???";
            Erogante.CodiceAzienda = s_codice_azienda_p;
        }

        public OEPrestazione(string s_codice_p, string s_codice_azienda_p, string s_codice_sistema_p)
        {
            InitBasicVars();

            Codice = s_codice_p;
            Erogante.Codice = s_codice_sistema_p;
            Erogante.CodiceAzienda = s_codice_azienda_p;
        }

        public OEPrestazione(string s_codice_p, string s_descrizione_p, OEPrestazioneTipo e_tipo_p, OESistemaErogante o_erogante_p)
        {
            InitBasicVars();

            Codice = s_codice_p;
            Descrizione = s_descrizione_p;
            Tipo = e_tipo_p;
            Erogante = o_erogante_p;
        }

        private void InitBasicVars()
        {
            Codice = "";
            Descrizione = "";
            Tipo = OEPrestazioneTipo.NN;
            Erogante = new OESistemaErogante();
        }

    }

    [DataContract(Name = "OESistemaErogante")]
    public class OESistemaErogante
    {

        [DataMember]
        public string Codice { get; set; }
        [DataMember]
        public string Descrizione { get; set; }
        [DataMember]
        public string CodiceAzienda { get; set; }
        [DataMember]
        public string DescrizioneAzienda { get; set; }

        public OESistemaErogante()
        {
            InitBasicVars();
        }

        public OESistemaErogante(string s_c, string s_d, string s_c_azienda, string s_d_azienda)
        {
            InitBasicVars();

            Codice = s_c;
            Descrizione = s_d;
            CodiceAzienda = s_c_azienda;
            DescrizioneAzienda = s_d_azienda;

        }

        private void InitBasicVars()
        {
            Codice = "";
            Descrizione = "";
            CodiceAzienda = "";
            DescrizioneAzienda = "";
        }

    }

    [DataContract(Name = "OEOrdinePrestazione")]
    public class OEOrdinePrestazione
    {
        [DataMember]
        public OEPrestazione Prestazione { get; set; }
        [DataMember]
        public OEStatoRichiedente StatoRichiedente { get; set; }

        [DataMember]
        public OEStatoErogante StatoErogante { get; set; }
        [DataMember]
        public OEValiditaOrdine StatoValidazione { get; set; }
        [DataMember]
        public string DescrizioneValidazione { get; set; }
        [DataMember]
        public List<OEDatoAccessorio> DatiAccessoriRichiedente { get; set; }
        [DataMember]
        public List<OEDatoAccessorio> DatiAccessoriErogante { get; set; }

        [DataMember]
        public DateTime? DataPianificata { get; set; }

        public OEOrdinePrestazione()
        {
            InitBasicVars();
        }

        private void InitBasicVars()
        {
            Prestazione = new OEPrestazione();

            StatoErogante = OEStatoErogante.NN;
            StatoRichiedente = OEStatoRichiedente.NN;

            StatoValidazione = OEValiditaOrdine.NN;
            DescrizioneValidazione = "";

            DatiAccessoriRichiedente = new List<OEDatoAccessorio>();
            DatiAccessoriErogante = new List<OEDatoAccessorio>();

            DataPianificata = null;
        }

    }

    [DataContract(Name = "OEOrdineDettaglio")]
    public class OEOrdineDettaglio
    {

        [DataMember]
        public OEOrdineTestata OrdineTestata { get; set; }
        [DataMember]
        public List<OEOrdinePrestazione> Prestazioni { get; set; }

        public OEOrdineDettaglio()
        {
            InitBasicVars();
        }

        private void InitBasicVars()
        {
            OrdineTestata = null;
            Prestazioni = new List<OEOrdinePrestazione>();
        }

        public OEOrdinePrestazione GetPrestazione(string s_codazienda, string s_coderogante, string s_codprestazione)
        {
            foreach (OEOrdinePrestazione o_prestazione in Prestazioni)
                if (
                    (s_codazienda == o_prestazione.Prestazione.Erogante.CodiceAzienda) &&
                    (s_coderogante == o_prestazione.Prestazione.Erogante.Codice) &&
                    (s_codprestazione == o_prestazione.Prestazione.Codice)
                    )
                    return o_prestazione;

            return null;
        }

    }

    [DataContract(Name = "OEDatiAccessoriDescrittore")]
    public class OEDatiAccessoriDescrittore
    {
        [DataMember]
        public string Codice { get; set; }
        [DataMember]
        public string Descrizione { get; set; }
        [DataMember]
        public string Etichetta { get; set; }
        [DataMember]
        public string Gruppo { get; set; }

        [DataMember]
        public bool Obbligatorio { get; set; }
        [DataMember]
        public int Ordinamento { get; set; }

        [DataMember]
        public bool Ripetibile { get; set; }
        [DataMember]
        public string Tipo { get; set; }
        [DataMember]
        public string Valori { get; set; }
        [DataMember]
        public string ValidazioneMessaggio { get; set; }
        [DataMember]
        public string ValidazioneRegEx { get; set; }
        [DataMember]
        public bool Testata { get; set; }

        [DataMember]
        public List<OEPrestazione> PrestazioniAssociate { get; set; }

        public OEDatiAccessoriDescrittore()
        {
            InitBasicVars();
        }

        private void InitBasicVars()
        {
            Codice = "";
            Descrizione = "";
            Etichetta = "";

            Gruppo = "";
            Obbligatorio = false;
            Ordinamento = 0;
            Ripetibile = false;
            Tipo = "";
            Valori = "";

            ValidazioneMessaggio = "";
            ValidazioneRegEx = "";

            Testata = true;

            PrestazioniAssociate = new List<OEPrestazione>();
        }

    }

    [DataContract(Name = "OEDatoAccessorio")]
    public class OEDatoAccessorio
    {

        [DataMember]
        public string Codice { get; set; }

        [DataMember]
        public string Tipo { get; set; }
        [DataMember]
        public string Valore { get; set; }
        [DataMember]
        public int Ripetizione { get; set; }

        public OEDatoAccessorio()
        {
            InitBasicVars();
        }

        public OEDatoAccessorio(string s_codice, string s_valore, string s_tipo, int i_ripetizione)
        {
            InitBasicVars();

            Codice = s_codice;
            Tipo = s_tipo;
            Valore = s_valore;
            Ripetizione = i_ripetizione;
        }

        private void InitBasicVars()
        {
            Codice = "";
            Tipo = "";
            Valore = "";
            Ripetizione = 1;
        }

    }

    [DataContract(Name = "OEGruppoPrestazione")]
    public class OEGruppoPrestazione
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Descrizione { get; set; }
        [DataMember]
        public int NumeroPrestazioni { get; set; }
        [DataMember]
        public string SistemiEroganti { get; set; }

        public OEGruppoPrestazione()
        {
            InitBasicVars();
        }

        public OEGruppoPrestazione(string s_id, string s_descrizione, int i_numeroprestazioni, string s_sistemieroganti)
        {
            InitBasicVars();

            ID = s_id;
            Descrizione = s_descrizione;
            NumeroPrestazioni = i_numeroprestazioni;
            SistemiEroganti = s_sistemieroganti;
        }

        private void InitBasicVars()
        {
            ID = string.Empty;
            Descrizione = string.Empty;
            NumeroPrestazioni = 0;
            SistemiEroganti = string.Empty;
        }

    }

}