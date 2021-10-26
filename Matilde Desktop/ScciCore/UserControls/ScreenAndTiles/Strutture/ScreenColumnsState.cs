using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.CdssScreenTiles;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore.UserControls.ScreenAndTiles
{
    internal class ScreenColumnsState
    {
        public ScreenColumnsState()
        {
            this.Columns = new Dictionary<String, TileColumnState>();
        }

        private Dictionary<String, TileColumnState> Columns { get; set; }

        public bool Exist(T_ScreenTileRow tileObject)
        {
            string key = this.getColumnKeyFromObj(tileObject);

            bool exist = this.Columns.TryGetValue(key, out TileColumnState tileColumnState);

            return exist;
        }

        public void Add(T_ScreenTileRow tileObject, en_TileGridColumnState columnState = en_TileGridColumnState.normal, bool dataLoaded = true)
        {
            string key = this.getColumnKeyFromObj(tileObject);
            this.Columns.Add(key, new TileColumnState(columnState, dataLoaded));
        }

        public void Update(T_ScreenTileRow tileObject, en_TileGridColumnState? columnState = null, bool? dataLoaded = null)
        {
            string key = this.getColumnKeyFromObj(tileObject);

            if (columnState.HasValue)
                this.Columns[key].ColumnState = columnState.Value;

            if (dataLoaded.HasValue)
                this.Columns[key].DataLoaded = dataLoaded.Value;
        }

        public void AddOrUpdate(T_ScreenTileRow tileObject, en_TileGridColumnState columnState = en_TileGridColumnState.normal, bool dataLoaded = true)
        {
            string key = this.getColumnKeyFromObj(tileObject);

            bool exist = this.Columns.TryGetValue(key, out TileColumnState tileColumnState);

            if (exist == false)
            {
                this.Columns.Add(key, new TileColumnState(columnState, dataLoaded));
            }
            else
            {
                this.Columns[key].ColumnState = columnState;
                this.Columns[key].DataLoaded = dataLoaded;
            }
        }

        public TileColumnState GetTileColumnState(T_ScreenTileRow tileObject)
        {
            string key = this.getColumnKeyFromObj(tileObject);

            return this.Columns[key];
        }

        public void Clear()
        {
            this.Columns.Clear();
        }

        private string getColumnKeyFromObj(T_ScreenTileRow tileObject)
        {
            if (tileObject == null) return null;

            string key = $"{tileObject.CodScreen}@";
            if (tileObject.Fissa == true)
                key += "FIX@";
            else
                key += "MOV@";

            key += $"{tileObject.Colonna}";

            return key;
        }


    }

}
