using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.Helpers;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Scci.Statics;


namespace UnicodeSrl.Scci.Model
{

    public class T_StatoOrdine : FwModelRow<T_StatoOrdine>
    {

        public static T_StatoOrdine Select(string codice)
        {
            FwDataBufferedList<T_StatoOrdine> result = null;

            if (TableCache.IsInTableCache("T_StatoOrdine") == false)
            {
                using (FwDataConnection cnn = new FwDataConnection(Database.ConnectionString))
                {
                    result = cnn.Query<FwDataBufferedList<T_StatoOrdine>>("Select * From T_StatoOrdine", null, CommandType.Text);
                    List<object> list = result.Buffer.ToList<object>();
                    TableCache.AddToCache("T_StatoOrdine", list);
                }

            }

            T_StatoOrdine row = TableCache.GetCachedRow<T_StatoOrdine>("T_StatoOrdine", (x => x.Codice.ToUpper() == codice.ToUpper()));

            return row;
        }

        [ValidationKey]
        public String Codice { get; set; }
        public String Descrizione { get; set; }
        public String Colore { get; set; }

        [DataFieldIgnore]
        public Color ColorObject
        {
            get
            {
                if (this.Colore == "") return Color.Transparent;
                if (this.Colore == null) return Color.Transparent;

                return ScciDbHelper.GetColorFromString(this.Colore);
            }
        }

    }
}
