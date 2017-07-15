using System;
using Coding4Fun.ScriptTD.Engine.Data;
using Coding4Fun.ScriptTD.Engine.Data.Abstracts;
using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.Engine.Logic.Instances
{
    public class TowerInstance
    {
        public TowerData Data;

        public Vector2 Position;

        public IWeapon Weapon;

        public string TowerLoadId;

        public int TotalCost = 0;

        public float BuildTime;
        public float BuildProgress
        {
            get
            {
                if (IsSelling)
                {
                    return BuildTime >= Data.SellTime ? 1 : BuildTime / Data.SellTime;
                }
                else
                {
                    return BuildTime >= Data.BuildTime ? 1 : BuildTime / Data.BuildTime;
                }
            }
            set { BuildTime = value * Data.BuildTime; }
        }

        public bool IsSelling;

        public void Initialize()
        {
            Weapon = (IWeapon)Activator.CreateInstance(Data.WeaponType);
            Weapon.Texture = Data.WeaponTexture;
            Weapon.TowerData = Data;
            TotalCost += Data.Cost;
            IsSelling = false;
        }

        public void Sell()
        {
            IsSelling = true;
            BuildTime = Data.SellTime;
        }

        public void Update(float elapsedSeconds, GameSession session)
        {
            float elapsedMs = elapsedSeconds * 1000;

            if ((BuildTime < Data.BuildTime) && !IsSelling)
            {
                BuildTime += elapsedMs;
            }
            else
            {
                if (IsSelling)
                {
                    BuildTime -= elapsedMs;
                }
                else
                {
                    Weapon.Update(elapsedSeconds, ref session);

                    if (Weapon.CanFire())
                    {
                        if (Weapon.TargetAndFire(ref session.Enemies, ref Position, session.Grid.CellSize))
                            Audio.PlaySfx(Data.ShotSoundId);
                    }
                }
            }
        }
    }
}
