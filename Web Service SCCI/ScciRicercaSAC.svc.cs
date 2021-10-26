using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci;
using UnicodeSrl.Framework.Diagnostics;
using UnicodeSrl.Framework.Extension;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.Model;

namespace WsSCCI
{
    public class ScciRicercaSAC : IScciRicercaSAC
    {

        public List<PazienteSac> RicercaPazientiSAC(string cognome, string nome, DateTime datanascita, string luogonascita, string codfiscale)
        {

            List<PazienteSac> oListPazienteSac = new List<PazienteSac>();

            try
            {

                UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;


                string paramcognome = (cognome != null ? cognome + @"*" : string.Empty);
                string paramnome = (nome != null ? nome + @"*" : string.Empty);
                string paramdatanascita = (datanascita != null && datanascita != DateTime.MinValue ? datanascita.ToString("yyyy-MM-dd") : string.Empty);
                string paramluogonascita = (luogonascita != null ? luogonascita + @"*" : string.Empty);
                string paramcodfiscale = (codfiscale != null ? codfiscale : string.Empty);

                string cfgval = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACNumRecords);

                int nMaxRecord = 100;
                if (cfgval.IsNotNullOrEmpty()) nMaxRecord = Convert.ToInt32(cfgval);

                net.asmn.sac.Pazienti oSAC = null;
                oSAC = new net.asmn.sac.Pazienti();
                                oSAC.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSAC);

                                oSAC.UseDefaultCredentials = false;
                oSAC.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.SAC);

                net.asmn.sac.PazientiCercaResponsePazientiCercaResult paz = new net.asmn.sac.PazientiCercaResponsePazientiCercaResult();
                paz = oSAC.PazientiCerca(paramcognome, paramnome, paramdatanascita, paramluogonascita, paramcodfiscale, "", nMaxRecord);

