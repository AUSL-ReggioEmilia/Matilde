using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Scci.DataContracts;

namespace UnicodeSrl.Scci.Plugin
{
    public class MbrDC :
        MarshalByRefObject
    {

        public Gestore GetGestore(ScciAmbiente ambiente)
        {

            try
            {

                Gestore oGestore = new Gestore();

                oGestore.Valutatore.Fillers.Add("FillerSistema", new EvaluatorFillerSistema());
                oGestore.Valutatore.Fillers.Add("FillerScci", new EvaluatorFillerScci());
                oGestore.Valutatore.Fillers.Add("FillerScheda", new EvaluatorFillerScheda());
                oGestore.Valutatore.Fillers.Add("FillerAltraScheda", new EvaluatorFillerAltraScheda());
                oGestore.Valutatore.Fillers.Add("FillerDLookUp", new EvaluatorFillerDLookUp());
                oGestore.Valutatore.Fillers.Add("FillerGeneric", new UnicodeSrl.Evaluator.EvaluatorFillerGeneric());

                oGestore.Contesto = new Dictionary<string, object>();
                oGestore.Contesto = (from x in ambiente.Contesto
                                     where (x.Value != null && x.Value.GetType() != typeof(UnicodeSrl.DatiClinici.DC.DcSchedaDati))
                                     select x).ToDictionary(x => x.Key, x => x.Value);

                return oGestore;

            }
            catch (Exception)
            {
                return null;
            }

        }

    }
}
