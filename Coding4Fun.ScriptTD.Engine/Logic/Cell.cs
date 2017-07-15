using Coding4Fun.ScriptTD.Common;
using Coding4Fun.ScriptTD.Engine.Logic.Instances;

namespace Coding4Fun.ScriptTD.Engine.Logic
{
    public class Cell
    {
        public string Id;

        public SpecialCellType CellType = SpecialCellType.None;

        public int X, Y;

        public bool IsLandObstruction
        {
            get { return CellType == SpecialCellType.Obstruction || Tower != null; }
        }

        public bool Buildable;

        public bool InvalidCell;

        public TowerInstance Tower;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
            Buildable = true;
        }
    }
}
