using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci.Statics
{
    public static class CommonConstants
    {

        public const string C_DESC_PRI_PROGRAMMATA = "Programmata";
        public const string C_DESC_PRI_NONDEFINITA = "Non Definita";
        public const string C_DESC_PRI_ORDINARIA = "Ordinaria";
        public const string C_DESC_PRI_URGENTEDIFFERIBILE = "Urgente Differibile";
        public const string C_DESC_PRI_URGENTE = "Urgente";
        public const string C_DESC_PRI_CRITICA = "Urgente 2H";

        public const string C_ALLEGATO_TAG_START = @"[Allegato Nr. ";
        public const string C_ALLEGATO_TAG_END = @"]";

        public const string C_SCCI_CHANNEL = @"SCCI_C1@{0}";

        public const string C_CONTATORE_CARTELLA = @"CONCAR";

    }
}
