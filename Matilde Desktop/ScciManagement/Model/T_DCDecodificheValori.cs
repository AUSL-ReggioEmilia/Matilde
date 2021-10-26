using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.ScciManagement.Model
{

    public static partial class FwDataConnectionExt
    {

        public static FwDataBufferedList<T_DCDecodificheValori> T_DCDecodificheValori(this FwDataConnection cnn, List<string> lstDecodifiche, List<string> lstOrdinamento)
        {

            string ssql = $"Select * From T_DCDecodificheValori" +
                            $"\nWhere {(lstDecodifiche.Count > 0 ? "CodDec in ('" + string.Join("','", lstDecodifiche) + "') " : "0=1")}" +
                            $"\nOrder By {(lstOrdinamento.Count > 0 ? string.Join(",", lstOrdinamento) : "CodDec, Codice")}";
            return cnn.T_DCDecodificheValoriAll(ssql);

        }

        private static FwDataBufferedList<T_DCDecodificheValori> T_DCDecodificheValoriAll(this FwDataConnection cnn, string sql)
        {
            return cnn.Query<FwDataBufferedList<T_DCDecodificheValori>>(sql, null, CommandType.Text); ;
        }

    }

    [ModelTable(@"T_DCDecodificheValori")]
    public class T_DCDecodificheValoriBuffer : FwModelBuffer<T_DCDecodificheValori>
    {
        public T_DCDecodificheValoriBuffer() : base()
        {
        }

        public T_DCDecodificheValoriBuffer(string conn) : base(conn)
        {
        }

                                                        public static T_DCDecodificheValoriBuffer Select(string conn)
        {
            T_DCDecodificheValoriBuffer data = new T_DCDecodificheValoriBuffer(conn);
            data.Select(useCache: false );

            return data;
        }

                                                        public static T_DCDecodificheValoriBuffer Select(string conn, string codDec, int cacheMinutes = 0)
        {
            T_DCDecodificheValoriBuffer data = new T_DCDecodificheValoriBuffer(conn);
            string where = $"CodDec='{codDec}'";
            string orderBy = "Ordine ASC";

            data.Select(where: where, orderBy: orderBy, useCache: true, cacheExpireMinutes: cacheMinutes);

            return data;
        }

                                                        public static DataTable SelectEavDataTable(string conn, string codDec)
        {
            using (FwDataConnection fdc = new FwDataConnection(conn))
            {
                                string sqlval = $"Select CodDec as ID_Dizionari, Codice as ID, Descrizione, Ordine, Icona As Immagine, DtValI As DataInizioVal, DtValF As DataFineVal From T_DCDecodificheValori Where " +
                                $"CodDec = '{codDec}' order by Ordine";

                DataTable val = fdc.Query<DataTable>(sqlval);

                return val;
            }
        }

    }


    [DataContract()]
    [ModelTable(@"T_DCDecodificheValori")]
    public class T_DCDecodificheValori : FwModelRow<T_DCDecodificheValori>
    {
        public T_DCDecodificheValori() : base()
        {
        }

        [DataMember()]
        public String CodDec { get; set; }

        [DataMember()]
        public String Codice { get; set; }

        [DataMember()]
        public String Descrizione { get; set; }

        [DataMember()]
        public Int32? Ordine { get; set; }

        [DataMember()]
        public DateTime? DtValI { get; set; }

        [DataMember()]
        public DateTime? DtValF { get; set; }

        [DataMember()]
        public UniFwByteArray Icona { get; set; }

        [DataMember()]
        public String InfoRTF { get; set; }

        [DataMember()]
        public String Path { get; set; }

    }

}
