using System;
using System.Collections.Generic;
using System.Linq;
using Coding4Fun.ScriptTD.Engine.Logic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Coding4Fun.ScriptTD.Engine
{
    public static class Audio
    {
        private static Dictionary<string, SoundEffect> _sfx = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, List<Song>> _playlists = new Dictionary<string, List<Song>>();
        private static List<Song> _curPlaylist;
        private static int _curTrack;

        private static List<SoundEffectInstance> _playing = new List<SoundEffectInstance>();
        private static List<SoundEffectInstance> _toremove = new List<SoundEffectInstance>();

        private static ContentManager _content;
        private static float _musicVol;
        private static float _sfxVol;

        private static bool _isPlaying = true;

        public const string MusicPath = @"Audio\Music\";
        public const string SfxPath = @"Audio\SFX\";

        public static void Initialize(IServiceProvider services)
        {
            _content = new ContentManager(services, "Content");
            _musicVol = Settings.MusicVolume;
            _sfxVol = Settings.SfxVolume;
        }

        public static bool RegisterSfx(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            try
            {
                if (_sfx.ContainsKey(name))
                    return true;

                var m = _content.Load<SoundEffect>(SfxPath + name);
                _sfx.Add(name, m);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void PlaySfx(string name)
        {
            if (_sfx.ContainsKey(name))
            {
                var inst = _sfx[name].CreateInstance();
                inst.Volume = _sfxVol;
                _playing.Add(inst);
                try
                {
                    inst.Play();
                    if (!_isPlaying)
                        inst.Pause();
                }
                catch (InstancePlayLimitException)
                {
                    _playing.Remove(inst);
                }
            }
        }

        public static bool RegisterPlayList(string name, ref List<string> playlist)
        {
            try
            {
                List<Song> pl = new List<Song>(playlist.Count);

            	pl.AddRange(playlist.Select(t => _content.Load<Song>(MusicPath + t)));

            	_playlists.Add(name, pl);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void PlayPlaylist(string name, bool immediate)
        {
            if (!_playlists.ContainsKey(name) || !MediaPlayer.GameHasControl)
                return;

            if (immediate)
                StopPlaylist();

            _curPlaylist = _playlists[name];
            _curTrack = _curPlaylist.Count;
        }

        public static void StopPlaylist()
        {
            MediaPlayer.Stop();
            _curPlaylist = null;
        }

        public static void Update()
        {
            bool sfxChanged = false;
            if (Settings.MusicVolume != _musicVol)
            {
                _musicVol = Settings.MusicVolume;
                MediaPlayer.Volume = _musicVol;
            }

            if (Settings.SfxVolume != _sfxVol)
            {
                _sfxVol = Settings.SfxVolume;
                sfxChanged = true;
            }

            foreach (var effect in _playing)
            {
            	if (effect.State == SoundState.Stopped)
            	{
            		_toremove.Add(effect);
            	}
            	else if (sfxChanged)
            	{
            		effect.Volume = _sfxVol;
            	}
            }
        	
			foreach (var effect in _toremove)
        	{
        		_playing.Remove(effect);
        	}

        	_toremove.Clear();

            if (MediaPlayer.GameHasControl && (MediaPlayer.State == MediaState.Paused || MediaPlayer.State == MediaState.Stopped) && _curPlaylist != null && _isPlaying)
            {
                _curTrack = (_curTrack + 1) % _curPlaylist.Count;
                MediaPlayer.Play(_curPlaylist[_curTrack]);
            }
        }

        public static void PauseAll()
        {
        	foreach (var effect in _playing)
        	{
        		effect.Pause();
        	}

        	_isPlaying = false;
        }

    	public static void ResumeAll()
    	{
    		foreach (var effect in _playing)
    		{
				effect.Resume();
    		}

    		_isPlaying = true;
    	}

    	public static void UnloadContent()
        {
            if (_content != null)
                _content.Unload();

            _sfx.Clear();
            _playlists.Clear();
        }

        public static void KillAllSfx()
        {
        	foreach (var effect in _playing)
        	{
        		effect.Stop(true);
        	}
        }
    }
}
