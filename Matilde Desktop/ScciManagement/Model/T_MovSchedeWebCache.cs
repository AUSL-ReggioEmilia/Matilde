using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;

namespace UnicodeSrl.ScciManagement.Model
{
    public static class MSP_MovSchedeWebCache
    {
                                                        public static T_MovSchedeWebCache MSP_MovSchedeWebCache_Last(string codScheda, string idSessione, string conn)
        {
            using (FwDataConnection fdc = new FwDataConnection(conn))
            {
                FwDataParametersList fplist = new FwDataParametersList()
                {
                    new FwDataParameter ("@codScheda", codScheda ,ParameterDirection.Input, DbType.String,20),
                    new FwDataParameter ("@idSessione", new Guid( idSessione), ParameterDirection.Input, DbType.Guid, 0),
                };

                T_MovSchedeWebCache rv = fdc.QueryFirst<T_MovSchedeWebCache>(@"MSP_MovSchedeWebCache_Last", fplist, CommandType.StoredProcedure);

                return rv;
            }
        }

                                                        public async static Task DeleteAsync(string codScheda, string idSessione, string conn)
        {
            using (FwDataConnection fdc = new FwDataConnection(conn))
            {
                FwDataParametersList fplist = new FwDataParametersList()
                {
                    new FwDataParameter ("@codScheda", codScheda ,ParameterDirection.Input, DbType.String,20),
                    new FwDataParameter ("@idSessione", new Guid( idSessione), ParameterDirection.Input, DbType.Guid, 0),
                };

                await fdc.ExecuteStoredAsync(@"MSP_MovSchedeWebCache_Delete", fplist);
            }
        }

    }


    [DataContract()]
    [ModelTable("T_MovSchedeWebCache")]
    public class T_MovSchedeWebCache : FwModelRow<T_MovSchedeWebCache>
    {
        public T_MovSchedeWebCache() : base(MyStatics.Configurazione.ConnectionString)
        {
        }

        [DataMember()]
        [ValidationKey()]
        public Decimal IDNum { get; set; }

        [DataMember()]
        [ValidationRequired()]
        public Guid IDSessione { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(20)]
        public String CodScheda { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(100)]
        public String CodUtenteUltimaModifica { get; set; }

        [DataMember()]
        [ValidationRequired()]
        public DateTime DataUltimaModifica { get; set; }

        [DataMember()]
        [ValidationRequired()]
        public String Dati { get; set; }

    }
}
