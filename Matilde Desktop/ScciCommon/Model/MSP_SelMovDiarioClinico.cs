using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnicodeSrl.Framework.Data.CustomAttributes;

namespace UnicodeSrl.Scci.Model
{
    public class MSP_SelMovDiarioClinico
    {
        public DateTime DataInserimento { get; set; }

        public DateTime DataEvento { get; set; }
        public string CodStato { get; set; }
        public String AnteprimaRTF { get; set; }

        [DataFieldIgnore]
        public string Stato
        {
            get
            {
                string codStato = this.CodStato.ToUpper();
                if (codStato == @"IC")
                    return "BOZZA";
                else
                    return "";
            }
        }

        public Guid ID { get; set; }

        public Guid IdEpisodio { get; set; }

        public String NumeroNosologico { get; set; }

        public String NumeroListaAttesa { get; set; }

        public String DescrTipoDiario { get; set; }

        public String CodUtente { get; set; }

        public String DescrUtente { get; set; }

        public Int32 PermessoValida { get; set; }

        public Int32 PermessoUAFirma { get; set; }

        public Int32 PermessoModifica { get; set; }

        public Int32 PermessoCopia { get; set; }

        public Int32 PermessoCancella { get; set; }

        public Int32 PermessoAnnulla { get; set; }

        [DataFieldIgnore]
        public String EpisodioGrid
        {
            get;
            set;
        }

    }
}
