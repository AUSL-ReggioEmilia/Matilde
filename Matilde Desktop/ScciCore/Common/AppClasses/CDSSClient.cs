using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.ScciCore
{
    [Serializable()]
    public class CDSSClient
    {

        #region Declare

        private List<Plugin> _elementi = new List<Plugin>();

        private const string K_DEF_TIPOCDSS = @"HOM";
        #endregion

        #region Costructor

        public CDSSClient(string codruolo)
        {
            this.Carica(codruolo);
        }

        #endregion

        #region Property

        public List<Plugin> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        #endregion

        #region Method

        private void Carica(string sCodRuolo)
        {

            try
            {
                FwDataBufferedList<SelCdssRuoloRow> cdsslist = new FwDataBufferedList<SelCdssRuoloRow>();

                using (FwDataConnection cnn = new FwDataConnection(Database.ConnectionString))
                {
                    cdsslist = cnn.MSP_SelCDSSClientRuolo(sCodRuolo, null, K_DEF_TIPOCDSS);
                }

                foreach (SelCdssRuoloRow item in cdsslist.Buffer)
                {

                    byte[] icon = new byte[0];
                    if (item.IconaPlugin != null)
                        icon = item.IconaPlugin as byte[];

                    Plugin plugin = new Plugin(item.CodPlugin, item.DescrizionePlugin, item.NomePlugin, item.ComandoPlugin, item.ModalitaPlugin, item.CodRuolo, item.OrdinePlugin, icon);

                    _elementi.Add(plugin);
                }

            }
            catch (Exception ex)
            {
                _elementi.Clear();
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

    }
}
