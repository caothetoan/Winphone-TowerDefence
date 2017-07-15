using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.Common.Abstracts
{
    public abstract class WavePart
    {
        public int TotalEnemies;

        public float StartTimeOffset;
        public float TimeBetweenSpawns;

        public string StartCell;
        public string EndCell;

        public Vector2 SpawnPoint;
        public Vector2 DespawnPoint;
    }
}
