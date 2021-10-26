using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore
{
    public static class TileContolFactory
    {

        public static ITileUserCtl CreateTileUserCtl(AppDataMarshaler appDataMarshaler, T_ScreenTileRow screenTileRow)
        {
            SelCdssRuoloRow cdssRow = screenTileRow.GetSelCdssRuoloRow(appDataMarshaler.CodRuolo);

            if (cdssRow == null) return null;

            Assembly plugAssembly = TilePluginLoader.GetTilePluginAssembly(cdssRow);

            Type t = plugAssembly.GetType(cdssRow.ComandoPlugin);

            object[] args = new object[3];
            args[0] = appDataMarshaler;
            args[1] = screenTileRow;
            args[2] = cdssRow;

            ITileUserCtl tileControl = (ITileUserCtl)Activator.CreateInstance(t, args);

            return tileControl;
        }



    }
}
