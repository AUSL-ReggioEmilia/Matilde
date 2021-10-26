using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnicodeSrl.Framework.Diagnostics;

namespace WsSCCI
{
    internal static class ErrorHandler
    {
                        public static void AddException(Exception ex)
        {
            bool        bUseEventLog = false;
            string      sLogPath = "";
            int         nMaxFiles = 20;
            string      connDW = "";

            DiagnosticStatics.SetDiagnosticsDefaults();

            try
            {
                bUseEventLog = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Diag_UseEvtLog"]);
                sLogPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Diag_LogPath"]);
                nMaxFiles = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Diag_LogMaxFiles"]);
                connDW = System.Configuration.ConfigurationManager.AppSettings["ConnStringDW"];
            }
            catch 
            {
            }

            DiagnosticStatics.DefDbgSettings.Application = @"WsScciMain";


            DiagnosticStatics.DefDbgSettings.SendToWebService = false;                  DiagnosticStatics.DefDbgSettings.WebServiceUrl = "";

                        DiagnosticStatics.DefDbgSettings.DbConnNameOrString = connDW;
            DiagnosticStatics.DefDbgSettings.SaveToDatabase = true;

                        DiagnosticStatics.DefDbgSettings.SaveToEventLog = bUseEventLog;

                        if (sLogPath != "")
            {
                DiagnosticStatics.DefDbgSettings.SaveToCustomLog = true;
                DiagnosticStatics.DefDbgSettings.CustomLogPath = sLogPath;
            }
            else
                DiagnosticStatics.DefDbgSettings.SaveToCustomLog = false;      
                        DiagnosticStatics.DefDbgSettings.MaxLogFiles = nMaxFiles;



                        DiagnosticStatics.AddDebugInfo(ex);

        }
    }
}