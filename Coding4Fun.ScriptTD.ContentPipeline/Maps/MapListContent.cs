using System.Collections.Generic;

namespace Coding4Fun.ScriptTD.ContentPipeline.Maps
{
    public class MapListContent
    {
        public readonly List<MapListingContent> RawData = new List<MapListingContent>();
        public readonly Dictionary<string, MapListingContent> Maps = new Dictionary<string, MapListingContent>();
    }
}
