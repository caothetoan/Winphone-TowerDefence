using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Coding4Fun.ScriptTD.ContentPipeline.Maps.Writers
{
    [ContentTypeWriter]
    public class MapListDataWriter : ContentTypeWriter<MapListContent>
    {
        protected override void Write(ContentWriter output, MapListContent value)
        {
            output.Write(value.Maps.Count);
            foreach (var map in value.Maps)
            {
                output.Write(map.Key);
                output.WriteObject(map.Value);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
			return "Coding4Fun.ScriptTD.Engine.Data.Readers.MapListDataReader, Coding4Fun.ScriptTD.Engine"; 
        }
    }
}
