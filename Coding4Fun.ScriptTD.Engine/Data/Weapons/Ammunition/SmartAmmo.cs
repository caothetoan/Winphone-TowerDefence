using System;
using Coding4Fun.ScriptTD.Engine.Data.Abstracts;
using Coding4Fun.ScriptTD.Engine.Logic;
using Coding4Fun.ScriptTD.Engine.Logic.Instances;
using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.Engine.Data.Weapons.Ammunition
{
    internal class SmartAmmo : Ammo, IAmmo
    {
        public EnemyInstance Target;

        public void Reset()
        {
            if (HitAnimation != null)
            {
                HitAnimation.Stop();
                HitAnimation.Reset();
            }

            IsAlive = true;
            IsHit = false;
            DistanceTravelled = 0;
        }

        public void Update(float elapsedSeconds, TowerData data, ref GameSession session)
        {
            if (IsHit)
            {
                if (HitAnimation != null && HitAnimation.IsPlaying)
                {
                    HitAnimation.Update(elapsedSeconds);
                }
                else
                {
                    IsAlive = false;
                }
            }
            else
            {
                var v = (Target.Position - Position);
                v.Normalize();
                Rotation = (float)Math.Atan2(v.Y, v.X) + MathHelper.PiOver2;
                v *= elapsedSeconds * Speed;
                Position += v;
                DistanceTravelled += v.Length();

                if (DistanceTravelled >= (data.MaxRange * session.Grid.CellSize))
                {
                    IsAlive = false;
                }
                else
                {
                    float halfCell = session.Grid.CellSize / 2f;
                    halfCell *= halfCell;

                    if ((Target.Position - Position).LengthSquared() <= halfCell)
                    {
                        Target.TakeDamage(data.FullName, data.DPS);
                        Audio.PlaySfx(data.HitSoundId);

                        if (HitAnimation != null)
                        {
                            IsHit = true;
                            HitAnimation.Play(data.AnimationFps);
                        }
                        else
                        {
                            IsAlive = false;
                        }
                    }
                }
            }
        }
    }
}
