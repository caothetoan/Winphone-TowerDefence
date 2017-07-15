using System.Collections.Generic;
using Coding4Fun.ScriptTD.Engine.Data;
using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.Engine.Logic
{
    public class Grid
    {
        public Cell this[int indexX, int indexY]
        {
            get { return CellArray[(indexY * Columns) + indexX]; }
        }

        public Cell[] CellArray;
        public int Columns;
        public int Rows;

        public readonly Dictionary<string, Cell> SpecialCells = new Dictionary<string, Cell>();

        public Vector2 TopLeft;
        public float CellSize;

        public Grid(int cellsX, int cellsY)
        {
            Columns = cellsX;
            Rows = cellsY;
            CellArray = new Cell[cellsX * cellsY];

            for (int i = 0; i < cellsX * cellsY; i++)
            {
                CellArray[i] = new Cell(i % cellsX, i / cellsX);
            }
        }

        public void ApplyMap(ref MapData map)
        {
            for (int i = 0; i < map.SpecialCells.Count; i++)
            {
                var cell = this[map.SpecialCells[i].X, map.SpecialCells[i].Y];
                if (!string.IsNullOrEmpty(map.SpecialCells[i].Id))
                {
                    cell.Id = map.SpecialCells[i].Id;
                    cell.CellType = map.SpecialCells[i].CellType;
                    cell.Buildable = map.SpecialCells[i].Buildable;
                    SpecialCells.Add(cell.Id, cell);
                }
                else
                {
                    cell.Buildable = map.SpecialCells[i].Buildable;
                    cell.CellType = map.SpecialCells[i].CellType;
                }
            }
        }

        public bool IsCellValid(int cellX, int cellY)
        {
            return (cellX >= 0 && cellX < Columns) && (cellY >= 0 && cellY < Rows);
        }

        public Vector2 GetCellCenter(int cellX, int cellY)
        {
            float halfCell = CellSize / 2f;
            return TopLeft + new Vector2((cellX * CellSize) + halfCell, (cellY * CellSize) + halfCell);
        }

        public Vector2 GetCellCenter(ref Cell cell)
        {
            float halfCell = CellSize / 2f;
            return TopLeft + new Vector2((cell.X * CellSize) + halfCell, (cell.Y * CellSize) + halfCell);
        }

        public void GetCell(Vector2 position, out int x, out int y)
        {
            var p = position - TopLeft;
            x = (int)(p.X / CellSize);
            y = (int)(p.Y / CellSize);
        }
    }
}
