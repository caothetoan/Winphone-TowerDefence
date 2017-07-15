using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Screens
{
    public interface IScreen
    {
        ScreenManager Manager { get; }
        bool IsLoaded { get; }
        bool IsInitialized { get; }

        void Initialize(ScreenManager manager);
        void LoadGraphics(GraphicsDevice device);
        void UnloadContent();

        void Update(float elapsedSeconds);
        void Draw(GraphicsDevice device);

        void OnNavigateTo();
        void OnNavigateAway();
    }
}
