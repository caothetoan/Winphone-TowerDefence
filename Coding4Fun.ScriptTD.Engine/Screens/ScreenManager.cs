using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Screens
{
    public class ScreenManager
    {
        private readonly Dictionary<string, IScreen> _screens = new Dictionary<string, IScreen>();

        public IScreen CurrentScreen;
        private IScreen _nextScreen;

        public readonly LoadingScreen LoadingScreen;

        public readonly IServiceProvider Services;
        public readonly GraphicsDevice GraphicsDevice;

        private readonly Game _game;

        public ScreenManager(IServiceProvider services, GraphicsDevice graphics, Game game)
        {
            Services = services;
            GraphicsDevice = graphics;
            LoadingScreen = new LoadingScreen();
            AddScreen("loading", LoadingScreen);
            _game = game;
        }

        public void QuitGame()
        {
            _game.Exit();
        }

        public void AddScreen(string name, IScreen screen)
        {
            if (_screens.ContainsKey(name))
                return;

            _screens.Add(name, screen);

            if (!screen.IsInitialized)
                screen.Initialize(this);
        }

        public bool HasScreen(string name)
        {
            return _screens.ContainsKey(name);
        }

        public T GetScreen<T>(string name) where T : IScreen
        {
        	if (HasScreen(name) && (_screens[name] is T))
                return (T)_screens[name];
        	
			return default(T);
        }

    	public void TransitionTo(string name)
        {
            if (_screens.ContainsKey(name))
                TransitionTo(_screens[name]);
        }

        public void TransitionTo(IScreen screen)
        {
            CloseScreen();

            if (!screen.IsLoaded)
            {
                _nextScreen = screen;
                _nextScreen.LoadGraphics(GraphicsDevice);
            }
            else
            {
                CurrentScreen = screen;
                CurrentScreen.OnNavigateTo();
            }
        }

        public void CloseScreen()
        {
            if (CurrentScreen != null)
            {
                CurrentScreen.OnNavigateAway();
                CurrentScreen = null;
            }
        }

        public void Draw(GraphicsDevice graphics)
        {
            if (CurrentScreen == null || !CurrentScreen.IsLoaded)
            {
                if (LoadingScreen.IsLoaded)
                    LoadingScreen.Draw(graphics);
            }
            else
            {
                CurrentScreen.Draw(graphics);
            }
        }

        public void Update(float elapsedSeconds)
        {
            if (CurrentScreen == null || !CurrentScreen.IsLoaded)
            {
                LoadingScreen.Update(elapsedSeconds);

                if (_nextScreen != null && _nextScreen.IsLoaded)
                {
                    CurrentScreen = _nextScreen;
                    CurrentScreen.OnNavigateTo();
                }
            }
            else
            {
                CurrentScreen.Update(elapsedSeconds);
            }
        }

        public void LoadAllContent(GraphicsDevice graphics)
        {
        	foreach (var screen in _screens.Values.Where(screen => !screen.IsLoaded))
        	{
        		screen.LoadGraphics(graphics);
        	}
        }

    	public void UnloadAllContent()
    	{
    		foreach (var screen in _screens.Values.Where(screen => screen.IsLoaded))
    		{
    			screen.UnloadContent();
    		}
    	}
    }
}
