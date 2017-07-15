using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coding4Fun.ScriptTD.Engine.Data;
using Coding4Fun.ScriptTD.Engine.Logic.Instances;
using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.Engine.Logic
{
    public class Spawner
    {
        private class SpawnRun
        {
            public float HealthMod;
            public EnemyData Data;
            public string ResumeEnemyId;

            public float TimeToNext;
            public float TimeBetween;
            public int TotalToMake;
            public int NumCreated;

            public Vector2 SpawnPoint, DespawnPoint, StartPoint;
            public int StartX, StartY;
            public int DestX, DestY;
        }

        private readonly Queue<EnemyInstance> _free = new Queue<EnemyInstance>();

        public bool IsSpawning;

        private readonly List<SpawnRun> _runs = new List<SpawnRun>();

        public void Update(float elapsedSeconds, ref List<EnemyInstance> enemyList)
        {
            int numFinished = 0;

            foreach (SpawnRun run in _runs)
            {
                if (run.NumCreated < run.TotalToMake)
                {
                    run.TimeToNext -= elapsedSeconds;
                    if (run.TimeToNext <= 0)
                    {
                        var enemy = _free.Count > 0 ? _free.Dequeue() : new EnemyInstance();
                        enemy.Data = run.Data;
                        enemy.ResetToData(run.HealthMod);
                        enemy.Position = run.SpawnPoint;
                        enemy.DestX = run.DestX;
                        enemy.DestY = run.DestY;
                        enemy.StartPoint = run.StartPoint;
                        enemy.StartX = run.StartX;
                        enemy.StartY = run.StartY;
                        enemy.DespawnPoint = run.DespawnPoint;
                        enemyList.Add(enemy);

                        Audio.PlaySfx(enemy.Data.SpawnSoundId);

                        run.NumCreated++;
                        run.TimeToNext = run.TimeBetween;
                    }
                }
                else
                {
                    numFinished++;
                }
            }

            if (numFinished >= _runs.Count)
            {
                IsSpawning = false;
                _runs.Clear();
            }
        }

        public void Start(ref WaveData currentWave, GameSession session)
        {
            // build a list of spawnruns with timetonext set to the start offset
            foreach (var wp in currentWave.WaveParts)
            {
                var run = new SpawnRun
                                {
                                    Data = wp.Enemy,
                                    HealthMod = currentWave.HealthModifier,
                                    NumCreated = 0,
                                    TimeToNext = wp.StartTimeOffset,
                                    TotalToMake = wp.TotalEnemies,
                                    TimeBetween = wp.TimeBetweenSpawns
                                };

                var destCell = session.Grid.SpecialCells[wp.EndCell];
                run.DestX = destCell.X;
                run.DestY = destCell.Y;

                var startCell = session.Grid.SpecialCells[wp.StartCell];
                run.StartX = startCell.X;
                run.StartY = startCell.Y;
                run.StartPoint = session.Grid.GetCellCenter(run.StartX, run.StartY);

                if (wp.SpawnPoint == Vector2.Zero)
                {
                    run.SpawnPoint = run.StartPoint;
                    if (startCell.X == 0)
                        run.SpawnPoint.X = -session.Grid.CellSize;
                }
                else
                {
                    run.SpawnPoint = wp.SpawnPoint;
                }

                if (wp.DespawnPoint == Vector2.Zero)
                {
                    run.DespawnPoint = session.Grid.GetCellCenter(run.DestX, run.DestY);
                    if (run.DestX == session.Grid.Columns - 1)
                        run.DespawnPoint.X = 800 + session.Grid.CellSize; //Screen width + single cellsize
                }
                else
                {
                    run.DespawnPoint = wp.DespawnPoint;
                }

                _runs.Add(run);
            }
            IsSpawning = true;
        }

        public void ResumeLoad(ref WaveData currentWave)
        {
            var d = currentWave.WaveParts.Select(t => t.Enemy).ToDictionary(e => e.Id);

            foreach (var run in _runs)
            {
                run.Data = d[run.ResumeEnemyId];
            }
        }

        public void Recycle(ref EnemyInstance enemy)
        {
            _free.Enqueue(enemy);
        }

        public void Save(BinaryWriter bw)
        {
            bw.Write(IsSpawning);
            bw.Write(_runs.Count);

            foreach (var run in _runs)
            {
                bw.Write(run.HealthMod);
                bw.Write(run.Data.Id);
                bw.Write(run.TimeToNext);
                bw.Write(run.TimeBetween);
                bw.Write(run.TotalToMake);
                bw.Write(run.NumCreated);
                bw.Write(run.SpawnPoint.X);
                bw.Write(run.SpawnPoint.Y);
                bw.Write(run.DespawnPoint.X);
                bw.Write(run.DespawnPoint.Y);
                bw.Write(run.StartPoint.X);
                bw.Write(run.StartPoint.Y);
                bw.Write(run.StartX);
                bw.Write(run.StartY);
                bw.Write(run.DestX);
                bw.Write(run.DestY);
            }
        }

        public void Load(BinaryReader br)
        {
            IsSpawning = br.ReadBoolean();
            int numRuns = br.ReadInt32();

            for (int i = 0; i < numRuns; i++)
            {
                var run = new SpawnRun
                            {
                                HealthMod = br.ReadSingle(),
                                ResumeEnemyId = br.ReadString(),
                                TimeToNext = br.ReadSingle(),
                                TimeBetween = br.ReadSingle(),
                                TotalToMake = br.ReadInt32(),
                                NumCreated = br.ReadInt32(),
                                SpawnPoint = new Vector2(br.ReadSingle(), br.ReadSingle()),
                                DespawnPoint = new Vector2(br.ReadSingle(), br.ReadSingle()),
                                StartPoint = new Vector2(br.ReadSingle(), br.ReadSingle()),
                                StartX = br.ReadInt32(),
                                StartY = br.ReadInt32(),
                                DestX = br.ReadInt32(),
                                DestY = br.ReadInt32()
                            };

                _runs.Add(run);
            }
        }
    }
}
