using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Coding4Fun.ScriptTD.Common;
using Coding4Fun.ScriptTD.ContentPipeline.Helpers;
using Coding4Fun.ScriptTD.ContentPipeline.Maps.Waves;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Coding4Fun.ScriptTD.ContentPipeline.Maps
{
    [ContentImporter(".xml", DisplayName = "Coding4Fun - Map Data Importer", DefaultProcessor = "MapProcessor")]
    public class MapImporter : ContentImporter<MapContent>
    {
        public override MapContent Import(string filename, ContentImporterContext context)
        {
            XDocument xdoc = XDocument.Load(filename);

            var content = (from m in xdoc.Elements("Map")
                           select new MapContent
                           {
                               Id = m.Attribute("MapId").Value,
                               FriendlyName = m.Attribute("FriendlyName", m.Attribute("MapId").Value),
                               StartingCash = m.Attribute("StartingCash", 0f),
                               StartingLives = m.Attribute("NumLives", 1),
                               TexturePath = m.Attribute("BackgroundTexture").Value,
                               VictorySoundId = m.Attribute("VictorySoundId", ""),
                               DefeatSoundId = m.Attribute("DefeatSoundId", ""),
                               WaveStartSoundId = m.Attribute("WaveStartSoundId", ""),
                               MapStartSoundId = m.Attribute("MapStartSoundId", ""),
                               SpecialCells = LoadCells(m.Element("SpecialCells")),
                               TowerImport = LoadTowers(m.Element("AllowedTowers")),
                               Waves = LoadWaves(m.Element("Waves"))
                           }).Single();
            content.CellCombos = DetermineCombos(content.SpecialCells, content.Waves);

            return content;
        }

        private static List<CellCombo> DetermineCombos(List<SpecialCell> cells, List<WaveContent> waves)
        {
            var combos = new List<CellCombo>();
            var cache = new Dictionary<string, CellCombo>();

            foreach (var wave in waves)
            {
                foreach (var part in wave.WaveParts)
                {
                    if (cache.ContainsKey(part.StartCell + part.EndCell))
                        continue;

                    var cc = new CellCombo();
                    for (int i = 0; i < cells.Count; i++)
                    {
                        if (cells[i].Id.Equals(part.StartCell))
                        {
                            cc.Cell1 = cells[i];
                        }
                        else if (cells[i].Id.Equals(part.EndCell))
                        {
                            cc.Cell2 = cells[i];
                        }
                    }
                    combos.Add(cc);
                    cache.Add(cc.Cell1.Id + cc.Cell2.Id, cc);
                }
            }

            return combos;
        }

        private List<WaveContent> LoadWaves(XElement m)
        {
            return (from w in m.Elements("Wave")
                    select new WaveContent
                    {
                        HealthModifier = w.Attribute("HealthMod", 1f),
                        WorthModifier = w.Attribute("WorthMod", 1f),
                        WaveParts = LoadWaveParts(w)
                    }).ToList();
        }

        private static List<WavePartContent> LoadWaveParts(XElement w)
        {
            return (from e in w.Elements("Creep")
                    select new WavePartContent
                    {
                        StartCell = e.Attribute("Entrance").Value,
                        EndCell = e.Attribute("Exit").Value,
                        SpawnPoint = e.Attribute("SpawnPoint", Vector2.Zero),
                        DespawnPoint = e.Attribute("DespawnPoint", Vector2.Zero),
                        TotalEnemies = e.Attribute("Number", 1),
                        StartTimeOffset = e.Attribute("SpawnTimeOffset", 0f) / 1000f,
                        TimeBetweenSpawns = e.Attribute("TimeBetweenMs", 0f) / 1000f,
                        EnemyPath = Path.ChangeExtension(e.Attribute("CreepId").Value, ".xml")
                    }).ToList();
        }

        private static List<Tuple<string, int>> LoadTowers(XElement m)
        {
            return (from t in m.Elements("Tower")
                    select new Tuple<string, int>(Path.ChangeExtension(t.Attribute("Id").Value, ".xml"), t.Attribute("MaxLevel", 1))).ToList();
        }

        private static List<SpecialCell> LoadCells(XElement m)
        {
            return (from s in m.Elements("Cell")
                    select new SpecialCell
                    {
                        Id = s.Attribute("CellId", ""),
                        X = s.Attribute("X", 0),
                        Y = s.Attribute("Y", 0),
                        CellType = (SpecialCellType)Enum.Parse(typeof(SpecialCellType), s.Attribute("Type").Value),
                        Buildable = s.Attribute("Buildable", false)
                    }).ToList();
        }
    }
}
