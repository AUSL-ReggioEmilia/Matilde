using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.EavFramework;
using UnicodeSrl.Framework;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.ScciManagement.Model
{
    [ModelTable(@"T_DCDecodifiche")]
    public class T_DCDecodificheBuffer : FwModelBuffer<T_DCDecodifiche>
    {
                                                        public static T_DCDecodificheBuffer Select(string conn)
        {
            T_DCDecodificheBuffer data = new T_DCDecodificheBuffer(conn);
            data.Select(useCache: false );

            return data;
        }

                                                        public static T_DCDecodifiche SelectFirst(string conn, string codice)
        {
            T_DCDecodificheBuffer data = new T_DCDecodificheBuffer(conn);
            string where = $"Codice ='{codice}'";
            data.Select(where: where, useCache: false);

            return data.First;
        }

                                                        public async static Task AddToEavSchemaAsync(EavSchema schema, string conn)
        {
                        List<String> dicKeys = schema.GetMissingDictionaryKeys();
            if (dicKeys.Count > 0)
            {
                foreach (string cod in dicKeys)
                {
                    EavDictionary sccidec = await T_DCDecodifiche.ToEavDictionaryAsync(conn, cod);
                    schema.Dictionaries.Add(sccidec.Key, sccidec);
                }
            }
        }

        public T_DCDecodificheBuffer() : base()
        {
        }

        public T_DCDecodificheBuffer(string conn) : base(conn)
        {
        }

    }


    [DataContract()]
    [ModelTable(@"T_DCDecodifiche")]
    public class T_DCDecodifiche : FwModelRow<T_DCDecodifiche>
    {
                                                                public async static Task<EavDictionary> ToEavDictionaryAsync(string conn, string codice)
        {
            EavDictionary eavdict = null ;

            T_DCDecodifiche dcDecod = T_DCDecodificheBuffer.SelectFirst(conn, codice);

            DataTable dataTableValori = T_DCDecodificheValoriBuffer.SelectEavDataTable(conn, codice);

                        DataRow[] mkrows = dataTableValori.Select("Immagine IS NOT NULL");
            bool hasMarkers = ((mkrows != null) && (mkrows.Length > 0));

            eavdict = new EavDictionary(dcDecod.Codice, dcDecod.Descrizione, hasMarkers, dataTableValori);

            return eavdict;
        }

        public T_DCDecodifiche() : base()
        {
        }

        public T_DCDecodifiche(string conn) : base(conn)
        {
        }

        [ValidationKey]
        [DataMember()]
        public String Codice { get; set; }

        [DataMember()]
        public String Descrizione { get; set; }

    }
}
