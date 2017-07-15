using Coding4Fun.ScriptTD.Common;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Data.Readers
{
    public class EnemyDataReader : ContentTypeReader<EnemyData>
    {
        protected override EnemyData Read(ContentReader input, EnemyData existingInstance)
        {
            var data = new EnemyData
                             	{
                             		Id = input.ReadString(),
                             		TotalHealth = input.ReadSingle(),
                             		TotalSpeed = input.ReadSingle(),
                             		CanFly = input.ReadBoolean(),
                             		TotalWorth = input.ReadSingle(),
                             		Texture = input.ReadObject<Texture2D>(),
                             		DeathSoundId = input.ReadString(),
                             		SpawnSoundId = input.ReadString()
                             	};

        	int numRes = input.ReadInt32();

            for (int i = 0; i < numRes; i++)
            {
                string key = input.ReadString();
                var r = new ResistanceData
                                   	{
                                   		TowerId = input.ReadString(),
                                   		TowerLevel = input.ReadInt32(),
                                   		Multiplier = input.ReadSingle(),
                                   		Type = (ResistanceData.ResistanceType) input.ReadInt16()
                                   	};

            	data.Resistances.Add(key, r);
            }

            return data;
        }
    }
}
