using System.Collections.Generic;

namespace Coding4Fun.ScriptTD.Common.Abstracts
{
    public abstract class Map
    {
        public string Id;
        public string FriendlyName;

        public float StartingCash;
        public int StartingLives;

        public string VictorySoundId;
        public string DefeatSoundId;
        public string WaveStartSoundId;
        public string MapStartSoundId;

		public List<SpecialCell> SpecialCells = new List<SpecialCell>();
		public List<CellCombo> CellCombos = new List<CellCombo>();
    }
}
