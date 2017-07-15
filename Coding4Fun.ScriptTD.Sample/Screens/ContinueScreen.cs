using Coding4Fun.ScriptTD.Engine.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Coding4Fun.ScriptTD.Common.GUI;

namespace Coding4Fun.ScriptTD.Sample.Screens
{
    public class ContinueScreen : IScreen
    {
        public const string VictoryName = "victory";
        public const string DefeatName = "defeat";

        public ScreenManager Manager { get; private set; }

        public bool IsLoaded { get; private set; }

        public bool IsInitialized { get; private set; }

        private Window _gui;
        private RenderEngine _renderer;

        public ContentManager Content;
        private readonly bool _isVictoryScreen;
        private SpriteBatch _sb;

        private bool _backPressed = false;

        public ContinueScreen(bool isVictoryScreen)
        {
            _isVictoryScreen = isVictoryScreen;
        }

        public void Initialize(ScreenManager manager)
        {
            Manager = manager;
            IsInitialized = true;
        }

        public void LoadGraphics(GraphicsDevice device)
        {
            _sb = new SpriteBatch(device);

        	_gui = Content.Load<Window>(_isVictoryScreen ? @"Data\GUI\Victory" : @"Data\GUI\Defeat");
        	_renderer = new RenderEngine();
            _gui.RegisterVisuals(_renderer);
            _renderer.LoadGraphics(Content, device);

            if (_gui.Controls.ContainsKey("continue"))
                _gui.Controls["continue"].Tapped += GotoMainMenu;

            IsLoaded = true;
        }

        private void GotoMainMenu(IControl sender, Vector2 position)
        {
            Manager.TransitionTo(MainMenuScreen.Name);
        }

        public void Update(float elapsedSeconds)
        {
            _gui.UpdateInput();
            _gui.Update(elapsedSeconds);
            _renderer.Update(elapsedSeconds);

            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
            {
                _backPressed = true;
            }
            else if (_backPressed)
            {
                _backPressed = false;
                Manager.TransitionTo(MainMenuScreen.Name);
            }
        }

        public void Draw(GraphicsDevice device)
        {
            _renderer.Draw(device, _sb);
        }

		public void UnloadContent() { }

        public void OnNavigateTo()
        {
            _backPressed = false;
        }

        public void OnNavigateAway() { }
    }
}
