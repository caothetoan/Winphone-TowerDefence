using System;
using Coding4Fun.ScriptTD.Common.Abstracts;

using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Data
{
    public class TowerData : Tower
    {
        public Type WeaponType;

        public string FullName { get { return TowerId + TowerLevel; } }

        public Texture2D Texture;
        public Texture2D WeaponTexture;
        public Texture2D HitTexture;
    }
}
