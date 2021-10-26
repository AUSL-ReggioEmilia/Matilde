using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data;

namespace UnicodeSrl.Scci
{
    public static class ExtMigration
    {

        public static void FromSqlParameterCollection(this FwDataParametersList plist, SqlParameter[] coll)
        {
            plist.Clear();

            foreach (SqlParameter item in coll)
            {
                FwDataParameter p = new FwDataParameter();
                p.FromDataParameter(item);
                plist.Add(p);
            }
        }

        public static void FromSqlParameterExt(this FwDataParametersList plist, SqlParameterExt[] coll)
        {
            plist.Clear();

            if (coll == null)
                return;

            foreach (SqlParameterExt item in coll)
            {
                FwDataParameter p = new FwDataParameter();
                p.FromDataParameter(item.SQLParam);
                plist.Add(p);
            }
        }
    }
}
