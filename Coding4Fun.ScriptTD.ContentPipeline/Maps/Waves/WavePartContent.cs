using Coding4Fun.ScriptTD.Common.Abstracts;
using Coding4Fun.ScriptTD.ContentPipeline.Enemies;

using Microsoft.Xna.Framework.Content;

namespace Coding4Fun.ScriptTD.ContentPipeline.Maps.Waves
{
    public class WavePartContent : WavePart
    {
        [ContentSerializerIgnore]
        public string EnemyPath;

        public EnemyContent Enemy;
    }
}
