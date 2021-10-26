using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciManagement
{
    public partial class PUDataBindings
    {

        #region Declare

        private UnicodeSrl.Sys.Data2008.DataBindings _DataBinds = new UnicodeSrl.Sys.Data2008.DataBindings();
        private UnicodeSrl.Sys.Data2008.SqlStruct _SqlSelect = new UnicodeSrl.Sys.Data2008.SqlStruct();
        private UnicodeSrl.Sys.Data2008.SqlStruct _SqlDelete = new UnicodeSrl.Sys.Data2008.SqlStruct();

        #endregion

        #region Property

        public UnicodeSrl.Sys.Data2008.DataBindings DataBindings
        {
            get { return _DataBinds; }
            set { _DataBinds = value; }
        }

        public UnicodeSrl.Sys.Data2008.SqlStruct SqlSelect
        {
            get { return _SqlSelect; }
            set { _SqlSelect = value; }
        }

        public UnicodeSrl.Sys.Data2008.SqlStruct SqlDelete
        {
            get { return _SqlDelete; }
            set { _SqlDelete = value; }
        }

        public DataSet DsLogPrima { get; set; }

        public DataSet DsLogDopo { get; set; }

        #endregion

    }
}
