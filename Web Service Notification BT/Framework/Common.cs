using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnicodeSrl.NotSvc
{
    public static class Common
    {
        public static string ConnString
        {
            get
            {
                Encryption enc = new Encryption();
                string s = enc.DecryptString(System.Web.Configuration.WebConfigurationManager.AppSettings["ConnStr"].ToString());

                return s;
            }
        }

        public static string LogSACNotificaPaziente
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["LogSACNotificaPaziente"].ToString();
            }
        }

        public static string LogOENotificaRichiesta
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["LogOENotificaRichiesta"].ToString();
            }
        }

        public static string LogOENotificaStato
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["LogOENotificaStato"].ToString();
            }
        }

        public static string LogDWHNotificaADT
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["LogDWHNotificaADT"].ToString();
            }
        }

        public static string LogDWHNotificaALA
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["LogDWHNotificaALA"].ToString();
            }
        }

        public static string LogDWHNotificaReferti
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["LogDWHNotificaReferti"].ToString();
            }
        }

        public static string LogDWHNotificaRefertiSalo
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["LogDWHNotificaRefertiSalo"].ToString();
            }
        }

        public static string SistemaEroganteRefertiSalo
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["SistemaEroganteRefertiSalo"].ToString();
            }
        }

        public static string LogDWHNotificaRefertiFenix
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["LogDWHNotificaRefertiFenix"].ToString();
            }
        }

        public static string SistemaEroganteRefertiFenix
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["SistemaEroganteRefertiFenix"].ToString();
            }
        }

        public static string LogUtente
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["LogUtente"].ToString();
            }
        }



        internal static string SQLString(string vsString)
        {
            return vsString.Replace(@"'", @"''");
        }

        internal static string SQLDate(DateTime vDate)
        {
            string sRet = vDate.ToString(@"yyyy/MM/dd");
            sRet = @"Convert(DateTime,'" + sRet + @"',120)";
            return sRet;
        }

        internal static string SQLDateTime(DateTime vDate)
        {
            string sRet = "";

            if (vDate != DateTime.MinValue)
            {
                sRet = vDate.ToString(@"yyyy/MM/dd HH:mm:ss").Replace(@".", @":");
                sRet = @"Convert(DateTime,'" + sRet + @"',120)";
            }
            else
            {
                DateTime min = new DateTime(1900, 1, 1);
                sRet = min.ToString(@"yyyy/MM/dd HH:mm:ss").Replace(@".", @":");
                sRet = @"Convert(DateTime,'" + sRet + @"',120)";
            }

            return sRet;
        }

    }
}