using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using UnicodeSrl.NotSvc;


namespace UnicodeSrl.ASMN.WebNotificationService.DWH
{
    /// <summary>
    /// Summary description for NotificaReferti
    /// </summary>
    [WebService(Name = "BasicHttpBinding_IService", Namespace = "http://schemas.progel.it/WCF/Dwh/SoapRefertoOutput/1.0")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class NotificaReferti : INotificaReferti
    {

        public void ListenData(SoapRefertoType value)
        {
            try
            {
                DataLog _Dl = new DataLog();
                _Dl.ConnectionString = Common.ConnString;
                _Dl.CodEvento = Common.LogDWHNotificaReferti;
                _Dl.CodUtente = Common.LogUtente;

                if (_Dl.CheckEvento(Common.LogDWHNotificaReferti) == true)
                {
                    string xmlString = null;
                    XmlSerializer serializer = new XmlSerializer(value.GetType());
                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, value);
                        xmlString = writer.ToString();
                    }
                    _Dl.EseguiLog(DataLog.enumTipoOperazione.Modifica, Common.LogDWHNotificaReferti, "", xmlString);
                    
                    if (value.Referto.SistemaErogante == Common.SistemaEroganteRefertiSalo)
                    {
                        // se l'evento di referto deriva da salonet inseriamo nuovamente l'evento 
                        // con codice DWHREF002 per la creazione automatica di schede da referto si dala
                        _Dl.CodEvento = Common.LogDWHNotificaRefertiSalo;
                        _Dl.EseguiLog(DataLog.enumTipoOperazione.Modifica, Common.LogDWHNotificaRefertiSalo, "", xmlString);
                    }
                    else if (value.Referto.SistemaErogante == Common.SistemaEroganteRefertiFenix)
                    {
                        // se l'evento di referto deriva da FENIX inseriamo nuovamente l'evento 
                        // con codice DWHREF003 per la creazione automatica di schede da referto di fenix
                        _Dl.CodEvento = Common.LogDWHNotificaRefertiFenix;
                        _Dl.EseguiLog(DataLog.enumTipoOperazione.Modifica, Common.LogDWHNotificaRefertiFenix, "", xmlString);
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                throw new SoapException(ex.Message, SoapException.ServerFaultCode, ex);
            }
        }
    }
}
