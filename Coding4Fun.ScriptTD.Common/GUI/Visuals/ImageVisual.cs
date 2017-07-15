using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Common.GUI.Visuals
{
    public class ImageVisual : IVisual
    {
        public IControl Owner { get; set; }

        public string Name { get; set; }

        public Vector2 Location { get; set; }

        public Vector2 Size { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        public Visibility Visibility { get; set; }

        public string ImagePath;

        [ContentSerializerIgnore]
        public Texture2D Texture;
        public Color DisabledTint = Color.White;
        public Color Tint = Color.White;
        private Rectangle _layoutRect = new Rectangle();
        public bool UseParentSize = false;

        public void LoadGraphics(ContentManager content, GraphicsDevice graphics)
        {
            if (!string.IsNullOrEmpty(ImagePath))
            {
                Texture = content.Load<Texture2D>(ImagePath);
                if (!UseParentSize)
                    Size = new Vector2(Texture.Width, Texture.Height);
            }
        }

        public void UnloadGraphics()
        {
        }

        public void Update(float elapsedSeconds)
        {
        }

        public virtual void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (Texture == null)
                return;

            var size = UseParentSize ? Owner.Size : Size;
            var loc = Location;
            var ownerLoc = Owner.Location;
            var ownerSize = Owner.Size;
            Vector2 finalLoc;
            LayoutHelper.DoLayout(HorizontalAlignment, VerticalAlignment, ref ownerLoc, ref ownerSize, ref loc, ref size, out finalLoc);

            _layoutRect.X = (int)finalLoc.X;
            _layoutRect.Y = (int)finalLoc.Y;
            _layoutRect.Width = (int)size.X;
            _layoutRect.Height = (int)size.Y;

            spriteBatch.Draw(Texture, _layoutRect, Owner.Enabled ? Tint : DisabledTint);
        }
    }
}
