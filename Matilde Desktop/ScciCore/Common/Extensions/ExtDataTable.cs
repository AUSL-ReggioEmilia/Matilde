using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciCore.Common.Extensions
{
    public static class ExtDataTable
    {
        public static bool IsEqual(this DataTable tbl1, DataTable tbl2, List<String> ignoreColumns)
        {
            if ((tbl1 == null) || (tbl2 == null))
                return false;

            if (tbl1.Rows.Count != tbl2.Rows.Count || tbl1.Columns.Count != tbl2.Columns.Count)
                return false;

            List<int> exclusions = new List<int>();

            if (ignoreColumns != null)
            {
                foreach (string colkey in ignoreColumns)
                {
                    int idx = tbl1.Columns.IndexOf(colkey);
                    exclusions.Add(idx);
                }
            }

            for (int i = 0; i < tbl1.Rows.Count; i++)
            {
                for (int c = 0; c < tbl1.Columns.Count; c++)
                {
                    if (exclusions.Exists(p => p == c) == false)
                    {
                        if (Equals(tbl1.Rows[i][c], tbl2.Rows[i][c]) == false)
                            return false;
                    }

                }
            }
            return true;
        }


    }
}
