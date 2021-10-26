using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace UnicodeSrl.NotSvc
{
    internal class DataLog
    {
        public enum enumTipoOperazione
        {
            Nessuna     = 0,
            Nuovo       = 1,
            Modifica    = 2,
            Cancella    = 3
        }
        

        public string ConnectionString  { get; set; }
        public string CodUtente         { get; set; }
        public string CodEvento         { get; set; }

        public SortedList<string, string> Eventi
        {
            get 
            {
                SortedList<string, string> evts = new SortedList<string, string>();

                DataTable dt = Database.GetDatatable(@"Select * From T_Eventi Where Attivo = 1");

                foreach (DataRow dr in dt.Rows)
                {
                    evts.Add(dr["Codice"].ToString(), dr["Descrizione"].ToString());
                }

                return evts;
            }

        }

        public SortedList<string, string> Sistemi
        {
            get
            {
                SortedList<string, string> evts = new SortedList<string, string>();

                DataTable dt = Database.GetDatatable(@"Select * From T_Sistemi Where Attivo = 1");

                foreach (DataRow dr in dt.Rows)
                {
                    evts.Add(dr["Codice"].ToString(), dr["Descrizione"].ToString());
                }

                return evts;
            }

        }


        public bool CheckEvento(string codEvt )
        {
            SortedList<string, string> evts = this.Eventi;
            return evts.ContainsKey(codEvt);
        }

        public bool CheckSistema(string codSys)
        {
            SortedList<string, string> sys = this.Sistemi;
            return sys.ContainsKey(codSys);
        }

        public void EseguiLog(enumTipoOperazione TipoOperazione, String CodEvento, String xmlPrima, String xmlDopo)
        {
            string sql = @"Select * From T_MovDataLog Where 0=1";

            DataSet ds = Database.GetDataset(sql);
            DataRow dr = ds.Tables[0].NewRow();

            string[] ip = UnicodeSrl.Framework.Utility.Windows.IpAddress();

            dr["Data"]          = DateTime.Now.ToString();
            dr["DataUTC"]       = DateTime.Now.ToUniversalTime().ToString();
            dr["CodUtente"]     = this.CodUtente;
            dr["ComputerName"]  = Environment.MachineName;
            dr["IpAddress"]     = ip[0];
            dr["CodEvento"]     = this.CodEvento;
            dr["TipoOperazione"] = TipoOperazione;
            dr["Operazione"]    = TipoOperazione.ToString ();

            switch (TipoOperazione)
            {
                case enumTipoOperazione.Nessuna:
                    break;
                case enumTipoOperazione.Nuovo:
                    if (xmlDopo != "") dr["LogDopo"] = xmlDopo;
                    break;

                case enumTipoOperazione.Modifica:
                    if (xmlPrima != "") dr["LogPrima"]  = xmlPrima;
                    if (xmlDopo != "") dr["LogDopo"]    = xmlDopo;
                    break;

                case enumTipoOperazione.Cancella:
                    if (xmlPrima != "") dr["LogPrima"] = xmlPrima;
                    break;

                default:
                    break;
            }

            ds.Tables[0].Rows.Add(dr);
            Database.SaveDataset(ds, sql );

        }

    }
}