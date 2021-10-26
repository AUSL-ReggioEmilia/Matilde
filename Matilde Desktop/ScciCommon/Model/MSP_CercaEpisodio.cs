using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Utility;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{
    [DataContract]
    public class MSP_CercaEpisodioRow
    {
        public MSP_CercaEpisodioRow()
        {

        }

        public MSP_CercaEpisodioRow(DataRow row)
        {
            this.FromDataRow(row);
        }

        public void FromDataRow(DataRow row)
        {
            List<PropertyInfo> props = ReflectionHelper.GetSettableProps(this.GetType());

            foreach (PropertyInfo pi in props)
            {
                if (Attribute.IsDefined(pi, typeof(DataFieldIgnoreAttribute)) == false)
                {
                    string fld = "";


                    if (Attribute.IsDefined(pi, typeof(DataFieldAttribute)))
                    {
#if NET40
                        DataFieldAttribute df = pi.GetCustomAttribute<DataFieldAttribute>();
#elif NET472
                        DataFieldAttribute df = System.Reflection.CustomAttributeExtensions.GetCustomAttribute<DataFieldAttribute>(pi);
#endif
                        fld = df.DataField;
                    }
                    else
                        fld = pi.Name;

                    if (row.Table.Columns.Contains(fld) && pi.CanWrite)
                    {
                        object value = row[fld];

                        if (value == DBNull.Value)
                            pi.SetValue(this, null, null);
                        else
                            pi.SetValue(this, row[fld], null);

                    }

                }
            }

        }


        public Byte[] GetImageConsenso()
        {
            if ((this.CodStatoConsensoCalcolato == null) || (this.CodStatoConsensoCalcolato == "")) return null;

            Dictionary<String, object> imgData = null;

            string sql = $"Select Icona from T_StatoConsensoCalcolato Where Codice = '{this.CodStatoConsensoCalcolato}'";

            using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
            {
                imgData = conn.QueryFirst(sql, null, CommandType.Text);
            }

            if (imgData == null) return null;
            byte[] imgBytes = (byte[])imgData.Values.First();


            return imgBytes;
        }

        #region  DataProps

        [DataMember()]
        [ValidationKey()]
        public Guid IDTrasferimento { get; set; }

        [DataMember()]
        public Guid IDEpisodio { get; set; }

        [DataMember()]
        public Guid? IDPaziente { get; set; }

        [DataMember()]
        public String CodSac { get; set; }

        [DataMember()]
        public String Cognome { get; set; }

        [DataMember()]
        public String Nome { get; set; }

        [DataMember()]
        public DateTime? DataNascita { get; set; }

        [DataMember()]
        public String Sesso { get; set; }

        [DataMember()]
        public String ComuneNascita { get; set; }

        [DataMember()]
        public String CodProvinciaNascita { get; set; }

        [DataMember()]
        public String Paziente { get; set; }

        [DataMember()]
        public String Paziente2 { get; set; }

        [DataMember()]
        [ValidationStringLenght(286)]
        public String Paziente3 { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodiceFiscale { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodUA { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String DescrUA { get; set; }

        [DataMember()]
        [DataField("UO - Settore")]
        public String UoSettore { get; set; }

        [DataMember()]
        public DateTime? DataRicovero { get; set; }

        [DataMember()]
        public DateTime? DataDimissione { get; set; }

        [DataMember()]
        public DateTime? DataIngresso { get; set; }

        [DataMember()]
        public DateTime? DataUscita { get; set; }

        [DataMember()]
        public DateTime? DataOrdinamentoCartella { get; set; }

        [DataMember()]
        [DataField("Data Ingresso Data Ricovero")]
        public String DataIngressoDataRicovero { get; set; }

        [DataMember()]
        public DateTime? DataIngressoGriglia { get; set; }

        [DataMember()]
        [ValidationStringLenght(8)]
        public DateTime? DataRicoveroGriglia { get; set; }

        [DataMember()]
        [ValidationStringLenght(56)]
        [DataField("Data Dimissione Data Trasferimento")]
        public String DataDimissioneDataTrasferimento { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodStatoTrasferimento { get; set; }

        [DataMember()]
        public String DecrStato { get; set; }

        [DataMember()]
        public String ColoreStatoTrasferimento { get; set; }

        [DataMember()]
        public String CodUO { get; set; }

        [DataMember()]
        public String DescrUO { get; set; }

        [DataMember()]
        public String CodSettore { get; set; }

        [DataMember()]
        public String DescrSettore { get; set; }

        [DataMember()]
        public String CodStanza { get; set; }

        [DataMember()]
        public String DescrStanza { get; set; }

        [DataMember()]
        public String CodLetto { get; set; }

        [DataMember()]
        public String DescrLetto { get; set; }

        [DataMember()]
        public String DescStanzaLetto { get; set; }

        [DataMember()]
        public String CodTipoEpisodio { get; set; }

        [DataMember()]
        public String NumeroNosologico { get; set; }

        [DataMember()]
        public String NumeroListaAttesa { get; set; }

        [DataMember()]
        public String DescEpisodio { get; set; }

        [DataMember()]
        public Int32 NumAllergie { get; set; }

        [DataMember()]
        public Int32 NumAlertGenerici { get; set; }

        [DataMember()]
        public Int32 NumEvidenzaClinica { get; set; }

        [DataMember()]
        public Int32? FlagCartellaInVisione { get; set; }

        [DataMember()]
        public Int32? FlagHoDatoCartellaInVisione { get; set; }

        [DataMember()]
        public String CodStatoCartella { get; set; }

        [DataMember()]
        public String DescrStatoCartella { get; set; }

        [DataMember()]
        public String ColoreStatoCartella { get; set; }

        [DataMember()]
        public String DescrCartellaGriglia { get; set; }

        [DataMember()]
        public Int32? IconaStatoCartella { get; set; }

        [DataMember()]
        public String NumeroCartella { get; set; }

        [DataMember()]
        public Guid? IDCartella { get; set; }

        [DataMember()]
        public Int32? FlagPazienteSeguito { get; set; }

        [DataMember()]
        public Int32? FlagCartellaDaChiudere { get; set; }

        [DataMember()]
        public String DescrStatoGriglia { get; set; }

        [DataMember()]
        public String CodStatoConsensoCalcolato { get; set; }

        #endregion  DataProps

        #region     Prop: Età

        public string Eta
        {
            get
            {

                if (this.EtaAnni > 14)
                    return this.EtaAnni.ToString() + "aa";
                else
                    return this.EtaPediatrico;

            }

        }

        public int EtaAnni
        {
            get
            {
                int iAnni = -1;

                if ((this.DataNascita.HasValue) && (this.DataNascita.Value.Ticks != 0))
                {
                    iAnni = DateTime.Now.Year - this.DataNascita.Value.Year;

                    if (this.DataNascita.Value.DayOfYear > DateTime.Now.Date.DayOfYear && iAnni > 0)
                        iAnni -= 1;
                }

                return iAnni;

            }

        }

        public string EtaPediatrico
        {
            get
            {

                string sRet = string.Empty;

                if ((this.DataNascita.HasValue) && (this.DataNascita.Value.Ticks != 0))
                {
                    TimeSpan ts = DateTime.Now.Subtract(this.DataNascita.Value);
                    DateTime dateTimeDiff = DateTime.MinValue + ts;

                    double totalYears = ts.TotalDays / 365.2425;
                    int intYrs = (int)totalYears;

                    if (totalYears >= 1)
                    {
                        sRet = string.Format("{0}aa {1}m {2}g", intYrs, dateTimeDiff.Month - 1, dateTimeDiff.Day - 1);
                    }
                    else if (dateTimeDiff.Month > 1)
                    {
                        sRet = string.Format("{0}m {1}g", dateTimeDiff.Month - 1, dateTimeDiff.Day - 1);
                    }
                    else
                    {
                        sRet = string.Format("{0}g", dateTimeDiff.Day - 1);
                    }
                }
                else
                {
                    sRet = "Indefinita";
                }

                return sRet;
            }

        }

        #endregion  Prop: Età

    }
}
