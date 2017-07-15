using System.Collections.Generic;
using Coding4Fun.ScriptTD.Engine.Data.Abstracts;
using Coding4Fun.ScriptTD.Engine.Data.Weapons.Ammunition;
using Coding4Fun.ScriptTD.Engine.Logic;
using Coding4Fun.ScriptTD.Engine.Logic.Instances;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.Data.Weapons
{
    public class Laser : IWeapon
    {
        public Texture2D Texture { get; set; }

        public TowerData TowerData { get; set; }

        private float _sinceLastShot;

        public bool CanFire()
        {
            return _sinceLastShot >= TowerData.ReloadTime && !_beamAmmo.IsAlive;
        }

        private readonly BeamAmmo _beamAmmo = new BeamAmmo();

        public bool TargetAndFire(ref List<EnemyInstance> enemies, ref Vector2 towerPos, float gridCellSize)
        {
            float maxRange = TowerData.MaxRange * gridCellSize;
            float maxRSq = maxRange * maxRange;
            float minRange = TowerData.MinRange * gridCellSize;
            float minRSq = minRange * minRange;

            int towerX = (int)(towerPos.X / gridCellSize);
            int towerY = (int)(towerPos.Y / gridCellSize);

            int direction = -1;

            foreach (var enemyInstance in enemies)
            {
            	int eX = (int)(enemyInstance.Position.X / gridCellSize);
            	int eY = (int)(enemyInstance.Position.Y / gridCellSize);

            	if (eX == towerX || eY == towerY)
            	{
            		float distSq = (enemyInstance.Position - towerPos).LengthSquared();
            		if (distSq <= maxRSq && distSq >= minRSq)
            		{
            			if (eX == towerX)
            			{
            				direction = eY < towerY ? 0 : 1;
            			}
            			else
            			{
            				direction = eX < towerX ? 3 : 2;
            			}
            		}
            	}
            }

            if (direction >= 0)
            {
                _beamAmmo.Reset();
            	_beamAmmo.StartPos = towerPos;
                _beamAmmo.EndPos = towerPos;
                _beamAmmo.Lifetime = TowerData.ShotSpeed; //  Speed = laser lifetime for laser weapons
                _beamAmmo.Column = towerX;
                _beamAmmo.Row = towerY;
                _beamAmmo.MaxRangeSq = maxRSq;

                switch (direction)
                {
                    // North
                    case 0:
                        _beamAmmo.EndPos.Y -= maxRange;
                        _beamAmmo.IsVertical = true;
                        break;

                    // South
                    case 1:
                        _beamAmmo.EndPos.Y += maxRange;
                        _beamAmmo.IsVertical = true;
                        break;

                    // East
                    case 2:
                        _beamAmmo.EndPos.X += maxRange;
                        _beamAmmo.IsVertical = false;
                        break;

                    // West
                    case 3:
                        _beamAmmo.EndPos.X -= maxRange;
                        _beamAmmo.IsVertical = false;
                        break;

                    default:
                        return false;
                }

                _beamAmmo.CellDrawIncrement = _beamAmmo.EndPos - _beamAmmo.StartPos;
                _beamAmmo.NumCells = (int)(_beamAmmo.CellDrawIncrement.Length() / gridCellSize);
                _beamAmmo.CellDrawIncrement.Normalize();
                _beamAmmo.CellDrawIncrement *= gridCellSize;

                return true;
            }

            return false;
        }

        public void Update(float elapsedSeconds, ref GameSession session)
        {
            if (_sinceLastShot < TowerData.ReloadTime)
                _sinceLastShot += elapsedSeconds * 1000;

            if (_beamAmmo.IsAlive)
            {
                _beamAmmo.AliveTime += elapsedSeconds;

                if (_beamAmmo.AliveTime >= _beamAmmo.Lifetime)
                {
                    _beamAmmo.IsAlive = false;
                    _sinceLastShot = 0;
                }

                foreach (var enemyInstance in session.Enemies)
                {
                	float distSq = (enemyInstance.Position - _beamAmmo.StartPos).LengthSquared();

                	if (_beamAmmo.IsVertical)
                	{
                		int eX = (int)(enemyInstance.Position.X / session.Grid.CellSize);

                		if (eX == _beamAmmo.Column && distSq <= _beamAmmo.MaxRangeSq)
                		{
                			enemyInstance.TakeDamage(TowerData.FullName, TowerData.DPS * elapsedSeconds);
                			Audio.PlaySfx(TowerData.HitSoundId);
                		}
                	}
                	else
                	{
                		int eY = (int)(enemyInstance.Position.Y / session.Grid.CellSize);

                		if (eY == _beamAmmo.Row && distSq <= _beamAmmo.MaxRangeSq)
                		{
                			enemyInstance.TakeDamage(TowerData.FullName, TowerData.DPS * elapsedSeconds);
                			Audio.PlaySfx(TowerData.HitSoundId);
                		}
                	}
                }
            }
        }

        public void Draw(GraphicsDevice device, SpriteBatch sb)
        {
            if (_beamAmmo.IsAlive)
            {
                Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

                for (int i = 1; i < _beamAmmo.NumCells; i++)
                {
                    sb.Draw(Texture, _beamAmmo.StartPos + (_beamAmmo.CellDrawIncrement * i), null, Color.White, _beamAmmo.IsVertical ? MathHelper.PiOver2 : 0, origin, Vector2.One, SpriteEffects.None, 0);
                }
            }
        }
    }
}
