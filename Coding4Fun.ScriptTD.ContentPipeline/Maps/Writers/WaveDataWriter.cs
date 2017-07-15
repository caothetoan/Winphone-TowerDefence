using Coding4Fun.ScriptTD.ContentPipeline.Maps.Waves;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Coding4Fun.ScriptTD.ContentPipeline.Maps.Writers
{
    [ContentTypeWriter]
    public class WaveDataWriter : ContentTypeWriter<WaveContent>
    {
        protected override void Write(ContentWriter output, WaveContent value)
        {
            output.Write(value.HealthModifier);
            output.Write(value.WorthModifier);
            output.Write(value.WaveParts.Count);

            foreach (var part in value.WaveParts)
            {
            	output.Write(part.TotalEnemies);
            	output.Write(part.StartTimeOffset);
            	output.Write(part.TimeBetweenSpawns);
            	output.Write(part.StartCell);
            	output.Write(part.EndCell);
            	output.Write(part.SpawnPoint);
            	output.Write(part.DespawnPoint);
            	output.WriteObject(part.Enemy);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
			return "Coding4Fun.ScriptTD.Engine.Data.Readers.WaveDataReader, Coding4Fun.ScriptTD.Engine"; 
        }
    }
}
