using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Coding4Fun.ScriptTD.ContentPipeline.Towers
{
    [ContentTypeWriter]
    public class TowerDataWriter : ContentTypeWriter<TowerContent>
    {
        protected override void Write(ContentWriter output, TowerContent value)
        {
            output.Write(value.TowerId);
            output.Write(value.TowerLevel);
            output.Write(value.WeaponName);
            output.Write(value.Cost);
            output.Write(value.ReloadTime);
            output.Write(value.BuildTime);
            output.Write(value.SellTime);
            output.Write(value.CanShootFlyers);
            output.Write(value.CanShootLand);
            output.Write(value.MinRange);
            output.Write(value.MaxRange);
            output.Write(value.DPS);
            output.Write(value.ShotSpeed);
            output.WriteObject(value.Texture);
            output.WriteObject(value.WeaponTexture);
            output.Write(value.ShotSoundId);
            output.Write(value.BuildSoundId);
            output.Write(value.SellSoundId);
            output.Write(value.UpgradeSoundId);
            output.Write(value.HitSoundId);
            output.WriteObject(value.HitTexture);
            output.Write(value.AnimationFps);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
			return "Coding4Fun.ScriptTD.Engine.Data.Readers.TowerDataReader, Coding4Fun.ScriptTD.Engine"; 
        }
    }
}
