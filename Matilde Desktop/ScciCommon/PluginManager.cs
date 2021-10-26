using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.IO;

namespace UnicodeSrl.Scci.Plugin
{

    public delegate object SendDataDelegate(IScciPlugin PlugIn, Dictionary<string, object> Dictionary, out int ThreadID);
    public delegate void NotifyCallbackDelegate(NotifyCallbackEventArgs e);


    public class NotifyCallbackEventArgs
    {
        public string info;
        public bool result;
    }

    public class DataSenderServer
    {

        public NotifyCallbackDelegate NotifyCallback;

        public void SendData(IScciPlugin PlugIn, Dictionary<string, object> Dictionary)
        {

            DataSender _sender = new DataSender();
            SendDataDelegate dlg = new SendDataDelegate(_sender.SendData);

            int outParam;
            dlg.BeginInvoke(PlugIn, Dictionary, out outParam, new AsyncCallback(SendDataEnd), Dictionary);

        }

        internal void SendDataEnd(IAsyncResult ar)
        {

            AsyncResult async = (AsyncResult)ar;
            SendDataDelegate dlg = (SendDataDelegate)async.AsyncDelegate;
            int id;
            object res = (object)dlg.EndInvoke(out id, ar);

            NotifyCallbackEventArgs e = new NotifyCallbackEventArgs();

            e.info = res.ToString();
            e.result = true;
            if (res == null) res = false;

            if (NotifyCallback != null) NotifyCallback.Invoke(e);

        }
    }

    public class DataSender
    {

        public object SendData(IScciPlugin PlugIn, Dictionary<string, object> Dictionary, out int ThreadID)
        {

            object ret = null;
            ThreadID = Thread.CurrentThread.ManagedThreadId;

            ret = PlugIn.Esegui(Dictionary);

            return ret;

        }

    }

}
