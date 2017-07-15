using Coding4Fun.ScriptTD.Engine.Data.Abstracts;
using Coding4Fun.ScriptTD.Engine.Logic;

namespace Coding4Fun.ScriptTD.Engine.Data.Weapons.Ammunition
{
    internal class DumbAmmo : Ammo, IAmmo
    {
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
                var v = Direction * (elapsedSeconds * Speed);
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

                    foreach (var e in session.Enemies)
                    {
                        if ((e.Position - Position).LengthSquared() <= halfCell)
                        {
                            e.TakeDamage(data.FullName, data.DPS);
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
                            break;
                        }
                    }
                }
            }
        }
    }
}
