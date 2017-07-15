using System.Collections.Generic;
using Coding4Fun.ScriptTD.Engine.Data.Abstracts;
using Coding4Fun.ScriptTD.Engine.Data.Weapons.Ammunition;
using Coding4Fun.ScriptTD.Engine.Logic;
using Coding4Fun.ScriptTD.Engine.Logic.Instances;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Data.Weapons
{
    public class RadialShockwave : IWeapon
    {
        public Texture2D Texture { get; set; }

        public TowerData TowerData { get; set; }

        private float _sinceLastShot;

        public bool CanFire()
        {
            return _sinceLastShot >= TowerData.ReloadTime;
        }

        private readonly List<WaveAmmo> _waves = new List<WaveAmmo>();
        private readonly Queue<WaveAmmo> _pool = new Queue<WaveAmmo>();
        private readonly List<WaveAmmo> _toRemove = new List<WaveAmmo>();

        public bool TargetAndFire(ref List<EnemyInstance> enemies, ref Vector2 towerPos, float gridCellSize)
        {
            float maxRange = TowerData.MaxRange * gridCellSize;
            float minRange = TowerData.MinRange * gridCellSize;

            float maxRSq = maxRange * maxRange;
            float minRSq = minRange * minRange;

            foreach (var enemyInstance in enemies)
            {
            	float dist = (enemyInstance.Position - towerPos).LengthSquared();

				var canTarget = ((TowerData.CanShootFlyers && enemyInstance.Data.CanFly) || (TowerData.CanShootLand && !enemyInstance.Data.CanFly));

				if (canTarget && dist <= maxRSq && dist >= minRSq)
            	{
            		WaveAmmo w = _pool.Count > 0 ? _pool.Dequeue() : new WaveAmmo();

            		w.Reset();
            		w.Epicenter = towerPos;
            		w.Speed = TowerData.ShotSpeed * gridCellSize;
            		w.MaxRadius = maxRange;
            		
            		_waves.Add(w);
            		_sinceLastShot = 0;
            		return true;
            	}
            }
            return false;
        }

        public void Update(float elapsedSeconds, ref GameSession session)
        {
            if (_sinceLastShot < TowerData.ReloadTime)
                _sinceLastShot += elapsedSeconds * 1000;

            foreach (var wave in _waves)
            {
            	wave.Update(elapsedSeconds, TowerData, ref session);

            	if (!wave.IsAlive)
            	{
            		_pool.Enqueue(wave);
            		_toRemove.Add(wave);
            	}
            }

            foreach (var wave in _toRemove)
            {
            	_waves.Remove(wave);
            }
        	_toRemove.Clear();
        }

        public void Draw(GraphicsDevice device, SpriteBatch sb)
        {
            Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            Vector2 scale = Vector2.One;

            foreach (var wave in _waves)
            {
            	float texScale = 1f / (Texture.Width / (wave.MaxRadius * 2f));
            	scale.X = texScale * (wave.CurrentRadius / wave.MaxRadius);
            	scale.Y = scale.X;
            	sb.Draw(Texture, wave.Epicenter, null, Color.White, 0, origin, scale, SpriteEffects.None, 0);
            }
        }
    }
}
