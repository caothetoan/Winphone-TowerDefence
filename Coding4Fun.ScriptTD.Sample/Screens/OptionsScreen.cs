using System;
using Coding4Fun.ScriptTD.Engine.GUI;
using Coding4Fun.ScriptTD.Engine.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Coding4Fun.ScriptTD.Common.GUI;

namespace Coding4Fun.ScriptTD.Sample.Screens
{
    public class OptionsScreen : IScreen
    {
        public const string Name = "options";

        public ScreenManager Manager { get; private set; }

        public bool IsLoaded { get; private set; }

        public bool IsInitialized { get; private set; }

        private Window _gui;
        private RenderEngine _renderer;

        private BackgroundLoader _bg;
        private SpriteBatch _sb;

        public IScreen PrevScreen;

        private bool _backDown;

        public void Initialize(ScreenManager manager)
        {
            Manager = manager;
            _bg = new BackgroundLoader(manager.Services);
            IsInitialized = true;
        }

        public void LoadGraphics(GraphicsDevice device)
        {
            _bg.BeginLoad(c =>
                {
                    _gui = c.Load<Window>(@"Data\GUI\Options");
                    RegisterInput();

                    if (_gui.Controls.ContainsKey("back"))
                        _gui.Controls["back"].Tapped += (s, v) => { if (PrevScreen != null) Manager.TransitionTo(PrevScreen); };

                    _renderer = new RenderEngine();
                    _gui.RegisterVisuals(_renderer);
                    _renderer.LoadGraphics(c, device);

                    _sb = new SpriteBatch(device);

                    IsLoaded = true;
                });
        }

        private void RegisterInput()
        {
            if (_gui.Controls.ContainsKey("showgrid") && _gui.Controls["showgrid"] is CheckBox)
                _gui.Controls["showgrid"].Tapped += (c, v) =>
                    {
                        Settings.ShowGrid = ((CheckBox)c).Checked;
                        RefreshOptions();
                    };

            if (_gui.Controls.ContainsKey("sfxdown"))
            {
                Action<IControl, Vector2> m = (c, v) =>
                {
                    Settings.SfxVolume = Settings.SfxVolume <= 0 ? 0 : Settings.SfxVolume - (1f / 100f);
                    RefreshOptions();
                };
                _gui.Controls["sfxdown"].Tapped += m;
                _gui.Controls["sfxdown"].DoubleTapped += m;
            }

            if (_gui.Controls.ContainsKey("sfxup"))
            {
                Action<IControl, Vector2> m = (c, v) =>
                {
                    Settings.SfxVolume = Settings.SfxVolume >= 1 ? 1 : Settings.SfxVolume + (1f / 100f);
                    RefreshOptions();
                };
                _gui.Controls["sfxup"].Tapped += m;
                _gui.Controls["sfxup"].DoubleTapped += m;
            }

            if (_gui.Controls.ContainsKey("musicdown"))
            {
                Action<IControl, Vector2> m = (c, v) =>
                {
                    Settings.MusicVolume = Settings.MusicVolume <= 0 ? 0 : Settings.MusicVolume - (1f / 100f);
                    RefreshOptions();
                };
                _gui.Controls["musicdown"].Tapped += m;
                _gui.Controls["musicdown"].DoubleTapped += m;
            }

            if (_gui.Controls.ContainsKey("musicup"))
            {
                Action<IControl, Vector2> m = (c, v) =>
                {
                    Settings.MusicVolume = Settings.MusicVolume >= 1 ? 1 : Settings.MusicVolume + (1f / 100f);
                    RefreshOptions();
                };
                _gui.Controls["musicup"].Tapped += m;
                _gui.Controls["musicup"].DoubleTapped += m;
            }

            IControl muteControl;
            if (_gui.Controls.TryGetValue("mute", out muteControl))
            {
                if (muteControl is CheckBox)
                {
                    muteControl.Tapped += (c, v) =>
                        {
                            Settings.Mute = ((CheckBox)c).Checked;
                            RefreshOptions();
                        };
                }
            }
        }

        public void UnloadContent()
        {
            _bg.UnloadContent();
        }

        public void Update(float elapsedSeconds)
        {
            _gui.UpdateInput();
            _gui.Update(elapsedSeconds);
            _renderer.Update(elapsedSeconds);

            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
            {
                _backDown = true;
            }
            else if (_backDown)
            {
                _backDown = false;
                if (PrevScreen != null)
                    Manager.TransitionTo(PrevScreen);
            }
        }

        public void Draw(GraphicsDevice device)
        {
            _renderer.Draw(device, _sb);
        }

        public void OnNavigateTo()
        {
            RefreshOptions();
        }

        private void RefreshOptions()
        {
            if (_gui.Controls.ContainsKey("showgrid") && _gui.Controls["showgrid"] is CheckBox)
                ((CheckBox)_gui.Controls["showgrid"]).Checked = Settings.ShowGrid;

            if (_gui.Controls.ContainsKey("sfxval"))
                _gui.Controls["sfxval"].Value = (int)(Settings.SfxVolume * 100f);

            if (_gui.Controls.ContainsKey("musicval"))
                _gui.Controls["musicval"].Value = (int)(Settings.MusicVolume * 100f);

            IControl muteOption;
            if (_gui.Controls.TryGetValue("mute", out muteOption))
            {
                if (muteOption is CheckBox)
                    ((CheckBox)muteOption).Checked = Settings.Mute;
            }
        }

        public void OnNavigateAway() { }
    }
}
