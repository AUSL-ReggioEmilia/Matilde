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
        public static FwDataBufferedList<T_ScreenRow> MSP_SelScreen(this FwDataConnection cnn, string codUA, string codRuolo, en_TipoScreen codTipoScreen)
        {
            FwDataBufferedList<T_ScreenRow> result = null;

            XmlParameter xp = new XmlParameter();

            xp.AddParameterIfNotEmpty("CodUA", codUA);
            xp.AddParameterIfNotEmpty("CodRuolo", codRuolo);
            xp.AddParameterIfNotEmpty("CodTipoScreen", codTipoScreen.ToString());

            result = cnn.Query<FwDataBufferedList<T_ScreenRow>>("MSP_SelScreen", xp.ToFwDataParametersList(), CommandType.StoredProcedure);

            return result;
        }
    }

    [DataContract]
    [ModelTable("T_Screen")]
    public class T_ScreenRow
        : FwModelRow<T_ScreenRow>
    {
        public T_ScreenRow()
        {
            this.FwDataConnection = new FwDataConnection(Database.ConnectionString);
        }

        [DataMember]
        [ValidationStringLenght(20)]
        [ValidationKey]
        public String Codice { get; set; }

        [DataMember]
        [ValidationStringLenght(255)]
        public String Descrizione { get; set; }

        [DataMember]
        public String Attributi { get; set; }

        [DataMember]
        public Int16 Righe
        {
            get;
            set;
        }

        [DataMember]
        public Int16 Colonne { get; set; }

        [DataMember]
        [ValidationStringLenght(20)]
        public String CodTipoScreen { get; set; }

        [DataMember]
        public System.Double? AltezzaRigaGrid { get; set; }

        [DataMember]
        public System.Double? LarghezzaColonnaGrid { get; set; }

        [DataMember]
        public System.Boolean? CaricaPerRiga { get; set; }

        [DataMember]
        public System.Boolean? AdattaAltezzaRighe { get; set; }

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

        #region     Model Collegati

        [DataFieldIgnore]
        public en_TipoScreen TipoScreen
        {
            get
            {
                return (en_TipoScreen)Enum.Parse(typeof(en_TipoScreen), this.CodTipoScreen);
            }
            set
            {
                this.CodTipoScreen = value.ToString();
            }
        }

        #endregion

    }
}
