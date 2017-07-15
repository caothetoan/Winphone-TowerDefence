using Coding4Fun.ScriptTD.Common.Abstracts;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Coding4Fun.ScriptTD.ContentPipeline.Towers
{
    public class TowerContent : Tower
    {
        public Texture2DContent WeaponTexture;
        public Texture2DContent Texture;
        public Texture2DContent HitTexture;

        [ContentSerializerIgnore]
        public string TexturePath;

        [ContentSerializerIgnore]
        public string WeaponTexturePath;
    }
}
