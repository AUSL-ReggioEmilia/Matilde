using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnicodeSrl.Framework.Diagnostics;
using UnicodeSrl.Framework.DiagWcfSvc;

namespace UnicodeSrl.NotSvc
{
    internal static class ErrorHandler
    {
        // Invia una exception a log.
        // Utilizza tutti i parametri di default
        public static void AddException(Exception ex)
        {
            bool bUseEventLog = false;
            string sLogPath = "";
            int nMaxFiles = 20;

            Statics.SetDiagnosticsDefaults();

            try
            {
                bUseEventLog = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Diag_UseEvtLog"]);
                sLogPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Diag_LogPath"]);
                nMaxFiles = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Diag_LogMaxFiles"]);
            }
            catch
            {
            }

            DiagnosticStatics.DefDbgSettings.Application = @"NotificationWebSvc";


            DiagnosticStatics.DefDbgSettings.SaveToCustomLog = false;
            DiagnosticStatics.DefDbgSettings.SaveToEventLog = false;
            DiagnosticStatics.DefDbgSettings.SendToWebService = true;
            DiagnosticStatics.DefDbgSettings.WebServiceUrl = System.Configuration.ConfigurationManager.AppSettings["WS_DIAG_URL"];


            // Event Log
            Statics.DefDbgSettings.SaveToEventLog = bUseEventLog;

            // Log Files
            if (sLogPath != "")
            {
                Statics.DefDbgSettings.SaveToCustomLog = true;
                Statics.DefDbgSettings.CustomLogPath = sLogPath;
            }

            // Manutebnzioni
            Statics.DefDbgSettings.MaxLogFiles = nMaxFiles;


            if (Statics.DefDbgSettings.WebServiceUrl == "") Statics.DefDbgSettings.SendToWebService = false;

            // Send
            DiagnosticStatics.AddDebugInfo(ex);

        }
    }
}