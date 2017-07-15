using Coding4Fun.ScriptTD.Engine.Data.Abstracts;
using Coding4Fun.ScriptTD.Engine.Logic;
using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.Engine.Data.Weapons.Ammunition
{
    internal class BeamAmmo : IAmmo
    {
        public Vector2 StartPos;
        public Vector2 EndPos;

        public float MaxRangeSq;
        public float Lifetime;
        public float AliveTime;

        public bool IsAlive; 
        public bool IsVertical;

        public Vector2 CellDrawIncrement;

        public int NumCells;
        public int Row;
        public int Column;

        public void Update(float elapsedSeconds, TowerData data, ref GameSession session) { }

        public void Reset()
        {
            IsAlive = true;
            AliveTime = 0;
        }
    }
}
