using Coding4Fun.ScriptTD.Engine.Data.Abstracts;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Coding4Fun.ScriptTD.ContentPipeline.Enemies
{
    public class EnemyContent : Enemy
    {
        public Texture2DContent Texture;

        [ContentSerializerIgnore]
        public string TexturePath;
    }
}
