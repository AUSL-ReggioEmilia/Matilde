using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnicodeSrl.DatiClinici.Common.Enums;
using UnicodeSrl.DatiClinici.DC;

namespace UnicodeSrl.ScciCore.Common.Extensions
{
    public static class ExtSchedeDati
    {
        public static DcAttributo CreateDcAttributo(string id, string valore)
        {
            DcAttributo att = new DcAttributo();
            att.ID = id;
            att.Value = valore;

            return att;
        }

        public static DcAttributo CreateDcAttributo(EnumAttributiSchedaLayout id, string valore)
        {
            return ExtSchedeDati.CreateDcAttributo(id.ToString(), valore);
        }



    }
}
