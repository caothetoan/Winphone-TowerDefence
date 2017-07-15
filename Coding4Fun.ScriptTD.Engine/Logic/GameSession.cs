using System;
using System.Collections.Generic;
using System.IO;
using Coding4Fun.ScriptTD.Engine.Data;
using Coding4Fun.ScriptTD.Engine.Logic.Instances;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Coding4Fun.ScriptTD.Engine.Logic
{
    public class GameSession
    {
        public MapListingData MapLoadInfo;
        public MapData Map;
        public bool Loaded { get; private set; }

        public float RemainingCash;
        public int RemainingLives;
        public float CurrentPoints;

        public WaveData CurrentWave;
        private int _waveIndex;
        public int WaveNumber { get { return _waveIndex + 1; } }

        public float TimeToWaveSpawn;

        public bool IsInProgress = false;

        private readonly Spawner _spawner = new Spawner();
        public List<EnemyInstance> Enemies = new List<EnemyInstance>();
        public List<TowerInstance> Towers = new List<TowerInstance>();
        private readonly List<EnemyInstance> _toDelete = new List<EnemyInstance>();
        private readonly List<TowerInstance> _towerDelete = new List<TowerInstance>();

        public Grid Grid = new Grid(19, 11);

        public readonly PathPlanner Paths = new PathPlanner();

        private bool _resumed, _firstUpdate;

        // Action<This, DidWeWin>
        public event Action<GameSession, bool> GameComplete;

        public void LoadMapData(ContentManager content)
        {
            if (MapLoadInfo != null)
            {
                Map = content.Load<MapData>(MapLoadInfo.DataFilePath);
                Grid.ApplyMap(ref Map);
                Paths.InitNodes(ref Grid);
                Loaded = true;

                foreach (var tower in Map.AvailableTowers)
                {
                    Audio.RegisterSfx(tower.BuildSoundId);
                    Audio.RegisterSfx(tower.SellSoundId);
                    Audio.RegisterSfx(tower.ShotSoundId);
                    Audio.RegisterSfx(tower.UpgradeSoundId);
                    Audio.RegisterSfx(tower.HitSoundId);
                }

                foreach (var wave in Map.Waves)
                {
                    foreach (var part in wave.WaveParts)
                    {
                        var enemyData = part.Enemy;
                        Audio.RegisterSfx(enemyData.DeathSoundId);
                        Audio.RegisterSfx(enemyData.SpawnSoundId);
                    }
                }

                Audio.RegisterSfx(Map.VictorySoundId);
                Audio.RegisterSfx(Map.DefeatSoundId);
                Audio.RegisterSfx(Map.WaveStartSoundId);
                Audio.RegisterSfx(Map.MapStartSoundId);

                if (_resumed)
                {
                    CurrentWave = Map.Waves[_waveIndex];
                    _spawner.ResumeLoad(ref CurrentWave);

                    foreach (var towerInstance in Towers)
                    {
                        foreach (var tower in Map.AvailableTowers)
                        {
                            if (tower.FullName.Equals(towerInstance.TowerLoadId))
                            {
                                towerInstance.Data = tower;
                                break;
                            }
                        }

                        towerInstance.Initialize();

                        int x, y;
                        Grid.GetCell(towerInstance.Position, out x, out y);
                        Grid[x, y].Tower = towerInstance;
                    }

                    var d = new Dictionary<string, EnemyData>();
                    for (int i = 0; i < Map.Waves.Count; i++)
                    {
                        for (int j = 0; j < Map.Waves[i].WaveParts.Count; j++)
                        {
                            if (!d.ContainsKey(Map.Waves[i].WaveParts[j].Enemy.Id))
                                d.Add(Map.Waves[i].WaveParts[j].Enemy.Id, Map.Waves[i].WaveParts[j].Enemy);
                        }
                    }

                    for (int i = 0; i < Enemies.Count; i++)
                    {
                        var e = Enemies[i];
                        e.Data = d[e.ResumeEnemyId];
                        if (e.Stage == EnemyInstance.PathStage.GoingToDest)
                        {
                            int x, y;
                            Grid.GetCell(e.Position, out x, out y);
                            e.Path = Paths.GetPath(Grid[x, y], Grid[e.DestX, e.DestY], e.Data.CanFly);
                        }
                    }
                }
            }
        }

        public void InitializeNewMap()
        {
            if (!Loaded)
                throw new NullReferenceException();

            RemainingCash = Map.StartingCash;
            RemainingLives = Map.StartingLives;
            CurrentPoints = 0;
            _waveIndex = 0;
            CurrentWave = Map.Waves[_waveIndex];
            TimeToWaveSpawn = CurrentWave.WarmupTime;
            IsInProgress = true;
            _firstUpdate = true;
        }

        public TowerInstance BuildTower(string towerName, int level, int cellX, int cellY, bool instaBuild)
        {
            if (!Grid.IsCellValid(cellX, cellY))
                return null;

            TowerData data = null;
            for (int i = 0; i < Map.AvailableTowers.Count; i++)
            {
                if (Map.AvailableTowers[i].TowerLevel == level && Map.AvailableTowers[i].TowerId.Equals(towerName))
                {
                    data = Map.AvailableTowers[i];
                    break;
                }
            }

            if (data != null && data.Cost <= RemainingCash)
            {
                TowerInstance t = new TowerInstance();
                t.Data = data;
                t.Position = Grid.GetCellCenter(cellX, cellY);
                t.Initialize();
                t.BuildProgress = instaBuild ? 1 : 0;
                Towers.Add(t);
                RemainingCash -= data.Cost;
                Grid[cellX, cellY].Tower = t;
                if (!instaBuild)
                    Audio.PlaySfx(t.Data.BuildSoundId);
                return t;
            }
            return null;
        }

        public bool UpgradeTower(TowerInstance instance, bool instaBuild)
        {
            if (instance == null)
                return false;

            int nxtLvl = instance.Data.TowerLevel + 1;
            for (int i = 0; i < Map.AvailableTowers.Count; i++)
            {
                if (Map.AvailableTowers[i].TowerLevel == nxtLvl && Map.AvailableTowers[i].TowerId.Equals(instance.Data.TowerId))
                {
                    if (Map.AvailableTowers[i].Cost <= RemainingCash)
                    {
                        instance.Data = Map.AvailableTowers[i];
                        instance.Initialize();
                        instance.BuildProgress = instaBuild ? 1 : 0;
                        RemainingCash -= instance.Data.Cost;
                        if (!instaBuild)
                            Audio.PlaySfx(instance.Data.UpgradeSoundId);
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        public bool SellTower(TowerInstance instance)
        {
            if (instance != null)
            {
                int x, y;
                Grid.GetCell(instance.Position, out x, out y);
                Grid[x, y].Tower.Sell();
                RemainingCash += instance.TotalCost;
                Audio.PlaySfx(instance.Data.SellSoundId);
                return true;
            }
            return false;
        }

        public bool CanUpgrade(string towerId, int upgradeLevel, out float cost)
        {
            cost = 0;
            if (!String.IsNullOrEmpty(towerId) && upgradeLevel > 1)
            {
                for (int i = 0; i < Map.AvailableTowers.Count; i++)
                {
                    if (Map.AvailableTowers[i].FullName.Equals(towerId + upgradeLevel))
                    {
                        cost = Map.AvailableTowers[i].Cost;
                        return true;
                    }
                }
            }
            return false;
        }

        public void ForceWaveStart()
        {
            // Hack: Use a tiny value to still enter the correct path in the next update and "Start" before spawning
            TimeToWaveSpawn = 0.001f;
        }

        public void Update(float elapsedSeconds)
        {
            if (!IsInProgress)
                return;

            if (_firstUpdate)
            {
                Audio.PlaySfx(Map.MapStartSoundId);
                _firstUpdate = false;
            }

            // Count down until wave spawn, or process the wave if the timeout has been exceeded
            if (TimeToWaveSpawn > 0)
            {
                TimeToWaveSpawn -= elapsedSeconds;
                if (TimeToWaveSpawn <= 0)
                {
                    _spawner.Start(ref CurrentWave, this);
                    Audio.PlaySfx(Map.WaveStartSoundId);
                }
            }
            else
            {
                // Spawning
                _spawner.Update(elapsedSeconds, ref Enemies);
                // Pathfinding
                Paths.UpdatePaths();
                // Enemy logic & cleanup
                for (int i = 0; i < Enemies.Count; i++)
                {
                    var enemy = Enemies[i];
                    enemy.Update(elapsedSeconds, this);
                    if (enemy.Health <= 0)
                    {
                        if (enemy.Escaped)
                        {
                            RemainingLives--;
                        }
                        else
                        {
                            RemainingCash += enemy.Data.TotalWorth * CurrentWave.WorthModifier;
                            CurrentPoints += enemy.Data.TotalHealth;
                        }
                        _spawner.Recycle(ref enemy);
                        _toDelete.Add(enemy);
                    }
                }
                for (int i = 0; i < _toDelete.Count; i++)
                {
                    int x, y;
                    Grid.GetCell(_toDelete[i].Position, out x, out y);
                    Grid[x, y].Tower = null;
                    Enemies.Remove(_toDelete[i]);
                }
                _toDelete.Clear();

                if (!_spawner.IsSpawning && Enemies.Count == 0)
                {
                    _waveIndex++;
                    if (_waveIndex < Map.Waves.Count)
                    {
                        // Move to next wave
                        CurrentWave = Map.Waves[_waveIndex];
                        TimeToWaveSpawn = CurrentWave.WarmupTime;
                    }
                    else
                    {
                        // Victory
                        IsInProgress = false;
                        Audio.KillAllSfx();
                        Audio.PlaySfx(Map.VictorySoundId);
                        if (GameComplete != null)
                            GameComplete(this, true);
                    }
                }
            }

            // Tower logic
            for (int i = 0; i < Towers.Count; i++)
            {
                Towers[i].Update(elapsedSeconds, this);
                if (Towers[i].IsSelling && Towers[i].BuildProgress <= 0)
                {
                    _towerDelete.Add(Towers[i]);
                    int x, y;
                    Grid.GetCell(Towers[i].Position, out x, out y);
                    Grid[x, y].Tower = null;
                }
            }
            for (int i = 0; i < _towerDelete.Count; i++)
                Towers.Remove(_towerDelete[i]);
            _towerDelete.Clear();

            if (RemainingLives <= 0)
            {
                // Defeat
                IsInProgress = false;
                Audio.KillAllSfx();
                Audio.PlaySfx(Map.DefeatSoundId);
                if (GameComplete != null)
                    GameComplete(this, false);
            }
        }

        public void SaveState(BinaryWriter bw)
        {
            bw.Write("v2");
            bw.Write(MapLoadInfo.Id);
            bw.Write(MapLoadInfo.FriendlyName);
            bw.Write(MapLoadInfo.DataFilePath);

            bw.Write(RemainingCash);
            bw.Write(RemainingLives);
            bw.Write(CurrentPoints);

            bw.Write(_waveIndex);
            bw.Write(TimeToWaveSpawn);

            _spawner.Save(bw);

            bw.Write(Enemies.Count);
            for (int i = 0; i < Enemies.Count; i++)
            {
                var e = Enemies[i];
                bw.Write(e.Data.Id);
                bw.Write(e.Health);
                bw.Write(e.ModifiedTotalHealth);
                bw.Write(e.Position.X);
                bw.Write(e.Position.Y);
                bw.Write(e.StartX);
                bw.Write(e.StartY);
                bw.Write(e.DestX);
                bw.Write(e.DestY);
                bw.Write(e.DespawnPoint.X);
                bw.Write(e.DespawnPoint.Y);
                bw.Write(e.StartPoint.X);
                bw.Write(e.StartPoint.Y);
                bw.Write(e.Escaped);
                bw.Write((short)e.Stage);
            }

            bw.Write(Towers.Count);
            for (int i = 0; i < Towers.Count; i++)
            {
                bw.Write(Towers[i].Position.X);
                bw.Write(Towers[i].Position.Y);
                bw.Write(Towers[i].BuildTime);
                bw.Write(Towers[i].Data.TowerId + Towers[i].Data.TowerLevel);
            }
        }

        public void LoadState(BinaryReader br)
        {
            var ver = br.ReadString();
            if (!ver.Equals("v2"))
                br.BaseStream.Seek(0, SeekOrigin.Begin);
            MapLoadInfo = new MapListingData();
            MapLoadInfo.Id = br.ReadString();
            MapLoadInfo.FriendlyName = br.ReadString();
            MapLoadInfo.DataFilePath = br.ReadString();

            RemainingCash = br.ReadSingle();
            RemainingLives = br.ReadInt32();
            CurrentPoints = br.ReadSingle();

            _waveIndex = br.ReadInt32();
            TimeToWaveSpawn = br.ReadSingle();

            _spawner.Load(br);

            int numEnemies = br.ReadInt32();
            for (int i = 0; i < numEnemies; i++)
            {
                EnemyInstance e = new EnemyInstance();
                e.ResumeEnemyId = br.ReadString();
                e.Health = br.ReadSingle();
                if (ver.Equals("v2"))
                    e.ModifiedTotalHealth = br.ReadSingle();
                e.Position = new Vector2(br.ReadSingle(), br.ReadSingle());
                e.StartX = br.ReadInt32();
                e.StartY = br.ReadInt32();
                e.DestX = br.ReadInt32();
                e.DestY = br.ReadInt32();
                e.DespawnPoint = new Vector2(br.ReadSingle(), br.ReadSingle());
                e.StartPoint = new Vector2(br.ReadSingle(), br.ReadSingle());
                e.Escaped = br.ReadBoolean();
                e.Stage = (EnemyInstance.PathStage)br.ReadInt16();
                Enemies.Add(e);
            }

            int numTowers = br.ReadInt32();
            for (int i = 0; i < numTowers; i++)
            {
                TowerInstance t = new TowerInstance();
                t.Position = new Vector2(br.ReadSingle(), br.ReadSingle());
                t.BuildTime = br.ReadSingle();
                t.TowerLoadId = br.ReadString();
                Towers.Add(t);
            }

            IsInProgress = true;
            _resumed = true;
            _firstUpdate = true;
        }
    }
}
