using System.Collections.Generic;
using System.Linq;
using Coding4Fun.ScriptTD.Engine.Data.Abstracts;
using Coding4Fun.ScriptTD.Engine.Data.Weapons.Ammunition;
using Coding4Fun.ScriptTD.Engine.Logic;
using Coding4Fun.ScriptTD.Engine.Logic.Instances;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Data.Weapons
{
    public class SmartProjectile : IWeapon
    {
        public Texture2D Texture { get; set; }

        public TowerData TowerData { get; set; }

        private float _sinceLastShot;

        private readonly List<SmartAmmo> _projectiles = new List<SmartAmmo>();
        private readonly Queue<SmartAmmo> _pool = new Queue<SmartAmmo>();
        private readonly List<SmartAmmo> _toRemove = new List<SmartAmmo>();

        public bool CanFire()
        {
            return _sinceLastShot >= TowerData.ReloadTime;
        }

        public bool TargetAndFire(ref List<EnemyInstance> enemies, ref Vector2 towerPos, float gridCellSize)
        {
            EnemyInstance closest = null;
            float distSq = float.MaxValue;
            float maxRange = gridCellSize * TowerData.MaxRange;
            
            float minRange = gridCellSize * TowerData.MinRange;

			maxRange *= maxRange;
			minRange *= minRange;
    
			foreach (var enemyInstance in enemies)
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
            	var item = _pool.Count > 0 ? _pool.Dequeue() : new SmartAmmo();
                item.Reset();
                item.Position = towerPos;
                item.Speed = TowerData.ShotSpeed * gridCellSize;
                item.Target = closest;

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

            foreach (var item in _projectiles)
            {
            	item.Update(elapsedSeconds, TowerData, ref session);
            	
				if (!item.IsAlive)
            	{
            		_pool.Enqueue(item);
            		_toRemove.Add(item);
            	}
            }

            foreach (var item in _toRemove)
            {
            	_projectiles.Remove(item);
            }

        	_toRemove.Clear();
        }

        public void Draw(GraphicsDevice device, SpriteBatch sb)
        {
        	var origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

        	foreach (var item in _projectiles.Where(p => p.IsAlive))
        	{
        		if (item.IsHit && item.HitAnimation != null)
        		{
        			item.HitAnimation.Draw(sb, item.Position, 0);
        		}
        		else
        		{
        			sb.Draw(Texture, item.Position, null, Color.White, item.Rotation, origin, Vector2.One, SpriteEffects.None, 0);
        		}
        	}
        }
    }
}
