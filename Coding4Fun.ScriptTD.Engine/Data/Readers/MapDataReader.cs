using Coding4Fun.ScriptTD.Common;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Data.Readers
{
    public class MapDataReader : ContentTypeReader<MapData>
    {
        protected override MapData Read(ContentReader input, MapData existingInstance)
        {
            var data = new MapData
                            {
                                Id = input.ReadString(),
                                FriendlyName = input.ReadString(),
                                StartingCash = input.ReadSingle(),
                                StartingLives = input.ReadInt32(),
                                Texture = input.ReadObject<Texture2D>(),
                                VictorySoundId = input.ReadString(),
                                DefeatSoundId = input.ReadString(),
                                WaveStartSoundId = input.ReadString(),
                                MapStartSoundId = input.ReadString()
                            };

            int numWaves = input.ReadInt32();
            for (int i = 0; i < numWaves; i++)
            {
                var wave = input.ReadObject<WaveData>();
                data.Waves.Add(wave);
            }

            int numTowers = input.ReadInt32();
            for (int i = 0; i < numTowers; i++)
            {
                var tower = input.ReadObject<TowerData>();
                data.AvailableTowers.Add(tower);
            }

            int numCells = input.ReadInt32();
            for (int i = 0; i < numCells; i++)
            {
                var cell = new SpecialCell
                                            {
                                                Id = input.ReadString(),
                                                X = input.ReadInt32(),
                                                Y = input.ReadInt32(),
                                                CellType = (SpecialCellType)input.ReadInt16(),
                                                Buildable = input.ReadBoolean()
                                            };
                data.SpecialCells.Add(cell);
            }

            int numCombos = input.ReadInt32();
            for (int i = 0; i < numCombos * 2; i += 2)
            {
                var combo = new CellCombo
                                {
                                    Cell1 = data.SpecialCells[input.ReadInt32()],
                                    Cell2 = data.SpecialCells[input.ReadInt32()]
                                };
                data.CellCombos.Add(combo);
            }

            return data;
        }
    }
}
