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
        public static FwDataBufferedList<T_ScreenTileRow> MSP_SelScreenTile(this FwDataConnection cnn, string codScreen)
        {
            FwDataBufferedList<T_ScreenTileRow> result = null;

            FwDataParametersList fwp = new FwDataParametersList();
            fwp.Add("codScreen", codScreen, ParameterDirection.Input, DbType.String, 20);

            result = cnn.Query<FwDataBufferedList<T_ScreenTileRow>>("MSP_SelScreenTile", fwp, CommandType.StoredProcedure);

            return result;
        }
    }


    [DataContract]
    [ModelTable("T_ScreenTile")]
    public class T_ScreenTileRow
        : FwModelRow<T_ScreenTileRow>
    {
        public T_ScreenTileRow()
        {
            this.FwDataConnection = new FwDataConnection(Database.ConnectionString);
        }

        [DataMember]
        [ValidationKey]
        public Decimal ID { get; set; }

        [DataMember]
        [ValidationRequired]
        public string CodScreen { get; set; }

        [DataMember]
        public string CodPlugin { get; set; }

        [DataMember]
        public string NomeTile { get; set; }

        [DataMember]
        public Int16 Riga { get; set; }

        [DataMember]
        public Int16 Colonna { get; set; }

        [DataMember]
        public Int16 Altezza { get; set; }

        [DataMember]
        public Int16 Larghezza { get; set; }

        [DataMember]
        public Boolean? InEvidenza { get; set; }

        [DataMember]
        public Boolean? Fissa { get; set; }

        [DataMember]
        public Boolean? NonCollassabile { get; set; }


        [DataMember]
        public Boolean Collassata { get; set; }

        [DataMember]
        public String Attributi
        {
            get;
            set;
        }

        [DataFieldIgnore]
        public XDocument AttributiXDoc
        {
            get
            {
                if (String.IsNullOrEmpty(this.Attributi) == false)
                    return XDocument.Parse(this.Attributi);
                else
                    return null;
            }
        }

        public List<String> GetParamValues(string paramName)
        {
            if (this.AttributiXDoc == null)
                return null;

            List<String> valRes = new List<string>();

            List<XElement> elements = this.AttributiXDoc.Descendants().Where(p => p.Name.LocalName == paramName).ToList();

            foreach (XElement ele in elements)
            {
                valRes.Add(ele.Value);
            }

            if (valRes.Count == 0)
                return null;
            else
                return valRes;
        }

        public List<XElement> GetParamElements(string paramName)
        {
            if (this.AttributiXDoc == null)
                return null;

            List<String> valRes = new List<string>();

            List<XElement> elements = this.AttributiXDoc.Descendants().Where(p => p.Name.LocalName == paramName).ToList();

            return elements;
        }

        public string[] GetParamValuesArray(string paramName)
        {
            List<String> val = this.GetParamValues(paramName);

            if (val == null)
                return null;

            return val.ToArray();
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


        private SelCdssRuoloRow m_SelCdssRuoloRow = null;

        public SelCdssRuoloRow GetSelCdssRuoloRow(string codRuolo)
        {
            if (m_SelCdssRuoloRow == null)
            {
                FwDataBufferedList<SelCdssRuoloRow> result = new FwDataBufferedList<SelCdssRuoloRow>();

                using (FwDataConnection cnn = new FwDataConnection(this.FwDataConnection.ConnectionString))
                {
                    result = cnn.MSP_SelCDSSClientRuolo(codRuolo, this.CodPlugin);
                }

                if (result.Buffer.Count > 0)
                    m_SelCdssRuoloRow = result.Buffer.First();
                else
                    m_SelCdssRuoloRow = null;
            }

            return m_SelCdssRuoloRow;
        }



    }
}
