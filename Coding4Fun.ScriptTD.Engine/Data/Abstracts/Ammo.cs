using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.Engine.Data.Abstracts
{
    public abstract class Ammo
    {
        public Vector2 Position;
        public Vector2 Direction;
        public bool IsAlive = true;
        public bool IsHit;
        public float Speed;
        public float Rotation;
        public AnimatedSprite HitAnimation;

        protected float DistanceTravelled;
    }
}
