using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Coding4Fun.ScriptTD.Common.GUI;

namespace Coding4Fun.ScriptTD.Engine.GUI
{
    public class TowerButton : Button
    {
        public string TowerId;

        [ContentSerializerIgnore]
        public float InnerRadius;
        [ContentSerializerIgnore]
        public float OuterRadius;

        [ContentSerializerIgnore]
        public Texture2D TowerDragTex;

        [ContentSerializerIgnore]
        public float TowerCost;
    }
}
