using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.Scci.DWH
{
    public static class DWHXmlCreator
    {
        public static Tuple<string, string> generaXMLCartella(MSP_SelExportDWHCartella cartella, string path, bool chiusa, int azione)
        {


            XmlSerializer ser = new XmlSerializer(typeof(MessaggioRefertoType));

            MessaggioRefertoType messaggio = new MessaggioRefertoType();

            RefertoType referto = new RefertoType();
            PazienteType paziente = new PazienteType();
            GeneralitaType generalita = new GeneralitaType();
            CodiceDescrizioneType repartoErogante = new CodiceDescrizioneType();

            AllegatoType allegatoPDF = new AllegatoType();
            AllegatoType[] allegati = new AllegatoType[1];

            Stream xmlStreamMessage = new MemoryStream();

            Tuple<string, string> result;
            try
            {
                messaggio.Azione = azione;
                messaggio.DataSequenza = DateTime.Now;

                referto.IdEsterno = cartella.IDCartella.ToString();
                referto.AziendaErogante = cartella.AziendaErogante;
                referto.SistemaErogante = cartella.SistemaErogante;
                repartoErogante.Codice = cartella.RepartoEroganteCodice;
                repartoErogante.Descrizione = cartella.RepartoEroganteDescrizione;
                referto.RepartoErogante = repartoErogante;

                referto.Attributi = new AttributoType[1];
                referto.Attributi[0] = new AttributoType() { Nome = @"BarcodeCup2000", Valore = cartella.BarcodeCartella };

                DateTime dataNascita = DateTime.MinValue;
                if (cartella.PazienteDataNascita.HasValue) dataNascita = cartella.PazienteDataNascita.Value;

                paziente.IdEsterno = cartella.PazienteIDEsterno;
                paziente.CodiceAnagraficaErogante = cartella.PazienteIDEsterno;
                paziente.NomeAnagraficaErogante = "SAC";

                generalita.Cognome = cartella.PazienteCognome;
                generalita.Nome = cartella.PazienteNome;
                generalita.DataNascita = dataNascita;
                generalita.DataNascitaSpecified = true;
                generalita.Sesso = cartella.PazienteSesso;
                generalita.CodiceFiscale = cartella.PazienteCodiceFiscale;
                paziente.Generalita = generalita;

                referto.Paziente = paziente;
                if (chiusa == true) referto.StatoRichiesta = StatoRichiestaEnum.Item1;
                else if (chiusa == false) referto.StatoRichiesta = StatoRichiestaEnum.Item0;

                referto.DataReferto = cartella.DataSequenza;
                referto.NumeroReferto = cartella.NumeroCartella;
                referto.NumeroNosologico = cartella.NumeroNosologico;

                allegatoPDF.IdEsterno = cartella.IDCartella.ToString();
                allegatoPDF.NomeFile = cartella.IDCartella.ToString();
                allegatoPDF.DataFile = cartella.DataSequenza;
                allegatoPDF.MimeType = "application/pdf";
                allegatoPDF.MimeData = cartella.PDFCartella.Value;

                allegati[0] = allegatoPDF;
                referto.Allegati = allegati;

                messaggio.Referto = referto;

                ser.Serialize(xmlStreamMessage, messaggio);

                xmlStreamMessage.Position = 0;
                var sr = new StreamReader(xmlStreamMessage);

                string xmlMessage = sr.ReadToEnd();

                result = new Tuple<string, string>(inviaCartella(xmlMessage, Path.Combine(path, cartella.IDCartella.ToString() + @".xml")), xmlMessage);


            }
            catch (Exception ex)
            {
                throw new Exception("Errore in generaXMLCartella", ex);
            }

            return result;

        }

        public static Tuple<string, string> generaXMLCartella(string codiceFiscale, string numeroCartella, DateTime dataSequenza, string aziendaErogante, string sistemaErogante, string repEroganteCod,
                                             string repEroganteDesc, string idCartella, string idSac, string cognome, string nome, string sesso,
                                             DateTime dataNascita, string nosologico, string nomePDF, byte[] base64, string path, bool cartellaChiusa, int azione)
        {


            XmlSerializer ser = new XmlSerializer(typeof(MessaggioRefertoType));

            MessaggioRefertoType messaggio = new MessaggioRefertoType();

            RefertoType referto = new RefertoType();
            PazienteType paziente = new PazienteType();
            GeneralitaType generalita = new GeneralitaType();
            CodiceDescrizioneType repartoErogante = new CodiceDescrizioneType();

            AllegatoType allegatoPDF = new AllegatoType();
            AllegatoType[] allegati = new AllegatoType[1];

            Stream xmlStreamMessage = new MemoryStream();

            Tuple<string, string> result;
            try
            {
                messaggio.Azione = azione;
                messaggio.DataSequenza = DateTime.Now;


                referto.IdEsterno = idCartella;

                referto.AziendaErogante = aziendaErogante;
                referto.SistemaErogante = sistemaErogante;

                repartoErogante.Codice = repEroganteCod;
                repartoErogante.Descrizione = repEroganteDesc;
                referto.RepartoErogante = repartoErogante;

                paziente.IdEsterno = idSac;
                paziente.CodiceAnagraficaErogante = idSac;
                paziente.NomeAnagraficaErogante = "SAC";

                generalita.Cognome = cognome;
                generalita.Nome = nome;
                generalita.DataNascita = dataNascita;
                generalita.DataNascitaSpecified = true;
                generalita.Sesso = sesso;
                generalita.CodiceFiscale = codiceFiscale;
                paziente.Generalita = generalita;
                referto.Paziente = paziente;
                if (cartellaChiusa == true)
                    referto.StatoRichiesta = StatoRichiestaEnum.Item1;
                if (cartellaChiusa == false)
                    referto.StatoRichiesta = StatoRichiestaEnum.Item0;

                referto.DataReferto = dataSequenza;
                referto.NumeroReferto = numeroCartella;
                referto.NumeroNosologico = nosologico;

                allegatoPDF.IdEsterno = nomePDF;
                allegatoPDF.NomeFile = nomePDF;
                allegatoPDF.DataFile = dataSequenza;
                allegatoPDF.MimeType = "application/pdf";
                if (base64 != null)
                    allegatoPDF.MimeData = base64;



                allegati[0] = allegatoPDF;
                referto.Allegati = allegati;

                messaggio.Referto = referto;

                ser.Serialize(xmlStreamMessage, messaggio);

                xmlStreamMessage.Position = 0;
                var sr = new StreamReader(xmlStreamMessage);

                string xmlMessage = sr.ReadToEnd();

                result = new Tuple<string, string>(inviaCartella(xmlMessage, Path.Combine(path, idCartella + @".xml")), xmlMessage);


            }
            catch (Exception ex)
            {
                result = new Tuple<string, string>(ex.ToString(), "ERRORE NELLA CREAZIONE DEL FILE XML");
            }

            return result;

        }

        public static Tuple<string, string> generaXMLScheda(string codiceFiscale,
                                                            string idesterno,
                                                            DateTime dataSequenza,
                                                            string aziendaErogante,
                                                            string sistemaErogante,
                                                            string repEroganteCod,
                                                            string repEroganteDesc,
                                                            string idSac,
                                                            string cognome,
                                                            string nome,
                                                            string sesso,
                                                            DateTime dataNascita,
                                                            int statorichiesta,
                                                            DateTime datareferto,
                                                            string numeroreferto,
                                                            string nosologico,
                                                            string medicorefertantecodice,
                                                            string medicorefertantedescrizione,
                                                            string CodPrestazioneDWH,
                                                            string DescrizionePrestazioneDWH,
                                                            byte[] base64,
                                                            string path,
                                                            int azione)
        {
            return generaXMLScheda(codiceFiscale,
                                    idesterno,
                                    dataSequenza,
                                    aziendaErogante,
                                    sistemaErogante,
                                    repEroganteCod,
                                    repEroganteDesc,
                                    idSac,
                                    cognome,
                                    nome,
                                    sesso,
                                    dataNascita,
                                    statorichiesta,
                                    datareferto,
                                    numeroreferto,
                                    nosologico,
                                    medicorefertantecodice,
                                    medicorefertantedescrizione,
                                    CodPrestazioneDWH,
                                    DescrizionePrestazioneDWH,
                                    base64,
                                    path,
                                    azione,
                                    "",
                                    new List<KeyValuePair<string, string>>());
        }
        public static Tuple<string, string> generaXMLScheda(string codiceFiscale,
                                                            string idesterno,
                                                            DateTime dataSequenza,
                                                            string aziendaErogante,
                                                            string sistemaErogante,
                                                            string repEroganteCod,
                                                            string repEroganteDesc,
                                                            string idSac,
                                                            string cognome,
                                                            string nome,
                                                            string sesso,
                                                            DateTime dataNascita,
                                                            int statorichiesta,
                                                            DateTime datareferto,
                                                            string numeroreferto,
                                                            string nosologico,
                                                            string medicorefertantecodice,
                                                            string medicorefertantedescrizione,
                                                            string CodPrestazioneDWH,
                                                            string DescrizionePrestazioneDWH,
                                                            byte[] base64,
                                                            string path,
                                                            int azione,
                                                            string AnteprimaDWH,
                                                            List<KeyValuePair<string, string>> lst_attributiLayerDWH)
        {


            XmlSerializer ser = new XmlSerializer(typeof(MessaggioRefertoType));

            MessaggioRefertoType messaggio = new MessaggioRefertoType();

            RefertoType referto = new RefertoType();
            PazienteType paziente = new PazienteType();
            GeneralitaType generalita = new GeneralitaType();
            CodiceDescrizioneType repartoErogante = new CodiceDescrizioneType();

            AllegatoType allegatoPDF = new AllegatoType();
            AllegatoType[] allegati = new AllegatoType[1];

            PrestazioneType prestazione = new PrestazioneType();
            PrestazioneType[] prestazioni = new PrestazioneType[1];

            Stream xmlStreamMessage = new MemoryStream();

            Tuple<string, string> result;
            try
            {
                messaggio.Azione = azione;
                messaggio.DataSequenza = DateTime.Now;


                referto.IdEsterno = idesterno;

                referto.AziendaErogante = aziendaErogante;
                referto.SistemaErogante = sistemaErogante;

                repartoErogante.Codice = repEroganteCod;
                repartoErogante.Descrizione = repEroganteDesc;
                referto.RepartoErogante = repartoErogante;

                paziente.IdEsterno = idSac;
                paziente.CodiceAnagraficaErogante = idSac;
                paziente.NomeAnagraficaErogante = "SAC";

                generalita.Cognome = cognome;
                generalita.Nome = nome;
                generalita.DataNascita = dataNascita;
                generalita.DataNascitaSpecified = true;
                generalita.Sesso = sesso;
                generalita.CodiceFiscale = codiceFiscale;
                paziente.Generalita = generalita;
                referto.Paziente = paziente;

                if (statorichiesta == 0)
                {
                    referto.StatoRichiesta = StatoRichiestaEnum.Item0;
                }
                else if (statorichiesta == 1)
                {
                    referto.StatoRichiesta = StatoRichiestaEnum.Item1;
                }
                else if (statorichiesta == 2)
                {
                    referto.StatoRichiesta = StatoRichiestaEnum.Item2;
                }
                else if (statorichiesta == 3)
                {
                    referto.StatoRichiesta = StatoRichiestaEnum.Item3;
                }

                referto.DataReferto = dataSequenza;
                referto.NumeroReferto = numeroreferto;
                referto.NumeroNosologico = nosologico;
                referto.MedicoRefertante = new CodiceDescrizioneType();
                referto.MedicoRefertante.Codice = medicorefertantecodice;
                referto.MedicoRefertante.Descrizione = medicorefertantedescrizione;

                allegatoPDF.IdEsterno = idesterno;
                allegatoPDF.NomeFile = idesterno;
                allegatoPDF.DataFile = dataSequenza;
                allegatoPDF.MimeType = "application/pdf";
                if (base64 != null)
                    allegatoPDF.MimeData = base64;



                allegati[0] = allegatoPDF;
                referto.Allegati = allegati;

                if (CodPrestazioneDWH != string.Empty && DescrizionePrestazioneDWH != string.Empty)
                {

                    prestazione.IdEsterno = idesterno;
                    prestazione.Prestazione = new CodiceDescrizioneType();
                    prestazione.Prestazione.Codice = CodPrestazioneDWH;
                    prestazione.Prestazione.Descrizione = DescrizionePrestazioneDWH;

                    prestazioni[0] = prestazione;
                    referto.Prestazioni = prestazioni;

                }

                List<AttributoType> lst_AttributoType = new List<AttributoType>();

                if (AnteprimaDWH != string.Empty)
                {
                    lst_AttributoType.Add(new AttributoType() { Nome = @"Anteprima", Valore = AnteprimaDWH });
                }

                if (lst_attributiLayerDWH.Count > 0)
                {
                    foreach (KeyValuePair<string, string> kvp in lst_attributiLayerDWH)
                    {
                        lst_AttributoType.Add(new AttributoType() { Nome = kvp.Key, Valore = kvp.Value });
                    }
                }
                if (lst_AttributoType.Count > 0)
                {
                    referto.Attributi = lst_AttributoType.ToArray();
                }

                messaggio.Referto = referto;

                ser.Serialize(xmlStreamMessage, messaggio);

                xmlStreamMessage.Position = 0;
                var sr = new StreamReader(xmlStreamMessage);

                string xmlMessage = sr.ReadToEnd();

                result = new Tuple<string, string>(inviaCartella(xmlMessage, Path.Combine(path, idesterno + @".xml")), xmlMessage);


            }
            catch (Exception ex)
            {
                result = new Tuple<string, string>(ex.ToString(), "ERRORE NELLA CREAZIONE DEL FILE XML");
            }

            return result;

        }

        public static string inviaCartella(string xml, string indirizzo)
        {
            string result = string.Empty;
            try
            {

                StreamWriter fileReferto = new StreamWriter(indirizzo);

                fileReferto.Write(xml);
                fileReferto.Close();
                result = indirizzo;
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }

            return result;

        }

    }
}
