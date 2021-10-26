using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("SchedeTestataPaziente")]
    public class SchedeTestataPazienteBuffer : FwModelBuffer<SchedeTestataPaziente>
    {
        public SchedeTestataPazienteBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("SchedeTestataPaziente")]
    public class SchedeTestataPaziente : FwModelRow<SchedeTestataPaziente>
    {
        public SchedeTestataPaziente() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(27)]
        public String Sez03 { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String NomeScheda { get; set; }

        [DataMember()]
        public String AnteprimaRTF { get; set; }

        [DataMember()]
        [ValidationStringLenght(10)]
        public String DataCreazione { get; set; }

        [DataMember()]
        [ValidationStringLenght(16)]
        public String DataUltimaModifica { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String CodUtenteUltimaModifica { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String DescrUtenteUltimaModifica { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String DescrUtenteValidazione { get; set; }

        [DataMember()]
        [ValidationStringLenght(16)]
        public String DataValidazione { get; set; }

    }
}
