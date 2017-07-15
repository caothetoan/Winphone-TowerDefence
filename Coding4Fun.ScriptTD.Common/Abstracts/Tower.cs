namespace Coding4Fun.ScriptTD.Common.Abstracts
{
    public abstract class Tower
    {
        public string TowerId;
        public int TowerLevel;

        public int Cost;
        public float ReloadTime;
        public float BuildTime;
        public float SellTime;

        public bool CanShootFlyers;
        public bool CanShootLand;

        public float MinRange;
        public float MaxRange;
        public float DPS;
        public float ShotSpeed;
        
        public string WeaponName;

        public string ShotSoundId;
        public string BuildSoundId;
        public string SellSoundId;
        public string UpgradeSoundId;
        public string HitSoundId;

        public string HitTexturePath;
        public float AnimationFps;
    }
}
