using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Coding4Fun.ScriptTD.ContentPipeline.Helpers;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Coding4Fun.ScriptTD.ContentPipeline.Towers
{
    [ContentImporter(".xml", DisplayName = "Coding4Fun - Tower Data Importer", DefaultProcessor = "TowerProcessor")]
    public class TowerImporter : ContentImporter<List<TowerContent>>
    {
        public override List<TowerContent> Import(string filename, ContentImporterContext context)
        {
            XDocument xdoc = XDocument.Load(filename);

            var tower = xdoc.Element("Tower");

            string id = tower.Attribute("Id").Value;
            string weapontype = tower.Attribute("WeaponType").Value;

            int level = 1;

            var levels = from l in tower.Elements("Level")
                         select new TowerContent
                                    {
                                        TowerId = id,
                                        TowerLevel = level++,
                                        WeaponName = weapontype,
                                        Cost = l.Attribute("Cost", 0),
                                        ReloadTime = l.Attribute("ReloadTimeMs", 0f),
                                        BuildTime = l.Attribute("TimeToBuildMs", 0f),
                                        SellTime = l.Attribute("TimeToSellMs", 0f),
                                        CanShootFlyers = l.Attribute("CanShootFlying", false),
                                        CanShootLand = l.Attribute("CanShootLand", false),
                                        MinRange = l.Attribute("MinRange", 0f),
                                        MaxRange = l.Attribute("MaxRange", 1f),
                                        DPS = l.Attribute("Damage", 0f),
                                        ShotSpeed = l.Attribute("ShotSpeed", 1f),
                                        TexturePath = l.Attribute("Texture").Value,
                                        WeaponTexturePath = l.Attribute("ShotTexture").Value,
                                        ShotSoundId = l.Attribute("ShotSoundId", ""),
                                        BuildSoundId = l.Attribute("BuildSoundId", ""),
                                        SellSoundId = l.Attribute("SellSoundId", ""),
                                        UpgradeSoundId = l.Attribute("UpgradeSoundId", ""),
                                        HitSoundId = l.Attribute("HitSoundId", ""),
                                        HitTexturePath = l.Attribute("HitTexture", ""),
                                        AnimationFps = l.Attribute("HitAnimationFPS", 1)
                                    };

            return levels.ToList();
        }
    }
}
