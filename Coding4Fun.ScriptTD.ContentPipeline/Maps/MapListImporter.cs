using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Coding4Fun.ScriptTD.ContentPipeline.Maps
{
    [ContentImporter(".xml", DisplayName = "Coding4Fun - Map List Importer", DefaultProcessor = "MapListProcessor")]
    public class MapListImporter : ContentImporter<MapListContent>
    {
        public override MapListContent Import(string filename, ContentImporterContext context)
        {
            XDocument xdoc = XDocument.Load(filename);

            MapListContent content = new MapListContent();

            var maps = from m in xdoc.Element("Maps").Elements("Map")
                       select new MapListingContent
                       {
                           DataFilePath = m.Attribute("Data").Value,
                           Prerequisites = LoadPrereqs(m)
                       };
            content.RawData.AddRange(maps.ToList());

            return content;
        }

        private static List<string> LoadPrereqs(XElement m)
        {
            List<string> list = new List<string>();
            try
            {
                list.AddRange(m.Elements("Prerequisite").Select(x => x.Value).ToList());
            }
            catch (NullReferenceException)
            {
            }
            return list;
        }
    }
}
