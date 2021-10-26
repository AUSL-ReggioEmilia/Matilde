using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Interfaces;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Extension;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{
    public static partial class FwDataConnectionExt
    {

        public static T_ConfigRow T_Config(this FwDataConnection cnn, EnumConfigTable idConfig)
        {
            FwDataBufferedList<T_ConfigRow> result = null;

            if (TableCache.IsInTableCache("T_Config") == false)
            {
                result = cnn.Query<FwDataBufferedList<T_ConfigRow>>("Select * From T_Config", null, CommandType.Text);
                List<object> list = result.Buffer.ToList<object>();
                TableCache.AddToCache("T_Config", list);
            }

            T_ConfigRow row = TableCache.GetCachedRow<T_ConfigRow>("T_Config", (x => x.ID == Convert.ToInt32(idConfig)));

            return row;
        }

    }



    [DataContract]
    [ModelTable("T_Config")]
    public class T_ConfigRow
        : FwModelRow<T_ConfigRow>
    {
        public T_ConfigRow()
        {
            if (Database.ConnectionString.IsNotNullOrEmpty())
                this.FwDataConnection = new FwDataConnection(Database.ConnectionString);
        }

        [DataMember]
        [ValidationKey]
        public int ID { get; set; }

        [DataMember]
        public string Descrizione { get; set; }

        [DataMember]
        public string Valore { get; set; }

        [IgnoreDataMember]
        public EnumConfigTable EnumID
        {
            get
            {
                return (EnumConfigTable)this.ID;
            }
        }

    }
}
