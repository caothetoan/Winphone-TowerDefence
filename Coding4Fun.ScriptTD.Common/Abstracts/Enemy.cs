using System.Collections.Generic;
using Coding4Fun.ScriptTD.Common;

namespace Coding4Fun.ScriptTD.Engine.Data.Abstracts
{
    public abstract class Enemy
    {
        // Key = TowerId + TowerLevel
        public Dictionary<string, ResistanceData> Resistances = new Dictionary<string,ResistanceData>();

        public string Id;

        public float TotalHealth;
        public float TotalSpeed;

        public bool CanFly;

        public float TotalWorth;

        public string DeathSoundId;
        public string SpawnSoundId;
    }
}
