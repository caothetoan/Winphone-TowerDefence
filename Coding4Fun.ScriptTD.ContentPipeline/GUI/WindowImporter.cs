using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Coding4Fun.ScriptTD.ContentPipeline.Helpers;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Coding4Fun.ScriptTD.ContentPipeline.GUI
{
    [ContentImporter(".xml", DisplayName = "Coding4Fun - Window Importer", DefaultProcessor = "WindowProcessor")]
	public class WindowImporter : ContentImporter<WindowContent>
    {
		public override WindowContent Import(string filename, ContentImporterContext context)
        {
            var content = new WindowContent();
            var doc = XDocument.Load(filename);
            var windowNode = doc.Element("Window");

            content.Attributes = LoadAttributes(windowNode);

			if (windowNode != null)
			{
				var vis = windowNode.Element("Visual");

				if (vis != null)
					content.Visuals = LoadVisuals(vis);
			}

        	if (windowNode != null)
        	{
        		XElement template;

        		if ((template = windowNode.Element("Templates")) != null)
        			content.Templates = LoadTemplates(template);

        		content.Controls = (from c in windowNode.Elements()
        		                    where !c.Name.LocalName.Equals("Visual") && !c.Name.LocalName.Equals("Templates")
        		                    select new ControlContent
        		                           	{
        		                           		Type = c.Name.LocalName.ToUpper(),
        		                           		Attributes = LoadAttributes(c),
        		                           		Template = c.Attribute("Template", "").ToUpper(),
        		                           		Visuals = LoadVisuals(c.Element("Visual"))
        		                           	}).ToList();
        	}

        	if (content == default(WindowContent))
                throw new NullReferenceException("Failed to load in the Window specification XML.");

            return content;
        }

        private Dictionary<string, TemplateContent> LoadTemplates(XElement templates)
        {
            return (from t in templates.Elements("Template")
                    select new TemplateContent
                    {
                        Name = t.Attributes().Where(x => x.Name.LocalName.ToLower().Equals("name")).Select(x => x.Value).Single().ToUpper(),
                        Visuals = LoadVisuals(t)
                    }).ToDictionary(t => t.Name);
        }

        public Dictionary<string, string> LoadAttributes(XElement element)
        {
            return (from a in element.Attributes()
                    select new Tuple<string, string>(a.Name.LocalName.ToUpper(), a.Value)).ToDictionary(x => x.Item1, x => x.Item2);
        }

        public List<VisualContent> LoadVisuals(XElement visualElement)
        {
        	if (visualElement != null)
            {
                return (from v in visualElement.Elements()
                        select new VisualContent
                                   {
                                       Attributes = LoadAttributes(v),
                                       Type = v.Name.LocalName.ToUpper()
                                   }).ToList();
            }
        	
			return new List<VisualContent>();
        }
    }
}
