using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Coding4Fun.ScriptTD.Common;
using Coding4Fun.ScriptTD.ContentPipeline.Helpers;

using Microsoft.Xna.Framework.Content.Pipeline;

namespace Coding4Fun.ScriptTD.ContentPipeline.Enemies
{
    [ContentImporter(".xml", DisplayName = "Coding4Fun - Enemy Data Importer", DefaultProcessor = "EnemyProcessor")]
    public class EnemyImporter : ContentImporter<EnemyContent>
    {
        public override EnemyContent Import(string filename, ContentImporterContext context)
        {
            var xdoc = XDocument.Load(filename);

            return (from e in xdoc.Elements("Creep")
                    select new EnemyContent
                    {
                        Id = e.Attribute("Id").Value,
                        CanFly = e.Attribute("CanFly", false),
                        TotalSpeed = e.Attribute("Speed", 1f),
                        TotalHealth = e.Attribute("Health", 1f),
                        TotalWorth = e.Attribute("Worth", 0f),
                        TexturePath = e.Attribute("Texture").Value,
                        DeathSoundId = e.Attribute("DeathSoundId", ""),
                        SpawnSoundId = e.Attribute("SpawnSoundId", ""),
                        Resistances = LoadResistances(e)
                    }).Single();
        }

        private Dictionary<string, ResistanceData> LoadResistances(XElement e)
        {
        	var data = from r in e.Elements("Resistance")
                       select new ResistanceData
                       {
                           TowerId = r.Attribute("TowerId").Value,
                           TowerLevel = r.Attribute("TowerLevel", 1),
                           Multiplier = r.Attribute("Multiplier", 1f),
                           Type = (ResistanceData.ResistanceType)Enum.Parse(typeof(ResistanceData.ResistanceType), r.Attribute("Type", "Damage"))
                       };

        	return data.ToDictionary(r => r.TowerId + r.TowerLevel);
        }
    }
}