                                if (paz != null && paz.PazientiCerca.Length > 0)
                {

                                        for (int i = 0; i < paz.PazientiCerca.Length; i++)
                    {

                        PazienteSac oPazienteSac = new PazienteSac();

                        oPazienteSac.CodSAC = paz.PazientiCerca[i].Id;
                        oPazienteSac.Cognome = paz.PazientiCerca[i].Cognome;
                        oPazienteSac.Nome = paz.PazientiCerca[i].Nome;
                        oPazienteSac.Paziente = paz.PazientiCerca[i].Cognome + " " + paz.PazientiCerca[i].Nome;

                        oPazienteSac.DataNascita = paz.PazientiCerca[i].DataNascita;
                        oPazienteSac.CodComuneNascita = paz.PazientiCerca[i].ComuneNascitaCodice;
                        oPazienteSac.ComuneNascita = paz.PazientiCerca[i].ComuneNascitaNome;

                        if (paz.PazientiCerca[i].NazionalitaNome != "") oPazienteSac.ComuneNascita += @" (" + UnicodeSrl.Scci.Statics.Database.testoSQL(paz.PazientiCerca[i].NazionalitaNome) + @")";

                        oPazienteSac.NascitaDescrizione = paz.PazientiCerca[i].DataNascita.ToString(@"dd/MM/yyyy") + " " + oPazienteSac.ComuneNascita.ToString();
                        oPazienteSac.LocalitaNascita = string.Empty;

                        oPazienteSac.Sesso = paz.PazientiCerca[i].Sesso;
                        oPazienteSac.CodiceFiscale = paz.PazientiCerca[i].CodiceFiscale;

                                                net.asmn.sac.PazientiDettaglioByIdResponsePazientiDettaglioByIdResult dett = new net.asmn.sac.PazientiDettaglioByIdResponsePazientiDettaglioByIdResult();
                        dett = oSAC.PazientiDettaglioById(paz.PazientiCerca[i].Id);

                        if (dett.PazientiDettaglio != null && dett.PazientiDettaglio.Length > 0)
                        {
                                                        oPazienteSac.CAPResidenza = dett.PazientiDettaglio[0].CapRes;
                            oPazienteSac.CodComuneResidenza = dett.PazientiDettaglio[0].ComuneResCodice;
                            oPazienteSac.ComuneResidenza = dett.PazientiDettaglio[0].ComuneResNome;
                            oPazienteSac.IndirizzoResidenza = dett.PazientiDettaglio[0].IndirizzoRes;
                            oPazienteSac.LocalitaResidenza = dett.PazientiDettaglio[0].LocalitaRes;
                            oPazienteSac.CodProvinciaResidenza = dett.PazientiDettaglio[0].ProvinciaResCodice;
                            oPazienteSac.ProvinciaResidenza = dett.PazientiDettaglio[0].ProvinciaResNome;
                            oPazienteSac.CodRegioneResidenza = dett.PazientiDettaglio[0].RegioneResCodice;
                            oPazienteSac.RegioneResidenza = dett.PazientiDettaglio[0].RegioneResNome;

                                                        oPazienteSac.CAPDomicilio = dett.PazientiDettaglio[0].CapDom;
                            oPazienteSac.CodComuneDomicilio = dett.PazientiDettaglio[0].ComuneDomCodice;
                            oPazienteSac.ComuneDomicilio = dett.PazientiDettaglio[0].ComuneDomNome;
                            oPazienteSac.IndirizzoDomicilio = dett.PazientiDettaglio[0].IndirizzoDom;
                            oPazienteSac.LocalitaDomicilio = dett.PazientiDettaglio[0].LocalitaDom;
                            oPazienteSac.CodProvinciaDomicilio = dett.PazientiDettaglio[0].ProvinciaDomCodice;
                            oPazienteSac.ProvinciaDomicilio = dett.PazientiDettaglio[0].ProvinciaDomNome;
                                                        

                                                        oPazienteSac.TerminazioneCodice = dett.PazientiDettaglio[0].CodiceTerminazione;
                            oPazienteSac.TerminazioneDescrizione = dett.PazientiDettaglio[0].DescrizioneTerminazione;
                            oPazienteSac.TerminazioneData = dett.PazientiDettaglio[0].DataTerminazioneAss;
                            if (oPazienteSac.TerminazioneCodice == "4" && dett.PazientiDettaglio[0].DataTerminazioneAssSpecified) oPazienteSac.DataDecesso = oPazienteSac.TerminazioneData;


                        }

                        dett = null;

                        oListPazienteSac.Add(oPazienteSac);

                    }

                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oListPazienteSac = new List<PazienteSac>();
            }

