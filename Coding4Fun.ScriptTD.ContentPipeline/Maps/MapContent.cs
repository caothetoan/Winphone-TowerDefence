using System;
using System.Collections.Generic;
using Coding4Fun.ScriptTD.Common.Abstracts;
using Coding4Fun.ScriptTD.ContentPipeline.Maps.Waves;
using Coding4Fun.ScriptTD.ContentPipeline.Towers;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Coding4Fun.ScriptTD.ContentPipeline.Maps
{
    public class MapContent : Map
    {
        public List<WaveContent> Waves;
        public List<TowerContent> AvailableTowers;

        [ContentSerializerIgnore]
        public string TexturePath;

        [ContentSerializerIgnore]
        public List<Tuple<string, int>> TowerImport;

        public Texture2DContent Texture;
    }
}
