using Microsoft.Xna.Framework.Content;

namespace Coding4Fun.ScriptTD.Engine.Data.Readers
{
	public class WaveDataReader : ContentTypeReader<WaveData>
	{
		protected override WaveData Read(ContentReader input, WaveData existingInstance)
		{
			WaveData w = new WaveData
			             	{
			             		HealthModifier = input.ReadSingle(),
			             		WorthModifier = input.ReadSingle()
			             	};

			int numParts = input.ReadInt32();
			for (int i = 0; i < numParts; i++)
			{
				var p = new WavePartData();
				w.WaveParts.Add(p);

				p.TotalEnemies = input.ReadInt32();
				p.StartTimeOffset = input.ReadSingle();
				p.TimeBetweenSpawns = input.ReadSingle();
				p.StartCell = input.ReadString();
				p.EndCell = input.ReadString();
				p.SpawnPoint = input.ReadVector2();
				p.DespawnPoint = input.ReadVector2();
				p.Enemy = input.ReadObject<EnemyData>();
			}

			return w;
		}
	}
}
