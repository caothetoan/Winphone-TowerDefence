using System.Collections.Generic;

namespace Coding4Fun.ScriptTD.ContentPipeline.GUI
{
    public class WindowContent
    {
        public Dictionary<string, TemplateContent> Templates;
        public List<VisualContent> Visuals;
        public List<ControlContent> Controls;
        public Dictionary<string, string> Attributes;
    }
}
