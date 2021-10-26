using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci.Helpers
{
    public static class ScciDbHelper
    {
        public static Color GetColorFromString(string colorString)
        {

            Color Colore = default(Color);
            string sColore = colorString;
            char[] splitchar1 = ",".ToCharArray();
            char[] splitchar2 = "=".ToCharArray();

            if (sColore.LastIndexOf("[") > 0)
            {
                sColore = sColore.Substring(sColore.LastIndexOf("[") + 1, sColore.LastIndexOf("]") - sColore.LastIndexOf("[") - 1);

                if (!Information.IsDBNull(sColore) & sColore != "Empty")
                {
                    Colore = Color.FromName(sColore);
                    if (Colore.ToArgb() == 0 && Information.IsArray(sColore.Split(splitchar1)))
                    {
                        int A = 0;
                        int R = 0;
                        int G = 0;
                        int B = 0;
                        if (Information.IsNumeric(sColore.Split(splitchar1)[0].Split(splitchar2)[1])) A = Convert.ToInt32(sColore.Split(splitchar1)[0].Split(splitchar2)[1]);
                        if (Information.IsNumeric(sColore.Split(splitchar1)[1].Split(splitchar2)[1])) R = Convert.ToInt32(sColore.Split(splitchar1)[1].Split(splitchar2)[1]);
                        if (Information.IsNumeric(sColore.Split(splitchar1)[2].Split(splitchar2)[1])) G = Convert.ToInt32(sColore.Split(splitchar1)[2].Split(splitchar2)[1]);
                        if (Information.IsNumeric(sColore.Split(splitchar1)[3].Split(splitchar2)[1])) B = Convert.ToInt32(sColore.Split(splitchar1)[3].Split(splitchar2)[1]);

                        Colore = Color.FromArgb(A, R, G, B);
                    }
                }
                else
                {
                    Colore = Color.FromArgb(0, 0, 0, 0);
                }

            }

            if (Colore.ToArgb() == 0 & Information.IsNumeric(sColore))
            {
                Colore = Color.FromArgb(Convert.ToInt32(sColore));
            }

            return Colore;

        }

    }
}
