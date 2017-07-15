using Coding4Fun.ScriptTD.ContentPipeline.Helpers;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Coding4Fun.ScriptTD.ContentPipeline.Enemies
{
    [ContentProcessor(DisplayName = "Coding4Fun - Enemy Data Processor")]
    public class EnemyProcessor : ContentProcessor<EnemyContent, EnemyContent>
    {
        public override EnemyContent Process(EnemyContent input, ContentProcessorContext context)
        {
            input.Texture = LoadingHelper.LoadTexture(context, @"Textures\Creeps\" + input.TexturePath);

            return input;
        }
    }
}