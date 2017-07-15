using System;
using Coding4Fun.ScriptTD.Engine.GUI;
using Coding4Fun.ScriptTD.Engine.Logic;
using Coding4Fun.ScriptTD.Engine.Profile;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Coding4Fun.ScriptTD.Engine.Screens
{
    public class MainMenuScreen : IScreen
    {
        public const string Name = "mainmenu";

        public ScreenManager Manager { get; private set; }

        public bool IsLoaded { get; private set; }

        public bool IsInitialized { get; private set; }

        private Window _window;
        private RenderEngine _renderer;
        private SpriteBatch _sb;
        private  ContentManager _content;

        public MainMenuScreen(ContentManager content)
        {
            _content = content;
        }

        public void Initialize(ScreenManager manager)
        {
            Manager = manager;
            _renderer = new RenderEngine();
            IsInitialized = true;
        }

        public void LoadGraphics(GraphicsDevice device)
        {
            if (!IsInitialized)
                throw new NullReferenceException("Screen must be initialized before it can load.");

            _window = _content.Load<Window>(@"Data\GUI\MainMenu");
            _window.RegisterVisuals(_renderer);
            _renderer.LoadGraphics(_content, device);
            RegisterInput();
            _sb = new SpriteBatch(device);

            Audio.RegisterPlayList(Name, ref _window.Playlist);

            OnNavigateTo();

            IsLoaded = true;
        }

        public void UnloadContent()
        {
            _window.UnRegisterVisuals(_renderer);
            _content.Unload();
        }

        public void Update(float elapsedSeconds)
        {
            _window.UpdateInput();
            _window.Update(elapsedSeconds);
            _renderer.Update(elapsedSeconds);

            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
                Manager.QuitGame();
        }

        public void Draw(GraphicsDevice device)
        {
            device.Clear(Color.Black);
            _renderer.Draw(device, _sb);
        }

        public void OnNavigateTo()
        {
            if (_window.Controls.ContainsKey("resume"))
            {
                _window.Controls["resume"].Enabled = ProfileManager.CurrentProfile.FileExists("snapshot.dat");
                _window.Controls["resume"].Visible = _window.Controls["resume"].Enabled;
            }

            Audio.PlayPlaylist(Name, true);
        }

        private void RegisterInput()
        {
            RegisterPlay();
        }

        private void RegisterPlay()
        {
            _window.Controls["play"].Tapped += (c, v) =>
            {
                if (!Manager.HasScreen(LevelSelectScreen.Name))
                {
                    Manager.AddScreen(LevelSelectScreen.Name, new LevelSelectScreen());
                }
                Manager.TransitionTo(LevelSelectScreen.Name);
            };

            _window.Controls["quit"].Tapped += (c, v) => Manager.QuitGame();

            _window.Controls["resume"].Tapped += (c, v) =>
                {
                    if (!Manager.HasScreen(GameScreen.Name))
                        Manager.AddScreen(GameScreen.Name, new GameScreen());
                    var scr = Manager.GetScreen<GameScreen>(GameScreen.Name);
                    scr.UnloadContent();
                    scr.Game = new GameSession();
                    using (var br = ProfileManager.CurrentProfile.Read("snapshot.dat"))
                    {
                        scr.Game.LoadState(br);
                    }
                    ProfileManager.CurrentProfile.CloseStream();
                    Manager.TransitionTo(GameScreen.Name);
                };

            _window.Controls["highscores"].Tapped += (c, v) =>
            {
                if (!Manager.HasScreen(HighScoreScreen.Name))
                {
                    Manager.AddScreen(HighScoreScreen.Name, new HighScoreScreen());
                }
                Manager.TransitionTo(HighScoreScreen.Name);
            };

            _window.Controls["help"].Tapped += (c, v) =>
            {
                if (!Manager.HasScreen(HelpScreen.Name))
                {
                    Manager.AddScreen(HelpScreen.Name, new HelpScreen());
                }
                Manager.TransitionTo(HelpScreen.Name);
            };
        }

        public void OnNavigateAway() { }
    }
}
