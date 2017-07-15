using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Common.GUI.Visuals
{
    public class GridOverlayVisual : IVisual
    {
        public IControl Owner { get; set; }

        public string Name { get; set; }

        public Vector2 Location { get; set; }

        public Vector2 Size { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        public Visibility Visibility { get; set; }

        public float GridSpacing = 35f;

        public Color GridColor;

        private BasicEffect _fx;

        private readonly List<VertexPositionColor> _verts = new List<VertexPositionColor>();
        private VertexBuffer _vb;

        private Vector3 _finalPos3;

        public void LoadGraphics(ContentManager content, GraphicsDevice graphics)
        {
            _fx = new BasicEffect(graphics)
                    {
                        LightingEnabled = false,
                        View = Matrix.Identity,
                        VertexColorEnabled = true,
                        Projection = Matrix.CreateOrthographicOffCenter(0, 800, 0, 480, 0, 1)
                    };

            if (_verts.Count == 0)
            {
                GenerateVerts();
            }

            _vb = new VertexBuffer(graphics, typeof(VertexPositionColor), _verts.Count, BufferUsage.WriteOnly);
            _vb.SetData(_verts.ToArray());
        }

        private void GenerateVerts()
        {
            for (float x = 0; x <= Size.X; x += GridSpacing)
            {
                _verts.Add(new VertexPositionColor(new Vector3(x, 0, 0), GridColor));
                _verts.Add(new VertexPositionColor(new Vector3(x, Size.Y, 0), GridColor));
            }

            for (float y = 0; y <= Size.Y; y += GridSpacing)
            {
                _verts.Add(new VertexPositionColor(new Vector3(0, y, 0), GridColor));
                _verts.Add(new VertexPositionColor(new Vector3(Size.X, y, 0), GridColor));
            }
        }

        public void UnloadGraphics() { }

        public void Update(float elapsedSeconds) { }

        public void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            var ownerPos = Owner.Location;
            var ownerSize = Owner.Size;
            var pos = Location;
            var size = Size;
            Vector2 finalPos;
            LayoutHelper.DoLayout(HorizontalAlignment, VerticalAlignment, ref ownerPos, ref ownerSize, ref pos, ref size,
                                  out finalPos);

            _finalPos3.X = finalPos.X;
            _finalPos3.Y = finalPos.Y;

            _fx.World = Matrix.CreateTranslation(_finalPos3);
            _fx.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(_vb);
            device.DrawPrimitives(PrimitiveType.LineList, 0, _verts.Count / 2);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
        }
    }
}
