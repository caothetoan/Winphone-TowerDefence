using System;
using System.Collections.Generic;
using Coding4Fun.ScriptTD.Engine.Data;
using Coding4Fun.ScriptTD.Engine.GUI;
using Coding4Fun.ScriptTD.Engine.GUI.Visuals;
using Coding4Fun.ScriptTD.Engine.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Coding4Fun.ScriptTD.Engine.Screens
{
    public class LevelSelectScreen : IScreen
    {
        public const string Name = "levelselect";

        public ScreenManager Manager { get; private set; }

        public bool IsLoaded { get; private set; }

        public bool IsInitialized { get; private set; }

        private RenderEngine _renderer;
        private Window _window;

        private BackgroundLoader _loader;
        private SpriteBatch _sb;

        private MapListData _maps;
        private int _start;
        private readonly List<IControl> _thumbControls = new List<IControl>();

        private IControl _prevBtn, _nextBtn;

        private bool _backDown, _transitioning;

        public void Initialize(ScreenManager manager)
        {
            Manager = manager;
            _loader = new BackgroundLoader(manager.Services);
            _renderer = new RenderEngine();

            IsInitialized = true;
        }

        public void LoadGraphics(GraphicsDevice device)
        {
            if (!IsInitialized)
                throw new NullReferenceException("Screen must be initialized before LoadGraphics can be called.");

            _loader.BeginLoad(new LoadingDelegate(c =>
                                                      {
                                                          _window = c.Load<Window>(@"Data\GUI\LevelSelect");
                                                          foreach (var control in _window.Controls.Values)
                                                          {
                                                              if (control.Name.StartsWith("level"))
                                                              {
                                                                  _thumbControls.Add(control);
                                                              }
                                                              else
                                                              {
                                                                  if (control.Name.Equals("next"))
                                                                      _nextBtn = control;
                                                                  else if (control.Name.Equals("prev"))
                                                                      _prevBtn = control;
                                                              }
                                                          }
                                                          _window.RegisterVisuals(_renderer);
                                                          _renderer.LoadGraphics(c, device);
                                                          RegisterInput();
                                                          _sb = new SpriteBatch(device);
                                                          _maps = c.Load<MapListData>(@"Data\maps");
                                                          UpdateButtons();
                                                          IsLoaded = true;
                                                      }));
        }

        public void UnloadContent()
        {
            _window.UnRegisterVisuals(_renderer);
            _loader.UnloadContent();
        }

        public void Update(float elapsedSeconds)
        {
            _window.UpdateInput();
            _window.Update(elapsedSeconds);
            _renderer.Update(elapsedSeconds);

            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
            {
                _backDown = true;
            }
            else if (_backDown)
            {
                Manager.TransitionTo(MainMenuScreen.Name);
                _backDown = false;
            }
        }

        public void Draw(GraphicsDevice device)
        {
            device.Clear(Color.Black);
            _renderer.Draw(device, _sb);
        }

        public void OnNavigateTo()
        {
            _transitioning = false;
            UpdateButtons();
        }

        private void NextPage()
        {
            if ((_start + _thumbControls.Count) < _maps.Maps.Count)
                _start += _thumbControls.Count;

            UpdateButtons();
        }

        private void PrevPage()
        {
            if (_start > 0)
            {
                if ((_start - _thumbControls.Count) < 0)
                    _start = 0;
                else
                    _start -= _thumbControls.Count;

                UpdateButtons();
            }
        }

        private void UpdateButtons()
        {
            var en = _maps.Maps.GetEnumerator();
            
			for (int i = 0; i < _start; i++)
            {
                en.MoveNext();
            }

            foreach (var control in _thumbControls)
            {
            	if (!en.MoveNext())
            	{
            		control.Visible = false;
            		control.Enabled = false;
            		continue;
            	}

            	control.Visible = true;

            	bool valid = true;

            	for (int j = 0; j < en.Current.Value.Prerequisites.Count; j++)
            	{
            		valid &= ProgressTracker.GetStatus(en.Current.Value.Prerequisites[j]);
            	}

            	control.Enabled = valid;
            	control.Tag = en.Current.Key;
            	control.Text = en.Current.Value.FriendlyName;

            	if (control.NamedVisuals.ContainsKey("thumb"))
            	{
            		var img = (ImageVisual)control.NamedVisuals["thumb"];
            		img.Texture = en.Current.Value.Thumbnail;
            	}
            }

            if (_nextBtn != null)
            {
                _nextBtn.Visible = (_start + _thumbControls.Count) < _maps.Maps.Count;
                _nextBtn.Enabled = _nextBtn.Visible;
            }

            if (_prevBtn != null)
            {
                _prevBtn.Visible = _start > 0;
            }
        }

        private void RegisterInput()
        {
            _window.Flicked += (c, d) =>
                               	{
                               		if (d.X < 0)
                               			NextPage();
                               		else if (d.X > 0)
                               			PrevPage();
                               	};

            _window.Controls["next"].Tapped += (c, p) => NextPage();

            _window.Controls["prev"].Tapped += (c, p) => PrevPage();

            foreach (var control in _thumbControls)
            {
            	control.Tapped += LoadMap;
            }
        }

        private void LoadMap(IControl control, Vector2 pos)
        {
            if (_maps.Maps.ContainsKey(control.Tag) && !_transitioning)
            {
                var map = _maps.Maps[control.Tag];
                var session = new GameSession
                {
                    MapLoadInfo = map
                };

                if (!Manager.HasScreen(GameScreen.Name))
                    Manager.AddScreen(GameScreen.Name, new GameScreen());
                
				var scr = Manager.GetScreen<GameScreen>(GameScreen.Name);
                scr.UnloadContent();
                scr.Game = session;
                
				Manager.TransitionTo(GameScreen.Name);
                _transitioning = true;
            }
        }

		public void OnNavigateAway() { }
    }
}
