using Coding4Fun.ScriptTD.Engine.Data.Abstracts;
using Coding4Fun.ScriptTD.Engine.Logic;
using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.Engine.Data.Weapons.Ammunition
{
    internal class WaveAmmo : IAmmo
    {
        public Vector2 Epicenter;

        public float CurrentRadius;
        public float MaxRadius;
        public float Speed;

        public bool IsAlive;

        public void Reset()
        {
            CurrentRadius = 0;
            IsAlive = true;
        }

        public void Update(float elapsedSeconds, TowerData data, ref GameSession session)
        {
            CurrentRadius += Speed * elapsedSeconds;
            if (CurrentRadius >= MaxRadius)
                IsAlive = false;

            float rsq = CurrentRadius * CurrentRadius;

            foreach (var enemyInstance in session.Enemies)
            {
                if ((enemyInstance.Position - Epicenter).LengthSquared() <= rsq && ((enemyInstance.Data.CanFly && data.CanShootFlyers) || (!enemyInstance.Data.CanFly && data.CanShootLand)))
                {
                    enemyInstance.TakeDamage(data.FullName, data.DPS * elapsedSeconds);
                    Audio.PlaySfx(data.HitSoundId);
                }
            }
        }
    }
}
