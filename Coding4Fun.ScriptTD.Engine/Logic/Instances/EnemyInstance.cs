using Coding4Fun.ScriptTD.Common;
using Coding4Fun.ScriptTD.Engine.Data;

using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.Engine.Logic.Instances
{
    public class EnemyInstance
    {
        public EnemyData Data;
        public string ResumeEnemyId;

        public float Health;

        public Vector2 Position;
        public Vector2 CurrentDirection;

        public int StartX, StartY;
        public int DestX, DestY;

        public Vector2 DespawnPoint;
        public Vector2 StartPoint;

        public bool Escaped;

        public float ModifiedTotalHealth;

        public bool ShowHealth { get { return _healthTimeout < HealthMaxTimeout; } }
        private float _healthTimeout = HealthMaxTimeout;
        private const float HealthMaxTimeout = 4000;

        public enum PathStage : short
        {
            GoingToStart,
            GoingToDest,
            GoingToDespawn
        }
        public PathStage Stage = PathStage.GoingToStart;

        public Path Path;

        public void ResetToData(float healthModifier)
        {
            Health = Data.TotalHealth * healthModifier;
            Position = Vector2.Zero;
            Stage = PathStage.GoingToStart;
            Escaped = false;
            ModifiedTotalHealth = Health;
        }

        public void Update(float elapsedSeconds, GameSession session)
        {
            if (ShowHealth)
                _healthTimeout += elapsedSeconds * 1000;

            Vector2 targetPoint = Vector2.Zero;

            switch (Stage)
            {
                case PathStage.GoingToStart:
                    targetPoint = StartPoint;
                    break;

                case PathStage.GoingToDest:
                    targetPoint = (Path.PathFound && Path.Nodes.Count > 0) ? session.Grid.GetCellCenter(ref Path.Nodes[0].Cell) : Position;
                    break;

                case PathStage.GoingToDespawn:
                    targetPoint = DespawnPoint;
                    break;
            }

            var dir = targetPoint - Position;

            if (dir != Vector2.Zero)
                dir.Normalize();

            CurrentDirection = dir;
            dir *= (Data.TotalSpeed * session.Grid.CellSize) * elapsedSeconds;
            Position += dir;

            if ((targetPoint - Position).LengthSquared() <= (session.Grid.CellSize * session.Grid.CellSize))
            {
                switch (Stage)
                {
                    case PathStage.GoingToStart:
                        Stage = PathStage.GoingToDest;
                        Path = session.Paths.GetPath(session.Grid[StartX, StartY], session.Grid[DestX, DestY],
                                                     Data.CanFly);
                        break;

                    case PathStage.GoingToDest:
                        // move along the path until at dest
                        if (Path.PathFound && Path.Nodes.Count > 0)
                        {
                            if (Path.Nodes.Count == 1)
                            {
                                Stage = PathStage.GoingToDespawn;
                                session.Paths.ReleasePath(Path);
                            }
                            else
                            {
                                Path.CompleteNode(Path.Nodes[0]);
                            }
                        }
                        break;

                    case PathStage.GoingToDespawn:
                        Health = 0;
                        Escaped = true;
                        break;
                }
            }
        }

        public void TakeDamage(string towerFullName, float damage)
        {
            if (Data.Resistances.ContainsKey(towerFullName))
            {
                var res = Data.Resistances[towerFullName];

                if (res.Type == ResistanceData.ResistanceType.Damage)
                    damage *= res.Multiplier;
            }

            Health -= damage;
            _healthTimeout = 0;

            if (Health <= 0)
                Audio.PlaySfx(Data.DeathSoundId);
        }
    }
}
