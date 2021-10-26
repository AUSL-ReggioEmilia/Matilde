using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Threading;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Framework.Diagnostics;
using System.Threading.Tasks;

namespace UnicodeSrl.ScciCore.ThreadingObj
{
    internal class WorkerRunStoredProcedure :
ICustomWorker
    {
        public string ThreadName { get; set; }
        private String ConnectionString { get; set; }
        private String XmlParam { get; set; }
        private String StoredProcedure { get; set; }
        private SendOrPostCallback DataReadCallback { get; set; }
        private SynchronizationContext Context { get; set; }
        private bool AskForDataset { get; set; }


        public WorkerRunStoredProcedure(string connString, string sproc, string xmlParam, SendOrPostCallback cb, SynchronizationContext context, bool askForDataset = false)
        {
            this.ConnectionString = connString;
            this.StoredProcedure = sproc;
            this.XmlParam = xmlParam;

            this.Context = context;
            this.DataReadCallback = cb;
            this.AskForDataset = askForDataset;

            this.ThreadName = this.StoredProcedure;
        }

        public WorkerRunStoredProcedure()
        {
        }

        public void CancelCallback(object state)
        {
            Thread t = (Thread)state;
            t.Abort();
        }

        public void OnThreadAborted(ThreadingEventArgs args)
        {

        }

        public void OnThreadCompleted(ThreadingEventArgs args)
        {

        }

        public void OnThreadException(ThreadingExceptionEventArgs args)
        {
            UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(args.Exception);
        }

        public void OnThreadStarted(ThreadingEventArgs args)
        {

        }

        public void Worker(object parameters)
        {
            if ((parameters != null) && (parameters is CancellationToken))
            {
                CancellationToken token = (CancellationToken)token;
                Worker(token);
            }
            else
                Worker(null);
        }

        public void Worker(CancellationToken token)
        {
            if (this.AskForDataset)
                workerDataset(token);
            else
                workerDatatable(token);
        }

        public async Task RunWorkerAsync()
        {
            await Task.Run
                (
                    () =>
                    {
                        if (this.AskForDataset)
                            workerDataset(CancellationToken.None);
                        else
                            workerDatatable(CancellationToken.None);
                    }
                );
        }


        private void workerDatatable(CancellationToken token)
        {
            DataTable dt = new DataTable();

            using (FwDataConnection fdc = new FwDataConnection(this.ConnectionString))
            {
                FwDataParametersList fpl = new FwDataParametersList
                {
                    { "xParametri", this.XmlParam, ParameterDirection.Input, DbType.Xml }
                };
                dt = fdc.Query<DataTable>(this.StoredProcedure, fpl, CommandType.StoredProcedure);
            }

            if (token.IsCancellationRequested)
                return;

            if (this.DataReadCallback != null)
            {
                if (this.Context != null)
                    this.Context.Send(this.DataReadCallback, dt);
                else
                    this.DataReadCallback.Invoke(dt);
            }
        }
        private void workerDataset(CancellationToken token)
        {

            DataSet ds = new DataSet();

            using (FwDataConnection fdc = new FwDataConnection(this.ConnectionString))
            {
                FwDataParametersList fpl = new FwDataParametersList
                {
                    { "xParametri", this.XmlParam, ParameterDirection.Input, DbType.Xml }
                };
                ds = fdc.Query<DataSet>(this.StoredProcedure, fpl, CommandType.StoredProcedure);

            }

            if (token.IsCancellationRequested)
                return;

            if (this.DataReadCallback != null)
            {
                if (this.Context != null)
                    this.Context.Post(this.DataReadCallback, ds);
                else
                    this.DataReadCallback.Invoke(ds);
            }
        }

    }
}
