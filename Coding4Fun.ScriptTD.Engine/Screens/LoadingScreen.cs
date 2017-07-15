using Coding4Fun.ScriptTD.Engine.GUI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Screens
{
    public class LoadingScreen : IScreen
    {
        public const string Name = "loading";

        public ScreenManager Manager { get; private set; }
        public bool IsLoaded { get; private set; }
        public bool IsInitialized { get; private set; }

        private ContentManager _content;

        private Window _window;
        private RenderEngine _renderer;
        private SpriteBatch _sb;

        public void Initialize(ScreenManager manager)
        {
            Manager = manager;
            IsInitialized = true;
        }

        public void LoadGraphics(GraphicsDevice device)
        {
            _content = new ContentManager(Manager.Services, "Content");
            _renderer = new RenderEngine();
            _window = _content.Load<Window>(@"Data\GUI\Loading");
            _window.RegisterVisuals(_renderer);
            _renderer.LoadGraphics(_content, device);
            _sb = new SpriteBatch(device);
            IsLoaded = true;
        }

        public void UnloadContent()
        {
            _window.UnRegisterVisuals(_renderer);
            _content.Unload();
            _sb.Dispose();
        }

        public void Update(float elapsedSeconds)
        {
            _window.UpdateInput();
            _window.Update(elapsedSeconds);
            _renderer.Update(elapsedSeconds);
        }

        public void Draw(GraphicsDevice device)
        {
            _renderer.Draw(device, _sb);
        }

        public void OnNavigateTo() { }

        public void OnNavigateAway() { }
    }
}
