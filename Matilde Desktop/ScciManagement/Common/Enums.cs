using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnicodeSrl.ScciManagement
{
    public static class Enums
    {

        public enum EnumImageSize
        {
            isz16 = 0,
            isz32 = 1,
            isz48 = 2,
            isz256 = 3
        }

        public enum EnumModalityPopUp
        {
            mpNuovo = 0,
            mpModifica = 1,
            mpCancella = 2,
            mpVisualizza = 3,
            mpCopia = 4,
            mpEsporta = 5,
            mpImporta = 6
        }

        public enum EnumDataNames
        {
            Nessuno = -1,
            T_Login = 0,
            T_Moduli = 1,
            T_Ruoli = 2,
            T_Aziende = 3,
            T_UnitaAtomiche = 4,
            T_DiarioMedico = 5,
            T_DiarioInfermieristico = 6,
            T_TipoParametroVitale = 7,
            T_TipoEvidenzaClinica = 8,
            T_Report = 9,
            T_Maschere = 10,
            T_FormatoReport = 11,
            T_TestiPredefiniti = 12,
            T_TipoPrescrizione = 13,
            T_ViaSomministrazione = 14,
            T_TipoAllegato = 15,
            T_MovNews = 16,
            T_UnitaOperative = 17,
            T_StatoEvidenzaClinica = 18,
            T_TipoAppuntamento = 19,
            T_StatoAppuntamento = 20,
            T_TipoTaskInfermieristico = 21,
            T_StatoTaskInfermieristico = 22,
            T_Stanze = 23,
            T_Schede = 24,
            T_SchedeVersioni = 25,
            T_StatoPrescrizione = 26,
            T_TipoEpisodio = 27,
            T_StatoTrasferimento = 28,
            T_TipoDiario = 29,
            T_Letti = 30,
            T_Settori = 31,
            T_TipoScheda = 32,
            T_DCDecodifiche = 33,
            T_DCDecodificheValori = 34,
            T_Agende = 35,
            T_TipoAgenda = 36,
            T_AssUAUOLetti = 37,
            T_StatoDiario = 38,
            T_StatoEvidenzaClinicaVisione = 39,
            T_TipoAlertAllergiaAnamnesi = 40,
            T_TipoAlertGenerico = 41,
            T_StatoAlertGenerico = 42,
            T_StatoAlertAllergiaAnamnesi = 43,
            T_StatoParametroVitale = 44,
            T_StatoScheda = 45,
            T_Sistemi = 46,
            T_FormatoAllegati = 47,
            T_TipoOrdine = 48,
            T_StatoOrdine = 49,
            T_StatoPrescrizioneTempi = 50,
            T_StatoAppuntamentoAgende = 51,
            T_StatoAllegato = 52,
            T_StatoCartella = 53,
            T_StatoEpisodio = 54,
            T_Protocolli = 55,
            T_ProtocolliTempi = 56,
            T_SezioniFUT = 57,
            T_StatoContinuazione = 58,
            T_CDSSAzioni = 59,
            T_CDSSPlugins = 60,
            T_CDSSStruttura = 61,
            T_StatoCartellaInVisione = 62,
            T_EBM = 63,
            T_ConfigPC = 64,
            T_Integra_Destinatari = 65,
            T_Integra_Campi = 66,
            T_Contatori = 67,
            T_FiltriSpeciali = 68,
            T_ProtocolliAttivita = 69,
            T_ProtocolliAttivitaTempi = 70,
            T_ProtocolliAttivitaTempiTipoTask = 71,
            T_CDSSStrutturaRuoli = 72,
            T_Festivita = 73,
            T_TipoSelezione = 74,
            T_Selezioni = 75,
            T_TipoFiltroSpeciale = 76,
            T_AgendePeriodi = 77,
            T_StatoSchedaCalcolato = 78,
            T_ModalitaCopiaPrecedente = 79,
            T_Screen = 80,
            T_ScreenTile = 81,
            T_ProtocolliPrescrizioni = 82,
            T_EntitaAllegato = 83,
            T_AssUAIntestazioni = 84,
            T_TipoConsegna = 85,
            T_StatoConsegna = 86,
            T_OEFormule = 87,
            T_OEAttributi = 88,
            T_FestivitaAgende = 89,
            T_TestiNotePredefiniti = 90,
            T_StatoCartellaInfo = 91,
            T_TipoConsegnaPaziente = 92,
            T_StatoConsegnaPaziente = 93,
            T_StatoConsegnaPazienteRuoli = 94,
            T_StatoConsensoCalcolato = 95

        }

        public enum EnumCheckTVSelezione
        {
            tutti = 0,
            selezionati = 1,
            non_selezionati = 2
        }

        public enum EnumEntitaLog
        {
            Nessuna = 0,

            T_Schede = 1,
            T_SchedePadri = 2,
            T_SchedeCopia = 3,
            T_AssUAEntita = 4,
            T_AssRuoliAzioni = 5,

            T_SchedeVersioni = 6,
            T_TipoEvidenzaClinica = 7,

            T_ConfigCE = 8,

            T_MovTrasferimenti = 9

        }

        public static string GetEnumDescription(Enum value)
        {

            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();

        }

    }
}
