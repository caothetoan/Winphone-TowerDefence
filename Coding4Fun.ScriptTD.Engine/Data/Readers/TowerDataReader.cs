using Coding4Fun.ScriptTD.Engine.Data.Weapons;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Data.Readers
{
    public class TowerDataReader : ContentTypeReader<TowerData>
    {
		protected override TowerData Read(ContentReader input, TowerData existingInstance)
		{
			var data = new TowerData
			           	{
			           		TowerId = input.ReadString(),
			           		TowerLevel = input.ReadInt32()
			           	};

			string weapontype = input.ReadString();
			var w = Armory.GetWeaponType(weapontype);
			if (w == null)
				return null;
			data.WeaponType = w;

			data.Cost = input.ReadInt32();
			data.ReloadTime = input.ReadSingle();
			data.BuildTime = input.ReadSingle();
			data.SellTime = input.ReadSingle();
			data.CanShootFlyers = input.ReadBoolean();
			data.CanShootLand = input.ReadBoolean();
			data.MinRange = input.ReadSingle();
			data.MaxRange = input.ReadSingle();
			data.DPS = input.ReadSingle();
			data.ShotSpeed = input.ReadSingle();
			data.Texture = input.ReadObject<Texture2D>();
			data.WeaponTexture = input.ReadObject<Texture2D>();
			data.ShotSoundId = input.ReadString();
			data.BuildSoundId = input.ReadString();
			data.SellSoundId = input.ReadString();
			data.UpgradeSoundId = input.ReadString();
			data.HitSoundId = input.ReadString();
			data.HitTexture = input.ReadObject<Texture2D>();
			data.AnimationFps = input.ReadSingle();

			return data;
		}
    }
}
