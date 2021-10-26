using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{
    public static partial class FwDataConnectionExt
    {
        public static FwDataBufferedList<MSP_SelMovEvidenzaClinicaRow> MSP_SelMovEvidenzaClinica(this FwDataConnection cnn, XmlParameter xp)
        {
            FwDataBufferedList<MSP_SelMovEvidenzaClinicaRow> result = cnn.Query<FwDataBufferedList<MSP_SelMovEvidenzaClinicaRow>>("MSP_SelMovEvidenzaClinica", xp.ToFwDataParametersList(), CommandType.StoredProcedure);

            return result;
        }


    }

    [DataContract]
    public class MSP_SelMovEvidenzaClinicaRow
    {
        public MSP_SelMovEvidenzaClinicaRow()
        { }

        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public Guid? IDPaziente { get; set; }

        [DataMember]
        public Guid? IDEpisodio { get; set; }

        [DataMember]
        public Guid? IDTrasferimento { get; set; }

        [DataMember]
        public String IDRefertoDWH { get; set; }

        [DataMember]
        public DateTime? DataEvento { get; set; }

        [DataMember]
        public DateTime? DataEventoDWH { get; set; }

        [DataMember]
        public String Anteprima { get; set; }

        [DataMember]
        public String CodTipo { get; set; }

        [DataMember]
        public String DescrTipo { get; set; }

        [DataMember]
        public String CodStato { get; set; }

        [DataMember]
        public String DescrStato { get; set; }

        [DataMember]
        public String CodStatoVisione { get; set; }

        [DataMember]
        public String DescrStatoVisione { get; set; }

        [DataMember]
        public int EsistePDFDWH { get; set; }

        [DataMember]
        public int PermessoVista { get; set; }

        [DataMember]
        public int PermessoGrafico { get; set; }

        [DataMember]
        public int PermessoCancella { get; set; }

        [DataMember]
        public System.Decimal? IDIcona { get; set; }

        [DataMember]
        [DataFieldIgnore]
        public byte[] Image { get; set; }

        [DataMember]
        [DataFieldIgnore]
        public string SistemaEroganteDWH { get; set; }

        [DataMember]
        [DataFieldIgnore]
        public string NumeroRefertoDWH { get; set; }

        [DataMember]
        [DataFieldIgnore]
        public string DWHCodRepartoRichiedente { get; set; }

        [DataMember]
        [DataFieldIgnore]
        public string DWHDescRepartoRichiedente { get; set; }

        public void FromRefertoDWH(RefertoDWH referto, String idPaziente)
        {
            this.IDPaziente = new Guid(idPaziente);
            this.IDEpisodio = null;

            this.IDRefertoDWH = referto.IDReferto;
            this.DataEvento = referto.DataReferto;
            this.Anteprima = referto.TestoAnteprima;

            this.CodTipo = referto.CodTipoEvidenzaClinica;
            this.DescrTipo = referto.DescTipoEvidenzaClinica;

            this.CodStato = referto.CodStatoEvidenzaClinica;
            this.DescrStato = referto.DescStatoEvidenzaClinica;

            this.CodStatoVisione = "";
            this.DescrStatoVisione = "";

            this.PermessoVista = 0;
            this.PermessoGrafico = 0;

            if (referto.CodTipoEvidenzaClinica.Trim().ToUpper() == EnumCodTipoEvidenzaClinica.LAB.ToString().ToUpper())
                this.PermessoGrafico = 1;

            this.IDIcona = this.getIDIcona();

            this.PermessoCancella = 0;

            this.EsistePDFDWH = 1;

            this.SistemaEroganteDWH = referto.SistemaErogante;
            this.NumeroRefertoDWH = referto.NumeroReferto;

            this.DataEventoDWH = referto.DataEventoDWH;

            this.DWHCodRepartoRichiedente = referto.DWHCodRepartoRichiedente;
            this.DWHDescRepartoRichiedente = referto.DWHDescRepartoRichiedente;


        }

        private int getIDIcona()
        {

            try
            {
                XmlParameter xp = new XmlParameter();
                xp.AddParameter("CodEntita", EnumEntita.EVC.ToString());
                xp.AddParameter("CodTipo", this.CodTipo);
                xp.AddParameter("CodStato", this.CodStato);
                xp.AddParameter("DatiEstesi", "0");

                using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
                {
                    DataTable dtIcon = fdc.Query<DataTable>("MSP_SelIcona", xp.ToFwDataParametersList(), CommandType.StoredProcedure);

                    if (dtIcon != null && dtIcon.Rows.Count > 0 && !dtIcon.Rows[0].IsNull("IDNum"))
                    {
                        return Convert.ToInt32(dtIcon.Rows[0]["IDNum"]);
                    }

                    fdc.Close();
                }

            }
            catch (Exception)
            {
            }

            return 0;
        }

    }
}
