using Coding4Fun.ScriptTD.Engine.Profile;

namespace Coding4Fun.ScriptTD.Engine.Logic
{
    public static class Settings
    {
        public static bool ShowGrid;
        private static float _sfxVol;
        public static bool Mute;
        public static float SfxVolume
        {
            get
            {
                return Mute ? 0 : _sfxVol;
            }
            set
            {
                _sfxVol = value;
            }
        }
        private static float _musicVol;
        public static float MusicVolume
        {
            get
            {
                return Mute ? 0 : _musicVol;
            }
            set
            {
                _musicVol = value;
            }
        }

        public static void Save()
        {
            using (var bw = ProfileManager.CurrentProfile.Write("settings.dat"))
            {
                bw.Write(ShowGrid);
                bw.Write(_sfxVol);
                bw.Write(_musicVol);
                bw.Write(Mute);
            }
        }

        public static void Load()
        {
            if (ProfileManager.CurrentProfile.FileExists("settings.dat"))
            {
                using (var br = ProfileManager.CurrentProfile.Read("settings.dat"))
                {
                    ShowGrid = br.ReadBoolean();
                    _sfxVol = br.ReadSingle();
                    _musicVol = br.ReadSingle();
                    if (br.BaseStream.Position < br.BaseStream.Length)
                        Mute = br.ReadBoolean();
                    else
                        Mute = _sfxVol <=0;
                }
            }
            else
            {
                ShowGrid = true;
                SfxVolume = 1;
                MusicVolume = 1;
                Mute = false;
            }
        }
    }
}
