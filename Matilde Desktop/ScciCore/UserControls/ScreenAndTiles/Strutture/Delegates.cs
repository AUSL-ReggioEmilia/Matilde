using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciCore.UserControls.ScreenAndTiles
{
    public delegate void SelectTileRowDelegate(int row);

    public delegate void EpisodeEventHandler(object sender, EpisodeEventArgs args);

    public delegate void ScreenGridColumnEventHandler(object sender, ScreenGridColumnEventArgs args);


    public class EpisodeEventArgs
    {
        public EpisodeEventArgs(string idTrasferimento)
        {
            this.IdTrasferimento = idTrasferimento;
        }

        public string IdTrasferimento { get; }
    }


    public class ScreenGridColumnEventArgs
    {
        public ScreenGridColumnEventArgs(int colIndex)
        {
            this.ColumndIndex = colIndex;
        }

        public int ColumndIndex { get; private set; }
    }
}
