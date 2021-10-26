using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("TaskInfermierisrtici")]
    public class TaskInfermierisrticiBuffer : FwModelBuffer<TaskInfermierisrtici>
    {
        public TaskInfermierisrticiBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("TaskInfermierisrtici")]
    public class TaskInfermierisrtici : FwModelRow<TaskInfermierisrtici>
    {
        public TaskInfermierisrtici() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(26)]
        public String Sez11 { get; set; }

        [DataMember()]
        [ValidationStringLenght(60)]
        public String DescrDate { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(14)]
        public String TestoProgrammazione { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(100)]
        public String UtenteProgrammatore { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(26)]
        public String DataProgrammata { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String TipoTaskInfermieristico { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String Stato { get; set; }

        [DataMember()]
        public String AnteprimaRTF { get; set; }

        [DataMember()]
        [ValidationRequired()]
        public Int32 FlagEsteso { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(14)]
        public String TestoErogazioneAnnullamento { get; set; }

        [DataMember()]
        [ValidationStringLenght(26)]
        public String DataErogazioneAnnullamento { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String UtenteEsecutoreAnnullatore { get; set; }

        [DataMember()]
        [ValidationStringLenght(6319)]
        public String DescrProgrammazioneErogazione { get; set; }

        [DataMember()]
        [ValidationStringLenght(4000)]
        public String Note { get; set; }

    }
}