            return oListPazienteSac;

        }

        public PazienteSac RicercaPazientiSACByID(string idsac)
        {

            PazienteSac oPazienteSac = new PazienteSac();

            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;

            try
            {

                net.asmn.sac.Pazienti oSAC = null;
                oSAC = new net.asmn.sac.Pazienti();
                                oSAC.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSAC);
                                oSAC.UseDefaultCredentials = false;
                oSAC.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.SAC);

                                net.asmn.sac.PazientiCercaByIdResponsePazientiCercaByIdResult oPaz = new net.asmn.sac.PazientiCercaByIdResponsePazientiCercaByIdResult();
                oPaz = oSAC.PazientiCercaById(idsac);

                if (oPaz.Pazienti != null && oPaz.Pazienti.Length > 0)
                {

                    oPazienteSac.CodSAC = oPaz.Pazienti[0].Id;
                    oPazienteSac.Cognome = oPaz.Pazienti[0].Cognome;
                    oPazienteSac.Nome = oPaz.Pazienti[0].Nome;
                    oPazienteSac.Paziente = oPaz.Pazienti[0].Cognome + " " + oPaz.Pazienti[0].Nome;

                    oPazienteSac.DataNascita = oPaz.Pazienti[0].DataNascita;
                    oPazienteSac.CodComuneNascita = oPaz.Pazienti[0].ComuneNascitaCodice;
                    oPazienteSac.ComuneNascita = oPaz.Pazienti[0].ComuneNascitaNome;

                    oPazienteSac.NascitaDescrizione = oPaz.Pazienti[0].DataNascita.ToString(@"dd/MM/yyyy") + " " + oPazienteSac.ComuneNascita.ToString();
                    oPazienteSac.LocalitaNascita = string.Empty;

                    oPazienteSac.Sesso = oPaz.Pazienti[0].Sesso;
                    oPazienteSac.CodiceFiscale = oPaz.Pazienti[0].CodiceFiscale;

                                        oPazienteSac.TerminazioneCodice = oPaz.Pazienti[0].CodiceTerminazione;
                    oPazienteSac.TerminazioneDescrizione = oPaz.Pazienti[0].DescrizioneTerminazione;
                    oPazienteSac.TerminazioneData = oPaz.Pazienti[0].DataTerminazioneAss;
                    if (oPazienteSac.TerminazioneCodice == "4" && oPaz.Pazienti[0].DataTerminazioneAssSpecified) oPazienteSac.DataDecesso = oPazienteSac.TerminazioneData;

                    oPazienteSac.CAPResidenza = oPaz.Pazienti[0].CapRes;
                    oPazienteSac.CodComuneResidenza = oPaz.Pazienti[0].ComuneResCodice;
                    oPazienteSac.ComuneResidenza = oPaz.Pazienti[0].ComuneResNome;
                    oPazienteSac.IndirizzoResidenza = oPaz.Pazienti[0].IndirizzoRes;
                    oPazienteSac.LocalitaResidenza = oPaz.Pazienti[0].LocalitaRes;
                    oPazienteSac.CodProvinciaResidenza = oPaz.Pazienti[0].ProvinciaResCodice;
                    oPazienteSac.ProvinciaResidenza = oPaz.Pazienti[0].ProvinciaResNome;
                    oPazienteSac.CodRegioneResidenza = oPaz.Pazienti[0].RegioneResCodice;
                    oPazienteSac.RegioneResidenza = oPaz.Pazienti[0].RegioneResNome;

                    oPazienteSac.Nazionalita = oPaz.Pazienti[0].NazionalitaNome;
                    oPazienteSac.CodProvinciaNascita = oPaz.Pazienti[0].ProvinciaNascitaCodice;
                    oPazienteSac.ProvinciaNascita = oPaz.Pazienti[0].ProvinciaNascitaNome;

                    oPazienteSac.CAPDomicilio = oPaz.Pazienti[0].CapDom;
                    oPazienteSac.CodComuneDomicilio = oPaz.Pazienti[0].ComuneDomCodice;
                    oPazienteSac.ComuneDomicilio = oPaz.Pazienti[0].ComuneDomNome;
                    oPazienteSac.IndirizzoDomicilio = oPaz.Pazienti[0].IndirizzoDom;
                    oPazienteSac.LocalitaDomicilio = oPaz.Pazienti[0].LocalitaDom;
                    oPazienteSac.CodProvinciaDomicilio = oPaz.Pazienti[0].ProvinciaDomCodice;
                    oPazienteSac.ProvinciaDomicilio = oPaz.Pazienti[0].ProvinciaDomNome;


                }

                oPaz = null;

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oPazienteSac = new PazienteSac();
            }

            return oPazienteSac;

        }

        public PazienteSacDatiAggiuntivi PazienteSacDatiAggiuntiviByID(string idsac)
        {

            PazienteSacDatiAggiuntivi oPazienteSac = new PazienteSacDatiAggiuntivi();

            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;

            try
            {

                net.asmn.sac.Pazienti oSAC = null;
                oSAC = new net.asmn.sac.Pazienti();
                                oSAC.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSAC);
                                oSAC.UseDefaultCredentials = false;
                oSAC.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.SAC);

                                net.asmn.sac.PazientiDettaglio2ByIdResponsePazientiDettaglio2ByIdResult dett2 = new net.asmn.sac.PazientiDettaglio2ByIdResponsePazientiDettaglio2ByIdResult();
                dett2 = oSAC.PazientiDettaglio2ById(idsac);

                if (dett2.PazientiDettaglio2 != null && dett2.PazientiDettaglio2.Length > 0)
                {

                                        oPazienteSac.Telefono1 = dett2.PazientiDettaglio2[0].Telefono1;
                    oPazienteSac.Telefono2 = dett2.PazientiDettaglio2[0].Telefono2;
                    oPazienteSac.Telefono3 = dett2.PazientiDettaglio2[0].Telefono3;
                    oPazienteSac.CodiceMedicoDiBase = dett2.PazientiDettaglio2[0].CodiceMedicoDiBase;
                    oPazienteSac.CodiceFiscaleMedicoDiBase = dett2.PazientiDettaglio2[0].CodiceFiscaleMedicoDiBase;
                    oPazienteSac.CognomeNomeMedicoDiBase = dett2.PazientiDettaglio2[0].CognomeNomeMedicoDiBase;
                    oPazienteSac.DistrettoMedicoDiBase = dett2.PazientiDettaglio2[0].DistrettoMedicoDiBase;
                    oPazienteSac.DataSceltaMedicoDiBase = dett2.PazientiDettaglio2[0].DataSceltaMedicoDiBase;

                                        if (dett2.PazientiDettaglio2[0].PazientiDettaglioEsenzioni != null && dett2.PazientiDettaglio2[0].PazientiDettaglioEsenzioni.Length > 0)
                    {

                                                for (int i = 0; i < dett2.PazientiDettaglio2[0].PazientiDettaglioEsenzioni.Length; i++)
                        {

                            PazienteSacEsenzioni oEsenzione = new PazienteSacEsenzioni();

                            oEsenzione.CodiceEsenzione = dett2.PazientiDettaglio2[0].PazientiDettaglioEsenzioni[i].CodiceEsenzione;
                            oEsenzione.TestoEsenzione = dett2.PazientiDettaglio2[0].PazientiDettaglioEsenzioni[i].TestoEsenzione;
                            oEsenzione.DataInizioValidita = dett2.PazientiDettaglio2[0].PazientiDettaglioEsenzioni[i].DataInizioValidita;
                            oEsenzione.DataFineValidita = dett2.PazientiDettaglio2[0].PazientiDettaglioEsenzioni[i].DataFineValidita;
                            oEsenzione.CodiceDiagnosi = dett2.PazientiDettaglio2[0].PazientiDettaglioEsenzioni[i].CodiceDiagnosi;
                            oEsenzione.DecodificaEsenzioneDiagnosi = dett2.PazientiDettaglio2[0].PazientiDettaglioEsenzioni[i].DecodificaEsenzioneDiagnosi;

                            oPazienteSac.Esenzioni.Add(oEsenzione);

                        }

                    }

                                        if (dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi != null && dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi.Length > 0)
                    {

                                                for (int i = 0; i < dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi.Length; i++)
                        {

                            PazienteSacConsensi oConsensi = new PazienteSacConsensi();

                            oConsensi.Provenienza = dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi[i].Provenienza;
                            oConsensi.IDProvenienza = dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi[i].IdProvenienza;
                            oConsensi.Tipo = dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi[i].Tipo;
                            oConsensi.DataStato = dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi[i].DataStato;
                            oConsensi.Stato = dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi[i].Stato;
                            oConsensi.OperatoreId = dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi[i].OperatoreId;
                            oConsensi.OperatoreCognome = dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi[i].OperatoreCognome;
                            oConsensi.OperatoreNome = dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi[i].OperatoreNome;
                            oConsensi.OperatoreComputer = dett2.PazientiDettaglio2[0].PazientiDettaglioConsensi[i].OperatoreComputer;

                            oPazienteSac.Consensi.Add(oConsensi);

                        }

                    }

                }

                dett2 = null;

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                oPazienteSac = new PazienteSacDatiAggiuntivi();
            }

            return oPazienteSac;

        }

    }

}
