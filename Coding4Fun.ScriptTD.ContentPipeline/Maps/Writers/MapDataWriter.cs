using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Coding4Fun.ScriptTD.ContentPipeline.Maps.Writers
{
    [ContentTypeWriter]
    public class MapDataWriter : ContentTypeWriter<MapContent>
    {
        protected override void Write(ContentWriter output, MapContent value)
        {
            output.Write(value.Id);
            output.Write(value.FriendlyName);
            output.Write(value.StartingCash);
            output.Write(value.StartingLives);
            output.WriteObject(value.Texture);

            output.Write(value.VictorySoundId);
            output.Write(value.DefeatSoundId);
            output.Write(value.WaveStartSoundId);
            output.Write(value.MapStartSoundId);

            output.Write(value.Waves.Count);
            foreach (var t in value.Waves)
            {
            	output.WriteObject(t);
            }

        	output.Write(value.AvailableTowers.Count);
            foreach (var t in value.AvailableTowers)
            {
            	output.WriteObject(t);
            }

        	output.Write(value.SpecialCells.Count);
			foreach(var item in value.SpecialCells)
			{
				output.Write(item.Id);
				output.Write(item.X);
				output.Write(item.Y);
				output.Write((short)item.CellType);
                output.Write(item.Buildable);
            }

            output.Write(value.CellCombos.Count);
			foreach (var item in value.CellCombos)
			{
				output.Write(value.SpecialCells.IndexOf(item.Cell1));
				output.Write(value.SpecialCells.IndexOf(item.Cell2));
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
			return "Coding4Fun.ScriptTD.Engine.Data.Readers.MapDataReader, Coding4Fun.ScriptTD.Engine"; 
        }
    }
}
