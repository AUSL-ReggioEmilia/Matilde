using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.CdssScreenTiles;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore.UserControls.ScreenAndTiles
{
    internal class TileColumnState
    {
        public TileColumnState(en_TileGridColumnState columnState = en_TileGridColumnState.normal, bool dataLoaded = true)
        {
            this.ColumnState = columnState;
            this.DataLoaded = dataLoaded;
        }

        internal en_TileGridColumnState ColumnState { get; set; }
        internal bool DataLoaded { get; set; }
    }
}
