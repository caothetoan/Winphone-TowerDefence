using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;

namespace Coding4Fun.ScriptTD.Common.GUI
{
    public class Window : IControl
    {
        private const int HalfFingerSize = 15;
        private Rectangle _fingerRect = new Rectangle(0, 0, HalfFingerSize * 2, HalfFingerSize * 2);

        private readonly List<IVisual> _visuals = new List<IVisual>();
        public List<IVisual> Visuals { get { return _visuals; } }
        private readonly Dictionary<string, IVisual> _namedVisuals = new Dictionary<string, IVisual>();
        public Dictionary<string, IVisual> NamedVisuals { get { return _namedVisuals; } }

        public readonly Dictionary<string, IControl> Controls = new Dictionary<string, IControl>();

        public string Name { get; set; }

        public Vector2 Location { get; set; }

        public Vector2 Size { get; set; }

        public Rectangle Bounds { get; set; }

        public Rectangle TouchBounds { get; set; }

        [ContentSerializerIgnore]
        public IControl Parent { get; set; }

        public string Text { get; set; }

        public float Value { get; set; }

        public bool Enabled { get; set; }

        public bool Visible { get; set; }

        public string Tag { get; set; }

        public List<string> Playlist = new List<string>();


        public event Action<IControl, Vector2> Tapped;
        public event Action<IControl, Vector2> DoubleTapped;
        public event Action<IControl, Vector2> Flicked;

        private Vector2 _prevDragPos;
        private Vector2 _firstTouchPoint = -Vector2.UnitX;

        public Window()
        {
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.DoubleTap | GestureType.Flick;
        }

        public void RegisterVisuals(RenderEngine renderer)
        {
            for (int i = 0; i < Visuals.Count; i++)
            {
                Visuals[i].Owner = this;
                renderer.Visuals.Add(Visuals[i]);

                if (!string.IsNullOrEmpty(_visuals[i].Name) && !_namedVisuals.ContainsKey(_visuals[i].Name))
                    _namedVisuals.Add(_visuals[i].Name, _visuals[i]);
            }

            foreach (var control in Controls.Values)
            {
                control.Parent = this;
                control.RegisterVisuals(renderer);
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
            if (!Enabled)
                return;

            var e = Controls.GetEnumerator();

            while (e.MoveNext())
            {
                if (e.Current.Value.Enabled)
                    e.Current.Value.Update(elapsedSeconds);
            }
        }

        // This is called to handle input, takes over the entire gesture system
        public bool UpdateInput()
        {
            if (!Enabled)
                return false;

            var touches = TouchPanel.GetState();

            bool action = false;
            while (TouchPanel.IsGestureAvailable)
            {
                var gesture = TouchPanel.ReadGesture();
                action |= BubbleGesture(ref gesture);
            }
            return action;
        }

        // For use when gesture reading is handled externally
        public bool BubbleGesture(ref GestureSample gesture)
        {
            if (!Enabled)
                return false;

            Vector2 pos = gesture.Position;

            if (gesture.GestureType == GestureType.Flick)
            {
                if (Flicked != null)
                    Flicked(this, gesture.Delta);
                return true;
            }
            else
            {
                _fingerRect.X = (int)pos.X - HalfFingerSize;
                _fingerRect.Y = (int)pos.Y - HalfFingerSize;

                var e = Controls.GetEnumerator();
                bool triggered = false;
                while (e.MoveNext())
                {
                    if (!e.Current.Value.Enabled)
                        continue;

                    var control = e.Current.Value;

                    if (control.TouchBounds.Intersects(_fingerRect))
                    {

                        switch (gesture.GestureType)
                        {
                            case GestureType.DoubleTap:
                                control.OnDoubleTap(ref pos);
                                return true;

                            case GestureType.Tap:
                                control.OnTap(ref pos);
                                return true;

                            default:
                                break;
                        }
                    }
                }

                if (Enabled && TouchBounds.Contains(_fingerRect))
                {
                    switch (gesture.GestureType)
                    {
                        case GestureType.DoubleTap:
                            OnDoubleTap(ref pos);
                            triggered = true;
                            break;

                        case GestureType.Tap:
                            OnTap(ref pos);
                            triggered = true;
                            break;

                        default:
                            break;
                    }
                }
                return triggered;
            }
        }

        public IControl GetEnabledChildAtPoint(Vector2 point)
        {
            var e = Controls.GetEnumerator();
            while (e.MoveNext())
            {
                if (!e.Current.Value.Enabled)
                    continue;

                _fingerRect.X = (int)point.X - HalfFingerSize;
                _fingerRect.Y = (int)point.Y - HalfFingerSize;

                if (e.Current.Value.TouchBounds.Intersects(_fingerRect))
                    return e.Current.Value;
            }
            return null;
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
