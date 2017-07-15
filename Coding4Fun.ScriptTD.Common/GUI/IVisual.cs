using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Common.GUI
{
    public interface IVisual
    {
        IControl Owner { get; set; }

        string Name { get; set; }

        Vector2 Location { get; set; }
        Vector2 Size { get; set; }
        HorizontalAlignment HorizontalAlignment { get; set; }
        VerticalAlignment VerticalAlignment { get; set; }

        Visibility Visibility { get; set; }

        void LoadGraphics(ContentManager content, GraphicsDevice graphics);
        void UnloadGraphics();
        void Update(float elapsedSeconds);
        void Draw(GraphicsDevice device, SpriteBatch spriteBatch);
    }
}
