using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using UnicodeSrl.Framework.Diagnostics;

namespace WsSCCI
{
            public class ScciConsensiSAC : IScciConsensiSAC
    {
        public string AggiungiConsenso(string idsac,
                                        string pazientecognome, string pazientenome, string pazientecodicefiscale,
                                        string operatoreid, string operatorecognome, string operatorenome, string operatorecomputer,
                                        string tipoconsenso )
        {

            string sret = string.Empty;

            UnicodeSrl.Scci.Statics.Database.ConnectionString = Common.ConnString;

            try
            {

                net.asmn.sacconsensi.Consensi oSAC = null;
                oSAC = new net.asmn.sacconsensi.Consensi();
                                oSAC.Url = UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSACConsensi);
                                oSAC.UseDefaultCredentials = false;
                oSAC.Credentials = Credentials.CredenzialiScci(Credentials.enumTipoCredenziali.SAC);

                                                                net.asmn.sacconsensi.Consenso oConsenso = new net.asmn.sacconsensi.Consenso();
                oConsenso.OperatoreId = operatoreid;
                oConsenso.OperatoreCognome = operatorecognome;
                oConsenso.OperatoreNome = operatorenome;
                oConsenso.OperatoreComputer = operatorecomputer;

                oConsenso.PazienteProvenienza = "SAC";
                oConsenso.PazienteIdProvenienza = idsac;
                oConsenso.PazienteCodiceFiscale = pazientecodicefiscale;
                oConsenso.PazienteCognome = pazientecognome;
                oConsenso.PazienteNome = pazientenome;

                switch (tipoconsenso)
                {
                    case "Generico":
                        oConsenso.Tipo = net.asmn.sacconsensi.TipoConsenso.Generico;
                        break;
                    case "Dossier":
                        oConsenso.Tipo = net.asmn.sacconsensi.TipoConsenso.Dossier;
                        break;
                    case "DossierStorico":
                        oConsenso.Tipo = net.asmn.sacconsensi.TipoConsenso.DossierStorico;
                        break;
                }
                oConsenso.DataStato = DateTime.Now;
                oConsenso.Stato = true;

              

                                                                net.asmn.sacconsensi.ConsensiAggiungiResponseConsensi[] oConsensiResponse = oSAC.ConsensiAggiungi(oConsenso);
                if (oConsensiResponse != null && oConsensiResponse.Length > 0)
                {

                }
                else
                {
                    sret = @"Consenso NON inserito!";
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                sret = ex.Message;
            }

                        return sret;

        }


    }
}
