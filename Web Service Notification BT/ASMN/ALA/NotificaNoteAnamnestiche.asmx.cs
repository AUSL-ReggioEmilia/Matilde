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

namespace UnicodeSrl.ASMN.WebNotificationService.ALA
{
    /// <summary>
    /// Summary description for NotificaNoteAnamnestiche
    /// </summary>
    [WebService(Name = "BasicHttpBinding_IService", Namespace = "http://schemas.progel.it/WCF/Dwh/SoapNotaAnamnesticaOutput/1.0")]
    [ToolboxItem(false)]
    public class NotificaNoteAnamnestiche : INotificaNoteAnamnestiche
    {

        public void ListenData(SoapNotaAnamnesticaType value)
        {
            try
            {
                DataLog _Dl = new DataLog();
                _Dl.ConnectionString = Common.ConnString;
                _Dl.CodEvento = Common.LogDWHNotificaALA;
                _Dl.CodUtente = Common.LogUtente;

                if (_Dl.CheckEvento(Common.LogDWHNotificaALA) == true)
                {
                    string xmlString = null;
                    XmlSerializer serializer = new XmlSerializer(value.GetType());
                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, value);
                        xmlString = writer.ToString();
                    }

                    _Dl.EseguiLog(DataLog.enumTipoOperazione.Modifica, Common.LogDWHNotificaALA, "", xmlString);
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
