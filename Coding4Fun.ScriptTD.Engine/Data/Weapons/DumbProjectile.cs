using System;
using System.Collections.Generic;
using Coding4Fun.ScriptTD.Engine.Data.Abstracts;
using Coding4Fun.ScriptTD.Engine.Data.Weapons.Ammunition;
using Coding4Fun.ScriptTD.Engine.Logic;
using Coding4Fun.ScriptTD.Engine.Logic.Instances;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Data.Weapons
{
    public class DumbProjectile : IWeapon
    {
        public Texture2D Texture { get; set; }

        public TowerData TowerData { get; set; }

        private float _sinceLastShot;

        public bool CanFire()
        {
            return _sinceLastShot >= TowerData.ReloadTime;
        }

        private readonly List<DumbAmmo> _projectiles = new List<DumbAmmo>();
        private readonly Queue<DumbAmmo> _pool = new Queue<DumbAmmo>();
        private readonly List<DumbAmmo> _toRemove = new List<DumbAmmo>();

        public bool TargetAndFire(ref List<EnemyInstance> enemies, ref Vector2 towerPos, float gridCellSize)
        {
            EnemyInstance closest = null;
            float distSq = float.MaxValue;

            float maxRange = gridCellSize * TowerData.MaxRange;
            maxRange *= maxRange;

            float minRange = gridCellSize * TowerData.MinRange;
            minRange *= minRange;

            foreach (EnemyInstance enemyInstance in enemies)
            {
            	if ((TowerData.CanShootFlyers && enemyInstance.Data.CanFly) || (TowerData.CanShootLand && !enemyInstance.Data.CanFly))
            	{
            		float d = (enemyInstance.Position - towerPos).LengthSquared();

            		if (d < distSq && d <= maxRange && d >= minRange)
            		{
            			closest = enemyInstance;
            			distSq = d;
            		}
            	}
            }

        	if (closest != null)
            {
            	var item = _pool.Count > 0 ? _pool.Dequeue() : new DumbAmmo();

                item.Reset();
                item.Position = towerPos;
                item.Speed = TowerData.ShotSpeed * gridCellSize;
                item.Direction = ((((closest.Data.TotalSpeed * gridCellSize) * closest.CurrentDirection) + closest.Position) -
                               item.Position) / item.Speed;
                item.Direction.Normalize();
                item.Rotation = MathHelper.WrapAngle((float)Math.Atan2(item.Direction.Y, item.Direction.X) + MathHelper.PiOver2);

                if (TowerData.HitTexture != null && item.HitAnimation == null)
                {
                    item.HitAnimation = new AnimatedSprite(new Point((int)(TowerData.HitTexture.Width / TowerData.AnimationFps), TowerData.HitTexture.Height), false);
                    item.HitAnimation.SetTexture(TowerData.HitTexture);
                }

                _projectiles.Add(item);
                _sinceLastShot = 0;

                return true;
            }
            return false;
        }

        public void Update(float elapsedSeconds, ref GameSession session)
        {
            _sinceLastShot += elapsedSeconds * 1000;

            foreach (var projectile in _projectiles)
            {
            	projectile.Update(elapsedSeconds, TowerData, ref session);

            	if (!projectile.IsAlive)
            	{
            		_pool.Enqueue(projectile);
            		_toRemove.Add(projectile);
            	}
            }

            foreach (var projectile in _toRemove)
            {
            	_projectiles.Remove(projectile);
            }

        	_toRemove.Clear();
        }

        public void Draw(GraphicsDevice device, SpriteBatch sb)
        {
        	var origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

        	foreach (var item in _projectiles)
        	{
        		if (item.IsAlive && item.IsHit && item.HitAnimation != null)
        		{
        			item.HitAnimation.Draw(sb, item.Position, 0);
        		}
        		else
        		{
        			sb.Draw(Texture, 
						item.Position, 
						null, 
						Color.White, 
						item.Rotation, 
						origin, 
						Vector2.One,
						SpriteEffects.None, 
						0);
        		}
        	}
        }
    }
}
