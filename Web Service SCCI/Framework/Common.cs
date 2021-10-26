using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci;

namespace WsSCCI
{
    public static class Common
    {

                public const string GC_TBSTATI = "Stati";
        public const string GC_TBTIPI = "Tipi";

        public static string ConnString
        {
            get
            {
                UnicodeSrl.Scci.Encryption ocrypt = new UnicodeSrl.Scci.Encryption(UnicodeSrl.Scci.Statics.EncryptionStatics.GC_DECRYPTKEY, UnicodeSrl.Scci.Statics.EncryptionStatics.GC_INITVECTOR);
                string sreturn = ocrypt.DecryptString(System.Web.Configuration.WebConfigurationManager.AppSettings["ConnStr"].ToString());
                ocrypt = null;
                return sreturn;
            }
        }

        public static string DiagnosticsWebServiceUrl
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["DiagnosticUrl"].ToString();
            } 
        }

                                                        public static Gestore getGestore(ScciAmbiente amb, MovScheda movScheda)
        {
            Gestore oGestore = new Gestore();
                        oGestore.Valutatore.Fillers.Add("FillerSistema", new EvaluatorFillerSistema());
            oGestore.Valutatore.Fillers.Add("FillerScci", new EvaluatorFillerScci());
            oGestore.Valutatore.Fillers.Add("FillerScheda", new EvaluatorFillerScheda());
            oGestore.Valutatore.Fillers.Add("FillerAltraScheda", new EvaluatorFillerAltraScheda());
            oGestore.Valutatore.Fillers.Add("FillerDLookUp", new EvaluatorFillerDLookUp());
            oGestore.Valutatore.Fillers.Add("FillerGeneric", new UnicodeSrl.Evaluator.EvaluatorFillerGeneric());
                        oGestore.Contesto = new Dictionary<string, object>();
            oGestore.Contesto = (from x in amb.Contesto
                                 where (x.Value != null &&
                                        x.Value.GetType() != typeof(UnicodeSrl.DatiClinici.DC.DcSchedaDati) &&
                                        x.Value.GetType() != typeof(string))
                                 select x).ToDictionary(x => x.Key, x => x.Value);

                        if (movScheda != null)
            {
                oGestore.SchedaXML = movScheda.Scheda.StrutturaXML;
                oGestore.SchedaLayoutsXML = movScheda.Scheda.LayoutXML;
                oGestore.SchedaDatiXML = movScheda.DatiXML;
                oGestore.Decodifiche = movScheda.Scheda.DizionarioValori();
            }

            return oGestore;
        }

    }
}