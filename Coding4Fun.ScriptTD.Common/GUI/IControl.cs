using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Coding4Fun.ScriptTD.Common.GUI
{
    public interface IControl
    {
        List<IVisual> Visuals { get; }
        Dictionary<string, IVisual> NamedVisuals { get; }

        string Name { get; set; }

        Vector2 Location { get; set; }
        Vector2 Size { get; set; }
        Rectangle Bounds { get; set; }
        Rectangle TouchBounds { get; set; }

        [ContentSerializerIgnore]
        IControl Parent { get; set; }

        string Text { get; set; }
        float Value { get; set; }
        bool Enabled { get; set; }
        bool Visible { get; set; }
        string Tag { get; set; }

        event Action<IControl, Vector2> Tapped;
        event Action<IControl, Vector2> DoubleTapped;

        // Ensure visuals are assigned to this object as well
        void RegisterVisuals(RenderEngine renderer);
        void UnRegisterVisuals(RenderEngine renderer);
        void Update(float elapsedSeconds);

        void RecalculateBounds();

        void OnTap(ref Vector2 pos);
        void OnDoubleTap(ref Vector2 pos);
    }
}
