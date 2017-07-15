using System.Collections.Generic;
using Coding4Fun.ScriptTD.Engine.Logic;
using Coding4Fun.ScriptTD.Engine.Logic.Instances;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Data.Abstracts
{
    public interface IWeapon
    {
        Texture2D Texture { get; set; }
        TowerData TowerData { get; set; }

        bool CanFire();

        bool TargetAndFire(ref List<EnemyInstance> enemies, ref Vector2 towerPos, float gridCellSize);

        void Update(float elapsedSeconds, ref GameSession session);

        void Draw(GraphicsDevice device, SpriteBatch sb);
    }
}
