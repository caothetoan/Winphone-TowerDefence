namespace Coding4Fun.ScriptTD.Common
{
    public class ResistanceData
    {
        public enum ResistanceType : short
        {
            Damage
        }

        public ResistanceType Type;
        public string TowerId;
        public int TowerLevel;
        public float Multiplier;
    }
}
