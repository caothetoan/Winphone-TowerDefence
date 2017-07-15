using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Common.GUI.Visuals
{
    public enum TextSource
    {
        Manual,
        ControlText,
        ControlValue
    }

    public class TextVisual : IVisual
    {
        public IControl Owner { get; set; }

        public string Name { get; set; }

        // Absolute location if Alignment is None, otherwise margin
        // Always relative to Owner location
        public Vector2 Location { get; set; }

        // Does Nothing
        public Vector2 Size { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        public Visibility Visibility { get; set; }

        public string FontPath;
        private SpriteFont _font;
        public Color TextColor = Color.Black;
        public Color DisabledColor = Color.Black;
        public TextSource Source;
        public string ManualText = "";
        public bool AllowParentResize;
        public Color StrokeColor = Color.Black;
        public Vector2 StrokeOffset = new Vector2(2f);

        public void LoadGraphics(ContentManager content, GraphicsDevice graphics)
        {
            if (!string.IsNullOrEmpty(FontPath))
            {
                _font = content.Load<SpriteFont>(FontPath);
            }
        }

        public void UnloadGraphics() { }

        public void Update(float elapsedSeconds) { }

        public void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (_font == null)
                return;

            string text = "";
            switch (Source)
            {
                case TextSource.Manual:
                    text = ManualText;
                    break;
                case TextSource.ControlText:
                    text = Owner.Text;
                    break;
                case TextSource.ControlValue:
                    text = Owner.Value.ToString();
                    break;
            }

            var textSize = _font.MeasureString(text);
            var ownerSize = Owner.Size;

            if (AllowParentResize)
            {
                bool recalc = false;

                if (ownerSize.X < textSize.X)
                {
                    ownerSize.X = textSize.X;
                    recalc = true;
                }

                if (ownerSize.Y < textSize.Y)
                {
                    ownerSize.Y = textSize.Y;
                    recalc = true;
                }

                if (recalc)
                {
                    Owner.Size = ownerSize;
                    Owner.RecalculateBounds();
                }
            }

            Vector2 finalLoc;
            Vector2 ownerLoc = Owner.Location;
            Vector2 loc = Location;
            LayoutHelper.DoLayout(HorizontalAlignment, VerticalAlignment, ref ownerLoc, ref ownerSize, ref loc, ref textSize, out finalLoc);

            if (StrokeColor != Color.Transparent)
                spriteBatch.DrawString(_font, text, finalLoc + StrokeOffset, StrokeColor);
            spriteBatch.DrawString(_font, text, finalLoc, Owner.Enabled ? TextColor : DisabledColor);
        }
    }
}
