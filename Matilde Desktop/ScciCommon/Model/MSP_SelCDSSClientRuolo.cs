using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{
    public static partial class FwDataConnectionExt
    {
        public static FwDataBufferedList<SelCdssRuoloRow> MSP_SelCDSSClientRuolo(this FwDataConnection cnn, string codRuolo, string codCdss = null, string codTipoCdss = null)
        {
            FwDataBufferedList<SelCdssRuoloRow> result = null;

            XmlParameter xp = new XmlParameter();

            xp.AddParameterIfNotEmpty("CodRuolo", codRuolo);
            xp.AddParameterIfNotEmpty("CodCDSS", codCdss);
            xp.AddParameterIfNotEmpty("CodTipoCDSS", codTipoCdss);

            result = cnn.Query<FwDataBufferedList<SelCdssRuoloRow>>("MSP_SelCDSSClientRuolo", xp.ToFwDataParametersList(), CommandType.StoredProcedure);

            return result;
        }
    }

    [DataContract]
    [ModelStoredProcedure("MSP_SelCDSSClientRuolo")]
    public class SelCdssRuoloRow
        : FwModelRow<SelCdssRuoloRow>
    {
        public SelCdssRuoloRow()
        {
            this.FwDataConnection = new FwDataConnection(Database.ConnectionString);
        }

        [DataMember]
        [ValidationKey]
        public Decimal ID { get; set; }

        [DataMember]
        public String CodRuolo { get; set; }

        [DataMember]
        public String CodAzione { get; set; }

        [DataMember]
        public String DescrizioneAzione { get; set; }

        [DataMember]
        public String CodPlugin { get; set; }

        [DataMember]
        public String DescrizionePlugin { get; set; }

        [DataMember]
        public String NomePlugin { get; set; }

        [DataMember]
        public String ComandoPlugin { get; set; }

        [DataMember]
        public String ModalitaPlugin { get; set; }

        [DataMember]
        public Int32 OrdinePlugin { get; set; }

        [DataMember]
        public String Parametri { get; set; }

        [DataMember]
        public Object IconaPlugin { get; set; }

        [DataFieldIgnore]
        public XDocument ParametriXDoc
        {
            get
            {
                if (String.IsNullOrEmpty(this.Parametri) == false)
                    return XDocument.Parse(this.Parametri);
                else
                    return null;
            }
        }

        public List<String> GetParamValues(string paramName)
        {
            if (this.ParametriXDoc == null)
                return null;

            List<String> valRes = new List<string>();

            List<XElement> elements = this.ParametriXDoc.Descendants().Where(p => p.Name.LocalName == paramName).ToList();

            foreach (XElement ele in elements)
            {
                valRes.Add(ele.Value);
            }

            if (valRes.Count == 0)
                return null;
            else
                return valRes;
        }

        public bool GetParamBool(string paramName, bool defaultForNull)
        {
            List<String> lval = this.GetParamValues(paramName);
            if (lval == null)
                return defaultForNull;
            else
            {
                string val = lval.First().ToUpper();

                bool restult = (
                                    (val == "S") ||
                                    (val == "1") ||
                                    (val == "TRUE")
                                );

                return restult;
            }

        }

        public bool? GetParamNlBool(string paramName, bool? defaultForNull)
        {
            List<String> lval = this.GetParamValues(paramName);
            if (lval == null)
                return defaultForNull;
            else
            {
                string val = lval.First().ToUpper();

                bool restult = (
                                    (val == "S") ||
                                    (val == "1") ||
                                    (val == "TRUE")
                                );

                return restult;
            }

        }

        public string GetParamString(string paramName, string defaultForNull)
        {
            List<String> lval = this.GetParamValues(paramName);
            if (lval == null)
                return defaultForNull;
            else
            {
                return lval.First();
            }
        }

        public string[] GetParamValuesArray(string paramName)
        {
            List<String> val = this.GetParamValues(paramName);

            if (val == null)
                return null;

            return val.ToArray();
        }

    }
}
