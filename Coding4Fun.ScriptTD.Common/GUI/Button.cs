using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Coding4Fun.ScriptTD.Common.GUI
{
    public class Button : IControl
    {
        private readonly List<IVisual> _visuals = new List<IVisual>();
        public List<IVisual> Visuals { get { return _visuals; } }
        private readonly Dictionary<string, IVisual> _namedVisuals = new Dictionary<string, IVisual>();
        public Dictionary<string, IVisual> NamedVisuals { get { return _namedVisuals; } }

        public string Name { get; set; }

        private Vector2 _loc = Vector2.Zero;
        public Vector2 Location
        {
            get
            {
                return Parent.Location + _loc;
            }
            set
            {
                _loc = value;
            }
        }

        [ContentSerializerIgnore]
        public IControl Parent { get; set; }

        public Vector2 Size { get; set; }

        public Rectangle Bounds { get; set; }

        public Rectangle TouchBounds { get; set; }

        public string Text { get; set; }

        public float Value { get; set; }

        private bool _enabled;
        public bool Enabled
        {
            get { return Parent.Enabled && _enabled; }
            set { _enabled = value; }
        }

        private bool _visible;
        public bool Visible
        {
            get { return Parent.Visible && _visible; }
            set { _visible = value; }
        }

        public string Tag { get; set; }

        public event Action<IControl, Vector2> Tapped;
        public event Action<IControl, Vector2> DoubleTapped;

        public void RegisterVisuals(RenderEngine renderer)
        {
            for (int i = 0; i < Visuals.Count; i++)
            {
                Visuals[i].Owner = this;
                renderer.Visuals.Add(Visuals[i]);

                if (!string.IsNullOrEmpty(_visuals[i].Name) && !_namedVisuals.ContainsKey(_visuals[i].Name))
                    _namedVisuals.Add(_visuals[i].Name, _visuals[i]);
            }
        }

        public void UnRegisterVisuals(RenderEngine renderer)
        {
            foreach (var guiVisual in Visuals)
            {
                renderer.Visuals.Remove(guiVisual);
            }
        }

        public void Update(float elapsedSeconds)
        {
        }

        public void RecalculateBounds()
        {
            Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)Size.X,
                                          (int)Size.Y);
            TouchBounds = Bounds;
        }


        public void OnTap(ref Vector2 pos)
        {
            if (Tapped != null)
                Tapped(this, pos);
        }

        public void OnDoubleTap(ref Vector2 pos)
        {
            if (DoubleTapped != null)
                DoubleTapped(this, pos);
        }
    }
}
