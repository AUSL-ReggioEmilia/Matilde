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

namespace UnicodeSrl.Scci.Model
{

    public static partial class FwDataConnectionExt
    {

        public static T_AgendeRow T_Agende(this FwDataConnection cnn, string codagenda)
        {

            FwDataBufferedList<T_AgendeRow> result = null;

            if (TableCache.IsInTableCache("T_Agende") == false)
            {
                result = cnn.Query<FwDataBufferedList<T_AgendeRow>>("Select * From T_Agende", null, CommandType.Text);
                List<object> list = result.Buffer.ToList<object>();
                TableCache.AddToCache("T_Agende", list);
            }

            T_AgendeRow row = TableCache.GetCachedRow<T_AgendeRow>("T_Agende", (x => x.Codice == codagenda));

            return row;

        }

    }

    [DataContract()]
    [ModelTable("T_Agende")]
    public class T_AgendeRow : FwModelRow<T_AgendeRow>
    {
        public T_AgendeRow() : base()
        {
        }

        [DataMember()]
        [ValidationKey()]
        [ValidationStringLenght(20)]
        public String Codice { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String Descrizione { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodTipoAgenda { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String Colore { get; set; }

        [DataMember()]
        public String ElencoCampi { get; set; }

        [DataMember()]
        public Int16? IntervalloSlot { get; set; }

        [DataMember()]
        public String OrariLavoro { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodEntita { get; set; }

        [DataMember()]
        public Int32? Ordine { get; set; }

        [DataMember()]
        public Boolean? UsaColoreTipoAppuntamento { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String DescrizioneAlternativa { get; set; }

        [DataMember()]
        public Int32? MassimoAnticipoPrenotazione { get; set; }

        [DataMember()]
        public Int32? MassimoRitardoPrenotazione { get; set; }

        [DataMember()]
        public Boolean? Lista { get; set; }

        [DataMember()]
        public String ParametriLista { get; set; }

        [DataMember()]
        public String Risorse { get; set; }

        [DataMember()]
        public Boolean? EscludiFestivita { get; set; }

    }

}
