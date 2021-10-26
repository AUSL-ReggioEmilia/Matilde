using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

namespace UnicodeSrl.Scci.PluginClient
{

    public class DataBindingsForAzioni : IDisposable
    {

        public DataBindingsForAzioni()
        {

            DataMember = string.Empty;
            DataSource = null;

            CodUAMember = string.Empty;
            CodAzioneMember = string.Empty;
            DescrizioneAzioneMember = string.Empty;
            CodPluginMember = string.Empty;
            DescrizionePluginMember = string.Empty;
            NomePluginMember = string.Empty;
            DescrizionePluginMember = string.Empty;
            ComandoPluginMember = string.Empty;
            ModalitaPluginMember = string.Empty;
            IconaPluginMember = string.Empty;

            Azioni = new Azioni();

        }

        public string DataMember { get; set; }
        public object DataSource { get; set; }

        public string CodUAMember { get; set; }
        public string CodAzioneMember { get; set; }
        public string DescrizioneAzioneMember { get; set; }
        public string CodPluginMember { get; set; }
        public string DescrizionePluginMember { get; set; }
        public string NomePluginMember { get; set; }
        public string ComandoPluginMember { get; set; }
        public string ModalitaPluginMember { get; set; }
        public string OrdinePluginMember { get; set; }
        public string IconaPluginMember { get; set; }

        public Azioni Azioni { get; set; }

        public void SetDataBinding(object datasource, string datamember)
        {

            DataSource = datasource;
            DataMember = datamember;

            Azione oAzione = null;
            Plugin oPlugin = null;
            DataTable dt = null;

            Azioni = new Azioni();

            try
            {

                if (DataSource != null)
                {

                    if (DataSource.GetType() == typeof(DataTable))
                    {
                        dt = (DataTable)DataSource;
                    }
                    else if (DataSource.GetType() == typeof(DataSet))
                    {
                        if (DataMember != string.Empty)
                        {
                            dt = ((DataSet)DataSource).Tables[DataMember];
                        }
                        else
                        {
                            dt = ((DataSet)DataSource).Tables[0];
                        }
                    }

                    foreach (DataRow dr in dt.Rows)
                    {

                        if (Azioni.ContainsKey(dr[CodAzioneMember].ToString()) == false)
                        {
                            oAzione = new Azione(dr[CodAzioneMember].ToString(), dr[DescrizioneAzioneMember].ToString());
                            Azioni.Add(oAzione.Codice, oAzione);
                        }

                        oPlugin = new Plugin(dr[CodPluginMember].ToString(), dr[DescrizionePluginMember].ToString(),
                                                dr[NomePluginMember].ToString(), dr[ComandoPluginMember].ToString(),
                                                dr[ModalitaPluginMember].ToString(), dr[CodUAMember].ToString(),
                                                int.Parse(dr[OrdinePluginMember].ToString()),
                                                (dr.IsNull(IconaPluginMember) ? null : (byte[])dr[IconaPluginMember]));
                        Azioni[dr[CodAzioneMember].ToString()].Plugins.Add(oPlugin);

                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public void Dispose()
        {
            if (DataSource != null)
            {
                DataSource = null;
            }
        }

    }

}
