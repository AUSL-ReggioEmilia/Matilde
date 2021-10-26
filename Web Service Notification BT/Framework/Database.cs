using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using UnicodeSrl.Framework.Data;



namespace UnicodeSrl.NotSvc
{
    internal static class Database
    {

        internal async static Task< DataTable> GetDatatableAsync(string sql)
        {
            try
            {
                SqlDataObject sdo = new SqlDataObject();
                sdo.ConnectionString = Common.ConnString;
                sdo.Sql = sql;

                DataTable dt = await sdo.GetDataTableAsync();                

                return dt;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                return null;
            }
        }

        internal async static Task<DataSet> GetDatasetAsync(string sql)
        {
            try
            {
                SqlDataObject sdo = new SqlDataObject();
                sdo.ConnectionString = Common.ConnString;
                sdo.Sql = sql;

                DataSet ds = await sdo.GetDataAsync();

                return ds;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                return null;
            }
        }


        internal static DataTable GetDatatable(string sql)
        {
            try
            {
                SqlDataObject sdo = new SqlDataObject();
                sdo.ConnectionString = Common.ConnString;
                sdo.Sql = sql;

                DataTable dt = sdo.GetDataTable();

                return dt;

            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                return null;
            }
        }

        internal static DataSet GetDataset(string sql)
        {
            try
            {
                SqlDataObject sdo = new SqlDataObject();
                sdo.ConnectionString = Common.ConnString;
                sdo.Sql = sql;

                DataSet ds =  sdo.GetData();

                return ds;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                return null;
            }
        }

        internal static bool SaveDataset(DataSet ds , string sql)
        {
            try
            {
                SqlDataObject sdo = new SqlDataObject();
                sdo.ConnectionString = Common.ConnString;
                sdo.Sql = sql;

                sdo.SaveData(ds);

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
                return false ;
            }
        }
    }
}