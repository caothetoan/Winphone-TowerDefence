using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Coding4Fun.ScriptTD.ContentPipeline.Enemies
{
    [ContentTypeWriter]
    public class EnemyDataWriter : ContentTypeWriter<EnemyContent>
    {
        protected override void Write(ContentWriter output, EnemyContent value)
        {
            output.Write(value.Id);
            output.Write(value.TotalHealth);
            output.Write(value.TotalSpeed);
            output.Write(value.CanFly);
            output.Write(value.TotalWorth);
            output.WriteObject(value.Texture);
            output.Write(value.DeathSoundId);
            output.Write(value.SpawnSoundId);
			output.Write(value.Resistances.Count);

            var en = value.Resistances.GetEnumerator();

            while(en.MoveNext())
            {
            	output.Write(en.Current.Key);
            	output.Write(en.Current.Value.TowerId);
            	output.Write(en.Current.Value.TowerLevel);
            	output.Write(en.Current.Value.Multiplier);
            	output.Write((short)en.Current.Value.Type);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
			return "Coding4Fun.ScriptTD.Engine.Data.Readers.EnemyDataReader, Coding4Fun.ScriptTD.Engine"; 
        }
    }
}
