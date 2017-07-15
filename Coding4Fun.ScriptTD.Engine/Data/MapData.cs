using System.Collections.Generic;
using Coding4Fun.ScriptTD.Common.Abstracts;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Data
{
    public class MapData : Map
    {
        public Texture2D Texture;

        public readonly List<WaveData> Waves = new List<WaveData>();
        public readonly List<TowerData> AvailableTowers = new List<TowerData>();
    }
}
