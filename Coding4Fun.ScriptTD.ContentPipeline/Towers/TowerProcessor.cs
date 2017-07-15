using System.Collections.Generic;

using Coding4Fun.ScriptTD.ContentPipeline.Helpers;

using Microsoft.Xna.Framework.Content.Pipeline;

namespace Coding4Fun.ScriptTD.ContentPipeline.Towers
{
    [ContentProcessor(DisplayName = "Coding4Fun - Tower Data Processor")]
    public class TowerProcessor : ContentProcessor<List<TowerContent>, List<TowerContent>>
    {
        public override List<TowerContent> Process(List<TowerContent> input, ContentProcessorContext context)
        {
        	foreach (var tower in input)
        	{
        		tower.Texture = LoadingHelper.LoadTexture(context, @"Textures\Towers\" + tower.TexturePath);
        		tower.WeaponTexture = LoadingHelper.LoadTexture(context, @"Textures\Weapons\" + tower.WeaponTexturePath);

        		if (!string.IsNullOrWhiteSpace(tower.HitTexturePath))
        			tower.HitTexture = LoadingHelper.LoadTexture(context, @"Textures\HitEffects\" + tower.HitTexturePath);
        	}

        	return input;
        }
    }
}