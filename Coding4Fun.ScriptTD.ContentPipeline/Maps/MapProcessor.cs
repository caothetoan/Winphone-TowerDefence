using System.Collections.Generic;
using System.IO;
using Coding4Fun.ScriptTD.ContentPipeline.Enemies;
using Coding4Fun.ScriptTD.ContentPipeline.Helpers;
using Coding4Fun.ScriptTD.ContentPipeline.Towers;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Coding4Fun.ScriptTD.ContentPipeline.Maps
{
    [ContentProcessor(DisplayName = "Coding4Fun - Map Data Processor")]
    public class MapProcessor : ContentProcessor<MapContent, MapContent>
    {
        public override MapContent Process(MapContent input, ContentProcessorContext context)
        {
            input.TexturePath = Path.Combine("Textures\\Maps\\", input.TexturePath);
            input.Texture = LoadingHelper.LoadTexture(context, input.TexturePath);

            for (int i = 0; i < input.Waves.Count; i++)
            {
                for (int j = 0; j < input.Waves[i].WaveParts.Count; j++)
                {
                    var p = Path.Combine("Data\\Enemies\\", input.Waves[i].WaveParts[j].EnemyPath);
                    input.Waves[i].WaveParts[j].Enemy = context.BuildAndLoadAsset<EnemyContent, EnemyContent>(new ExternalReference<EnemyContent>(p), "EnemyProcessor", null, "EnemyImporter");
                }
            }

            input.AvailableTowers = new List<TowerContent>();
            foreach (var t in input.TowerImport)
            {
                var p = Path.Combine("Data\\Towers\\", Path.ChangeExtension(t.Item1, ".xml"));
                var towers = context.BuildAndLoadAsset<List<TowerContent>, List<TowerContent>>(new ExternalReference<List<TowerContent>>(p), "TowerProcessor", null, "TowerImporter");
                foreach (var tower in towers)
                {
                    if (tower.TowerLevel <= t.Item2)
                        input.AvailableTowers.Add(tower);
                }
            }

            return input;
        }
    }
}