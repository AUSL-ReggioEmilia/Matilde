using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnicodeSrl.Evaluator;
using UnicodeSrl.DatiClinici.DC;
using System.Reflection;
using System.Data;

using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.DatiClinici.Common;
using UnicodeSrl.Scci.RTF;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Framework.Utility;
using UnicodeSrl.Scci.DataContracts;

namespace UnicodeSrl.Scci
{

    public class EvaluatorFillerSistema : BaseFiller
    {
        public EvaluatorFillerSistema(bool solalettura = false)
        {
            this.SolaLettura = solalettura;
        }

        public bool SolaLettura { get; set; }

        public override string Process(ParseResult o_parseresult, Dictionary<string, object> dict_context)
        {

            string sReturn = string.Empty;

            try
            {

                DateTime dt = DateTime.Now;
                switch (o_parseresult.Ids[0].ToUpper())
                {

                    case "SISTEMA":
                        switch (o_parseresult.Ids[1].ToUpper())
                        {

                            case "DATA":
                                sReturn = string.Format("{0:00}/{1:00}/{2:0000}", dt.Day, dt.Month, dt.Year);
                                break;

                            case "DATAORA":
                                sReturn = string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute);
                                break;

                            case "ORA":
                                sReturn = string.Format("{0:00}:{1:00}", dt.Hour, dt.Minute);
                                break;

                            case "NOMEPC":
                                if (dict_context.ContainsKey("Ambiente") == true)
                                {
                                    Type type = dict_context["Ambiente"].GetType();
                                    PropertyInfo info = type.GetProperty("Nomepc");
                                    if (info == null) { return ""; }

                                    object obj = info.GetValue(dict_context["Ambiente"], null);

                                    sReturn = obj.ToString();
                                }

                                break;

                            case "DESCRIZIONELOGIN":
                                if (dict_context.ContainsKey("Ambiente") == true)
                                {
                                    Type type = dict_context["Ambiente"].GetType();
                                    PropertyInfo info = type.GetProperty("Codlogin");
                                    if (info == null) { return ""; }

                                    object obj = info.GetValue(dict_context["Ambiente"], null);

                                    sReturn = CercaLogin(obj.ToString(), (Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"], "Descrizione");

                                }
                                break;

                            case "CODICEFISCALELOGIN":
                                if (dict_context.ContainsKey("Ambiente") == true)
                                {
                                    Type type = dict_context["Ambiente"].GetType();
                                    PropertyInfo info = type.GetProperty("Codlogin");
                                    if (info == null) { return ""; }

                                    object obj = info.GetValue(dict_context["Ambiente"], null);

                                    sReturn = CercaLogin(obj.ToString(), (Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"], "CodiceFiscale");

                                }
                                break;

                            case "CONTATORE":
                                sReturn = CercaContatore(o_parseresult.Result[o_parseresult.Ids[1]][0]);
                                break;

                        }

                        break;

                }

            }
            catch (Exception)
            {
            }

            return sReturn;

        }

        private string CercaLogin(string codlogin, Scci.DataContracts.ScciAmbiente ambiente, string parametro)
        {

            string sReturn = string.Empty;

            try
            {

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                spcoll[0] = new SqlParameterExt("sCodLogin", codlogin, ParameterDirection.Input, SqlDbType.VarChar);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelLogin", spcoll);

                if (dt.Rows.Count != 0)
                {
                    sReturn = dt.Rows[0][parametro].ToString();
                }

            }
            catch (Exception)
            {
                return sReturn;
            }

            return sReturn;

        }

        private string CercaContatore(string contatore)
        {

            string sReturn = "0";

            try
            {

                T_Contatori oContatori = new T_Contatori();
                oContatori.Codice = contatore;
                if (oContatori.TrySelect())
                {
                    oContatori.Valore += 1;
                    if (this.SolaLettura == false)
                    {
                        oContatori.Update();
                    }
                    sReturn = oContatori.Valore.ToString();
                }

                oContatori = null;

            }
            catch (Exception)
            {
                return sReturn;
            }

            return sReturn;

        }

    }

    public class EvaluatorFillerScci : BaseFiller
    {

        public override string Process(ParseResult o_parseresult, Dictionary<string, object> dict_context)
        {

            string sReturn = string.Empty;

            try
            {
                string ctxObjectKey = o_parseresult.Ids[0].ToUpper();

                object ctxObject = dict_context.FirstOrDefault(x => x.Key.ToUpper() == ctxObjectKey).Value;

                if (ctxObject == null) return null;

                Type ctxType = ctxObject.GetType();

                string propName = o_parseresult.Ids[1];
                PropertyInfo pinfo = ReflectionHelper.GetProperty(ctxType, propName);

                Object objValue = null;
                if (pinfo != null) objValue = pinfo.GetValue(ctxObject);

                switch (ctxObjectKey)
                {
                    case "PAZIENTE":

                        switch (propName.ToUpper())
                        {
                            case "DATANASCITA":
                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = (DateTime)objValue;
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000}", dt.Day, dt.Month, dt.Year);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            default:
                                sReturn = Convert.ToString(objValue);
                                break;

                        }

                        break;

                    case "EPISODIO":

                        switch (propName.ToUpper())
                        {
                            case "DATADIMISSIONE":
                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000}", dt.Day, dt.Month, dt.Year);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            case "DATAORADIMISSIONE":
                                pinfo = ctxType.GetProperty("DataDimissione");
                                objValue = pinfo.GetValue(ctxObject);

                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}:{5:00}", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute, dt.Second);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            case "DATARICOVERO":
                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000}", dt.Day, dt.Month, dt.Year);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            case "DATAORARICOVERO":
                                pinfo = ctxType.GetProperty("DataRicovero");
                                objValue = pinfo.GetValue(ctxObject);
                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}:{5:00}", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute, dt.Second);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            default:

                                sReturn = Convert.ToString(objValue);
                                break;

                        }

                        break;

                    case "TRASFERIMENTO":
                        switch (propName.ToUpper())
                        {
                            case "DATAORAINGRESSO":
                                pinfo = ctxType.GetProperty("DataIngresso");
                                objValue = pinfo.GetValue(ctxObject);

                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}:{5:00}", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute, dt.Second);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            case "DATAINGRESSO":
                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000}", dt.Day, dt.Month, dt.Year);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            case "DATAORAUSCITA":
                                pinfo = ctxType.GetProperty("DataUscita");
                                objValue = pinfo.GetValue(ctxObject);

                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}:{5:00}", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute, dt.Second);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            case "DATAUSCITA":
                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000}", dt.Day, dt.Month, dt.Year);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            default:
                                sReturn = Convert.ToString(objValue);
                                break;

                        }

                        break;

                    case "CARTELLA":

                        switch (propName.ToUpper())
                        {

                            case "DATAAPERTURA":
                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000}", dt.Day, dt.Month, dt.Year);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            case "DATAORAAPERTURA":

                                pinfo = ctxType.GetProperty("DataApertura");
                                objValue = pinfo.GetValue(ctxObject);

                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}:{5:00}", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute, dt.Second);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            case "DATACHIUSURA":
                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000}", dt.Day, dt.Month, dt.Year);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            case "DATAORACHIUSURA":

                                pinfo = ctxType.GetProperty("DataChiusura");
                                objValue = pinfo.GetValue(ctxObject);

                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}:{5:00}", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute, dt.Second);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            default:
                                sReturn = Convert.ToString(objValue);
                                break;

                        }

                        break;

                    case "MOVSCHEDA":
                        MovScheda ctxScheda = (MovScheda)ctxObject;
                        MovScheda oMs = new MovScheda(ctxScheda.IDMovScheda, (Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"]);

                        switch (propName.ToUpper())
                        {

                            case "DATACREAZIONE":
                            case "DATAULTIMAMODIFICA":

                                objValue = pinfo.GetValue(oMs, null);
                                if (objValue.GetType() == typeof(DateTime))
                                {
                                    DateTime dt = Convert.ToDateTime(objValue);
                                    if (dt.Ticks != 0)
                                    {
                                        sReturn = string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}:{5:00}", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute, dt.Second);
                                    }
                                    else
                                    { return " "; }
                                }
                                break;

                            case "UTENTERILEVAZIONE":
                                PropertyInfo infoUR = ctxType.GetProperty("CodUtenteRilevazione");
                                if (infoUR == null) { return ""; }
                                object objUR = infoUR.GetValue(oMs, null);
                                sReturn = CercaLogin(objUR.ToString(), (Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"]);
                                break;

                            case "UTENTEULTIMAMODIFICA":
                                PropertyInfo infoUM = ctxType.GetProperty("CodUtenteUltimaModifica");
                                if (infoUM == null) { return ""; }
                                object objUM = infoUM.GetValue(oMs, null);
                                sReturn = CercaLogin(objUM.ToString(), (Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"]);
                                break;

                            default:
                                sReturn = Convert.ToString(objValue);
                                break;

                        }
                        break;

                    case "MOVALERTGENERICO":
                        sReturn = Convert.ToString(objValue);
                        break;

                }
            }
            catch (Exception)
            {
            }

            return sReturn;

        }

        private string CercaLogin(string codlogin, Scci.DataContracts.ScciAmbiente ambiente)
        {

            string sReturn = string.Empty;

            try
            {

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                spcoll[0] = new SqlParameterExt("sCodLogin", codlogin, ParameterDirection.Input, SqlDbType.VarChar);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelLogin", spcoll);

                if (dt.Rows.Count != 0)
                {
                    sReturn = dt.Rows[0]["Descrizione"].ToString();
                }

            }
            catch (Exception)
            {
                return sReturn;
            }

            return sReturn;

        }

    }

    public class EvaluatorFillerScheda : BaseFiller
    {

        public override string Process(ParseResult o_parseresult, Dictionary<string, object> dict_context)
        {

            string sReturn = string.Empty;
            int nsequenza = 1;
            bool baggregazione = false;

            try
            {

                foreach (string key in dict_context.Keys)
                {
                    if (dict_context[key].GetType() == typeof(DcSchedaDati))
                    {

                        DcSchedaDati oSchedaDati = (DcSchedaDati)dict_context[key];

                        try
                        {

                            foreach (KeyValuePair<string, List<string>> oPR in o_parseresult.Result)
                            {

                                for (int x = 0; x < o_parseresult.Ids.Count; x++)
                                {

                                    nsequenza = 1;

                                    if (oPR.Key == o_parseresult.Ids[x])
                                    {

                                        if (((List<string>)oPR.Value).Count == 1 || ((List<string>)oPR.Value).Count == 2)
                                        {

                                            switch (((List<string>)oPR.Value)[0].ToString().ToUpper())
                                            {

                                                case "U":
                                                    nsequenza = LeggeSequenze(o_parseresult.Ids[x], oSchedaDati);
                                                    break;

                                                case "SOMMA":
                                                case "MEDIA":
                                                case "MINIMO":
                                                case "MASSIMO":
                                                case "CONTEGGIO":
                                                case "CONCATENA":
                                                    nsequenza = LeggeSequenze(o_parseresult.Ids[x], oSchedaDati);
                                                    baggregazione = true;
                                                    break;

                                                default:
                                                    nsequenza = int.Parse(((List<string>)oPR.Value)[0].ToString());
                                                    break;

                                            }

                                        }

                                        DcDato oDcDato = oSchedaDati.Dati[string.Format("{0}_{1}", o_parseresult.Ids[x], nsequenza.ToString())];

                                        if (oDcDato.Value.GetType() == typeof(DcSchedaDati))
                                        {
                                            oSchedaDati = (DcSchedaDati)oDcDato.Value;
                                            int i = 1;
                                            if (o_parseresult.Result.Count > 0)
                                            {
                                                while (i < o_parseresult.Result.Count - 1)
                                                {
                                                    oSchedaDati = (DcSchedaDati)oSchedaDati.Dati[string.Format("{0}_{1}", o_parseresult.Ids[i], nsequenza.ToString())].Value;
                                                    i++;
                                                }

                                            }
                                            return oSchedaDati.Dati[string.Format("{0}_{1}", o_parseresult.Ids[i], nsequenza.ToString())].Value.ToString();
                                        }
                                        else
                                        {
                                            if (baggregazione == false)
                                            {
                                                if (o_parseresult.Result.Count == 2)
                                                {
                                                    Type type = oDcDato.GetType();
                                                    PropertyInfo info = type.GetProperty(o_parseresult.Ids[1]);
                                                    if (info == null) { return ""; }
                                                    object obj = info.GetValue(oDcDato, null);
                                                    return obj.ToString();
                                                }
                                                else
                                                {
                                                    return oDcDato.Value.ToString();
                                                }
                                            }
                                            else
                                            {
                                                if (((List<string>)oPR.Value).Count == 1)
                                                {
                                                    return DatoAggregato(oSchedaDati, o_parseresult.Ids[x], nsequenza, ((List<string>)oPR.Value)[0].ToString().ToUpper());
                                                }
                                                else if (((List<string>)oPR.Value).Count == 2)
                                                {
                                                    return DatoAggregato(oSchedaDati, o_parseresult.Ids[x], nsequenza, ((List<string>)oPR.Value)[0].ToString().ToUpper(), ((List<string>)oPR.Value)[1].ToString());
                                                }
                                            }
                                        }

                                    }

                                }

                            }

                        }
                        catch (Exception)
                        {
                            return sReturn;
                        }

                    }
                }

            }
            catch (Exception)
            {
            }

            return sReturn;

        }

        private int LeggeSequenze(string key, DcSchedaDati schedadati)
        {

            int nRet = 0;
            int nSeq = 0;
            DcDato oDcDato = null;

            try
            {

                do
                {
                    nSeq++;
                    oDcDato = schedadati.LeggeDato(key, nSeq);

                } while (oDcDato != null);

                nRet = nSeq - 1;

            }
            catch (Exception)
            {
                nRet = 1;
            }

            return nRet;

        }

        private string DatoAggregato(DcSchedaDati oSchedaDati, string key, int nsequenza, string operazione, string separatore = "")
        {

            if (separatore == string.Empty)
            {

                IFormatProvider provider = System.Globalization.CultureInfo.InvariantCulture;
                double[] numbers = (from dato in oSchedaDati.Dati.Values.ToList<DcDato>()
                                    where dato.ID == key
                                    select Convert.ToDouble((dato.Value.ToString() == "" ? "0" : dato.Value.ToString()), provider)).ToArray();

                switch (operazione.ToUpper())
                {

                    case "SOMMA":
                        return numbers.Sum().ToString().Replace(",", ".");

                    case "MEDIA":
                        return numbers.Average().ToString().Replace(",", ".");

                    case "MINIMO":
                        return numbers.Min().ToString().Replace(",", ".");

                    case "MASSIMO":
                        return numbers.Max().ToString().Replace(",", ".");

                    case "CONTEGGIO":
                        return numbers.Length.ToString();

                    default:
                        return "";

                }

            }
            else
            {

                string[] stringa = (from dato in oSchedaDati.Dati.Values.ToList<DcDato>()
                                    where dato.ID == key
                                    select dato.Value.ToString()).ToArray();

                switch (operazione.ToUpper())
                {

                    case "CONCATENA":
                        separatore = separatore.TrimStart().TrimEnd().Replace("'", "");
                        separatore = separatore.ToUpper().Replace("CRLF", Environment.NewLine);
                        separatore = separatore.ToUpper().Replace("COMMA", ",");

                        return string.Join(separatore, stringa);


                    default:
                        return "";

                }

            }

        }

    }

    public class EvaluatorFillerAltraScheda : BaseFiller
    {

        private const string C_ATTRIBUTO = @"ATTRIBUTO";

        public override string Process(ParseResult o_parseresult, Dictionary<string, object> dict_context)
        {

            string sReturn = string.Empty;
            int nsequenza = 1;
            bool baggregazione = false;

            try
            {

                foreach (string key in dict_context.Keys)
                {

                    try
                    {

                        if (o_parseresult.Result.Count == 2 || o_parseresult.Result.Count == 3)
                        {

                            string idscheda = CercaScheda(o_parseresult.Ids[0],
                                                            ((List<string>)o_parseresult.Result[o_parseresult.Ids[0]]),
                                                            (Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"]);

                            if (idscheda != string.Empty)
                            {

                                MovScheda oMovScheda = new MovScheda(idscheda, (Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"]);
                                DcSchedaDati oSchedaDati = (DcSchedaDati)Serializer.FromXmlString(oMovScheda.DatiXML, typeof(DcSchedaDati));

                                if (o_parseresult.Ids[1].ToUpper() == C_ATTRIBUTO.ToUpper())
                                {
                                    Type typeA = oMovScheda.GetType();
                                    string propName = o_parseresult.Result[o_parseresult.Ids[1]][0];

                                    PropertyInfo infoA = ReflectionHelper.GetProperty(typeA, propName);
                                    if (infoA == null)
                                    {
                                        switch (o_parseresult.Result[o_parseresult.Ids[1]][0].ToUpper())
                                        {

                                            case "UTENTERILEVAZIONE":
                                                infoA = typeA.GetProperty("CodUtenteRilevazione");
                                                if (infoA == null) { return ""; }
                                                object objUR = infoA.GetValue(oMovScheda, null);
                                                return CercaLogin(objUR.ToString(), (Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"]);

                                            case "UTENTEULTIMAMODIFICA":
                                                infoA = typeA.GetProperty("CodUtenteUltimaModifica");
                                                if (infoA == null) { return ""; }
                                                object objUUM = infoA.GetValue(oMovScheda, null);
                                                return CercaLogin(objUUM.ToString(), (Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"]);

                                            default:
                                                return "";

                                        }
                                    }
                                    else
                                    {
                                        object objA = infoA.GetValue(oMovScheda, null);
                                        if (objA.GetType() == typeof(DateTime))
                                        {
                                            DateTime dt = (DateTime)objA;
                                            if (dt.Ticks != 0)
                                            {
                                                sReturn = string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}:{5:00}", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute, dt.Second);
                                            }
                                            else
                                            {
                                                return " ";
                                            }
                                        }
                                        else
                                        {
                                            return objA.ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    if (o_parseresult.Result.ContainsKey(o_parseresult.Ids[1]) == true)
                                    {
                                        if (((List<string>)o_parseresult.Result[o_parseresult.Ids[1]]).Count == 1 || ((List<string>)o_parseresult.Result[o_parseresult.Ids[1]]).Count == 2)
                                        {
                                            switch (o_parseresult.Result[o_parseresult.Ids[1]][0].ToString().ToUpper())
                                            {

                                                case "U":
                                                    nsequenza = LeggeSequenze(o_parseresult.Ids[1], oSchedaDati);
                                                    break;

                                                case "SOMMA":
                                                case "MEDIA":
                                                case "MINIMO":
                                                case "MASSIMO":
                                                case "CONTEGGIO":
                                                case "CONCATENA":
                                                    nsequenza = LeggeSequenze(o_parseresult.Ids[1], oSchedaDati);
                                                    baggregazione = true;
                                                    break;

                                                default:
                                                    nsequenza = int.Parse(o_parseresult.Result[o_parseresult.Ids[1]][0].ToString());
                                                    break;

                                            }
                                        }
                                    }

                                    if (o_parseresult.Result.Count == 3)
                                    {
                                        DcDato oDcDato = oSchedaDati.Dati[string.Format("{0}_{1}", o_parseresult.Ids[1], nsequenza.ToString())];
                                        if (oDcDato.Value.GetType() == typeof(DcSchedaDati))
                                        {
                                            return ((DcSchedaDati)oDcDato.Value).Dati[string.Format("{0}_{1}", o_parseresult.Ids[2], nsequenza.ToString())].Value.ToString();
                                        }
                                        else
                                        {
                                            Type type = oDcDato.GetType();
                                            PropertyInfo info = type.GetProperty(o_parseresult.Ids[2]);
                                            if (info == null) { return ""; }
                                            object obj = info.GetValue(oDcDato, null);
                                            return obj.ToString();
                                        }
                                    }
                                    else
                                    {
                                        if (baggregazione == false)
                                        {
                                            return oSchedaDati.Dati[string.Format("{0}_{1}", o_parseresult.Ids[1], nsequenza.ToString())].Value.ToString();
                                        }
                                        else
                                        {
                                            if (((List<string>)o_parseresult.Result[o_parseresult.Ids[1]]).Count == 1)
                                            {
                                                return DatoAggregato(oSchedaDati, o_parseresult.Ids[1], nsequenza, o_parseresult.Result[o_parseresult.Ids[1]][0].ToString().ToUpper());
                                            }
                                            else if (((List<string>)o_parseresult.Result[o_parseresult.Ids[1]]).Count == 2)
                                            {
                                                return DatoAggregato(oSchedaDati, o_parseresult.Ids[1], nsequenza, o_parseresult.Result[o_parseresult.Ids[1]][0].ToString().ToUpper(), o_parseresult.Result[o_parseresult.Ids[1]][1].ToString().ToUpper());
                                            }
                                        }
                                    }
                                }

                            }

                        }

                    }
                    catch (Exception)
                    {
                        return sReturn;
                    }

                }

            }
            catch (Exception)
            {
            }

            return sReturn;

        }

        private string CercaScheda(string codscheda, List<string> TipoRicerca, Scci.DataContracts.ScciAmbiente ambiente)
        {

            string sReturn = string.Empty;

            try
            {


                Parametri op = new Parametri(ambiente);
                op.Parametro.Add("CodScheda", codscheda);

                op.Parametro.Add("IDPaziente", ambiente.Idpaziente);
                op.Parametro.Add("IDEpisodio", ambiente.Idepisodio);
                op.Parametro.Add("IDTrasferimento", ambiente.IdTrasferimento);

                if (TipoRicerca.Count > 0)
                {
                    op.Parametro.Add("TipoRicerca", TipoRicerca[0]);
                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelSchedaDaFiller", spcoll);

                if (dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Esito"].ToString() == "1")
                    {
                        sReturn = dt.Rows[0]["IDScheda"].ToString();
                    }

                }

            }
            catch (Exception)
            {
                return sReturn;
            }

            return sReturn;

        }

        private int LeggeSequenze(string key, DcSchedaDati schedadati)
        {

            int nRet = 0;
            int nSeq = 0;
            DcDato oDcDato = null;

            try
            {

                do
                {
                    nSeq++;
                    oDcDato = schedadati.LeggeDato(key, nSeq);

                } while (oDcDato != null);

                nRet = nSeq - 1;

            }
            catch (Exception)
            {
                nRet = 1;
            }

            return nRet;

        }

        private string CercaLogin(string codlogin, Scci.DataContracts.ScciAmbiente ambiente)
        {

            string sReturn = string.Empty;

            try
            {

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                spcoll[0] = new SqlParameterExt("sCodLogin", codlogin, ParameterDirection.Input, SqlDbType.VarChar);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelLogin", spcoll);

                if (dt.Rows.Count != 0)
                {
                    sReturn = dt.Rows[0]["Descrizione"].ToString();
                }

            }
            catch (Exception)
            {
                return sReturn;
            }

            return sReturn;

        }

        private string DatoAggregato(DcSchedaDati oSchedaDati, string key, int nsequenza, string operazione, string separatore = "")
        {

            if (separatore == string.Empty)
            {

                IFormatProvider provider = System.Globalization.CultureInfo.InvariantCulture;
                double[] numbers = (from dato in oSchedaDati.Dati.Values.ToList<DcDato>()
                                    where dato.ID == key
                                    select Convert.ToDouble((dato.Value.ToString() == "" ? "0" : dato.Value.ToString()), provider)).ToArray();

                switch (operazione.ToUpper())
                {

                    case "SOMMA":
                        return numbers.Sum().ToString().Replace(",", ".");

                    case "MEDIA":
                        return numbers.Average().ToString().Replace(",", ".");

                    case "MINIMO":
                        return numbers.Min().ToString().Replace(",", ".");

                    case "MASSIMO":
                        return numbers.Max().ToString().Replace(",", ".");

                    case "CONTEGGIO":
                        return numbers.Length.ToString();

                    default:
                        return "";

                }

            }
            else
            {

                string[] stringa = (from dato in oSchedaDati.Dati.Values.ToList<DcDato>()
                                    where dato.ID == key
                                    select dato.Value.ToString()).ToArray();

                switch (operazione.ToUpper())
                {

                    case "CONCATENA":
                        separatore = separatore.TrimStart().TrimEnd().Replace("'", "");
                        separatore = separatore.ToUpper().Replace("CRLF", Environment.NewLine);
                        separatore = separatore.ToUpper().Replace("COMMA", ",");
                        return string.Join(separatore, stringa);

                    default:
                        return "";

                }

            }

        }

    }

    public class EvaluatorFillerDLookUp : BaseFiller
    {

        private const string C_TIPORICERCA = @"DLOOKUP";
        private const string C_NOTAANAMNESTICA = @"NOTAANAMNESTICA";
        private const string C_DATAINGRUA = @"DATAINGRESSOUA";
        private const string C_DATAORAINGRUA = @"DATAORAINGRESSOUA";

        private const string C_TIPOTXT = @"TXT";
        private const string C_TIPORTF = @"RTF";

        private string CercaTestiNA(string tipo, List<string> filtri, Scci.DataContracts.ScciAmbiente ambiente)
        {

            string sReturn = string.Empty;
            RtfFiles rtf = new RtfFiles();

            try
            {

                Parametri op = new Parametri(ambiente);
                op.Parametro.Add("IDPaziente", ambiente.Idpaziente);
                op.Parametro.Add("VisualizzazioneSintetica", "1");

                if (filtri.Count > 0)
                {
                    string[] id = filtri.ToArray();
                    op.ParametroRipetibile.Add("CodTipoAlertAllergiaAnamnesi", id);
                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovAlertAllergieAnamnesi", spcoll);

                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow dr in dt.Rows)
                    {

                        switch (tipo)
                        {

                            case C_TIPOTXT:
                                string testotxt = dr["AnteprimaTXT"].ToString();
                                sReturn += testotxt;
                                break;

                            case C_TIPORTF:
                                string testortf = dr["AnteprimaRTF"].ToString();
                                sReturn = rtf.joinRtf(testortf, sReturn);
                                break;

                        }

                    }

                }

            }
            catch (Exception)
            {

            }

            return sReturn;

        }

        private DateTime? procDataIngressoUA(ScciAmbiente ambiente)
        {
            using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
            {
                FwDataParametersList plist = new FwDataParametersList();
                plist.Add("uIDTrasferimento", new Guid(ambiente.IdTrasferimento), ParameterDirection.Input, DbType.Guid);
                DataTable data = fdc.Query<DataTable>("MSP_DLookup_DataIngressoUA", plist, CommandType.StoredProcedure);

                if ((data == null) || (data.Rows.Count == 0)) return null;

                DateTime dt = (DateTime)data.Rows[0][0];
                return dt;
            }
        }

        public override string Process(ParseResult o_parseresult, Dictionary<string, object> dict_context)
        {

            string sReturn = string.Empty;

            try
            {
                if (o_parseresult.Result.Count == 2 && o_parseresult.Ids[0].ToUpper() == C_TIPORICERCA)
                {
                    switch (o_parseresult.Ids[1].ToUpper())
                    {

                        case C_DATAINGRUA:
                            DateTime? fillvalue = procDataIngressoUA((Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"]);
                            if (fillvalue == null) return "";
                            DateTime dt = fillvalue.Value;
                            sReturn = string.Format("{0:00}/{1:00}/{2:0000}", dt.Day, dt.Month, dt.Year);

                            break;

                        case C_DATAORAINGRUA:
                            DateTime? fillvaluedt = procDataIngressoUA((Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"]);
                            if (fillvaluedt == null) return "";
                            DateTime dto = fillvaluedt.Value;
                            sReturn = string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}:{5:00}", dto.Day, dto.Month, dto.Year, dto.Hour, dto.Minute, dto.Second);

                            break;

                    }

                }

                if (o_parseresult.Result.Count == 3 && o_parseresult.Ids[0].ToUpper() == C_TIPORICERCA)
                {

                    switch (o_parseresult.Ids[1].ToUpper())
                    {

                        case C_NOTAANAMNESTICA:
                            sReturn = CercaTestiNA(o_parseresult.Ids[2].ToUpper(),
                                                    (List<string>)o_parseresult.Result[o_parseresult.Ids[1]],
                                                    (Scci.DataContracts.ScciAmbiente)dict_context["Ambiente"]);
                            break;

                    }

                }

            }
            catch (Exception)
            {
            }

            return sReturn;

        }

    }

}
