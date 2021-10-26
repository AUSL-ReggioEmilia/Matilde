using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.Scci.Model
{
    public static partial class FwDataConnectionExt
    {
        public static T_RuoliRow T_RuoliRow(this FwDataConnection cnn, string codRuolo)
        {
            T_RuoliRow ruolo = new T_RuoliRow(codRuolo);
            T_RuoliRow cached = RowCache.GetCachedRow<T_RuoliRow>(x => x.Codice == codRuolo, ruolo);

            return cached;

        }

        public static FwDataBufferedList<T_RuoliRow> T_RuoliConsegneRow(this FwDataConnection cnn, string tipoconsegna)
        {

            string ssql = "Select * From T_Ruoli";

            ssql += "\nWhere Codice In (Select CodRuolo from T_AssRuoliAzioni " +
                                        "Where CodEntita = 'CSP' And CodVoce = '" + tipoconsegna + "' And CodAzione = 'CND')";

            ssql += "\nOrder By Descrizione";

            return cnn.T_RuoliAll(ssql);

        }

        private static FwDataBufferedList<T_RuoliRow> T_RuoliAll(this FwDataConnection cnn, string sql)
        {
            return cnn.Query<FwDataBufferedList<T_RuoliRow>>(sql, null, CommandType.Text); ;
        }

    }

    [DataContract]
    [ModelTable("T_Ruoli")]
    public class T_RuoliRow
        : FwModelRow<T_RuoliRow>
    {
        public T_RuoliRow()
        {
            this.FwDataConnection = new FwDataConnection(Database.ConnectionString);
        }

        public T_RuoliRow(string codice) : this()
        {
            this.Codice = codice;
        }

        [DataMember]
        [ValidationStringLenght(20)]
        [ValidationKey]
        public String Codice { get; set; }

        [ValidationStringLenght(255)]
        public String Descrizione { get; set; }

        [DataMember]
        [ValidationStringLenght(4000)]
        public string Note { get; set; }

        [DataMember]
        [ValidationStringLenght(20)]
        public string CodTipoDiario { get; set; }

        [DataMember]
        public int? NumMaxCercaEpi { get; set; }

        [DataMember]
        public bool? RichiediPassword { get; set; }

        [DataMember]
        public bool? LimitaEVCAmbulatoriale { get; set; }

        private FwDataBufferedList<T_ModuliRow> m_moduli = null;

        public FwDataBufferedList<T_ModuliRow> Moduli
        {
            get
            {
                if (m_moduli == null)
                {
                    XmlParameter xp = new XmlParameter();
                    xp.AddParameter("CodRuolo", this.Codice);

                    using (FwDataConnection cnn = new FwDataConnection(Database.ConnectionString))
                    {
                        m_moduli = cnn.Query<FwDataBufferedList<T_ModuliRow>>("MSP_SelModuli", xp.ToFwDataParametersList(), CommandType.StoredProcedure);
                    }
                }

                return m_moduli;
            }
        }




        public bool HasModule(string codModulo)
        {
            IEnumerable<T_ModuliRow> res = this.Moduli.Buffer.Where(m => m.Codice == codModulo);

            if (res.Count() > 0)
                return true;
            else
                return false;
        }

    }
}
