using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Coding4Fun.ScriptTD.ContentPipeline.Maps
{
    [ContentProcessor(DisplayName = "Coding4Fun - Map List Processor")]
    public class MapListProcessor : ContentProcessor<MapListContent, MapListContent>
    {
        public override MapListContent Process(MapListContent input, ContentProcessorContext context)
        {
            for (int i = 0; i < input.RawData.Count; i++)
            {
                var map = input.RawData[i];
                ProcessMap(ref map, context);
                input.Maps.Add(map.Id, map);
            }

            return input;
        }

        private static void ProcessMap(ref MapListingContent map, ContentProcessorContext context)
        {
            var origFile = Path.Combine("Data\\Maps\\", map.DataFilePath);
            var path = context.BuildAsset<MapContent, MapContent>(new ExternalReference<MapContent>(origFile), "MapProcessor", null, "MapImporter", null).Filename;
            var data = context.BuildAndLoadAsset<MapContent, MapContent>(new ExternalReference<MapContent>(origFile), "MapProcessor", null, "MapImporter");
            map.DataFilePath = Path.Combine(@"Data\Maps\", Path.GetFileNameWithoutExtension(path));

            map.Id = data.Id;
            map.FriendlyName = data.FriendlyName;
            map.Thumbnail = data.Texture; // todo: resize thumbnail texture
        }
    }
}
